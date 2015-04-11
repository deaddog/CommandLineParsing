using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides a collection of validation methods for parameters, as extension methods.
    /// These methods are all based on the <see cref="Parameter{T}.Validate"/> method.
    /// </summary>
    public static class ValidationExtension
    {
        /// <summary>
        /// Provides a validation method for the <see cref="Parameter{T}"/>.
        /// </summary>
        /// <typeparam name="T">The value type of the <see cref="Parameter{T}"/>.</typeparam>
        /// <param name="parameter">The <see cref="Parameter{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that takes the parsed value as input and returns <c>true</c> if the value is valid; otherwise is must return <c>false</c>.</param>
        /// <param name="errorMessage">A function that generates the error message that should be the validation result if <see cref="validator"/> returns <c>false</c>.</param>
        public static void Validate<T>(this Parameter<T> parameter, Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            parameter.Validate(x => validator(x) ? Message.NoError : errorMessage(x));
        }
        /// <summary>
        /// Provides a validation method for the <see cref="Parameter{T}"/>.
        /// </summary>
        /// <typeparam name="T">The value type of the <see cref="Parameter{T}"/>.</typeparam>
        /// <param name="parameter">The <see cref="Parameter{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that takes the parsed value as input and returns <c>true</c> if the value is valid; otherwise is must return <c>false</c>.</param>
        /// <param name="errorMessage">The error message that should be the validation result if <see cref="validator"/> returns <c>false</c>.</param>
        public static void Validate<T>(this Parameter<T> parameter, Func<T, bool> validator, Message errorMessage)
        {
            parameter.Validate(x => validator(x) ? Message.NoError : errorMessage);
        }

        /// <summary>
        /// Provides a validation method for the <see cref="Parameter{T}"/> that validates each element in the parsed array.
        /// </summary>
        /// <typeparam name="T">The value type of each element in the <see cref="Parameter{T}"/> value array.</typeparam>
        /// <param name="parameter">The <see cref="Parameter{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that returns the result of validating a single element in <paramref name="parameter"/>.
        /// Use <see cref="Message.NoError"/> to indicate validation success.
        /// If a single element is not validated, the remaining elements are not validated.</param>
        public static void ValidateEach<T>(this Parameter<T[]> parameter, Func<T, Message> validator)
        {
            parameter.Validate(x =>
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
        /// Provides a validation method for the <see cref="Parameter{T}"/> that validates each element in the parsed array.
        /// </summary>
        /// <typeparam name="T">The value type of each element in the <see cref="Parameter{T}"/> value array.</typeparam>
        /// <param name="parameter">The <see cref="Parameter{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that validates a single <typeparamref name="T"/> element.
        /// The method should return <c>true</c> if the value is valid; otherwise <c>false</c>.</param>
        /// <param name="errorMessage">A function that takes the value that could not be validated and returns an error message describing the reason for the message.</param>
        public static void ValidateEach<T>(this Parameter<T[]> parameter, Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            parameter.ValidateEach(x => validator(x) ? Message.NoError : errorMessage(x));
        }
        /// <summary>
        /// Provides a validation method for the <see cref="Parameter{T}"/> that validates each element in the parsed array.
        /// </summary>
        /// <typeparam name="T">The value type of each element in the <see cref="Parameter{T}"/> value array.</typeparam>
        /// <param name="parameter">The <see cref="Parameter{T}"/> to which the validation should be applied.</param>
        /// <param name="validator">A function that validates a single <typeparamref name="T"/> element.
        /// The method should return <c>true</c> if the value is valid; otherwise <c>false</c>.</param>
        /// <param name="errorMessage">The error message that should be returned if validation of a single element fails (if <paramref name="validator"/> returns <c>false</c>).</param>
        public static void ValidateEach<T>(this Parameter<T[]> parameter, Func<T, bool> validator, Message errorMessage)
        {
            parameter.ValidateEach(x => validator(x) ? Message.NoError : errorMessage);
        }

        public static void ValidateRegex(this Parameter<string> parameter, string regex, Func<string, Message> errorMessage)
        {
            ValidateRegex(parameter, new Regex(regex), errorMessage);
        }
        public static void ValidateRegex(this Parameter<string> parameter, Regex regex, Func<string, Message> errorMessage)
        {
            parameter.Validate(x => regex.IsMatch(x) ? Message.NoError : errorMessage(x));
        }
        public static void ValidateRegex(this Parameter<string> parameter, string regex, Message errorMessage)
        {
            ValidateRegex(parameter, new Regex(regex), errorMessage);
        }
        public static void ValidateRegex(this Parameter<string> parameter, Regex regex, Message errorMessage)
        {
            parameter.Validate(x => regex.IsMatch(x) ? Message.NoError : errorMessage);
        }
        public static void ValidateRegex(this Parameter<string> parameter, string regex)
        {
            ValidateRegex(parameter, new Regex(regex));
        }
        public static void ValidateRegex(this Parameter<string> parameter, Regex regex)
        {
            parameter.Validate(x => regex.IsMatch(x) ? Message.NoError : "The \"" + parameter.Name + "\" parameter value must match the regex: [Cyan:" + regex + "]");
        }
    }
}
