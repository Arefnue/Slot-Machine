using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SlotMachine
{
    public static class SaveLoad
    {
        public static void Save<T>(T data)
        {
            var binaryFormatter = new BinaryFormatter();

            var path = GetSavePath();
            if (File.Exists(path))
                File.Delete(path);

            var fileStream = new FileStream(path, FileMode.Create);
            binaryFormatter.Serialize(fileStream, data);
        }

        public static T Load<T>()
        {
            if (!File.Exists(GetSavePath())) return (T)default;

            var binaryFormatter = new BinaryFormatter();

            var fileStream = new FileStream(GetSavePath(), FileMode.Open);
            
            if (fileStream.Length == 0)
                return default;

            var data = (T)binaryFormatter.Deserialize(fileStream);
          
            return data;
        }

        
        private static string GetSavePath()
        {
            return Application.persistentDataPath + "/game.data";
        }

    }
}