using CommandLineParsing.Consoles;

namespace CommandLineParsing.UITests
{
    class Program
    {
        static readonly SharedConsole _console = new SharedConsole();

        static void Main(string[] args)
        {
            // Setting the active console to the shared one, use _console.BufferStrings and _console.WindowStrings to inspect the state.
            IConsole c = SystemConsole.Instance;

            // Do stuff using the console, by interacting with ColorConsole.
        }
    }
}
