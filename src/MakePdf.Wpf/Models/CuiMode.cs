using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MakePdf.Wpf.Models
{
    class CuiMode
    {
        ILogger logger;
        Runner runner;

        public CuiMode(Runner runner, ILogger logger)
        {
            this.logger = logger;
            this.runner = runner;
        }

        public int Start(string inputFile)
        {
            var fullpath = Path.GetFullPath(inputFile);
            var setting = Setting.ReadFile(fullpath);
            var workingDirectory = Path.GetDirectoryName(fullpath);

            logger.LogInformation("Start CUI Mode");
            try
            {
                var result = runner.RunAsync(workingDirectory, setting.OutputFile, setting).Result;
                if (result)
                {
                    logger.LogInformation("Completed");
                }
                else
                {
                    logger.LogWarning("Failed");
                }

                return result ? 0 : 1;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return 2;
            }
        }
    }
}
