namespace CommandLineParsing.Input
{
    /// <summary>
    /// Represent the event handler that handles <see cref="IMenuOption.TextChanged"/> events.
    /// </summary>
    /// <param name="option">The menu option associated with the event.</param>
    /// <param name="oldText">The old <see cref="IMenuOption.Text"/> from before the event was executed.</param>
    public delegate void MenuOptionTextChanged(IMenuOption option, string oldText);
}
