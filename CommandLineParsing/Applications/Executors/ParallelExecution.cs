using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLineParsing.Applications.Executors
{
    public class ParallelExecution : IExecution
    {
        private readonly IImmutableList<IExecution> _executions;

        public static ParallelExecution Create(params IExecution[] executions)
        {
            return new ParallelExecution
            (
                executions.Aggregate
                (
                    seed: ImmutableList<IExecution>.Empty,
                    func: (l, e) => e switch
                    {
                        ParallelExecution p => l.AddRange(p._executions),
                        _ => l.Add(e)
                    }
                )
            );
        }

        private ParallelExecution(IImmutableList<IExecution> executions)
        {
            _executions = executions ?? throw new ArgumentNullException(nameof(executions));
        }

        public Task ExecuteAsync(ArgumentSet args, CancellationToken cancellationToken)
        {
            return Task.WhenAll(_executions.Select(e => e.ExecuteAsync(args, cancellationToken)));
        }
    }
}
