using ConsoleTools.Coloring;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ConsoleTools.Formatting
{
    public static class FormatterExtensions
    {
        public static Formatter<T> With<T>(this Formatter<T> formatter, string name, Func<T, string> selector)
        {
            return With(formatter, name, selector, x => x);
        }
        public static Formatter<T> With<T>(this Formatter<T> formatter, string name, Func<T, string> selector, Func<Variable<T>, Variable<T>> configure)
        {
            return formatter.WithVariable(name, configure(new Variable<T>
            (
                selector: selector,
                paddedLength: null,
                dynamicColors: ImmutableDictionary<string, Func<T, Color>>.Empty
            )));
        }

        public static Formatter<T> WithTyped<T, TValue>(this Formatter<T> formatter, string name, Func<T, TValue> selector)
        {
            return WithTyped(formatter, name, selector, x => x);
        }
        public static Formatter<T> WithTyped<T, TValue>(this Formatter<T> formatter, string name, Func<T, TValue> selector, Func<Variable<TValue>, Variable<TValue>> configure)
        {
            var variable = configure(new Variable<TValue>
            (
                selector: x => x?.ToString() ?? string.Empty,
                paddedLength: null,
                dynamicColors: ImmutableDictionary<string, Func<TValue, Color>>.Empty
            ));

            static Func<T, Color> Wrap(Func<T, TValue> value, Func<TValue, Color> selector) => (T x) => selector(value(x));

            return formatter.WithVariable(name, new Variable<T>
            (
                selector: x => variable.Selector(selector(x)),
                paddedLength: variable.PaddedLength,
                dynamicColors: variable.DynamicColors.ToImmutableDictionary
                (
                    keySelector: x => x.Key,
                    elementSelector: x => Wrap(selector, x.Value)
                )
            ));
        }

        public static Formatter<T> WithListFunction<T, TItem>(this Formatter<T> formatter, string name, Func<T, IEnumerable<TItem>> itemsSelector, IFormatter<TItem> itemFormatter)
        {
            return formatter.WithFunction
            (
                name: name,
                function: new ListFunction<T, TItem>
                (
                    itemsSelector: itemsSelector,
                    itemFormatter: itemFormatter
                )
            );
        }
        public static Formatter<T> WithListFunction<T, TItem>(this Formatter<T> formatter, string name, Func<T, IEnumerable<TItem>> itemsSelector, Func<Formatter<TItem>, Formatter<TItem>> itemFormatter)
        {
            return WithListFunction
            (
                formatter: formatter,
                name: name,
                itemsSelector: itemsSelector,
                itemFormatter: itemFormatter(Formatter<TItem>.Empty)
            );
        }
    }
}
