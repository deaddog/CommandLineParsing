using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CommandLineParsing.Validation
{
    /// <summary>
    /// Provides a set of factory methods for constructing <see cref="IValidator"/>s.
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Creates a validator that always returns <see cref="Message.NoError"/>.
        /// </summary>
        public static IValidator NoRules => new AndValidator(ImmutableList<IValidator>.Empty);

        /// <summary>
        /// Creates a <see cref="IValidator"/> from a function.
        /// </summary>
        /// <param name="validator">The validation method that defines the context validation.</param>
        /// <returns>A <see cref="IValidator"/> defined by <paramref name="validator"/>.</returns>
        public static IValidator Create(Func<Message> validator) => new FuncValidator(validator);
        /// <summary>
        /// Creates a <see cref="IValidator"/> from a collection of validators, checking them in sequence.
        /// </summary>
        /// <param name="validators">A collection of <see cref="IValidator"/>s that should be composed into a new validator.</param>
        /// <returns>A validator that checks all <paramref name="validators"/> in sequence.</returns>
        public static IValidator And(IEnumerable<IValidator> validators) => new AndValidator(validators.ToImmutableList());
        /// <summary>
        /// Creates a <see cref="IValidator"/> from a collection of validators, checking them in sequence.
        /// </summary>
        /// <param name="validators">A collection of <see cref="IValidator"/>s that should be composed into a new validator.</param>
        /// <returns>A validator that checks all <paramref name="validators"/> in sequence.</returns>
        public static IValidator And(params IValidator[] validators) => new AndValidator(validators.ToImmutableList());
    }

    /// <summary>
    /// Provides a set of factory methods for constructing <see cref="IValidator{T}"/>s.
    /// </summary>
    /// <typeparam name="T">The type of elements the constructed validators will be able to validate.</typeparam>
    public static class Validator<T>
    {
        /// <summary>
        /// Creates a validator that always returns <see cref="Message.NoError"/>.
        /// </summary>
        public static IValidator<T> NoRules => new AndValidator<T>(ImmutableList<IValidator<T>>.Empty);

        /// <summary>
        /// Creates a <see cref="IValidator{T}"/> from a function.
        /// </summary>
        /// <param name="validator">The validation method that defines the value validation.</param>
        /// <returns>A <see cref="IValidator{T}"/> defined by <paramref name="validator"/>.</returns>
        public static IValidator<T> Create(Func<T, Message> validator) => new FuncValidator<T>(validator);
        /// <summary>
        /// Creates a <see cref="IValidator{T}"/> from a collection of validators, checking them in sequence.
        /// </summary>
        /// <param name="validators">A collection of <see cref="IValidator{T}"/>s that should be composed into a new validator.</param>
        /// <returns>A validator that checks all <paramref name="validators"/> in sequence.</returns>
        public static IValidator<T> And(IEnumerable<IValidator<T>> validators) => new AndValidator<T>(validators.ToImmutableList());
        /// <summary>
        /// Creates a <see cref="IValidator{T}"/> from a collection of validators, checking them in sequence.
        /// </summary>
        /// <param name="validators">A collection of <see cref="IValidator{T}"/>s that should be composed into a new validator.</param>
        /// <returns>A validator that checks all <paramref name="validators"/> in sequence.</returns>
        public static IValidator<T> And(params IValidator<T>[] validators) => new AndValidator<T>(validators.ToImmutableList());
    }
}
