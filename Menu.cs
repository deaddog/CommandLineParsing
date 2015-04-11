using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a menu where each option is associated with an <see cref="Action"/> delegate.
    /// </summary>
    public class Menu : MenuBase<Action>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        /// <param name="labels">Defines the type of labeling used when displaying this menu.</param>
        public Menu(MenuLabeling labels)
            : base(labels)
        {
        }

        /// <summary>
        /// Sets the cancel option for the menu. No action will take place if the cancel option is selected.
        /// </summary>
        /// <param name="text">The text displayed for the cancel option.</param>
        public void SetCancel(string text)
        {
            base.SetCancel(text, () => { });
        }

        /// <summary>
        /// Shows the menu and waits for an option to be selected.
        /// When an option has been selected, its corresponding delegate is executed.
        /// </summary>
        /// <param name="cleanup">Determines what kind of console cleanup should be applied after displaying the menu.</param>
        public void Show(MenuCleanup cleanup)
        {
            ShowAndSelect(cleanup).Action();
        }
        /// <summary>
        /// Shows the menu and waits for an option to be selected.
        /// When an option has been selected, its corresponding delegate is executed.
        /// </summary>
        /// <param name="repeat">A boolean indicating whether the menu should be displayed repeatedly until the cancel option is selected.</param>
        /// <param name="showchoices">if set to <c>true</c> the chosen options are listed as they are selected in the menu.</param>
        public void Show(bool repeat, bool showchoices)
        {
            if (!this.CanCancel && repeat)
                throw new InvalidOperationException("A menu cannot auto-repeat without a cancel option.");

            MenuOption selected;
            do
            {
                selected = ShowAndSelect(showchoices ? MenuCleanup.RemoveMenuShowChoice : MenuCleanup.RemoveMenu);
                selected.Action();
            } while (repeat && !selected.IsCancel);
        }
    }
}
