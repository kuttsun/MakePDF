using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Extensions.CommandLineUtils;

using MakePdf.Wpf.Models;

namespace MakePdf.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static int Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var appName = assembly.GetName().Name;
            var appVersion = "";

            Console.WriteLine($"{appName} {appVersion}");

            // Analyze program arguments

            var cla = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                // Application name
                Name = appName,
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
                    catch(Exception)
                    {
                        param = "failed";
                    }

                    // Restart application
                    Process.Start($@"{dstDir.Value()}\{targetAppName.Value()}", param);

                    return 0;
                });
            });

            // Default behavior
            cla.OnExecute(() =>
            {
                App app = new App();
                app.InitializeComponent();
                return app.Run();
            });

            try
            {
                return cla.Execute(args);
            }
            catch (Exception e)
            {
                Debug.Assert(false);
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
