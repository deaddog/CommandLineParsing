using System;
using System.Text;

namespace CommandLineParsing.Internals
{
    internal class ReadLineHelper
    {
        private readonly int position;
        private readonly StringBuilder sb;

        public ReadLineHelper()
        {
            position = Console.CursorLeft;
            sb = new StringBuilder();
        }
    }
}
