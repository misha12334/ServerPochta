using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    [Serializable()]
    class User
    {
        public int _id;
      

        public string _FIO;   
        public string _city;     

        public string _adress;
        public string _telephone;
        public string _login;
        public string _password;    

        public string _status;              //Пользователь, оператор, курьер
    }
}
