using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Prism.Events;

using MakePdf.Core;

namespace MakePdf.Wpf.Models
{
    public class Runner
    {
        MakePdfCore core;

        public Runner(MakePdfCore core)
        {
            this.core = core;
            core.Subscriber += x => Messenger.Instance[MessengerType.Processing].GetEvent<PubSubEvent<string>>().Publish(x);
        }

        public async Task<bool> RunAsync(IEnumerable<string> items, string outputFullpath, Setting setting)
        {
            var ret = await core.RunAsync(items, outputFullpath, setting);

            return ret;
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

            var ret = await core.RunAsync(inputDirectory, outputFullpath, setting);

            return ret;
        }

        public static bool IsSupported(string fullpath) => Support.IsSupported(fullpath);
    }
}
