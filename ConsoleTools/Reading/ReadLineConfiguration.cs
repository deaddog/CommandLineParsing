using ConsoleTools.Parsing;
using ConsoleTools.Validation;
using System;

namespace ConsoleTools.Reading
{
    public class ReadLineConfiguration<T> : IPromptComposer<ReadLineConfiguration<T>>, ICleanupComposer<ReadLineConfiguration<T>>, IParserComposer<T, ReadLineConfiguration<T>>, IValidatorComposer<T, ReadLineConfiguration<T>>
    {
        public static ReadLineConfiguration<T> Default { get; } = new ReadLineConfiguration<T>
        (
            prompt: ConsoleString.Empty,
            initial: string.Empty,
            cleanup: new CleanupConfiguration
            (
                Reading.Cleanup.None,
                Reading.Cleanup.None
            ),
            parser: ParserRegistry.Default.TryGet<T>(out var p) ? p : new MissingParser<T>(),
            validator: Validator<T>.NoRules
        );

        public ReadLineConfiguration(ConsoleString prompt, string initial, CleanupConfiguration cleanup, IParser<T> parser, IValidator<T> validator)
        {
            Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
            Initial = initial ?? throw new ArgumentNullException(nameof(initial));
            Cleanup = cleanup ?? throw new ArgumentNullException(nameof(cleanup));

            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public ConsoleString Prompt { get; }
        public ReadLineConfiguration<T> WithPrompt(ConsoleString prompt)
        {
            return new ReadLineConfiguration<T>
            (
                prompt: prompt,
                initial: Initial,
                cleanup: Cleanup,
                parser: Parser,
                validator: Validator
            );
        }

        public string Initial { get; }

        public CleanupConfiguration Cleanup { get; }
        public ReadLineConfiguration<T> WithCleanup(CleanupConfiguration cleanup)
        {
            return new ReadLineConfiguration<T>
            (
                prompt: Prompt,
                initial: Initial,
                cleanup: cleanup,
                parser: Parser,
                validator: Validator
            );
        }

        public IParser<T> Parser { get; }
        public ReadLineConfiguration<T> WithParser(IParser<T> parser)
        {
            return new ReadLineConfiguration<T>
            (
                prompt: Prompt,
                initial: Initial,
                cleanup: Cleanup,
                parser: parser,
                validator: Validator
            );
        }

        public IValidator<T> Validator { get; }
        public ReadLineConfiguration<T> WithValidator(IValidator<T> validator)
        {
            return new ReadLineConfiguration<T>
            (
                prompt: Prompt,
                initial: Initial,
                cleanup: Cleanup,
                parser: Parser,
                validator: validator
            );
        }
    }
}
