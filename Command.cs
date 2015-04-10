﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides functionality for defining commands with parameters.
    /// Parameters are defined by declaring them in <see cref="Command"/> subtypes.
    /// Parameters must be declared as readonly and cannot be initialized (no constructor).
    /// Initialization is handled automatically.
    /// </summary>
    public abstract partial class Command
    {
        private const string SUBCOMMAND_DESCRIPTION = "N/A - Commands have no description.";

        private CommandCollection subcommands;
        private ParameterCollection parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        public Command()
        {
            this.subcommands = new CommandCollection(this);
            this.parameters = new ParameterCollection();

            this.initializeParameters();
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

                command().RunCommand(input, help);

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

        /// <summary>
        /// Gets a help message for the <see cref="Command"/>.
        /// This method can be overridden to provide a command specific message.
        /// See the <see cref="GetSubCommandsMessage"/> and the <see cref="GetParametersMessage"/> methods.
        /// Messages can be combined using the + (plus) operator.
        /// </summary>
        /// <returns>A help <see cref="Message"/> for this <see cref="Command"/>.</returns>
        protected virtual Message GetHelpMessage()
        {
            var message = Message.NoError;

            if (subcommands.Any())
                message += "Commands:" + GetSubCommandsMessage(2);

            if (parameters.Any())
                message += "Parameters:" + GetParametersMessage(2);

            return message;
        }

        /// <summary>
        /// Gets a <see cref="Message"/> containing lines with each subcommand name and description.
        /// </summary>
        /// <param name="indentation">The number of indentation spaces that should be applied to each line.</param>
        /// <returns>A <see cref="Message"/> that represents a complete description of all subcommands associated with this <see cref="Command"/>.</returns>
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
        /// <summary>
        /// Gets a <see cref="Message"/> containing lines with each parameter name, alternatives and description.
        /// </summary>
        /// <param name="indentation">The number of indentation spaces that should be applied to each line.</param>
        /// <returns>A <see cref="Message"/> that represents a complete description of all parameters associated with this <see cref="Command"/>.</returns>
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

        public void RunCommand(string[] args, string help = null)
        {
            var msg = ParseAndExecute(args, help);

            if (msg.IsError)
                ColorConsole.WriteLine(msg.GetMessage());
        }
        public void RunCommand(string argsAsString, string help = null)
        {
            RunCommand(simulateParse(argsAsString), help);
        }

        /// <summary>
        /// Provides a collection of the subcommands included in a <see cref="Command"/>.
        /// </summary>
        public class CommandCollection : IEnumerable<KeyValuePair<string, Command>>
        {
            private Command owner;
            private Dictionary<string, Command> commands;

            internal CommandCollection(Command owner)
            {
                this.owner = owner;
                this.commands = new Dictionary<string, Command>();
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="CommandCollection"/> is empty.
            /// </summary>
            /// <value>
            ///   <c>true</c> if empty; otherwise, <c>false</c>.
            /// </value>
            public bool Empty
            {
                get { return commands.Count == 0; }
            }

            /// <summary>
            /// Determines whether the <see cref="CommandCollection"/> contains a <see cref="Command"/> named <paramref name="name"/>.
            /// </summary>
            /// <param name="name">The name to search for.</param>
            /// <returns><c>true</c> if this instance contains a <see cref="Command"/> associated with <paramref name="name"/>; otherwise, <c>false</c>.</returns>
            public bool ContainsName(string name)
            {
                return commands.ContainsKey(name);
            }
            /// <summary>
            /// Gets the <see cref="Command"/> that is associated with <paramref name="name"/>.
            /// </summary>
            /// <param name="name">The name to search for.</param>
            /// <param name="command">When the method returns, contains the <see cref="Command"/> associated with <paramref name="name"/>, if such a <see cref="Command"/> exists; otherwise, <c>null</c>.</param>
            /// <returns><c>true</c> if this instance contains a <see cref="Command"/> associated with <paramref name="name"/>; otherwise, <c>false</c>.</returns>
            public bool TryGetCommand(string name, out Command command)
            {
                return commands.TryGetValue(name, out command);
            }

            /// <summary>
            /// Adds a <see cref="Command"/> to the <see cref="CommandCollection"/>.
            /// </summary>
            /// <param name="name">The name that <paramref name="command"/> should be associated with as a subcommand.</param>
            /// <param name="command">The command that is added to the <see cref="CommandCollection"/>.</param>
            public void Add(string name, Command command)
            {
                if (name == null)
                    throw new ArgumentNullException("name");
                if (command == null)
                    throw new ArgumentNullException("command");

                this.commands.Add(name, command);
            }
            /// <summary>
            /// Adds an <see cref="Action"/> to the <see cref="CommandCollection"/>, representing the <see cref="Command.Execute"/> method.
            /// </summary>
            /// <param name="name">The name that <paramref name="action"/> should be associated with as a subcommand.</param>
            /// <param name="action">The method that should be executed when the subcommand is executed.</param>
            public void Add(string name, Action action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");

                Add(name, new ActionCommand(action, null));
            }
            /// <summary>
            /// Adds an <see cref="Action"/> and a <see cref="Func{Message}"/> to the <see cref="CommandCollection"/>, representing the <see cref="Command.Execute"/> and <see cref="Command.Validate"/> methods.
            /// </summary>
            /// <param name="name">The name that the two methods should be associated with as a subcommand.</param>
            /// <param name="action">The method that should be executed when the subcommand is executed.</param>
            /// <param name="validation">The method that should be executed when the subcommand is validated. Use <see cref="Message.NoError"/> to indicate validation success.</param>
            public void Add(string name, Action action, Func<Message> validation)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                if (validation == null)
                    throw new ArgumentNullException("validation");

                Add(name, new ActionCommand(action, validation));
            }

            private class ActionCommand : Command
            {
                private Action action;
                private Func<Message> validation;

                public ActionCommand(Action action, Func<Message> validation)
                {
                    this.action = action;
                    this.validation = validation;
                }

                protected override Message Validate()
                {
                    if (validation != null)
                        return validation();
                    else
                        return base.Validate();
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

        /// <summary>
        /// Provides a collection of the parameters included in a <see cref="Command"/>.
        /// </summary>
        public class ParameterCollection : IEnumerable<Parameter>
        {
            private Parameter noName;
            /// <summary>
            /// Gets a value indicating whether this instance has an unnamed parameter.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a no name parameter; otherwise, <c>false</c>.
            /// </value>
            public bool HasNoName
            {
                get { return noName != null; }
            }
            /// <summary>
            /// Gets the unnamed parameter associated with this <see cref="ParameterCollection"/>.
            /// </summary>
            /// <value>
            /// The unnamed parameter associated with this <see cref="ParameterCollection"/> if one exists; otherwise, <c>null</c>.
            /// </value>
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

            /// <summary>
            /// Determines whether the <see cref="ParameterCollection"/> contains a <see cref="Parameter"/> associated with <paramref name="argument"/>.
            /// </summary>
            /// <param name="argument">The argument to search for.</param>
            /// <returns><c>true</c> if this instance contains a <see cref="Parameter"/> associated with <paramref name="argument"/>; otherwise, <c>false</c>.</returns>
            public bool ContainsKey(string argument)
            {
                return parameters.ContainsKey(argument);
            }
            /// <summary>
            /// Gets the <see cref="Parameter"/> that is associated with <paramref name="argument"/>.
            /// </summary>
            /// <param name="argument">The argument to search for.</param>
            /// <param name="parameter">When the method returns, contains the <see cref="Parameter"/> associated with <paramref name="argument"/>, if such a <see cref="Parameter"/> exists; otherwise, <c>null</c>.</param>
            /// <returns><c>true</c> if this instance contains a <see cref="Parameter"/> associated with <paramref name="argument"/>; otherwise, <c>false</c>.</returns>
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
