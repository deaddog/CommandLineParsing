using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public abstract partial class Command
    {
        private Dictionary<string, Parameter> parameters;
        private List<Parameter> parsers;

        private SubCommandCollection subcommands;

        public Command()
        {
            this.parameters = new Dictionary<string, Parameter>();
            this.parsers = new List<Parameter>();

            this.subcommands = new SubCommandCollection();

            this.initializeParameters();
        }

        public SubCommandCollection SubCommands
        {
            get { return subcommands; }
        }

        protected virtual Message Validate()
        {
            return Message.NoError;
        }
        protected virtual void Execute()
        {
        }

        public Message ParseAndExecute(string[] args)
        {
            var argumentStack = CommandLineParsing.Argument.Parse(args);

            return execute(argumentStack);
        }

        private Message execute(Stack<Argument> argumentStack)
        {
            var unusedParsers = new List<Parameter>(parsers.Where(x => x.IsRequired));
            while (argumentStack.Count > 0)
            {
                var arg = argumentStack.Pop();
                Parameter parameter;
                if (!parameters.TryGetValue(arg.Key, out parameter))
                {
                    UnknownArgumentMessage unknown = new UnknownArgumentMessage(arg.Key);
                    var g = parameters.GroupBy(x => x.Value, x => x.Key).Select(x => x.ToArray());
                    foreach (var a in g)
                        unknown.AddAlternative(a, parameters[a[0]].Description);
                    return unknown;
                }

                unusedParsers.Remove(parameter);
                var msg = parameter.Handle(arg);

                if (msg.IsError)
                    return msg;
            }

            if (unusedParsers.Count > 0)
                return unusedParsers[0].RequiredMessage;

            var validMessage = Validate();
            if (validMessage.IsError)
                return validMessage;

            Execute();

            return Message.NoError;
        }

        public class SubCommandCollection
        {
            private Dictionary<string, Command> commands;

            public SubCommandCollection()
            {
                this.commands = new Dictionary<string, Command>();
            }

            internal string[] CommandNames
            {
                get { return commands.Keys.ToArray(); }
            }

            internal bool TryGetCommand(string name, out Command command)
            {
                return commands.TryGetValue(name, out command);
            }

            public void Add(string name, Command command)
            {
                this.commands.Add(name, command);
            }
        }
    }
}
