using CommandLineParsing.Output;
using System;
using System.Collections.Immutable;

namespace CommandLineParsing.Input.Reading
{
    public class ReadcharConfiguration<T>
    {
        public ReadcharConfiguration(ConsoleString prompt, IImmutableDictionary<char, T> options)
        {
            Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ConsoleString Prompt { get; }
        public IImmutableDictionary<char, T> Options { get; }
    }

    public static class ReadcharConfiguration
    {
        public static ReadcharConfiguration<T> Create<T>()
        {
            return new ReadcharConfiguration<T>
            (
                prompt: ConsoleString.Empty,
                options: ImmutableDictionary<char, T>.Empty
            );
        }
    }
}
