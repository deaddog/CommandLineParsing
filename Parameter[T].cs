using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a <see cref="Parameter"/> that is associated with a value type.
    /// A <see cref="Parameter{T}"/> will read a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value that is accepted for the <see cref="Parameter"/>.
    /// This type can be any type that has a static <see cref="TryParse{T}"/> method, an array of such a type or any enumeration type.</typeparam>
    public class Parameter<T> : Parameter
    {
#pragma warning disable 1591

        protected T value;

        private SmartParser<T> parser;

        protected readonly Validator<T> validator;

        private Message defaultTypeError(string input)
        {
            if (Name == null)
                return $@"The ""{input}"" argument could not be parsed to a value of type {typeof(T).Name}.";
            else
                return $@"The ""{input}"" argument for the parameter ""{Name}"", could not be parsed to a value of type {typeof(T).Name}.";
        }

#pragma warning restore

        internal Parameter(string name, string[] alternatives, string description, Message required, bool enumIgnore)
            : base(name, alternatives, description, required)
        {
            this.value = default(T);

            this.parser = new SmartParser<T>(
                enumIgnore,
                $"The type { typeof(T).Name } is not supported. Set a parser method using the {nameof(SetParser)} method.",
                $"No value provided for argument \"{name}\".",
                $"Only one value can be provided for argument \"{name}\".",
                defaultTypeError, true);

            this.validator = new Validator<T>();
        }

        /// <summary>
        /// Gets or sets the value parsed by this <see cref="Parameter{T}"/>, or the default value if <see cref="Parameter.IsSet"/> is <c>false</c>.
        /// Setting the value will override any previously parsed value and any default value.
        /// </summary>
        /// <value>
        /// The value parsed by this <see cref="Parameter{T}"/>.
        /// </value>
        public virtual T Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Sets the default value for this <see cref="Parameter{T}"/>.
        /// This method replaces any value set by previous invocations of the method and any value set using the <see cref="Default"/> attribute.
        /// Unlike the <see cref="Default"/> attribute, this methods allows for using an expression as the default value.
        /// </summary>
        /// <param name="value">The default value of this <see cref="Parameter{T}"/> if the parameter is not used when executing its containing command.</param>
        public void SetDefault(T value)
        {
            if (this.IsRequired)
                throw new InvalidOperationException("A parameter cannot be both required and have a default value.");

            this.value = value;
            this.IsSet = false;
        }

        public void SetParser(ParameterTryParse<T> parser) => this.parser.Parser = parser;

        /// <summary>
        /// Gets or sets the function that is used to generate type error messages for this <see cref="Parameter{T}"/>.
        /// Type error messages, are messages where the input string could not be parsed by the appropriate <see cref="TryParse{T}"/> method.
        /// The <see cref="string"/> parameter for the function is the string that the <see cref="Parameter{T}"/> could not parse.
        /// </summary>
        /// <value>
        /// The function that generates type error messages for this <see cref="Parameter{T}"/>.
        /// </value>
        public Func<string, Message> TypeErrorMessage
        {
            get { return parser.TypeErrorMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                parser.TypeErrorMessage = value;
                parser.UseParserMessage = false;
            }
        }
        /// <summary>
        /// Gets or sets the <see cref="Message"/> returned when no value is provided for this <see cref="Parameter{T}"/>.
        /// This <see cref="Message"/> is not applied when <typeparamref name="T"/> is an array.
        /// </summary>
        /// <value>
        /// The no value message.
        /// </value>
        public Message NoValueMessage
        {
            get { return parser.NoValueMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!value.IsError)
                    throw new ArgumentException("An error message cannot be the NoError message.", "value");

                parser.NoValueMessage = value;
            }
        }
        /// <summary>
        /// Gets or sets the <see cref="Message"/> returned when multiple values are provided for this <see cref="Parameter{T}"/>.
        /// This <see cref="Message"/> is not applied when <typeparamref name="T"/> is an array.
        /// </summary>
        /// <value>
        /// The multiple values message.
        /// </value>
        public Message MultipleValuesMessage
        {
            get { return parser.MultipleValuesMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!value.IsError)
                    throw new ArgumentException("An error message cannot be the NoError message.", "value");

                parser.MultipleValuesMessage = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="CommandLineParsing.Validator{T}"/> that handles all validation for this <see cref="Parameter{T}"/>.
        /// </summary>
        public Validator<T> Validator
        {
            get { return validator; }
        }

        internal override Message Handle(string[] values)
        {
            T temp;

            Message msg = parser.Parse(values, out temp);

            if (msg.IsError)
                return msg;

            msg = validator.Validate(temp);
            if (msg.IsError)
                return msg;

            IsSet = true;
            value = temp;
            doCallback();

            return Message.NoError;
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this <see cref="Parameter{T}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}[{1}] = {2}{3}",
                Name,
                typeof(T).Name,
                Object.ReferenceEquals(value, null) ? "<null>" : value.ToString(),
                IsSet ? "" : " (default)");
        }
    }
}
