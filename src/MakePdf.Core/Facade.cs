using System;
using System.Collections.Generic;
using System.Text;

namespace MakePdf.Core
{
    public class Facade
    {
        public void Run(string[] files)
        {
            foreach (var file in files)
            {
                var doc = Factory.Create(file);
            }
        }

        public void Run()
        {

        }
    }
}
