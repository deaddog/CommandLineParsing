using System;

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

        public static Argument<T> Create<T>(IParameter parameter, string name, string[] args, T value)
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
        public Argument(IParameter parameter, string name, string[] args, T value)
        {
            Parameter = parameter;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
            Used = true;
        }
        public Argument(IParameter parameter)
        {
            Parameter = parameter;
            Name = null;
            Value = default;
            Used = false;
        }

        public IParameter Parameter { get; }
        public string? Name { get; }
        public string[]? Args { get; }

        public bool Used { get; }
        public T Value { get; }
    }
}
