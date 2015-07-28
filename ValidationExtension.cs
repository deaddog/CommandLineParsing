using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides a collection of validation methods for parameters, as extension methods.
    /// These methods are all based on the <see cref="Validator{T}.Validate"/> method.
    /// </summary>
    public static class ValidationExtension
    {
        /// <summary>
        /// Provides a validation method for this <see cref="Validator{T}"/>.
        /// </summary>
        /// <typeparam name="T">The value type of the <see cref="Validator{T}"/>.</typeparam>
        /// <param name="validatorElement">The <see cref="Validator{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that takes the parsed value as input and returns <c>true</c> if the value is valid; otherwise is must return <c>false</c>.</param>
        /// <param name="errorMessage">A function that generates the error message that should be the validation result if <paramref name="validator"/> returns <c>false</c>.</param>
        public static void Add<T>(this Validator<T> validatorElement, Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            validatorElement.Add(x => validator(x) ? Message.NoError : errorMessage(x));
        }
        /// <summary>
        /// Provides a validation method for the <see cref="Validator{T}"/>.
        /// </summary>
        /// <typeparam name="T">The value type of the <see cref="Validator{T}"/>.</typeparam>
        /// <param name="validatorElement">The <see cref="Validator{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that takes the parsed value as input and returns <c>true</c> if the value is valid; otherwise is must return <c>false</c>.</param>
        /// <param name="errorMessage">The error message that should be the validation result if <paramref name="validator"/> returns <c>false</c>.</param>
        public static void Add<T>(this Validator<T> validatorElement, Func<T, bool> validator, Message errorMessage)
        {
            validatorElement.Add(x => validator(x) ? Message.NoError : errorMessage);
        }
        /// <summary>
        /// Provides a validation method for the <see cref="Validator{T}"/>.
        /// Errors in validation return a generic error message.
        /// </summary>
        /// <typeparam name="T">The value type of the <see cref="Validator{T}"/>.</typeparam>
        /// <param name="validatorElement">The <see cref="Validator{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that takes the parsed value as input and returns <c>true</c> if the value is valid; otherwise is must return <c>false</c>.</param>
        public static void Add<T>(this Validator<T> validatorElement, Func<T, bool> validator)
        {
            validatorElement.Add(x => validator(x) ? Message.NoError : $"Invalid value '{x}'.");
        }

        /// <summary>
        /// Provides a validation method for the <see cref="Validator{T}"/> that validates each element in an array.
        /// </summary>
        /// <typeparam name="T">The element type of the <see cref="Validator{T}"/> array.</typeparam>
        /// <param name="validatorElement">The <see cref="Validator{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that returns the result of validating a single element in <paramref name="validatorElement"/>.
        /// Use <see cref="Message.NoError"/> to indicate validation success.
        /// If a single element is not validated, the remaining elements are not validated.</param>
        public static void AddForeach<T>(this Validator<T[]> validatorElement, Func<T, Message> validator)
        {
            validatorElement.Add(x =>
            {
                for (int i = 0; i < x.Length; i++)
                {
                    var msg = validator(x[i]);
                    if (msg.IsError)
                        return msg;
                }
                return Message.NoError;
            });
        }
        /// <summary>
        /// Provides a validation method for the <see cref="Validator{T}"/> that validates each element in an array.
        /// </summary>
        /// <typeparam name="T">The element type of the <see cref="Validator{T}"/> array.</typeparam>
        /// <param name="validatorElement">The <see cref="Validator{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that validates a single <typeparamref name="T"/> element.
        /// The method should return <c>true</c> if the value is valid; otherwise <c>false</c>.</param>
        /// <param name="errorMessage">A function that takes the value that could not be validated and returns an error message describing the reason for the message.</param>
        public static void AddForeach<T>(this Validator<T[]> validatorElement, Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            validatorElement.AddForeach(x => validator(x) ? Message.NoError : errorMessage(x));
        }
        /// <summary>
        /// Provides a validation method for the <see cref="Validator{T}"/> that validates each element in an array.
        /// </summary>
        /// <typeparam name="T">The element type of the <see cref="Validator{T}"/> array.</typeparam>
        /// <param name="validatorElement">The <see cref="Validator{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that validates a single <typeparamref name="T"/> element.
        /// The method should return <c>true</c> if the value is valid; otherwise <c>false</c>.</param>
        /// <param name="errorMessage">The error message that should be returned if validation of a single element fails (if <paramref name="validator"/> returns <c>false</c>).</param>
        public static void AddForeach<T>(this Validator<T[]> validatorElement, Func<T, bool> validator, Message errorMessage)
        {
            validatorElement.AddForeach(x => validator(x) ? Message.NoError : errorMessage);
        }


        /// <summary>
        /// Validates each element in <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of elements that should be validated.</param>
        /// <param name="validator">A function that returns the result of validating a single element in <paramref name="collection"/>.
        /// Use <see cref="Message.NoError"/> to indicate validation success.
        /// If a single element is not validated, the remaining elements are not validated.</param>
        /// <returns>The <see cref="Message"/> that is the result of validating each element in <paramref name="collection"/>.</returns>
        public static Message ValidateEach<T>(this IEnumerable<T> collection, Func<T, Message> validator)
        {
            foreach (var t in collection)
            {
                var msg = validator(t);
                if (msg.IsError)
                    return msg;
            }
            return Message.NoError;
        }
        /// <summary>
        /// Validates each element in <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of elements that should be validated.</param>
        /// <param name="validator">A function that validates a single <typeparamref name="T"/> element.
        /// The method should return <c>true</c> if the value is valid; otherwise <c>false</c>.</param>
        /// <param name="errorMessage">A function that takes the value that could not be validated and returns an error message describing the reason for the message.</param>
        /// <returns>The <see cref="Message"/> that is the result of validating each element in <paramref name="collection"/>.</returns>
        public static Message ValidateEach<T>(this IEnumerable<T> collection, Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            return ValidateEach(collection, x => validator(x) ? Message.NoError : errorMessage(x));
        }
        /// <summary>
        /// Validates each element in <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of elements that should be validated.</param>
        /// <param name="validator">A function that validates a single <typeparamref name="T"/> element.
        /// The method should return <c>true</c> if the value is valid; otherwise <c>false</c>.</param>
        /// <param name="errorMessage">The error message that should be returned if validation of a single element fails (if <paramref name="validator"/> returns <c>false</c>).</param>
        /// <returns>The <see cref="Message"/> that is the result of validating each element in <paramref name="collection"/>.</returns>
        public static Message ValidateEach<T>(this IEnumerable<T> collection, Func<T, bool> validator, Message errorMessage)
        {
            return ValidateEach(collection, x => validator(x) ? Message.NoError : errorMessage);
        }

        public static void AddRegex(this Validator<string> validatorElement, string regex, Func<string, Message> errorMessage)
        {
            AddRegex(validatorElement, new Regex(regex), errorMessage);
        }
        public static void AddRegex(this Validator<string> validatorElement, Regex regex, Func<string, Message> errorMessage)
        {
            validatorElement.Add(x => regex.IsMatch(x) ? Message.NoError : errorMessage(x));
        }
        public static void AddRegex(this Validator<string> validatorElement, string regex, Message errorMessage)
        {
            AddRegex(validatorElement, new Regex(regex), errorMessage);
        }
        public static void AddRegex(this Validator<string> validatorElement, Regex regex, Message errorMessage)
        {
            validatorElement.Add(x => regex.IsMatch(x) ? Message.NoError : errorMessage);
        }
        public static void AddRegex(this Validator<string> validatorElement, string regex)
        {
            AddRegex(validatorElement, new Regex(regex));
        }
        public static void AddRegex(this Validator<string> validatorElement, Regex regex)
        {
            validatorElement.Add(x => regex.IsMatch(x) ? Message.NoError : "The string \"" + x + "\" does not match the regex: [Cyan:" + regex + "]");
        }
    }
}
