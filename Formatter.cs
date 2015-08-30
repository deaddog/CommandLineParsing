using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Uses a set of specialized collections to set up a custom formatter.
    /// Setting up a <see cref="Formatter"/> is done with generic methods.
    /// </summary>
    public partial class Formatter : IFormatter
    {
        private VariableCollection variables;
        private ConditionCollection conditions;
        private FunctionCollection functions;

        private Dictionary<Type, object> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="Formatter"/> class.
        /// </summary>
        public Formatter()
        {
            this.variables = new VariableCollection();
            this.conditions = new ConditionCollection();
            this.functions = new FunctionCollection(this);

            this.items = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Gets the collection of variables associated with this <see cref="Formatter"/>.
        /// </summary>
        public VariableCollection Variables => variables;
        /// <summary>
        /// Gets the collection of conditions associated with this <see cref="Formatter"/>.
        /// </summary>
        public ConditionCollection Conditions => conditions;
        /// <summary>
        /// Gets the collection of functions associated with this <see cref="Formatter"/>.
        /// </summary>
        public FunctionCollection Functions => functions;

        /// <summary>
        /// Writes an item to the console using the rules defined in this <see cref="Formatter"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item that should written to the console.</typeparam>
        /// <param name="item">The item that should be written to console.</param>
        /// <param name="format">The format applied by this <see cref="Formatter"/> when writing.
        /// See <see cref="ColorConsole.EvaluateFormat(string, IFormatter)"/> for format defails.</param>
        public void Write<T>(T item, string format)
        {
            ColorConsole.Write(EvaluateFormat(item, format));
        }
        /// <summary>
        /// Writes an item to the console, followed by the current line terminator, using the rules defined in this <see cref="Formatter"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item that should written to the console.</typeparam>
        /// <param name="item">The item that should be written to console.</param>
        /// <param name="format">The format applied by this <see cref="Formatter"/> when writing.
        /// See <see cref="ColorConsole.EvaluateFormat(string, IFormatter)"/> for format defails.</param>
        public void WriteLine<T>(T item, string format)
        {
            ColorConsole.WriteLine(EvaluateFormat(item, format));
        }

        /// <summary>
        /// Evaluates a format given a specific item.
        /// </summary>
        /// <typeparam name="T">The type of the item that should formatted.</typeparam>
        /// <param name="item">The item that should be formatted.</param>
        /// <param name="format">The format applied by this <see cref="Formatter"/> when evaluating.
        /// See <see cref="ColorConsole.EvaluateFormat(string, IFormatter)"/> for format defails.</param>
        /// <returns>The result of the string translation.</returns>
        public string EvaluateFormat<T>(T item, string format)
        {
            var type = typeof(T);
            bool hasOld = items.ContainsKey(type);
            object old = hasOld ? items[type] : null;

            items[type] = item;
            string res = ColorConsole.EvaluateFormat(format, this);

            if (hasOld)
                items[type] = old;
            else
                items.Remove(type);

            return res;
        }
        /// <summary>
        /// Evaluates a format of each item in a collection of items, joining them by a string.
        /// </summary>
        /// <typeparam name="T">The type of the items that should be formatted.</typeparam>
        /// <param name="collection">The collection of items that should be formatted.</param>
        /// <param name="format">The format applied to each element in <paramref name="collection"/> by this <see cref="Formatter"/> when evaluating.
        /// See <see cref="ColorConsole.EvaluateFormat(string, IFormatter)"/> for format defails.</param>
        /// <param name="separator1">A string that is placed between the formatted items.</param>
        /// <returns>The result of the string translation of each item.</returns>
        public string EvaluateFormat<T>(IEnumerable<T> collection, string format, string separator1)
        {
            using (var e = collection.GetEnumerator())
            {
                if (!e.MoveNext())
                    return string.Empty;

                string res = EvaluateFormat(e.Current, format);

                while (e.MoveNext())
                    res += separator1 + EvaluateFormat(e.Current, format);

                return res;
            }
        }
        /// <summary>
        /// Evaluates a format of each item in a collection of items, joining them by a string.
        /// </summary>
        /// <typeparam name="T">The type of the items that should be formatted.</typeparam>
        /// <param name="collection">The collection of items that should be formatted.</param>
        /// <param name="format">The format applied to each element in <paramref name="collection"/> by this <see cref="Formatter"/> when evaluating.
        /// See <see cref="ColorConsole.EvaluateFormat(string, IFormatter)"/> for format defails.</param>
        /// <param name="separator1">A string that is placed between the formatted items (except the final two).</param>
        /// <param name="separator2">A string that is placed between the final two formatted items.</param>
        /// <returns>The result of the string translation of each item.</returns>
        public string EvaluateFormat<T>(IEnumerable<T> collection, string format, string separator1, string separator2)
        {
            using (var e = collection.GetEnumerator())
            {

                if (!e.MoveNext())
                    return string.Empty;

                string res = EvaluateFormat(e.Current, format);

                if (!e.MoveNext())
                    return res;

                var temp = e.Current;
                while (e.MoveNext())
                {
                    res += separator1 + EvaluateFormat(temp, format);
                    temp = e.Current;
                }

                return res + separator2 + EvaluateFormat(temp, format);
            }
        }

        string IFormatter.GetVariable(string variable)
        {
            Variable v;
            if (!variables.TryGet(variable, out v))
                return null;

            object item;
            if (!items.TryGetValue(v.Type, out item))
                return null;

            return v.Replace.Invoke(item);
        }
        string IFormatter.GetAutoColor(string variable)
        {
            Variable v;
            if (!variables.TryGet(variable, out v))
                return null;

            object item;
            if (!items.TryGetValue(v.Type, out item))
                return null;

            return v.AutoColor?.Invoke(item);
        }
        int? IFormatter.GetAlignedLength(string variable)
        {
            Variable v;
            if (!variables.TryGet(variable, out v))
                return null;

            return v.Padding;
        }

        bool? IFormatter.ValidateCondition(string condition)
        {
            Condition c;
            if (!conditions.TryGet(condition, out c))
                return null;

            object item;
            if (!items.TryGetValue(c.Type, out item))
                return null;

            return c.Check?.Invoke(item);
        }
        string IFormatter.EvaluateFunction(string function, string[] args)
        {
            Function[] f;
            if (!functions.TryGet(function, out f))
                return null;

            for (int i = 0; i < f.Length; i++)
            {
                object item;
                if (!items.TryGetValue(f[i].Type, out item))
                    continue;

                var res = f[i].Func(item, args);
                if (res != null)
                    return res;
            }

            return null;
        }
    }
}
