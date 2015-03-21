using System;

namespace CommandLineParsing
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class Name : Attribute
    {
        internal readonly string[] names;

        public Name(string name, params string[] alternatives)
        {
            this.names = new string[1 + alternatives.Length];
            this.names[0] = name;
            alternatives.CopyTo(this.names, 1);
        }
    }
}
