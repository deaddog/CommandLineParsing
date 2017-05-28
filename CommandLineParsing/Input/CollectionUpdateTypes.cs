namespace CommandLineParsing.Input
{
    /// <summary>
    /// Represents the different types of changes that can occur in a <see cref="MenuOptionCollection{TOption}"/>.
    /// </summary>
    public enum CollectionUpdateTypes
    {
        /// <summary>
        /// The collection is cleared.
        /// </summary>
        Clear,
        /// <summary>
        /// New elements are inserted into the collection.
        /// </summary>
        Insert,
        /// <summary>
        /// Elements are removed from the collection.
        /// </summary>
        Remove,
        /// <summary>
        /// Collection elements are replaced by new elements. The number of elements removed will exactly match the number added.
        /// </summary>
        Replace,
        /// <summary>
        /// A change has occured in an element, requiring an update.
        /// </summary>
        Update
    }
}
