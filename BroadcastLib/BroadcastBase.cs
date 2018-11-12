using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using log4net;

namespace BroadcastLib
{
    public class BroadcastBase
    {
        protected bool isOpen;
        protected bool isDisposed;
        protected System.Net.Sockets.UdpClient m_udpClient;
        protected System.Net.IPEndPoint m_endPoint;
        protected ILog m_logger;

        public void Init(ILog logger = null)
        {
            try
            {
                m_logger = logger;
                isOpen = true;
            }
            catch (Exception e)
            {
                Error("Error initializing broadcast object", e);
            }

        }

        public virtual void Close()
        {
            if (isOpen)
            {
                isOpen = false;
                if (m_udpClient != null)
                {
                    m_udpClient.Close();
                }
            }
        }

        public event ClosedEventHandler Closed;
        protected void OnClosed(ClosedEventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, e);
            }
        }

        // if true and if an ILog object was passed to Init, we will log debug messages
        public bool DebugMode { get; set; }

        public void Debug(string message)
        {
            if (DebugMode && (m_logger != null))
                m_logger.Debug(message);

        }
        public void Error(string message, Exception e)
        {
            if (m_logger != null)
                m_logger.Error(message, e);
            else
                throw e;
        }
    }
}
