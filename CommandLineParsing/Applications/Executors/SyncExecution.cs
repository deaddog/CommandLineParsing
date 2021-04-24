using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLineParsing.Applications.Executors
{
    public class SyncExecution : IExecution
    {
        private readonly Action<ArgumentSet> _execution;

        public SyncExecution(Action<ArgumentSet> execution)
        {
            _execution = execution ?? throw new ArgumentNullException(nameof(execution));
        }

        public Task ExecuteAsync(ArgumentSet args, CancellationToken cancellationToken)
        {
            return Task.Run(() => _execution(args), cancellationToken);
        }
    }
}
