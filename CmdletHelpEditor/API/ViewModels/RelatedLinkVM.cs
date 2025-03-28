#nullable enable
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.ViewModels;
public class RelatedLinkVM : ExampleRelatedLinkVM<PsCommandRelatedLinkVM> {
    public void SetCmdlet(CmdletObject? newCmdlet) {
        OnCmdletSet(newCmdlet?.RelatedLinks);
    }
    protected override PsCommandRelatedLinkVM CreateNewItem() {
        return new PsCommandRelatedLinkVM { LinkText = "<Link>" };
    }
}