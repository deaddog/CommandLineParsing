using System.Text.RegularExpressions;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides a generic format that provides a system of creating custom string-formatting.
    /// See the constructor for more detail on the format.
    /// </summary>
    public abstract class FormattedPrinter
    {
        private readonly string format;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedPrinter"/> class.
        /// </summary>
        /// <param name="format">The format used by the <see cref="FormattedPrinter"/>.</param>
        /// <remarks>
        /// Text in the <paramref name="format"/> string is printed literally with the following exceptions:
        /// - <code>"$variable"</code> | Results in a call to <see cref="GetVariable(string)"/> with <code>"variable"</code> as parameter, replacing the variable with some other content.;
        /// - <code>"$variable+"</code> or <code>"$+variable"</code> | Allows for padding of a variable by calling <see cref="GetAlignedLength(string)"/> with <code>"variable"</code> as parameter. The location of the + indicates which end of the variable that is padded.
        /// - <code>"[color:text]"</code> | Prints <code>"text"</code> using <code>"color"</code> as color. The color string is looked up in <see cref="ColorConsole.Colors"/>.
        /// - <code>"[auto:text $variable text]"</code> | As the above, but calls <see cref="GetAutoColor(string)"/> with <code>"variable"</code> as parameter to obtain the color used before looking it up.
        /// - <code>"?condition{content}"</code> | Calls <see cref="ValidateCondition(string)"/> with <code>"condition"</code> as parameter and only prints <code>"content"</code> if the method returns true.
        /// - <code>"@function{arg1@arg2@arg3...}</code> | Calls <see cref="EvaluateFunction(string, string[])"/> with <code>"function"</code> as first parameter and an array with <code>{ "arg1", "arg2", "arg3, ...}</code> as second parameter.
        /// All of the above allow for nesting within each other.
        /// </remarks>
        public FormattedPrinter(string format)
        {
            this.format = format;
        }

        protected string Handle()
        {
            return Handle(format);
        }
        protected string Handle(string text)
        {
            int index = 0;

            while (index < text.Length)
                switch (text[index])
                {
                    case '[': // Coloring
                        {
                            int end = findEnd(text, index, '[', ']');
                            var block = text.Substring(index + 1, end - index - 1);
                            string replace = colorBlock(block);
                            text = text.Substring(0, index) + replace + text.Substring(end + 1);
                            index += replace.Length;
                        }
                        break;

                    case '?': // Conditional
                        {
                            var match = Regex.Match(text.Substring(index), @"\?[^\{]*");
                            var end = findEnd(text, index + match.Value.Length, '{', '}');
                            var block = text.Substring(index + match.Value.Length + 1, end - index - match.Value.Length - 1);

                            string replace = "";
                            var condition = ValidateCondition(match.Value.Substring(1));
                            if (!condition.HasValue)
                                replace = "?" + match.Value + "{" + Handle(block) + "}";
                            else if (condition.Value)
                                replace = Handle(block);

                            text = text.Substring(0, index) + replace + text.Substring(end + 1);
                            index += replace.Length;
                        }
                        break;

                    case '@': // Listing/Function
                        {
                            var match = Regex.Match(text.Substring(index), @"\@[^\{]*");
                            var end = findEnd(text, index + match.Value.Length, '{', '}');
                            var block = text.Substring(index + match.Value.Length + 1, end - index - match.Value.Length - 1);
                            string replace = EvaluateFunction(match.Value.Substring(1), block.Split('@'));
                            text = text.Substring(0, index) + replace + text.Substring(end + 1);
                            index += replace.Length;
                        }
                        break;

                    case '$': // Variable
                        {
                            var match = Regex.Match(text.Substring(index), @"^\$([a-z]|\+)+");
                            var end = match.Index + index + match.Length;
                            string replace = getVariable(match.Value.Substring(1));
                            text = text.Substring(0, index) + replace + text.Substring(end);
                            index += replace.Length;
                        }
                        break;
                    case '\\':
                        if (text.Length == index + 1)
                            index++;
                        else if (text[index + 1] == '[' || text[index + 1] == ']')
                            index += 2;
                        else
                        {
                            text = text.Substring(0, index) + text.Substring(index + 1);
                            index++;
                        }
                        break;

                    default: // Skip content
                        index = text.IndexOfAny(new char[] { '[', '?', '@', '$', '\\' }, index);
                        if (index < 0) index = text.Length;
                        break;
                }

            return text;
        }

        private string getVariable(string variable)
        {
            bool padLeft = variable[0] == '+';
            bool padRight = variable[variable.Length - 1] == '+';

            if (padLeft) variable = variable.Substring(1);
            if (padRight) variable = variable.Substring(0, variable.Length - 1);

            string res = GetVariable(variable);
            if (res == null)
                return "$" + (padLeft ? "+" : "") + variable + (padRight ? "+" : "");

            if (padLeft || padRight)
            {
                int? size = GetAlignedLength(variable);
                if (size.HasValue)
                {
                    if (padLeft && padRight)
                        res = res.PadLeft(size.Value / 2).PadRight(size.Value - (size.Value / 2));
                    else if (padLeft)
                        res = res.PadLeft(size.Value);
                    else
                        res = res.PadRight(size.Value);
                }
            }

            return res;
        }

        protected virtual int? GetAlignedLength(string variable)
        {
            return null;
        }
        protected virtual string GetVariable(string variable)
        {
            return null;
        }
        protected virtual string GetAutoColor(string variable)
        {
            return string.Empty;
        }

        private string colorBlock(string format)
        {
            Match m = Regex.Match(format, "^(?<color>[^:]+):(?<content>.*)$", RegexOptions.Singleline);
            if (!m.Success)
                return null;

            string color_str = m.Groups["color"].Value;
            string content = m.Groups["content"].Value;

            if (color_str.ToLower() == "auto")
            {
                Match autoColor = Regex.Match(content, @"\$([a-z]|\+)+");

                if (autoColor.Success)
                {
                    string variable = autoColor.Value.Substring(1);
                    if (variable[0] == '+') variable = variable.Substring(1);
                    if (variable[variable.Length - 1] == '+') variable = variable.Substring(0, variable.Length - 1);

                    color_str = GetAutoColor(variable) ?? string.Empty;
                }
                else
                    color_str = string.Empty;
            }

            return $"[{color_str}:{Handle(content)}]";
        }
        protected virtual bool? ValidateCondition(string condition)
        {
            return null;
        }
        protected virtual string EvaluateFunction(string function, string[] args)
        {
            return "@" + function + "{" + string.Join("@", args) + "}";
        }

        private int findEnd(string text, int index, char open, char close)
        {
            int count = 0;
            do
            {
                if (text[index] == '\\') { index += 2; continue; }
                if (text[index] == open) count++;
                else if (text[index] == close) count--;
                index++;
            } while (count > 0 && index < text.Length);
            if (count == 0) index--;

            return index;
        }
    }
}
