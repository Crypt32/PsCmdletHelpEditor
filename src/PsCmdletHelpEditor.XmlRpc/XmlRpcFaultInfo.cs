using System;
using CookComputing.XmlRpc;

namespace PsCmdletHelpEditor.XmlRpc {
    public class XmlRpcFaultInfo {
        [XmlRpcMember("faultCode")]
        public Int32 FaultCode { get; set; }
        [XmlRpcMember("faultString")]
        public String FaultString { get; set; }
    }
}