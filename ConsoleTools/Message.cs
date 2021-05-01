using System;

namespace ConsoleTools
{
    public class Message
    {
        public static Message NoError { get; } = new Message();

        private readonly ConsoleString _content;

        private Message()
        {
            IsError = false;
            _content = default!;
        }

        public Message(ConsoleString content)
        {
            IsError = true;
            _content = content ?? throw new ArgumentNullException(nameof(content));

            if (_content.Length == 0)
                throw new ArgumentException($"{nameof(Message)} cannot use an empty {nameof(ConsoleString)}.");
        }

        public ConsoleString Content => IsError ? _content : throw new InvalidOperationException($"Only error messages have {nameof(Content)}. Check {nameof(IsError)} before accessing.");
        public bool IsError { get; }
    }

    public class Message<T>
    {
        private readonly ConsoleString _content;
        private readonly T _value;

        public Message(T value)
        {
            IsError = false;
            _content = default!;
            _value = value;
        }
        public Message(ConsoleString content)
        {
            IsError = true;
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _value = default!;

            if (content.Length == 0)
                throw new ArgumentException($"{nameof(Message)} cannot use an empty {nameof(ConsoleString)}.");
        }

        public ConsoleString Content => IsError ? _content : throw new InvalidOperationException($"Only error messages have {nameof(Content)}. Check {nameof(IsError)} before accessing.");
        public T Value => IsError ? throw new InvalidOperationException($"Only succes messages have {nameof(Value)}. Check {nameof(IsError)} before accessing.") : _value;

        public bool IsError { get; }
    }
}
