using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    public partial class Formatter
    {
        internal class Variable
        {
            public readonly Type Type;
            public readonly Func<object, string> Replace;
            public readonly Func<object, string> AutoColor;
            public readonly int? Padding;

            public Variable(Type type, Func<object, string> replace, Func<object, string> autoColor, int? padding)
            {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                if (replace == null)
                    throw new ArgumentNullException(nameof(replace));

                // providing null for the color function indicates that auto-color cannot be applied to the variable.

                this.Type = type;
                this.Replace = replace;
                this.AutoColor = autoColor;
                this.Padding = padding;
            }
        }
        internal class Condition
        {
            public readonly Type Type;
            public readonly Func<object, bool> Check;

            public Condition(Type type, Func<object, bool> check)
            {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                if (check == null)
                    throw new ArgumentNullException(nameof(check));

                this.Type = type;
                this.Check = check;
            }
        }
        internal class Function
        {
            public readonly Type Type;
            public readonly Func<object, string[], string> Func;

            public Function(Type type, Func<object, string[], string> function)
            {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                if (function == null)
                    throw new ArgumentNullException(nameof(function));

                this.Type = type;
                this.Func = function;
            }
        }

        public class VariableCollection
        {
            private Dictionary<string, Variable> elements;

            internal VariableCollection()
            {
                this.elements = new Dictionary<string, Variable>();
            }

            internal void Add(string identifier, Variable variable)
            {
                if (identifier == null)
                    throw new ArgumentNullException(nameof(identifier));

                if (variable == null)
                    throw new ArgumentNullException(nameof(variable));

                elements.Add(identifier, variable);
            }
            internal bool TryGet(string identifier, out Variable variable)
            {
                return elements.TryGetValue(identifier, out variable);
            }
        }

        public class ConditionCollection
        {
            private Dictionary<string, Condition> elements;

            internal ConditionCollection()
            {
                this.elements = new Dictionary<string, Condition>();
            }

            internal void Add(string identifier, Condition condition)
            {
                if (identifier == null)
                    throw new ArgumentNullException(nameof(identifier));

                if (condition == null)
                    throw new ArgumentNullException(nameof(condition));

                elements.Add(identifier, condition);
            }
            internal bool TryGet(string identifier, out Condition condition)
            {
                return elements.TryGetValue(identifier, out condition);
            }
        }

        public class FunctionCollection
        {
            private Dictionary<string, List<Function>> functions;

            internal FunctionCollection()
            {
                this.functions = new Dictionary<string, List<Function>>();
            }

            private void Add(string name, Function function)
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                if (function == null)
                    throw new ArgumentNullException(nameof(function));

                if (!functions.ContainsKey(name))
                    functions[name] = new List<Function>();

                functions[name].Add(function);
            }
            internal bool TryGet(string name, out Function[] functions)
            {
                List<Function> list;
                if (this.functions.TryGetValue(name, out list))
                {
                    functions = list.ToArray();
                    return true;
                }
                else
                {
                    functions = null;
                    return false;
                }
            }
        }
    }
}
