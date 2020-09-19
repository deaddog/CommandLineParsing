using System;
using System.Collections.Generic;

namespace CommandLineParsing.Validation
{
    /// <summary>
    /// Defines extension methods for composing validators.
    /// </summary>
    public static class ValidatorComposerExtensions
    {
        /// <summary>
        /// Adds a predicate method to the validator composer, with a generic message when the predicate does not hold true.
        /// </summary>
        /// <typeparam name="TReturn">The returned composer type. See <see cref="IValidatorComposer{TReturn}"/> for details.</typeparam>
        /// <param name="composer">The validator composer being extended.</param>
        /// <param name="predicate">A method that returns a bool indicating if validation was successful.</param>
        /// <returns>A <typeparamref name="TReturn"/> that extends <paramref name="composer"/> with <paramref name="predicate"/>.</returns>
        public static TReturn Where<TReturn>(this IValidatorComposer<TReturn> composer, Func<bool> predicate)
        {
            return Where(composer, predicate, new Message("Validation failed."));
        }
        /// <summary>
        /// Adds a predicate method to the validator composer, using <paramref name="errorMessage"/> when the predicate does not hold true.
        /// </summary>
        /// <typeparam name="TReturn">The returned composer type. See <see cref="IValidatorComposer{TReturn}"/> for details.</typeparam>
        /// <param name="composer">The validator composer being extended.</param>
        /// <param name="predicate">A method that returns a bool indicating if validation was successful.</param>
        /// <param name="errorMessage">The message returned when <paramref name="predicate"/> returns <c>false</c>.</param>
        /// <returns>A <typeparamref name="TReturn"/> that extends <paramref name="composer"/> with <paramref name="predicate"/>.</returns>
        public static TReturn Where<TReturn>(this IValidatorComposer<TReturn> composer, Func<bool> predicate, Message errorMessage)
        {
            return Where(composer, Validator.Create(() => predicate() ? Message.NoError : errorMessage));
        }
        /// <summary>
        /// Adds a validator to the validator composer with an <c>And</c> operator.
        /// </summary>
        /// <typeparam name="TReturn">The returned composer type. See <see cref="IValidatorComposer{TReturn}"/> for details.</typeparam>
        /// <param name="composer">The validator composer being extended.</param>
        /// <param name="validator">A validator that should be added to the composer.</param>
        /// <returns>A <typeparamref name="TReturn"/> that extends <paramref name="composer"/> with the validation rules defined by <paramref name="validator"/>.</returns>
        public static TReturn Where<TReturn>(this IValidatorComposer<TReturn> composer, IValidator validator)
        {
            return composer.WithValidator(Validator.And(composer.Validator, validator));
        }

