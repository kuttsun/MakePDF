using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Extensions.Logging;

namespace MakePdf.Wpf.Models
{
    class CuiMode
    {
        Setting setting;
        string workingDirectory;
        ILogger logger;

        public CuiMode(string inputFile)
        {
            var fullpath = Path.GetFullPath(inputFile);
            setting = Setting.ReadFile(fullpath);
            workingDirectory = Path.GetDirectoryName(fullpath);
            logger = Model.Instance.Logger;
        }

        public int Start()
        {
            logger.LogInformation("Start CUI Mode");
            try
            {
                var result = Model.Instance.RunAsync(workingDirectory, setting.OutputFile, setting).Result;
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
