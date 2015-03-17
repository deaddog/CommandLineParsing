using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class ArgumentParser<T>
    {
        private Action<T> callback;

        internal ArgumentParser()
        {
            this.callback = null;
        }

        public ArgumentParser<T> Callback(Action<T> callback)
        {
            if (this.callback == null)
                this.callback = callback;
            else
                this.callback += callback;

            return this;
        }
    }
}
