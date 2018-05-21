using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;
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
        // https://docs.microsoft.com/en-us/windows/console/attachconsole
        [DllImport("Kernel32.dll")]
        static extern bool AttachConsole(int processId);
        const int ATTACH_PARENT_PROCESS = -1;

        // https://docs.microsoft.com/en-us/windows/console/allocconsole
        [DllImport("Kernel32.dll")]
        static extern bool AllocConsole();

        // https://docs.microsoft.com/en-us/windows/console/freeconsole
        [DllImport("Kernel32.dll")]
        static extern bool FreeConsole();

        [STAThread]
        public static int Main(string[] args)
        {
            var attachConsole = AttachConsole(ATTACH_PARENT_PROCESS);

            var myInfo = MyInformation.Instance;

            // Update
            var mgr = Service.Provider.GetService<Updater>();
            if (mgr.CanUpdate(args))
            {
                if (attachConsole == false)
                {
                    // Create Console
                    if (AllocConsole() == false)
                    {
                        Debug.Assert(false, "Update failed");
                        return 1;
                    }

                    var stream = Console.OpenStandardOutput();
                    var stdout = new StreamWriter(stream)
                    {
                        AutoFlush = true,
                    };
                    Console.SetOut(stdout);
                }                

                mgr.Update(args);
                mgr.RestartApplication(args);

                if (attachConsole == false)
                {
                    FreeConsole();
                }
                return 0;
            }

            // Update is complete
            mgr.Completed(args);

            // Analyze program arguments
            var cla = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                // Application name
                Name = myInfo.Name,
            };

            cla.HelpOption("-?|-h|--help");

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

                    var logger = Service.Provider.GetService<ILogger<App>>();
                    logger.LogInformation($"{myInfo.Name} {myInfo.AssemblyInformationalVersion}");

                    if (file.HasValue())
                    {
                        var cuimode = Service.Provider.GetService<CuiMode>();
                        return cuimode.Start(file.Value());
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
