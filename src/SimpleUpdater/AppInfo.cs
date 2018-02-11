using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using Newtonsoft.Json;

using SimpleUpdater.Common;

namespace SimpleUpdater
{
    public class AppInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public List<FileInfo> Files { get; set; } = new List<FileInfo>();

        public void AddFileInfo(string[] files)
        {
            foreach (var file in files)
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var hash = Hash.GetHash<SHA256CryptoServiceProvider>(fs);
                    Files.Add(new FileInfo { Name = file, Hash = hash });
                    Console.WriteLine(file);
                    Console.WriteLine(hash);
                }
            }
        }

        public void WriteFile(string fileName)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(json);
            }
        }

        public static AppInfo ReadFile(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                return JsonConvert.DeserializeObject<AppInfo>(sr.ReadToEnd());
            }
        }

        public static AppInfo ReadString(string str)
        {
            return JsonConvert.DeserializeObject<AppInfo>(str);
        }
    }
}
