using System.Collections.Generic;

namespace CommandLineParsing.Input
{
    /// <summary>
    /// Defines a collection of <see cref="MenuOption{T}"/> that is displayed by a <see cref="MenuDisplay{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the values managed by the <see cref="MenuOptionCollection{T}"/>.</typeparam>
    public class MenuOptionCollection<T>
    {
        private readonly MenuDisplay<T> _display;
        private readonly List<MenuOption<T>> _options;

        internal MenuOptionCollection(MenuDisplay<T> display)
        {
            _display = display;
            _options = new List<MenuOption<T>>();
        }
    }
}
