using System;

namespace CommandLineParsing.UITests
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TestNameAttribute : Attribute
    {
        public string Name { get; }

        public TestNameAttribute(string name)
        {
            Name = name;
        }
    }
}
