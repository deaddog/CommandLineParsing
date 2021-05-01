using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ConsoleTools.Colors
{
    public class ColorTable : IColorTable
    {
        static ColorTable()
        {
            Default = new ColorTable
            (
                colors: ImmutableDictionary.CreateRange
                (
                    keyComparer: StringComparer.OrdinalIgnoreCase,
                    items: Enum.GetValues<ConsoleColor>().Select(k => KeyValuePair.Create(k.ToString(), k))
                )
            );
        }
        public static ColorTable Default { get; }

        private readonly IImmutableDictionary<string, ConsoleColor> _colors;

        public ColorTable(IImmutableDictionary<string, ConsoleColor> colors)
        {
            _colors = colors ?? throw new ArgumentNullException(nameof(colors));
        }

        public ColorTable With(string name, ConsoleColor color)
        {
            return new ColorTable
            (
                colors: _colors.Add(name, color)
            );
        }
        public ColorTable Without(string name)
        {
            return new ColorTable
            (
                colors: _colors.Remove(name)
            );
        }

        public ConsoleColor? this[string name]
        {
            get
            {
                if (name is null)
                    throw new ArgumentNullException(nameof(name));

                if (_colors.TryGetValue(name, out ConsoleColor c)) return c;

                return null;
            }
        }
    }
}
