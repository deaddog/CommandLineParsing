using CommandLineParsing.Consoles;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CommandLineParsing.Tests
{
    public class TestingConsole : IConsole
    {
        private ConsoleSize _bufferSize;
        private ConsolePoint _cursorPosition;

        private char[,] _content;
        private ConsoleColor[,] _foreground;
        private ConsoleColor[,] _background;

        private readonly InputCollection _input;

        public TestingConsole()
        {
            _bufferSize = new ConsoleSize(80, 300);
            _cursorPosition = new ConsolePoint(0, 0);

            CursorVisible = true;
            ResetColor();

            _content = new char[_bufferSize.Width, _bufferSize.Height];
            _foreground = new ConsoleColor[_bufferSize.Width, _bufferSize.Height];
            _background = new ConsoleColor[_bufferSize.Width, _bufferSize.Height];

            for (int x = 0; x < _bufferSize.Width; x++)
                for (int y = 0; y < _bufferSize.Height; y++)
                {
                    _content[x, y] = ' ';
                    _foreground[x, y] = ForegroundColor;
                    _background[x, y] = BackgroundColor;
                }

            _input = new InputCollection();
        }

        public InputCollection Input => _input;

        public TestingConsoleString[] GetBufferStrings()
        {
            var strings = new List<TestingConsoleString>();

            var rows = _content.GetLength(1);
            for (int r = 0; r < rows; r++)
            {
                int start = GetFirstActive(r);
                if (start == -1)
                    continue;
                int end = GetLastActive(r);

                var segments = new List<TestingConsoleSegment>();

                var text = _content[start, r].ToString();

                for (int i = start + 1; i <= end; i++)
                    if (_foreground[i, r] != _foreground[i - 1, r] || _background[i, r] != _background[i - 1, r])
                    {
                        segments.Add(new TestingConsoleSegment(text, _foreground[i - 1, r], _background[i - 1, r]));
                        text = _content[i, r].ToString();
                    }
                    else
                        text += _content[i, r];

                segments.Add(new TestingConsoleSegment(text, _foreground[end, r], _background[end, r]));
                strings.Add(new TestingConsoleString(new ConsolePoint(start, r), segments));
            }

            return strings.ToArray();
        }
        private int GetFirstActive(int rowIndex)
        {
            for (int i = 0; i < _content.GetLength(0); i++)
                if (_content[i, rowIndex] != ' ')
                    return i;
            return -1;
        }
        private int GetLastActive(int rowIndex)
        {
            for (int i = _content.GetLength(0) - 1; i >= 0; i--)
                if (_content[i, rowIndex] != ' ')
                    return i;
            return -1;
        }

        public int BufferWidth
        {
            get { return _bufferSize.Width; }
            set { SetBufferSize(value, BufferHeight); }
        }
        public int BufferHeight
        {
            get { return _bufferSize.Height; }
            set { SetBufferSize(BufferWidth, value); }
        }
        public void SetBufferSize(int width, int height)
        {
            var newsize = new ConsoleSize(width, height);

            if (newsize == _bufferSize)
                return;

            var newContent = new char[newsize.Width, newsize.Height];
            var newForeground = new ConsoleColor[newsize.Width, newsize.Height];
            var newBackground = new ConsoleColor[newsize.Width, newsize.Height];

            for (int x = 0; x < newsize.Width; x++)
                for (int y = 0; y < newsize.Height; y++)
                    if (x >= _bufferSize.Width || y >= _bufferSize.Height)
                    {
                        newContent[x, y] = ' ';
                        newForeground[x, y] = ForegroundColor;
                        newBackground[x, y] = BackgroundColor;
                    }
                    else
                    {
                        newContent[x, y] = _content[x, y];
                        newForeground[x, y] = _foreground[x, y];
                        newBackground[x, y] = _background[x, y];
                    }

            _content = newContent;
            _foreground = newForeground;
            _background = newBackground;

            _bufferSize = newsize;
        }

        public int CursorLeft
        {
            get { return _cursorPosition.Left; }
            set { SetCursorPosition(value, CursorTop); }
        }
        public int CursorTop
        {
            get { return _cursorPosition.Top; }
            set { SetCursorPosition(CursorLeft, value); }
        }
        public void SetCursorPosition(int left, int top)
        {
            _cursorPosition = new ConsolePoint(left, top);
        }

        public int WindowWidth
        {
            get { throw new NotSupportedException(nameof(WindowWidth) + " is not supported by " + nameof(TestingConsole)); }
            set { throw new NotSupportedException(nameof(WindowWidth) + " is not supported by " + nameof(TestingConsole)); }
        }
        public int WindowHeight
        {
            get { throw new NotSupportedException(nameof(WindowHeight) + " is not supported by " + nameof(TestingConsole)); }
            set { throw new NotSupportedException(nameof(WindowHeight) + " is not supported by " + nameof(TestingConsole)); }
        }
        public void SetWindowSize(int width, int height)
        {
            throw new NotSupportedException(nameof(SetWindowSize) + " is not supported by " + nameof(TestingConsole));
        }

        public int WindowLeft
        {
            get { throw new NotSupportedException(nameof(WindowLeft) + " is not supported by " + nameof(TestingConsole)); }
            set { throw new NotSupportedException(nameof(WindowLeft) + " is not supported by " + nameof(TestingConsole)); }
        }
        public int WindowTop
        {
            get { throw new NotSupportedException(nameof(WindowTop) + " is not supported by " + nameof(TestingConsole)); }
            set { throw new NotSupportedException(nameof(WindowTop) + " is not supported by " + nameof(TestingConsole)); }
        }
        public void SetWindowPosition(int left, int top)
        {
            throw new NotSupportedException(nameof(SetWindowPosition) + " is not supported by " + nameof(TestingConsole));
        }

        public bool CursorVisible { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public void ResetColor()
        {
            ForegroundColor = ConsoleColor.Gray;
            BackgroundColor = ConsoleColor.Black;
        }

        public void Write(string value)
        {
            if (value == null)
                return;

            foreach (var c in value)
                WriteChar(c);
        }
        public void WriteLine(string value) => Write(value + "\n");
        private void WriteChar(char c)
        {
            if (CommandLineParsing.Input.ConsoleReader.IsInputCharacter(c))
            {
                _content[_cursorPosition.Left, _cursorPosition.Top] = c;
                _foreground[_cursorPosition.Left, _cursorPosition.Top] = ForegroundColor;
                _background[_cursorPosition.Left, _cursorPosition.Top] = BackgroundColor;

                _cursorPosition.Left++;
                if (_cursorPosition.Left >= BufferWidth)
                    _cursorPosition = new ConsolePoint(0, _cursorPosition.Top + 1);
            }
            else
                switch (c)
                {
                    case '\n':
                        _cursorPosition = new ConsolePoint(0, _cursorPosition.Top + 1);
                        break;

                    case '\r':
                        //ignore
                        break;

                    default:
                        throw new NotSupportedException($"The character value \"{(int)c}\" is not supported by {nameof(TestingConsole)}");
                }
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            var next = _input.ReadNext();

            if (!intercept)
                WriteChar(next.KeyChar);

            return next;
        }

        public class InputCollection
        {
            private readonly Queue<ConsoleKeyInfo> _keys;

            public InputCollection()
            {
                _keys = new Queue<ConsoleKeyInfo>();
            }

            public ConsoleKeyInfo ReadNext()
            {
                if (_keys.Count == 0)
                    throw new InvalidOperationException($"All input characters have already been read from the {nameof(TestingConsole)}.");
                else
                    return _keys.Dequeue();
            }

            public void Enqueue(ConsoleKeyInfo key)
            {
                _keys.Enqueue(key);
            }
            public void Enqueue(ConsoleKey key, ConsoleModifiers modifiers = 0)
            {
                Enqueue(new ConsoleKeyInfo('\0', key, modifiers.HasFlag(ConsoleModifiers.Shift), modifiers.HasFlag(ConsoleModifiers.Alt), modifiers.HasFlag(ConsoleModifiers.Control)));
            }
            public void Enqueue(char key)
            {
                if (key >= 'a' && key <= 'z')
                    Enqueue(new ConsoleKeyInfo(key, ConsoleKey.A + key - 'a', false, false, false));
                else if (key >= 'A' && key <= 'Z')
                    Enqueue(new ConsoleKeyInfo(key, ConsoleKey.A + key - 'A', false, false, false));
                else
                    switch (key)
                    {
                        case ' ':
                            Enqueue(new ConsoleKeyInfo(key, ConsoleKey.Spacebar, false, false, false));
                            break;
                        case '\n':
                            Enqueue(new ConsoleKeyInfo(key, ConsoleKey.Enter, false, false, false));
                            break;

                        default:
                            Debugger.Break();
                            throw new NotSupportedException($"The character {key} is not supported by the {nameof(TestingConsole)}.");
                    }
            }
            public void Enqueue(string text)
            {
                foreach (var c in text)
                    Enqueue(c);
            }
        }
    }
}
