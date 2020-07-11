using System;
using CommandLine;
using onmov200;


namespace program
{
    class Program
    {
        static void Extract(ExtractOptions options)
        {
            OnMov200 onMov200 = new OnMov200(options.RootDir, options.OutputDir ?? options.RootDir);

            onMov200.PrintSummary();

            onMov200.ExtractAll();
        }

        static void List(ListOptions options)
        {
            OnMov200 onMov200 = new OnMov200(options.RootDir, options.RootDir);

            onMov200.PrintSummary();
        }

        static void FastFix(FastFixOptions options)
        {
            OnMov200 onMov200 = new OnMov200(options.RootDir, options.RootDir);

            Console.WriteLine("not yet implemented");
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<ExtractOptions, ListOptions, FastFixOptions>(args)
                .WithParsed<ExtractOptions>(options => Extract(options))
                .WithParsed<ListOptions>(options => List(options))
                .WithParsed<FastFixOptions>(options => FastFix(options));


            ;
        }
    }
}