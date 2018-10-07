using System;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    public class UserInfo {
        [XmlRpcMember("userid")]
        public String UserId { get; set; }
        [XmlRpcMember("firstname")]
        public String FirstName { get; set; }
        [XmlRpcMember("lastname")]
        public String LastName { get; set; }
        [XmlRpcMember("nickname")]
        public String NickName { get; set; }
        [XmlRpcMember("email")]
        public String Email { get; set; }
        [XmlRpcMember("url")]
        public String Url { get; set; }
    }
}