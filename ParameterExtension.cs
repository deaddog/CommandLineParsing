using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides additional methods for the <see cref="Parameter"/> class.
    /// </summary>
    public static class ParameterExtension
    {
        /// <summary>
        /// Sets the parser used by the <see cref="Parameter{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the values managed by the parameter.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parser">The new parser.</param>
        public static void SetParser<T>(this Parameter<T[]> parameter, TryParse<T> parser)
        {
            if (parser == null)
                parameter.SetParser((ParameterTryParse<T[]>)null);
            else
                parameter.SetParser(new TryParseWrap<T>(parameter, parser).Parse);
        }

        public static void SetParser<T>(this Parameter<T[]> parameter, MessageTryParse<T> parser)
        {
            if (parser == null)
                parameter.SetParser((ParameterTryParse<T[]>)null);
            else
                parameter.SetParser(new MessageTryParseWrap<T>(parser).Parse);
        }

        internal class TryParseWrap<T>
        {
            private Parameter<T[]> parameter;
            private TryParse<T> parser;

            public TryParseWrap(Parameter<T[]> parameter, TryParse<T> parser)
            {
                this.parameter = parameter;
                this.parser = parser;
            }

            public Message Parse(string[] args, out T[] result)
            {
                T[] temp = new T[args.Length];
                result = default(T[]);

                for (int i = 0; i < args.Length; i++)
                {
                    if (!parser(args[i], out temp[i]))
                        return parameter.TypeErrorMessage(args[i]);
                }

                result = temp;
                return Message.NoError;
            }
        }

        internal class MessageTryParseWrap<T>
        {
            private MessageTryParse<T> parser;

            public MessageTryParseWrap(MessageTryParse<T> parser)
            {
                this.parser = parser;
            }

            public Message Parse(string[] args, out T[] result)
            {
                T[] temp = new T[args.Length];
                result = default(T[]);

                for (int i = 0; i < args.Length; i++)
                {
                    Message msg = parser(args[i], out temp[i]);
                    if (msg.IsError)
                        return msg;
                }

                result = temp;
                return Message.NoError;
            }
        }
    }
}
