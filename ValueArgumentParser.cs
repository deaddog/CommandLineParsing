using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    public abstract class ValueArgumentParser<TValue, TParser> : ArgumentParser where TParser : ArgumentParser
    {
        private string description;

        private bool typeValidatorSet;
        private Func<string, Message> typeValidator;
        private List<Func<TValue, Message>> validator;

        private Message required;

        private Action<TValue> callback;
        private bool defaultSet;
        private TValue defaultValue;

        internal Message doTypeValidation(string value)
        {
            return typeValidator(value);
        }
        internal Message doValidationAndCallback(TValue value)
        {
            for (int i = 0; i < validator.Count; i++)
            {
                var msg = validator[i](value);
                if (msg.IsError)
                    return msg;
            }

            if (callback != null)
                callback(value);

            return Message.NoError;
        }

        internal ValueArgumentParser(string name)
            : base(name)
        {
            this.description = null;

            this.typeValidatorSet = false;
            this.typeValidator = x => ("Argument \"" + Name + "\" with value \"" + x + "\" could not be parsed to a value of type " + typeof(TValue).Name + ".");
            this.validator = new List<Func<TValue, Message>>();

            this.required = Message.NoError;

            this.callback = null;
            this.defaultSet = false;
            this.defaultValue = default(TValue);
        }

        internal override string Description
        {
            get { return description; }
        }

        public TParser WithDescription(string description)
        {
            this.description = description;
            return this as TParser;
        }

        public TParser Callback(Action<TValue> callback)
        {
            if (this.callback == null)
                this.callback = callback;
            else
                this.callback += callback;

            return this as TParser;
        }

        public TParser DefaultValue(TValue value)
        {
            if (required.IsError)
                throw new InvalidOperationException("An argument cannot be required and have a default value at the same time.");

            this.defaultSet = true;
            this.defaultValue = value;

            return this as TParser;
        }

        public TParser ValidateType(Message errorMessage)
        {
            return ValidateType(x => errorMessage);
        }
        public TParser ValidateType(Func<string, Message> errorMessage)
        {
            if (this.typeValidator != null && typeValidatorSet)
                throw new InvalidOperationException("Type validation can only be applied to " + this.GetType().Name + " once.");

            this.typeValidatorSet = true;
            this.typeValidator = errorMessage;
            return this as TParser;
        }

        public TParser Validate(Func<TValue, Message> validator)
        {
            this.validator.Add(validator);

            return this as TParser;
        }
        public TParser Validate(Func<TValue, bool> validator, Func<TValue, Message> errorMessage)
        {
            return Validate(x => validator(x) ? Message.NoError : errorMessage(x));
        }
        public TParser Validate(Func<TValue, bool> validator, Message errorMessage)
        {
            return Validate(x => validator(x) ? Message.NoError : errorMessage);
        }

        internal override bool IsRequired
        {
            get { return !required.IsError; }
        }
        internal override Message RequiredMessage
        {
            get { return required; }
        }

        public TParser Required()
        {
            return Required("You must specify the \"" + Name + "\" argument to execute this command.");
        }
        public TParser Required(Message errorMessage)
        {
            if (defaultSet)
                throw new InvalidOperationException("An argument cannot be required and have a default value at the same time.");

            this.required = errorMessage;

            return this as TParser;
        }
    }
}
