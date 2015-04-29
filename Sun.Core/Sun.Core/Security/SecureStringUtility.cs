using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;

namespace Sun.Core.Security
{
    /// <summary>
    /// This utility holds some usefull functions when dealing with secure strings
    /// </summary>
    public static class SecureStringUtility
    {
        public static String SecureStringToString(SecureString input)
        {
            string output = "";
            IntPtr ptr = SecureStringToBSTR(input);
            output = PtrToStringBSTR(ptr);
            return output;
        }

        public static SecureString StringToSecureString(string input)
        {
            var secureString = new SecureString();

            for (int i = 0; i < input.Length; i++)
            {
                secureString.AppendChar(input[i]);
            }
            return secureString;
        }

        private static IntPtr SecureStringToBSTR(SecureString ss)
        {
            if (ss == null)
                return IntPtr.Zero;

            IntPtr ptr = new IntPtr();
            ptr = Marshal.SecureStringToBSTR(ss);
            return ptr;
        }

        private static string PtrToStringBSTR(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            string s = Marshal.PtrToStringBSTR(ptr);
            Marshal.ZeroFreeBSTR(ptr);
            return s;
        }   
    }
}
