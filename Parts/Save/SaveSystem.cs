using System;
using UnityEngine;
using Newtonsoft.Json;

namespace PartsKit
{
    public class SaveSystem
    {
        public static SaveSystem Global { get; } = new SaveSystem(); //默认全局存储系统
        public EventItem SaveEvent { get; } = new EventItem();

        public string UserKey { get; set; } = string.Empty;

        public void Set<T>(string name, T data)
        {
            if (!string.IsNullOrEmpty(UserKey))
            {
                name = UserKey + name;
            }
            string dataStr = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(name, dataStr);
        }

        public bool Get<T>(string name, out T data)
        {
            if (!string.IsNullOrEmpty(UserKey))
            {
                name = UserKey + name;
            }
            return Get(name, null, out data);
        }

        public bool Get<T>(string name, Func<T> getDefault, out T data)
        {
            if (!PlayerPrefs.HasKey(name))
            {
                data = getDefault == null ? default : getDefault.Invoke();
                return false;
            }

            string dataStr = PlayerPrefs.GetString(name);
            try
            {
                data = JsonConvert.DeserializeObject<T>(dataStr);
            }
            catch (Exception e)
            {
                CustomLog.LogError(e);
                data = getDefault == null ? default : getDefault.Invoke();
                return false;
            }

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