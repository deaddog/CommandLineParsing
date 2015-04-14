using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents the baseclass for a menu displayed by the <see cref="Console"/> type.
    /// </summary>
    /// <typeparam name="ActionType">The type of actions (delegates) associated with each entry in the menu.</typeparam>
    public abstract class MenuBase<ActionType> where ActionType : class
    {
        private List<MenuOption> options;
        private MenuOption cancel;

        private MenuLabeling labels;

        /// <summary>
        /// Gets a boolean value indicating whether or not the menu has a "cancel" option.
        /// </summary>
        public bool CanCancel
        {
            get { return cancel != null; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuBase{ActionType}" /> class.
        /// </summary>
        /// <param name="labels">Defines the type of labeling used when displaying this menu.</param>
        public MenuBase(MenuLabeling labels)
        {
            this.labels = labels;

            this.options = new List<MenuOption>();
            this.cancel = null;
        }

        /// <summary>
        /// Adds a new option to the menu.
        /// </summary>
        /// <param name="text">The text displayed for the new option.</param>
        /// <param name="action">The action associated with the new option.</param>
        public void Add(string text, ActionType action)
        {
            this.options.Add(new MenuOption(false, text, action));
        }

        /// <summary>
        /// Sets the cancel option for the menu.
        /// </summary>
        /// <param name="text">The text displayed for the cancel option.</param>
        /// <param name="action">The action associated with the cancel option.</param>
        public void SetCancel(string text, ActionType action)
        {
            this.cancel = new MenuOption(true, text, action);
        }

        /// <summary>
        /// Gets the number of options available in the menu (excluding the cancel option).
        /// </summary>
        public int Count
        {
            get { return options.Count; }
        }

        /// <summary>
        /// Displays the menu and returns the selected <see cref="MenuOption" />.
        /// </summary>
        /// <param name="cleanup">Determines what kind of console cleanup should be applied after displaying the menu.</param>
        /// <param name="indentation">A string that is used to indent each line in the menu.</param>
        /// <returns>
        /// The selected <see cref="MenuOption" />.
        /// </returns>
        protected MenuOption ShowAndSelect(MenuCleanup cleanup, string indentation)
        {
            Console.CursorVisible = false;

            int indentW = indentation.Length;

            int zeroPosition = Console.CursorTop;
            int cursorPosition = Console.CursorTop;
            for (int i = 0; i < options.Count; i++)
            {
                char prefix = prefixFromIndex(i);
                if (prefix == ' ')
                    ColorConsole.WriteLine(indentation + "     {1}", prefix, options[i].Text);
                else
                    ColorConsole.WriteLine(indentation + "  {0}: {1}", prefix, options[i].Text);
            }

            if (CanCancel)
                ColorConsole.WriteLine(indentation + "  0: " + cancel.Text);


            int finalPosition = Console.CursorTop;
            Console.SetCursorPosition(indentW, cursorPosition);
            Console.Write('>');

            int selected = -1;
            while (selected == -1)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                int keyIndex = indexFromKey(key.KeyChar);
                if (keyIndex < options.Count)
                {
                    selected = keyIndex;
                    if (selected == -1 && CanCancel)
                        selected = options.Count;
                }
                else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.UpArrow)
                {
                    int nextPos = key.Key == ConsoleKey.DownArrow ? cursorPosition + 1 : cursorPosition - 1;
                    int lastPos = CanCancel ? options.Count + zeroPosition : options.Count + zeroPosition - 1;

                    if (nextPos - zeroPosition < 0)
                        nextPos = lastPos;
                    else if (nextPos > lastPos)
                        nextPos = zeroPosition;
                    Console.SetCursorPosition(indentW, cursorPosition);
                    Console.Write(' ');
                    Console.SetCursorPosition(indentW, nextPos);
                    Console.Write('>');
                    cursorPosition = nextPos;
                }
                else if (key.Key == ConsoleKey.Enter)
                    selected = cursorPosition - zeroPosition;
                else if (key.Key == ConsoleKey.Escape && CanCancel)
                    selected = options.Count;
            }

            MenuOption result = selected == options.Count ? cancel : options[selected];

            if (cleanup == MenuCleanup.RemoveMenu || cleanup == MenuCleanup.RemoveMenuShowChoice)
            {
                Console.SetCursorPosition(0, zeroPosition);
                for (int i = 0; i < options.Count; i++)
                    ColorConsole.WriteLine(new string(' ', options[i].Text.Length + 5 + indentW));

                if (CanCancel)
                    ColorConsole.WriteLine(new string(' ', cancel.Text.Length + 5 + indentW));

                finalPosition = zeroPosition;
            }

            Console.SetCursorPosition(0, finalPosition);
            Console.CursorVisible = true;

            if (cleanup == MenuCleanup.RemoveMenuShowChoice)
                ColorConsole.WriteLine("Selected {0}: {1}", prefixFromIndex(selected), result.Text);

            return result;
        }

        private char prefixFromIndex(int index)
        {
            if (index < 0)
                return ' ';

            if (index == Count)
                return '0';

            switch (labels)
            {
                case MenuLabeling.None:
                    return ' ';

                case MenuLabeling.Numbers:
                    return index < 9 ? (char)('1' + index) : ' ';

                case MenuLabeling.Letters:
                    return (index + 'a') <= 'z' ? (char)(index + 'a') : ' ';

                case MenuLabeling.LettersUpper:
                    return (index + 'A') <= 'Z' ? (char)(index + 'A') : ' ';

                case MenuLabeling.NumbersAndLetters:
                    return index < 9 ? (char)('1' + index) :
                        (index - 9 + 'a') <= 'z' ? (char)(index - 9 + 'a') : ' ';

                case MenuLabeling.NumbersAndLettersUpper:
                    return index < 9 ? (char)('1' + index) :
                        (index - 9 + 'A') <= 'Z' ? (char)(index - 9 + 'A') : ' ';
                default:
                    return ' ';
            }
        }
        private int indexFromKey(char keyChar)
        {
            if (keyChar == '0')
                return -1;

            if (char.IsUpper(keyChar))
                keyChar = char.ToLower(keyChar);

            switch (labels)
            {
                case MenuLabeling.None:
                    return int.MaxValue;

                case MenuLabeling.Numbers:
                    return char.IsNumber(keyChar) ? int.Parse(keyChar.ToString()) - 1 : int.MaxValue;

                case MenuLabeling.Letters:
                    return char.IsLetter(keyChar) ? keyChar - 'a' : int.MaxValue;

                case MenuLabeling.LettersUpper:
                    return char.IsLetter(keyChar) ? keyChar - 'a' : int.MaxValue;

                case MenuLabeling.NumbersAndLetters:
                case MenuLabeling.NumbersAndLettersUpper:
                    return char.IsNumber(keyChar) ? int.Parse(keyChar.ToString()) - 1 :
                        char.IsLetter(keyChar) ? keyChar - 'a' + 9 : int.MaxValue;
                default:
                    return int.MaxValue;
            }
        }

        /// <summary>
        /// Describes a single option in a menu.
        /// </summary>
        protected class MenuOption
        {
            /// <summary>
            /// Indicates if the option is a cancel option.
            /// </summary>
            public readonly bool IsCancel;
            /// <summary>
            /// The text displayed in the menu for this option.
            /// </summary>
            public readonly string Text;
            /// <summary>
            /// The action that should be executed when the option is selected.
            /// </summary>
            public readonly ActionType Action;

            internal MenuOption(bool isCancel, string text, ActionType action)
            {
                this.IsCancel = isCancel;
                this.Text = text;
                this.Action = action;
            }
        }
    }
}
