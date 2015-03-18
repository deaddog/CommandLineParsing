using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    public class Argument
    {
        private string key;
        private string[] values;

        public static Stack<Argument> Parse(string[] args)
        {
            return new Stack<Argument>(parse(args).Reverse());
        }
        private static IEnumerable<Argument> parse(string[] args)
        {
            string key = null;
            List<string> values = new List<string>();

            foreach (var a in args)
            {
                bool isKey = RegexLookup.ArgumentName.IsMatch(a);

                if (key == null && !isKey)
                    yield return new Argument(a, new string[0]);

                else if (isKey)
                {
                    if (key != null)
                    {
                        yield return new Argument(key, values);
                        key = null;
                        values.Clear();
                    }
                    key = a;
                }
                else
                    values.Add(a);
            }

            if (key != null)
                yield return new Argument(key, values);
        }

        public Argument(string key, IEnumerable<string> values)
        {
            this.key = key.ToLower();
            this.values = values.ToArray();
        }

        public string Key
        {
            get { return key; }
        }

        public int Count
        {
            get { return values.Length; }
        }

        public string this[int index]
        {
            get { return values[index]; }
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", key, string.Join(", ", values));
        }
    }
}
