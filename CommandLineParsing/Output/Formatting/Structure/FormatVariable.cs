using System;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a variable part of a format structure.
    /// </summary>
    public class FormatVariable : FormatElement
    {
        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the type of padding that should be applied to the variable.
        /// </summary>
        public FormatPaddings Padding { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatVariable"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="padding">The type of padding applied to the variable.</param>
        public FormatVariable(string name, FormatPaddings padding)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Padding = padding;

            if (Name.Contains(" "))
                throw new ArgumentException("Variable names cannot contain spaces.", nameof(name));
        }
    }
}
