using CommandLine;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace VisualStudioUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<DefaultConfiguration>(args).WithParsed(Upgrade);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        static void Upgrade(DefaultConfiguration configuration)
        {
            configuration.Initialize();
            var version = GetVersion(configuration.Version);

            var solutionFiles = Directory.EnumerateFiles(configuration.Path, "*.sln", SearchOption.AllDirectories);
            foreach (string solutionFile in solutionFiles)
            {
                Console.WriteLine(solutionFile);

                var fileContent = File.ReadAllText(solutionFile);
                fileContent = Regex.Replace(fileContent, @"(?<=^# Visual Studio )\d+", version.YearVersion, RegexOptions.Multiline);
                fileContent = Regex.Replace(fileContent, @"(?<=^VisualStudioVersion = )\d+\.\d+\.\d+\.\d+", version.TechnicalVersion, RegexOptions.Multiline);
                fileContent = Regex.Replace(fileContent, @"(?<=^MinimumVisualStudioVersion = )\d+\.\d+\.\d+\.\d+", version.MinimumCompatibleTechnicalVersion, RegexOptions.Multiline);
                File.WriteAllText(solutionFile, fileContent, Encoding.UTF8);
            }

            var projectFiles = Directory.EnumerateFiles(configuration.Path, "*.*proj", SearchOption.AllDirectories);
            foreach (string projectFile in projectFiles)
            {
                Console.WriteLine(projectFile);

                var fileContent = File.ReadAllText(projectFile);
                var newFileContent = Regex.Replace(fileContent, @"(?<=^<Project.*ToolsVersion=\"")\d+\.\d+(?=\"")", version.ToolsVersion, RegexOptions.Multiline);
                newFileContent = UpdateLanguageVersion(newFileContent, configuration.LangVersion);
                if (!ReferenceEquals(newFileContent, fileContent))
                {
                    File.WriteAllText(projectFile, newFileContent, Encoding.UTF8);
                }
            }
        }

        static string UpdateLanguageVersion(string projectFileContent, string langVersion)
        {
            if (string.IsNullOrWhiteSpace(langVersion))
            {
                return projectFileContent;
            }

            if (langVersion == "default")
            {
                return Regex.Replace(projectFileContent, @"\s*<LangVersion>[^<]*<\/LangVersion>", string.Empty, RegexOptions.Multiline);
            }

            var newFileContent = Regex.Replace(projectFileContent, @"(?<=<LangVersion>)[^<]*(?=<\/LangVersion>)", langVersion, RegexOptions.Multiline);
            if (ReferenceEquals(newFileContent, projectFileContent))
            {
                var endOfFirstPropertyGroup = projectFileContent.IndexOf("  </PropertyGroup>", StringComparison.Ordinal);
                newFileContent = projectFileContent.Insert(endOfFirstPropertyGroup, $"    <LangVersion>{langVersion}</LangVersion>\r\n");
            }

            return newFileContent;
        }

        static VisualStudioVersion GetVersion(string versionName)
        {
            switch (versionName)
            {
                case "2019":
                case "VS2019":
                case "16.1.1":
                    return new VisualStudioVersion
                    {
                        YearVersion = "16",
                        TechnicalVersion = "16.0.28922.388",
                        MinimumCompatibleTechnicalVersion = "16.0.0.0",
                        ToolsVersion = "15.0",
                    };
                case "2017":
                case "VS2017":
                case "15.6.3":
                    return new VisualStudioVersion
                    {
                        YearVersion = "15",
                        TechnicalVersion = "15.0.27428.2011",
                        MinimumCompatibleTechnicalVersion = "15.0.0.0",
                        ToolsVersion = "15.0",
                    };
                case "2015":
                case "VS2015":
                case "14.0":
                case "2015 Update 3":
                case "VS2015 Update 3":
                    return new VisualStudioVersion
                    {
                        YearVersion = "14",
                        TechnicalVersion = "14.0.25420.1",
                        MinimumCompatibleTechnicalVersion = "14.0.0.0",
                        ToolsVersion = "14.0",
                    };
                case "2015 Update 2":
                case "VS2015 Update 2":
                    return new VisualStudioVersion
                    {
                        YearVersion = "14",
                        TechnicalVersion = "14.0.25123.0",
                        MinimumCompatibleTechnicalVersion = "14.0.0.0",
                        ToolsVersion = "14.0",
                    };
                case "2013":
                case "VS2013":
                case "12.0":
                case "2013 Update 3":
                case "VS2013 Update 5":
                    return new VisualStudioVersion
                    {
                        YearVersion = "2013",
                        TechnicalVersion = "12.0.40629.0",
                        MinimumCompatibleTechnicalVersion = "10.0.40219.1",
                        ToolsVersion = "12.0",
                    };
                default:
                    throw new ArgumentException($"\"{versionName}\" is not a supported visual studio version name");
            }
        }
    }
}
