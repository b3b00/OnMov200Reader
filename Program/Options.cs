using CommandLine;

namespace program
{
    [Verb("extract", HelpText = "extract activities")]
    public class ExtractOptions
    {
        [Option('r', "root", Required = true, HelpText = "OnMov200 storage root directory.")]
        public string RootDir { get; set; }
        
        [Option('o', "output", Required = false, HelpText = "output directory for GPX files.")]
        public string OutputDir { get; set; }
    }
    
    [Verb("fastfix", HelpText = "update fastfix data.")]
    public class FastFixOptions
    {
        [Option('r', "root", Required = true, HelpText = "OnMov200 storage root directory.")]
        public string RootDir { get; set; }
                
    }
    
    [Verb("list", HelpText = "list activities")]
    public class ListOptions
    {
        [Option('r', "root", Required = true, HelpText = "OnMov200 storage root directory.")]
        public string RootDir { get; set; }
    }
}