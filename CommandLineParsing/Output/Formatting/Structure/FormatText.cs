﻿using System;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a text-only part of a format structure.
    /// </summary>
    public class FormatText : FormatElement, IEquatable<FormatText>
    {
        /// <summary>
        /// Gets the text represented in the format, without any additional structure.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatText"/> class.
        /// </summary>
        /// <param name="text">The format text.</param>
        public FormatText(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

#pragma warning disable CS1591
        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public bool Equals(FormatText other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else
                return Text.Equals(other.Text);
        }
        public override bool Equals(FormatElement other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else if (other is FormatText text)
                return Equals(text);
            else
                return false;
        }
#pragma warning restore CS1591
    }
}
