# Moogle
> Proyecto de Programación I.
> Facultad de Matemática y Computación - Universidad de La Habana.
> Cursos 2021, 2022.

En este proyecto titulado "Moogle!" me he valido de los conocimientos adquiridos sobre C# en los últimos meses para crear un motor de búsqueda. Gracias al metodo de "ensallo y error" también pude con HTML hacer algunos cambios en la interfaz que hacen el "Moogle!" más vistoso. Sin más dilación procedo a enunciar y explicar las clases que he creado.

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
   
    Implementé los operadores correspondientes. Si alguna palabra del query no aparece en ningún documento envío un string de sugerencia donde por medio de formula de la Distancia de Levenshtein determino si sugerir: cambiar la palabra por otra cercana o prescindir de ella.


(Las otras clases ya existían ya.)
