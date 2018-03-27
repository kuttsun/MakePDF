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
    sealed class Model
    {
        MakePdfCore core;
        ILogger logger;

        public static Model Instance { get; } = new Model();


        public Model()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddNLog();
            loggerFactory.ConfigureNLog("NLog.config");

            logger = loggerFactory.CreateLogger("logfile");

            core = new MakePdfCore(logger);
        }

        public async Task<bool> RunAsync(IEnumerable<string> items, string outputFullpath)
        {
            return await core.RunAsync(items, outputFullpath);
        }

        public async Task<bool> RunAsync(string inputDirectory, string outputFullpath, Setting setting)
        {
            return await core.RunAsync(inputDirectory, outputFullpath, setting);
        }

        public bool IsSupported(string fullpath) => core.IsSupported(fullpath);
    }
}
