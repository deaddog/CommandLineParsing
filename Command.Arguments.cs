using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public partial class Command
    {
        private class executor
        {
            private Command command;
            private List<Parameter> unusedParsers;

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

                unusedParsers = new List<Parameter>(command.parsers);
                while (args.Count > 0)
                {
                    if (RegexLookup.ArgumentName.IsMatch(args.Peek()))
                    {
                        Message parMessage = handleParameter();
                        if (parMessage.IsError)
                            return parMessage;
                    }
                    else
                        nonameArgs.Add(args.Pop());
                }

                var req = unusedParsers.FirstOrDefault(x => x.IsRequired);
                if (req != null)
                    return req.RequiredMessage;

                if (command.hasNoName)
                {
                    var nonameMessage = command.noName.Handle(new Argument(nonameArgs));
                    if (nonameMessage.IsError)
                        return nonameMessage;
                }

                var validMessage = command.Validate();
                if (validMessage.IsError)
                    return validMessage;

                command.Execute();

                return Message.NoError;
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

                while (args.Count > 0 && !RegexLookup.ArgumentName.IsMatch(args.Peek()) &&
                    (!command.hasNoName || parameter.CanHandle(args.Peek())))
                {
                    values.Add(args.Pop());
                }

                unusedParsers.Remove(parameter);
                return parameter.Handle(new Argument(key, values));
            }
        }
    }
}
