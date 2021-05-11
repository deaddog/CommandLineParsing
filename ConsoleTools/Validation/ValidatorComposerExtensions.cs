using System;
using System.Collections.Generic;

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
                onError: value => ConsoleString.Create
                (
                    new ConsoleString.Segment("'", Coloring.Colors.ErrorMessage),
                    new ConsoleString.Segment(value?.ToString() ?? "", Coloring.Colors.ErrorValue),
                    new ConsoleString.Segment($"' is not a valid value", Coloring.Colors.ErrorMessage)
                )
            );
        }
        public static TReturn Where<T, TReturn>(this IValidatorComposer<T, TReturn> composer, Predicate<T> predicate, ConsoleString errorMessage)
        {
            return Where
            (
                composer: composer,
                predicate: predicate,
                errorMessage: new Message(errorMessage)
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
        public static TReturn Where<T, TReturn>(this IValidatorComposer<T, TReturn> composer, Predicate<T> predicate, Func<T, ConsoleString> onError)
        {
            return Where
            (
                composer: composer,
                predicate: predicate,
                onError: x => new Message(onError(x))
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

        public static TReturn WhereAll<T, TItem, TReturn>(this IValidatorComposer<T, TReturn> composer, Func<T, IEnumerable<TItem>> itemsSelector, Func<ValidatorComposer<TItem>, ValidatorComposer<TItem>> compose)
        {
            return WhereAll
            (
                composer: composer,
                itemsSelector: itemsSelector,
                itemValidator: compose(new ValidatorComposer<TItem>(Validator<TItem>.NoRules)).Validator
            );
        }
        public static TReturn WhereAll<T, TItem, TReturn>(this IValidatorComposer<T, TReturn> composer, Func<T, IEnumerable<TItem>> itemsSelector, IValidator<TItem> itemValidator)
        {
            return Where(composer, Validator<T>.Create(x =>
            {
                using (var e = itemsSelector(x).GetEnumerator())
                {
                    var m = itemValidator.Validate(e.Current);
                    if (m.IsError)
                        return m;
                }

                return Message.NoError;
            }));
        }
    }
}
