using CommandLineParsing.Tests.TestComponents;
using NUnit.Framework;

namespace CommandLineParsing.Tests
{
    public abstract class ConsoleTestBase
    {
        [SetUp]
        public void CreateConsole()
        {
            var console = new TestingConsole();

            Console = console;
            AssertConsole = new ConsoleAssertions(console);
        }

        public IConsole Console { get; private set; }
        public ConsoleAssertions AssertConsole { get; private set; }
    }
}
