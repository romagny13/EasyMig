using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace EasyMigApp.Models
{
    public class XmlSerializerService
    {
        public void Save<T>(string fileName, T obj)
        {
            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, obj);
            }
        }

        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public T Load<T>(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }
    }

    public class Session
    {
        private static List<string> emptyList;
        private static List<string> connectionStrings;
        private static string fileName;

        static Session()
        {
            connectionStrings = new List<string>();
            emptyList = new List<string>();
            fileName = "session.xml";
        }

        public static void AddConnectionString(string value)
        {
            if (!connectionStrings.Contains(value))
            {
                if (connectionStrings.Count > 5)
                {
                    connectionStrings.RemoveAt(0);
                }
                connectionStrings.Add(value);
            }
        }

        public static void Save()
        {
            var serializer = new XmlSerializerService();
            serializer.Save(fileName, connectionStrings);
        }

        public static List<string> Restore()
        {
            var serializer = new XmlSerializerService();
            if (serializer.FileExists(fileName))
            {
                connectionStrings = serializer.Load<List<string>>(fileName);
                return connectionStrings;
            }
            else
            {
                return emptyList;
            }
        }
    }

}
