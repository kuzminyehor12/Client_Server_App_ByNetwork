using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Lab3.Server.Utility;
using System.Configuration;
using Checkers.Server.Networking;
using System.Windows.Forms;

namespace Lab3.Client.Networking
{
    public static class TCPClient
    {
        private static Socket _client;
        
        static TCPClient()
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public static void Connect()
        {
            while (!_client.Connected)
            {
                try
                {
                    var ip = NetworkUtility.GetIP();
                    _client.Connect(ip, int.Parse(ConfigurationManager.AppSettings["Port"]));
                }
                catch (SocketException)
                {
                    throw;
                }
            }
        }

        public static void Send(string request, out string response)
        {
            try
            {
                response = string.Empty;
                var buffer = Encoding.ASCII.GetBytes(request);
                _client.Send(buffer);
                Array.Clear(buffer, 0, buffer.Length);

                var receivingData = new byte[1024 * 1024];
                int received = _client.Receive(receivingData);
                var data = new byte[received];
                Array.Copy(receivingData, data, received);
                response = Encoding.ASCII.GetString(data);
            }
            catch (Exception)
            {
                response = "Server error";
            }
        }

        public static void Dispose()
        {
            _client.Close();
        }
    }
}
