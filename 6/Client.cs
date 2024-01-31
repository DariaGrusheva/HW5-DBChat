using _6.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _6
{
    internal class Client
    {
        private readonly IMessageSource _messageSource;
        private readonly IPEndPoint _ep;
        private readonly string _name;
        public Client(IMessageSource messageSource, IPEndPoint ep, string name)
        {
            _messageSource = messageSource;
            _ep = ep;
            _name = name;
        }

        private void Register ()// регистраия и отправка сообщения клиенту
        {
            var messageJson = new MessageUDP() //объект класса с полями имя, текст и т.д.
            { 
                Command = Command.Register,// зарегистрировали сообщение
                FromName = _name,// от кого
            };
            _messageSource.SendMessage(messageJson, _ep); //отправка 
        }

        public void ClientSendler()// отпрака
        {
           
            while (true)
            {
                Console.WriteLine("Введите сообщение:");
                string text = Console.ReadLine();
                Console.WriteLine("Введите получателя:");
                string toName = Console.ReadLine();
                if (string.IsNullOrEmpty(toName))
                 continue;
                    var messageJson = new MessageUDP()
                    {
                        Text = text,
                        FromName = _name,
                        ToName = toName,
                    };
                    _messageSource.SendMessage(messageJson, _ep);
                
            }
        }

        public void ClientListener()//прослушка
        {
            Register();
            IPEndPoint ep = new IPEndPoint(_ep.Address, _ep.Port);
            while (true)
            {
                Console.WriteLine("Ожидаем сообщения");
                MessageUDP message = _messageSource.ReceiveMessage(ref ep);
                Console.WriteLine(message.ToString());
            }
        }


    }
}
