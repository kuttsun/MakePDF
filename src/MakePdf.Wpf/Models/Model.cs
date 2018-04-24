using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using MakePdf.Core;

namespace MakePdf.Wpf.Models
{
    sealed class Model
    {
        IServiceProvider serviceProvider;
        MakePdfCore core;

        public static Model Instance { get; } = new Model();


        public Model()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection, Log.LoggerFactory);

            serviceProvider = serviceCollection.BuildServiceProvider();

            core = serviceProvider.GetService<MakePdfCore>();
        }

        void ConfigureServices(IServiceCollection services, ILoggerFactory loggerFactory)
        {
            // Add logger to DI container
            services.AddSingleton(loggerFactory);
            services.AddLogging();

            // Ad configuration to DI container
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("usersettings.json", optional: true)
                .Build();

            services.AddSingleton(configuration);

            // Add options to DI container
            services.AddOptions();

            // Add application to DI container
            services.AddSingleton<MakePdfCore>();
        }

        public async Task<bool> RunAsync(IEnumerable<string> items, string outputFullpath)
        {
            return await core.RunAsync(items, outputFullpath);
        }

        public async Task<bool> RunAsync(string inputDirectory, string outputPath, Setting setting)
        {
            var outputFullpath = "";

            if (Path.IsPathRooted(outputPath))
            {
                // outputPath is absolute path.
                outputFullpath = outputPath;
            }
            else
            {
                // outputPath is relative path.
                outputFullpath = inputDirectory + @"\" + outputPath;
            }
            return await core.RunAsync(inputDirectory, outputFullpath, setting);
        }

        public static bool IsSupported(string fullpath) => Support.IsSupported(fullpath);
    }
}
