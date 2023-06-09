using System;
using CmdletHelpEditor.API.Abstractions;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;

public class BlogInfo : ViewModelBase, IBlogInfo {
    String blogId, blogName, url;

    public String BlogID {
        get => blogId;
        set {
            blogId = value;
            OnPropertyChanged(nameof(BlogID));
        }
    }
    public String BlogName {
        get => blogName;
        set {
            blogName = value;
            OnPropertyChanged(nameof(BlogName));
        }
    }
    public String URL {
        get => url;
        set {
            url = value;
            OnPropertyChanged(nameof(URL));
        }
    }
}