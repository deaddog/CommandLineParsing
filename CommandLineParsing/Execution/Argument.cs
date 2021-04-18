using System;
using System.Collections.Immutable;

namespace CommandLineParsing.Execution
{
    public static class Argument
    {
        public static Argument<T> Create<T>(IParameter parameter)
        {
            return new Argument<T>
            (
                parameter: parameter
            );
        }
        public static Argument<T> Create<T>(IParameter parameter, string name, ImmutableArray<string> args, Message<T> message)
        {
            return new Argument<T>
            (
                parameter: parameter,
                name: name,
                args: args,
                message: message
            );
        }
    }

    public class Argument<T> where T: notnull
    {
        public Argument(IParameter parameter, string name, ImmutableArray<string> args, Message<T> message)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Args = args;
            Value = message.IsError ? default : message.Value;
            Message = message.IsError ? new Message(message.Content) : Message.NoError;
        }
        public Argument(IParameter parameter)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Name = null;
            Args = ImmutableArray<string>.Empty;
            Message = Message.NoError;
            Value = default;
        }

        public IParameter Parameter { get; }

        public string? Name { get; }
        public ImmutableArray<string> Args { get; }

        public Message Message { get; }
        public T Value { get; }
    }
}
