using System;

namespace ConsoleTools.Formatting.Structure
{
    public class VariableFormat : Format
    {
        public enum Paddings
        {
            None,
            PadLeft,
            PadRight,
            PadBoth
        }

        public VariableFormat(string name, Paddings padding)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Padding = padding;

            if (Name.Contains(" "))
                throw new ArgumentException("Variable names cannot contain spaces.", nameof(name));
        }

        public string Name { get; }
        public Paddings Padding { get; }

        public override bool Equals(Format? other)
        {
            return other is VariableFormat obj &&
                Name.Equals(obj.Name) &&
                Padding == obj.Padding;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Padding);
        }
    }
}
