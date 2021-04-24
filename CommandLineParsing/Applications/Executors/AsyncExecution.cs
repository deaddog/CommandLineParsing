using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLineParsing.Applications.Executors
{
    public class AsyncExecution : IExecution
    {
        private readonly Func<ArgumentSet, CancellationToken, Task> _execution;

        public AsyncExecution(Func<ArgumentSet, CancellationToken, Task> execution)
        {
            _execution = execution ?? throw new ArgumentNullException(nameof(execution));
        }

        public Task ExecuteAsync(ArgumentSet args, CancellationToken cancellationToken)
        {
            return _execution(args, cancellationToken);
        }
    }
}
