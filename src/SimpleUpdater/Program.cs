using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

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

            // Create a file list
            cla.Command("list", command =>
            {
                command.Description = "Create a file list.";
                command.HelpOption("-?|-h|--help");

                var targetDir = command.Option("-d|--dir", "Target directory", CommandOptionType.SingleValue);
                var outputFilename = command.Option("-o|--output", "Output file name", CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    CreateFileList(targetDir.Value(), outputFilename.Value());
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

        static void CreateFileList(string target, string output)
        {
            var files = Directory.GetFiles(target, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var hash = Hash.GetHash<SHA256CryptoServiceProvider>(fs);
                    Console.WriteLine(file);
                    Console.WriteLine(hash);
                }
            }
        }
    }
}
