using System;

namespace PsCmdletHelpEditor.BLL.Tools {
    class XmlToken {
        public XmlToken() { }
        public XmlToken(String text, Int32 index, XmlTokenEnum type) {
            Text = text;
            Index = index;
            Type = type;
        }
        public String Text { get; set; }
        public Int32 Index { get; set; }
        public XmlTokenEnum Type { get; set; }
    }
}
