using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a function evaluation part of a format structure.
    /// </summary>
    public class FormatFunction : FormatElement, IEquatable<FormatFunction>
    {
        /// <summary>
        /// Gets the name of the function that should be evaluated.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the arguments specified for evaluation of the function.
        /// </summary>
        public ReadOnlyCollection<FormatElement> Arguments { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatFunction"/> class.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="arguments">The function arguments.</param>
        public FormatFunction(string name, IEnumerable<FormatElement> arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            Arguments = new ReadOnlyCollection<FormatElement>(arguments.ToList());
        }

#pragma warning disable CS1591
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Arguments.GetHashCode();
        }

        public bool Equals(FormatFunction other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else
                return Name.Equals(other.Name) && Arguments.Count.Equals(other.Arguments.Count) &&
                    Arguments.Zip(other.Arguments, (x, y) => x.Equals(y)).All(x => x);
        }
        public override bool Equals(FormatElement other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else if (other is FormatFunction func)
                return Equals(func);
            else
                return false;
        }
#pragma warning restore CS1591
    }
}
