using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    /// <summary>
    /// Defines what type of console cleanup should be applied after completing a readline.
    /// </summary>
    public enum ReadLineCleanup
    {
        /// <summary>
        /// No cleanup is applied.
        /// </summary>
        None,
        /// <summary>
        /// Only the prompt displayed with the readline is removed.
        /// </summary>
        RemovePrompt,
        /// <summary>
        /// Both the prompt and the entered value are removed.
        /// </summary>
        RemoveAll
    }
}
