using System.Collections.Generic;
using System.Linq;

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

            public static Message Execute(Command command, IEnumerable<string> args, string help)
            {
                return new executor(args).execute(command, help);
            }

            private Message execute(Command command, string help)
            {
                this.command = command;

                if (args.Count > 0 && help == args.Peek())
                    return command.GetHelpMessage();

                if (args.Count > 0 && !RegexLookup.ArgumentName.IsMatch(args.Peek()))
                {
                    string firstArg = args.Pop();

                    if(command.parameters.HasNoName)
                    {
                        if (command.subcommands.ContainsName(firstArg))
                            return executeSubCommand(firstArg, help);
                        else
                            nonameArgs.Add(firstArg);
                    }
                    else
                        return executeSubCommand(firstArg, help);
                }

                Message startvalid = command.ValidateStart();
                if (startvalid.IsError)
                    return startvalid;

                unusedParsers = new List<Parameter>(command.parameters);
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

                if (command.parameters.HasNoName && nonameArgs.Count > 0)
                {
                    var nonameMessage = command.parameters.NoName.Handle(new Argument(nonameArgs));
                    if (nonameMessage.IsError)
                        return nonameMessage;
                }

                var validMessage = command.Validate();
                if (validMessage.IsError)
                    return validMessage;

                command.Execute();

                return Message.NoError;
            }
            private Message executeSubCommand(string arg, string help)
            {
                Command cmd;
                if (command.subcommands.TryGetCommand(arg, out cmd))
                    return execute(cmd, help);
                else
                {
                    UnknownArgumentMessage unknown = new UnknownArgumentMessage(arg, UnknownArgumentMessage.ArgumentType.SubCommand);
                    foreach (var n in command.subcommands)
                        unknown.AddAlternative(n.Key, SUBCOMMAND_DESCRIPTION);
                    return unknown;
                }
            }
            private Message handleParameter()
            {
                string key = args.Pop();
                List<string> values = new List<string>();

                Parameter parameter;

                if (!command.parameters.TryGetParameter(key, out parameter))
                {
                    UnknownArgumentMessage unknown = new UnknownArgumentMessage(key, UnknownArgumentMessage.ArgumentType.Parameter);
                    foreach (var par in command.parameters)
                        unknown.AddAlternative(par.GetNames(true).ToArray(), par.Description);
                    return unknown;
                }

                while (args.Count > 0 && !RegexLookup.ArgumentName.IsMatch(args.Peek()) &&
                    (!command.parameters.HasNoName || parameter.CanHandle(args.Peek())))
                {
                    values.Add(args.Pop());
                }

                unusedParsers.Remove(parameter);
                return parameter.Handle(new Argument(key, values));
            }
        }
    }
}
