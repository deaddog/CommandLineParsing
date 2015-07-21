using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    public class SelectionMenu<T>
    {
        private List<MenuOption> options;
        private string doneText;

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu{T}" /> class.
        /// </summary>
        public SelectionMenu()
        {
            this.options = new List<MenuOption>();
            this.doneText = "Done";
        }

        /// <summary>
        /// Adds a new option to the menu.
        /// </summary>
        /// <param name="text">The text displayed for the new option.</param>
        /// <param name="value">The value associated with the new option.</param>
        public void Add(string text, T value)
        {
            if (ColorConsole.HasColors(text))
                Add(text, ColorConsole.ClearColors(text), value);
            else
                Add(text + " *", text, value);
        }

        /// <summary>
        /// Adds a new option to the menu.
        /// </summary>
        /// <param name="onText">The text displayed when the new option is selected.</param>
        /// <param name="offText">The text displayed when the new option is not selected.</param>
        /// <param name="value">The value associated with the new option.</param>
        public void Add(string onText, string offText, T value)
        {
            this.options.Add(new MenuOption(onText, offText, value));
        }

        /// <summary>
        /// Adds a new option to the menu.
        /// Selecting this option will return the default value for <typeparamref name="T"/>.
        /// </summary>
        /// <param name="text">The text displayed for the new option.</param>
        public void Add(string text)
        {
            this.Add(text, default(T));
        }

        /// <summary>
        /// Adds a new option to the menu.
        /// Selecting this option will return the default value for <typeparamref name="T"/>.
        /// </summary>
        /// <param name="onText">The text displayed when the new option is selected.</param>
        /// <param name="offText">The text displayed when the new option is not selected.</param>
        public void Add(string onText, string offText)
        {
            this.Add(onText, offText, default(T));
        }

        /// <summary>
        /// Gets or sets the text displayed for the menu option that terminates selection.
        /// Initializes to 'Done'.
        /// </summary>
        public string DoneText
        {
            get { return doneText; }
            set { doneText = value; }
        }

        /// <summary>
        /// Gets the number of options available in the menu (excluding the cancel option).
        /// </summary>
        public int Count
        {
            get { return options.Count; }
        }

        /// <summary>
        /// Displays the menu and returns an array with the selected <see cref="MenuOption"/>s.
        /// </summary>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <returns>
        /// The selected <see cref="MenuOption"/>s array.
        /// </returns>
        public MenuOption[] ShowAndSelect(MenuSettings settings)
        {
            if (settings == null) settings = new MenuSettings();

            Console.CursorVisible = false;

            int indentW = (settings.Indentation ?? string.Empty).Length;

            int zeroPosition = Console.CursorTop;
            int cursorPosition = Console.CursorTop;
            for (int i = 0; i < options.Count; i++)
            {
                char prefix = prefixFromIndex(i, settings.Labeling);
                if (prefix == ' ')
                    ColorConsole.WriteLine(settings.Indentation + "     {1}", prefix, options[i].Text);
                else
                    ColorConsole.WriteLine(settings.Indentation + "  {0}: {1}", prefix, options[i].Text);
            }

            ColorConsole.WriteLine(settings.Indentation + "  0: " + doneText);


            int finalPosition = Console.CursorTop;
            Console.SetCursorPosition(indentW, cursorPosition);
            Console.Write('>');

            bool done = false;
            List<int> selectedIndices = new List<int>();

            while (!done)
            {
                int selected = -1;
                while (selected == -1)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    int keyIndex = indexFromKey(key.KeyChar, settings.Labeling);
                    if (keyIndex < options.Count)
                    {
                        selected = keyIndex;
                        if (selected == -1)
                            selected = options.Count;
                    }
                    else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.UpArrow)
                    {
                        int nextPos = key.Key == ConsoleKey.DownArrow ? cursorPosition + 1 : cursorPosition - 1;
                        int lastPos = options.Count + zeroPosition;

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
                    else if (key.Key == ConsoleKey.Escape)
                        selected = options.Count;
                }

                if (selected == options.Count)
                    done = true;
                else
                {
                    selectedIndices.Add(selected);
                }
            }

            selectedIndices.Sort();

            MenuOption[] result = new MenuOption[selectedIndices.Count];
            for (int i = 0; i < selectedIndices.Count; i++) result[i] = options[selectedIndices[i]];

            if (settings.Cleanup == MenuCleanup.RemoveMenu || settings.Cleanup == MenuCleanup.RemoveMenuShowChoice)
            {
                Console.SetCursorPosition(0, zeroPosition);
                for (int i = 0; i < options.Count; i++)
                    ColorConsole.WriteLine(new string(' ', options[i].Text.Length + 5 + indentW));

                ColorConsole.WriteLine(new string(' ', doneText.Length + 5 + indentW));

                finalPosition = zeroPosition;
            }

            Console.SetCursorPosition(0, finalPosition);
            Console.CursorVisible = true;

            if (settings.Cleanup == MenuCleanup.RemoveMenuShowChoice)
                for (int i = 0; i < selectedIndices.Count; i++)
                    ColorConsole.WriteLine("Selected {0}: {1}", prefixFromIndex(selectedIndices[i], settings.Labeling), result[i].Text);

            return result;
        }

        private char prefixFromIndex(int index, MenuLabeling labeling)
        {
            if (index < 0)
                return ' ';

            if (index == Count)
                return '0';

            switch (labeling)
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
        private int indexFromKey(char keyChar, MenuLabeling labeling)
        {
            if (keyChar == '0')
                return -1;

            if (char.IsUpper(keyChar))
                keyChar = char.ToLower(keyChar);

            switch (labeling)
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
        public class MenuOption
        {
            internal string Text
            {
                get { return selected ? OnText : OffText; }
            }

            /// <summary>
            /// The text displayed in the menu for this option when it is selected.
            /// </summary>
            public readonly string OnText;
            /// <summary>
            /// The text displayed in the menu for this option when it is not selected.
            /// </summary>
            public readonly string OffText;
            /// <summary>
            /// The value that is associated with this <see cref="MenuOption"/>.
            /// </summary>
            public readonly T Value;

            internal bool selected;

            internal MenuOption(string onText, string offText, T value, bool selected)
            {
                this.OnText = onText;
                this.OffText = offText;
                this.Value = value;
                this.selected = selected;
            }
        }
    }
}