        /// <summary>
        /// Adds a predicate method to the validator composer, with a generic message when the predicate does not hold true.
        /// </summary>
        /// <typeparam name="T">The type of elements the final validator will be able to validate.</typeparam>
        /// <typeparam name="TReturn">The returned composer type. See <see cref="IValidatorComposer{T, TReturn}"/> for details.</typeparam>
        /// <param name="composer">The validator composer being extended.</param>
        /// <param name="predicate">A method that takes an element for validation as input and returns a bool indicating if validation was successful.</param>
        /// <returns>A <typeparamref name="TReturn"/> that extends <paramref name="composer"/> with <paramref name="predicate"/>.</returns>
        public static TReturn Where<T, TReturn>(this IValidatorComposer<T, TReturn> composer, Predicate<T> predicate)
        {
            return Where(composer, predicate, x => new Message($"[red:{x}] is not a valid value."));
        }
        /// <summary>
        /// Adds a predicate method to the validator composer, using <paramref name="errorMessage"/> when the predicate does not hold true.
        /// </summary>
        /// <typeparam name="T">The type of elements the final validator will be able to validate.</typeparam>
        /// <typeparam name="TReturn">The returned composer type. See <see cref="IValidatorComposer{T, TReturn}"/> for details.</typeparam>
        /// <param name="composer">The validator composer being extended.</param>
        /// <param name="predicate">A method that takes an element for validation as input and returns a bool indicating if validation was successful.</param>
        /// <param name="errorMessage">The message returned when <paramref name="predicate"/> returns <c>false</c>.</param>
        /// <returns>A <typeparamref name="TReturn"/> that extends <paramref name="composer"/> with <paramref name="predicate"/>.</returns>
        public static TReturn Where<T, TReturn>(this IValidatorComposer<T, TReturn> composer, Predicate<T> predicate, Message errorMessage)
        {
            return Where(composer, Validator<T>.Create(x => predicate(x) ? Message.NoError : errorMessage));
        }
        /// <summary>
        /// Adds a predicate method to the validator composer, using <paramref name="onError"/> when the predicate does not hold true.
        /// </summary>
        /// <typeparam name="T">The type of elements the final validator will be able to validate.</typeparam>
        /// <typeparam name="TReturn">The returned composer type. See <see cref="IValidatorComposer{T, TReturn}"/> for details.</typeparam>
        /// <param name="composer">The validator composer being extended.</param>
        /// <param name="predicate">A method that takes an element for validation as input and returns a bool indicating if validation was successful.</param>
        /// <param name="onError">A method that generates an error message from the invalid element, when <paramref name="predicate"/> returns <c>false</c>.</param>
        /// <returns>A <typeparamref name="TReturn"/> that extends <paramref name="composer"/> with <paramref name="predicate"/>.</returns>
        public static TReturn Where<T, TReturn>(this IValidatorComposer<T, TReturn> composer, Predicate<T> predicate, Func<T, Message> onError)
        {
            return Where(composer, Validator<T>.Create(x => predicate(x) ? Message.NoError : onError(x)));
        }
        /// <summary>
        /// Adds a validator to the validator composer with an <c>And</c> operator.
        /// </summary>
        /// <typeparam name="T">The type of elements the final validator will be able to validate.</typeparam>
        /// <typeparam name="TReturn">The returned composer type. See <see cref="IValidatorComposer{T, TReturn}"/> for details.</typeparam>
        /// <param name="composer">The validator composer being extended.</param>
        /// <param name="validator">A validator that should be added to the composer.</param>
        /// <returns>A <typeparamref name="TReturn"/> that extends <paramref name="composer"/> with the validation rules defined by <paramref name="validator"/>.</returns>
        public static TReturn Where<T, TReturn>(this IValidatorComposer<T, TReturn> composer, IValidator<T> validator)
        {
            return composer.WithValidator(Validator<T>.And(composer.Validator, validator));
        }

        /// <summary>
        /// Adds a validator to the validator composer, checking all elements in a collection.
        /// </summary>
        /// <typeparam name="T">The type of elements the final validator will be able to validate.</typeparam>
        /// <typeparam name="TItem">The type of elements in the inner collection.</typeparam>
        /// <typeparam name="TReturn">The returned composer type. See <see cref="IValidatorComposer{T, TReturn}"/> for details.</typeparam>
        /// <param name="composer">The validator composer being extended.</param>
        /// <param name="itemsSelector">A method that gets the collection of elments to validate.</param>
        /// <param name="itemValidatorComposer">A method for fluently configure validation of each element returned by <paramref name="itemsSelector"/>.</param>
        /// <returns>A <typeparamref name="TReturn"/> that extends <paramref name="composer"/> with the validation rules defined by <paramref name="itemValidatorComposer"/>.</returns>
        public static TReturn WhereAll<T, TItem, TReturn>(this IValidatorComposer<T, TReturn> composer, Func<T, IEnumerable<TItem>> itemsSelector, Func<ValidatorComposer<TItem>, ValidatorComposer<TItem>> itemValidatorComposer)
        {
            return WhereAll(composer, itemsSelector, itemValidatorComposer(ValidatorComposer<TItem>.NoRules).Validator);
        }
        /// <summary>
        /// Adds a validator to the validator composer, checking all elements in a collection.
        /// </summary>
        /// <typeparam name="T">The type of elements the final validator will be able to validate.</typeparam>
        /// <typeparam name="TItem">The type of elements in the inner collection.</typeparam>
        /// <typeparam name="TReturn">The returned composer type. See <see cref="IValidatorComposer{T, TReturn}"/> for details.</typeparam>
        /// <param name="composer">The validator composer being extended.</param>
        /// <param name="itemsSelector">A method that gets the collection of elments to validate.</param>
        /// <param name="itemValidator">The validator that should be used to validate each element in the collection returned by <paramref name="itemsSelector"/>.</param>
        /// <returns>A <typeparamref name="TReturn"/> that extends <paramref name="composer"/> with the validation rules defined by <paramref name="itemValidator"/>.</returns>
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
