using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Defines a collection of validation methods.
    /// </summary>
    public class Validator
    {
        private EnsureCollection ensure;
        private FailureCollection fail;
        private List<Func<Message>> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator"/> class.
        /// </summary>
        public Validator()
        {
            this.validators = new List<Func<Message>>();

            this.ensure = new EnsureCollection(this);
            this.fail = new FailureCollection(this);
        }

        /// <summary>
        /// Provides a validation method for this <see cref="Validator"/>.
        /// </summary>
        /// <param name="validator">A function that performs some validation and returns a <see cref="Message"/> with the result.
        /// If the validation was successful <see cref="Message.NoError"/> should be returned by the method; otherwise an appropriate <see cref="Message"/> should be returned.</param>
        public void Add(Func<Message> validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            this.validators.Add(validator);
        }

        /// <summary>
        /// Validates that only one of <paramref name="parameters"/> is set.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><see cref="Message.NoError"/> if zero or one <see cref="Parameter"/> is set; otherwise an error message describing the problem.</returns>
        public void AddOnlyOne(params Parameter[] parameters)
        {
            Add(() =>
            {
                Parameter first = null;

                for (int i = 0; i < parameters.Length; i++)
                    if (parameters[i].IsSet)
                    {
                        if (first == null)
                            first = parameters[i];
                        else
                            return string.Format("The {0} {1} cannot be used with the {2} {3}.",
                                first.Name, first is FlagParameter ? "flag" : "parameter",
                                parameters[i].Name, parameters[i] is FlagParameter ? "flag" : "parameter");
                    }

                return Message.NoError;
            });
        }
        /// <summary>
        /// Validates that if <paramref name="first"/> is set, none of <paramref name="parameters"/> is set.
        /// </summary>
        /// <param name="first">The first <see cref="Parameter"/>. If this is set, none of the remaining parameters can be set.</param>
        /// <param name="parameters">The <see cref="Parameter"/>s to check if <paramref name="first"/> is set.</param>
        public void AddIfFirstNotRest(Parameter first, params Parameter[] parameters)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            Add(() =>
            {
                if (!first.IsSet)
                    return Message.NoError;

                for (int i = 0; i < parameters.Length; i++)
                    if (parameters[i].IsSet)
                        return string.Format("The {0} {1} cannot be used with the {2} {3}.",
                            first.Name, first is FlagParameter ? "flag" : "parameter",
                            parameters[i].Name, parameters[i] is FlagParameter ? "flag" : "parameter");

                return Message.NoError;
            });
        }

        /// <summary>
        /// Validates using the validation methods stored in this <see cref="Validator"/>.
        /// </summary>
        /// <returns>A <see cref="Message"/> representing the error that occured during validation; or <see cref="Message.NoError"/> if no error occured.</returns>
        public Message Validate()
        {
            for (int i = 0; i < validators.Count; i++)
            {
                var msg = validators[i]();
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
        /// Provides a convenient way to define complex validation methods for a <see cref="Validator"/> object.
        /// </summary>
        public class ValidationAdder
        {
            /// <summary>
            /// A method that adds validation methods to the <see cref="Validator"/> that this <see cref="ValidationAdder"/> is associated with.
            /// </summary>
            protected readonly Action<Func<Message>> Add;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValidationAdder" /> class.
            /// </summary>
            /// <param name="validator">The <see cref="Validator"/> to which this <see cref="ValidationAdder"/> should add validation methods.</param>
            public ValidationAdder(Validator validator)
            {
                if (validator == null)
                    throw new ArgumentNullException(nameof(validator));

                this.Add = validator.Add;
            }
        }

        /// <summary>
        /// Provides methods for describing validation methods that ensure conditions are true.
        /// </summary>
        public class EnsureCollection
        {
            private Validator parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="EnsureCollection" /> class.
            /// </summary>
            /// <param name="validator">The <see cref="Validator"/> to which this <see cref="EnsureCollection"/> should add validation methods.</param>
            public EnsureCollection(Validator validator)
            {
                if (validator == null)
                    throw new ArgumentNullException(nameof(validator));

                this.parent = validator;
            }

            /// <summary>
            /// Ensures that a certain condition is met before continuing the validation.
            /// If it isn't <paramref name="errorMessage"/> is returned when validating.
            /// </summary>
            /// <param name="condition">The condition (expressed as a function) that should evaluate to <c>true</c> to pass validation.</param>
            /// <param name="errorMessage">The error message to return if <paramref name="condition"/> evaluates to <c>false</c>.</param>
            public void That(Func<bool> condition, Message errorMessage)
            {
                if (condition == null)
                    throw new ArgumentNullException(nameof(condition));
                if (errorMessage == null)
                    throw new ArgumentNullException(nameof(errorMessage));
                if (!errorMessage.IsError)
                    throw new ArgumentException($"Error message cannot be {nameof(Message.NoError)}.", nameof(errorMessage));

                parent.Add(() => condition() ? Message.NoError : errorMessage);
            }
        }
        /// <summary>
        /// Provides methods for describing validation methods that should cause validation to fail if true.
        /// </summary>
        public class FailureCollection
        {
            private Validator parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="FailureCollection" /> class.
            /// </summary>
            /// <param name="validator">The <see cref="Validator"/> to which this <see cref="FailureCollection"/> should add validation methods.</param>
            public FailureCollection(Validator validator)
            {
                if (validator == null)
                    throw new ArgumentNullException(nameof(validator));

                this.parent = validator;
            }

            /// <summary>
            /// Tests if a failure condition (expressed as a function) is <c>true</c>.
            /// If so, validation fails and returns <paramref name="errorMessage"/>.
            /// </summary>
            /// <param name="condition">The condition (expressed as a function) that should evaluate to <c>false</c> to pass validation.</param>
            /// <param name="errorMessage">The error message to return if <paramref name="condition"/> evaluates to <c>true</c>.</param>
            public void If(Func<bool> condition, Message errorMessage)
            {
                if (condition == null)
                    throw new ArgumentNullException(nameof(condition));
                if (errorMessage == null)
                    throw new ArgumentNullException(nameof(errorMessage));
                if (!errorMessage.IsError)
                    throw new ArgumentException($"Error message cannot be {nameof(Message.NoError)}.", nameof(errorMessage));

                parent.Add(() => condition() ? errorMessage : Message.NoError);
            }
        }
    }
}
