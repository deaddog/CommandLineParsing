﻿using System;
using System.Collections.Immutable;

namespace ConsoleTools.Parsing
{
    public class ParserRegistry
    {
        public static ParserRegistry Empty { get; } = new ParserRegistry(ImmutableDictionary<Type, object>.Empty);
        public static ParserRegistry Default { get; } = Empty
            .With(new StringParser())
            .With(new BooleanParser())
            .With(new TryParser<byte>(byte.TryParse, "a byte"))
            .With(new TryParser<sbyte>(sbyte.TryParse, "a signed byte"))
            .With(new TryParser<short>(short.TryParse, "an integer"))
            .With(new TryParser<ushort>(ushort.TryParse, "an integer"))
            .With(new TryParser<int>(int.TryParse, "an integer"))
            .With(new TryParser<uint>(uint.TryParse, "an integer"))
            .With(new TryParser<long>(long.TryParse, "an integer"))
            .With(new TryParser<ulong>(ulong.TryParse, "an integer"))
            .With(new TryParser<float>(float.TryParse, "a numeric value"))
            .With(new TryParser<double>(double.TryParse, "a numeric value"));

        private readonly IImmutableDictionary<Type, object> _parsers;

        private ParserRegistry(IImmutableDictionary<Type, object> parsers)
        {
            _parsers = parsers ?? throw new ArgumentNullException(nameof(parsers));
        }

        public IParser<T> Get<T>()
        {
            if (!TryGet<T>(out var parser))
                throw new MissingParserException(typeof(T), $"Parser registry does not contain a parser definition for type '{typeof(T).Name}'.");
            else
                return parser;
        }
        public bool TryGet<T>(out IParser<T> parser)
        {
            if (_parsers.TryGetValue(typeof(T), out var obj))
            {
                if (obj is not IParser<T> p)
                    throw new InvalidCastException($"Parser registry contained unsupported registration of '{obj.GetType().Name}' as a '{typeof(T).Name}' parser.");

                parser = p;
                return true;
            }
            else
            {
                parser = default!;
                return false;
            }
        }

        public ParserRegistry With<T>(IParser<T> parser)
        {
            return new ParserRegistry
            (
                parsers: _parsers.SetItem(typeof(T), parser ?? throw new ArgumentNullException(nameof(parser)))
            );
        }
        public ParserRegistry Without<T>()
        {
            return new ParserRegistry
            (
                parsers: _parsers.Remove(typeof(T))
            );
        }
    }
}