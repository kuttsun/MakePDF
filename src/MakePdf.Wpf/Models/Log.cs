using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;

namespace MakePdf.Wpf.Models
{
    public class Log
    {
        static ILoggerFactory loggerFactory;
        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (loggerFactory == null)
                {
                    loggerFactory = new LoggerFactory().AddNLog();
                }
                return loggerFactory;
            }
        }

        static ILogger logger;
        public static ILogger Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = LoggerFactory.CreateLogger("Wpf");
                }

                return logger;
            }
        }
    }
}
