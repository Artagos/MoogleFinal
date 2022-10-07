namespace MoogleEngine;
using System.Net.Mime;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;


//Recopilo las direcciones
static public class Dirs
{
    static public string path;
    static public string[] dirs;
    static Dirs() {
        path = @"../Content";
        dirs = Directory.GetFiles(path);
    }
}





