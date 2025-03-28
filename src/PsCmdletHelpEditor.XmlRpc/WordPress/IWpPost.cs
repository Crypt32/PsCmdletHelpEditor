using System;

namespace PsCmdletHelpEditor.XmlRpc.WordPress {
    public interface IWpPost {
        String PostType { get; set; }
        String Title { get; set; }
        String HTML { get; set; }
        //public virtual DateTime? DateCreated { get; set; }
    }
}