using CommandLineParsing.Output;
using CommandLineParsing.Parsing;
using System;
using System.Diagnostics;
using System.Reflection;

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
        private T value;
        private ParameterTryParse<T> _parserCustom;
        private readonly ParserSettings _parserSettings;
        private readonly Validator<T> validator;

        private Message defaultTypeError(string input)
        {
            if (Name == null)
                return new Message($@"The ""{input}"" argument could not be parsed to a value of type {typeof(T).Name}.");
            else
                return new Message($@"The ""{input}"" argument for the parameter ""{Name}"", could not be parsed to a value of type {typeof(T).Name}.");
        }

        internal Parameter(string name, string[] alternatives, string description, RequirementType? requirementType, Message required, bool enumIgnore)
            : base(name, alternatives, description, requirementType, required)
        {
            this.value = default(T);
            if (typeof(T).IsArray)
            {
                Array arr = (Array)Activator.CreateInstance(typeof(T), new object[] { 0 });
                this.value = (T)(object)arr;
            }

            _parserCustom = null;
            _parserSettings = new ParserSettings
            {
                EnumIgnoreCase = enumIgnore,
                NoValueMessage = new Message($"No value provided for argument \"{name}\"."),
                MultipleValuesMessage = new Message($"Only one value can be provided for argument \"{name}\"."),
                TypeErrorMessage = defaultTypeError,
                UseParserMessage = true
            };

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
            get
            {
                if (!IsSet && RequirementType == CommandLineParsing.RequirementType.PromptWhenUsed)
                    Prompt(RequiredMessage.Content);

                return value;
            }
            set { this.value = value; }
        }
        /// <summary>
        /// Prompts the user for a value for the parameter using the specified prompt message.
        /// The method only returns when the user has provided a new value that can be validated using <see cref="Validator"/>.
        /// Existing value (if any) will be overwritten.
        /// </summary>
        /// <param name="promptMessage">The prompt message.</param>
        public override void Prompt(ConsoleString promptMessage)
        {
            T temp = default(T);
            if (typeof(T).GetTypeInfo().IsEnum)
            {
                Consoles.System.Write(promptMessage);
                var left = Consoles.System.CursorLeft;
                Consoles.System.WriteLine();

                temp = Consoles.System.MenuSelectEnum<T>(cleanup: MenuCleanup.RemoveMenu);

                Consoles.System.SetCursorPosition(left, Consoles.System.CursorTop - 1);
                Consoles.System.RenderLine(temp?.ToString() ?? string.Empty);
            }
            else
                throw new NotSupportedException("Prompt is not supported in this intermediate state");
                //temp = Consoles.System.ReadLine<T>(_parserCustom, _parserSettings, promptMessage, validator: validator);

            IsSet = true;
            value = temp;
            doCallback();
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

        /// <summary>
        /// Sets the parser used by the <see cref="Parameter{T}"/>.
        /// </summary>
        /// <param name="parser">The new parser.</param>
        public void SetParser(ParameterTryParse<T> parser) => _parserCustom = parser;

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
            get { return _parserSettings.TypeErrorMessage; }
            set
            {
                _parserSettings.TypeErrorMessage = value ?? throw new ArgumentNullException("value");
                _parserSettings.UseParserMessage = false;
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
            get { return _parserSettings.NoValueMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!value.IsError)
                    throw new ArgumentException("An error message cannot be the NoError message.", "value");

                _parserSettings.NoValueMessage = value;
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
            get { return _parserSettings.MultipleValuesMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!value.IsError)
                    throw new ArgumentException("An error message cannot be the NoError message.", "value");

                _parserSettings.MultipleValuesMessage = value;
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

            Message msg;
            if (_parserCustom != null)
                msg = _parserCustom(values, out temp);
            else
            {
                var parser = new ReflectedParser<T>(new ReflectedParserSettings
                (
                    enumIgnoreCase: _parserSettings.EnumIgnoreCase,
                    noValueMessage: _parserSettings.NoValueMessage,
                    multipleValuesMessage: _parserSettings.MultipleValuesMessage,
                    typeErrorMessage: _parserSettings.TypeErrorMessage,
                    useParserMessage: _parserSettings.UseParserMessage
                ));

                var result = parser.Parse(values);

                if(result.IsError)
                {
                    msg = new Message(result.Content);
                    temp = default;
                }
                else
                {
                    msg = Message.NoError;
                    temp = result.Value;
                }
            }

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
