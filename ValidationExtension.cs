using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public static class ValidationExtension
    {
        public static void Validate<T>(this Parameter<T> parameter, Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            parameter.Validate(x => validator(x) ? Message.NoError : errorMessage(x));
        }
        public static void Validate<T>(this Parameter<T> parameter, Func<T, bool> validator, Message errorMessage)
        {
            parameter.Validate(x => validator(x) ? Message.NoError : errorMessage);
        }

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
        public static void ValidateEach<T>(this Parameter<T[]> parameter, Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            parameter.ValidateEach(x => validator(x) ? Message.NoError : errorMessage(x));
        }
        public static void ValidateEach<T>(this Parameter<T[]> parameter, Func<T, bool> validator, Message errorMessage)
        {
            parameter.ValidateEach(x => validator(x) ? Message.NoError : errorMessage);
        }

        public static void ValidateRegex(this Parameter<string> parameter, string regex, Func<string, Message> errorMessage)
        {
            parameter.Validate(x => System.Text.RegularExpressions.Regex.IsMatch(x, regex) ? Message.NoError : errorMessage(x));
        }
        public static void ValidateRegex(this Parameter<string> parameter, string regex, Message errorMessage)
        {
            parameter.Validate(x => System.Text.RegularExpressions.Regex.IsMatch(x, regex) ? Message.NoError : errorMessage);
        }
        public static void ValidateRegex(this Parameter<string> parameter, string regex)
        {
            new System.Text.RegularExpressions.Regex(regex);
            parameter.Validate(x => System.Text.RegularExpressions.Regex.IsMatch(x, regex) ? Message.NoError : "The \"" + parameter.Name + "\" parameter must match the regex: [[:Cyan:" + regex + "]]");
        }
    }
}
