using System;

namespace CommandLineParsing
{
    public class Formatter : IFormatter
    {
        public string GetVariable(string variable)
        {
            throw new NotImplementedException();
        }
        public string GetAutoColor(string variable)
        {
            throw new NotImplementedException();
        }
        public int? GetAlignedLength(string variable)
        {
            throw new NotImplementedException();
        }

        public bool? ValidateCondition(string condition)
        {
            throw new NotImplementedException();
        }
        public string EvaluateFunction(string function, string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
