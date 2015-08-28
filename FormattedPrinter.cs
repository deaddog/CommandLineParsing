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
        /// - <code>"$variable+"</code>, <code>"$+variable"</code> or <code>"$+variable+"</code> | Allows for padding of a variable by calling <see cref="GetAlignedLength(string)"/> with <code>"variable"</code> as parameter. The location of the + indicates which end of the variable that is padded. $+variable+ indicates centering.
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

        /// <summary>
        /// Evaluates the format of this <see cref="FormattedPrinter"/> and prints the result using the <see cref="ColorConsole"/>.
        /// </summary>
        protected void PrintFormat()
        {
            ColorConsole.Write(EvaluateFormat());
        }
        /// <summary>
        /// Evaluates the format of this <see cref="FormattedPrinter"/> and prints the result, followed by the current line terminator, using the <see cref="ColorConsole"/>.
        /// </summary>
        protected void PrintFormatLine()
        {
            ColorConsole.WriteLine(EvaluateFormat());
        }

        /// <summary>
        /// Evaluates the format string managed by this <see cref="FormattedPrinter"/> given its current state, by applying the format translation.
        /// </summary>
        /// <returns>The result of the evaluation.</returns>
        protected string EvaluateFormat()
        {
            return Evaluate(format);
        }

        /// <summary>
        /// When overriden in a derived class; gets the length (if any) that should be used when aligning a variable.
        /// If the variable must be printed in an aligned column that is x characters wide, x should be returned.
        /// This applies to any $+var, $var+ and $+var+ variables.
        /// </summary>
        /// <param name="variable">The variable to which padding might be applied. For a string of "$+var" only "var" will be the input to the method.</param>
        /// <returns><c>null</c> if the variable is not known; otherwise an int describing the padded size of the variable.</returns>
        protected virtual int? GetAlignedLength(string variable)
        {
            return null;
        }
        /// <summary>
        /// When overridden in a derived class; gets the content of a domain-specific variable.
        /// </summary>
        /// <param name="variable">The variable that should be replaced by some other content. For a string of "$+var" only "var" will be the input to the method.</param>
        /// <returns>The string that the variable should be replaced by.</returns>
        protected virtual string GetVariable(string variable)
        {
            return null;
        }
        /// <summary>
        /// Gets the color from a variable. This will typically apply to variables where color is determined from some state (open or closed).
        /// </summary>
        /// <param name="variable">The variable for which automated coloring should be determined. For a string of "$+var" only "var" will be the input to the method.</param>
        /// <returns>The color that should be applied to the variable.</returns>
        public string GetAutoColor(string variable)
        {
            return string.Empty;
        }

        /// <summary>
        /// Validates a condition on the form ?condition{content}.
        /// </summary>
        /// <param name="condition">The name of the condition to test for.</param>
        /// <returns><c>null</c>, if the condition does not exist. If it does than <c>true</c> if the condition evaluates to <c>true</c>; otherwise <c>false</c>.</returns>
        protected virtual bool? ValidateCondition(string condition)
        {
            return null;
        }
        /// <summary>
        /// Evaluates a function (with parameters) and returns its result.
        /// </summary>
        /// <param name="function">The name of the function that is to be executed.</param>
        /// <param name="args">An array of arguments for execution of the function.</param>
        /// <returns>The result of evaluating the function.</returns>
        public string EvaluateFunction(string function, string[] args)
        {
            return null;
        }
    }
}
