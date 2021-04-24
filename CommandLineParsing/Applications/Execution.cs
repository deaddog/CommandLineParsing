using CommandLineParsing.Applications.Executors;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLineParsing.Applications
{
    public static class Execution
    {
        public static TReturn OnExecute<TReturn>(this IComposer<IExecution, TReturn> composer, Action<ArgumentSet> execution)
        {
            return composer.With(SequentialExecution.Create(composer.State, new SyncExecution(execution)));
        }
        public static TReturn OnExecute<TReturn>(this IComposer<IExecution, TReturn> composer, Func<ArgumentSet, Task> execution)
        {
            return OnExecute(composer, (a, t) => execution(a));
        }
        public static TReturn OnExecute<TReturn>(this IComposer<IExecution, TReturn> composer, Func<ArgumentSet, CancellationToken, Task> execution)
        {
            return composer.With(SequentialExecution.Create(composer.State, new AsyncExecution(execution)));
        }


        //public static IExecution From(Action<ArgumentSet> action) => new SyncExecution(action);
        //public static IExecution From(Func<ArgumentSet, Task> )
        //public static Command OnExecute(this Command command, IExecution execution)
        //{
        //    return new Command
        //    (
        //        parameters: command.Parameters,
        //        execution: SequentialExecution.Create(command.Execution, execution)
        //    );
        //}
        //public static Command OnExecute(this Command command, Action<ArgumentSet> execution)
        //{
        //    return OnExecute(command, new SyncExecution(execution));
        //}
        //public static Command OnExecute(this Command command, Func<ArgumentSet, Task> execution)
        //{
        //    return OnExecute(command, new AsyncExecution((args, token) => execution(args)));
        //}
        //public static Command OnExecute(this Command command, Func<ArgumentSet, CancellationToken, Task> execution)
        //{
        //    return OnExecute(command, new AsyncExecution(execution));
        //}
    }

    public interface IComposer<T, out TReturn>
    {
        T State { get; }
        TReturn With(T state);
    }
}
