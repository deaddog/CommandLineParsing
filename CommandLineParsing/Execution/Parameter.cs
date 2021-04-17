using CommandLineParsing.Parsing;
using CommandLineParsing.Validation;
using System;
using System.Collections.Immutable;

namespace CommandLineParsing.Execution
{
    public class Parameter<T> : IParameter
    {
        public Parameter(IParser<T> parser, IValidator<T> validator, Usage usage)
        {
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            Usage = usage;
        }

        public IParser<T> Parser { get; }
        public IValidator<T> Validator { get; }
        public Usage Usage { get; }

        public ArgumentSet Resolve(ArgumentSet arguments, ImmutableArray<string> args)
        {
            throw new NotImplementedException();
        }
    }

    public enum Usage
    {
        ZeroOrOne,
        One,
        ZeroOrMany,
        OneOrMany
    }
}
