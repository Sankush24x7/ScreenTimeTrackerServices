using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ScreenTimeTracker
{
    public static class Encryptor
    {
        private static readonly byte[] keyBytes = Convert.FromBase64String("zJ6re+4XLm9Adnt/Ln3vz5MpQiH7by8iKoy32eXSUWw=");

        public static byte[] Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.GenerateIV();

            using MemoryStream ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
                sw.Flush();       // ✅ ensure all data is written
                cs.FlushFinalBlock(); // ✅ finalize encryption
            }

            return ms.ToArray();
        }

        public static string Decrypt(byte[] encryptedData)
        {
            using Aes aes = Aes.Create();
            aes.Key = keyBytes;

            using MemoryStream ms = new MemoryStream(encryptedData);
            byte[] iv = new byte[16];
            ms.Read(iv, 0, iv.Length);
            aes.IV = iv;

            using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }

}
