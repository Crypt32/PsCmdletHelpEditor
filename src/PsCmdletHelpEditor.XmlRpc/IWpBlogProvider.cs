using System;
using CookComputing.XmlRpc;
using PsCmdletHelpEditor.XmlRpc.WordPress;

namespace PsCmdletHelpEditor.XmlRpc {
    public interface IWpBlogProvider {
        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        WpPost[] GetWpRecentPosts(String blogid, String username, String password, Int32 numberOfPosts);
        [XmlRpcMethod("wp.newPost")]
        String AddWpPost(Int32 blogid, String username, String password, WpPostCreate post);
        [XmlRpcMethod("wp.editPost")]
        Boolean UpdateWpPost(Int32 blogid, String username, String password, Int32 postid, WpPostUpdate post);
        [XmlRpcMethod("wp.getPost")]
        WpGetPost GetWpPost(String username, String password, Int32 postid);
        [XmlRpcMethod("wp.getPosts")]
        WpGetPost[] GetPages(String blogid, String username, String password, XmlRpcPageFilter filter);
    }
}