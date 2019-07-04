using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace PsCmdletHelpEditor.BLL.Tools {
    public static class Crypt {
        /// <param name="password"></param>
        /// <returns>Encrypted password in Base64</returns>
        public static String EncryptPassword(SecureString password) {
            Byte[] encryptedData = ProtectedData.Protect(
                Encoding.Unicode.GetBytes(Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(password))),
                null,
                DataProtectionScope.CurrentUser
            );
            return Convert.ToBase64String(encryptedData);
        }
        /// <param name="encryptedPassword">encrypted password in Base64</param>
        /// <returns>String in plain text.</returns>
        public static SecureString DecryptPassword(String encryptedPassword) {
            SecureString ss = new SecureString();
            try {
                foreach (Byte b in ProtectedData.Unprotect(Convert.FromBase64String(encryptedPassword), null, DataProtectionScope.CurrentUser)) {
                    ss.AppendChar(Convert.ToChar(b));
                }
            } finally {
                ss.MakeReadOnly();
                GC.Collect();
            }
            return ss;
        }
        // converts SecureString to plain text.
        public static String SecureStringToString(SecureString value) {
            IntPtr bstr = Marshal.SecureStringToBSTR(value);
            try {
                return Marshal.PtrToStringBSTR(bstr).Replace("\0", null);
            } finally {
                Marshal.FreeBSTR(bstr);
            }
        }
        // plain text to SecureString
        public static SecureString StringToSecureString(String str) {
            var ss = new SecureString();
            foreach (Char c in str) {
                ss.AppendChar(c);
            }
            ss.MakeReadOnly();
            return ss;
        }
    }
}
