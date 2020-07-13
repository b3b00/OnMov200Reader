using CommandLine;

namespace program
{
    public class OnMov200Options
    {
        [Option('r', "root", Required = false, HelpText = "OnMov200 storage root directory.")]
        public string RootDir { get; set; }
        
        [Option('o', "output", Required = false, HelpText = "output directory for GPX files.")]
        public string OutputDir { get; set; }
    }
    
    [Verb("extract", HelpText = "extract activities")]
    public class ExtractOptions : OnMov200Options
    {
        
    }
    
    [Verb("fastfix", HelpText = "update fastfix data.")]
    public class FastFixOptions : OnMov200Options
    {
        [Option('f', "force", Required = false, HelpText = "force fastfix update.")]
        public bool Force { get; set; }
    }
    
    [Verb("list", HelpText = "list activities")]
    public class ListOptions : OnMov200Options
    {
        
    }
}