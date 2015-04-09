﻿using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CommandLineParsing
{
    public static class ColorConsole
    {
        private static readonly Regex colorRegex;

        static ColorConsole()
        {
            colorRegex = new Regex(@"(?<!\\)\[(?<color>[^:]+):(?<content>([^\\\]]|\\\])*)\]");
        }

        public static void Write(string format, params object[] args)
        {
            handle(args.Length == 0 ? format : string.Format(format, args), true, false);
        }
        public static void WriteLine(string format, params object[] args)
        {
            handle(args.Length == 0 ? format : string.Format(format, args), true, true);
        }

        public static void WriteNoColor(string format, params object[] args)
        {
            Console.Write(ClearColors(args.Length == 0 ? format : string.Format(format, args)));
        }
        public static void WriteLineNoColor(string format, params object[] args)
        {
            Console.WriteLine(ClearColors(args.Length == 0 ? format : string.Format(format, args)));
        }

        public static string ClearColors(string input)
        {
            StringBuilder sb = new StringBuilder();

            Match m;
            while ((m = colorRegex.Match(input)).Success)
            {
                sb.Append(input.Substring(0, m.Index));
                sb.Append(m.Groups["content"].Value);
                input = input.Substring(m.Index + m.Length);
            }
            sb.Append(input);

            return sb.ToString();
        }

        private static void handle(string input, bool allowcolor, bool newline)
        {
            var m = colorRegex.Match(input);
            if (m.Success)
            {
                string pre = input.Substring(0, m.Index).Replace("\\[", "[").Replace("\\]", "]");
                string post = input.Substring(m.Index + m.Length);

                string content = m.Groups["content"].Value.Replace("\\[", "[").Replace("\\]", "]");
                if (!allowcolor)
                    Console.Write(pre + content);
                else
                {
                    Console.Write(pre);
                    if (content.Length > 0)
                    {
                        var c = getColor(m.Groups["color"].Value);
                        if (c.HasValue)
                            Console.ForegroundColor = c.Value;
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

        private static ConsoleColor? getColor(string color)
        {
            ConsoleColor c;
            if (!Enum.TryParse(color, out c))
                return null;
            else
                return c;
        }
    }
}
