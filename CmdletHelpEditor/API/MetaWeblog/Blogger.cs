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
        readonly IMetaWeblogProvider<Int32> metaWeblogProvider;
        readonly XmlRpcClientProtocol clientProtocol;

        public Blogger(String url, String userName, SecureString password) {
            _userName = userName;
            _password = password;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            metaWeblogProvider = (IMetaWeblogProvider<Int32>)XmlRpcProxyGen.Create(typeof(IMetaWeblogProvider<Int32>));
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
        public List<Post<String>> GetRecentPosts(Int32 postCount = 5) {
            return new List<Post<String>>(metaWeblogProvider.GetRecentPosts(blogId, _userName, Crypt.SecureStringToString(_password), postCount));
        }
        public Post<Int32> GetPost(String postId) {
            var id = Convert.ToInt32(postId);
            return metaWeblogProvider.GetPost(id, _userName, Crypt.SecureStringToString(_password));
        }
        public String AddPost(Post<Int32> post, Boolean publish = true) {
            return metaWeblogProvider.AddPost(blogId, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        public Boolean UpdatePost(Post<Int32> post, Boolean publish = true) {
            var id = Convert.ToInt32(post.PostId);
            return metaWeblogProvider.UpdatePost(id, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        public Boolean DeletePost(Int32 postId, Boolean publish = false) {
            var id = Convert.ToInt32(postId);
            return metaWeblogProvider.DeletePost(String.Empty, id, _userName, Crypt.SecureStringToString(_password), publish);
        }
    }
}