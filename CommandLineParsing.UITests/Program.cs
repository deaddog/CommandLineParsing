using CommandLineParsing.Input.Reading;
using CommandLineParsing.Output.Formatting;
using CommandLineParsing.Output.Formatting.Structure;
using CommandLineParsing.Parsing;
using CommandLineParsing.Validation;
using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLineParsing.UITests
{
    class Program
    {
        static readonly SharedConsole _console = new SharedConsole();

        static void Main(string[] args)
        {
            // Setting the active console to the shared one, use _console.BufferStrings and _console.WindowStrings to inspect the state.
            IConsole c = Consoles.System;

            CommandLineParsing.Applications.Testing.MainTest(args);
        }

    }
}
