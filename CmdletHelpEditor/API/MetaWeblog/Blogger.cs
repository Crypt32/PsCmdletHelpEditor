using System.Collections.Generic;
using System.Security;
using CmdletHelpEditor.API.Tools;
using CookComputing.XmlRpc;
using System;
using System.Windows;

namespace CmdletHelpEditor.API.MetaWeblog {
	public class Blogger {
		String _blogId;
		readonly String _userName;
		readonly SecureString _password;
		readonly IMetaWeblogProvider metaWeblogProvider;
		readonly XmlRpcClientProtocol clientProtocol;

		public Blogger(String url, String userName, SecureString password) {
			_userName = userName;
			_password = password;

			metaWeblogProvider = (IMetaWeblogProvider)XmlRpcProxyGen.Create(typeof(IMetaWeblogProvider));
			clientProtocol = (XmlRpcClientProtocol)metaWeblogProvider;
			clientProtocol.Url = url;
            clientProtocol.UserAgent = "PS Cmdlet Help Editor";
			clientProtocol.NonStandard = XmlRpcNonStandard.All;
		}

		public void SetBlog(String blogId) {
			_blogId = blogId;
		}
		public IEnumerable<BlogInfo> GetUsersBlogs() {
			try {
				return metaWeblogProvider.GetUsersBlogs(String.Empty, _userName, Crypt.SecureStringToString(_password));
			} catch (Exception e) {
				Utils.MsgBox("Error", e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			return null;
		}
		public List<Post> GetRecentPosts(Int32 postCount = 5) {
			try {
				return new List<Post>(metaWeblogProvider.GetRecentPosts(_blogId, _userName, Crypt.SecureStringToString(_password), postCount));
			} catch (Exception e) {
				Utils.MsgBox("Error", e.Message);
			}
			return null;
		}
		public Post GetPost(String postId) {
			try {
				return metaWeblogProvider.GetPost(postId, _userName, Crypt.SecureStringToString(_password));
			} catch (Exception e) {
				Utils.MsgBox("Error", e.Message);
			}
			return null;
		}
		public String AddPost(Post post, Boolean publish = true) {
            return metaWeblogProvider.AddPost(_blogId, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
        public Boolean UpdatePost(Post post, Boolean publish = true) {
            return metaWeblogProvider.UpdatePost(post.PostId, _userName, Crypt.SecureStringToString(_password), post, publish);
        }
		public Boolean DeletePost(String postId, Boolean publish = false) {
			try {
				return metaWeblogProvider.DeletePost(String.Empty, postId, _userName, Crypt.SecureStringToString(_password), publish);
			} catch (Exception e) {
				Utils.MsgBox("Error", e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			return false;
		}
	}
}