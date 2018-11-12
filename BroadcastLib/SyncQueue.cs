using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;

namespace BroadcastLib
{
    public class SyncQueue<T> : Queue<T>, IDisposable
    {
        private EventWaitHandle m_go = new AutoResetEvent(false);

        private object m_lock = new object();
        private int m_count = 0;
        private bool m_stop;
        private Receiver<T> m_parent;
        private bool isDisposed;

        public SyncQueue(Receiver<T> parent) { m_parent = parent; }

        public new void Enqueue(T item)
        {
            if (Stop)
            {
                m_parent.Debug("Ignoring enqueue after stop");
            }
            else
            {
                int count;
                lock (m_lock)
                {
                    count = Interlocked.Increment(ref m_count);
                    Debug("Enqueing item", count, item);
                    base.Enqueue(item);
                }

                if (count == 1)
                {
                    m_parent.Debug("Firing wait handle");
                    m_go.Set();
                }
            }
        }

        public new T Dequeue()
        {
           T item = default(T);
           int count;

           if (Stop)
           {
               m_parent.Debug("Dequeueing null item after stop");
               return item;
           }

           while (Interlocked.Equals(m_count, 0))
           {
               m_parent.Debug("Queue empty - entering wait state");
               m_go.WaitOne();
               if (Stop)
               {
                   m_parent.Debug("Exited wait state - dequeueing null item after stop");
                   return item;
               }
           }

           lock (m_lock)
           {
                count = Interlocked.Decrement(ref m_count);
                item = base.Dequeue();
           }

           Debug("Dequeued item", count, item);
           return item;
        }

        public bool Stop 
        {
            get { return m_stop; }
            set 
            {
                m_stop = true;
                m_parent.Debug("Queue stopped - firing wait handle");
                m_go.Set();
            }
        }

        private void Debug(string message, int count, T item)
        {
            if (m_parent.DebugMode)
            {
                m_parent.Debug(String.Format("{0}, count={1}, message={2}", message, count, Utilities.TToString(item)));
            }
        }
  
        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.isDisposed)
            {
                isDisposed = true;
                if (disposing)
                {
                    if (m_go != null)
                    {
                        m_go.Dispose();
                    }
                }
            }
        }

        ~SyncQueue()      
        {
            Dispose(false);
        }
        #endregion
    }
}
