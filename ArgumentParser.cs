using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class ArgumentParser<T>
    {
        private Func<string, Message> typeValidator;
        private List<Func<T,Message>> validator;

        private Action<T> callback;
        private T defaultValue;

        internal ArgumentParser()
        {
            this.typeValidator = null;
            this.validator = new List<Func<T, Message>>();

            this.callback = null;
            this.defaultValue = default(T);
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
            this.defaultValue = value;

            return this;
        }

        public ArgumentParser<T> ValidateType()
        {
            return ValidateType(x => ("Argument " + x + "could not be parsed to a value of type " + typeof(T).Name + "."));
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
            return Validate(x => validator(x) ? errorMessage : Message.NoError);
        }
    }
}
