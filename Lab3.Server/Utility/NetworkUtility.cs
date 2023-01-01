using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Lab3.Server.Utility
{
    public static class NetworkUtility
    {
        public static string GetIP()
        {
            //IPHostEntry hostEntry = Dns.GetHostEntry(Environment.MachineName);

            //foreach (IPAddress address in hostEntry.AddressList)
            //{
            //    if (address.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        return address.ToString();
            //    }
            //}

            //return null;

            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());

            string ipadr = "192.168.0.106";

            //foreach (var address in localIp)
            //{
            //    if (address.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        ipadr = address.ToString();
            //    }
            //}

            return ipadr;
        }
    }
}
