using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Defines what type of console cleanup should be applied after displaying a menu.
    /// </summary>
    public enum MenuCleanup
    {
        /// <summary>
        /// No cleanup is applied.
        /// </summary>
        None,
        /// <summary>
        /// The menu is removed from the console.
        /// After an option has been selected, no part of the menu will remain visible.
        /// </summary>
        RemoveMenu,
        /// <summary>
        /// The menu is removed from the console.
        /// After an option has been selected, only the selected option will remain visible.
        /// </summary>
        RemoveMenuShowChoice
    }
}
