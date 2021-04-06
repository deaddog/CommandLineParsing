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

            // Do stuff using the console, by interacting with ColorConsole.

            var cmd = Command.Create()
                .WithParameter<string>("name", out var name, p => p
                    .Where(x => x.Length > 20)
                    /*.Required()*/)
                .WithParameter<int>("age", out var age)
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
        public static Command Create() { return null; }

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
        public static Command WithParameter<T>(this Command command, string name, IParameter<T> parameter)
        {
            return new Command
            (
                parameters: command.Parameters.Add(name, parameter),
                execution: command.Execution
            );
        }

        public static Command WithParameter<T>(this Command command, string name, out IParameter<T> parameter)
        {
            return WithParameter(command, name, out parameter, p => p);
        }
        public static Command WithParameter<T>(this Command command, string name, out IParameter<T> parameter, Func<Parameter<T>, Parameter<T>> builder)
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

    public static class ALDSJL
    {
        public static ICommand<ValueTuple<T1, T2>> WOOT<T1, T2>(this ICommand<ValueTuple<T1>> command, IParameter<T2> l)
        {
            throw new NotImplementedException();
        }
        public static ICommand<ValueTuple<T1, T2, T3>> WOOT<T1, T2, T3>(this ICommand<ValueTuple<T1, T2>> command, IParameter<T3> l)
        {
            throw new NotImplementedException();
        }
    }

    public static class Parameter
    {
        public static Parameter<T> Create<T>() => null;
    }

    public class Parameter<T> : IParameter<T>, IValidatorComposer<T, Parameter<T>>, IParserComposer<T, Parameter<T>>
    {
        public IValidator<T> Validator => throw new NotImplementedException();

        public IParser<T> Parser => throw new NotImplementedException();

        public IArgument<T> Resolve(ImmutableArray<string> args)
        {
            throw new NotImplementedException();
        }

        public Parameter<T> WithParser(IParser<T> parser)
        {
            throw new NotImplementedException();
        }

        public Parameter<T> WithValidator(IValidator<T> validator)
        {
            throw new NotImplementedException();
        }

        IArgument IParameter.Resolve(ImmutableArray<string> args)
        {
            throw new NotImplementedException();
        }
    }

    public class Parameters
    {
        public IParameter<int> Number { get; }
        public IParameter<string> Name { get; }
    }

    public class Arguments
    {
        public IArgument<int> Number { get; }
        public IArgument<string> Name { get; }
    }

    public interface IParameter
    {
        IArgument Resolve(ImmutableArray<string> args);
    }
    public interface IParameter<T> : IParameter
    {
        new IArgument<T> Resolve(ImmutableArray<string> args);
    }

    public interface IArgument
    {
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

    public static class ArgumentSetExtensions
    {
        public static IArgument<T> Get<T>(this IArgumentSet set, IParameter<T> parameter)
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

    public interface ICommand
    {
        Func<Task<Message>> Construct(ImmutableArray<string> arguments);
    }
    public interface ICommand<TArgs>
    {
        Func<TArgs, Task<Message>> Construct(ImmutableArray<string> arguments);
    }

    public class MySpecificArgs
    {
        public MySpecificArgs(IArgument<string> name, IArgument<int> age)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Age = age ?? throw new ArgumentNullException(nameof(age));
        }

        public IArgument<string> Name { get; }
        public IArgument<int> Age { get; }

        public IArgument<T> GetByName<T>(string name)
        {
            return name switch
            {
                nameof(Name) => (IArgument<T>)Name,
                nameof(Age) => (IArgument<T>)Age,
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };
        }
    }
}
