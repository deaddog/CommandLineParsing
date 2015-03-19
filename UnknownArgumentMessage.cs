using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class UnknownArgumentMessage : Message
    {
        private string argument;
        private Dictionary<string, string> alternativeArguments;

        public UnknownArgumentMessage(string argument)
        {
            this.argument = argument;
            this.alternativeArguments = new Dictionary<string, string>();
        }

        public void AddAlternative(string alternative, string description)
        {
            if (!this.alternativeArguments.ContainsKey(alternative))
                this.alternativeArguments.Add(alternative, description);
        }

        public override string GetMessage()
        {
            string message = string.Format("Argument [[:Yellow:{0}]] was not recognized. Did you mean any of the following:", argument);
            var list = alternativeArguments.Keys.OrderByDistance(argument).TakeWhile((arg, i) => i == 0 || arg.Item2 < 5).Select(x=>x.Item1).ToArray();
            var strLen = list.Max(x => x.Length);

            foreach (var a in list)
                message += "\n  " + a.PadRight(strLen + 4) + alternativeArguments[a];
            return message;
        }
    }
}
