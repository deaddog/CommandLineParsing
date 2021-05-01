using System;

namespace ConsoleTools
{
    public struct Color : IEquatable<Color>
    {
        public static Color NoColor { get; } = new Color();

        public static Color Parse(string color)
        {
            var c = NoColor;
            var pipeIndex = color.IndexOf('|');

            var foreground = pipeIndex >= 0 ? color.Substring(0, pipeIndex).Trim() : color.Trim();
            var background = pipeIndex >= 0 ? color.Substring(pipeIndex + 1).Trim() : string.Empty;

            return new Color
            (
                foreground: foreground,
                background: background
            );
        }

        public Color(string? foreground, string? background)
        {
            Foreground = foreground;
            Background = background;
        }

        public Color WithForeground(string foreground)
        {
            return new Color
            (
                foreground: foreground ?? throw new ArgumentNullException(nameof(foreground)),
                background: Background
            );
        }
        public Color WithBackground(string background)
        {
            return new Color
            (
                foreground: Foreground,
                background: background ?? throw new ArgumentNullException(nameof(background))
            );
        }

        public Color WithoutForeground()
        {
            return new Color
            (
                foreground: null,
                background: Background
            );
        }
        public Color WithoutBackground()
        {
            return new Color
            (
                foreground: Foreground,
                background: null
            );
        }

        public static bool operator ==(Color c1, Color c2)
        {
            return c1.Equals(c2);
        }
        public static bool operator !=(Color c1, Color c2)
        {
            return !(c1 == c2);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Foreground, Background);
        }

        public override bool Equals(object? obj)
        {
            return obj is Color color && Equals(color);
        }
        public bool Equals(Color other)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(Foreground, other.Foreground) && StringComparer.OrdinalIgnoreCase.Equals(Background, other.Background);
        }

        public string? Foreground { get; }
        public string? Background { get; }

        public bool HasForeground => !(Foreground is null);
        public bool HasBackground => !(Background is null);

        public override string ToString()
        {
            if (Background is null)
                return Foreground;
            else
                return $"{Foreground}|{Background}";
        }
    }
}
