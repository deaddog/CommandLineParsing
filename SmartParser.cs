namespace CommandLineParsing
{
    internal class SmartParser<T>
    {
        internal override Message HandleSingle(string[] values)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>(enumIgnore);

            T temp;

            if (values.Length == 0)
                return noValueMessage;
            else if (values.Length > 1)
                return multipleValuesMessage;
            else if (!parser(values[0], out temp))
                return typeErrorMessage(values[0]);

            var msg = validator.Validate(temp);
            if (msg.IsError)
                return msg;

            IsSet = true;
            value = temp;
            doCallback();

            return Message.NoError;
        }
        internal override Message HandleArray(string[] values)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>(enumIgnore);

            T[] temp = new T[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                if (!parser(values[i], out temp[i]))
                    return TypeErrorMessage(values[i]);
            }

            var msg = validator.Validate(temp);
            if (msg.IsError)
                return msg;

            IsSet = true;
            value = temp;
            doCallback();

            return Message.NoError;
        }
    }
}
