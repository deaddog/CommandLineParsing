using System;

namespace ConsoleTools.Formatting.Structure
{
    public class ConditionFormat : Format
    {
        public ConditionFormat(string name, bool isNegated, Format content)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsNegated = isNegated;
            Content = content ?? throw new ArgumentNullException(nameof(content));

            if (Name.Contains(" "))
                throw new ArgumentException("Condition names cannot contain spaces.", nameof(name));
        }

        public string Name { get; }
        public bool IsNegated { get; }
        public Format Content { get; }

        public override bool Equals(Format? other)
        {
            return other is ConditionFormat obj &&
                Name.Equals(obj.Name) &&
                IsNegated == obj.IsNegated &&
                Content.Equals(obj.Content);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, IsNegated, Content);
        }
    }
}
