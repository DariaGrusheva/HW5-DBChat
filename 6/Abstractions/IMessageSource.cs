using _6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _6.Abstractions
{
    public interface IMessageSource
    {

        public void SendMessage(MessageUDP message, IPEndPoint ep);

        public MessageUDP ReceiveMessage(ref IPEndPoint ep);
    }
}
