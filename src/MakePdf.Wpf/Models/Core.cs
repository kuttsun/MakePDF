using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using MakePdf.Core;

namespace MakePdf.Wpf.Models
{
    class Core
    {
        MakePdfCore core;
        ILogger logger;

        public Core(ILogger logger)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            });
            loggerFactory.ConfigureNLog("NLog.config");

            this.logger = loggerFactory.CreateLogger("logfile");

            core = new MakePdfCore(logger);
        }

        public async Task<bool> RunAsync(string outputFullpath, IEnumerable<string> items)
        {
            return await core.RunAsync(outputFullpath, items);
        }

        public async Task<bool> RunAsync(string inputDirectory, string outputFullpath, Setting setting)
        {
            return await core.RunAsync(inputDirectory, outputFullpath, setting);
        }

        public bool IsSupported(string fullpath) => core.IsSupported(fullpath);
    }
}
