using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public abstract partial class Command
    {
        private Parameter noName;
        private bool hasNoName
        {
            get { return noName != null; }
        }

        private Dictionary<string, Parameter> parameters;
        private List<Parameter> parsers;

        private CommandCollection subcommands;

        public Command()
        {
            this.parameters = new Dictionary<string, Parameter>();
            this.parsers = new List<Parameter>();

            this.subcommands = new CommandCollection();

            this.initializeParameters();
        }

        public static void RunCommand(Command command, string[] args)
        {
            var msg = command.ParseAndExecute(args);

            if (msg.IsError)
                ColorConsole.WriteLine(msg.GetMessage());
        }
        public static void RunCommand(Command command, string argsAsString)
        {
            RunCommand(command, simulateParse(argsAsString));
        }

        public static void SimulateREPL(Func<Command> command, string exit)
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

                RunCommand(command(), input);

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

        public Message ParseAndExecute(string[] args)
        {
            var argumentStack = CommandLineParsing.Argument.Parse(args);

            return execute(argumentStack);
        }
        public Message ParseAndExecute(string argsAsString)
        {
            return ParseAndExecute(simulateParse(argsAsString));
        }

        public class CommandCollection
        {
            private Dictionary<string, Command> commands;

            public CommandCollection()
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
        }
    }
}
