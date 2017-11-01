using CommandLineParsing.Consoles;
using System;

namespace CommandLineParsing.UITests
{
    public class SharedConsole : IConsole
    {
        private readonly Tests.TestingConsole _testConsole;
        private readonly IConsole _console;

        public SharedConsole()
        {
            _testConsole = new Tests.TestingConsole();
            _console = Consoles.SystemConsole.Instance;

            _testConsole.SetBufferSize(_console.BufferWidth, _console.BufferHeight);
            _testConsole.SetCursorPosition(_console.CursorLeft, _console.CursorTop);
            _testConsole.SetWindowSize(_console.WindowWidth, _console.WindowHeight);
            _testConsole.SetWindowPosition(_console.WindowLeft, _console.WindowTop);
        }

        public Tests.TestingConsoleString[] BufferStrings => _testConsole.BufferStrings;
        public Tests.TestingConsoleString[] WindowStrings => _testConsole.WindowStrings;

        public int BufferWidth
        {
            get { return _console.BufferWidth; }
            set
            {
                _console.BufferWidth = value;
                _testConsole.BufferWidth = value;
            }
        }
        public int BufferHeight
        {
            get { return _console.BufferHeight; }
            set
            {
                _console.BufferHeight = value;
                _testConsole.BufferHeight = value;
            }
        }
        public void SetBufferSize(int width, int height)
        {
            _console.SetBufferSize(width, height);
            _testConsole.SetBufferSize(width, height);
        }

        public int CursorLeft
        {
            get { return _console.CursorLeft; }
            set
            {
                _console.CursorLeft = value;
                _testConsole.CursorLeft = value;
            }
        }
        public int CursorTop
        {
            get { return _console.CursorTop; }
            set
            {
                _console.CursorTop = value;
                _testConsole.CursorTop = value;
            }
        }
        public void SetCursorPosition(int left, int top)
        {
            _console.SetCursorPosition(left, top);
            _testConsole.SetCursorPosition(left, top);
        }

        public int WindowWidth
        {
            get { return _console.WindowWidth; }
            set
            {
                _console.WindowWidth = value;
                _testConsole.WindowWidth = value;
            }
        }
        public int WindowHeight
        {
            get { return _console.WindowHeight; }
            set
            {
                _console.WindowHeight = value;
                _testConsole.WindowHeight = value;
            }
        }
        public void SetWindowSize(int width, int height)
        {
            _console.SetWindowSize(width, height);
            _testConsole.SetWindowSize(width, height);
        }

        public int WindowLeft
        {
            get { return _console.WindowLeft; }
            set
            {
                _console.WindowLeft = value;
                _testConsole.WindowLeft = value;
            }
        }
        public int WindowTop
        {
            get { return _console.WindowTop; }
            set
            {
                _console.WindowTop = value;
                _testConsole.WindowTop = value;
            }
        }
        public void SetWindowPosition(int left, int top)
        {
            _console.SetWindowPosition(left, top);
            _testConsole.SetWindowPosition(left, top);
        }

        public bool CursorVisible
        {
            get { return _console.CursorVisible; }
            set
            {
                _console.CursorVisible = value;
                _testConsole.CursorVisible = value;
            }
        }
        public ConsoleColor ForegroundColor
        {
            get { return _console.ForegroundColor; }
            set
            {
                _console.ForegroundColor = value;
                _testConsole.ForegroundColor = value;
            }
        }
        public ConsoleColor BackgroundColor
        {
            get { return _console.BackgroundColor; }
            set
            {
                _console.BackgroundColor = value;
                _testConsole.BackgroundColor = value;
            }
        }
        public void ResetColor()
        {
            _console.ResetColor();
            _testConsole.ResetColor();
        }

        public void Write(string value)
        {
            _console.Write(value);
            _testConsole.Write(value);
        }
        public void WriteLine(string value)
        {
            _console.WriteLine(value);
            _testConsole.WriteLine(value);
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            var result = _console.ReadKey(intercept);

            _testConsole.Input.Enqueue(result);
            _testConsole.ReadKey(intercept);

            return result;
        }
    }
}
