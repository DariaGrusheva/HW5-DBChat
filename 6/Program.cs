
// Итак, мы собираемся добавить поддержку работы с базой данных в наше приложение чата.
// Давайте разработаем для нее модель. Создадим новый проект - это будет серверное приложение.
// В проекте мы будем использовать CodeFirst подход. Начнем с двух таблиц - Messages и Users.
// В Messages должны храниться сообщения, тогда как в users список пользователей.
// Разработайте модель таким образом чтобы учесть что в сообщениях есть не только автор но и адресат и статус получениям им сообщения.

using _6.Abstractions;
using _6.Models;
using System.Net.Sockets;
using System.Net;

namespace _6
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5151);
            //UdpClient udpClient = new UdpClient();
            IMessageSource _message = new MessageSource(5151);
            if (args.Length == 0)
            {
                Server s = new(_message);
                s.Work();
                
            }
            else
            {
                /*Thread tr2 = new Thread(() => { Client.ClientSendler(args[0]); });
                tr2.Start();
                Thread tr1 = new Thread(() => { Client.ClientListener(); });
                tr1.Start();*/
                Client cl = new(_message, ep,"Cl");
                cl.ClientListener();
                cl.ClientSendler();


            }

            
            
        }
    }
}
