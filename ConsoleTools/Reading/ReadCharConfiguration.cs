using System;
using System.Collections.Immutable;

namespace ConsoleTools.Reading
{
    public class ReadCharConfiguration<T> : IPromptComposer<ReadCharConfiguration<T>>
    {
        public static ReadCharConfiguration<T> Default { get; } = new ReadCharConfiguration<T>
        (
            prompt: ConsoleString.Empty,
            options: ImmutableDictionary<char, T>.Empty
        );

        public ReadCharConfiguration(ConsoleString prompt, IImmutableDictionary<char, T> options)
        {
            Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ConsoleString Prompt { get; }
        public ReadCharConfiguration<T> WithPrompt(ConsoleString prompt)
        {
            return new ReadCharConfiguration<T>
            (
                prompt: prompt,
                options: Options
            );
        }

        public IImmutableDictionary<char, T> Options { get; }
    }
}
