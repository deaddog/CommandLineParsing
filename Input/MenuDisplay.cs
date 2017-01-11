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

                if (lengthDiff != 0)
                    for (int i = 0; i < options.Count; i++)
                    {
                        var newText = options[i].Text;
                        if (lengthDiff < 0)
                            newText += new string(' ', -lengthDiff);

                        if (i == SelectedIndex)
                            (origin + new ConsoleSize(0, i)).TemporaryShift(() => ColorConsole.Write(prompt + newText));
                        else
                            (origin + new ConsoleSize(0, i)).TemporaryShift(() => ColorConsole.Write(noPrompt + newText));
                    }
            }
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
            }
        }

        internal void UpdateOption(MenuOption<T> option, string oldText)
        {
            UpdateOption(options.IndexOf(option), oldText, option.Text);
        }
        internal void UpdateOption(int index, string oldText, string newText)
        {
            var offset = new ConsoleSize(prompt.Length, index);

            ColorConsole.TemporaryShift(origin + offset, () =>
            {
                int oldLen = ColorConsole.ClearColors(oldText).Length;
                Console.Write(new string(' ', oldLen) + new string('\b', oldLen));
                ColorConsole.Write(newText);
            });
        }

        /// <summary>
        /// Performs cleanup of the menu display as specified by <see cref="Cleanup"/>.
        /// </summary>
        public void Dispose()
        {
            if (Cleanup == InputCleanup.Clean)
            {
                ColorConsole.TemporaryShift(origin, () =>
                {
                    ColorConsole.CursorPosition = origin;
                    for (int i = 0; i < options.Count; i++)
                        ColorConsole.WriteLine(new string(' ', options[i].Text.Length + prompt.Length));
                });
            }
        }
    }
}
