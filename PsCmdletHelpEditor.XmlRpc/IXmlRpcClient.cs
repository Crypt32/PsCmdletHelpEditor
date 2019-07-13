using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PsCmdletHelpEditor.XmlRpc {
    public interface IXmlRpcClient {
        Task<XmlRpcBlogInfo[]> GetUserBlogsAsync();
        Task<List<XmlRpcPost>> GetRecentPostsAsync(Int32 postCount);
        Task<XmlRpcPost> GetPostAsync(String postId);
        Task<String> AddPostAsync(XmlRpcPost post, Boolean publish = true);
        Task<Boolean> UpdatePostAsync(XmlRpcPost post, Boolean publish = true);
        Task<Boolean> DeletePostAsync(XmlRpcPost post, Boolean publish = false);
    }
}