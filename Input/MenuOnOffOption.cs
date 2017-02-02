using CommandLineParsing.Output;

namespace CommandLineParsing.Input
{
    /// <summary>
    /// Represents a menu option with a On/Off switch and an associated value.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with the <see cref="MenuOption{T}"/>.</typeparam>
    public class MenuOnOffOption<T> : IMenuOption
    {
        private readonly ConsoleString _onText, _offText;
        private bool _on;

        /// <summary>
        /// Gets the text displayed in the menu for this option.
        /// </summary>
        public ConsoleString Text => _on ? _onText : _offText;
        /// <summary>
        /// Gets the value associated with this menu option.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuOnOffOption{T}"/> class.
        /// </summary>
        /// <param name="onText">The text displayed in the menu for this option, when <see cref="On"/> is <c>true</c>.</param>
        /// <param name="offText">The text displayed in the menu for this option, when <see cref="On"/> is <c>false</c>.</param>
        /// <param name="on">The initial on/off state of this option. The value can be update while the menu is displayed.</param>
        /// <param name="value">The value associated with this menu option.</param>
        public MenuOnOffOption(ConsoleString onText, ConsoleString offText, bool on, T value)
        {
            _onText = onText;
            _offText = offText;
            _on = on;
        }

        /// <summary>
        /// Occurs when <see cref="Text"/> changes value.
        /// </summary>
        public event MenuOptionTextChanged TextChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MenuOnOffOption{T}"/> is selected in a menu display.
        /// Setting the property will switch the text displayed in the menu.
        /// </summary>
        public bool On
        {
            get { return _on; }
            set
            {
                if (_on == value)
                    return;

                _on = value;
                TextChanged?.Invoke(this);
            }
        }
    }
}
