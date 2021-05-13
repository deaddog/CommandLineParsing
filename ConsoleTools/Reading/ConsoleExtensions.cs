using ConsoleTools.Validation;
using System;

namespace ConsoleTools.Reading
{
    public static class ConsoleExtensions
    {
        public static T ReadChar<T>(this IConsole console, Func<ReadCharConfiguration<T>, ReadCharConfiguration<T>> configure)
        {
            return ReadChar
            (
                console: console,
                configuration: configure(ReadCharConfiguration<T>.Default)
            );
        }
        public static T ReadChar<T>(this IConsole console, ReadCharConfiguration<T> configuration)
        {
            var cancelled = !BaseReadCharOrCancel
            (
                console: console,
                configuration: configuration,
                result: out var result,
                canCancel: false
            );

            if (cancelled)
                throw new InvalidOperationException("Non-cancelable read was cancelled.");

            return result;
        }

        public static bool ReadCharOrCancel<T>(this IConsole console, Func<ReadCharConfiguration<T>, ReadCharConfiguration<T>> configure, out T result)
        {
            return ReadCharOrCancel
            (
                console: console,
                configuration: configure(ReadCharConfiguration<T>.Default),
                result: out result
            );
        }
        public static bool ReadCharOrCancel<T>(this IConsole console, ReadCharConfiguration<T> configuration, out T result)
        {
            return BaseReadCharOrCancel
            (
                console: console,
                configuration: configuration,
                result: out result,
                canCancel: true
            );
        }
        private static bool BaseReadCharOrCancel<T>(IConsole console, ReadCharConfiguration<T> configuration, out T result, bool canCancel)
        {
            if (configuration.Options.Count == 0)
                throw new ArgumentException("Configuration must support at least one option.");

            console.Write(configuration.Prompt);

            ConsoleKeyInfo input;
            do
            {
                input = console.ReadKey(true);

                if (canCancel && input.Key == ConsoleKey.Escape)
                {
                    console.WriteLine();

                    result = default!;
                    return false;
                }
            } while (!configuration.Options.TryGetValue(input.KeyChar, out result!));

            console.WriteLine();

            return true;
        }

        public static T ReadLine<T>(this IConsole console)
        {
            return ReadLine
            (
                console: console,
                configuration: ReadLineConfiguration<T>.Default
            );
        }
        public static T ReadLine<T>(this IConsole console, Func<ReadLineConfiguration<T>, ReadLineConfiguration<T>> configure)
        {
            return ReadLine
            (
                console: console,
                configuration: configure(ReadLineConfiguration<T>.Default)
            );
        }
        public static T ReadLine<T>(this IConsole console, ReadLineConfiguration<T> configuration)
        {
            var cancelled = !BaseReadLineOrCancel
            (
                console: console,
                configuration: configuration,
                result: out var result,
                canCancel: false
            );

            if (cancelled)
                throw new InvalidOperationException("Non-cancelable read was cancelled.");

            return result;
        }

        public static bool ReadLineOrCancel<T>(this IConsole console, out T result)
        {
            return ReadLineOrCancel
            (
                console: console,
                configuration: ReadLineConfiguration<T>.Default,
                result: out result
            );
        }
        public static bool ReadLineOrCancel<T>(this IConsole console, Func<ReadLineConfiguration<T>, ReadLineConfiguration<T>> configure, out T result)
        {
            return ReadLineOrCancel
            (
                console: console,
                configuration: configure(ReadLineConfiguration<T>.Default),
                result: out result
            );
        }
        public static bool ReadLineOrCancel<T>(this IConsole console, ReadLineConfiguration<T> configuration, out T result)
        {
            return BaseReadLineOrCancel
            (
                console: console,
                configuration: configuration,
                result: out result,
                canCancel: true
            );
        }
        private static bool BaseReadLineOrCancel<T>(IConsole console, ReadLineConfiguration<T> configuration, out T result, bool canCancel)
        {
            var start = console.CursorPosition;
            console.Write(configuration.Prompt);
            var reader = new ConsoleReader(console) { Text = configuration.Initial };

            var cancelled = false;
            Message<T> msg;

            do
            {
                string input;

                if (canCancel)
                {
                    if (!reader.ReadLineOrCancel(out input))
                    {
                        result = default!;
                        cancelled = true;
                        break;
                    }
                }
                else
                {
                    input = reader.ReadLine();
                }

                msg = configuration.Parser.Parse(input);
                msg = configuration.Validator.Validate(msg);

                if (msg.IsError)
                {
                    ShowError(console, reader, msg.Content);
                    result = default!;
                }
                else
                    result = msg.Value;
            } while (msg.IsError);

            ApplyCleanup
            (
                prompt: configuration.Prompt,
                console: console,
                start: start,
                reader: reader,
                cleanup: cancelled ? configuration.Cleanup.Cancel : configuration.Cleanup.Success
            );

            return !cancelled;
        }

        private static void ShowError(IConsole console, ConsoleReader reader, ConsoleString message)
        {
            var text = reader.Text;
            reader.Text = "";

            var p = console.CursorPosition;
            console.Write(message);
            console.ReadKey(true);
            console.CursorPosition = p;
            console.Write(new string(' ', message.Length));
            console.CursorPosition = p;

            reader.Text = text;
        }
        private static void ApplyCleanup(ConsoleString prompt, IConsole console, Vector start, ConsoleReader reader, Cleanup cleanup)
        {
            switch (cleanup)
            {
                case Cleanup.None:
                    console.WriteLine();
                    break;

                case Cleanup.RemovePrompt:
                    var text = reader.Text;
                    reader.Text = string.Empty;
                    console.CursorPosition = start;
                    console.Write(new string(' ', prompt.Length));
                    console.CursorPosition = start;
                    console.WriteLine(text);
                    break;

                case Cleanup.Remove:
                    reader.Text = string.Empty;
                    console.CursorPosition = start;
                    console.Write(new string(' ', prompt.Length));
                    console.CursorPosition = start;
                    break;
            }
        }
    }
}
