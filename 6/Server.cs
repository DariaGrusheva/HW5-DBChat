using _6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _6
{
    public class Server
    {
        private readonly Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
        private UdpClient udpClient;

        public void Register(MessageUDP message, IPEndPoint fromep)
        {
            Console.WriteLine("Message Register, name = " + message.FromName);
            clients.Add(message.FromName, fromep);

            using (var ctx = new Context())
            {
                if (ctx.Users.FirstOrDefault(x => x.Name == message.FromName) != null) return;

                ctx.Add(new User { Name = message.FromName });

                ctx.SaveChanges();
            }
        }

        public void ConfirmMessageReceived(int? id)
        {
            Console.WriteLine("Message confirmation id=" + id);

            using (var ctx = new Context())
            {
                var msg = ctx.Messages.FirstOrDefault(x => x.Id == id);

                if (msg != null)
                {
                    msg.Received = true;
                    ctx.SaveChanges();
                }
            }
        }

        public void RelayMessage(MessageUDP message)
        {
            int? id = null;
            if (clients.TryGetValue(message.ToName, out IPEndPoint ep))
            {
                using (var ctx = new Context())
                {
                    var fromUser = ctx.Users.First(x => x.Name == message.FromName);
                    var toUser = ctx.Users.First(x => x.Name == message.ToName);
                    var msg = new Message { FromUser = fromUser, ToUser = toUser, Received = false, Text = message.Text };
                    ctx.Messages.Add(msg);

                    ctx.SaveChanges();

                    id = msg.Id;
                }

                var forwardMessageJson = new MessageUDP { Id = id, Command = Command.Message, ToName = message.ToName, FromName = message.FromName, Text = message.Text }.ToJson();

                byte[] forwardBytes = Encoding.ASCII.GetBytes(forwardMessageJson);

                udpClient.Send(forwardBytes, forwardBytes.Length, ep);
                Console.WriteLine($"Message Relayed, from = {message.FromName} to = {message.ToName}");
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }
        }

        public void ProcessMessage(MessageUDP message, IPEndPoint fromep)
        {
            Console.WriteLine($"Получено сообщение от {message.FromName} для {message.ToName} с командой {message.Command}:");
            Console.WriteLine(message.Text);

            if (message.Command == Command.Register)
            {
                Register(message, fromep);
            }
            else if (message.Command == Command.Confirmation)
            {
                Console.WriteLine("Confirmation receiver");
                ConfirmMessageReceived(message.Id);
            }
            else if (message.Command == Command.Message)
            {
                RelayMessage(message);
            }
        }


        void GetUnreadMessages(string userName)
        {
            if (clients.TryGetValue(userName, out IPEndPoint ep))
            {
                using (var ctx = new Context())
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Name == userName);
                    if (user != null)
                    {
                        var unreadMessages = user.ToMessages.Where(msg => !msg.Received).Select(msg => msg.Text).ToList();

                        var unreadMessagesJson = new MessageUDP
                        {
                            Command = Command.GetUnreadMessages,
                            FromName = "Server",
                            UnreadMessages = unreadMessages
                        };

                        byte[] unreadBytes = Encoding.ASCII.GetBytes(unreadMessagesJson.ToJson());
                        udpClient.Send(unreadBytes, unreadBytes.Length, ep);
                        Console.WriteLine($"Unread messages sent to {userName}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Пользователь {userName} не найден.");
            }
        }



        public void Work()
        {
            IPEndPoint remoteEndPoint;

            udpClient = new UdpClient(5430);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("UDP Клиент ожидает сообщений...");

            while (true)
            {
                Console.WriteLine("Ожидание сообщения...");
                byte[] receiveBytes = udpClient.Receive(ref remoteEndPoint);
                string receivedData = Encoding.ASCII.GetString(receiveBytes);

                Console.WriteLine(receivedData);

                try
                {
                    var message = MessageUDP.FromJson(receivedData);

                    ProcessMessage(message, remoteEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                }
            }
        }
    }
}
