using System;
using System.Collections;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Defines a collection of validation methods, that can be applied to data of the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements validated by the <see cref="Validator{T}"/>.</typeparam>
    public class Validator<T> : IEnumerable<Func<T, Message>>
    {
        private List<Func<T, Message>> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator{T}"/> class that accepts any <typeparamref name="T"/> values.
        /// </summary>
        public Validator()
        {
            this.validators = new List<Func<T, Message>>();
        }

        /// <summary>
        /// Provides a validation method for this <see cref="Validator{T}"/>.
        /// </summary>
        /// <param name="validator">A function that validates the parsed <typeparamref name="T"/> value and returns a <see cref="Message"/>.
        /// If the validation was successful <see cref="Message.NoError"/> should be returned by the method; otherwise an appropriate <see cref="Message"/> should be returned.</param>
        public void Add(Func<T, Message> validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            this.validators.Add(validator);
        }

        /// <summary>
        /// Provides a validation method for this <see cref="Validator{T}"/>.
        /// </summary>
        /// <param name="validator">A function that takes the parsed value as input and returns <c>true</c> if the value is valid; otherwise is must return <c>false</c>.</param>
        /// <param name="errorMessage">A function that generates the error message that should be the validation result if <paramref name="validator"/> returns <c>false</c>.</param>
        public void Add(Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            Add(x => validator(x) ? Message.NoError : errorMessage(x));
        }
        /// <summary>
        /// Provides a validation method for the <see cref="Validator{T}"/>.
        /// </summary>
        /// <param name="validator">A function that takes the parsed value as input and returns <c>true</c> if the value is valid; otherwise is must return <c>false</c>.</param>
        /// <param name="errorMessage">The error message that should be the validation result if <paramref name="validator"/> returns <c>false</c>.</param>
        public void Add(Func<T, bool> validator, Message errorMessage)
        {
            Add(x => validator(x) ? Message.NoError : errorMessage);
        }
        /// <summary>
        /// Provides a validation method for the <see cref="Validator{T}"/>.
        /// Errors in validation return a generic error message.
        /// </summary>
        /// <param name="validator">A function that takes the parsed value as input and returns <c>true</c> if the value is valid; otherwise is must return <c>false</c>.</param>
        public void Add(Func<T, bool> validator)
        {
            Add(x => validator(x) ? Message.NoError : $"Invalid value '{x}'.");
        }

        /// <summary>
        /// Validates <paramref name="value"/> using the validation methods stored in this <see cref="Validator{T}"/>.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>A <see cref="Message"/> representing the error that occured during validation; or <see cref="Message.NoError"/> if no error occured.</returns>
        public Message Validate(T value)
        {
            for (int i = 0; i < validators.Count; i++)
            {
                var msg = validators[i](value);
                if (msg.IsError)
                    return msg;
            }

            return Message.NoError;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var v in validators)
                yield return v;
        }
        IEnumerator<Func<T, Message>> IEnumerable<Func<T, Message>>.GetEnumerator()
        {
            foreach (var v in validators)
                yield return v;
        }
    }
}
