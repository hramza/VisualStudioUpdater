using CommandLine;

namespace VisualStudioUpdater
{
    public class DefaultConfiguration
    {

        [Option('p', "path", HelpText = "The solution directory")]
        public string Path { get; set; }

        [Option('v', "version", HelpText = "The visual studio version you target. By default, the vs 2019 will be targeted.")]
        public string Version { get; set; }

        [Option('l', "langversion", HelpText = "C# version to target")]
        public string LangVersion { get; set; }

        public void Initialize()
        {
            Path = Path ?? ".";
            Version = Version ?? "VS2019";
        }
    }
}