﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Topaz.Engine.Util
{
    class BinarySerialization
    {
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false, bool absolute = true)
        {
            if (absolute)
                filePath = Path.Combine(Engine.Core.Instance.BASE_STORAGE_PATH, filePath);

            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public static T ReadFromBinaryFile<T>(string filePath, bool absolute = true)
        {
            if (absolute)
                filePath = Path.Combine(Engine.Core.Instance.BASE_STORAGE_PATH, filePath);

            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }
}
