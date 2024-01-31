using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _6.Abstractions
{
    internal class MessageSource : IMessageSource
    {
        private readonly UdpClient _udpClient;
        public MessageSource(int port)
        {
            _udpClient = new UdpClient(port);
        }

        public void SendMessage(MessageUDP message, IPEndPoint ep)
        {
            string json = message.ToJson();
            byte[] data = Encoding.UTF8.GetBytes(json);
            _udpClient.Send(data, data.Length, ep);
        }

        public MessageUDP ReceiveMessage(ref IPEndPoint ep)
        {
            byte[] data = _udpClient.Receive(ref ep);
            string json = Encoding.UTF8.GetString(data);
            return MessageUDP.FromJson(json);
        }
    }
}
