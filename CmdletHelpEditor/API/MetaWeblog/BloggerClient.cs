using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;

namespace CmdletHelpEditor.API.MetaWeblog {
    class BloggerClient {
        readonly String _blogId;
        readonly String _userName;
        readonly SecureString _password;
        readonly IMetaWeblogProvider _metaWeblogProvider;

        public BloggerClient(ProviderInformation provider) {
            _userName = provider.UserName;
            _password = Crypt.DecryptPassword(provider.Password);
            _blogId = provider.Blog.BlogID;

            _metaWeblogProvider = (IMetaWeblogProvider)XmlRpcProxyGen.Create(typeof(IMetaWeblogProvider));
            XmlRpcClientProtocol clientProtocol = (XmlRpcClientProtocol)_metaWeblogProvider;
            clientProtocol.Url = provider.ProviderURL;
            clientProtocol.UserAgent = "PS Cmdlet Help Editor/" + Assembly.GetExecutingAssembly().GetName().Version;
            clientProtocol.NonStandard = XmlRpcNonStandard.All;
        }

        public Task<IEnumerable<BlogInfo>> GetUserBlogsAsync() {
            return Task.Factory.StartNew(GetUsersBlogs);
        }
        public Task<List<Post>> GetRecentPostsAsync(Int32 postCount = 5) {
            return Task.Factory.StartNew(() => GetRecentPosts(postCount));
        }
        public Task<Post> GetPostAsync(String postId) {
            return Task.Factory.StartNew(() => getPost(postId));
        }

        public Task<String> AddPostAsync(Post post, Boolean publish = true) {
            return Task.Factory.StartNew(() => addPost(post, publish));
        }
        public Task<Boolean> UpdatePostAsync(Post post, Boolean publish = true) {
            return Task.Factory.StartNew(() => updatePost(post, publish));
        }
        public Task<Boolean> DeletePostAsync(Post post, Boolean publish = false) {
            return Task.Factory.StartNew(() => deletePost(post, publish));
        }
        public IEnumerable<BlogInfo> GetUsersBlogs() {
            return _metaWeblogProvider.GetUsersBlogs(String.Empty, _userName, Crypt.SecureStringToString(_password));
        }
        public List<Post> GetRecentPosts(Int32 postCount) {
            return new List<Post>(_metaWeblogProvider.GetRecentPosts(_blogId, _userName, Crypt.SecureStringToString(_password), postCount));
        }
        Post getPost(String postId) {
            return _metaWeblogProvider.GetPost(postId, _userName, Crypt.SecureStringToString(_password));
        }
        String addPost(Post post, Boolean publish = true) {
            return _metaWeblogProvider.AddPost(_blogId, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        Boolean updatePost(Post post, Boolean publish = true) {
            return _metaWeblogProvider.UpdatePost(post.PostId, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        Boolean deletePost(Post post, Boolean publish = false) {
            return _metaWeblogProvider.DeletePost(String.Empty, post.PostId, _userName, Crypt.SecureStringToString(_password), publish);
        }
    }
}
