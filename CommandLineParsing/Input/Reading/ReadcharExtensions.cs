using CommandLineParsing.Output;
using CommandLineParsing.Validation;
using System.Linq;

namespace CommandLineParsing.Input.Reading
{
    public static class ReadcharExtensions
    {
        public static T Read<T>(this ReadcharConfiguration<T> configuration, IConsole console)
        {
            if (configuration.Options.Count == 0)
                throw new System.ArgumentException("Configuration must support at least one option.");

            var start = console.GetCursorPosition();
            console.Write(configuration.Prompt);

            System.ConsoleKeyInfo input;
            T value;
            do
            {
                input = console.ReadKey(true);
            } while (!configuration.Options.TryGetValue(input.KeyChar, out value));

            console.WriteLine();

            return value;
        }
        public static bool ReadOrCancel<T>(this ReadcharConfiguration<T> configuration, IConsole console, out T result)
        {
            if (configuration.Options.Count == 0)
                throw new System.ArgumentException("Configuration must support at least one option.");

            var start = console.GetCursorPosition();
            console.Write(configuration.Prompt);

            System.ConsoleKeyInfo input;
            do
            {
                input = console.ReadKey(true);

                if (input.Key == System.ConsoleKey.Escape)
                {
                    console.WriteLine();

                    result = default;
                    return false;
                }
            } while (!configuration.Options.TryGetValue(input.KeyChar, out result));

            console.WriteLine();

            return true;
        }
    }
}
