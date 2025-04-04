﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PsCmdletHelpEditor.XmlRpc.Helpers;
using PsCmdletHelpEditor.XmlRpc.WordPress;

namespace PsCmdletHelpEditor.XmlRpc {
    public class WpXmlRpcClient {
        readonly XmlRpcProviderInfo _provInfo;
        readonly IXmlRpcProvider _mwProvider;

        public WpXmlRpcClient(XmlRpcProviderInfo provider) {
            _provInfo = provider;
            _mwProvider = new XmlRpcProvider(provider.ProviderUrl);
        }

        public Task<XmlRpcBlogInfo[]> GetUserBlogsAsync() {
            return Task.Run(() =>
                _mwProvider.GetUsersBlogs(
                    String.Empty,
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString())
                );
        }
        public Task<List<WpPost>> GetRecentPostsAsync(Int32 postCount = 5) {
            return Task.Run(() =>
                _mwProvider.GetWpRecentPosts(
                    _provInfo.ProviderID,
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString(),
                    postCount).ToList()
                );
        }
        public Task<WpGetPost> GetPostAsync(Int32 postId) {
            return Task.Run(() =>
                _mwProvider.GetWpPost(
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString(),
                    postId)
                );
        }

        public Task<String> AddPostAsync(WpPostCreate post, Boolean publish = true) {
            return Task.Run(() =>
                _mwProvider.AddWpPost(
                    Convert.ToInt32(_provInfo.ProviderID),
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString(),
                    post)
                );
        }
        public Task<Boolean> UpdatePostAsync(WpPostUpdate post, Int32 postId, Boolean publish = true) {
            return Task.Run(() =>
                _mwProvider.UpdateWpPost(
                    Convert.ToInt32(_provInfo.ProviderID),
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString(),
                    postId,
                    post)
                );
        }
        public Task<Boolean> DeletePostAsync(XmlRpcPost post, Boolean publish = false) {
            return Task.Run(() =>
                _mwProvider.DeletePost(
                    String.Empty,
                    post.PostId,
                    _provInfo.UserName,
                    _provInfo.Password.ToPlainString(),
                    publish)
                );
        }

        public void SetBlog(String blogID) {
            _provInfo.ProviderID = blogID;
        }
    }
}