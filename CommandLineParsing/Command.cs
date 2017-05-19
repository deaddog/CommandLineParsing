using CommandLineParsing.Consoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private CommandCollection subcommands;
        private ParameterCollection parameters;

        private CommandValidator preValid, postValid;

        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        public Command()
        {
            this.subcommands = new CommandCollection(this);
            this.parameters = new ParameterCollection();

            this.preValid = new CommandValidator();
            this.postValid = new CommandValidator();

            this.description = this.GetType().GetCustomAttribute<Description>()?.description ?? string.Empty;

            this.initializeParameters();
        }

        /// <summary>
        /// Gets the description of this <see cref="Command"/>.
        /// The description is applied when the command is used as a subcommand.
        /// </summary>
        /// <value>
        /// The description of this <see cref="Parameter"/>.
        /// </value>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Simulates a read-evaluate-print-loop. This method is especially effective for debugging purposes.
        /// </summary>
        /// <param name="command">A function that returns a <see cref="Command"/> item to execute.</param>
        /// <param name="prefix">A string that is printed as part of the prompt, simulating a call to the application.</param>
        /// <param name="exit">A keyword that should end the read-evaluate-print-loop.</param>
        /// <param name="help">A string that identifies a keyword that can be used to display the help message for any command/subcommand when executing this <see cref="Command"/>.</param>
        public static void SimulateREPL(Func<Command> command, string prefix = null, string exit = null, string help = null)
        {
            prefix = prefix?.Trim() ?? string.Empty;
            if (prefix == string.Empty)
            {
                prefix = System.Reflection.Assembly.GetEntryAssembly().CodeBase;
                prefix = System.IO.Path.GetFileName(prefix);
                prefix = System.IO.Path.ChangeExtension(prefix, null);
            }
            if (prefix.Length > 0) prefix += " ";
            
            exit = exit?.Trim() ?? "exit";
            if (exit.Length == 0)
                throw new ArgumentException("To end the REPL an exit command must be supplied.", nameof(exit));

            ColorConsole.ActiveConsole.WriteLine($"Input command below (or \"{exit}\" to quit)");

            while (true)
            {
                ColorConsole.ActiveConsole.Write(prefix);

                string input = ColorConsole.ReadLine();

                if (input.Trim() == exit)
                    return;

                command().RunCommand(input, help);

                ColorConsole.ActiveConsole.ResetColor();
                ColorConsole.ActiveConsole.WriteLine();
            }
        }
        /// <summary>
        /// Simulates parsing of a string into an array of values done by the .Net framework before execution of main.
        /// </summary>
        /// <param name="input">The input string that is parsed.</param>
        /// <returns>An array of strings that are the result of splitting <paramref name="input"/>.</returns>
        public static string[] SimulateParse(string input)
        {
            input = input.Trim();

            List<string> res = new List<string>();

            while (input.Length > 0)
            {
                int offset, len;
                char lookfor;
                switch (input[0])
                {
                    case '\"': offset = 1; len = -2; lookfor = '\"'; break;
                    case '\'': offset = 1; len = -2; lookfor = '\''; break;
                    default: offset = 0; len = -1; lookfor = ' '; break;
                }

                int index = input.IndexOf(lookfor, offset);
                if (index == -1)
                {
                    res.Add(input);
                    input = string.Empty;
                }
                else
                {
                    string add = input.Substring(offset, index + len + 1);
                    if (add.Length > 0)
                        res.Add(add);
                    input = input.Substring(index + 1);
                }
            }

            return res.ToArray();
        }

        /// <summary>
        /// Gets a collection of the sub commands associated with this <see cref="Command"/>.
        /// </summary>
        public CommandCollection SubCommands
        {
            get { return subcommands; }
        }
        /// <summary>
        /// Translates <paramref name="alias"/> into <paramref name="replaceby"/> which is injected into the execution of this <see cref="Command"/>.
        /// </summary>
        /// <param name="alias">The name of the alias.</param>
        /// <param name="replaceby">The string that <paramref name="alias"/> should be replace by if <paramref name="alias"/> actually is an alias.</param>
        /// <returns><c>true</c>, if <paramref name="alias"/> is an alias for another string; otherwise, <c>false</c>.</returns>
        protected internal virtual bool HandleAlias(string alias, out string replaceby)
        {
            replaceby = string.Empty;
            return false;
        }
        /// <summary>
        /// Gets a collection of the parameters associated with this <see cref="Command"/>.
        /// </summary>
        public ParameterCollection Parameters
        {
            get { return parameters; }
        }

        /// <summary>
        /// Gets the <see cref="CommandLineParsing.Validator"/> that handles validation for this <see cref="Command"/>.
        /// This validation is executed before handling arguments.
        /// </summary>
        public CommandValidator PreValidator
        {
            get { return preValid; }
        }
        /// <summary>
        /// Gets the <see cref="CommandLineParsing.Validator"/> that handles validation for this <see cref="Command"/>.
        /// This validation is executed after handling arguments.
        /// </summary>
        public CommandValidator Validator
        {
            get { return postValid; }
        }

        /// <summary>
        /// When overridden in a derived class, performs any action that is associated with this <see cref="Command"/>.
        /// </summary>
        protected internal virtual void Execute()
        {
        }

        /// <summary>
        /// Gets a help message for the <see cref="Command"/>.
        /// This method can be overridden to provide a command specific message.
        /// See the <see cref="GetSubCommandsMessage"/> and the <see cref="GetParametersMessage"/> methods.
        /// Messages can be combined using the + (plus) operator.
        /// </summary>
        /// <returns>A help <see cref="Message"/> for this <see cref="Command"/>.</returns>
        protected internal virtual Message GetHelpMessage()
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
                sb.AppendLine(string.Format("{2}{0}  {1}", commands[i].Key.PadRight(len), commands[i].Value.Description, indent));
            if (commands.Length > 0)
                sb.Append(string.Format("{2}{0}  {1}", commands[commands.Length - 1].Key.PadRight(len), commands[commands.Length - 1].Value.Description, indent));

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

        /// <summary>
        /// Executes this <see cref="Command"/> and returns a resulting <see cref="Message"/>.
        /// </summary>
        /// <param name="args">The array of arguments that should be read by this <see cref="Command"/>.</param>
        /// <param name="help">A string that identifies a keyword that can be used to display the help message for any command/subcommand when executing this <see cref="Command"/>.</param>
        /// <returns>A <see cref="Message"/> that is the result of executing this <see cref="Command"/>.</returns>
        public Message ParseAndExecute(string[] args, string help = null)
        {
            return CommandExecutor.Execute(this, args, help);
        }
        /// <summary>
        /// Executes this <see cref="Command"/> and returns a resulting <see cref="Message"/>.
        /// </summary>
        /// <param name="argsAsString">The string of arguments that should be read by this <see cref="Command"/>.
        /// This is transformed into an array of arguments, using semantics similar to those used by the .NET framework.</param>
        /// <param name="help">A string that identifies a keyword that can be used to display the help message for any command/subcommand when executing this <see cref="Command"/>.</param>
        /// <returns>A <see cref="Message"/> that is the result of executing this <see cref="Command"/>.</returns>
        public Message ParseAndExecute(string argsAsString, string help = null)
        {
            return ParseAndExecute(SimulateParse(argsAsString), help);
        }

        /// <summary>
        /// Runs this <see cref="Command"/>, printing the resulting <see cref="Message"/> to standard output (<see cref="Console"/>).
        /// </summary>
        /// <param name="args">The array of arguments that should be read by this <see cref="Command"/>.</param>
        /// <param name="help">A string that identifies a keyword that can be used to display the help message for any command/subcommand when executing this <see cref="Command"/>.</param>
        public void RunCommand(string[] args, string help = null)
        {
            var msg = ParseAndExecute(args, help);

            if (msg.IsError)
                ColorConsole.WriteLine(msg.GetMessage());
        }
        /// <summary>
        /// Runs this <see cref="Command"/>, printing the resulting <see cref="Message"/> to standard output (<see cref="Console"/>).
        /// </summary>
        /// <param name="argsAsString">The string of arguments that should be read by this <see cref="Command"/>.
        /// This is transformed into an array of arguments, using semantics similar to those used by the .NET framework.</param>
        /// <param name="help">A string that identifies a keyword that can be used to display the help message for any command/subcommand when executing this <see cref="Command"/>.</param>
        public void RunCommand(string argsAsString, string help = null)
        {
            RunCommand(SimulateParse(argsAsString), help);
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

                if (!RegexLookup.SubcommandName.IsMatch(name))
                    throw new ArgumentException("Subcommand name \"" + name + "\" is illformed.", nameof(name));

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
            /// Adds an <see cref="Action"/> and a <see cref="Func{Message}"/> to the <see cref="CommandCollection"/>, representing the <see cref="Command.Execute"/> method and a validation method.
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

                public ActionCommand(Action action, Func<Message> validation)
                {
                    this.action = action;
                    if (validation != null)
                        this.postValid.Add(validation);
                }

                protected internal override void Execute()
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
