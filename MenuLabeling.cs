namespace CommandLineParsing
{
    /// <summary>
    /// Defines the labels used when displaying menues.
    /// </summary>
    public enum MenuLabeling
    {
        /// <summary>
        /// No labeling is used.
        /// </summary>
        None,
        /// <summary>
        /// Numbers (1-9) are used.
        /// </summary>
        Numbers,
        /// <summary>
        /// Letters (a-z) are used.
        /// </summary>
        Letters,
        /// <summary>
        /// Letters in upper case (A-Z) are used.
        /// </summary>
        LettersUpper,
        /// <summary>
        /// Numbers (1-9) and then letters (a-z) are used.
        /// </summary>
        NumbersAndLetters,
        /// <summary>
        /// Numbers (1-9) and then letters in upper case (A-Z) are used.
        /// </summary>
        NumbersAndLettersUpper
    }
}
