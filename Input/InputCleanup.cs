namespace CommandLineParsing.Input
{
    /// <summary>
    /// Defines what type of console cleanup should be applied after retrieving console input.
    /// </summary>
    public enum InputCleanup
    {
        /// <summary>
        /// No cleanup is applied.
        /// </summary>
        None,
        /// <summary>
        /// Removes the input from the console.
        /// </summary>
        Clean,
    }
}
