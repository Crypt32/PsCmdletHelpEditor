using System;
using System.ComponentModel;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    public class BlogInfo : INotifyPropertyChanged {
        String blogId, blogName, _url;

        [XmlRpcMember("blogid")]
        public String BlogID {
            get => blogId;
            set {
                blogId = value;
                OnPropertyChanged(nameof(BlogID));
            }
        }
        [XmlRpcMember("blogName")]
        public String BlogName {
            get => blogName;
            set {
                blogName = value;
                OnPropertyChanged(nameof(BlogName));
            }
        }
        [XmlRpcMember("url")]
        public String URL {
            get => _url;
            set {
                _url = value;
                OnPropertyChanged(nameof(URL));
            }
        }

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
