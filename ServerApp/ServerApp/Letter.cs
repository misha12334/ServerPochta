using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    [Serializable()]
    class Letter
    {
        public int _id;

        public int _id_sender;

        public string _FIO_recipient;

        public string _city_recipient;

        public string _adress_recipient;

        public string _status; //оптравлено, прибыло в город назначения, доставлено

        public int _id_courier;  //курьер
    }
}
