using System;

namespace ConsoleTools.Validation
{
    public static class ValidatorComposerExtensions
    {
        public static TReturn Where<T, TReturn>(this IValidatorComposer<T, TReturn> composer, Predicate<T> predicate)
        {
            return Where
            (
                composer: composer,
                predicate: predicate,
                onError: value => new Message(ConsoleString.Create
                (
                    new ConsoleString.Segment("'", Coloring.Colors.ErrorMessage),
                    new ConsoleString.Segment(value?.ToString() ?? "", Coloring.Colors.ErrorValue),
                    new ConsoleString.Segment($"' is not a valid value", Coloring.Colors.ErrorMessage)
                ))
            );
        }
        public static TReturn Where<T, TReturn>(this IValidatorComposer<T, TReturn> composer, Predicate<T> predicate, Message errorMessage)
        {
            return Where
            (
                composer: composer,
                validator: Validator<T>.Create(value => predicate(value) ? Message.NoError : errorMessage)
            );
        }
        public static TReturn Where<T, TReturn>(this IValidatorComposer<T, TReturn> composer, Predicate<T> predicate, Func<T, Message> onError)
        {
            return Where
            (
                composer: composer,
                validator: Validator<T>.Create(x => predicate(x) ? Message.NoError : onError(x))
            );
        }
        public static TReturn Where<T, TReturn>(this IValidatorComposer<T, TReturn> composer, IValidator<T> validator)
        {
            return composer.WithValidator
            (
                validator: AndValidator<T>.Create
                (
                    composer.Validator,
                    validator
                )
            );
        }
    }
}
