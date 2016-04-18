using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace ServerApp
{
    class Server
    {
        Socket sListener;
        List<User> userBook;

        List<Letter> letterBook;

        public Server(int Port)
        {
            userBook = new List<User>();
            letterBook = new List<Letter>();

            Console.WriteLine("Запуск сервера...");

            Console.WriteLine("Загрузка списка пользователей...");

            if (File.Exists(".\\Data\\Users.xml"))
            {
                userBook = Serialization.Deserialize<User>(".\\Data\\Users.xml");
                Console.WriteLine("Список пользователей успешно загружен");

                if (File.Exists(".\\Data\\Letters.xml"))
                {
                    letterBook = Serialization.Deserialize<Letter>(".\\Data\\Letters.xml");
                    Console.WriteLine("Список писем успешно загружен");
                }
                else
                {
                    Console.WriteLine("Не удалось загрузить файл со списком пользователей");
                }
            }
            else
            {
                Console.WriteLine("Не удалось загрузить файл со списком писем");
            }


            ConsoleHandler cc = new ConsoleHandler();
            cc.ControlEvent += new ConsoleHandler.ControlEventHandler(inputHandler);

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, Port);

            //Определение IP сервера
            String Host = System.Net.Dns.GetHostName();
            System.Net.IPAddress MyIp = Dns.GetHostByName(Host).AddressList[0];
            string StringMyIp = MyIp.ToString();

            Console.WriteLine("{0,-30} {1,20} {2,26} ", "Создание сокета...", StringMyIp, Port);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sListener.Bind(ipEndPoint);
            sListener.Listen(10);

            Console.WriteLine("Ожидание подключения пользователей...");

            // В бесконечном цикле
            while (true)
            {
                // Принимаем нового клиента
                Socket Client = sListener.Accept();

                // Создаем поток
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                // И запускаем этот поток, передавая ему принятого клиента
                Thread.Start(Client);
            }
        }


        void ClientThread(Object StateInfo)
        {
            // Просто создаем новый экземпляр класса Client и передаем ему приведенный к классу TcpClient объект StateInfo
            new ClientHandler((Socket)StateInfo, userBook, letterBook);
        }

        // Остановка сервера
        ~Server()
        {
            // Если "слушатель" был создан
            if (sListener != null)
            {
                sListener.Close();
            }
        }

        public void inputHandler(ConsoleHandler.ConsoleEvent consoleEvent)
        {
            if (consoleEvent == ConsoleHandler.ConsoleEvent.CtrlClose || consoleEvent == ConsoleHandler.ConsoleEvent.CtrlC)
            {
                Console.WriteLine("Сохранение списка пользователей...");
                Serialization.Serialize(userBook, ".\\Data\\Users.xml");


                Console.WriteLine("Сохранение списка писем...");
                Serialization.Serialize(letterBook, ".\\Data\\Letters.xml");
                Console.WriteLine("Сохранение завершено");
                System.Environment.Exit(-1);
            }
        }
    }
}
