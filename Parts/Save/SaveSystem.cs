using UnityEngine;
using Newtonsoft.Json;

namespace PartsKit
{
    public class SaveSystem
    {
        public static SaveSystem Global { get; } = new SaveSystem(); //默认全局存储系统
        public EventItem SaveEvent { get; } = new EventItem();

        public void Set<T>(string name, T data)
        {
            string dataStr = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(name, dataStr);
        }

        public bool Get<T>(string name, out T data)
        {
            if (!PlayerPrefs.HasKey(name))
            {
                data = default;
                return false;
            }

            string dataStr = PlayerPrefs.GetString(name);
            data = JsonConvert.DeserializeObject<T>(dataStr);
            return true;
        }

        public void Delete(string name)
        {
            PlayerPrefs.DeleteKey(name);
        }

        public void Save()
        {
            SaveEvent?.Trigger();
            PlayerPrefs.Save();
        }
    }
}