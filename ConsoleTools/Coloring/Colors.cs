using System.Text.RegularExpressions;

namespace ConsoleTools.Coloring
{
    public static class Colors
    {
        private static Color FromName(string name)
        {
            var cased = Regex.Replace(name, "[A-Z]", m => "_" + m.Value.ToLower());

            return new Color
            (
                foreground: $"{cased}_f",
                background: $"{cased}_b"
            );
        }

        public static Color ErrorMessage { get; } = FromName(nameof(ErrorMessage));
        public static Color ErrorValue { get; } = FromName(nameof(ErrorValue));

        public static Color UnknownFormatVariable { get; } = FromName(nameof(UnknownFormatVariable));
        public static Color UnknownFormatCondition { get; } = FromName(nameof(UnknownFormatCondition));
        public static Color UnknownFormatFunction { get; } = FromName(nameof(UnknownFormatFunction));
    }
}
