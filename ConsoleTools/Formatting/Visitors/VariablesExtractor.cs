using ConsoleTools.Formatting.Structure;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleTools.Formatting.Visitors
{
    public class VariablesExtractor : FormatVisitor<IEnumerable<VariableFormat>>
    {
        public static VariablesExtractor Instance { get; } = new VariablesExtractor();

        public override IEnumerable<VariableFormat> Visit(VariableFormat format) => new[] { format };
        public override IEnumerable<VariableFormat> Visit(ConcatenationFormat format) => format.Elements.SelectMany(Visit);

        public override IEnumerable<VariableFormat> Visit(ColorFormat format) => Enumerable.Empty<VariableFormat>();
        public override IEnumerable<VariableFormat> Visit(ConditionFormat format) => Enumerable.Empty<VariableFormat>();
        public override IEnumerable<VariableFormat> Visit(FunctionFormat format) => Enumerable.Empty<VariableFormat>();
        public override IEnumerable<VariableFormat> Visit(NoContentFormat format) => Enumerable.Empty<VariableFormat>();
        public override IEnumerable<VariableFormat> Visit(TextFormat format) => Enumerable.Empty<VariableFormat>();
    }
}
