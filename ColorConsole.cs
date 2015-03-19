using System;
using System.Text.RegularExpressions;

namespace CommandLineParsing
{
    public static class ColorConsole
    {
        private static readonly Regex colorRegex;

        static ColorConsole()
        {
            var names = Enum.GetNames(typeof(ConsoleColor));
            string namesRegex = string.Join("|", names);

            colorRegex = new Regex(@"\[\[:(?<color>" + namesRegex + @"):(?<content>([^\]]|\][^\]])*)\]\]");
        }

        public static void ToConsole(this string format, bool allowcolor, params object[] args)
        {
            handle(string.Format(format, args), allowcolor, false);
        }
        public static void ToConsoleLine(this string format, bool allowcolor, params object[] args)
        {
            handle(string.Format(format, args), allowcolor, true);
        }

        private static void handle(string input, bool allowcolor, bool newline)
        {
            var m = colorRegex.Match(input);
            if (m.Success)
            {
                string pre = input.Substring(0, m.Index);
                string post = input.Substring(m.Index + m.Length);

                string content = m.Groups["content"].Value;
                if (!allowcolor)
                    Console.WriteLine(pre + content);
                else
                {
                    Console.Write(pre);
                    if (content.Length > 0)
                    {
                        Console.ForegroundColor = getColor(m.Groups["color"].Value);
                        Console.Write(content);
                        Console.ResetColor();
                    }
                }

                handle(post, allowcolor, newline);
            }
            else if (newline)
                Console.WriteLine(input);
            else
                Console.Write(input);
        }

        private static ConsoleColor getColor(string color)
        {
            ConsoleColor c;
            if (!Enum.TryParse(color, out c))
                throw new ArgumentException("Unknown console color: " + color);
            else
                return c;
        }
    }
}
