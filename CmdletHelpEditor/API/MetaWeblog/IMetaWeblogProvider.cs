using System;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    public interface IMetaWeblogProvider<T> {
        #region MetaWeblog API
        [XmlRpcMethod("metaWeblog.newPost")]
        String AddPost(String blogid, String username, String password, WpPost post, Boolean publish);
        [XmlRpcMethod("wp.newPost")]
        String AddWpPost(Int32 blogid, String username, String password, WpPostCreate post);

        [XmlRpcMethod("metaWeblog.editPost")]
        Boolean UpdatePost(T postid, String username, String password, WpPost post, Boolean publish);
        [XmlRpcMethod("wp.editPost")]
        Boolean UpdateWpPost(T blogid, String username, String password, T postid, WpPostUpdate post);

        [XmlRpcMethod("metaWeblog.getPost")]
        WpGetPost GetPost(T postid, String username, String password);
        [XmlRpcMethod("wp.getPost")]
        WpGetPost GetWpPost(String username, String password, Int32 postid);

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

        #region Wordpress
        [XmlRpcMethod("wp.getPosts")]
        WpGetPost[] GetPages(String blogid, String username, String password, XmlRpcPageFilter filter);
        #endregion

    }
}
