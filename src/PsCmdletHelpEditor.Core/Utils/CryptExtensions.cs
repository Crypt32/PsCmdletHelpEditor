using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace PsCmdletHelpEditor.Core.Utils;
public static class CryptExtensions {
    /// <param name="password"></param>
    /// <returns>Encrypted password in Base64</returns>
    public static String EncryptPassword(this SecureString password) {
        Byte[] encryptedData = ProtectedData.Protect(
            Encoding.Unicode.GetBytes(Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(password))),
            null,
            DataProtectionScope.CurrentUser
        );
        return Convert.ToBase64String(encryptedData);
    }
    /// <param name="encryptedPassword">encrypted password in Base64</param>
    /// <returns>String in plain text.</returns>
    public static SecureString DecryptPassword(this String encryptedPassword) {
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
}
