using System;

namespace CommandLineParsing.Input.Reading
{
    public static class ConsoleReaderExtensions
    {
        public static string ReadLine(this ConsoleReader reader)
        {
            while (true)
            {
                var info = reader.Console.ReadKey(true);

                if (info.Key == ConsoleKey.Enter)
                    return reader.Text;
                else
                    reader.HandleKey(info);
            }
        }
        public static bool ReadLineOrCancel(this ConsoleReader reader, out string value)
        {
            while (true)
            {
                var info = reader.Console.ReadKey(true);

                if (info.Key == ConsoleKey.Enter)
                {
                    value = reader.Text;
                    return true;
                }
                else if (info.Key == ConsoleKey.Escape)
                {
                    value = default;
                    return false;
                }
                else
                    reader.HandleKey(info);
            }
        }
    }
}
