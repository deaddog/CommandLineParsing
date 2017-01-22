using CommandLineParsing.Output;
using System;

namespace CommandLineParsing.Input
{
    /// <summary>
    /// Provides methods for managing a menu in the console.
    /// </summary>
    /// <typeparam name="T">The type of the values selectable from the <see cref="MenuDisplay{T}"/>.</typeparam>
    public class MenuDisplay<T> : IConsoleInput
    {
        private readonly ConsolePoint origin;
        private readonly MenuOptionCollection<T> options;
        private int index;

        private ConsoleString prompt;
        private string noPrompt;

        private readonly PrefixKeyCollection _prefixTop;
        private readonly PrefixKeyCollection _prefixBottom;
        private bool _hasPrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuDisplay{T}"/> class.
        /// The menu will be displayed at the current cursor position.
        /// </summary>
        public MenuDisplay()
            : this(ColorConsole.CursorPosition)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuDisplay{T}"/> class.
        /// </summary>
        /// <param name="point">The point where the menu should be displayed.</param>
        public MenuDisplay(ConsolePoint point)
        {
            origin = point;
            options = new MenuOptionCollection<T>(this);
            index = -1;
            Prompt = new ConsoleString("> ");

            _prefixTop = new PrefixKeyCollection();
            _prefixBottom = new PrefixKeyCollection();
            _hasPrefix = false;

            _prefixTop.PrefixSetChanged += UpdatePrefixChange;
            _prefixBottom.PrefixSetChanged += UpdatePrefixChange;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuDisplay{T}"/> class.
        /// </summary>
        /// <param name="offset">The offset from the current cursor position where to menu should be displayed.</param>
        public MenuDisplay(ConsoleSize offset)
            : this(ColorConsole.CursorPosition + offset)
        {
        }

        /// <summary>
        /// Gets a collection of the <see cref="MenuOption{T}"/> elements displayed by this <see cref="MenuDisplay{T}"/>.
        /// </summary>
        public MenuOptionCollection<T> Options => options;
        /// <summary>
        /// Gets or sets the text that is prefixed on the currently selected option.
        /// </summary>
        public ConsoleString Prompt
        {
            get { return prompt; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (value == prompt)
                    return;

                var lengthDiff = value.Length - (prompt?.Length ?? 0);

                prompt = value;
                noPrompt = new string(' ', prompt.Length);

                UpdateAll(lengthDiff);
            }
        }


        /// <summary>
        /// Gets the collection of prefixes shown at the top of a menu.
        /// </summary>
        public PrefixKeyCollection PrefixesTop => _prefixTop;
        /// <summary>
        /// Gets the collection of prefixes shown at the bottom of a menu.
        /// Note that these are applied bottom-up, and take precedence over <see cref="PrefixesTop"/>.
        /// </summary>
        public PrefixKeyCollection PrefixesBottom => _prefixBottom;

        /// <summary>
        /// Gets the index in the <see cref="MenuDisplay{T}"/> based on a prefix character.
        /// </summary>
        /// <param name="prefixChar">The prefix character.</param>
        /// <returns>If <paramref name="prefixChar"/> is part of <see cref="PrefixesTop"/> or <see cref="PrefixesBottom"/>, the index of the menu option that corresponds to the prefix; otherwise <c>-1</c>.</returns>
        public int IndexFromPrefix(char prefixChar)
        {
            var bottomIndex = _prefixBottom.IndexFromPrefix(prefixChar);
            if (bottomIndex >= 0 && bottomIndex < options.Count)
                return options.Count - bottomIndex - 1;

            var topIndex = _prefixTop.IndexFromPrefix(prefixChar);
            if (topIndex >= 0 && topIndex < options.Count && options.Count - _prefixBottom.Count > topIndex)
                return topIndex;

            return -1;
        }

        /// <summary>
        /// Gets or sets the index of the selected option in the menu.
        /// A value of <c>-1</c> indicates that no element is selected.
        /// </summary>
        public int SelectedIndex
        {
            get { return index; }
            set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException(nameof(value), "Index cannot be less than -1.");

                if (value >= options.Count)
                    throw new ArgumentOutOfRangeException(nameof(value), $"No option available at index {value}. There are {options.Count} options.");

                if (value == index)
                    return;

                if (index != -1)
                    (origin + new ConsoleSize(0, index)).TemporaryShift(() => ColorConsole.Write(noPrompt));

                if (value != -1)
                    (origin + new ConsoleSize(0, value)).TemporaryShift(() => ColorConsole.Write(prompt));

                index = value;
            }
        }

        /// <summary>
        /// Gets the location where the readline is displayed. If 
        /// </summary>
        public ConsolePoint Origin => origin;

