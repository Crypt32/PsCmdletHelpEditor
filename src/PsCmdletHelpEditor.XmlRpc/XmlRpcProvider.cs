using System;
using System.Net;
using System.Reflection;
using CookComputing.XmlRpc;
using PsCmdletHelpEditor.XmlRpc.WordPress;

namespace PsCmdletHelpEditor.XmlRpc {
    public class XmlRpcProvider : IXmlRpcProvider {
        readonly IXmlRpcProvider _mwProvider;

        public XmlRpcProvider(String providerUrl) {
            _mwProvider = (IXmlRpcProvider)XmlRpcProxyGen.Create(typeof(IXmlRpcProvider));
            var clientProtocol = _mwProvider as XmlRpcClientProtocol;
            if (clientProtocol == null) {
                throw new ArgumentException("Not valid XmlRpcClientProtocol");
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            clientProtocol.Url = providerUrl;
            clientProtocol.UserAgent = "PS Cmdlet Help Editor/" + Assembly.GetExecutingAssembly().GetName().Version;
            clientProtocol.NonStandard = XmlRpcNonStandard.All;
        }

        public Boolean DeletePost(String key, String postid, String username, String password, Boolean publish) {
            return _mwProvider.DeletePost(key, postid, username, password, publish);
        }
        public XmlRpcBlogInfo[] GetUsersBlogs(String key, String username, String password) {
            return _mwProvider.GetUsersBlogs(key, username, password);
        }
        public XmlRpcUserInfo GetUserInfo(String key, String username, String password) {
            return _mwProvider.GetUserInfo(key, username, password);
        }
        public String AddPost(String blogid, String username, String password, XmlRpcPost post, Boolean publish) {
            return _mwProvider.AddPost(blogid, username, password, post, publish);
        }
        public Boolean UpdatePost(String postid, String username, String password, XmlRpcPost post, Boolean publish) {
            return _mwProvider.UpdatePost(postid, username, password, post, publish);
        }
        public XmlRpcPost GetPost(String postid, String username, String password) {
            return _mwProvider.GetPost(postid, username, password);
        }
        public XmlRpcCategoryInfo[] GetCategories(String blogid, String username, String password) {
            return _mwProvider.GetCategories(blogid, username, password);
        }
        public XmlRpcPost[] GetRecentPosts(String blogid, String username, String password, Int32 numberOfPosts) {
            return _mwProvider.GetRecentPosts(blogid, username, password, numberOfPosts);
        }
        public XmlRpcMediaObjectInfo NewMediaObject(String blogid, String username, String password, XmlRpcMediaObject mediaObject) {
            return _mwProvider.NewMediaObject(blogid, username, password, mediaObject);
        }
        public WpPost[] GetWpRecentPosts(String blogid, String username, String password, Int32 numberOfPosts) {
            return _mwProvider.GetWpRecentPosts(blogid, username, password, numberOfPosts);
        }
        public String AddWpPost(Int32 blogid, String username, String password, WpPostCreate post) {
            return _mwProvider.AddWpPost(blogid, username, password, post);
        }
        public Boolean UpdateWpPost(Int32 blogid, String username, String password, Int32 postid, WpPostUpdate post) {
            return _mwProvider.UpdateWpPost(blogid, username, password, postid, post);
        }
        public WpGetPost GetWpPost(String username, String password, Int32 postid) {
            return _mwProvider.GetWpPost(username, password, postid);
        }
        public WpGetPost[] GetPages(String blogid, String username, String password, XmlRpcPageFilter filter) {
            return _mwProvider.GetPages(blogid, username, password, filter);
        }
    }
}
