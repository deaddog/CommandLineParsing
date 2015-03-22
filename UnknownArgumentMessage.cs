using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class UnknownArgumentMessage : Message
    {
        public enum ArgumentType
        {
            SubCommand,
            Parameter
        }
        private static string argumentTypeString(ArgumentType arg)
        {
            switch (arg)
            {
                case ArgumentType.SubCommand: return "subcommand";
                case ArgumentType.Parameter: return "parameter";
                default:
                    throw new ArgumentOutOfRangeException("arg");
            }
        }

        private string argument;
        private ArgumentType argumentType;
        private Dictionary<string, string> alternativeArguments;

        public UnknownArgumentMessage(string argument, ArgumentType argumentType)
        {
            this.argument = argument;
            this.argumentType = argumentType;
            this.alternativeArguments = new Dictionary<string, string>();
        }

        public void AddAlternative(string alternative, string description)
        {
            if (!this.alternativeArguments.ContainsKey(alternative))
                this.alternativeArguments.Add(alternative, description);
        }

        public void AddAlternative(string[] alternatives, string description)
        {
            AddAlternative(alternatives.OrderByDistance(argument).First().Item1, description);
        }

        public override string GetMessage()
        {
            if (alternativeArguments.Count == 0)
                switch (argumentType)
                {
                    case ArgumentType.SubCommand: return string.Format("The executed command does not support any sub-commands. [[:Yellow:{0}]] is invalid.", argument);
                    case ArgumentType.Parameter: return string.Format("The executed command does not support any parameters. [[:Yellow:{0}]] is invalid.", argument);
                }

            string message = string.Format("{0} [[:Yellow:{1}]] was not recognized. Did you mean any of the following:", argumentTypeString(argumentType), argument);
            var list = alternativeArguments.Keys.OrderByDistance(argument).TakeWhile((arg, i) => i == 0 || arg.Item2 < 5).Select(x => x.Item1).ToArray();
            var strLen = list.Max(x => x.Length);

            foreach (var a in list)
                message += "\n  " + a.PadRight(strLen + 2) + alternativeArguments[a];
            return message;
        }
    }
}
