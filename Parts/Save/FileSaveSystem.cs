using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace PartsKit
{
    public class FileSaveSystem
    {
        public static FileSaveSystem Global { get; } = new FileSaveSystem(); //默认全局存储系统
        public EventItem SaveEvent { get; } = new EventItem();

        public string UserKey { get; private set; }

        private bool isEncryption;
        private string encryptionKey = "EncryptionKey";
        private string saltText = "SaltTextGoesHere";

        public void SetUser(string user)
        {
            UserKey = user;
        }

        public void SetEncryption(bool isEnc, string key, string text)
        {
            isEncryption = isEnc;
            encryptionKey = key;
            saltText = text;
        }

        public void Set<T>(string name, T data)
        {
            string dataStr = JsonConvert.SerializeObject(data);
            string savePath = GetSavePath(name, out string fileName, out string saveDir);
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            FileStream saveFile = File.Create(savePath);

            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamWriter streamWriter = new StreamWriter(memoryStream))
            {
                streamWriter.Write(dataStr);
                if (isEncryption)
                {
                    streamWriter.Flush();
                    memoryStream.Position = 0;
                    Encrypt(memoryStream, saveFile, encryptionKey);
                }
            }

            saveFile.Close();
        }

        public bool Get<T>(string name, out T data)
        {
            return Get(name, null, out data);
        }

        public bool Get<T>(string name, Func<T> getDefault, out T data)
        {
            string savePath = GetSavePath(name, out string fileName, out string saveDir);
            if (!File.Exists(savePath))
            {
                data = getDefault == null ? default : getDefault.Invoke();
                return false;
            }

            FileStream saveFile = File.Open(savePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            bool isSuccess;
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                try
                {
                    if (isEncryption)
                    {
                        Decrypt(saveFile, memoryStream, encryptionKey);
                        memoryStream.Position = 0;
                    }

                    data = JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
                    isSuccess = true;
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    CustomLog.LogError(e);
                    data = getDefault == null ? default : getDefault.Invoke();
                }
            }

            saveFile.Close();
            return isSuccess;
        }

        public void Delete(string name)
        {
            string savePath = GetSavePath(name, out string fileName, out string saveDir);

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
        }

        public void DeleteAllByUser(string user)
        {
            if (GetUserPath(user, out string path) && Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private bool HasCurUserKey()
        {
            return !string.IsNullOrEmpty(UserKey);
        }

        private string GetSavePath(string saveName, out string fileName, out string directory)
        {
            if (!GetUserPath(UserKey, out directory))
            {
                directory = GetCommonPath();
            }

            fileName = saveName;
            return $"{directory}/{fileName}";
        }

        private bool GetUserPath(string userKey, out string userData)
        {
            if (!HasCurUserKey())
            {
                userData = string.Empty;
                return false;
            }

            userData = $"{GetSaveRoot()}/{userKey}";
            return true;
        }

        private string GetCommonPath()
        {
            return $"{GetSaveRoot()}/Common";
        }

        private string GetSaveRoot()
        {
            return $"{Application.persistentDataPath}";
        }

        private void Encrypt(Stream inputStream, Stream outputStream, string sKey)
        {
            RijndaelManaged algorithm = new RijndaelManaged();
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(saltText));

            algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
            algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

            CryptoStream cryptoStream =
                new CryptoStream(inputStream, algorithm.CreateEncryptor(), CryptoStreamMode.Read);
            cryptoStream.CopyTo(outputStream);
        }

        private void Decrypt(Stream inputStream, Stream outputStream, string sKey)
        {
            RijndaelManaged algorithm = new RijndaelManaged();
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(saltText));

            algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
            algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

            CryptoStream cryptoStream =
                new CryptoStream(inputStream, algorithm.CreateDecryptor(), CryptoStreamMode.Read);
            cryptoStream.CopyTo(outputStream);
        }
    }
}