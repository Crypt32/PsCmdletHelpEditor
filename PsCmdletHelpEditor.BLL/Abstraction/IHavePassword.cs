using System.Security;

namespace PsCmdletHelpEditor.BLL.Abstraction {
    public interface IHavePassword {
        SecureString Password { get; }
    }
}