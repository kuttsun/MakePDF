using System;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.CommandLineUtils;

namespace SimpleUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Analyze program arguments

            var cla = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                // Application name
                Name = assembly.GetName().Name,
            };

            cla.HelpOption("-?|-h|--help");

            // Create a file list
            cla.Command("list", command =>
            {
                // 説明（ヘルプの出力で使用される）
                command.Description = "Create a file list.";

                command.HelpOption("-?|-h|--help");

                var targetDir = command.Option("-d|--dir", "Target directory", CommandOptionType.SingleValue);

                var outputDir = command.Option("-o|--output", "Target directory (Option)", CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {

                    //Update.Instance.CleanUp(Convert.ToInt32(pidOptions.Value()), deleteFileOptions.Values);
                    CreateFileList(targetDir.Value(), outputDir.Value());
                    Console.WriteLine("cleanup!");
                    return 0;
                });
            });

            // Default behavior
            cla.OnExecute(() =>
            {
                Console.WriteLine("Hello World.");
                return 0;
            });

            cla.Execute(args);
        }

        static void CreateFileList(string targetDir, string outputDir)
        {
            var files = Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories);

            foreach(var file in files)
            {
                Console.WriteLine(file);
            }
        }
    }
}
