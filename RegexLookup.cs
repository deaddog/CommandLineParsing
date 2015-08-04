using System.Text.RegularExpressions;

namespace CommandLineParsing
{
    internal static class RegexLookup
    {
        private static string letter = "[a-zA-Z]";
        private static string letterAndNumber = "[a-zA-Z0-9]";
        private static string letterThenBoth = letter + letterAndNumber + "*";

        public static readonly Regex ParameterName = new Regex($"^-+{letterThenBoth}(-+{letterThenBoth})*$");
    }
}
