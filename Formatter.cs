﻿using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    public partial class Formatter : IFormatter
    {
        private VariableCollection variables;
        private ConditionCollection conditions;
        private FunctionCollection functions;

        private Dictionary<Type, object> items;

        public Formatter()
        {
            this.variables = new VariableCollection();
            this.conditions = new ConditionCollection();
            this.functions = new FunctionCollection();

            this.items = new Dictionary<Type, object>();
        }

        public VariableCollection Variables => variables;
        public ConditionCollection Conditions => conditions;
        public FunctionCollection Functions => functions;

        public void Write<T>(T item, string format)
        {
            ColorConsole.Write(EvaluateFormat(item, format));
        }
        public void WriteLine<T>(T item, string format)
        {
            ColorConsole.WriteLine(EvaluateFormat(item, format));
        }

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
        public string EvaluateFormat<T>(IEnumerable<T> items, string format, string separator1)
        {
            using (var e = items.GetEnumerator())
            {
                if (!e.MoveNext())
                    return string.Empty;

                string res = EvaluateFormat(e.Current, format);

                while (e.MoveNext())
                    res += separator1 + EvaluateFormat(e.Current, format);

                return res;
            }
        }
        public string EvaluateFormat<T>(IEnumerable<T> items, string format, string separator1, string separator2)
        {
            using (var e = items.GetEnumerator())
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
