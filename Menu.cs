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
        public void Show()
        {
            this.Show(false);
        }
        /// <summary>
        /// Shows the menu and waits for an option to be selected.
        /// When an option has been selected, its corresponding delegate is executed.
        /// </summary>
        /// <param name="repeat">A boolean indicating whether the menu should be displayed repeatedly until the cancel option is selected.</param>
        public void Show(bool repeat)
        {
            if (!this.CanCancel && repeat)
                throw new InvalidOperationException("A menu cannot auto-repeat without a cancel option.");

            MenuOption selected;
            do
            {
                selected = ShowAndSelect();
                selected.Action();
            } while (repeat && !selected.IsCancel);
        }
    }
}
