using System;
using CookComputing.XmlRpc;

namespace PsCmdletHelpEditor.XmlRpc {
    /// <summary>
    /// Contains methods supported by a Metaweblog XML-RPC API.
    /// </summary>
    public interface IMetaWeblogProvider {
        /// <summary>
        /// Adds new post to blog.
        /// </summary>
        /// <param name="blogid">A blog ID (directory) to which the page is added.</param>
        /// <param name="username">User name to authenticate</param>
        /// <param name="password">User password to authenticate</param>
        /// <param name="post">Post to add.</param>
        /// <param name="publish">
        ///     <strong>True</strong> if the post is published, otherwise <strong>False</strong> and
        ///     post is saved as draft.
        /// </param>
        /// <returns>Post ID</returns>
        [XmlRpcMethod("metaWeblog.newPost")]
        String AddPost(String blogid, String username, String password, XmlRpcPost post, Boolean publish);
        /// <summary>
        /// Updates existing blog post.
        /// </summary>
        /// <param name="postid">Post ID to update.</param>
        /// <param name="username">User name to authenticate</param>
        /// <param name="password">User password to authenticate</param>
        /// <param name="post">Post to update.</param>
        /// <param name="publish">
        ///     <strong>True</strong> if the post is published, otherwise <strong>False</strong> and
        ///     post is saved as draft.
        /// </param>
        /// <returns>True.</returns>
        [XmlRpcMethod("metaWeblog.editPost")]
        Boolean UpdatePost(String postid, String username, String password, XmlRpcPost post, Boolean publish);
        /// <summary>
        /// Gets existing post.
        /// </summary>
        /// <param name="postid">Post ID to retrieve.</param>
        /// <param name="username">User name to authenticate</param>
        /// <param name="password">User password to authenticate</param>
        /// <returns>
        /// Requested post.
        /// </returns>
        [XmlRpcMethod("metaWeblog.getPost")]
        XmlRpcPost GetPost(String postid, String username, String password);
        /// <summary>
        /// Gets content categories for blog post.
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username">User name to authenticate</param>
        /// <param name="password">User password to authenticate</param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getCategories")]
        XmlRpcCategoryInfo[] GetCategories(String blogid, String username, String password);
        /// <summary>
        /// Gets recent posts for blog post. Posts are sorted by created date in descending order.
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username">User name to authenticate</param>
        /// <param name="password">User password to authenticate</param>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        XmlRpcPost[] GetRecentPosts(String blogid, String username, String password, Int32 numberOfPosts);
        /// <summary>
        /// Saves media attachment for blog post locally.
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username">User name to authenticate</param>
        /// <param name="password">User password to authenticate</param>
        /// <param name="mediaObject">
        /// Media attachment to save.
        /// </param>
        /// <returns>Information about saved media attachment.</returns>
        [XmlRpcMethod("metaWeblog.newMediaObject")]
        XmlRpcMediaObjectInfo NewMediaObject(String blogid, String username, String password, XmlRpcMediaObject mediaObject);
    }
}
