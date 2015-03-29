using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public partial class Command
    {
        private Message execute(Stack<Argument> argumentStack)
        {
            if (argumentStack.Count > 0 && !argumentStack.Peek().Key.StartsWith("-"))
            {
                var first = argumentStack.Pop();
                Command cmd;

                if (subcommands.TryGetCommand(first.Key, out cmd))
                    return cmd.execute(argumentStack);
                else
                {
                    UnknownArgumentMessage unknown = new UnknownArgumentMessage(first.Key, UnknownArgumentMessage.ArgumentType.SubCommand);
                    foreach (var n in subcommands.CommandNames)
                        unknown.AddAlternative(n, "N/A - Commands have no description.");
                    return unknown;
                }
            }

            Message startvalid = ValidateStart();
            if (startvalid.IsError)
                return startvalid;

            var unusedParsers = new List<Parameter>(parsers);
            var required = unusedParsers.Where(x => x.IsRequired);
            while (argumentStack.Count > 0)
            {
                var arg = argumentStack.Pop();
                Parameter parameter;
                if (!parameters.TryGetValue(arg.Key, out parameter))
                {
                    UnknownArgumentMessage unknown = new UnknownArgumentMessage(arg.Key, UnknownArgumentMessage.ArgumentType.Parameter);
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

            if (required.Any())
                return required.First().RequiredMessage;

            var validMessage = Validate();
            if (validMessage.IsError)
                return validMessage;

            Execute();

            return Message.NoError;
        }

        private class executor
        {
            private Command command;
            private Stack<string> args;
            private List<string> nonameArgs;

            private executor(IEnumerable<string> args)
            {
                this.command = null;
                this.args = new Stack<string>(args.Reverse());
                this.nonameArgs = new List<string>();
            }

            public static Message Execute(Command command, IEnumerable<string> args)
            {
                return new executor(args).execute(command);
            }

            private Message execute(Command command)
            {
                this.command = command;

                if (!RegexLookup.ArgumentName.IsMatch(args.Peek()))
                {
                    if (command.hasNoName) nonameArgs.Add(args.Pop());
                    else return executeSubCommand();
                }

                Message startvalid = command.ValidateStart();
                if (startvalid.IsError)
                    return startvalid;

                throw new NotImplementedException();
            }
            private Message executeSubCommand()
            {
                string a = args.Pop();
                Command cmd;
                if (command.subcommands.TryGetCommand(a, out cmd))
                    return execute(cmd);
                else
                {
                    UnknownArgumentMessage unknown = new UnknownArgumentMessage(a, UnknownArgumentMessage.ArgumentType.SubCommand);
                    foreach (var n in command.subcommands.CommandNames)
                        unknown.AddAlternative(n, "N/A - Commands have no description.");
                    return unknown;
                }
            }
            private Message handleParameter()
            {
                string key = args.Pop();
                List<string> values = new List<string>();

                Parameter parameter;

                if (!command.parameters.TryGetValue(key, out parameter))
                {
                    UnknownArgumentMessage unknown = new UnknownArgumentMessage(key, UnknownArgumentMessage.ArgumentType.Parameter);
                    var g = command.parameters.GroupBy(x => x.Value, x => x.Key).Select(x => x.ToArray());
                    foreach (var a in g)
                        unknown.AddAlternative(a, command.parameters[a[0]].Description);
                    return unknown;
                }

                while (args.Count > 0 && !RegexLookup.ArgumentName.IsMatch(args.Peek()))
                {
                    string value = args.Pop();
                    if (!command.hasNoName || parameter.CanHandle(value))
                        values.Add(value);
                    else
                        nonameArgs.Add(value);
                }

                unusedParsers.Remove(parameter);
                return parameter.Handle(new Argument(key, values));
            }
        }
    }
}
