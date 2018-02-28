using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;



namespace MakePdf.Wpf.Models
{
    public class Setting : MakePdf.Core.Setting
    {
        public string OutputFile { get; set; }

        public void WriteFile(string path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(json);
            }
        }

        public static Setting ReadFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                return JsonConvert.DeserializeObject<Setting>(sr.ReadToEnd());
            }
        }
    }
}