using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleTools.Applications
{
    public class ParsedResult<T> where T : class
    {
        public ParsedResult(T result, IImmutableQueue<string> arguments)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            foreach (var a in arguments)
                if (a is null)
                    throw new ArgumentNullException(nameof(arguments), "All arguments must be non-null.");
        }

        public T Result { get; }
        public IImmutableQueue<string> Arguments { get; }
    }

    public interface IArgumentsParser
    {
        (Command Command, ArgumentSet Arguments) Parse(Command command, ImmutableArray<string> args);
    }

    public interface IArgumentsParser
    {
        (Command Command, ArgumentSet Arguments) Parse(Command command, ImmutableArray<string> args);
    }

    public class DashedArgumentsParser : IArgumentsParser
    {
        private static readonly Regex _parRegex = new Regex("^-+[a-zA-Z][a-zA-Z0-9](-+[a-zA-Z][a-zA-Z0-9])*$");

        public (Command Command, ArgumentSet Arguments) Parse(Command command, ImmutableArray<string> args)
        {
            var arguments = ImmutableQueue.CreateRange(args);
            throw new NotImplementedException();
        }

        private static bool IsParameterName(string arg)
        {
            return Regex.IsMatch
            (
                input: arg,
                pattern: "^-+[a-zA-Z][a-zA-Z0-9](-+[a-zA-Z][a-zA-Z0-9])*$"
            );
        }

        private static bool IsEndOfArgs(string arg)
        {
            return arg == "==";
        }

        private static ParsedResult<Command> ParseCommand(Command command, IImmutableQueue<string> args)
        {
            if (!args.IsEmpty && command.Commands.TryGetValue(args.Peek(), out var sub))
                return ParseCommand(sub, args.Dequeue());
            else
                return new ParsedResult<Command>(command, args);
        }


        private static ArgumentSet ParseArguments(ImmutableArray<IParameter> parameters, IImmutableQueue<string> args)
        {
            return ParseArguments(args)
                .GroupBy(i => parameters.FirstOrDefault(p => IsMatch(p, i.Name)) ?? throw new MessageException("The parameter " + i.Name + " does not exist"))
                .Select(x => x.Key.Resolve(x.ToImmutableArray()))
                .Aggregate(ArgumentSet.Empty, ArgumentSet.Merge);
        }
        private static IEnumerable<Input> ParseArguments(IImmutableQueue<string> args)
        {
            if (args.IsEmpty)
                yield break;

            while (!args.IsEmpty && IsParameterName(args.Peek()))
            {
                args = args.Dequeue(out var arg);
                var values = ImmutableArray<string>.Empty;

                while (!args.IsEmpty && !IsParameterName(args.Peek()) && !IsEndOfArgs(args.Peek()))
                {
                    args = args.Dequeue(out var v);
                    values = values.Add(v);
                }

                yield return new Input(arg, values);
            }

            var noNameValues = ImmutableArray<string>.Empty;
            while (!args.IsEmpty)
            {
                args = args.Dequeue(out var arg);

                if (IsEndOfArgs(arg))
                    continue;

                noNameValues = noNameValues.Add(arg);
            }

            if (noNameValues.Length > 0)
                yield return new Input(string.Empty, noNameValues);
        }

        private static Message NoUnnamed(string value) => new Message($@"You must specify which parameter the value '{value}' is associated with (eg. [Example:--parameter-name {value}]).");
    }
}
