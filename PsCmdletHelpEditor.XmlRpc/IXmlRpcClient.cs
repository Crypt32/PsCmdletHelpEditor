using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PsCmdletHelpEditor.XmlRpc {
    public interface IXmlRpcClient {
        Task<BlogInfo[]> GetUserBlogsAsync();
        Task<List<Post>> GetRecentPostsAsync(Int32 postCount);
        Task<Post> GetPostAsync(String postId);
        Task<String> AddPostAsync(Post post, Boolean publish = true);
        Task<Boolean> UpdatePostAsync(Post post, Boolean publish = true);
        Task<Boolean> DeletePostAsync(Post post, Boolean publish = false);
    }
}