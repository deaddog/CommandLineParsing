using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Encapsulates settings for how a menu is displayed.
    /// </summary>
    public class MenuSettings : ICloneable
    {
        private MenuLabeling labeling;
        private MenuCleanup cleanup;
        private string indentation;
        private uint minimum, maximum;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuSettings"/> class.
        /// All options are set trough properties.
        /// </summary>
        public MenuSettings()
        {
            this.labeling = MenuLabeling.NumbersAndLetters;
            this.cleanup = MenuCleanup.None;
            this.indentation = string.Empty;
            this.minimum = 0;
            this.maximum = uint.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuSettings"/> class as a copy of an existing <see cref="MenuSettings"/> instance.
        /// </summary>
        /// <param name="settings">The settings to copy.</param>
        public MenuSettings(MenuSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            this.labeling = settings.labeling;
            this.cleanup = settings.cleanup;
            this.indentation = settings.indentation;
            this.minimum = settings.minimum;
            this.maximum = settings.maximum;
        }

        object ICloneable.Clone()
        {
            return new MenuSettings(this);
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

        /// <summary>
        /// Gets or sets the minimum number of items that must be selected in a <see cref="SelectionMenu{T}"/>.
        /// If this value is greater than or equal to the number of items displayed by the menu, all items must be selected.
        /// This setting does not apply to displaying <see cref="Menu{T}"/>.
        /// </summary>
        public uint MinimumSelected
        {
            get { return minimum; }
            set
            {
                if (value > maximum)
                    throw new ArgumentOutOfRangeException(nameof(value), $"The {MinimumSelected} value must be less than or equal to the {MaximumSelected} value.");

                minimum = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum number of items that can be selected in a <see cref="SelectionMenu{T}"/>.
        /// If this is set to <c>1</c> you should consider using a <see cref="Menu{T}"/> instead.
        /// This setting does not apply to displaying <see cref="Menu{T}"/>.
        /// </summary>
        public uint MaximumSelected
        {
            get { return maximum; }
            set
            {
                if (value < minimum)
                    throw new ArgumentOutOfRangeException(nameof(value), $"The {MaximumSelected} value must be greater than or equal to the {MinimumSelected} value.");

                maximum = value;
            }
        }
    }
}
