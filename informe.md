# Moogle
> Proyecto de Programación I.
> Facultad de Matemática y Computación - Universidad de La Habana.
> Cursos 2021, 2022.

En este proyecto titulado "Moogle!" me he valido de los conocimientos adquiridos sobre C# en los últimos meses para crear un motor de búsqueda. Empiricamente y por medio "ensallo y error", también pude con HTML hacer algunos cambios en la interfaz que hacen el "Moogle!" más vistoso. Sin más dilación procedo a enunciar y explicar las clases que he creado.

# -Dirs
```cs
static public class Dirs
{
    static public string path;
    static public string[] dirs;
    static Dirs() {
        path = @"../Content";
        dirs = Directory.GetFiles(path);
    }
}
```
En esta clase solo recojo las direcciones de los archivos de texto


# -Dirsfiles


En esta clase primero obtengo los textos de las direcciones recogidas en la clase Dirs 

```cs 
static public string[] textos;
``` 
, los guardo  y los depuro (quito todo caracter que no sea letra o número, sustituyo las vocales con tilde por las que no la tienen y lo paso todo a minusculas )

```cs
public static List<List<string>> LimpText;
```
, luego separo en cada documento las palabras sin repetir, y le asocio la cantidad de veces que aparece en ese documento, donde cada documento es un diccionario con sus palabras y la cantidad de veces que se repite.
 
 ```cs
 static public Dictionary<string, int>[] nonrepeated;
 ```

 Tambien creo un diccionario que contendra todas las palabras y la cantidad de documentos en los que aparece.
 ```cs
 static public Dictionary<string,int> Palabras;
 ```
 
  Valiéndome del modelo vectorial calculo Tf-Idf de cada palabra en cada documento y así quedaria vectorizado mi corpus.
  ```cs
  static public Dictionary <string,float>[] Tf_Idfs_documentos;
  ```

# -Query


Aqui luego de recibir la query
```cs
public static string query;
```

 y, como en la anterior clase, depurarla de caracteres raros, normalizarla y separar las palabras junto con la cantidad de veces que se repite cada una, pero esta vez dejando los caracteres de los operadores para detectarlos y luego quitarlos.
 ```cs
private List<string> operatedwords;
        
public  static Dictionary<string,int> nonrepeated;
 ```
 
  Vectorizo la query sacandole Tf-Idf a cada palabra de la query
  ```cs
  public static Dictionary<string,float> TfsIdfs;
  ```
  
   y obtengo la similitud con los documentos vectorizados por medio de la fórmula de la Similitud del Coseno.
   ```cs
   public static float[] SimCos()
   ```
   Implementé los operadores correspondientes.
 
 ### Operador "no debe aparecer"
 ```cs
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
 ```

 ### Operador " debe aparecer"
 ```cs
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
 ```
 
 ### Operador de importancia
 ```cs
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
 ```

 ### Operador de cercania
 ```cs
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

    //Tomo las palabras a analizar la distancia   
    var p1=operatedwords[i].Substring(0,pos);
    var p2=operatedwords[i].Substring(pos+1,operatedwords[i].Length-pos-1);

    /*Establezco el Score adicional que sera repartido luego
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
            /*En caso de contener las 2 palabras el documento, se le aumentara el score proporcionalmente 
            a la cercania*/
            Scores[j]+= additionalscore/MinorDistInArch;
        }
    }
    
    goto next;
}
```
Si alguna palabra del query no aparece en ningún documento envío un string de sugerencia donde por medio de formula de la Distancia de Levenshtein determino si sugerir: cambiar la palabra por otra cercana o prescindir de ella.
```cs
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
```


(Las otras clases ya existían.)
