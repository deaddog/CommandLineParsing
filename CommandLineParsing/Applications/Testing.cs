using CommandLineParsing.Applications.Composition;
using CommandLineParsing.Input.Reading;
using CommandLineParsing.Output;
using CommandLineParsing.Output.Formatting;
using CommandLineParsing.Output.Formatting.Structure;
using CommandLineParsing.Validation;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLineParsing.Applications
{
    public static class Testing
    {
        public static void MainTest(string[] args)
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
                    .WithEnvVar("MY_NAME")
                    .Required()
                )
                .WithParameter<int>("age", out var age)
                .WithParameter<int>("age2", out _)
                //.Where(args => args.Get(name).Used && args.Get(age).Used)
                .OnExecute(args =>
                {
                    var myName = args.Get(name);
                    Console.WriteLine($"Hello {myName.Value}");
                })
                .OnExecute(async (args, ct) => await Task.Delay(1000, ct))
                .OnExecute(args => ExecuteMyThing(args.Get(name), args.Get(age)))
                ;
        }

        static async Task ExecuteMyThing(Argument<string> name, Argument<int> age)
        {

        }
    }

    public class Command: IComposer<IExecution, Command> // : IValidatorComposer<ArgumentSet, Command>
    {
        public static Command Create()
        {
            return new Command
            (
                parameters: ImmutableArray<IParameter>.Empty,
                execution: SequentialExecution.Create()
            );
        }

        Command IComposer<IExecution, Command>.With(IExecution state)
        {
            return new Command
            (Parameters, state);
        }
        IExecution IComposer<IExecution, Command>.State => Execution;

        public Command(ImmutableArray<IParameter> parameters, IExecution execution)
        {
            Parameters = parameters;
            Execution = execution ?? throw new ArgumentNullException(nameof(execution));
        }

        public ImmutableArray<IParameter> Parameters { get; }
        public IExecution Execution { get; }
    }



    public static class CommandExtensions
    {
        public static Command WithParameter<T>(this Command command, string name, out Parameter<T> parameter)
        {
            return WithParameter(command, name, out parameter, p => p);
        }
        public static Command WithParameter<T>(this Command command, string name, out Parameter<T> parameter, Func<ParameterComposer<T>, ParameterComposer<T>> builder)
        {
            return new Command
            (
                parameters: command.Parameters.Add(parameter = builder(ParameterComposer.Create<T>(name)).GetParameter()),
                execution: command.Execution
            );
        }

        //public static Command OnExecute(this Command command, IExecution execution)
        //{
        //    return new Command
        //    (
        //        parameters: command.Parameters,
        //        execution: SequentialExecution.Create(command.Execution, execution)
        //    );
        //}
        //public static Command OnExecute(this Command command, Action<ArgumentSet> execution)
        //{
        //    return OnExecute(command, new SyncExecution(execution));
        //}
        //public static Command OnExecute(this Command command, Func<ArgumentSet, Task> execution)
        //{
        //    return OnExecute(command, new AsyncExecution((args, token) => execution(args)));
        //}
        //public static Command OnExecute(this Command command, Func<ArgumentSet, CancellationToken, Task> execution)
        //{
        //    return OnExecute(command, new AsyncExecution(execution));
        //}
    }

    public class ArgumentSet
    {
        private readonly ImmutableDictionary<string, object> _arguments;

        private ArgumentSet(ImmutableDictionary<string, object> arguments)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public ArgumentSet With<T>(Argument<T> argument)
        {
            return new ArgumentSet
            (
                arguments: _arguments.Add(argument.Name, argument)
            );
        }

        public Argument<T>? GetByName<T>(string name)
        {
            if (_arguments.TryGetValue(name, out var obj))
            {
                if (!(obj is Argument<T> arg))
                    throw new ArgumentException($"The argument '{name}' is not of type '{typeof(T).Name}'.", nameof(name));

                return arg;
            }

            return null;
        }

        public Argument<T>? Get<T>(IParameter parameter)
        {
            return GetByName<T>(parameter.ToString()) ?? Argument.Create<T>(parameter);
        }
    }

    public interface IArgumentsParser
    {
        (Command Command, ArgumentSet Arguments) Parse(Command command, ImmutableArray<string> args);
    }

    public static class ArgumentSetExtensions
    {
        public static Argument<T> Get<T>(this ArgumentSet set, Parameter<T> parameter)
        {
            return set.Get<T>(parameter);
        }
    }
}
