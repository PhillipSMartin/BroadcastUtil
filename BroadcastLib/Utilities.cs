using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BroadcastLib
{
    public class Utilities
    {
        public static string ByteArrayToString(byte[] datagram)
        {
            StringBuilder hex = new StringBuilder(datagram.Length * 2);
            foreach (byte b in datagram)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static string TToString<T>(T message)
        {
            if (message.GetType() == typeof(byte[]))
            {
                return ByteArrayToString(message as byte[]);
            }
            else
            {
                return message.ToString();
            }
        }
        public static T DatagramToT<T>(byte[] datagram)
        {
            object obj;
            if (typeof(T) == typeof(string))
            {
                obj = Encoding.UTF8.GetString(datagram);
            }
            else
            {
                obj = datagram;
            }
            return (T)obj;
        }

        // converts a string of hex characters to hex - e.g.  "0xAB0F" is converted to 0xAB0F
        public static byte[] HexStringToDatagram(string hex)
        {
            if (hex.StartsWith("0x"))
            {
                hex = hex.Substring(2);
                byte[] raw = new byte[hex.Length / 2];
                for (int i = 0; i < raw.Length; i++)
                {
                    raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }
                return raw;
            }
            else return null;
        }
    }
}
