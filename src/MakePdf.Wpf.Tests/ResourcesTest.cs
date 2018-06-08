using System;
using System.Resources;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace MakePdf.Wpf.Tests
{
    public class ResourcesTest
    {
        [Theory,
            InlineData("Resources.ja.resx")]
        public void LocalizeTest(string localizeFilePath)
        {
            var masterKeys = GetKeys(@"Resources\Resources.resx");
            var subKeys = GetKeys($@"Resources\{localizeFilePath}");

            for (int i = 0; i < masterKeys.Count; i++)
            {
                Assert.Equal(masterKeys[i], subKeys[i]);
            }

            Assert.Equal(masterKeys.Count, subKeys.Count);
        }

        IList<string> GetKeys(string path)
        {
            var keys = new List<string>();

            using (var reader = new ResXResourceReader(path))
            {
                foreach (DictionaryEntry entry in reader)
                {
                    keys.Add(entry.Key.ToString());
                }
            }

            keys.Sort();

            return keys;
        }
    }
}
