using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    public partial class Formatter
    {
        public class Item
        {
            public readonly Type Type;

            internal Item(Type type)
            {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                this.Type = type;
            }
        }
        public class Collection<T> where T : Item
        {
            private Dictionary<string, T> elements;

            internal Collection()
            {
                this.elements = new Dictionary<string, T>();
            }

            internal bool TryGet(string identifier, out T item)
            {
                return elements.TryGetValue(identifier, out item);
            }
        }
        
        public class Variable : Item
        {
            public readonly Func<object, string> Replace;
            public readonly Func<object, string> AutoColor;
            public readonly int? Padding;

            internal Variable(Type type, Func<object, string> replace, Func<object, string> autoColor, int? padding)
                : base(type)
            {
                if (replace == null)
                    throw new ArgumentNullException(nameof(replace));

                // providing null for the color function indicates that auto-color cannot be applied to the variable.

                this.Replace = replace;
                this.AutoColor = autoColor;
                this.Padding = padding;
            }
        }
        public class VariableCollection : Collection<Variable>
        {
        }
        
        public class ConditionCollection
        {
        }
        
        public class FunctionCollection
        {
        }
    }
}
