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
    }
}
