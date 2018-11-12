using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using log4net;
using System.Threading;
using System.ComponentModel;

namespace BroadcastLib
{
    public class Receiver<T> : BroadcastBase, IDisposable
    {
        private SyncQueue<T> m_queue;
        private BackgroundWorker m_dequeueThread = new BackgroundWorker();
        private BackgroundWorker m_listenerThread = new BackgroundWorker();

        // Init method
        //      called once - must be called before Receive
        // Parameters
        //      port - the port number to listen on
        //      logger - a log4net ILog interface for error or debug logging (can be null if no logging is desired)
        //
        public void Init(int port, ILog logger = null)
        {
  
            try
            {
                base.Init(logger);
                m_udpClient = new System.Net.Sockets.UdpClient(port);
                m_endPoint = new IPEndPoint(IPAddress.Any, port);
                m_queue = new SyncQueue<T>(this);
                Debug("Receiver initialized");
            }
            catch (Exception e)
            {
                Error("Error initializing broadcast receiver", e);
            }

        }

        // Receive method
        //      must have previously called Init
        //      Begins an asynchronous listening task that terminates when we call Close
        //
        // Parameters
        //      onMessage - a delegate to receive messages
        //
        public int Receive(MessageDelegate<T> onMessage)
        {
            int rc = 8;

            try
            {
                if (!isOpen)
                    throw new System.ApplicationException("Broadcast receiver not initialized");
                if (onMessage == null)
                    throw new ArgumentNullException("onMessage");


                m_dequeueThread.DoWork += DequeueThread;
                m_dequeueThread.RunWorkerAsync(onMessage);

                m_listenerThread.DoWork += ListenerThread;
                m_listenerThread.RunWorkerAsync();

                Debug("Asynchronous string listener started");
            }
            catch (Exception e)
            {
                Error("Error receiving broadcast message", e);
            }

            return rc;
        }

        void DequeueThread(object sender, DoWorkEventArgs e)
        {
            try
            {
                Debug("Dequeue thread started");

                MessageDelegate<T> onMessage = (MessageDelegate<T>)e.Argument;
                while (isOpen)
                {
                    T message = m_queue.Dequeue();
                    if (isOpen)
                        onMessage(message);
                }
            }
            catch (Exception ex)
            {
                Error("Exception in dequeue thread", ex);
                Close(ex);
            }
            finally
            {
                Debug("Dequeue thread ended");
            }
        }

        void ListenerThread(object sender, DoWorkEventArgs e)
        {
            try
            {
                Debug("Listener thread started");

                while (isOpen)
                {
                    T message = Utilities.DatagramToT<T>(m_udpClient.Receive(ref m_endPoint));

                    if (DebugMode)
                        Debug("Broadcast received: " + Utilities.TToString(message));
                    m_queue.Enqueue(message);
                 }
            }
            catch (Exception ex)
            {
                try
                {
                    if (isOpen)
                    {
                        if (m_logger != null)
                        {
                            m_logger.Error("Exception in listener thread", ex);
                        }

                        Close(ex);
                    }
                    else
                    {
                        if (DebugMode)
                            Debug("Receive interrupted: " + ex.Message);
                    }
                }
                finally
                {
                    Debug("Listener thread ended");
                }
            }
        }

        // Close method
        //      releases resources and fires ReceiverClosed event
        //
        public override void Close()
        {
            Close(null);
        }
        private void Close(Exception exception)
        {
            try
            {
                if (isOpen)
                {
                    isOpen = false;
                    m_queue.Stop = true;
                    Thread.Sleep(1000);

                    if (m_udpClient != null)
                    {
                        m_udpClient.Close();
                        Debug("Receiver closed");
                    }
                }
            }
            catch (Exception e)
            {
                Error("Error closing broadcast receiver", e);
            }
            finally
            {
                OnClosed(new ClosedEventArgs(exception));
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
                    if (m_udpClient != null)
                    {
                        ((IDisposable)m_udpClient).Dispose();
                    }
                    if (m_queue != null)
                    {
                        m_queue.Dispose();
                    }
                    if (m_dequeueThread != null)
                    {
                        m_dequeueThread.Dispose();
                    }
                    if (m_listenerThread != null)
                    {
                        m_listenerThread.Dispose();
                    }
                }
            }
        }

        ~Receiver()
        {
            Dispose(false);
        }
        #endregion
    }
}
