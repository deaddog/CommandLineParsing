using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Defines a collection of validation methods, that can be applied to data of the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements validated by the <see cref="Validator{T}"/>.</typeparam>
    public class Validator<T>
    {
        private EnsureCollection ensure;
        private FailureCollection fail;
        private List<Func<T, Message>> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator{T}"/> class that accepts any <typeparamref name="T"/> values.
        /// </summary>
        public Validator()
        {
            this.validators = new List<Func<T, Message>>();

            this.ensure = new EnsureCollection(this);
            this.fail = new FailureCollection(this);
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

        /// <summary>
        /// Gets an <see cref="EnsureCollection"/> that provides methods to ensure that certain conditions are met.
        /// </summary>
        public EnsureCollection Ensure => ensure;
        /// <summary>
        /// Gets a <see cref="FailureCollection"/> that provides methods to ensure that certain conditions are not met.
        /// </summary>
        public FailureCollection Fail => fail;

        /// <summary>
        /// Provides methods for describing validation methods that ensure conditions are true.
        /// </summary>
        public class EnsureCollection
        {
            private Validator<T> parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="Validator{T}.EnsureCollection" /> class.
            /// </summary>
            /// <param name="validator">The <see cref="Validator{T}"/> to which this <see cref="Validator{T}.EnsureCollection"/> should add validation methods.</param>
            public EnsureCollection(Validator<T> validator)
            {
                if (validator == null)
                    throw new ArgumentNullException(nameof(validator));

                this.parent = validator;
            }
        }
        /// <summary>
        /// Provides methods for describing validation methods that should cause validation to fail if true.
        /// </summary>
        public class FailureCollection
        {
            private Validator<T> parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="Validator{T}.FailureCollection" /> class.
            /// </summary>
            /// <param name="validator">The <see cref="Validator{T}"/> to which this <see cref="Validator{T}.FailureCollection"/> should add validation methods.</param>
            public FailureCollection(Validator<T> validator)
            {
                if (validator == null)
                    throw new ArgumentNullException(nameof(validator));

                this.parent = validator;
            }
        }
    }
}
