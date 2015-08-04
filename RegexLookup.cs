using System.Text.RegularExpressions;

namespace CommandLineParsing
{
    internal static class RegexLookup
    {
        public static readonly Regex ParameterName = new Regex("^--?[a-z]+(-[a-z]+)*$");
    }
}
