using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ConsoleTools.Applications
{
    public class ArgumentSet
    {
        private readonly IImmutableList<object> _arguments;

        public static ArgumentSet Create<T>(Argument<T> argument) where T : notnull
        {
            return new ArgumentSet
            (
                arguments: ImmutableList.Create<object>(argument)
            );
        }
        public static ArgumentSet Create<T>(IParameter parameter) where T : notnull
        {
            return new ArgumentSet
            (
                arguments: ImmutableList.Create<object>(new Argument<T>
                (
                    parameter: parameter
                ))
            );
        }
        public static ArgumentSet Create<T>(IParameter parameter, string name, ImmutableArray<string> args, T value) where T : notnull
        {
            return new ArgumentSet
            (
                arguments: ImmutableList.Create<object>(new Argument<T>
                (
                    parameter: parameter,
                    name: name,
                    args: args,
                    value: value
                ))
            );
        }

        public static ArgumentSet Merge(ArgumentSet set1, ArgumentSet set2)
        {
            return new ArgumentSet
            (
                arguments: set1._arguments.AddRange(set2._arguments)
            );
        }
        public static ArgumentSet Merge(IEnumerable<ArgumentSet> sets)
        {
            return new ArgumentSet
            (
                arguments: sets.Select(s => s._arguments).Aggregate((a, b) => a.AddRange(b))
            );
        }

        public static ArgumentSet Empty { get; }

        private ArgumentSet(IImmutableList<object> arguments)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        //public Argument<T>? GetByName<T>(string name)
        //{
        //    if (_arguments.TryGetValue(name, out var obj))
        //    {
        //        if (!(obj is Argument<T> arg))
        //            throw new ArgumentException($"The argument '{name}' is not of type '{typeof(T).Name}'.", nameof(name));

        //        return arg;
        //    }

        //    return null;
        //}

        //public Argument<T>? Get<T>(IParameter parameter)
        //{
        //    return GetByName<T>(parameter.ToString()) ?? Input.Create<T>(parameter);
        //}
    }
}
