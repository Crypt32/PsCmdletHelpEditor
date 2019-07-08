using System;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    public interface IMetaWeblogProvider<T> {
        #region MetaWeblog API
        [XmlRpcMethod("metaWeblog.newPost")]
        String AddPost(String blogid, String username, String password, Post<T> post, Boolean publish);

        [XmlRpcMethod("metaWeblog.editPost")]
        Boolean UpdatePost(T postid, String username, String password, Post<T> post, Boolean publish);

        [XmlRpcMethod("metaWeblog.getPost")]
        Post<Int32> GetPost(T postid, String username, String password);

        [XmlRpcMethod("metaWeblog.getCategories")]
        CategoryInfo[] GetCategories(String blogid, String username, String password);

        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        Post<String>[] GetRecentPosts(String blogid, String username, String password, Int32 numberOfPosts);

        [XmlRpcMethod("metaWeblog.newMediaObject")]
        MediaObjectInfo NewMediaObject(String blogid, String username, String password, MediaObject mediaObject);
        #endregion

        #region Blogger API
        [XmlRpcMethod("blogger.deletePost")]
        [return: XmlRpcReturnValue(Description = "Returns true.")]
        Boolean DeletePost(String key, T postid, String username, String password, Boolean publish);

        [XmlRpcMethod("blogger.getUsersBlogs")]
        BlogInfo[] GetUsersBlogs(String key, String username, String password);

        [XmlRpcMethod("blogger.getUserInfo")]
        UserInfo GetUserInfo(String key, String username, String password);
        #endregion
    }
}
