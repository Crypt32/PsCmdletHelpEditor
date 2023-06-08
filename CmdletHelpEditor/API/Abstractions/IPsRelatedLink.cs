using System;

namespace CmdletHelpEditor.API.Abstractions {
    public interface IPsRelatedLink {
        String LinkText { get; set; }
        String LinkUrl { get; set; }
    }
}