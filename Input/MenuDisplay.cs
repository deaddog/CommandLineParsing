namespace CommandLineParsing.Input
{
    /// <summary>
    /// Provides methods for managing a menu in the console.
    /// </summary>
    public class MenuDisplay
    {
        private readonly ConsolePoint origin;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuDisplay"/> class.
        /// The menu will be displayed at the current cursor position.
        /// </summary>
        public MenuDisplay()
            : this(ColorConsole.CursorPosition)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuDisplay"/> class.
        /// </summary>
        /// <param name="point">The point where the menu should be displayed.</param>
        public MenuDisplay(ConsolePoint point)
        {
            origin = point;
            options = new List<MenuOption<T>>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuDisplay{T}"/> class.
        /// </summary>
        /// <param name="offset">The offset from the current cursor position where to menu should be displayed.</param>
        public MenuDisplay(ConsoleSize offset)
            : this(ColorConsole.CursorPosition + offset)
        {
        }
    }
}
