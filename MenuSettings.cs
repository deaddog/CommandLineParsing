using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Encapsulates settings for how a menu is displayed.
    /// </summary>
    public class MenuSettings
    {
        private MenuLabeling labeling;
        private MenuCleanup cleanup;
        private string indentation;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuSettings"/> class.
        /// All options are set trough properties.
        /// </summary>
        public MenuSettings()
        {
            this.labeling = MenuLabeling.NumbersAndLetters;
            this.cleanup = MenuCleanup.None;
            this.indentation = string.Empty;
        }

        /// <summary>
        /// Gets or sets the labeling used for menu options.
        /// </summary>
        public MenuLabeling Labeling
        {
            get { return labeling; }
            set { labeling = value; }
        }
        /// <summary>
        /// Gets or sets the cleanup that should be performed after the menu has been shown.
        /// </summary>
        public MenuCleanup Cleanup
        {
            get { return cleanup; }
            set { cleanup = value; }
        }
        /// <summary>
        /// Gets or sets a string that should be used to indent the menu options. Each option is prepended by this value.
        /// </summary>
        public string Indentation
        {
            get { return indentation; }
            set { indentation = value; }
        }
    }
}
