using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using static System.ConsoleColor;

namespace ConsoleTools.Coloring
{
    public class ColorTable : IColorTable
    {
        public static ColorTable Empty { get; } = new ColorTable(ImmutableDictionary<string, ConsoleColor>.Empty);
        public static ColorTable System { get; } = new ColorTable
            (
                colors: ImmutableDictionary.CreateRange
                (
                    keyComparer: StringComparer.OrdinalIgnoreCase,
                    items: Enum.GetValues<ConsoleColor>().Select(k => KeyValuePair.Create(k.ToString(), k))
                )
            );
        public static ColorTable Default { get; } = System
            .With(Colors.ErrorMessage, foreground: Gray, background: null)
            .With(Colors.ErrorValue, foreground: White, background: null);

        private readonly IImmutableDictionary<string, ConsoleColor> _colors;

        public ColorTable(IImmutableDictionary<string, ConsoleColor> colors)
        {
            _colors = colors ?? throw new ArgumentNullException(nameof(colors));
        }

        public ColorTable With(string name, ConsoleColor color)
        {
            return new ColorTable
            (
                colors: _colors.SetItem(name, color)
            );
        }
        public ColorTable With(Color color, ConsoleColor? foreground, ConsoleColor? background)
        {
            var table = this;

            if (color.HasForeground)
            {
                if (foreground.HasValue)
                    table = table.With(color.Foreground!, foreground.Value);
                else
                    table = table.Without(color.Foreground!);
            }

            if (color.HasBackground)
            {
                if (background.HasValue)
                    table = table.With(color.Background!, background.Value);
                else
                    table = table.Without(color.Background!);
            }

            return table;
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
