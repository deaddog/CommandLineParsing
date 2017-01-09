namespace CommandLineParsing.Input
{
    /// <summary>
    /// Represent the event handler that handles <see cref="MenuOption{T}.TextChanged"/> events.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="option">The menu option associated with the event.</param>
    /// <param name="oldText">The old <see cref="MenuOption{T}.Text"/> from before the event was executed.</param>
    public delegate void MenuOptionTextChanged<TValue>(MenuOption<TValue> option, string oldText);
}
