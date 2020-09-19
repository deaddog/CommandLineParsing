using CommandLineParsing.Tests.TestComponents;
using NUnit.Framework;

namespace CommandLineParsing.Tests
{
    public abstract class ConsoleTestBase
    {
        [SetUp]
        public void CreateConsole()
        {
            Console = new TestingConsole();
            AssertConsole = new ConsoleAssertions(Console);
        }

        public TestingConsole Console { get; private set; }
        public ConsoleAssertions AssertConsole { get; private set; }
    }
}
