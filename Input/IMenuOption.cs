namespace CommandLineParsing.Input
{
    /// <summary>
    /// Represemts a single option in a menu.
    /// </summary>
    public interface IMenuOption
    {
        /// <summary>
        /// Gets the text displayed in the menu for this option.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Occurs when <see cref="Text"/> changes value.
        /// </summary>
        event MenuOptionTextChanged TextChanged;
    }
}
