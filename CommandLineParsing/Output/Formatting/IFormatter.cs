namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Defines methods for the implementation of a simple string format.
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Gets the length (if any) that should be used when aligning a variable.
        /// If the variable must be printed in an aligned column that is x characters wide, x should be returned.
        /// This applies to any $+var, $var+ and $+var+ variables.
        /// </summary>
        /// <param name="variable">The variable to which padding might be applied. For a string of "$+var" only "var" will be the input to the method.</param>
        /// <returns>An <see cref="int"/> describing the padded size of the variable, if the variable is known and padding applies; otherwise <c>null</c>.</returns>
        int? GetAlignedLength(string variable);
        /// <summary>
        /// Gets the content of a domain-specific variable.
        /// </summary>
        /// <param name="variable">The variable for which content should be inserted. For a string of "$+var" only "var" will be the input to the method.</param>
        /// <returns>The string that the variable should be replaced by, if the variable is known; otherwise <c>null</c>.</returns>
        string GetVariable(string variable);
        /// <summary>
        /// Determines if color-information in a variable value should be preserved when rendering the variables content.
        /// If color-information is not preserved, any color markup is escaped and the string is rendered literally.
        /// </summary>
        /// <param name="variable">The variable for which color-information should/should not be preserved. For a string of "$+var" only "var" will be the input to the method.</param>
        /// <returns><c>true</c>, if color-information should be preserved; <c>false</c> otherwise.</returns>
        bool GetPreserveColor(string variable);
        /// <summary>
        /// Gets the color from a variable. This will typically apply to variables where color is determined from some state (open or closed).
        /// </summary>
        /// <param name="variable">The variable for which automated coloring should be determined. For a string of "$+var" only "var" will be the input to the method.</param>
        /// <returns>The color that should be applied to the variable, if the variable is known and automated coloring applies; otherwise <c>null</c>.
        /// Colors are returned as <see cref="string"/> and can then be looked up in the <see cref="ColorConsole.Colors"/> table.</returns>
        string GetAutoColor(string variable);

        /// <summary>
        /// Validates a condition on the form ?condition{content}.
        /// </summary>
        /// <param name="condition">The name of the condition to test for.</param>
        /// <returns>The boolean value of the condition (if it was met given the current state) if the condition exists; otherwise <c>null</c>.</returns>
        bool? ValidateCondition(string condition);
        /// <summary>
        /// Evaluates a function (with parameters) and returns its result.
        /// </summary>
        /// <param name="function">The name of the function that is to be executed.</param>
        /// <param name="args">An array of arguments for execution of the function.</param>
        /// <returns>The result of evaluating the function if it exists and supports the given arguments; otherwise <c>null</c>.</returns>
        string EvaluateFunction(string function, string[] args);
    }
}
