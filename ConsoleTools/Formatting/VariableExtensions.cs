using ConsoleTools.Coloring;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleTools.Formatting
{
    public static class VariableExtensions
    {
        public static Variable<T> WithPaddedLength<T>(this Variable<T> variable, int length)
        {
            return new Variable<T>
            (
                selector: variable.Selector,
                paddedLength: length,
                dynamicColors: variable.DynamicColors
            );
        }
        public static Variable<T> WithPaddedLengthFrom<T>(this Variable<T> variable, IEnumerable<T> collection)
        {
            return variable.WithPaddedLength(collection.Select(x => variable.Selector(x).Length).Concat(new[] { 0 }).Max());
        }

        public static Variable<T> WithDynamicColor<T>(this Variable<T> variable, string color, Func<T, string?> colorSelector)
        {
            return WithDynamicColor(variable, color, x => Color.Parse(colorSelector(x) ?? string.Empty));
        }
        public static Variable<T> WithDynamicColor<T>(this Variable<T> variable, string color, Func<T, Color> colorSelector)
        {
            return new Variable<T>
            (
                selector: variable.Selector,
                paddedLength: variable.PaddedLength,
                dynamicColors: variable.DynamicColors.SetItem(color, colorSelector)
            );
        }
        public static Variable<T> WithAutoColor<T>(this Variable<T> variable, Func<T, string?> colorSelector)
        {
            return WithAutoColor(variable, x => Color.Parse(colorSelector(x) ?? string.Empty));
        }
        public static Variable<T> WithAutoColor<T>(this Variable<T> variable, Func<T, Color> colorSelector)
        {
            return variable.WithDynamicColor("auto", colorSelector);
        }
    }
}
