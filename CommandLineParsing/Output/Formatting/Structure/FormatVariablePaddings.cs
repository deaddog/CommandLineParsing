namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Describes the different types of padding that can be applied to variables in a format structure.
    /// </summary>
    public enum FormatVariablePaddings
    {
        /// <summary>
        /// No padding should be applied to the variable.
        /// </summary>
        None,
        /// <summary>
        /// Padding should be applied to the left side of the variable.
        /// </summary>
        PadLeft,
        /// <summary>
        /// Padding should be applied to the right side of the variable.
        /// </summary>
        PadRight,
        /// <summary>
        /// Padding should be applied to the both sides of the variable (centering).
        /// </summary>
        PadBoth
    }
}
