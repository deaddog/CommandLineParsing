using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a menu displayed in <see cref="Console"/> allowing the user to select multiple elements.
    /// For single-element selection see <see cref="Menu{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements returned by the menu.</typeparam>
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
        /// <param name="offText">The text displayed when the new option is not selected.
        /// If <c>null</c> coloring used to identify when the option is selected.</param>
        /// <param name="value">The value associated with the new option.</param>
        /// <param name="selected">if set to <c>true</c> the option will initially be selected when the menu is displayed.</param>
        public void Add(string text, string offText = null, T value = default(T), bool selected = false)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (offText == null)
                if (ColorConsole.HasColors(text))
                    offText = ColorConsole.ClearColors(text);
                else
                {
                    offText = text;
                    text = "[DarkGreen:" + text + "]";
                }

            this.options.Add(new MenuOption(text, offText, value, selected));
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
                    ColorConsole.WriteLine($"{settings.Indentation}     {options[i].Text}");
                else
                    ColorConsole.WriteLine($"{settings.Indentation}  {prefix}: {options[i].Text}");
            }

            int selectionCount = options.Count(x => x.selected);

            if (countInRange(selectionCount, settings))
                ColorConsole.WriteLine(settings.Indentation + "  0: " + doneText);
            else
                ColorConsole.WriteLine("");

            int finalPosition = Console.CursorTop;
            Console.SetCursorPosition(indentW, cursorPosition);
            Console.Write('>');

            while (true)
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

                        if (nextPos == lastPos && !countInRange(selectionCount, settings))
                            nextPos = key.Key == ConsoleKey.DownArrow ? 0 : (nextPos - 1);

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
                {
                    if (countInRange(selectionCount, settings))
                        break;
                    else
                        continue;
                }

                options[selected].selected = !options[selected].selected;
                Console.SetCursorPosition(indentW + 5, zeroPosition + selected);
                ColorConsole.Write(options[selected].Text);

                var newcount = selectionCount + (options[selected].selected ? 1 : -1);
                var isVisible = countInRange(selectionCount, settings);
                var beVisible = countInRange(newcount, settings);
                selectionCount = newcount;

                if (isVisible && !beVisible)
                {
                    Console.SetCursorPosition(0, finalPosition - 1);
                    Console.WriteLine(new string(' ', settings.Indentation.Length + 5 + ColorConsole.ClearColors(doneText).Length));
                }
                else if (!isVisible && beVisible)
                {
                    Console.SetCursorPosition(0, finalPosition - 1);
                    ColorConsole.WriteLine(settings.Indentation + "  0: " + doneText);
                }
            }

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
                for (int i = 0; i < options.Count; i++)
                    if (options[i].selected)
                        ColorConsole.WriteLine($"Selected {prefixFromIndex(i, settings.Labeling)}: {options[i].Text}");

            return options.Where(o => o.selected).ToArray();
        }

        private static bool countInRange(int count, MenuSettings settings)
        {
            return count >= settings.MinimumSelected && count <= settings.MaximumSelected;
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

                string on = ColorConsole.ClearColors(onText);
                string off = ColorConsole.ClearColors(offText);

                if (on.Length != off.Length)
                {
                    int max = Math.Max(on.Length, off.Length);

                    this.OnText = this.OnText.PadRight(max);
                    this.OffText = this.OffText.PadRight(max);
                }

                this.Value = value;
                this.selected = selected;
            }
        }
    }
}
