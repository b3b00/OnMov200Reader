using System;
using CommandLine;
using onmov200;


namespace program
{
    class Program
    {
        static void Extract(ExtractOptions options)
        {
            OnMov200 onMov200 = new OnMov200(options.RootDir,options.OutputDir);

            var activities  = onMov200.GetHeaders();
            int i = 0;
            foreach (var activity in activities)
            {
                i++;
                Console.WriteLine($"{i} - {activity}");
            };
            string which = Console.ReadLine();
            int index = -1;
            if (int.TryParse(which, out index))
            {
                if (index >= 1 && index <= activities.Count)
                {
                    onMov200.ExtractActivity(activities[index-1]);
                }
            }
            else
            {
                if (which == "all")
                {
                    onMov200.ExtractAll(activities);
                }
            }
        }

        static void List(ListOptions options)
        {
            OnMov200 onMov200 = new OnMov200(options.RootDir,options.OutputDir);
            onMov200.PrintSummary();
        }

        static void FastFix(FastFixOptions options)
        {
            OnMov200 onMov200 = new OnMov200(options.RootDir, options.RootDir);

            onMov200.UpDateFastFixIfNeeded(options.Force);
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