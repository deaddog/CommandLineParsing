using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public abstract partial class Command
    {
        private const string SUBCOMMAND_DESCRIPTION = "N/A - Commands have no description.";

        private CommandCollection subcommands;
        private ParameterCollection parameters;

        public Command()
        {
            this.subcommands = new CommandCollection(this);
            this.parameters = new ParameterCollection();

            this.initializeParameters();
        }

        public static void RunCommand(Command command, string[] args, string help = null)
        {
            var msg = command.ParseAndExecute(args, help);

            if (msg.IsError)
                ColorConsole.WriteLine(msg.GetMessage());
        }
        public static void RunCommand(Command command, string argsAsString, string help = null)
        {
            RunCommand(command, simulateParse(argsAsString), help);
        }

        public static void SimulateREPL(Func<Command> command, string exit, string help = null)
        {
            if (exit == null)
                throw new ArgumentNullException("exit");

            exit = exit.Trim();
            if (exit.Length == 0)
                throw new ArgumentException("To end the REPL an exit command must be supplied.", "exit");

            while (true)
            {
                Console.Write("Input command (or \"{0}\" to quit): ", exit);

                string input = Console.ReadLine();

                if (input.Trim() == exit)
                    return;

                RunCommand(command(), input, help);

                Console.ResetColor();
                Console.WriteLine();
            }
        }
        private static string[] simulateParse(string input)
        {
            input = input.Trim();

            var matches = System.Text.RegularExpressions.Regex.Matches(input, "[^ \"]+|\"[^\"]+\"");
            string[] inputArr = new string[matches.Count];
            for (int i = 0; i < inputArr.Length; i++)
            {
                inputArr[i] = matches[i].Value;
                if (inputArr[i][0] == '\"' && inputArr[i][inputArr[i].Length - 1] == '\"')
                    inputArr[i] = inputArr[i].Substring(1, inputArr[i].Length - 2);
            }
            return inputArr;
        }

        public CommandCollection SubCommands
        {
            get { return subcommands; }
        }
        public ParameterCollection Parameters
        {
            get { return parameters; }
        }

        protected Message ValidateEach<T>(IEnumerable<T> collection, Func<T, Message> validator)
        {
            foreach (var t in collection)
            {
                var msg = validator(t);
                if (msg.IsError)
                    return msg;
            }
            return Message.NoError;
        }
        protected Message ValidateEach<T>(IEnumerable<T> collection, Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            return ValidateEach(collection, x => validator(x) ? Message.NoError : errorMessage(x));
        }
        protected Message ValidateEach<T>(IEnumerable<T> collection, Func<T, bool> validator, Message errorMessage)
        {
            return ValidateEach(collection, x => validator(x) ? Message.NoError : errorMessage);
        }

        protected virtual Message ValidateStart()
        {
            return Message.NoError;
        }
        protected virtual Message Validate()
        {
            return Message.NoError;
        }
        protected virtual void Execute()
        {
        }

        protected virtual Message GetHelpMessage()
        {
            var message = Message.NoError;

            if (subcommands.Any())
                message += "Commands:" + GetSubCommandsMessage(2);

            if (parameters.Any())
                message += "Parameters:" + GetParametersMessage(2);

            return message;
        }

        protected Message GetSubCommandsMessage(int indentation)
        {
            StringBuilder sb = new StringBuilder();

            string indent = "".PadLeft(indentation);
            int len = subcommands.Select(x => x.Key.Length).Max();

            var commands = subcommands.ToArray();
            for (int i = 0; i < commands.Length - 1; i++)
                sb.AppendLine(string.Format("{2}{0}  {1}", commands[i].Key.PadRight(len), SUBCOMMAND_DESCRIPTION, indent));
            if (commands.Length > 0)
                sb.Append(string.Format("{2}{0}  {1}", commands[commands.Length - 1].Key.PadRight(len), SUBCOMMAND_DESCRIPTION, indent));

            return sb.ToString();
        }
        protected Message GetParametersMessage(int indentation)
        {
            StringBuilder sb = new StringBuilder();

            string indent = "".PadLeft(indentation);
            var pars = (from pp in parameters
                        let n = string.Join(", ", pp.GetNames(true))
                        select Tuple.Create(n, pp.Description)).ToArray();

            int len = pars.Select(x => x.Item1.Length).Max();
            for (int i = 0; i < pars.Length - 1; i++)
                sb.AppendLine(string.Format("{2}{0}  {1}", pars[i].Item1.PadRight(len), pars[i].Item2, indent));
            if (pars.Length > 0)
                sb.AppendLine(string.Format("{2}{0}  {1}", pars[pars.Length - 1].Item1.PadRight(len), pars[pars.Length - 1].Item2, indent));

            return sb.ToString();
        }

        public Message ParseAndExecute(string[] args, string help = null)
        {
            return executor.Execute(this, args, help);
        }
        public Message ParseAndExecute(string argsAsString, string help = null)
        {
            return ParseAndExecute(simulateParse(argsAsString), help);
        }

        public class CommandCollection : IEnumerable<KeyValuePair<string, Command>>
        {
            private Command owner;
            private Dictionary<string, Command> commands;

            internal CommandCollection(Command owner)
            {
                this.owner = owner;
                this.commands = new Dictionary<string, Command>();
            }

            internal string[] CommandNames
            {
                get { return commands.Keys.ToArray(); }
            }
            public bool Empty
            {
                get { return commands.Count == 0; }
            }

            internal bool ContainsName(string name)
            {
                return commands.ContainsKey(name);
            }
            internal bool TryGetCommand(string name, out Command command)
            {
                return commands.TryGetValue(name, out command);
            }

            public void Add(string name, Command command)
            {
                if (name == null)
                    throw new ArgumentNullException("name");
                if (command == null)
                    throw new ArgumentNullException("command");

                this.commands.Add(name, command);
            }
            public void Add(string name, Action action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");

                Add(name, new ActionCommand(action));
            }

            private class ActionCommand : Command
            {
                private Action action;

                public ActionCommand(Action action)
                {
                    if (action == null)
                        throw new ArgumentNullException("action");

                    this.action = action;
                }

                protected override void Execute()
                {
                    action();
                }
            }

            IEnumerator<KeyValuePair<string, Command>> IEnumerable<KeyValuePair<string, Command>>.GetEnumerator()
            {
                foreach (var cmd in commands)
                    yield return cmd;
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                foreach (var cmd in commands)
                    yield return cmd;
            }
        }

        public class ParameterCollection : IEnumerable<Parameter>
        {
            private Parameter noName;
            public bool HasNoName
            {
                get { return noName != null; }
            }
            public Parameter NoName
            {
                get { return noName; }
            }

            private Dictionary<string, Parameter> parameters;
            private List<Parameter> parsers;

            internal ParameterCollection()
            {
                this.parameters = new Dictionary<string, Parameter>();
                this.parsers = new List<Parameter>();
            }
            internal void Add(Parameter parameter)
            {
                if (!parameter.Unnamed)
                {
                    parsers.Add(parameter);
                    foreach (var name in parameter.GetNames(true))
                        parameters.Add(name, parameter);
                }
                else
                    noName = parameter;
            }

            public bool TryGetParameter(string argument, out Parameter parameter)
            {
                return parameters.TryGetValue(argument, out parameter);
            }

            IEnumerator<Parameter> IEnumerable<Parameter>.GetEnumerator()
            {
                foreach (var p in parsers)
                    yield return p;
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                foreach (var p in parsers)
                    yield return p;
            }
        }
    }
}
