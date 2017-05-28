namespace CommandLineParsing.Input
{
    /// <summary>
    /// Represent the event handler that handles <see cref="MenuOptionCollection{TOption}.CollectionChanged" /> events.
    /// </summary>
    /// <typeparam name="TOption">The type of the options managed by the <see cref="MenuOptionCollection{TOption}"/>.</typeparam>
    /// <param name="collection">The collection of menu options that has changed.</param>
    /// <param name="updateType">Type of the update.</param>
    /// <param name="index">The index of the first element affected by the change.</param>
    /// <param name="count">The number of elements affected by the change.</param>
    public delegate void CollectionChanged<TOption>(MenuOptionCollection<TOption> collection, CollectionUpdateTypes updateType, int index, int count) where TOption : class, IMenuOption;
}
