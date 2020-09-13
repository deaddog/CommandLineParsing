﻿using CommandLineParsing.Output;

namespace CommandLineParsing.Input.Reading
{
    public static class ReadlineConfigurationExtensions
    {
        public static ReadlineConfiguration<T> WithPrompt<T>(this ReadlineConfiguration<T> composer, ConsoleString prompt)
        {
            return new ReadlineConfiguration<T>
            (
                prompt: prompt,
                @default: composer.Default,
                cleanup: composer.Cleanup,
                parser: composer.Parser,
                validator: composer.Validator
            );
        }
        public static ReadlineConfiguration<T> WithoutPrompt<T>(this ReadlineConfiguration<T> composer) => composer.WithPrompt(ConsoleString.Empty);

        public static ReadlineConfiguration<T> WithDefault<T>(this ReadlineConfiguration<T> composer, string @default)
        {
            return new ReadlineConfiguration<T>
            (
                prompt: composer.Prompt,
                @default: @default,
                cleanup: composer.Cleanup,
                parser: composer.Parser,
                validator: composer.Validator
            );
        }
        public static ReadlineConfiguration<T> WithoutDefault<T>(this ReadlineConfiguration<T> composer) => composer.WithDefault(string.Empty);

        public static ReadlineConfiguration<T> WithCleanup<T>(this ReadlineConfiguration<T> composer, ReadLineCleanup cleanup = ReadLineCleanup.RemoveAll)
        {
            return composer.WithCleanup(cleanup, cleanup);
        }
        public static ReadlineConfiguration<T> WithCleanup<T>(this ReadlineConfiguration<T> composer, ReadLineCleanup success, ReadLineCleanup cancel)
        {
            return new ReadlineConfiguration<T>
            (
                prompt: composer.Prompt,
                @default: composer.Default,
                cleanup: new ReadlineCleanupConfiguration
                (
                    success: success,
                    cancel: cancel
                ),
                parser: composer.Parser,
                validator: composer.Validator
            );
        }
        public static ReadlineConfiguration<T> WithoutCleanup<T>(this ReadlineConfiguration<T> composer) => composer.WithCleanup(ReadLineCleanup.None);
    }
}
