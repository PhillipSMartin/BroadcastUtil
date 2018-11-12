using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BroadcastLib
{
    public class ClosedEventArgs : EventArgs
    {
        private readonly Exception e;

        public ClosedEventArgs(Exception e)
        {
            this.e = e;
        }

        public Exception Exception
        {
            get { return e; }
        }
        public bool IsError
        {
            get { return e != null; }
        }
    }
}
