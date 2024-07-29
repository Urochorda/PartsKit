using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace PartsKit
{
    public class SaveSystem
    {
        public static SaveSystem Global { get; } = new SaveSystem(); //默认全局存储系统
        public EventItem SaveEvent { get; } = new EventItem();

        public string UserKey { get; private set; }

        private List<string> userAllName = new List<string>();

        public void SetUser(string user)
        {
            UserKey = user;
            userAllName = GetUserAllName(user);
        }

        public void Set<T>(string name, T data)
        {
            name = GetName(name);
            string dataStr = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(name, dataStr);
            if (HasCurUserKey() && !userAllName.Contains(name))
            {
                userAllName.Add(name);
                SaveCurUserAllName();
            }
        }

        public bool Get<T>(string name, out T data)
        {
            return Get(name, null, out data);
        }

        public bool Get<T>(string name, Func<T> getDefault, out T data)
        {
            name = GetName(name);
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
            name = GetName(name);
            PlayerPrefs.DeleteKey(name);
            if (HasCurUserKey() && userAllName.Contains(name))
            {
                userAllName.Remove(name);
                SaveCurUserAllName();
            }
        }

        public void DeleteAllByUser(string user)
        {
            var targetAllName = GetUserAllName(user);
            foreach (string name in targetAllName)
            {
                Delete(name);
            }
        }

        public void Save()
        {
            SaveEvent?.Trigger();
            PlayerPrefs.Save();
        }

        private string GetName(string name)
        {
            if (HasCurUserKey())
            {
                name = $"{UserKey}.{name}";
            }

            return name;
        }

        private bool HasCurUserKey()
        {
            return !string.IsNullOrEmpty(UserKey);
        }

        private void SaveCurUserAllName()
        {
            string userAllNameStr = JsonConvert.SerializeObject(userAllName);
            PlayerPrefs.SetString(GetUserAllNameSaveKey(UserKey), userAllNameStr);
        }

        private string GetUserAllNameSaveKey(string user)
        {
            return $"{user}_AllName";
        }

        private List<string> GetUserAllName(string user)
        {
            Get(GetUserAllNameSaveKey(user), () => new List<string>(), out List<string> allName);
            return allName;
        }
    }
}