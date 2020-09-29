using CommandLineParsing.Output;
using CommandLineParsing.Parsing;
using CommandLineParsing.Validation;
using System;

namespace CommandLineParsing.Input.Reading
{
    public class ReadlineConfiguration<T> : IParserComposer<T, ReadlineConfiguration<T>>, IValidatorComposer<T, ReadlineConfiguration<T>>
    {
        public ReadlineConfiguration(ConsoleString prompt, string @default, ReadlineCleanupConfiguration cleanup, IParser<T> parser, IValidator<T> validator)
        {
            Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
            Default = @default ?? throw new ArgumentNullException(nameof(@default));
            Cleanup = cleanup ?? throw new ArgumentNullException(nameof(cleanup));

            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public ConsoleString Prompt { get; }
        public string Default { get; }
        public ReadlineCleanupConfiguration Cleanup { get; }

        public IParser<T> Parser { get; }
        public ReadlineConfiguration<T> WithParser(IParser<T> parser)
        {
            return new ReadlineConfiguration<T>
            (
                prompt: Prompt,
                @default: Default,
                cleanup: Cleanup,
                parser: parser,
                validator: Validator
            );
        }

        public IValidator<T> Validator { get; }
        public ReadlineConfiguration<T> WithValidator(IValidator<T> validator)
        {
            return new ReadlineConfiguration<T>
            (
                prompt: Prompt,
                @default: Default,
                cleanup: Cleanup,
                parser: Parser,
                validator: validator
            );
        }
    }

    public static class ReadlineConfiguration
    {
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

        public static ReadlineConfiguration<T> Create<T>()
        {
            return new ReadlineConfiguration<T>
            (
                prompt: ConsoleString.Empty,
                @default: string.Empty,
                cleanup: new ReadlineCleanupConfiguration
                (
                    success: ReadlineCleanup.None,
                    cancel: ReadlineCleanup.None
                ),
                parser: new ReflectedParser<T>(GetParserSettings<T>()),
                validator: Validation.Validator<T>.NoRules
            );
        }
    }
}
