using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public abstract class ArgumentParser<T>
    {
        private string name;

        private Func<string, Message> typeValidator;
        private List<Func<T, Message>> validator;

        private Message required;

        private Action<T> callback;
        private bool defaultSet;
        private T defaultValue;

        internal Message doTypeValidation(string value)
        {
            return typeValidator(value);
        }
        internal Message doValidationAndCallback(T value)
        {
            for (int i = 0; i < validator.Count; i++)
            {
                var msg = validator[i](value);
                if (msg != Message.NoError)
                    return msg;
            }

            callback(value);

            return Message.NoError;
        }

        internal ArgumentParser(string name)
        {
            this.name = name;

            this.typeValidator = null;
            this.validator = new List<Func<T, Message>>();

            this.required = Message.NoError;

            this.callback = null;
            this.defaultSet = false;
            this.defaultValue = default(T);
        }

        internal abstract Message Handle(Argument argument);

        internal bool IsRequired
        {
            get { return required != Message.NoError; }
        }
        internal Message RequiredMessage
        {
            get { return required; }
        }

        public ArgumentParser<T> Callback(Action<T> callback)
        {
            if (this.callback == null)
                this.callback = callback;
            else
                this.callback += callback;

            return this;
        }

        public ArgumentParser<T> DefaultValue(T value)
        {
            if (required != Message.NoError)
                throw new InvalidOperationException("An argument cannot be required and have a default value at the same time.");

            this.defaultSet = true;
            this.defaultValue = value;

            return this;
        }

        public ArgumentParser<T> ValidateType()
        {
            return ValidateType(x => ("Argument \"" + name + "\" with value \"" + x + "\" could not be parsed to a value of type " + typeof(T).Name + "."));
        }
        public ArgumentParser<T> ValidateType(Message errorMessage)
        {
            return ValidateType(x => errorMessage);
        }
        public ArgumentParser<T> ValidateType(Func<string, Message> errorMessage)
        {
            if (this.typeValidator != null)
                throw new InvalidOperationException("Type validation can only be applied to " + this.GetType().Name + " once.");

            this.typeValidator = errorMessage;
            return this;
        }

        public ArgumentParser<T> Validate(Func<T, Message> validator)
        {
            this.validator.Add(validator);

            return this;
        }
        public ArgumentParser<T> Validate(Func<T, bool> validator, Message errorMessage)
        {
            return Validate(x => validator(x) ? Message.NoError : errorMessage);
        }

        public ArgumentParser<T> Required()
        {
            return Required("You must specify the \"" + name + "\" argument to execute this command.");
        }
        public ArgumentParser<T> Required(Message errorMessage)
        {
            if (defaultSet)
                throw new InvalidOperationException("An argument cannot be required and have a default value at the same time.");

            this.required = errorMessage;

            return this;
        }
    }
}
