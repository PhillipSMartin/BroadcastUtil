using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BroadcastLib;
using log4net;
using log4net.Appender;
using System.Deployment.Application;

namespace SampleReceiver
{
    class Program
    {
        static ILog s_logger;

        static void Main(string[] args)
        {
            // Log4net is optional - you can pass null to the Init method instead of an ILog object

            log4net.Config.XmlConfigurator.Configure();
            s_logger = LogManager.GetLogger(typeof(Program));
            BroadcastBase receiver;

            // display directory of log4net log files
            foreach (var appender in LogManager.GetRepository().GetAppenders())
            {
                var fileAppender = appender as FileAppender;
                if (fileAppender != null)
                {
                     Console.WriteLine(fileAppender.Name + " will be in " + fileAppender.File);
                }
            }

            Console.WriteLine("Press 'x' to accept hex messages, any other key to accept string messages");
            ConsoleKeyInfo info = Console.ReadKey();


            if (info.KeyChar != 'x')
            {
                Receiver<string> stringReceiver = new Receiver<string>();
                receiver = stringReceiver;

                // instantiate receiver and initialize with port number and an ILog object (or null)
                stringReceiver.DebugMode = true;  // to log activity - if debugmode is false, will log only errors
                stringReceiver.Closed += receiver_Closed; // to wire event handler so we will know when receiver is closed and can restart it if necessary

                stringReceiver.Init(15000, s_logger);

                // begin asynchronous listening - specify a callback 
                stringReceiver.Receive(new MessageDelegate<string>(OnStringMessage));
            }

            else
            {
                Receiver<byte[]> hexReceiver = new Receiver<byte[]>();
                receiver = hexReceiver;

                // instantiate receiver and initialize with port number and an ILog object (or null)
                hexReceiver.DebugMode = true;  // to log activity - if debugmode is false, will log only errors
                hexReceiver.Closed += receiver_Closed; // to wire event handler so we will know when receiver is closed and can restart it if necessary

                hexReceiver.Init(15000, s_logger);

                // begin asynchronous listening - specify a callback 
                hexReceiver.Receive(new MessageDelegate<byte[]>(OnHexMessage));
            }

            // allow user to terminate receiver by pressing any key
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            info = Console.ReadKey();

            // close the receiver
             receiver.Close();
             receiver.Closed -= receiver_Closed;
        }

        static void receiver_Closed(object sender, ClosedEventArgs e)
        {
            if (e.IsError)
                Console.WriteLine("Receiver has been closed due to an error: " + e.Exception.Message);
        }

        // callbacks
        static void OnStringMessage(string message)
                            {
            Console.WriteLine(message);
        }
        static void OnHexMessage(byte[] datagram)
        {
            Console.WriteLine(Utilities.ByteArrayToString(datagram));
        }
    }
}
