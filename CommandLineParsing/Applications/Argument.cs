using System;
using System.Collections.Immutable;

namespace CommandLineParsing.Applications
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
        public static Argument<T> Create<T>(IParameter parameter, string name, ImmutableArray<string> args, T value)
        {
            return new Argument<T>
            (
                parameter: parameter,
                name: name,
                args: args,
                value: value
            );
        }
    }

    public class Argument<T> where T: notnull
    {
        public Argument(IParameter parameter, string name, ImmutableArray<string> args, T value)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Used = true;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Args = args;
            Value = value;
        }
        public Argument(IParameter parameter)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Used = false;
            Name = null;
            Args = ImmutableArray<string>.Empty;
            Value = default;
        }

        public IParameter Parameter { get; }

        public bool Used { get; }
        public string? Name { get; }

        public ImmutableArray<string> Args { get; }
        public T Value { get; }
    }
}
