using ConsoleTools.Formatting.Structure;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ConsoleTools.Formatting
{
    public class ListFunction<T, TItem> : IFunction<T>
    {
        private readonly Func<T, IEnumerable<TItem>> _itemsSelector;
        private readonly IFormatter<TItem> _itemFormatter;

        public ListFunction(Func<T, IEnumerable<TItem>> itemsSelector, IFormatter<TItem> itemFormatter)
        {
            _itemsSelector = itemsSelector ?? throw new ArgumentNullException(nameof(itemsSelector));
            _itemFormatter = itemFormatter ?? throw new ArgumentNullException(nameof(itemFormatter));
        }

        public ConsoleString Evaluate(T item, IImmutableList<Format> arguments)
        {
            if (arguments.Count == 0)
                return ConsoleString.Empty;

            var items = _itemsSelector(item).ToImmutableList();
            var formats = arguments
                .RemoveAt(0)
                .Insert(0, NoContentFormat.Element)
                .Select(f => Format.Combine(f, arguments[0]))
                .ToImmutableList();

            if (formats.Count > items.Count)
                formats = formats.RemoveRange(1, formats.Count - items.Count);
            else if (formats.Count < items.Count)
                formats = formats.InsertRange(1, Enumerable.Repeat(formats[1], items.Count - formats.Count));

            return items
                .Select((item, i) => _itemFormatter.Format(formats[Math.Max(formats.Count - items.Count + i, 0)], items[i]))
                .Aggregate(ConsoleString.Empty, (a, b) => a + b);
        }
    }
}
