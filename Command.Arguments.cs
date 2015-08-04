using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    public partial class Command
    {
        private class executor
        {
            private readonly Command command;
            private List<Parameter> unusedParsers;

            private Stack<string> args;
            private List<string> nonameArgs;

            private executor(Command command, Stack<string> args)
            {
                this.command = command;
                this.args = args;
                this.nonameArgs = new List<string>();
            }

            public static Message Execute(Command command, IEnumerable<string> args, string help)
            {
                Stack<string> arguments = new Stack<string>(args.Reverse());
                command = findCommand(command, arguments);

                if (arguments.Count == 1 && arguments.Peek() == help)
                    return command.GetHelpMessage();

                return new executor(command, arguments).execute();
            }

            private static Command findCommand(Command root, Stack<string> args)
            {
                if (args.Count == 0)
                    return root;

                Command res;
                if (RegexLookup.SubcommandName.IsMatch(args.Peek()) && root.subcommands.TryGetCommand(args.Peek(), out res))
                {
                    args.Pop();
                    return findCommand(res, args);
                }

                return root;
            }

            private Message execute()
            {
                Message msg;
                
                msg = command.preValid.Validate();
                if (msg.IsError)
                    return msg;

                unusedParsers = new List<Parameter>(command.parameters);
                while (args.Count > 0)
                {
                    if (RegexLookup.ParameterName.IsMatch(args.Peek()))
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

                msg = command.postValid.Validate();
                if (msg.IsError)
                    return msg;

                command.Execute();

                return Message.NoError;
            }
            
            private Message handleParameter()
            {
                string key = args.Pop();
                List<string> values = new List<string>();

                Parameter parameter;

                if (!command.parameters.TryGetParameter(key, out parameter))
                    return UnknownArgumentMessage.FromParameters(command, key);

                while (args.Count > 0 && !RegexLookup.ParameterName.IsMatch(args.Peek()) &&
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
