using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.CommandLineUtils;

using MakePdf.Wpf.Models;

namespace MakePdf.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);

        [STAThread]
        public static int Main(string[] args)
        {
            AttachConsole(-1);

            var myInfo = MyInformation.Instance;

            // Analyze program arguments

            var cla = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                // Application name
                Name = myInfo.Name,
            };

            cla.HelpOption("-?|-h|--help");

            // Start update
            cla.Command("update", command =>
            {
                command.Description = "Start update.";
                command.HelpOption("-?|-h|--help");

                var pid = command.Option("--pid", "Process ID of target application", CommandOptionType.SingleValue);
                var targetAppName = command.Option("-n|--name", "Application name. Default is assembly name", CommandOptionType.SingleValue);
                var srcDir = command.Option("-s|--src", "Source directory (new version directory)", CommandOptionType.SingleValue);
                var dstDir = command.Option("-d|--dst", "Destination directory (current version direcroty)", CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    string param;
                    try
                    {
                        Updater.Instance.Update(pid.Value(), srcDir.Value(), dstDir.Value());

                        param = "completed";
                    }
                    catch (Exception)
                    {
                        param = "failed";
                    }

                    // Restart application
                    Process.Start($@"{dstDir.Value()}\{targetAppName.Value()}", param);

                    return 0;
                });
            });

            // Default behavior
            var version = cla.Option("-v|--version", "Show version", CommandOptionType.NoValue);
            var file = cla.Option("-f|--file", "Input file (CUI mode)", CommandOptionType.SingleValue);
            cla.OnExecute(() =>
            {
                if (version.HasValue())
                {
                    Console.WriteLine($"\n{myInfo.Name} {myInfo.AssemblyInformationalVersion}");
                    return 0;
                }

                var logger = Model.Instance.Logger;
                logger.LogInformation($"{myInfo.Name} {myInfo.AssemblyInformationalVersion}");

                if (file.HasValue())
                {
                    return new CuiMode(file.Value()).Start();
                }

                App app = new App();
                app.InitializeComponent();
                return app.Run();
            });

            // Execution
            try
            {
                return cla.Execute(args);
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
                return 1;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new Bootstrapper().Run();
        }
    }
}
