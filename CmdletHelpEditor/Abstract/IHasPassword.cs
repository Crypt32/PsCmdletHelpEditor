using System.Security;

namespace CmdletHelpEditor.Abstract;
public interface IHasPassword {
    SecureString Password { get; }
}