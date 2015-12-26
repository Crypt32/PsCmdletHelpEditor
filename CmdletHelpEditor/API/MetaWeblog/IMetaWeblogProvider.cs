using CookComputing.XmlRpc;
using System;
using System.ServiceModel;

namespace CmdletHelpEditor.API.MetaWeblog {
	[ServiceContract]
	public interface IMetaWeblogProvider {
		#region MetaWeblog API

		[XmlRpcMethod("metaWeblog.newPost")]
		[OperationContract(Action = "metaWeblog.newPost")]
		String AddPost(String blogid, String username, String password, Post post, Boolean publish);

		[XmlRpcMethod("metaWeblog.editPost")]
		Boolean UpdatePost(String postid, String username, String password, Post post, Boolean publish);

		[XmlRpcMethod("metaWeblog.getPost")]
		Post GetPost(String postid, String username, String password);

		[XmlRpcMethod("metaWeblog.getCategories")]
		CategoryInfo[] GetCategories(String blogid, String username, String password);

		[XmlRpcMethod("metaWeblog.getRecentPosts")]
		Post[] GetRecentPosts(String blogid, String username, String password, Int32 numberOfPosts);

		[XmlRpcMethod("metaWeblog.newMediaObject")]
		MediaObjectInfo NewMediaObject(String blogid, String username, String password, MediaObject mediaObject);

		#endregion

		#region Blogger API

		[XmlRpcMethod("blogger.deletePost")]
		[return: XmlRpcReturnValue(Description = "Returns true.")]
		Boolean DeletePost(String key, String postid, String username, String password, Boolean publish);

		[XmlRpcMethod("blogger.getUsersBlogs")]
		BlogInfo[] GetUsersBlogs(String key, String username, String password);

		[XmlRpcMethod("blogger.getUserInfo")]
		UserInfo GetUserInfo(String key, String username, String password);

		#endregion
	}
}
