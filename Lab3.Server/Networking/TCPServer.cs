using Lab3.Business.Models;
using Lab3.Client.Models;
using Lab3.Server.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Checkers.Server.Networking
{
    public class TCPServer
    {
        private static TCPServer _instance;
        private byte[] _buffer;
        private static readonly object _lock = new object();
        private static int _threadSafeBoolValue = 0;

        public static bool SharedLock
        {
            get
            {
                return Interlocked.CompareExchange(ref _threadSafeBoolValue, 1, 1) == 1;
            }
            set
            {
                if (value)
                {
                    Interlocked.CompareExchange(ref _threadSafeBoolValue, 1, 0);
                }
                else
                {
                    Interlocked.CompareExchange(ref _threadSafeBoolValue, 0, 1);
                }
            }
        }

        private List<Socket> _clients;
        private Socket _server;
        private const int Backlog = 1;

        public static TCPServer Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance is null)
                    {
                        _instance = new TCPServer();
                    }

                    return _instance;
                }
            }
        }


        public TCPServer()
        {
            _buffer = new byte[1024 * 1024];
            _clients = new List<Socket>();
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Setup()
        {
            Console.WriteLine("Setting up server...");
            _server.Bind(new IPEndPoint(IPAddress.Any, int.Parse(ConfigurationManager.AppSettings["Port"])));
            _server.Listen(Backlog);
            Console.WriteLine("Server started");
            _server.BeginAccept(AcceptClientCallback, null);
        }

        private void AcceptClientCallback(IAsyncResult asyncResult)
        {
            var socket = _server.EndAccept(asyncResult);
            _clients.Add(socket);
            Console.WriteLine("Client connected");
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveBufferCallback, socket);
            _server.BeginAccept(AcceptClientCallback, null);
        }

        private async void ReceiveBufferCallback(IAsyncResult asyncResult)
        {
            var socket = asyncResult.AsyncState as Socket;
            int received = socket.EndReceive(asyncResult);

            var tempBuffer = new byte[received];
            Array.Copy(_buffer, tempBuffer, received);

            var text = Encoding.ASCII.GetString(tempBuffer);
            Console.WriteLine("Operation received: " + text);

            await InvokeOperation(socket, text);

            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveBufferCallback, socket);
            _server.BeginAccept(AcceptClientCallback, null);
        }

        private async Task InvokeOperation(Socket socket, string text)
        {
            var operation = string.Concat(text.TakeWhile(c => !char.IsWhiteSpace(c)));

            foreach (var method in Router.ReadRouter)
            {
                if (operation.Equals(method.Key))
                {
                    if (!SharedLock)
                    {
                        var response = JArray.FromObject(await method.Value.Invoke());
                        var data = Encoding.ASCII.GetBytes(response.ToString());
                        socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendDataCallback, socket);
                        return;
                    }

                    SendPendingMessage(socket);
                }
            }

            foreach (var method in Router.ReadSingleRouter)
            {
                if (operation.Equals(method.Key))
                {
                    if (!SharedLock)
                    {
                        var id = text.Replace(method.Key, string.Empty).Trim();
                        var response = JObject.FromObject(await method.Value.Invoke(Guid.Parse(id)));
                        var data = Encoding.ASCII.GetBytes(response.ToString());
                        socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendDataCallback, socket);
                        return;
                        
                    }

                    SendPendingMessage(socket);
                }
            }

            foreach (var method in Router.ReadSingleWithUniqueRouter)
            {
                if (operation.Equals(method.Key))
                {
                    if (!SharedLock)
                    {
                        var parameter = text.Replace(method.Key, string.Empty).Trim();
                        var response = JObject.FromObject(await method.Value.Invoke(parameter));
                        var data = Encoding.ASCII.GetBytes(response.ToString());
                        socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendDataCallback, socket);
                        return;
                    }

                    SendPendingMessage(socket);
                }
            }

            foreach (var method in Router.WriteEmployeeRouter)
            {
                if (operation.Equals(method.Key))
                {
                    if (!SharedLock)
                    {
                        try
                        {
                            SharedLock = true;
                            var requestBody = text.Replace(method.Key, string.Empty).Trim();
                            var model = JObject.Parse(requestBody);
                            await method.Value.Invoke(model.ToObject<Employee>());
                            var response = ResponseResult.Success;
                            var data = Encoding.ASCII.GetBytes(response.ToString());
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendDataCallback, socket);
                            SharedLock = false;
                            return;
                        }
                        catch (Exception)
                        {
                            var response = ResponseResult.Failure;
                            var data = Encoding.ASCII.GetBytes(response.ToString());
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendDataCallback, socket);
                            SharedLock = false;
                            return;
                        }
                    }

                    SendPendingMessage(socket);
                }
            }

            foreach (var method in Router.WriteGroupRouter)
            {
                if (operation.Equals(method.Key))
                {
                    if (!SharedLock)
                    {
                        try
                        {
                            SharedLock = true;
                            var requestBody = text.Replace(method.Key, string.Empty).Trim();
                            var model = JObject.Parse(requestBody);
                            await method.Value.Invoke(model.ToObject<Group>());
                            var response = ResponseResult.Success;
                            var data = Encoding.ASCII.GetBytes(response.ToString());
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendDataCallback, socket);
                            SharedLock = false;
                            return;
                        }
                        catch (Exception)
                        {
                            var response = ResponseResult.Failure;
                            var data = Encoding.ASCII.GetBytes(response.ToString());
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendDataCallback, socket);
                            SharedLock = false;
                            return;
                        }
                    }

                    SendPendingMessage(socket);
                }
            }

            foreach (var method in Router.WriteLogRouter)
            {
                if (operation.Equals(method.Key))
                {
                    if (!SharedLock)
                    {
                        try
                        {
                            SharedLock = true;
                            var requestBody = text.Replace(method.Key, string.Empty).Trim();
                            var model = JObject.Parse(requestBody.Trim());
                            await method.Value.Invoke(model.ToObject<Log>());
                            var response = ResponseResult.Success;
                            var data = Encoding.ASCII.GetBytes(response.ToString());
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendDataCallback, socket);
                            SharedLock = false;
                            return;
                        }
                        catch (Exception)
                        {
                            var response = ResponseResult.Failure;
                            var data = Encoding.ASCII.GetBytes(response.ToString());
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendDataCallback, socket);
                            SharedLock = false;
                            return;
                        }
                    }

                    SendPendingMessage(socket);
                }
            }
        }

        private void SendDataCallback(IAsyncResult asyncResult)
        {
            var socket = asyncResult.AsyncState as Socket;
            socket.EndSend(asyncResult);
        }

        private void SendPendingMessage(Socket socket)
        {
            var lockResponse = "Locked";
            var lockData = Encoding.ASCII.GetBytes(lockResponse.ToString());
            socket.BeginSend(lockData, 0, lockData.Length, SocketFlags.None, SendDataCallback, socket);
        }
    }
}
