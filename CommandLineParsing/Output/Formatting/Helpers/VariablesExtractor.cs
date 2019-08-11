using CommandLineParsing.Output.Formatting.Structure;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing.Output.Formatting.Helpers
{
    internal class VariablesExtractor : FormatVisitor<IEnumerable<FormatVariableElement>>
    {
        public override IEnumerable<FormatVariableElement> Visit(FormatVariableElement format)
        {
            yield return format;
        }

        public override IEnumerable<FormatVariableElement> Visit(FormatConcatenationElement format)
        {
            return format.Elements.SelectMany(Visit);
        }

        public override IEnumerable<FormatVariableElement> Visit(FormatColorElement format) => new FormatVariableElement[0];
        public override IEnumerable<FormatVariableElement> Visit(FormatConditionElement format) => new FormatVariableElement[0];
        public override IEnumerable<FormatVariableElement> Visit(FormatFunctionElement format) => new FormatVariableElement[0];
        public override IEnumerable<FormatVariableElement> Visit(FormatNoContentElement format) => new FormatVariableElement[0];
        public override IEnumerable<FormatVariableElement> Visit(FormatTextElement format) => new FormatVariableElement[0];
    }
}
