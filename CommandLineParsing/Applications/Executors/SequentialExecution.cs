using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLineParsing.Applications.Executors
{
    public class SequentialExecution : IExecution
    {
        private readonly IImmutableList<IExecution> _executions;

        public static SequentialExecution Create(params IExecution[] executions)
        {
            return new SequentialExecution
            (
                executions.Aggregate
                (
                    seed: ImmutableList<IExecution>.Empty,
                    func: (l, e) => e switch
                    {
                        SequentialExecution s => l.AddRange(s._executions),
                        _ => l.Add(e)
                    }
                )
            );
        }

        private SequentialExecution(IImmutableList<IExecution> executions)
        {
            _executions = executions ?? throw new ArgumentNullException(nameof(executions));
        }

        public async Task ExecuteAsync(ArgumentSet args, CancellationToken cancellationToken)
        {
            foreach (var e in _executions)
                await e.ExecuteAsync(args, cancellationToken);
        }
    }
}
