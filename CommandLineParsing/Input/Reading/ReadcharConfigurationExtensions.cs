﻿using CommandLineParsing.Output;

namespace CommandLineParsing.Input.Reading
{
    public static class ReadcharConfigurationExtensions
    {
        public static ReadcharConfiguration<T> WithPrompt<T>(this ReadcharConfiguration<T> composer, ConsoleString prompt)
        {
            return new ReadcharConfiguration<T>
            (
                prompt: prompt,
                options: composer.Options
            );
        }
        public static ReadcharConfiguration<T> WithoutPrompt<T>(this ReadcharConfiguration<T> composer) => composer.WithPrompt(ConsoleString.Empty);

        public static ReadcharConfiguration<T> WithOption<T>(this ReadcharConfiguration<T> composer, char character, T value)
        {
            return new ReadcharConfiguration<T>
            (
                prompt: composer.Prompt,
                options: composer.Options.SetItem(character, value)
            );
        }
    }
}
