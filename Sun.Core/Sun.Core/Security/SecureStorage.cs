using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using Microsoft.Win32;

namespace Sun.Core.Security
{
    /// <summary>
    /// This static class holds functions to safely store
    /// data like passwords, ...
    /// </summary>
    public static class SecureStorage
    {
        /// <summary>
        /// This function stores user credentials using the Windows Data Protection API
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void StorePerUserCredentials(string userName, SecureString password, string fileName)
        {
            // Generate additional entropy (will be used as the Initialization vector)
            // This is basically the (2048-bit) encryption key used to encrypt the credentials
            // The encryption key changes everytime the credentials get stored for increased security (everytime someone logs in with "Remember Me" ticked)
            byte[] entropy = new byte[256];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            var currentUserRegistry = Registry.CurrentUser.OpenSubKey("Software\\SystemsUnitedNavy", true);
            if (currentUserRegistry == null)
                currentUserRegistry = Registry.CurrentUser.CreateSubKey("Software\\SystemsUnitedNavy", RegistryKeyPermissionCheck.Default);

            currentUserRegistry.SetValue("SecureCredentialsStorageEntropy", entropy);


            var data = ProtectedData.Protect(StringToByteArray(string.Format("{0};#{1}",
                userName, SecureStringUtility.SecureStringToString(password))),
                entropy,
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

            // Read the encryption key from the registry
            var sunKey = Registry.CurrentUser.OpenSubKey("Software\\SystemsUnitedNavy");
            if (sunKey == null) // If no encryption key exists, return no credentials as we can't decrypt the credentials files in this case
                return;

            var entropy = sunKey.GetValue("SecureCredentialsStorageEntropy");

            var decryptedData = ProtectedData.Unprotect(File.ReadAllBytes(fileName), (byte[])entropy, DataProtectionScope.CurrentUser);
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
