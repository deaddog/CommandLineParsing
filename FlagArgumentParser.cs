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

        private Func<string, Message> hasValueMessage;

        public FlagArgumentParser(string name)
            : base(name)
        {
            this.description = null;
            this.callback = null;

            this.hasValueMessage = x => x + " is a flag argument, it does not support values.";
        }

        internal override Message Handle(Argument argument)
        {
            if (argument.Count > 0)
                    return hasValueMessage(argument.Key);
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

        public FlagArgumentParser HasValue(Message errorMessage)
        {
            return HasValue(x => errorMessage);
        }
        public FlagArgumentParser HasValue(Func<string, Message> errorMessage)
        {
            hasValueMessage = errorMessage;
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
