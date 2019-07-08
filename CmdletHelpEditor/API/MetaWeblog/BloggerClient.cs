using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    class BloggerClient {
        readonly String _blogId;
        readonly String _userName;
        readonly SecureString _password;
        readonly IMetaWeblogProvider<Int32> _metaWeblogProvider;

        public BloggerClient(ProviderInformation provider) {
            _userName = provider.UserName;
            _password = Crypt.DecryptPassword(provider.Password);
            _blogId = provider.Blog.BlogID;

            _metaWeblogProvider = (IMetaWeblogProvider<Int32>)XmlRpcProxyGen.Create(typeof(IMetaWeblogProvider<Int32>));
            XmlRpcClientProtocol clientProtocol = (XmlRpcClientProtocol)_metaWeblogProvider;
            clientProtocol.Url = provider.ProviderURL;
            clientProtocol.UserAgent = "PS Cmdlet Help Editor/" + Assembly.GetExecutingAssembly().GetName().Version;
            clientProtocol.NonStandard = XmlRpcNonStandard.All;
        }

        public Task<IEnumerable<BlogInfo>> GetUserBlogsAsync() {
            return Task.Factory.StartNew(GetUsersBlogs);
        }
        public Task<List<Post<String>>> GetRecentPostsAsync(Int32 postCount = 5) {
            return Task.Factory.StartNew(() => GetRecentPosts(postCount));
        }
        public Task<Post<Int32>> GetPostAsync(String postId) {
            return Task.Factory.StartNew(() => getPost(postId));
        }

        public Task<String> AddPostAsync(Post<Int32> post, Boolean publish = true) {
            return Task.Factory.StartNew(() => addPost(post, publish));
        }
        public Task<Boolean> UpdatePostAsync(Post<Int32> post, Boolean publish = true) {
            return Task.Factory.StartNew(() => updatePost(post, publish));
        }
        public Task<Boolean> DeletePostAsync(Post<Int32> post, Boolean publish = false) {
            return Task.Factory.StartNew(() => deletePost(post, publish));
        }
        public IEnumerable<BlogInfo> GetUsersBlogs() {
            return _metaWeblogProvider.GetUsersBlogs(String.Empty, _userName, Crypt.SecureStringToString(_password));
        }
        public List<Post<String>> GetRecentPosts(Int32 postCount) {
            return new List<Post<String>>(_metaWeblogProvider.GetRecentPosts(_blogId, _userName, Crypt.SecureStringToString(_password), postCount));
        }
        Post<Int32> getPost(String postId) {
            var id = Convert.ToInt32(postId);
            return _metaWeblogProvider.GetPost(id, _userName, Crypt.SecureStringToString(_password));
        }
        String addPost(Post<Int32> post, Boolean publish = true) {
            return _metaWeblogProvider.AddPost(_blogId, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        Boolean updatePost(Post<Int32> post, Boolean publish = true) {
            var id = Convert.ToInt32(post.PostId);
            return _metaWeblogProvider.UpdatePost(id, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        Boolean deletePost(Post<Int32> post, Boolean publish = false) {
            var id = Convert.ToInt32(post.PostId);
            return _metaWeblogProvider.DeletePost(String.Empty, id, _userName, Crypt.SecureStringToString(_password), publish);
        }
    }
}
