using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class FlagArgumentParser : ArgumentParser
    {
        private string description;

        public FlagArgumentParser(string name)
            : base(name)
        {
            this.description = null;
        }

        internal override string Description
        {
            get { return description; }
        }
        public FlagArgumentParser WithDescription(string description)
        {
            this.description = description;
            return this;
        }
    }
}
