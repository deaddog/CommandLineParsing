using ConsoleTools.Coloring;
using System;
using System.Collections.Immutable;

namespace ConsoleTools.Formatting
{
    public class Variable<T>
    {
        public Variable(Func<T, string> selector, int? paddedLength, IImmutableDictionary<string, Func<T, Color>> dynamicColors)
        {
            if (paddedLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(paddedLength));

            PaddedLength = paddedLength;

            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
            DynamicColors = dynamicColors ?? throw new ArgumentNullException(nameof(dynamicColors));
        }

        public int? PaddedLength { get; }

        public Func<T, string> Selector { get; }
        public IImmutableDictionary<string, Func<T, Color>> DynamicColors { get; }
    }
}
