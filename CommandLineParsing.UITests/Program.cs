using CommandLineParsing.Input.Reading;
using CommandLineParsing.Output.Formatting;
using CommandLineParsing.Output.Formatting.Structure;
using CommandLineParsing.Parsing;
using CommandLineParsing.Validation;
using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLineParsing.UITests
{
    class Program
    {
        static readonly SharedConsole _console = new SharedConsole();

        static void Main(string[] args)
        {
            // Setting the active console to the shared one, use _console.BufferStrings and _console.WindowStrings to inspect the state.
            IConsole c = Consoles.System;

            // gh issues 1 2 3 --list -f '$name'

            // null > [issues 1 2 3]
            // list > []
            // f    > ['$name']

            // Do stuff using the console, by interacting with ColorConsole.

            var cmd = Command.Create()
                .WithParameter<string>("name", out var name, p => p
                    .Where(x => x.Length > 20)
                    /*.Required()*/)
                .WithParameter<int>("age", out var age)
                .WithParameter<int>("age2", out _)
                .OnExecute(args =>
                {
                    var myName = args.Get(name);
                    Console.WriteLine($"Hello {myName.Value}");
                })
                .OnExecute(async (args, ct) => await Task.Delay(1000, ct))
                .OnExecute(args => ExecuteMyThing(args.Get(name), args.Get(age)))
                ;
        }

        static async Task ExecuteMyThing(IArgument<string> name, IArgument<int> age)
        {

        }
    }

    public class Command
    {
        public static Command Create()
        {
            return new Command
            (
                parameters: ImmutableDictionary<string, IParameter>.Empty,
                execution: (args, ct) => Task.CompletedTask
            );
        }

        public Command(IImmutableDictionary<string, IParameter> parameters, Func<IArgumentSet, CancellationToken, Task> execution)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Execution = execution ?? throw new ArgumentNullException(nameof(execution));
        }

        public IImmutableDictionary<string, IParameter> Parameters { get; }
        public Func<IArgumentSet, CancellationToken, Task> Execution { get; }
    }

    public static class CommandExtensions
    {
        public static Command WithParameter<T>(this Command command, string name, Parameter<T> parameter)
        {
            return new Command
            (
                parameters: command.Parameters.Add(name, parameter),
                execution: command.Execution
            );
        }

        public static Command WithParameter<T>(this Command command, string name, out Parameter<T> parameter)
        {
            return WithParameter(command, name, out parameter, p => p);
        }
        public static Command WithParameter<T>(this Command command, string name, out Parameter<T> parameter, Func<Parameter<T>, Parameter<T>> builder)
        {
            return WithParameter(command, name, parameter = builder(Parameter.Create<T>()));
        }

        public static Command OnExecute(this Command command, Action<IArgumentSet> action)
        {
            return new Command
            (
                parameters: command.Parameters,
                execution: (args, ct) => command.Execution.Invoke(args, ct).ContinueWith(_ => action(args))
            );
        }
        public static Command OnExecute(this Command command, Func<IArgumentSet, Task> action)
        {
            return new Command
            (
                parameters: command.Parameters,
                execution: (args, ct) => command.Execution.Invoke(args, ct).ContinueWith(_ => action(args)).Unwrap()
            );
        }
        public static Command OnExecute(this Command command, Func<IArgumentSet, CancellationToken, Task> action)
        {
            return new Command
            (
                parameters: command.Parameters,
                execution: (args, ct) => command.Execution.Invoke(args, ct).ContinueWith(_ => action(args, ct)).Unwrap()
            );
        }
    }

    public static class Parameter
    {
        public static Parameter<T> Create<T>() => null;
    }

    public class Parameter<T> : IParameter, IValidatorComposer<T, Parameter<T>>, IParserComposer<T, Parameter<T>>
    {
        public IValidator<T> Validator => throw new NotImplementedException();
        public IParser<T> Parser => throw new NotImplementedException();

        public Parameter<T> WithParser(IParser<T> parser)
        {
            throw new NotImplementedException();
        }
        public Parameter<T> WithValidator(IValidator<T> validator)
        {
            throw new NotImplementedException();
        }

        public IArgument<T> Resolve(ImmutableArray<string> args)
        {
            throw new NotImplementedException();
        }
        IArgument IParameter.Resolve(ImmutableArray<string> args)
        {
            throw new NotImplementedException();
        }
    }

    public interface IParameter
    {
        IArgument Resolve(ImmutableArray<string> args);
    }

    public interface IArgument
    {
        IParameter Parameter { get; }
    }
    public interface IArgument<T> : IArgument
    {
        T Value { get; set; }
    }
    public interface IArgumentSet
    {
        ImmutableArray<string> GetNames();

        Type GetValueType(string name);
        IArgument<T> GetByName<T>(string name);
    }

    public interface IArgumentsParser
    {
        (Command Command, IImmutableDictionary<string, IArgument> Arguments) Parse(Command command, ImmutableArray<string> arguments);
    }

    public static class ArgumentSetExtensions
    {
        public static IArgument Get(this IArgumentSet set, IParameter parameter)
        {
            return null;
            //return set.GetByName<T>(null);
        }
        public static IArgument<T> Get<T>(this IArgumentSet set, Parameter<T> parameter)
        {
            return set.GetByName<T>(null);
        }
    }

    public interface ICommandParser
    {
        Func<Task> Parse(CommandConfiguration configuration, ImmutableArray<string> arguments);
    }

    public class CommandConfiguration
    {
        public CommandConfiguration(IImmutableDictionary<string, CommandConfiguration> commands, ImmutableArray<IParameter> parameters)
        {
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));
            Parameters = parameters;
        }

        public IImmutableDictionary<string, CommandConfiguration> Commands { get; }
        public ImmutableArray<IParameter> Parameters { get; }
    }
}
