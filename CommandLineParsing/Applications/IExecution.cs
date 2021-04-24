using System.Threading;
using System.Threading.Tasks;

namespace CommandLineParsing.Applications
{
    public interface IExecution
    {
        Task ExecuteAsync(ArgumentSet args, CancellationToken cancellationToken);
    }
}
