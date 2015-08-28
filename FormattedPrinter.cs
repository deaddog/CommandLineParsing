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
        public FormattedPrinter(string format)
        {
            this.format = format;
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
