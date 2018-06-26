using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Prism.Events;

using MakePdf.Wpf.Models.Settings;

namespace MakePdf.Wpf.Models
{
    public class Options
    {
        public UserSetting UserSetting { get; }

        public Options(IOptions<UserSetting> userSetting)
        {
            UserSetting = userSetting.Value;
        }

        public void AddRecentFile(string path)
        {
            var recentFiles = UserSetting.RecentFiles.Where(x => x != path).ToList();
            recentFiles.Add(path);
            UserSetting.RecentFiles = recentFiles;
            WriteFile();

            Messenger.Instance[MessengerType.UpdateRecentFiles].GetEvent<PubSubEvent<string>>().Publish(path);
        }

        public void WriteFile()
        {
            var fullpath = Directory.GetCurrentDirectory() + @"\usersettings.json";

            var json = JsonConvert.SerializeObject(this, Formatting.Indented);

            using (FileStream fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(json);
            }
        }
    }
}
