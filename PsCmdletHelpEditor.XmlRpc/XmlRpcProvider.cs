using System;
using System.Net;
using System.Reflection;
using CookComputing.XmlRpc;

namespace PsCmdletHelpEditor.XmlRpc {
    public class XmlRpcProvider : XmlRpcService, IXmlRpcProvider {
        readonly IXmlRpcProvider _mwProvider;

        public XmlRpcProvider(String serverUrl) {
            _mwProvider = (IXmlRpcProvider)XmlRpcProxyGen.Create(typeof(IXmlRpcProvider));
            XmlRpcClientProtocol clientProtocol = _mwProvider as XmlRpcClientProtocol;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            clientProtocol.Url = serverUrl;
            clientProtocol.UserAgent = "PS Cmdlet Help Editor/" + Assembly.GetExecutingAssembly().GetName().Version;
            clientProtocol.NonStandard = XmlRpcNonStandard.All;
        }

        public Boolean DeletePost(String key, String postid, String username, String password, Boolean publish) {
            return _mwProvider.DeletePost(key, postid, username, password, publish);
        }
        public BlogInfo[] GetUsersBlogs(String key, String username, String password) {
            return _mwProvider.GetUsersBlogs(key, username, password);
        }
        public UserInfo GetUserInfo(String key, String username, String password) {
            return _mwProvider.GetUserInfo(key, username, password);
        }
        public String AddPost(String blogid, String username, String password, Post post, Boolean publish) {
            return _mwProvider.AddPost(blogid, username, password, post, publish);
        }
        public Boolean UpdatePost(String postid, String username, String password, Post post, Boolean publish) {
            return _mwProvider.UpdatePost(postid, username, password, post, publish);
        }
        public Post GetPost(String postid, String username, String password) {
            return _mwProvider.GetPost(postid, username, password);
        }
        public CategoryInfo[] GetCategories(String blogid, String username, String password) {
            return _mwProvider.GetCategories(blogid, username, password);
        }
        public Post[] GetRecentPosts(String blogid, String username, String password, Int32 numberOfPosts) {
            return _mwProvider.GetRecentPosts(blogid, username, password, numberOfPosts);
        }
        public MediaObjectInfo NewMediaObject(String blogid, String username, String password, MediaObject mediaObject) {
            return _mwProvider.NewMediaObject(blogid, username, password, mediaObject);
        }
    }
}
