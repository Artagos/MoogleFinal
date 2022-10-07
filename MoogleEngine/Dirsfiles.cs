using System.Collections.Specialized;
namespace MoogleEngine;
using System.Net.Mime;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;

//Proceso el Corpus
static public class dirsfiles
    {
    static public string[] Texts;
    public static List<List<string>> LimpText;
    static public Dictionary<string, int>[] NonRepeated;
    static public Dictionary<string,int> Words;
    static public Dictionary <string,float>[] Tf_Idfs_documents;
    //constructor
    static dirsfiles()
    {
        Texts = new string[Dirs.dirs.Length];

        for(int i=0; i<Dirs.dirs.Length;i++)
        {
            Texts[i]= File.ReadAllText(Dirs.dirs[i]);
        }

        LimpText= TextLimpio();
        NonRepeated= NonReapeatedWords(); 
        Tf_Idfs_documents=Tf_Idfs();
    }

    

    //depuro el texto
    public static List<List<string>> TextLimpio()
    {
        var textolimpio = new List<List<string>>();
        char[] cosasraras = new char[]{' ',',',';','>','<',']','[','!','#','$','%','&','"','(',')','*','+','-','.','/',':','=','?','\t','\r','\n','\\'}; 
        for(int i=0;i<Texts.Length;i++)
        {
            string[] subsequence = Texts[i].ToLower().Replace('á','a')
            .Replace('é','e')
            .Replace('í','i')
            .Replace('ó','o')
            .Replace('ú','u')
            .Split(cosasraras , System.StringSplitOptions.RemoveEmptyEntries);

            string[] sincosasraras= new string[subsequence.Length];
            for(int j=0;j<sincosasraras.Length;j++)
            {
                sincosasraras[j]= subsequence[j];
            }
            textolimpio.Add(sincosasraras.ToList());
            
        }
        return textolimpio;
        
    }

    /*Tomo las palabras que tienes cada documento(sin repetir)
    y la cantidad de veces que se repite cada palabra. Tambien saca el vocabulario de todos los documentos
    y la cantidad de documentos en los que aparece cada palabra*/
    static Dictionary<string,int>[] NonReapeatedWords()
    {
        var Vocabulary = new Dictionary<string,int>();
        var Docs = LimpText;
        var NewDicArr= new Dictionary<string,int>[Docs.Count()];
        for(int i=0;i<Docs.Count();i++)
        {
            var newdic= new Dictionary<string,int>();
            for(int j=0;j<Docs[i].Count();j++)
            {
                if(newdic.ContainsKey(Docs[i][j]))
                {
                    newdic[Docs[i][j]]++;
                }
                else 
                {
                    newdic.Add(Docs[i][j],1);
                    if(Vocabulary.ContainsKey(Docs[i][j]))
                    {
                        Vocabulary[Docs[i][j]]++;
                    
                    }
                    else
                    {
                        Vocabulary.Add(Docs[i][j],1);
                    }
                }
                
            }
            NewDicArr[i]=newdic;
        }
        Words=Vocabulary;
        return NewDicArr;
    }
    //Metodo para calcular TF-Idf
    static Dictionary<string,float>[] Tf_Idfs()
    {
        
        float DocsNumber= LimpText.Count();
        var Tf_Idfs= new Dictionary<string,float>[(int)DocsNumber];
    
        for(int i=0;i<DocsNumber;i++)
        {
            var aux =NonRepeated[i].Count();
            Tf_Idfs[i] = new Dictionary<string, float>();
            for(int j=0;j<aux;j++)
            {
                
                float tf =(float)NonRepeated[i].ElementAt(j).Value/(float)LimpText[i].Count();
                
                float idf = (float)Math.Log10(DocsNumber/(float)Words[NonRepeated[i].ElementAt(j).Key]);
                
                Tf_Idfs[i].Add(NonRepeated[i].ElementAt(j).Key,(float)(tf*idf));
            }

        }
        return Tf_Idfs;
        
    }


}
