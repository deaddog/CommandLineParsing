using CommandLineParsing.Parsing;
using CommandLineParsing.Validation;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace CommandLineParsing.Applications
{
    public class Parameter<T> : IParameter
    {
        public Parameter(ImmutableArray<string> names, string description, IParser<T> parser, IValidator<T> validator, Usage usage)
        {
            if (names.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Parameter name cannot be null/empty", nameof(names));

            Names = names;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            Usage = usage;
        }

        public ImmutableArray<string> Names { get; }
        public string Description { get; }
        public IParser<T> Parser { get; }
        public IValidator<T> Validator { get; }
        public Usage Usage { get; }

        public ArgumentSet Resolve(ArgumentSet arguments, string name, ImmutableArray<string> args)
        {
            var parsed = Parser.Parse(args.ToArray());
            var validated = Validator.Validate(parsed);

            var value = validated.GetValueOrThrow();

            return arguments.With(new Argument<T>
            (
                parameter: this,
                name: name,
                args: args,
                value: value
            ));
        }
    }
}
