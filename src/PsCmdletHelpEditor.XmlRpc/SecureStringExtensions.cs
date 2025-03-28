using System;
using System.Runtime.InteropServices;
using System.Security;

namespace PsCmdletHelpEditor.XmlRpc {
    static class SecureStringExtensions {
        public static String ToPlainString(this SecureString secureString) {
            IntPtr bstr = Marshal.SecureStringToBSTR(secureString);
            try {
                return Marshal.PtrToStringBSTR(bstr).Replace("\0", null);
            } finally {
                Marshal.FreeBSTR(bstr);
            }
        }
    }
}
