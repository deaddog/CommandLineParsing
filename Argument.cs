using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    internal class Argument
    {
        private string[] values;

        public Argument(IEnumerable<string> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            this.values = values.ToArray();
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
            return $"Count: {Count}";
        }
    }
}
