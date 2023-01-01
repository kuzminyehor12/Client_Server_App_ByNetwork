using Checkers.Server.Networking;
using Lab3.Server.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab3.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            TCPServer.Instance.Setup();
            Console.ReadLine();
        }
    }
}
