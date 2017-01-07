namespace CommandLineParsing.Input
{
    /// <summary>
    /// Provides methods for managing a menu in the console.
    /// </summary>
    /// <typeparam name="T">The type of the values selectable from the <see cref="MenuDisplay{T}"/>.</typeparam>
    public class MenuDisplay<T>
    {
        private readonly ConsolePoint origin;
        private readonly MenuOptionCollection<T> options;

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
    }
}
