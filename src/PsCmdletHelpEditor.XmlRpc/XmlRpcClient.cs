using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PsCmdletHelpEditor.XmlRpc.Helpers;

namespace PsCmdletHelpEditor.XmlRpc {
    public class XmlRpcClient : IXmlRpcClient {
        readonly XmlRpcProviderInfo _provInfo;
        readonly IXmlRpcProvider _mwProvider;

        public XmlRpcClient(XmlRpcProviderInfo provider) {
            _provInfo = provider;
            _mwProvider = new XmlRpcProvider(provider.ProviderUrl);
        }

        public Task<XmlRpcBlogInfo[]> GetUserBlogsAsync() {
            return Task.Factory.StartNew(() =>
                _mwProvider.GetUsersBlogs(
                    String.Empty,
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString())
                );
        }
        public Task<List<XmlRpcPost>> GetRecentPostsAsync(Int32 postCount = 5) {
            return Task.Factory.StartNew(() =>
                _mwProvider.GetRecentPosts(
                    _provInfo.ProviderID,
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString(),
                    postCount).ToList()
                );
        }
        public Task<XmlRpcPost> GetPostAsync(String postId) {
            return Task.Factory.StartNew(() =>
                _mwProvider.GetPost(
                    postId,
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString())
                );
        }

        public Task<String> AddPostAsync(XmlRpcPost post, Boolean publish = true) {
            return Task.Factory.StartNew(() =>
                _mwProvider.AddPost(
                    _provInfo.ProviderID,
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString(),
                    post,
                    publish)
                );
        }
        public Task<Boolean> UpdatePostAsync(XmlRpcPost post, Boolean publish = true) {
            return Task.Factory.StartNew(() =>
                _mwProvider.UpdatePost(
                    post.PostId,
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString(),
                    post,
                    publish)
                );
        }
        public Task<Boolean> DeletePostAsync(XmlRpcPost post, Boolean publish = false) {
            return Task.Factory.StartNew(() =>
                _mwProvider.DeletePost(
                    String.Empty,
                    post.PostId,
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString(),
                    publish)
                );
        }
    }
}