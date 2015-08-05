using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    internal class CommandExecutor
    {
        private readonly Command command;
        private readonly ArgumentQueue arguments;

        private CommandExecutor(Command command, ArgumentQueue arguments)
        {
            this.command = command;
            this.arguments = null;
        }

        public static Message Execute(Command command, IEnumerable<string> args, string help)
        {
            ArgumentQueue arguments = new ArgumentQueue(args);
            command = findCommand(command, arguments);

            if (arguments.Count == 1 && arguments.Peek == help)
                return command.GetHelpMessage();

            return new CommandExecutor(command, arguments).execute();
        }

        private static Command findCommand(Command root, ArgumentQueue args)
        {
            if (args.Count == 0)
                return root;

            Command res;
            if (RegexLookup.SubcommandName.IsMatch(args.Peek) && root.SubCommands.TryGetCommand(args.Peek, out res))
            {
                args.Dequeue();
                return findCommand(res, args);
            }

            return root;
        }

        private Message NoUnnamed(string value) => $@"You must specify which parameter the value '{value}' is associated with; ([Example:--\[parameter-name\] {value}]).";

        private Message execute()
        {
            Message msg;

            msg = command.PreValidator.Validate();
            if (msg.IsError)
                return msg;

            msg = parseArguments();
            if (msg.IsError)
                return msg;

            msg = command.Validator.Validate();
            if (msg.IsError)
                return msg;

            command.Execute();

            return Message.NoError;
        }

        private Message parseArguments()
        {
            Message msg = Message.NoError;

            while (arguments.Count > 0)
            {
                if (RegexLookup.ParameterName.IsMatch(arguments.Peek))
                {
                    Parameter par = findParameter(command, arguments.Dequeue(), out msg);
                    if (msg.IsError)
                        return msg;

                    msg = handleParameter(par);
                    if (msg.IsError)
                        return msg;
                }
                else
                {
                    if (command.Parameters.HasNoName && command.Parameters.NoName.CanHandle(arguments.Peek))
                        arguments.Skip();
                    else if (RegexLookup.SubcommandName.IsMatch(arguments.Peek))
                        return UnknownArgumentMessage.FromSubcommands(command, arguments.Dequeue());
                    else
                        return NoUnnamed(arguments.Dequeue());
                }
            }

            msg = command.Parameters.FirstOrDefault(x => !x.IsSet && x.IsRequired)?.RequiredMessage ?? Message.NoError;
            if (msg.IsError)
                return msg;

            string[] nonameArgs = arguments.PopSkipped();
            if (nonameArgs.Length > 0)
                if (command.Parameters.HasNoName)
                {
                    msg = command.Parameters.NoName.Handle(nonameArgs);
                    if (msg.IsError)
                        return msg;
                }
                else
                    return NoUnnamed(nonameArgs[0]);

            return Message.NoError;
        }

        private static Parameter findParameter(Command command, string arg, out Message message)
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
            while (arguments.Count > 0 && !RegexLookup.ParameterName.IsMatch(arguments.Peek))
                if (parameter is FlagParameter && command.Parameters.HasNoName)
                    arguments.Skip();
                else
                    arguments.Accept();

            return parameter.Handle(arguments.PopAccepted());
        }
    }
}
