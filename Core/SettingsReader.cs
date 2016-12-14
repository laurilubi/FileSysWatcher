using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileSysWatcher.Modules;

namespace FileSysWatcher
{
    public static class SettingsReader
    {
        public static List<Section> GetSectionsConfig(string filename)
        {
            var sections = new List<Section>();
            var rawSections = ParseConfigFile(filename);
            foreach (var rawSection in rawSections)
            {
                var name = rawSection[0];
                if (name.Length > 2) name = name.Substring(1, name.Length - 2);
                var settings = new Settings();
                foreach (var line in rawSection.Skip(1))
                {
                    var split = line.Split('=');
                    var key = split[0].Trim();
                    var value = split[1].Trim();
                    settings.Add(key, value);
                }
                sections.Add(new Section(name, settings));
            }
            return sections;
        }

        public static List<List<string>> ParseConfigFile(string filename)
        {
            var result = new List<List<string>>();
            var lines = File.ReadAllLines(filename).ToList();
            lines.Add("[MethodInternalDummySection]");
            List<string> currentSectionConfig = null;
            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (line == "") continue;
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    if (currentSectionConfig != null)
                    {
                        result.Add(currentSectionConfig);
                    }
                    currentSectionConfig = new List<string>();
                }
                if (currentSectionConfig == null) continue;
                currentSectionConfig.Add(line);
            }
            return result;
        }
    }
}
