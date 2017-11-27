using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace Serialization
{
    [DataContract]
    public class SerializableObject
    {
        public string Serialize()
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(this.GetType());

            using (MemoryStream ms = new MemoryStream())
            {
                jsonFormatter.WriteObject(ms, this);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        public static T Deserialize<T>(string data)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(data)))
            {
                try
                {
                    T info = (T)jsonFormatter.ReadObject(ms);

                    return info;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Json deserealization exception: " + e.ToString());
                }

                return default(T);
            }
        }
    }
}
