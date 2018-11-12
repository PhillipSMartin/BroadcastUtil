using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BroadcastLib;
using log4net;

namespace SampleSender
{
    class Program
    {
        static ILog s_logger;

        static void Main(string[] args)
        {
            // Log4net is optional - you can pass null to the Init method instead of an ILog object

            log4net.Config.XmlConfigurator.Configure();
            s_logger = new Logger( LogManager.GetLogger(typeof(Program)) );

            // instantiate sender and initialize with port number and an ILog object (or null)
            Sender sender = new Sender();
            sender.DebugMode = true;  // to log activity - if debugmode is false, will log only errors

            sender.Init(15000, s_logger);

            // can set throttle time in ms if necessary - defaults to 20
            // sender.ThrottleTime = ??;

            // ask user for message to send
            Console.WriteLine("Enter (1) a message to send (start with '0x' to send a hex message) or (2) an integer, indicating how many canned messages to send");
            string message = Console.ReadLine();
            int numberOfMessagesToSend;
            while (!String.IsNullOrEmpty(message))
            {
                if (int.TryParse(message, out numberOfMessagesToSend))
                {
                    int i = 1;
                    while (i <= numberOfMessagesToSend)
                    {
                        sender.Send(String.Format("Message number {0}", i++));
                    }
                }
                else
                {
                    if (message.StartsWith("0x"))
                    {
                        byte[] datagram = Utilities.HexStringToDatagram(message);
                        sender.Send(datagram);
                    }
                    else
                    {
                        sender.Send(message);
                    }
                }
                message = Console.ReadLine();
            }

            // close the sender
            sender.Close();
        }

     }
}
