using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommandLineParsing
{
    public class Config
    {
        private static void ensurePath(string path)
        {
            string[] levels = path.Split(Path.DirectorySeparatorChar);
            var drive = new DriveInfo(levels[0]);
            if (drive.DriveType == DriveType.NoRootDirectory ||
                drive.DriveType == DriveType.Unknown)
                throw new ArgumentException("Unable to evaluate path drive; " + levels[0], "path");

            if (!drive.IsReady)
                throw new ArgumentException("Drive '" + levels[0] + "' is not ready.", "path");

            path = levels[0] + "\\";
            for (int i = 1; i < levels.Length; i++)
            {
                path = Path.Combine(path, levels[i]);
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                    dir.Create();
            }
        }

        private static Tuple<string, string> loadKeyValuePair(string line)
        {
            line = line.Trim();

            var pair = lineRegex.Match(line);

            if (!pair.Success)
                return null;
            else
                return Tuple.Create(pair.Groups["name"].Value, pair.Groups["value"].Value);
        }

        static Config()
        {
            string KEYCHARS = "[a-zA-Z][a-zA-Z0-9]*";
            string NAME = "(?<rootname>" + KEYCHARS + ")";
            string SUBNAME = "(\\.(?<subname>" + KEYCHARS + "))";
            string KEY = "\\.(?<key>" + KEYCHARS + ")";

            string R_KEY = string.Format("{0}{1}*{2}", NAME, SUBNAME, KEY);

            keyRegex = new Regex(R_KEY);
            lineRegex = new Regex("^(?<name>" + R_KEY + @")[ \t]*=[ \t]*(?<value>[^ \t][^\r\n]*)");
        }

        private static readonly Regex keyRegex;
        private static readonly Regex lineRegex;

        private string filePath;
        private Dictionary<string, string> values;

        public Config(string filePath)
        {
            this.filePath = filePath;

            ensurePath(Path.GetDirectoryName(filePath));

            values = new Dictionary<string, string>();
            if (!File.Exists(filePath))
                return;

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            foreach (var l in lines)
            {
                var temp = loadKeyValuePair(l);
                if (temp != null)
                    values[temp.Item1] = temp.Item2;
            }
        }

        public string this[string key]
        {
            get
            {
                key = key.ToLower();

                string result = null;
                values.TryGetValue(key, out result);
                return result;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                if (value == null)
                    throw new ArgumentNullException("value");

                key = key.Trim().ToLower();
                value = value.Trim();

                if (key.Length == 0)
                    return;

                if (!keyRegex.IsMatch(key))
                    return;
                if (value.Contains('\r') || value.Contains('\n'))
                    return;

                string setting = string.Format("{0} = {1}", key, value);

                if (values.ContainsKey(key))
                {
                    bool updated = false;
                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    for (int i = 0; i < lines.Length; i++)
                        if (Regex.IsMatch(lines[i], key + " *="))
                            if (updated)
                                lines[i] = null;
                            else
                            {
                                lines[i] = setting;
                                updated = true;
                            }

                    File.WriteAllText(filePath, string.Join("\n", lines.Where(x => x != null)) + "\n", Encoding.UTF8);
                }
                else
                    File.AppendAllText(filePath, setting + "\n", Encoding.UTF8);

                values[key] = value;
            }
        }

        public void Remove(string key)
        {
            if (!Regex.IsMatch(key, "^[a-zA-Z0-9]+(\\.[a-zA-Z0-9]+)*$"))
                return;

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            for (int i = 0; i < lines.Length; i++)
                if (Regex.IsMatch(lines[i], key + " *="))
                    lines[i] = null;
            values.Remove(key);

            File.WriteAllText(filePath, string.Join("\n", lines.Where(x => x != null)) + "\n", Encoding.UTF8);
        }

        public void Clear()
        {
            File.WriteAllText(filePath, "");
            values.Clear();
        }

        public IEnumerable<KeyValuePair<string, string>> GetAll()
        {
            foreach (var pair in values)
                yield return pair;
        }
    }
}
