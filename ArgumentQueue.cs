using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    internal class ArgumentQueue
    {
        private int index;
        private List<string> args;
        private List<string> accepted;
        
        public ArgumentQueue(IEnumerable<string> args)
        {
            this.index = 0;
            this.args = new List<string>(args);
            this.accepted = new List<string>();
        }

        public int Count
        {
            get { return args.Count - index; }
        }

        public void Prepend(IEnumerable<string> args)
        {
            this.args.InsertRange(index, args);
        }

        public string Peek
        {
            get { return index < args.Count ? args[index] : null; }
        }
        public string Dequeue()
        {
            if (index >= args.Count)
                throw new InvalidOperationException($"Nothing can be taken as the of the {nameof(ArgumentQueue)} is empty.");

            string take = args[index];
            args.RemoveAt(index);
            return take;
        }

        public void Accept()
        {
            if (index >= args.Count)
                throw new InvalidOperationException($"Nothing can be accepted as the of the {nameof(ArgumentQueue)} is empty.");

            accepted.Add(args[index]);
            args.RemoveAt(index);
        }
        public void Skip()
        {
            if (index >= args.Count)
                throw new InvalidOperationException($"Nothing can be skipped as the of the {nameof(ArgumentQueue)} is empty.");

            index++;
        }

        public string[] PopAccepted()
        {
            string[] pop = accepted.ToArray();
            accepted.Clear();
            return pop;
        }
        public string[] PopSkipped()
        {
            var pop = args.Take(index).ToArray();
            index = 0;
            return pop;
        }
    }
}
