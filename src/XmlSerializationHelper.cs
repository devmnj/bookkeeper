using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace accounts
{
    public abstract class XmlSerializationHelper
    {
        public static T Load<T>(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextReader reader = new StreamReader(fileName);
            T obj = (T)serializer.Deserialize(reader);
            reader.Close();
            return obj;
        }

        public static void Save<T>(T obj, string fileName)
        {
            TextWriter writer = new StreamWriter(fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, obj);
            writer.Close();
        }
    }
}
