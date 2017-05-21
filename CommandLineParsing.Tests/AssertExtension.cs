using NUnit.Framework;
using System.Linq;

namespace CommandLineParsing.Tests
{
    public static class AssertExtension
    {
        public static void AssertLine(this TestingConsoleString[] state, int lineIndex, string text, int startIndex = 0)
        {
            var str = state.FirstOrDefault(x => x.Position.Top == lineIndex);

            if (str == null)
                Assert.Fail($"The console did not contain line index {lineIndex}.");
            else
            {
                var diff = text.Length - text.TrimStart(' ').Length;
                if (diff != 0)
                {
                    text = text.Substring(diff);
                    startIndex += diff;
                }

                Assert.AreEqual(startIndex, str.Position.Left);
                Assert.AreEqual(text, str.Text);
            }
        }
    }
}
