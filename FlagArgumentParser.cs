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
        private Action callback;

        public FlagArgumentParser(string name)
            : base(name)
        {
            this.description = null;
            this.callback = null;
        }

        internal override Message Handle(Argument argument)
        {
            if (callback != null)
                callback();

            return Message.NoError;
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
        public FlagArgumentParser Callback(Action callback)
        {
            this.callback += callback;
            return this;
        }

        internal override bool IsRequired
        {
            get { return false; }
        }
        internal override Message RequiredMessage
        {
            get { return Message.NoError; }
        }
    }
}
