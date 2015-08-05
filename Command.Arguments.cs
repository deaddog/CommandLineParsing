using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    public partial class Command
    {
        private class executor
        {
            private readonly Command command;

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

                msg = parseArguments();
                if (msg.IsError)
                    return msg;

                msg = command.postValid.Validate();
                if (msg.IsError)
                    return msg;

                command.Execute();

                return Message.NoError;
            }

            private Message parseArguments()
            {
                Message msg = Message.NoError;

                while (args.Count > 0)
                {
                    if (RegexLookup.ParameterName.IsMatch(args.Peek()))
                    {
                        Parameter par = findParameter(args.Pop(), out msg);
                        if (msg.IsError)
                            return msg;

                        msg = handleParameter(par);
                        if (msg.IsError)
                            return msg;
                    }
                    else
                    {
                        if (command.Parameters.HasNoName && command.Parameters.NoName.CanHandle(args.Peek()))
                            nonameArgs.Add(args.Pop());
                        else if (RegexLookup.SubcommandName.IsMatch(args.Peek()))
                            return UnknownArgumentMessage.FromSubcommands(command, args.Pop());
                        else
                            return $"You must specify which parameter the value is associated to; ([Example:--parameter {args.Pop()}]).";
                    }
                }

                msg = command.Parameters.FirstOrDefault(x => !x.IsSet && x.IsRequired)?.RequiredMessage ?? Message.NoError;
                if (msg.IsError)
                    return msg;

                if (command.parameters.HasNoName && nonameArgs.Count > 0)
                {
                    msg = command.parameters.NoName.Handle(new Argument(nonameArgs));
                    if (msg.IsError)
                        return msg;
                }

                return Message.NoError;
            }

            private Parameter findParameter(string arg, out Message message)
            {
                Parameter par;
                if (command.Parameters.TryGetParameter(arg, out par))
                {
                    message = Message.NoError;
                    return par;
                }
                else
                {
                    message = UnknownArgumentMessage.FromParameters(command, arg);
                    return null;
                }
            }

            private Message handleParameter(Parameter parameter)
            {
                List<string> values = new List<string>();
                while (args.Count > 0 && !RegexLookup.ParameterName.IsMatch(args.Peek()))
                    if (parameter is FlagParameter && command.Parameters.HasNoName)
                        nonameArgs.Add(args.Pop());
                    else
                        values.Add(args.Pop());

                return parameter.Handle(new Argument(values));
            }
        }
    }
}
