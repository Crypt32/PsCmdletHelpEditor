using System;
using System.Security;

namespace PsCmdletHelpEditor.XmlRpc {
    public class XmlRpcProviderInfo {

        public XmlRpcProviderInfo(String provUrl, String userName, SecureString password) {
            ProviderUrl = provUrl;
            UserName = userName;
            Password = password;
        }

        public String ProviderID { get; set; }
        public String ProviderUrl { get; }
        public String UserName { get; }
        public SecureString Password { get; }
    }
}
