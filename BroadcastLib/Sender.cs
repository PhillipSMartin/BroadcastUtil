using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using log4net;

namespace BroadcastLib
{
    public class Sender : BroadcastBase, IDisposable
    {
        private const int DefaultThrottle = 20;
        
        // Init method
        //      called once - must be called before Send
        // Parameters
        //      port - the port number to be broadcast on
        //      logger - a log4net ILog interface for error or debug logging (can be null if no logging is desired)
        //
        public void Init(int port, ILog logger = null)
        { 
            try
            {
                

                base.Init(logger);
                m_udpClient = new System.Net.Sockets.UdpClient();
                this.m_endPoint = new IPEndPoint(IPAddress.Broadcast, port);
                this.ThrottleTime = DefaultThrottle;
                Debug("Sender initialized");
            }
            catch (Exception e)
            {
                Error("Error initializing broadcast sender", e);
            }

        }

        // Send method
        //      must have previously called Init
        // Parameters
        //      message - a string to broadcast
        //
        public int Send(string message)
        {
            int rc = 0;

            try
            {
                if (!isOpen)
                    throw new System.ApplicationException("Broadcast sender not initialized");

                if (String.IsNullOrEmpty(message))
                    throw new ArgumentNullException("message");

                byte[] datagram = Encoding.ASCII.GetBytes(message);
                rc = m_udpClient.Send(datagram, datagram.Length, m_endPoint);
                Debug("Broadcast sent: " + message);

                Throttle();
            }
            catch (Exception e)
            {
                Error("Error sending broadcast message", e);
            }

            return rc;
        }

        // Send method
        //      must have previously called Init
        // Parameters
        //      datagram - a byte array to broadcast
        //
        public int Send(byte[] datagram)
        {
            int rc = 0;

            try
            {
                if (!isOpen)
                     throw new System.ApplicationException("Broadcast sender not initialized");
 
                if (datagram == null)
                    throw new ArgumentNullException("datagram");

                rc = m_udpClient.Send(datagram, datagram.Length, m_endPoint);
                Debug("Broadcast sent: " + Utilities.ByteArrayToString(datagram));

                Throttle();
            }
            catch (Exception e)
            {
                Error("Error sending broadcast message", e);
             }

            return rc;
        }

        // Close method
        //      releases resources
        //
        public override void Close()
        {
            try
            {
                if (isOpen)
                {
                    base.Close();
                    Debug("Sender closed");
                }
            }
            catch (Exception e)
            {
                Error("Error closing broadcast sender", e);
            }
        }

        private void Throttle()
        {
            if (ThrottleTime > 0)
            {
                Debug(String.Format("Throttling for {0} ms", ThrottleTime));
                System.Threading.Thread.Sleep(ThrottleTime);
            }
        }

        // milliseconds to throttle between sends
        public int ThrottleTime { get; set; }

        
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
                }
            }
        }

        ~Sender()      
        {
            Dispose(false);
        }
        #endregion
   }
}
