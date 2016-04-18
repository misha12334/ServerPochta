using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    class Serialization
    {
        public static void Serialize<T>(List<T> list, string file)
        {
            using (Stream stream = File.Open(file, FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, list);
            }
        }

        public static List<T> Deserialize<T>(string file) where T : class, new()
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter bf = new BinaryFormatter();

            List<T> list = (List<T>)bf.Deserialize(fs);
            fs.Close();

            return list;
        }
    }
}