        /// <summary>
        /// Gets the type of cleanup that should be applied when disposing the <see cref="MenuDisplay{T}"/>.
        /// </summary>
        public InputCleanup Cleanup { get; set; }

        /// <summary>
        /// Handles the specified key by updating the state of the <see cref="MenuDisplay{T}"/>.
        /// </summary>
        /// <param name="key">The key to process.</param>
        public void HandleKey(ConsoleKeyInfo key)
        {
            if (Options.Count == 0)
                return;

            switch (key.Key)
            {
                case ConsoleKey.DownArrow:
                    if (SelectedIndex == Options.Count - 1)
                        SelectedIndex = 0;
                    else
                        SelectedIndex++;
                    break;

                case ConsoleKey.UpArrow:
                    if (SelectedIndex <= 0)
                        SelectedIndex = Options.Count - 1;
                    else
                        SelectedIndex--;
                    break;

                case ConsoleKey.Home:
                    SelectedIndex = 0;
                    break;

                case ConsoleKey.End:
                    SelectedIndex = Options.Count - 1;
                    break;

                case ConsoleKey.Backspace:
                case ConsoleKey.Delete:
                    if (SelectedIndex >= 0)
                        Options.RemoveAt(SelectedIndex);
                    break;

                default:
                    int prefixIndex = IndexFromPrefix(key.KeyChar);
                    if (prefixIndex >= 0)
                        SelectedIndex = prefixIndex;
                    break;
            }
        }

        private void UpdatePrefixChange()
        {
            var oldHas = _hasPrefix;
            _hasPrefix = _prefixTop.Count > 0 || _prefixTop.Count > 0;

            if (_hasPrefix == oldHas)
                UpdateAll(0);
            else if (!_hasPrefix)
                UpdateAll(-3);
            else
                UpdateAll(3);
        }
        private void UpdateAll(int lengthDiff)
        {
            if (lengthDiff != 0)
                for (int i = 0; i < options.Count; i++)
                {
                    var newText = options[i].Text;
                    if (lengthDiff < 0)
                        newText += new string(' ', -lengthDiff);

                    if (i == SelectedIndex)
                        (origin + new ConsoleSize(0, i)).TemporaryShift(() => ColorConsole.Write(prompt + GetPrefix(i) + newText));
                    else
                        (origin + new ConsoleSize(0, i)).TemporaryShift(() => ColorConsole.Write(noPrompt + GetPrefix(i) + newText));
                }
            else
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == SelectedIndex)
                        (origin + new ConsoleSize(0, i)).TemporaryShift(() => ColorConsole.Write(prompt + GetPrefix(i)));
                    else
                        (origin + new ConsoleSize(0, i)).TemporaryShift(() => ColorConsole.Write(noPrompt + GetPrefix(i)));
                }
        }
        internal void UpdateOption(MenuOption<T> option, string oldText)
        {
            UpdateOption(options.IndexOf(option), oldText, option.Text);
        }
        internal void UpdateOption(int index, string oldText, string newText)
        {
            var oldPrefix = GetPrefix(index);
            var newPrefix = string.IsNullOrEmpty(newText) ? "" : oldPrefix;

            var offset = new ConsoleSize(prompt.Length, index);

            ColorConsole.TemporaryShift(origin + offset, () =>
            {
                int oldLen = ColorConsole.ClearColors(oldPrefix + oldText).Length;
                Console.Write(new string(' ', oldLen) + new string('\b', oldLen));
                ColorConsole.Write(newPrefix + newText);
            });

            if (string.IsNullOrEmpty(oldText) || string.IsNullOrEmpty(newText))
                UpdateAll(0);
        }

        private string GetPrefix(int index)
        {
            if (_prefixTop.Count == 0 && _prefixBottom.Count == 0)
                return "";

            char? prefix =
                _prefixBottom.PrefixFromIndex(options.Count - index - 1) ??
                _prefixTop.PrefixFromIndex(index);

            if (prefix.HasValue)
                return prefix + ": ";
            else
                return "   ";
        }

        /// <summary>
        /// Performs cleanup of the menu display as specified by <see cref="Cleanup"/>.
        /// </summary>
        public void Dispose()
        {
            if (Cleanup == InputCleanup.Clean)
            {
                var prefixLength = GetPrefix(0).Length;

                ColorConsole.TemporaryShift(origin, () =>
                {
                    ColorConsole.CursorPosition = origin;
                    for (int i = 0; i < options.Count; i++)
                        ColorConsole.WriteLine(new string(' ', options[i].Text.Length + prompt.Length + prefixLength));
                });
            }
        }
    }
}
