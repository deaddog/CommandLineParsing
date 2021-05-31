using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTools.Applications
{
    public class Command
    {
        public static Command Create()
        {
            return new Command
            (
                commands: ImmutableDictionary<string, Command>.Empty,
                parameters: ImmutableArray<IParameter>.Empty,
                execution: null
            );
        }

        public Command(IImmutableDictionary<string, Command> commands, ImmutableArray<IParameter> parameters, IExecution execution)
        {
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));
            Parameters = parameters;
            Execution = execution ?? throw new ArgumentNullException(nameof(execution));
        }

        public IImmutableDictionary<string, Command> Commands { get; }
        public ImmutableArray<IParameter> Parameters { get; }
        public IExecution Execution { get; }
    }

    public static class CommandExtensions
    {
        public static async Task RunAsync(this Command command, IConsole console, IArgumentsParser parser, string[] args)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));
            if (console is null)
                throw new ArgumentNullException(nameof(console));

            try
            {
                var (cmd, arguments) = parser.Parse(command, args.ToImmutableArray());

                await cmd.Execution.ExecuteAsync(console, arguments, CancellationToken.None);
            }
            catch (MessageException exception)
            {
                console.WriteLine(exception.Message);
            }
        }
    }

    public interface IExecution
    {
        Task ExecuteAsync(IConsole console, ArgumentSet args, CancellationToken cancellationToken);
    }
}
