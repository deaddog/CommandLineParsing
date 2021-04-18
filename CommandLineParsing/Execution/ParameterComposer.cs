using CommandLineParsing.Parsing;
using CommandLineParsing.Validation;
using System;
using System.Collections.Immutable;

namespace CommandLineParsing.Execution
{
    public static class ParameterComposer
    {
        public static ParameterComposer<T> Create<T>(string name, params string[] alternatives)
        {
            return new ParameterComposer<T>
            (
                names: ImmutableArray.Create(alternatives).Insert(0, name),
                description: string.Empty,
                parser: new ReflectedParser<T>(GetParserSettings<T>()),
                validator: Validation.Validator<T>.NoRules,
                environmentVariable: null,
                usage: Usage.ZeroOrOne
            );
        }

        private static ReflectedParserSettings GetParserSettings<T>()
        {
            string typename = typeof(T).Name;

            return new ReflectedParserSettings
            (
                enumIgnoreCase: true,
                noValueMessage: Message.NoError,
                multipleValuesMessage: new Message("Only one value can be specified."),
                typeErrorMessage: x => new Message($"{x} is not a {typename} value."),
                useParserMessage: true
            );
        }
    }

    public class ParameterComposer<T> : IParserComposer<T, ParameterComposer<T>>, IValidatorComposer<T, ParameterComposer<T>>
    {
        public ParameterComposer(ImmutableArray<string> names, string description, IParser<T> parser, IValidator<T> validator, string? environmentVariable, Usage usage)
        {
            Names = names;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            EnvironmentVariable = environmentVariable;
            Usage = usage;
        }

        public ImmutableArray<string> Names { get; }
        public string Description { get; }
        public IParser<T> Parser { get; }
        public IValidator<T> Validator { get; }
        public string EnvironmentVariable { get; }
        public Usage Usage { get; }

        public ParameterComposer<T> WithParser(IParser<T> parser)
        {
            return new ParameterComposer<T>
            (
                names: Names,
                description: Description,
                parser: Parser,
                validator: Validator,
                environmentVariable: EnvironmentVariable,
                usage: Usage
            );
        }
        public ParameterComposer<T> WithValidator(IValidator<T> validator)
        {
            return new ParameterComposer<T>
            (
                names: Names,
                description: Description,
                parser: Parser,
                validator: Validator,
                environmentVariable: EnvironmentVariable,
                usage: Usage
            );
        }

        public Parameter<T> GetParameter()
        {
            return new Parameter<T>
            (
                names: Names,
                description: Description,
                parser: Parser,
                validator: Validator,
                usage: Usage
            );
        }
    }
}
