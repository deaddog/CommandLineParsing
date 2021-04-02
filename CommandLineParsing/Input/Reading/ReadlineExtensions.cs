using CommandLineParsing.Output;
using CommandLineParsing.Validation;
using System.Linq;

namespace CommandLineParsing.Input.Reading
{
    public static class ReadlineExtensions
    {
        public static T Read<T>(this ReadlineConfiguration<T> configuration, IConsole console)
        {
            var start = console.GetCursorPosition();
            console.Write(configuration.Prompt);
            var reader = new ConsoleReader(console) { Text = configuration.Default };

            Message<T> msg;

            do
            {
                var input = reader.ReadLine();
                var arrayInput = typeof(T).IsArray ? Command.SimulateParse(input) : new[] { input };

                msg = configuration.Parser.Parse(arrayInput);
                msg = configuration.Validator.Validate(msg);

                if (msg.IsError)
                    ShowError(console, reader, msg.Content);
            } while (msg.IsError);

            ApplyCleanup
            (
                prompt: configuration.Prompt,
                console: console,
                start: start,
                reader: reader,
                cleanup: configuration.Cleanup.Success
            );

            return msg.Value;
        }
        public static bool ReadOrCancel<T>(this ReadlineConfiguration<T> configuration, IConsole console, out T result)
        {
            var start = console.GetCursorPosition();
            console.Write(configuration.Prompt);
            var reader = new ConsoleReader(console) { Text = configuration.Default };

            var cancelled = false;
            Message<T> msg;

            do
            {
                if (!reader.ReadLineOrCancel(out var input))
                {
                    result = default;
                    cancelled = true;
                    break;
                }

                var arrayInput = typeof(T).IsArray ? Command.SimulateParse(input) : new[] { input };

                msg = configuration.Parser.Parse(arrayInput);
                msg = configuration.Validator.Validate(msg);

                if (msg.IsError)
                {
                    ShowError(console, reader, msg.Content);
                    result = default;
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

        private static void ShowError(IConsole console, ConsoleReader reader, ConsoleString msg)
        {
            var text = reader.Text;
            reader.Text = "";

            var red = Color.NoColor.WithForeground("red");

            var message = new ConsoleString(msg.Select(s => new ConsoleStringSegment(s.Content, s.HasColor ? s.Color : red)));
            console.TemporaryShift(c => c.Write(message));

            console.TemporaryShift(c => c.ReadKey(true));
            console.TemporaryShift(c => c.Write(new string(' ', message.Length)));

            reader.Text = text;
        }
        private static void ApplyCleanup(ConsoleString prompt, IConsole console, ConsolePoint start, ConsoleReader reader, ReadlineCleanup cleanup)
        {
            switch (cleanup)
            {
                case ReadlineCleanup.None:
                    console.WriteLine();
                    break;

                case ReadlineCleanup.RemovePrompt:
                    var text = reader.Text;
                    reader.Text = string.Empty;
                    console.SetCursorPosition(start);
                    console.Write(new string(' ', prompt.Length));
                    console.SetCursorPosition(start);
                    console.WriteLine(text);
                    break;

                case ReadlineCleanup.RemoveAll:
                    reader.Text = string.Empty;
                    console.SetCursorPosition(start);
                    console.Write(new string(' ', prompt.Length));
                    console.SetCursorPosition(start);
                    break;
            }
        }
    }
}
