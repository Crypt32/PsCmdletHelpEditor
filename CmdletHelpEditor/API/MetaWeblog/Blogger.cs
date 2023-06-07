using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security;
using CmdletHelpEditor.API.Tools;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    public class Blogger {
        String blogId;
        readonly String _userName;
        readonly SecureString _password;
        readonly IMetaWeblogProvider metaWeblogProvider;
        readonly XmlRpcClientProtocol clientProtocol;

        public Blogger(String url, String userName, SecureString password) {
            _userName = userName;
            _password = password;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            metaWeblogProvider = (IMetaWeblogProvider)XmlRpcProxyGen.Create(typeof(IMetaWeblogProvider));
            clientProtocol = (XmlRpcClientProtocol)metaWeblogProvider;
            clientProtocol.Url = url;
            clientProtocol.UserAgent = "PS Cmdlet Help Editor/" + Assembly.GetExecutingAssembly().GetName().Version;
            clientProtocol.NonStandard = XmlRpcNonStandard.All;
        }

        public void SetBlog(String blog) {
            this.blogId = blog;
        }
        public IEnumerable<BlogInfo> GetUsersBlogs() {
            return metaWeblogProvider.GetUsersBlogs(String.Empty, _userName, Crypt.SecureStringToString(_password));
        }
        public List<Post> GetRecentPosts(Int32 postCount = 5) {
            return new List<Post>(metaWeblogProvider.GetRecentPosts(blogId, _userName, Crypt.SecureStringToString(_password), postCount));
        }
        public Post GetPost(String postId) {
            return metaWeblogProvider.GetPost(postId, _userName, Crypt.SecureStringToString(_password));
        }
        public String AddPost(Post post, Boolean publish = true) {
            return metaWeblogProvider.AddPost(blogId, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        public Boolean UpdatePost(Post post, Boolean publish = true) {
            return metaWeblogProvider.UpdatePost(post.PostId, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        public Boolean DeletePost(String postId, Boolean publish = false) {
            return metaWeblogProvider.DeletePost(String.Empty, postId, _userName, Crypt.SecureStringToString(_password), publish);
        }
    }
}