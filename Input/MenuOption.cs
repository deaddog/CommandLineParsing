namespace CommandLineParsing.Input
{
    /// <summary>
    /// Represemts a single option in a menu.
    /// </summary>
    public abstract class MenuOption<T>
    {
        /// <summary>
        /// The text displayed in the menu for this option.
        /// </summary>
        public abstract string Text { get; }
        /// <summary>
        /// The value associated with this menu option.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuOption{T}"/> class.
        /// </summary>
        /// <param name="value">The value assocated with the option.</param>
        public MenuOption(T value)
        {
            Value = value;
        }
    }
}
