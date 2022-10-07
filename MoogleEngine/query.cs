namespace MoogleEngine
{
using System;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading.Tasks;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Linq;


    
    public class Query
    {
        public static string query;
        
        private List<string> operatedwords;
        
        public  static Dictionary<string,int> nonrepeated;
        
        public static Dictionary<string,float> TfsIdfs; 

        //Constructor
        public Query(string busqueda)
        {
            operatedwords= new List<string>();
            query=busqueda;
            var aux = OperatedQuery();
            nonrepeated =NonRepeatedWords();
            //comprobaciones para sacar palabras con operdores
            for(int i=0; i<aux.Length;i++)
            {
                if(aux[i][0] == '!')
                {
                    operatedwords.Add(aux[i]);
                    goto next;
                }
                if(aux[i][0] == '^')
                {
                    operatedwords.Add(aux[i]);
                    goto next;
                }
                if(aux[i].Contains('~'))
                {
                    operatedwords.Add(aux[i]);
                    goto next;
                }
                if(aux[i][0] == '*')
                {
                    operatedwords.Add(aux[i]);
                    goto next;
                }
                next:;


            }
            TfsIdfs=Tf_Idf();

        }

        //Depura el texto exceptuando los operadores
        public static string[] OperatedQuery()
        {
            char[] WeirdCharacters = new char[]{' ',',',';','>','<',']','[','#','$','%','&','"','(',')','+','-','.','/',':','=','?'}; 
            string[] subsequence = query.Split(WeirdCharacters , System.StringSplitOptions.RemoveEmptyEntries);
            string[] WithoutWCharacters= new string[subsequence.Length];
            for(int i=0;i<WithoutWCharacters.Length;i++)
            {
                WithoutWCharacters[i]= subsequence[i].ToLower();
            }
            return WithoutWCharacters;   
        }
        //limpia el texto completamente(sustituye las vocalescon tilde también)
        public static string[] CleanQuery()
        {
            char[] WeirdCharacters = new char[]{' ','~','*','!','^',',',';','>','<',']','[','#','$','%','&','"','(',')','+','-','.','/',':','=','?'}; 
            string[] subsequence = query.Split(WeirdCharacters , System.StringSplitOptions.RemoveEmptyEntries);
            string[] WithoutWCharacters= new string[subsequence.Length];
            for(int i=0;i<WithoutWCharacters.Length; i++)
            {
                WithoutWCharacters[i]= subsequence[i].ToLower().Replace('á','a')
                .Replace('é','e')
                .Replace('í','i')
                .Replace('ó','o')
                .Replace('ú','u');
            }
            return WithoutWCharacters;  
        }

        //Saca las palabras (sin repetir) y guarda las veces que se repiten 
        public static Dictionary<string,int> NonRepeatedWords()
        {
            var CleanQ = CleanQuery();
            var newdic= new Dictionary<string,int>();
            for(int i=0;i<CleanQ.Count();i++)
            {
                if(newdic.ContainsKey(CleanQ[i]))
                {
                    newdic[CleanQ[i]]++;
                }
                else
                {
                    newdic.Add(CleanQ[i],1);
                }
            }
            return newdic;
        }

        //Calcula Tf del query
        public static float[] Tf()
        {
            var CleanQ = nonrepeated;
            
            var tfs = new float[CleanQ.Count()];
            for(int i=0;i<tfs.Length;i++)
            {
                tfs[i] = (float)CleanQ.ElementAt(i).Value/CleanQ.Count();
            }
            return tfs;
        }


        //Calcula Idf del query
        static float[] Idf()
        {
            var words = nonrepeated;
            float archivos = dirsfiles.LimpText.Count();
            var idfs = new float[words.Count()];
            for(int i=0;i<words.Count();i++)
            {
                if(dirsfiles.Words.ContainsKey(words.ElementAt(i).Key))
                {
                   idfs[i]= (float)Math.Log10(archivos/(float)dirsfiles.Words[words.ElementAt(i).Key]); 
                }
                else 
                {
                    idfs[i]=0;
                }
                
            }
            return idfs;    
        }

        //Calcula Tf-Idf del query
        public static Dictionary<string,float> Tf_Idf()
        {
            var words = nonrepeated;
            var Tf_Idfs= new Dictionary<string,float>();
            for(int i=0;i<words.Count();i++)
            {
                Tf_Idfs.Add(words.ElementAt(i).Key, Tf()[i]*Idf()[i]);
                
                
            }
            return Tf_Idfs;
            
        }

        /*Calcula la similitud del query con los archivos del corpus
        por medio de la formula del coseno */
        public static float[] SimCos()
        {
            var DocsTfIdfs =dirsfiles.Tf_Idfs_documents;
            var CleanQ = TfsIdfs;
            var Simcos= new float[DocsTfIdfs.Length];
            for(int i=0;i<DocsTfIdfs.Length;i++)
            {
                float sqrdD=0;
                float sqrdQ=0;
                float sum=0;
                int j=0;
                while(j<CleanQ.Count()||j<DocsTfIdfs[i].Count())
                {
                    
                    if(j<CleanQ.Count())
                    {
                        sqrdQ +=(float)Math.Pow(CleanQ.ElementAt(j).Value,2);
                        if(DocsTfIdfs[i].ContainsKey(CleanQ.ElementAt(j).Key))
                        {
                            sum +=DocsTfIdfs[i][CleanQ.ElementAt(j).Key]*CleanQ.ElementAt(j).Value;
                        }
                    }
                    if(j<DocsTfIdfs[i].Count())
                    {
                        sqrdD +=(float)Math.Pow(DocsTfIdfs[i].ElementAt(j).Value,2);
                    }
                    j++;
                }
                
                Simcos[i]=sum/(float)Math.Sqrt(sqrdD)*(float)Math.Sqrt(sqrdQ);
                
            }
            return Simcos;
        }
        
        //Metodo del que me auxilio para ordenar y depurar los resultados de la busqueda
        static SearchItem[] Order(SearchItem[] CleanQ)
        {
            var x= CleanQ.ToList();
            var sorted = x.OrderByDescending(x =>x.Score).ToList();
            var finalsorted=sorted.Where(x => x.Score>0);
            return finalsorted.ToArray();  
        }

        /*Metodo que devuelve los resultados de la busqueda*/
        public SearchItem[] Search()
        {
            var Docs = dirsfiles.LimpText;
            var direcciones = Dirs.dirs;
            var Scores = SimCos();
            var Results = new SearchItem[Scores.Length];
            
            
            //COMPROBACION DE OPERADORES//
            for(int i=0; i<operatedwords.Count();i++)
            {
                //Palabra indeseada
                if(operatedwords[i][0] == '!')
                {
                    
                    for(int j=0;j<Docs.Count();j++)
                    {
                        if(Docs[j].Contains((operatedwords[i].Substring(1,operatedwords[i].Length-1))))
                        {
                            Scores[j]=0;
                        }
                    }
                    goto next;
                }
                //Plabra necesaria
                if(operatedwords[i][0] == '^')
                {
                    for(int j=0;j<Docs.Count();j++)
                    {
                        if(!Docs[j].Contains((operatedwords[i].Substring(1,operatedwords[i].Length-1))))
                        {
                            Scores[j]=0;
                        }
                    }
                    
                    goto next;
                }

                //operador de cercania
                if(operatedwords[i].Contains('~'))
                {
                    int pos=0;
                    for(int j=0;j<operatedwords[i].Length;j++)
                    {
                        if(operatedwords[i][j]=='~')
                        {
                            pos=j;
                        }
                    }

                    //Tomo las palabras analizar la distancia   
                    var p1=operatedwords[i].Substring(0,pos);
                    var p2=operatedwords[i].Substring(pos+1,operatedwords[i].Length-pos-1);

                    /*Establezco el Score adicional que sera repartidoc luego
                    proporcionalmente a la cercania*/
                    float additionalscore= (float)10000;
                    //Variable donde guardo la menor distancia
                    int MinorDistInArch=int.MaxValue;
                    
                    for (int j=0; j<Docs.Count();j++)
                    {
                        if(Docs[j].Contains(p1)&&Docs[j].Contains(p2))
                        {
                            for(int z=0;z<Docs[j].Count();z++)
                            {
                                if(Docs[j][z]==p1)
                                {
                                    for(int h=z+1;h<Docs[j].Count();h++)
                                    {
                                        if(Docs[j][h]==p2)
                                        {
                                            if(Math.Sqrt(Math.Pow(h-z,2))<MinorDistInArch)
                                            {
                                                MinorDistInArch=(int)Math.Sqrt(Math.Pow(h-z,2));
                                            }
                                        }
                                    }
                                    

                                }
                                if(Docs[j][z]==p2)
                                {
                                    for(int h=z+1;h<Docs[j].Count();h++)
                                    {
                                        if(Docs[j][h]==p1)
                                        {
                                            if(Math.Sqrt(Math.Pow(h-z,2))<MinorDistInArch)
                                            {
                                                MinorDistInArch=(int)Math.Sqrt(Math.Pow(h-z,2));
                                            }
                                        }
                                    }
                                }
                            }
                            Scores[j]+= additionalscore/MinorDistInArch;
                        }


                    }
                    
                    goto next;
                }
                
                
                //operdaor de importancia
                if(operatedwords[i][0] == '*')
                {
                    float AditionalScore = (float)0.250;
                    int cant=1;
                    for(int j=1;j<operatedwords[i].Length;j++)
                    {
                        if(operatedwords[i][j] =='*')
                        {
                            cant+=1;
                        }
                        else
                        {
                            goto seguir;
                        }
                        
                    }
                    seguir:
                   
                    if (cant == operatedwords[i].Length)
                    {
                        goto next;
                    }
                    else
                    {
                        for(int z =0; z<Docs.Count();z++)
                        {
                            if(Docs[z].Contains(operatedwords[i].Substring(cant)))
                            {
                                Scores[z]+= (float)cant*AditionalScore;
                            }
                        }
                    }
                }
                next:;


            }
            
            //snippet
            var aux=TfsIdfs.OrderByDescending(x=> x.Value);
            for(int i=0;i<Results.Length;i++)
            {
                string snippet = "";
                if(dirsfiles.Texts[i].Length<100)
                {
                    snippet =dirsfiles.Texts[i];   
                }
                else
                {
                    for(int j=0;j<aux.Count();j++)
                    {
                        if(dirsfiles.Texts[i].Contains(" "+aux.ElementAt(j).Key+" "))
                        {
                            string word= aux.ElementAt(j).Key;
                            int index =dirsfiles.Texts[i].IndexOf(" "+ word +" ");
                            if(index +150<dirsfiles.Texts[i].Length)
                            {
                                snippet=dirsfiles.Texts[i].Substring(index,100);
                                goto jump;
                            }
                            else
                            {
                                snippet=dirsfiles.Texts[i].Substring(index,dirsfiles.Texts[i].Length-index-1);
                                goto jump;
                            }
                        }
                        
                        else if(dirsfiles.Texts[i].Contains(" "+aux.ElementAt(j).Key+","))
                        {
                            string word= aux.ElementAt(j).Key;
                            int index =dirsfiles.Texts[i].IndexOf(" "+ word +",");
                            if(index +150<dirsfiles.Texts[i].Length)
                            {
                                snippet=dirsfiles.Texts[i].Substring(index,100);
                                goto jump;
                            }
                            else
                            {
                                snippet=dirsfiles.Texts[i].Substring(index,dirsfiles.Texts[i].Length-index-1);
                                goto jump;
                            }
                        }
                        else if(dirsfiles.Texts[i].Contains(" "+aux.ElementAt(j).Key+"."))
                        {
                            string word= aux.ElementAt(j).Key;
                            int index =dirsfiles.Texts[i].IndexOf(" "+ word +".");
                            if(index +150<dirsfiles.Texts[i].Length)
                            {
                                snippet=dirsfiles.Texts[i].Substring(index,100);
                                goto jump;
                            }
                            else
                            {
                                snippet=dirsfiles.Texts[i].Substring(index,dirsfiles.Texts[i].Length-index-1);
                                goto jump;
                            }
                        }
                        
                        else
                        {
                            snippet=dirsfiles.Texts[i].Substring(0,100);
                        }

                        
                    }

                    
                }
                jump:;
                var temp2= Path.GetFileName(direcciones[i]);
                Results[i] = new SearchItem(" ⚜️⚜️  " +temp2.Substring(0,temp2.Length-4)+ "  ⚜️⚜️", snippet,Scores[i]);
            }
            return Order(Results);
        }



        //Metodo para hacer sugerencias utilizando la distancia de Levenshtein
        public string suggestion()
        {
            string aux ="";
            var suggest = CleanQuery();
            float scores =float.MinValue;
            string answer= "";
            int control =0;
            int controlgen =0;
            for(int i=0;i<TfsIdfs.Count();i++)
            {
                /*solo relizo una sugerencia en caso de que exista alguna palabra
                cuyo Tf-Idf sea 0*/
                if(TfsIdfs.ElementAt(i).Value==0)
                {
                    /*Como lo que hare es modificar la query para sugerir, controlo si habra cambio o no en la query
                    para saber si en realidad no debo sugerir nada por medio de la variable control gen*/
                    controlgen=1;
                    //Aqui recorro por cada palabra de cada documento
                    for(int j=0;j<dirsfiles.Tf_Idfs_documents.Length;j++)
                    {
                        for(int z=0;z<dirsfiles.Tf_Idfs_documents[j].Count();z++)
                        {
                            /*Solamente sugiero las palabras que cuya distancia de Levenshtein es menor o igual
                            que 2*/
                            var distancia=Levenshtein(TfsIdfs.ElementAt(i).Key,dirsfiles.Tf_Idfs_documents[j].ElementAt(z).Key);
                            if(distancia<=2)
                            {
                                /*Escojo entre todas estas palabras que cumplen la condicion anterior la palabra
                                mas relevante(la que tiene mayor score)*/
                                if(dirsfiles.Tf_Idfs_documents[j].ElementAt(z).Value>scores)
                                {
                                    control +=1;
                                    scores=dirsfiles.Tf_Idfs_documents[j].ElementAt(z).Value;
                                    aux=dirsfiles.Tf_Idfs_documents[j].ElementAt(z).Key;
                                }
                            }
                        }
                    }
                    /*Si no se encuentra ninguna palabra cuya distancia sea menor igual que 2
                    no sugiero nada*/
                    if(!(control>0))
                    {
                        aux=""; 
                    }
                    control =0;
                    for(int j=0;j<suggest.Length;j++)
                    {
                        if(suggest[j]==TfsIdfs.ElementAt(i).Key)
                        {
                            suggest[j]=aux;
                        }
                    }
                    
                }
            }
            /*Se realiza sugerencia solo si se encuentra palabra con Score 0*/
            if(controlgen==1)
            {
                for(int z=0;z<suggest.Length;z++)
                {
                    answer+= " " + suggest[z];
                }
            }  
            return answer;
        } 


        //metodo de Levenshtein
        public static int Levenshtein(string s1,string s2)
        {
            int coste = 0;
            int n1 = s1.Length;
            int n2 = s2.Length;
            int[,] m = new int[n1 + 1, n2 + 1];

            for (int i = 0; i <= n1; i++)
            {
                m[i, 0] = i;
            }
            for (int i = 1; i <= n2; i++)
            {
                m[0, i] = i;
            }
            for (int i1 = 1; i1 <= n1; i1++)
            {
                for (int i2 = 1; i2 <= n2; i2++)
                {
                    coste = (s1[i1 - 1] == s2[i2 - 1]) ? 0 : 1;
                    m[i1, i2] = Math.Min(
                      Math.Min(
                        m[i1 - 1, i2] + 1,
                        m[i1, i2 - 1] + 1
                      ),
                      m[i1 - 1, i2 - 1] + coste
                    );
                }
            }
            return m[n1, n2];
        }      
    }
    

}