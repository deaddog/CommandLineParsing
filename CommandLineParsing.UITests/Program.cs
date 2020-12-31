using System;
using System.Collections.Immutable;
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

            // Do stuff using the console, by interacting with ColorConsole.

            var a1 = 
        }
    }

    public interface ICommand
    {
        Task ExecuteAsync(string[] args);
    }

    public class COMMAND : ICommand
    {
    }

    public class CMD<TParams>
    {
        public TParams Parameters { get; }
    }


    public class C<TArgs>
    {
        public Task<Message> ExecuteAsync(TArgs arguments)
        {

        }
    }




    public class Parameters
    {
        public IParameter<int> Number { get; }
        public IParameter<string> Name { get; }
    }

    public class Arguments
    {
        public IArgument<int> Number { get; }
        public IArgument<string> Name { get; }
    }

    public interface IParameter<T>
    {
        IArgument<T> Resolve(string[] args);
    }


    public interface IArgument<T>
    {
    }
}
