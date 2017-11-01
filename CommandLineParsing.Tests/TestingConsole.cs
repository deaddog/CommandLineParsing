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

        private List<char[]> _content;
        private List<ConsoleColor[]> _foreground;
        private List<ConsoleColor[]> _background;

        private readonly InputCollection _input;

        public TestingConsole()
        {
            _bufferSize = new ConsoleSize(80, 300);
            _cursorPosition = new ConsolePoint(0, 0);

            CursorVisible = true;
            ResetColor();

            _content = new List<char[]>();
            _foreground = new List<ConsoleColor[]>();
            _background = new List<ConsoleColor[]>();

            _input = new InputCollection();
        }

        public InputCollection Input => _input;

        private TestingConsoleString[] _bufferStrings;
        public TestingConsoleString[] BufferStrings
        {
            get
            {
                if (_bufferStrings == null)
                {
                    var strings = new List<TestingConsoleString>();

                    for (int r = 0; r < _content.Count; r++)
                    {
                        var row = _content[r];
                        var fg = _foreground[r];
                        var bg = _background[r];

                        int start = GetFirstActive(row);
                        if (start == -1)
                            continue;
                        int end = GetLastActive(row);

                        var segments = new List<TestingConsoleSegment>();

                        var text = row[start].ToString();

                        for (int i = start + 1; i <= end; i++)
                            if (fg[i] != fg[i - 1] || bg[i] != bg[i - 1])
                            {
                                segments.Add(new TestingConsoleSegment(text, fg[i - 1], bg[i - 1]));
                                text = row[i].ToString();
                            }
                            else
                                text += row[i];

                        segments.Add(new TestingConsoleSegment(text, fg[end], bg[end]));
                        strings.Add(new TestingConsoleString(new ConsolePoint(start, r), segments));
                    }
                    _bufferStrings = strings.ToArray();
                }

                return _bufferStrings;
            }
        }
        private int GetFirstActive(char[] row)
        {
            for (int i = 0; i < row.Length; i++)
                if (row[i] != ' ')
                    return i;
            return -1;
        }
        private int GetLastActive(char[] row)
        {
            for (int i = row.Length - 1; i >= 0; i--)
                if (row[i] != ' ')
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
            if (width == _bufferSize.Width && height == _bufferSize.Height)
                return;

            if (height < _bufferSize.Height)
            {
                _content.RemoveRange(height, _content.Count - height);
                _foreground.RemoveRange(height, _foreground.Count - height);
                _background.RemoveRange(height, _background.Count - height);
            }

            for (int r = 0; r < _content.Count; r++)
            {
                var c = _content[r];
                var fg = _foreground[r];
                var bg = _background[r];

                Array.Resize(ref c, width);
                Array.Resize(ref fg, width);
                Array.Resize(ref bg, width);

                for (int i = _bufferSize.Width; i < width; i++)
                {
                    c[i] = ' ';
                    fg[i] = ConsoleColor.DarkGray;
                    bg[i] = ConsoleColor.Black;
                }

                _content[r] = c;
                _foreground[r] = fg;
                _background[r] = bg;
            }

            _bufferSize = new ConsoleSize(width, height);
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
            _bufferStrings = null;

            if (CommandLineParsing.Input.ConsoleReader.IsInputCharacter(c))
            {
                while (_cursorPosition.Top >= _content.Count)
                {
                    _content.Add(new char[_bufferSize.Width]);
                    _foreground.Add(new ConsoleColor[_bufferSize.Width]);
                    _background.Add(new ConsoleColor[_bufferSize.Width]);

                    var index = _content.Count - 1;
                    for (int i = 0; i < _content[_content.Count - 1].Length; i++)
                    {
                        _content[index][i] = ' ';
                        _foreground[index][i] = ConsoleColor.DarkGray;
                        _background[index][i] = ConsoleColor.Black;
                    }

                    while (_content.Count > _bufferSize.Height)
                    {
                        _content.RemoveAt(0);
                        _foreground.RemoveAt(0);
                        _background.RemoveAt(0);
                    }
                }
                _content[_cursorPosition.Top][_cursorPosition.Left] = c;
                _foreground[_cursorPosition.Top][_cursorPosition.Left] = ForegroundColor;
                _background[_cursorPosition.Top][_cursorPosition.Left] = BackgroundColor;

                _cursorPosition.Left++;
                if (_cursorPosition.Left >= BufferWidth)
                    SetCursorPosition(0, _cursorPosition.Top + 1);
            }
            else
                switch (c)
                {
                    case '\n':
                        SetCursorPosition(0, _cursorPosition.Top + 1);
                        break;

                    case '\r':
                        //ignore
                        break;

                    case '\b':
                        if (_cursorPosition.Left == 0)
                            SetCursorPosition(BufferWidth - 1, _cursorPosition.Top - 1);
                        else
                            SetCursorPosition(_cursorPosition.Left - 1, _cursorPosition.Top);
                        break;

                    default:
                        Debugger.Break();
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
