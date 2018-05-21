using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using NLog.Extensions.Logging;

using MakePdf.Core;
using MakePdf.Wpf.Models.Settings;

namespace MakePdf.Wpf.Models
{
    static class Service
    {
        static IServiceProvider provider;
        public static IServiceProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    var services = ConfigureServices();
                    provider = services.BuildServiceProvider();
                }
                return provider;
            }
        }

        static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            // Add logger to DI container
            var loggerFactory = new LoggerFactory().AddNLog();
            services.AddSingleton(loggerFactory);
            services.AddLogging();

            // Add configuration to DI container
            var hoge = Directory.GetCurrentDirectory();
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(hoge)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            services.AddSingleton(configuration);

            // Add options to DI container
            services.AddOptions();

            // Initialize options
            services.Configure<AppSetting>(configuration.GetSection(nameof(AppSetting)));

            // Add application to DI container
            services.AddSingleton<MakePdfCore>();
            services.AddSingleton<Runner>();
            services.AddSingleton<Updater>();
            services.AddSingleton<CuiMode>();

            return services;
        }
    }
}
