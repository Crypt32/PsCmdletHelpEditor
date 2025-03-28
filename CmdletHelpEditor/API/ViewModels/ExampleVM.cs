#nullable enable
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.ViewModels;

public class ExampleVM : ExampleRelatedLinkVM<PsCommandExampleVM> {
    public void SetCmdlet(CmdletObject? newCmdlet) {
        OnCmdletSet(newCmdlet?.Examples);
    }
    protected override PsCommandExampleVM CreateNewItem() {
        return new PsCommandExampleVM {
            Name = $"Example {Count + 1}"
        };
    }
}
