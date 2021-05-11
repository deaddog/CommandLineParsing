using System;
using System.Text.RegularExpressions;

namespace ConsoleTools.Parsing
{
    public static class ParserComposerExtensions
    {
        public static TComposer WithParser<T, TComposer>(this IParserComposer<T, TComposer> composer, Func<string, T> parser)
        {
            return WithParser<T, TComposer>(composer, txt =>
            {
                try
                {
                    return new Message<T>(parser(txt));
                }
                catch
                {
                    return new Message<T>(ConsoleString.Create
                    (
                        new ConsoleString.Segment("Failed to parse '", Coloring.Colors.ErrorMessage),
                        new ConsoleString.Segment(txt, Coloring.Colors.ErrorValue),
                        new ConsoleString.Segment($"' as {typeof(T).Name}", Coloring.Colors.ErrorMessage)
                    ));
                }
            });
        }
        public static TComposer WithParser<T, TComposer>(this IParserComposer<T, TComposer> composer, Func<string, Message<T>> parser)
        {
            return composer.WithParser(new Parser<T>(parser));
        }

        public static TComposer WithRegexParser<T, TComposer>(this IParserComposer<T, TComposer> composer, string pattern, Func<Match, T> parser)
        {
            return WithRegexParser(composer, pattern, m => new Message<T>(parser(m)));
        }
        public static TComposer WithRegexParser<T, TComposer>(this IParserComposer<T, TComposer> composer, string pattern, Func<Match, Message<T>> parser)
        {
            var regex = new Regex(pattern);

            return composer.WithParser(txt =>
            {
                var match = regex.Match(txt);

                if (!match.Success)
                    return new Message<T>(ConsoleString.Create
                    (
                        new ConsoleString.Segment("'", Coloring.Colors.ErrorMessage),
                        new ConsoleString.Segment(txt, Coloring.Colors.ErrorValue),
                        new ConsoleString.Segment("' does not match pattern ", Coloring.Colors.ErrorMessage),
                        new ConsoleString.Segment(pattern, Coloring.Colors.ErrorValue)
                    ));
                try
                {
                    return parser(match);
                }
                catch
                {
                    return new Message<T>(ConsoleString.Create
                    (
                        new ConsoleString.Segment("Failed to parse '", Coloring.Colors.ErrorMessage),
                        new ConsoleString.Segment(txt, Coloring.Colors.ErrorValue),
                        new ConsoleString.Segment($"' as {typeof(T).Name}", Coloring.Colors.ErrorMessage)
                    ));
                }
            });
        }
    }
}
