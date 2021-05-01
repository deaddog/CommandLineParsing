using ConsoleTools.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTools
{
    public static class ConsoleExtensions
    {
        public static void SetColors(this IConsole console, Color color)
        {
            if (color.HasForeground)
            {
                var foreground = console.ColorTable[color.Foreground!];

                if (foreground.HasValue)
                    console.ForegroundColor = foreground.Value;
            }

            if (color.HasBackground)
            {
                var background = console.ColorTable[color.Background!];

                if (background.HasValue)
                    console.BackgroundColor = background.Value;
            }
        }

        public static void Write(this IConsole console, ConsoleString value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            ConsoleColor tempForeground = console.ForegroundColor;
            ConsoleColor tempBackground = console.BackgroundColor;

            try
            {
                foreach (var p in value)
                {
                    console.SetColors(p.Color);
                    console.Render(p.Content);

                    console.ForegroundColor = tempForeground;
                    console.BackgroundColor = tempBackground;
                }
            }
            finally
            {
                console.ForegroundColor = tempForeground;
                console.BackgroundColor = tempBackground;
            }
        }
        public static void WriteLine(this IConsole console, ConsoleString value)
        {
            Write(console, value + Environment.NewLine);
        }
        public static void WriteLine(this IConsole console) => console.Render(Environment.NewLine);
    }
}
