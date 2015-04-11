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
        protected bool isDefault;

        protected readonly bool enumIgnore;
        private TryParse<T> parser;

        private Func<string, Message> typeErrorMessage;
        private Message noValueMessage;
        private Message multipleValuesMessage;

        private List<Func<T, Message>> validators;

#pragma warning restore

        internal Parameter(string name, string[] alternatives, string description, Message required, bool enumIgnore)
            : base(name, alternatives, description, required)
        {
            this.value = default(T);
            this.isDefault = true;

            this.enumIgnore = enumIgnore;
            this.parser = null;

            this.typeErrorMessage = x => string.Format("Argument \"{0}\" with value \"{1}\" could not be parsed to a value of type {2}.", name, x, typeof(T).Name);
            this.noValueMessage = "No value provided for argument \"" + name + "\".";
            this.multipleValuesMessage = "Only one value can be provided for argument \"" + name + "\".";

            this.validators = new List<Func<T, Message>>();
        }

        /// <summary>
        /// Gets the value parsed by this <see cref="Parameter{T}"/>, or the default value if <see cref="IsDefault"/> is <c>true</c>.
        /// </summary>
        /// <value>
        /// The value parsed by this <see cref="Parameter{T}"/>.
        /// </value>
        public virtual T Value
        {
            get { return value; }
        }
        /// <summary>
        /// Gets a value indicating whether the <see cref="Value"/> property holds the default value.
        /// </summary>
        /// <value>
        /// <c>true</c> if this <see cref="Parameter{T}"/> is not provided for the executed command; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault
        {
            get { return isDefault; }
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
            this.isDefault = true;
        }

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
            get { return typeErrorMessage; }
            set
            {
                if (typeErrorMessage == null)
                    throw new ArgumentNullException("value");

                typeErrorMessage = value;
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
            get { return noValueMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!value.IsError)
                    throw new ArgumentException("An error message cannot be the NoError message.", "value");

                this.noValueMessage = value;
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
            get { return multipleValuesMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!value.IsError)
                    throw new ArgumentException("An error message cannot be the NoError message.", "value");

                this.multipleValuesMessage = value;
            }
        }

        /// <summary>
        /// Provides a validation method for this <see cref="Parameter{T}"/>.
        /// </summary>
        /// <param name="validator">A function that validates the parsed <typeparamref name="T"/> value and returns a <see cref="Message"/>.
        /// If the validation was successful <see cref="Message.NoError"/> should be returned by the method; otherwise an appropriate <see cref="Message"/> should be returned.</param>
        public void Validate(Func<T, Message> validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            this.validators.Add(validator);
        }

        internal override Message Handle(Argument argument)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>(enumIgnore);

            T temp;

            if (argument.Count == 0)
                return noValueMessage;
            else if (argument.Count > 1)
                return multipleValuesMessage;
            else if (!parser(argument[0], out temp))
                return typeErrorMessage(argument[0]);

            var msg = doValidation(temp);
            if (msg.IsError)
                return msg;

            isDefault = false;
            value = temp;
            doCallback();

            return Message.NoError;
        }
        internal override bool CanHandle(string value)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>(enumIgnore);

            T temp;
            return parser(value, out temp);
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
                isDefault ? " (default)" : "");
        }
    }
}
