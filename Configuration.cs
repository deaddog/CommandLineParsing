using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides a set of methods for managing a simple configuration file, with a collection of key=value pairs.
    /// </summary>
    public class Configuration : IConfiguration
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

        static Configuration()
        {
            string KEYCHARS = "[a-zA-Z][a-zA-Z0-9]*";
            string NAME = "(?<rootname>" + KEYCHARS + ")";
            string SUBNAME = "(\\.(?<subname>" + KEYCHARS + "))";
            string KEY = "\\.(?<key>" + KEYCHARS + ")";

            string R_KEY = string.Format("{0}{1}*{2}", NAME, SUBNAME, KEY);

            keyRegex = new Regex($"^{R_KEY}$");
            lineRegex = new Regex("^(?<name>" + R_KEY + @")[ \t]*=[ \t]*(?<value>[^ \t][^\r\n]*)");
        }

        private static readonly Regex keyRegex;
        private static readonly Regex sectionRegex;
        private static readonly Regex lineRegex;

        /// <summary>
        /// Determines whether <paramref name="key"/> is a valid <see cref="Configuration"/> key.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <returns><c>true</c> if <paramref name="key"/> can be used as a key in a <see cref="Configuration"/> object; otherwise <c>false</c>.</returns>
        public static bool IsKeyValid(string key)
        {
            return keyRegex.IsMatch(key);
        }

        private string filePath;
        private Dictionary<string, string> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="filePath">The file containing the configuration details.
        /// If the file does not exist it is created when a key/value pair is added.</param>
        public Configuration(string filePath)
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

        /// <summary>
        /// Gets or sets the value associated with a key.
        /// </summary>
        /// <param name="key">The key (name.name) that the value should be associated with.</param>
        /// <returns>The value that corresponds to <paramref name="key"/>.</returns>
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

        /// <summary>
        /// Removes the key/value pair with the specified key.
        /// The pair is also removed from the configuration file.
        /// </summary>
        /// <param name="key">The key that should be removed from the configuration.</param>
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

        /// <summary>
        /// Determines whether the specified key exists in the configuration file.
        /// </summary>
        /// <param name="key">The key that is looked for in the configuration file.</param>
        /// <returns><c>true</c>, if <paramref name="key"/> is defined in the configuration file; otherwise, <c>false</c>.</returns>
        public bool HasKey(string key)
        {
            return values.ContainsKey(key);
        }

        /// <summary>
        /// Clears the configuration. This also clear the configuration file.
        /// </summary>
        public void Clear()
        {
            File.WriteAllText(filePath, "");
            values.Clear();
        }

        /// <summary>
        /// Gets all the key/value pairs in the configuration.
        /// </summary>
        /// <returns>A collection of all key/value pairs in the configuration.</returns>
        public IEnumerable<KeyValuePair<string, string>> GetAll()
        {
            foreach (var pair in values)
                yield return pair;
        }
    }
}
