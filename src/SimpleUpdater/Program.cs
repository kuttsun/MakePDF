using System;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.CommandLineUtils;

namespace SimpleUpdater
{
    class Program
    {
        static int Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Analyze program arguments

            var cla = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                // Application name
                Name = assembly.GetName().Name,
            };

            cla.HelpOption("-?|-h|--help");

            // Create a AppInfo
            cla.Command("list", command =>
            {
                command.Description = "Create a file list.";
                command.HelpOption("-?|-h|--help");

                var targetDir = command.Option("-d|--dir", "Target directory", CommandOptionType.SingleValue);
                var outputFilename = command.Option("-o|--output", "Output file name (Option). Default is AppInfo.json", CommandOptionType.SingleValue);
                var appName = command.Option("-n|--name", "Application name (Option). Default is assembly name", CommandOptionType.SingleValue);
                var version = command.Option("-v|--version", "Application version (Option). Default is assembly version", CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    var files = Directory.GetFiles(targetDir.Value(), "*", SearchOption.AllDirectories);
                    var appInfo = new AppInfo()
                    {
                        Name = appName.Value() ?? Assembly.GetExecutingAssembly().GetName().Name,
                        Version = version.Value() ?? ""
                    };
                    appInfo.AddFileInfo(files);
                    appInfo.WriteFile(outputFilename.Value() ?? "AppInfo.json");
                    return 0;
                });
            });

            // Default behavior
            cla.OnExecute(() =>
            {
                Console.WriteLine("Hello World.");
                return 0;
            });

            try
            {
                return cla.Execute(args);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return 1;
            }
        }
    }
}
