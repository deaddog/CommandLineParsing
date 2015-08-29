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
        
        public class VariableCollection
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
