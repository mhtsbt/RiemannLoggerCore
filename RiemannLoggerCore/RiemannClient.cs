using RiemannLoggerCore.Models;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace RiemannLoggerCore
{
    public class RiemannClient : IDisposable
    {

        private readonly string hostname;
        private readonly int port;
        private readonly int connectTimeout;
        private readonly int receiveTimeout;
        private readonly int sendTimeout;
        private TcpClient _client;

        public RiemannClient(string hostname = "localhost", int port = 5555, int connectTimeout = 0, int receiveTimeout = 0, int sendTimeout = 0)
        {
            this.hostname = hostname;
            this.port = port;
            this.connectTimeout = connectTimeout;
            this.sendTimeout = sendTimeout;
            this.receiveTimeout = receiveTimeout;
        }

        private async Task<NetworkStream> GetNetworkStream()
        {
            if (_client == null)
            {

                var c = new TcpClient();
                if (this.connectTimeout > 0)
                {
                    await Task.WhenAny(c.ConnectAsync(hostname, port), Task.Delay(this.connectTimeout));
                    if (!c.Connected)
                        throw new SocketException(10060);
                }
                else
                {
                    await c.ConnectAsync(hostname, port);
                }

                _client = c;
                _client.SendTimeout = this.sendTimeout;
                _client.ReceiveTimeout = this.receiveTimeout;

            }

            return _client.GetStream();

        }

        public void SendAndReceiveAsync(byte[] message)
        {
            try
            {
                Stream tcpStream = GetNetworkStream().Result;
                WriteRequest(message, tcpStream);
            }
            catch (Exception)
            {
                Console.WriteLine("failed to log to Riemann server");
            }
        }

        private static void WriteRequest(byte[] message, Stream tcpStream)
        {
            var lengthBigEndian = BitConverter.GetBytes(message.Length);
            Array.Reverse(lengthBigEndian);
            tcpStream.Write(lengthBigEndian, 0, lengthBigEndian.Length);
            tcpStream.Write(message, 0, message.Length);
            tcpStream.Flush();
        }

        public void Dispose()
        {
            // _client.Dispose();
        }

        public void Send(string service, string state, string description, List<KeyValuePair<string, string>> args)
        {
            SendAsync(new Event()
            {
                Service = service,
                State = state,
                Description = description,
                Attributes = args.Select(x => new EventAttribute() { Key = x.Key, Value = x.Value }).ToArray()
            });
        }

        public void SendAsync(params Event[] events)
        {
            var msg = new Message() { Events = events };

            var stream = new MemoryStream();
            Serializer.Serialize(stream, msg);
            var buffer = stream.ToArray();

            SendAndReceiveAsync(buffer);
        }

    }
}
