using System;

namespace CmdletHelpEditor.API.Abstractions {
    public interface IPsExample {
        String Cmd { get; set; }
        String Description { get; set; }
        String Output { get; set; }
        String Name { get; set; }
    }
}