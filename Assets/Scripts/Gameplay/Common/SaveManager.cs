using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace FPS
{
    public static class SaveManager
    {
        private const string SAVE_FILE_NAME = "save.txt";
        private static readonly string SAVE_PATH = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        private const string ENCRYPTION_KEY = "b681e610e76216be305956abf2b7110a446677e246ed5e9cfeebbfa449444e2b"; // 256-bit key

        public static string ActiveSaveID { get; private set; }
        public static SaveDataContent LoadedData { get; private set; } = new SaveDataContent();
        private static SaveData _availableSaves = new SaveData();

        public static bool SaveInitialized = false;

        #region Initialization Methods
        public static void Initialize(string saveID)
        {
            ActiveSaveID = saveID;
            LoadData();
            SaveInitialized = true;
        }

        private static bool CheckForInitialization()
        {
            if (!SaveInitialized)
            {
                Debug.LogError("Save Manager not initialized yet!");
                return false;
            }
            return true;
        }
        #endregion

        #region Serialization Methods
        public static void SaveData()
        {
            if (!CheckForInitialization())
                return;

            SaveDataContent tempData = _availableSaves.AvailableSaves.FirstOrDefault(s => s.SaveID == ActiveSaveID);
            if (tempData != null)
            {
                int index = _availableSaves.AvailableSaves.IndexOf(tempData);
                _availableSaves.AvailableSaves[index] = LoadedData;
            }
            else
            {
                _availableSaves.AvailableSaves.Add(LoadedData);
            }

            string json = JsonUtility.ToJson(_availableSaves);
            string encryptedJson = Encrypt(json, ENCRYPTION_KEY);
            File.WriteAllText(SAVE_PATH, encryptedJson);
        }

        public static void LoadData()
        {
            if (!File.Exists(SAVE_PATH))
            {
                _availableSaves = new SaveData();
                LoadedData = new SaveDataContent();
                return;
            }

            try
            {
                string encryptedJson = File.ReadAllText(SAVE_PATH);
                string json = Decrypt(encryptedJson, ENCRYPTION_KEY);
                _availableSaves = JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load save file: {e.Message}");
                _availableSaves = new SaveData();
            }

            LoadedData = _availableSaves.AvailableSaves.FirstOrDefault(s => s.SaveID == ActiveSaveID) ?? new SaveDataContent();
        }
        #endregion

        #region Data Management Methods
        public static void SetResourceData(PlayerResourceData data, bool forceSaveData = true)
        {
            if (!CheckForInitialization())
                return;

            LoadedData.PlayerResourceData = data;
            if (forceSaveData)
                SaveData();
        }

        public static PlayerResourceData GetResourceData()
        {
            return CheckForInitialization() ? LoadedData.PlayerResourceData : null;
        }

        public static void SetInventoryData(InventoryData data, bool forceSaveData = true)
        {
            if (!CheckForInitialization())
                return;

            LoadedData.InventoryData = data;
            if (forceSaveData)
                SaveData();
        }

        public static InventoryData GetInventoryData()
        {
            return CheckForInitialization() ? LoadedData.InventoryData : null;
        }

        public static void DeleteAllSaveFiles()
        {
            if (File.Exists(SAVE_PATH))
            {
                File.Delete(SAVE_PATH);
                Debug.Log("Save file deleted.");
            }
        }

        public static void DeleteActiveSave()
        {
            if (!CheckForInitialization())
                return;
            SaveDataContent tempData = _availableSaves.AvailableSaves.FirstOrDefault(s => s.SaveID == ActiveSaveID);
            if (tempData != null)
            {
                _availableSaves.AvailableSaves.Remove(tempData);
                SaveData();
            }
        }
        #endregion

        #region Encryption Methods
        private static byte[] GetKeyBytes(string hexKey)
        {
            return Enumerable.Range(0, hexKey.Length / 2)
                .Select(i => Convert.ToByte(hexKey.Substring(i * 2, 2), 16))
                .ToArray();
        }

        private static string Encrypt(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                var keyBytes = GetKeyBytes(key);
                aesAlg.Key = keyBytes;
                aesAlg.GenerateIV();
                var iv = aesAlg.IV;

                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(iv, 0, iv.Length);
                    using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private static string Decrypt(string cipherText, string key)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            using (Aes aesAlg = Aes.Create())
            {
                var iv = new byte[aesAlg.BlockSize / 8];
                var cipher = new byte[fullCipher.Length - iv.Length];

                Array.Copy(fullCipher, iv, iv.Length);
                Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                var keyBytes = GetKeyBytes(key);
                aesAlg.Key = keyBytes;
                aesAlg.IV = iv;

                using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                using (var msDecrypt = new MemoryStream(cipher))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
        #endregion
    }

    [Serializable]
    public class SaveData
    {
        public List<SaveDataContent> AvailableSaves = new List<SaveDataContent>();
    }

    [Serializable]
    public class SaveDataContent
    {
        public string SaveID;
        public PlayerResourceData PlayerResourceData;
        public InventoryData InventoryData;
    }
}