using System;
using CookComputing.XmlRpc;

namespace PsCmdletHelpEditor.XmlRpc {
    /// <summary>
    /// Contains methods supported by a Blogger XML-RPC API.
    /// </summary>
    public interface IBloggerWeblogProvider {
        /// <summary>
        /// Deletes existing post.
        /// </summary>
        /// <param name="key">API key. Not used.</param>
        /// <param name="postid">Blog post to delete.</param>
        /// <param name="username">User name to authenticate</param>
        /// <param name="password">User password to authenticate</param>
        /// <param name="publish">Not used.</param>
        /// <returns>True.</returns>
        [XmlRpcMethod("blogger.deletePost")]
        Boolean DeletePost(String key, String postid, String username, String password, Boolean publish);
        /// <summary>
        /// Gets a collection of blogs that support Blogger XML-RPC API.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="username">User name to authenticate</param>
        /// <param name="password">User password to authenticate</param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.getUsersBlogs")]
        XmlRpcBlogInfo[] GetUsersBlogs(String key, String username, String password);
        /// <summary>
        /// Gets current user info.
        /// </summary>
        /// <param name="key">API key. Not used.</param>
        /// <param name="username">User name to authenticate</param>
        /// <param name="password">User password to authenticate</param>
        /// <returns>Current user info.</returns>
        [XmlRpcMethod("blogger.getUserInfo")]
        XmlRpcUserInfo GetUserInfo(String key, String username, String password);
    }
}
