using System;

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
        /// Numbers (0-9) are used.
        /// </summary>
        Numbers,
        /// <summary>
        /// Letters (a-z) are used.
        /// </summary>
        Letters,
        /// <summary>
        /// Numbers (0-9) and then letters (a-z) are used.
        /// </summary>
        NumbersAndLetters
    }
}
