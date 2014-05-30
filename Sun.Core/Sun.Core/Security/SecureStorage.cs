using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace Sun.Core.Security
{
    /// <summary>
    /// This static class holds functions to safely store
    /// data like passwords, ...
    /// </summary>
    public static class SecureStorage
    {
        /// <summary>
        /// This key is used to encrypt / decrypt the secure data
        /// 16 bytes give a 128-bit key
        /// </summary>
        private static readonly byte[] EncryptionKey = new byte[] { 125, 0, 27, 75, 200, 52, 10, 255, 6, 58, 22, 66, 159, 124, 165, 210 };

        /// <summary>
        /// This function stores user credentials using the Windows Data Protection API
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void StorePerUserCredentials(string userName, SecureString password, string fileName)
        {
            var data = ProtectedData.Protect(StringToByteArray(string.Format("{0};#{1}",
                userName, SecureStringUtility.SecureStringToString(password))),
                EncryptionKey, 
                DataProtectionScope.CurrentUser);

            File.WriteAllBytes(fileName, data);
        }

        /// <summary>
        /// This function restores user credentials using the Windows Data Protection API
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void RestorePerUserCredentials(out string userName, out SecureString password, string fileName)
        {
            userName = null;
            password = null;

            var decryptedData = ProtectedData.Unprotect(File.ReadAllBytes(fileName), EncryptionKey, DataProtectionScope.CurrentUser);
            string credentials = ByteArrayToString(decryptedData);
            var splitted = credentials.Split(new string[] {";#"}, StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length == 2)
            {
                userName = splitted[0];
                password = SecureStringUtility.StringToSecureString(splitted[1]);
            }
        }

        private static byte[] StringToByteArray(string str)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(str);
        }

        private static string ByteArrayToString(byte[] arr)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(arr);
        }
    }
}
