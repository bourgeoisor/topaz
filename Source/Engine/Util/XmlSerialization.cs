using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Topaz.Engine.Util
{
    public static class XmlSerialization
    {
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false, bool absolute = true) where T : new()
        {
            if (absolute)
                filePath = Path.Combine(Engine.Core.Instance.BASE_STORAGE_PATH, filePath);

            TextWriter writer = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static T ReadFromXmlFile<T>(string filePath, bool absolute = true) where T : new()
        {
            if (absolute)
                filePath = Path.Combine(Engine.Core.Instance.BASE_STORAGE_PATH, filePath);

            TextReader reader = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                return default(T);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
