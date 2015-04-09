using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    internal class Argument
    {
        private string key;
        private string[] values;

        public Argument(IEnumerable<string> values)
        {
            this.key = null;
            this.values = values.ToArray();
        }
        public Argument(string key, IEnumerable<string> values)
        {
            if (key == null)
                throw new ArgumentNullException("key");

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
