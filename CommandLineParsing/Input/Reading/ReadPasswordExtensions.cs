using CommandLineParsing.Input.Reading;
using System;
using System.Text;

namespace CommandLineParsing
{
    public static class ReadPasswordExtensions
    {
        public static string Read(this ReadPasswordConfiguration configuration, IConsole console)
        {
            console.Write(configuration.Prompt);

            int pos = console.CursorLeft;
            ConsoleKeyInfo info;

            StringBuilder sb = new StringBuilder(string.Empty);

            var maxLength = configuration.RepeatRender && configuration.RenderAs.Length > 0 ? int.MaxValue : configuration.RenderAs.Length;
            while (true)
            {
                info = console.ReadKey(true);
                if (info.Key == ConsoleKey.Backspace)
                {
                    console.CursorLeft = pos;
                    console.Render(new string(' ', Math.Min(maxLength, sb.Length)));
                    console.CursorLeft = pos;
                    sb.Clear();
                }

                else if (info.Key == ConsoleKey.Enter) { console.Render(Environment.NewLine); break; }

                else if (ConsoleReader.IsInputCharacter(info))
                {
                    sb.Append(info.KeyChar);
                    if (sb.Length <= maxLength)
                        console.Write(configuration.RenderAs[sb.Length % configuration.RenderAs.Length]);
                }
            }
            return sb.ToString();
        }
    }
}
