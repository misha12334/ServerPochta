using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    class ClientHandler
    {
        public ClientHandler(Socket Client, List<User> userBook, List<Letter> letterBook)
        {
            bool haveConnect = true;
            string Request = "";
            byte[] Buffer = new byte[1024];
            int Count;

            Count = Client.Receive(Buffer);

            string IpClient = Encoding.UTF8.GetString(Buffer, 0, Count);

            DateTime nowTime = DateTime.Now;
            User currentUser = new User();

            Console.WriteLine("{0,-30} {1,20} {2,26} ", "Подключен новый пользователь", IpClient, nowTime);

            while (true)
            {
                //Count = Client.Receive(Buffer);
                Request = getField(Client);

                nowTime = DateTime.Now;

                switch (Request)
                {
                    case "END":
                        Console.WriteLine("{0,-30} {1,20} {2,26} ", "Пользователь завершил работу", IpClient, nowTime);
                        Client.Close();
                        haveConnect = false;
                        break;
                    case "AUTH":
                        Authorization(Client, currentUser, userBook);
                        break;
                    case "REG":
                        Registration(Client, userBook);
                        break;
                    case "SEND":
                        SendLetter(Client, userBook, letterBook);
                        break;
                    case "LIST":
                        getListLetters(Client, letterBook);
                        break;
                    case "GET":
                        getLetter(Client, userBook, letterBook);
                        break;
                    case "LIST_ALL":
                        getListAllLetters(Client, letterBook);
                        break;
                    case "CURIER":
                        getListCuriers(Client, userBook);
                        break;
                    case "SAVE":
                        saveChanges(Client, userBook, letterBook);
                        break;
                    case "LETERS_CUR":
                        showAllCurrierLetters(Client, letterBook);
                        break;                  
                }

                if (haveConnect == false)
                {
                    break;
                }

                Console.WriteLine("{0,-30} {1,20} {2,26} ", "Поступил запрос " + Request, IpClient, nowTime);
            }
        }

        public void saveChanges(Socket Client, List<User> userBook, List<Letter> letterBook)
        {
            int idLetter = Convert.ToInt32(getField(Client));

            string status = getField(Client);
            string currier = getField(Client);
            int idCurrier = 100000;

            if (currier != "Не назначен")
            {
                idCurrier = userBook.Where(usr => usr._FIO == currier).First()._id;
            }

            for (int i = 0; i < letterBook.Count(); i++)
            {
                if (letterBook[i]._id == idLetter)
                {
                    letterBook[i]._status = status;
                    if (idCurrier != 100000)
                    {
                        letterBook[i]._id_courier = idCurrier;
                    }
                    else
                    {
                        letterBook[i]._id_courier = 100000;
                    }
                    break;
                }
            }
        }

        public void getListCuriers(Socket Client, List<User> userBook)
        {
            string city = getField(Client);
            IEnumerable<User> users = userBook.Where(usr => usr._city == city && usr._status == "Курьер");

            int countUsers = users.Count();

            sendField(Client, countUsers.ToString());

            for (int i = 0; i < countUsers; i++)
            {
                sendField(Client, users.ElementAt(i)._FIO);

            }
        }

        public void getLetter(Socket Client, List<User> usersBook, List<Letter> letterBook)
        {
            int idLetter = Convert.ToInt32(getField(Client));

            Letter letter = letterBook.Where(ltr => ltr._id == idLetter).First();

            User sender = usersBook.Where(usr => usr._id == letter._id_sender).First();
            User cur= new User();
            if(letter._id_courier!=100000)
                cur = usersBook.Where(usr => usr._id == letter._id_courier).First();

            sendField(Client, sender._FIO);
            sendField(Client, sender._city);
            sendField(Client, sender._adress);

            sendField(Client, letter._FIO_recipient);
            sendField(Client, letter._city_recipient);
            sendField(Client, letter._adress_recipient);

            sendField(Client, letter._status);
            if (letter._id_courier != 100000 )
            {
                sendField(Client, cur._FIO);
            }
            else
            {
                sendField(Client, "Не назначен");
            }
        }

        public void getListLetters(Socket Client, List<Letter> letterBook)
        {
            int idSender = Convert.ToInt32(getField(Client));

            IEnumerable <Letter> letters = letterBook.Where(ltr => ltr._id_sender == idSender);

            int countLetters = letters.Count();

            sendField(Client, countLetters.ToString());

            for (int i = 0; i < countLetters; i++)
            {
                sendField(Client, letters.ElementAt(i)._id.ToString());
                sendField(Client, letters.ElementAt(i)._FIO_recipient);
                sendField(Client, letters.ElementAt(i)._status);
            }
        }

        public void showAllCurrierLetters(Socket Client, List<Letter> letterBook)
        {
            int idCurrier = Convert.ToInt32(getField(Client));

            IEnumerable<Letter> letters = letterBook.Where(ltr => ltr._id_courier == idCurrier);

            int countLetters = letters.Count();

            sendField(Client, countLetters.ToString());

            for (int i = 0; i < countLetters; i++)
            {
                sendField(Client, letters.ElementAt(i)._id.ToString());
                sendField(Client, letters.ElementAt(i)._FIO_recipient);
                sendField(Client, letters.ElementAt(i)._status);
            }
        }

        public void getListAllLetters(Socket Client, List<Letter> letterBook)
        {
            string city = getField(Client);

            IEnumerable<Letter> letters = letterBook.Where(ltr => ltr._city_recipient == city);

            int countLetters = letters.Count();

            sendField(Client, countLetters.ToString());

            for (int i = 0; i < countLetters; i++)
            {
                sendField(Client, letters.ElementAt(i)._id.ToString());
                sendField(Client, letters.ElementAt(i)._FIO_recipient);
                sendField(Client, letters.ElementAt(i)._status);
            }
        }

        public void SendLetter(Socket Client, List<User> userBook, List<Letter> letterBook)
        {
            
            Letter letter = new Letter();

            //User sender = userBook.Where(usr => usr._id == idSender).First();
            letter._id_sender = Convert.ToInt32(getField(Client));
            letter._FIO_recipient = getField(Client);
            letter._city_recipient = getField(Client);
            letter._adress_recipient = getField(Client);
            letter._id_courier = 100000;
            letter._status = "Отправлено";


            if (letterBook.Count() > 0)
            {
                int maxId = letterBook.Max(c => c._id);

                letter._id = maxId + 1;
            }
            else
            {
                letter._id = 0;
            }

            letterBook.Add(letter);
            

        }

        public void Authorization(Socket client, User user, List<User> users)
        {
            user._login = getField(client);
            user._password = getField(client);

            if (users.Exists(us => us._login == user._login && us._password == user._password))
            {
                sendField(client, "+OK");

                User authUser = users.Where(us => us._login == user._login && us._password == user._password).First();

                sendField(client, authUser._id.ToString());
                sendField(client, authUser._FIO);
                sendField(client, authUser._status);
                sendField(client, authUser._city);
                sendField(client, authUser._adress);
                
            }
            else
            {
                sendField(client, "-ERR");
            }
        }

        public void Registration(Socket client, List<User> users)
        {
            User user = new User();

            user._login = getField(client);
            user._password = getField(client);
            user._FIO = getField(client);
            user._city = getField(client);
            user._adress = getField(client);
            user._telephone = getField(client);
            user._status = getField(client);

            if (users.Exists(us => us._login == user._login) == false)
            {
                addNewUser(users, user);

                sendField(client, "+OK");
            }
            else
            {
                sendField(client, "-ERR");
            }

        }

        private void addNewUser(List<User> list, User user)
        {
            if (list.Count() > 0)
            {
                int maxId = list.Max(c => c._id);

                user._id = maxId + 1;
            }
            else
            {
                user._id = 0;
            }

            list.Add(user);
        }
        

        private string fillBits(string str, int numberBits)
        {
            while (str.Length < numberBits)
            {
                str = "0" + str;
            }

            return str;
        }

        private void sendField(Socket Client, string field)
        {
            byte[] Size = new byte[4];
            byte[] Buffer = new byte[1024];
            int sizeBuffer;

            //Получение размера сообщения
            Buffer = Encoding.UTF8.GetBytes(field);
            sizeBuffer = Buffer.Length;
            Size = Encoding.Default.GetBytes(fillBits(sizeBuffer.ToString(), 4));

            //Отправить размер сообщения
            Client.Send(Size, 4, new SocketFlags());

            //Отправить сообщение с указанным размером
            Buffer = Encoding.UTF8.GetBytes(field);
            Client.Send(Buffer, sizeBuffer, new SocketFlags());
        }

        public string getField(Socket Client)
        {
            byte[] Buffer = new byte[1024];

            int Count = Client.Receive(Buffer, 4, new SocketFlags());               //Получение размера сообщения
            int Size = Convert.ToInt32(Encoding.UTF8.GetString(Buffer, 0, Count));  //Размер сообщения

            Count = Client.Receive(Buffer, Size, new SocketFlags());                //Получить сообщение с указанным размером
            return Encoding.UTF8.GetString(Buffer, 0, Count);
        }
    }
}
