using System;
using System.IO;
using System.Linq;
using onmov200;
using onmov200.parser;
using onmov200.gpx;
using onmov200.model;


namespace program
{
    class Program
    {
        
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Environment.Exit(1);
            }
            
            

            string root = args[0];
            
            OnMov200 onMov200 = new OnMov200(root,root); 

            onMov200.PrintSummary();
            
            onMov200.ExtractAll();

            ;
        }

       
    }
}