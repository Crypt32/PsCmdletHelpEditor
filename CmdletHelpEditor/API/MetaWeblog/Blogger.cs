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
            blogId = blog;
        }
        public IEnumerable<BlogInfo> GetUsersBlogs() {
            return metaWeblogProvider.GetUsersBlogs(String.Empty, _userName, Crypt.SecureStringToString(_password));
        }
        public List<Post<String>> GetRecentPosts(Int32 postCount = 5) {
            return new List<Post<String>>(metaWeblogProvider.GetRecentPosts(blogId, _userName, Crypt.SecureStringToString(_password), postCount));
        }
        public List<WpGetPost> GetPages(Int32 postCount = 5) {
            var filter = new XmlRpcPageFilter();
            filter.PostType = "page";
            filter.Number = postCount;
            return new List<WpGetPost>(metaWeblogProvider.GetPages(blogId, _userName, Crypt.SecureStringToString(_password), filter));
        }
        public WpGetPost GetPost(String postId) {
            Int32 id = Convert.ToInt32(postId);
            return metaWeblogProvider.GetPost(id, _userName, Crypt.SecureStringToString(_password));
        }
        public WpGetPost GetWpPost(String postId) {
            Int32 id = Convert.ToInt32(postId);
            return metaWeblogProvider.GetWpPost(_userName, Crypt.SecureStringToString(_password), id);
        }
        public String AddPost(WpPost post, Boolean publish = true) {
            return metaWeblogProvider.AddPost(blogId, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        public String AddWpPost(WpPostCreate post) {
            Int32 id = Convert.ToInt32(blogId);
            return metaWeblogProvider.AddWpPost(id, _userName, Crypt.SecureStringToString(_password), post);
        }
        public Boolean UpdatePost(WpPost post, String postID, Boolean publish = true) {
            var id = Convert.ToInt32(postID);
            return metaWeblogProvider.UpdatePost(id, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        public Boolean UpdateWpPost(WpPostUpdate post, String postID, Boolean publish = true) {
            var pageid = Convert.ToInt32(postID);
            var blogid = Convert.ToInt32(blogId);
            return metaWeblogProvider.UpdateWpPost(blogid, _userName, Crypt.SecureStringToString(_password), pageid, post);
        }
        public Boolean DeletePost(Int32 postId, Boolean publish = false) {
            var id = Convert.ToInt32(postId);
            return metaWeblogProvider.DeletePost(String.Empty, id, _userName, Crypt.SecureStringToString(_password), publish);
        }
    }
}