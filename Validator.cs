using System;
using System.Collections;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Defines a collection of validation methods.
    /// </summary>
    public class Validator : IEnumerable<Func<Message>>
    {
        private List<Func<Message>> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator"/> class.
        /// </summary>
        public Validator()
        {
            this.validators = new List<Func<Message>>();
        }

        /// <summary>
        /// Provides a validation method for this <see cref="Validator"/>.
        /// </summary>
        /// <param name="validator">A function that validates state and returns a <see cref="Message"/>.
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var v in validators)
                yield return v;
        }
        IEnumerator<Func<Message>> IEnumerable<Func<Message>>.GetEnumerator()
        {
            foreach (var v in validators)
                yield return v;
        }
    }
}
