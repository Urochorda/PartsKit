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
            string tempPath = savePath + ".tmp";

            if (!string.IsNullOrEmpty(saveDir) && !Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            try
            {
                using (FileStream tempSaveFile = File.Create(tempPath))
                {
                    if (isEncryption)
                    {
                        if (string.IsNullOrEmpty(encryptionKey))
                        {
                            throw new ArgumentNullException(nameof(encryptionKey), "参数不能为空！");
                        }
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                            {
                                streamWriter.Write(dataStr);
                                streamWriter.Flush();
                                memoryStream.Position = 0;
                                Encrypt(memoryStream, tempSaveFile, encryptionKey);
                            }
                        }
                    }
                    else
                    {
                        using (StreamWriter streamWriter = new StreamWriter(tempSaveFile, Encoding.UTF8))
                        {
                            streamWriter.Write(dataStr);
                        }
                    }
                }

                //原子替换，防止存档损坏
                if (File.Exists(savePath))
                {
                    File.Replace(tempPath, savePath, null);
                }
                else
                {
                    File.Move(tempPath, savePath);
                }
            }
            catch (Exception e)
            {
                CustomLog.LogError(e);
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
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

            bool isSuccess;
            try
            {
                using (FileStream saveFile = File.Open(savePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (isEncryption)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (StreamReader streamReader = new StreamReader(memoryStream))
                            {
                                Decrypt(saveFile, memoryStream, encryptionKey);
                                memoryStream.Position = 0;
                                string infoStr = streamReader.ReadToEnd();
                                data = JsonConvert.DeserializeObject<T>(infoStr);
                            }
                        }
                    }
                    else
                    {
                        using (StreamReader streamReader = new StreamReader(saveFile, Encoding.UTF8))
                        {
                            string infoStr = streamReader.ReadToEnd();
                            data = JsonConvert.DeserializeObject<T>(infoStr);
                        }
                    }

                    data ??= getDefault == null ? default : getDefault.Invoke();
                    isSuccess = true;
                }
            }
            catch (Exception e)
            {
                CustomLog.LogError(e);
                isSuccess = false;
                data = getDefault == null ? default : getDefault.Invoke();
            }

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

        /// <summary>
        /// 加密数据流
        /// </summary>
        private void Encrypt(Stream inputStream, Stream outputStream, string sKey)
        {
            using (Aes aes = Aes.Create())
            {
                using (Rfc2898DeriveBytes keyDerivation =
                       new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(saltText), 10000))
                {
                    aes.Key = keyDerivation.GetBytes(aes.KeySize / 8);
                    aes.IV = keyDerivation.GetBytes(aes.BlockSize / 8);
                }

                using (CryptoStream cryptoStream =
                       new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    inputStream.CopyTo(cryptoStream);
                }
            }
        }

        private void Decrypt(Stream inputStream, Stream outputStream, string sKey)
        {
            using (Aes aes = Aes.Create())
            {
                using (Rfc2898DeriveBytes keyDerivation =
                       new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(saltText), 10000))
                {
                    aes.Key = keyDerivation.GetBytes(aes.KeySize / 8);
                    aes.IV = keyDerivation.GetBytes(aes.BlockSize / 8);
                }

                using (CryptoStream cryptoStream =
                       new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(outputStream);
                }
            }
        }
    }
}