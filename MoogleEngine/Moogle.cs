using System;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading.Tasks;
    using System.ComponentModel;
    using System.Collections.Generic;
using System.Linq;
namespace MoogleEngine;

public static class Moogle
{
    public static SearchResult Query(string query) {
        
        /*SearchItem[] items = new SearchItem[3] {
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
        };*/
        var busqueda = new Query(query);
        

        return new SearchResult(busqueda.Search(), busqueda.suggestion());
    }    
}
