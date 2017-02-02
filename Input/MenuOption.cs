using CommandLineParsing.Output;

namespace CommandLineParsing.Input
{
    /// <summary>
    /// Represemts a simple option in a menu with an associated value.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with the <see cref="MenuOption{T}"/>.</typeparam>
    public class MenuOption<T> : IMenuOption
    {
        private ConsoleString text;

        /// <summary>
        /// Gets or sets the text displayed in the menu for this option.
        /// </summary>
        public ConsoleString Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;

                text = value;
                TextChanged?.Invoke(this);
            }
        }
        /// <summary>
        /// Gets the value associated with this menu option.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuOption{T}"/> class.
        /// </summary>
        /// <param name="text">The text displayed in the menu for this option. The value can be updated while the menu is displayed.</param>
        /// <param name="value">The value associated with this menu option.</param>
        public MenuOption(ConsoleString text, T value)
        {
            Text = text;
            Value = value;
        }

        /// <summary>
        /// Occurs when <see cref="Text"/> changes value.
        /// </summary>
        public event MenuOptionTextChanged TextChanged;
    }
}
