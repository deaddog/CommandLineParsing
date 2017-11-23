using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a sequence of multiple format elements.
    /// </summary>
    public class FormatConcatenation : FormatElement
    {
        /// <summary>
        /// Gets the sequence of elements.
        /// </summary>
        public ReadOnlyCollection<FormatElement> Elements { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatConcatenation"/> class.
        /// </summary>
        /// <param name="elements">A sequence of elements.</param>
        public FormatConcatenation(IEnumerable<FormatElement> elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));
            
            Elements = new ReadOnlyCollection<FormatElement>(elements.ToList());
        }
    }
}
