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

        private static string parseSection(string line)
        {
            var match = sectionRegex.Match(line.Trim());

            return match.Success ? match.Groups["key"].Value.ToLower() : null;
        }
        private static Tuple<string, string> parseKey(string key)
        {
            var match = keyRegex.Match(key.Trim().ToLower());

            return match.Success ? Tuple.Create(match.Groups["root"].Value, match.Groups["key"].Value) : null;
        }
        private static Tuple<string, string> parseLine(string line)
        {
            var match = lineRegex.Match(line.Trim());

            return match.Success ? Tuple.Create(match.Groups["key"].Value.ToLower(), match.Groups["value"].Value) : null;
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

            keyRegex = new Regex($@"^(?<root>{KEYCHARS})\.(?<key>({KEYCHARS}\.)*{KEYCHARS})$");
            sectionRegex = new Regex($@"^\[(?<key>{KEYCHARS})\]");
            lineRegex = new Regex($@"^(?<key>({KEYCHARS}\.)*{KEYCHARS})[ \t]*=[ \t]*(?<value>[^ \t][^\r\n]*)");
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

        private readonly Encoding encoding;
        private readonly string filePath;

        private KeySearchResult findKey(string key)
        {
            string[] content = File.Exists(filePath) ? File.ReadAllLines(filePath, encoding) : new string[0];
            return KeySearchResult.FindKey(key, content);
        }
        private KeySearchResult findKey(string key, string[] lines)
        {
            return KeySearchResult.FindKey(key, lines);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="filePath">The file containing the configuration details.
        /// If the file does not exist it is created when a key/value pair is added.</param>
        public Configuration(string filePath)
            : this(filePath, Encoding.UTF8)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="filePath">The file containing the configuration details.
        /// <param name="encoding">The encoding that should be used when performing IO operations.</param>
        /// If the file does not exist it is created when a key/value pair is added.</param>
        public Configuration(string filePath, Encoding encoding)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            this.filePath = filePath;
            this.encoding = encoding;

            ensurePath(Path.GetDirectoryName(filePath));
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
                if (!keyRegex.IsMatch(key))
                    return null;

                var index = findKey(key);
                if (!index.Exists)
                    return null;

                return parseLine(index.Lines[index.KeyIndex]).Item2;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                if (value == null)
                    throw new ArgumentNullException("value");

                var myKey = parseKey(key);

                if (myKey == null)
                    throw new ArgumentException("Invalid key format: " + key, nameof(key));
                if (value.Contains('\r') || value.Contains('\n'))
                    throw new ArgumentException("Newlines cannot be included in configuration values", nameof(value));

                var index = findKey(key);
                string[] newConfig;

                if (index.Exists)
                {
                    newConfig = new string[index.Lines.Length];
                    index.Lines.CopyTo(newConfig, 0);
                    newConfig[index.KeyIndex] = index.SectionExists ? $"{myKey.Item2} = {value}" : $"{myKey.Item1}.{myKey.Item2} = {value}";
                }
                else if (index.SectionExists)
                {
                    newConfig = new string[index.Lines.Length + 1];
                    Array.Copy(index.Lines, 0, newConfig, 0, index.KeyIndex);
                    newConfig[index.KeyIndex] = $"{myKey.Item2} = {value}";
                    Array.Copy(index.Lines, index.KeyIndex, newConfig, index.KeyIndex + 1, index.Lines.Length - index.KeyIndex);
                }
                else
                {
                    newConfig = new string[index.Lines.Length + 2];
                    index.Lines.CopyTo(newConfig, 0);

                    if (index.SectionIndex != newConfig.Length - 2)
                        throw new InvalidOperationException("Invalid index for new section.");
                    if (index.KeyIndex != newConfig.Length - 1)
                        throw new InvalidOperationException("Invalid index for new key.");

                    newConfig[index.SectionIndex] = $"[{myKey.Item1}]";
                    newConfig[index.KeyIndex] = $"{myKey.Item2} = {value}";
                }

                File.WriteAllLines(filePath, newConfig, encoding);
            }
        }

        /// <summary>
        /// Removes the key/value pair with the specified key.
        /// The pair is also removed from the configuration file.
        /// </summary>
        /// <param name="key">The key that should be removed from the configuration.</param>
        public void Remove(string key)
        {
            if (!keyRegex.IsMatch(key))
                return;

            var keyIndex = findKey(key);
            if (!keyIndex.Exists)
                return;

            bool removeSection = keyIndex.SectionExists && (keyIndex.SectionIndex + 2 >= keyIndex.Lines.Length || sectionRegex.IsMatch(keyIndex.Lines[keyIndex.SectionIndex + 2]));
            int first = removeSection ? keyIndex.SectionIndex : keyIndex.KeyIndex;
            int length = removeSection ? 2 : 1;
            string[] newConfig = new string[keyIndex.Lines.Length - length];

            Array.Copy(keyIndex.Lines, 0, newConfig, 0, first);
            Array.Copy(keyIndex.Lines, first + length, newConfig, first, newConfig.Length - first);

            File.WriteAllLines(filePath, newConfig, encoding);
        }

        /// <summary>
        /// Determines whether the specified key exists in the configuration file.
        /// </summary>
        /// <param name="key">The key that is looked for in the configuration file.</param>
        /// <returns><c>true</c>, if <paramref name="key"/> is defined in the configuration file; otherwise, <c>false</c>.</returns>
        public bool HasKey(string key)
        {
            if (!keyRegex.IsMatch(key))
                return false;

            return findKey(key).Exists;
        }

        /// <summary>
        /// Clears the configuration. This also clear the configuration file.
        /// </summary>
        public void Clear()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// Gets all the key/value pairs in the configuration.
        /// </summary>
        /// <returns>A collection of all key/value pairs in the configuration.</returns>
        public IEnumerable<KeyValuePair<string, string>> GetAll()
        {
            string[] lines = File.Exists(filePath) ? File.ReadAllLines(filePath) : new string[0];
            string section = null;
            for (int i = 0; i < lines.Length; i++)
            {
                var newSection = parseSection(lines[i]);
                if (newSection != null)
                {
                    section = newSection;
                    continue;
                }

                var pair = parseLine(lines[i]);
                yield return new KeyValuePair<string, string>(
                    section == null ? pair.Item1 : (section + "." + pair.Item1), pair.Item2);
            }
        }

        private class KeySearchResult
        {
            private int sectionIndex;
            private int keyIndex;
            private string[] lines;

            private KeySearchResult(int sectionIndex, int keyIndex, string[] lines)
            {
                this.sectionIndex = sectionIndex;
                this.keyIndex = keyIndex;
                this.lines = lines;
            }

            public const int NOSECTION = -1;

            public static KeySearchResult FindKey(string key, string[] lines)
            {
                key = key.ToLower();
                var searchKey = parseKey(key);

                int i = 0;
                for (; i < lines.Length && parseSection(lines[i]) == null; i++)
                {
                    var pair = parseLine(lines[i]);
                    if (pair == null)
                        continue;

                    if (pair.Item1 == key)
                        return new KeySearchResult(NOSECTION, i, lines);
                }

                for (; i < lines.Length && parseSection(lines[i]) != searchKey.Item1; i++) { }

                if (i == lines.Length)
                    return new KeySearchResult(~i, ~(i + 1), lines);

                int sectionIndex = i++;

                for (; i < lines.Length && parseSection(lines[i]) == null; i++)
                {
                    var pair = parseLine(lines[i]);
                    if (pair == null)
                        continue;

                    if (pair.Item1 == searchKey.Item2)
                        return new KeySearchResult(sectionIndex, i, lines);
                }

                return new KeySearchResult(sectionIndex, ~i, lines);
            }

            public bool SectionExists => sectionIndex >= 0;
            public bool Exists => keyIndex >= 0;

            public int SectionIndex => sectionIndex < 0 ? ~sectionIndex : sectionIndex;
            public int KeyIndex => keyIndex < 0 ? ~keyIndex : keyIndex;
            public string[] Lines => lines;
        }
    }
}
