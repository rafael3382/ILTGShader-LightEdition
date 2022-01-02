using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Engine;
using SimpleJson;

namespace Game
{
	// Token: 0x02000275 RID: 629
	public class DropboxExternalContentProvider : IExternalContentProvider, IDisposable
	{
		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x00096554 File Offset: 0x00094754
		public string DisplayName
		{
			get
			{
				return "Dropbox";
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06001426 RID: 5158 RVA: 0x0009655B File Offset: 0x0009475B
		public string Description
		{
			get
			{
				if (!this.IsLoggedIn)
				{
					return "Not logged in";
				}
				return "Logged in";
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06001427 RID: 5159 RVA: 0x00096570 File Offset: 0x00094770
		public bool SupportsListing
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06001428 RID: 5160 RVA: 0x00096573 File Offset: 0x00094773
		public bool SupportsLinks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06001429 RID: 5161 RVA: 0x00096576 File Offset: 0x00094776
		public bool RequiresLogin
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x0600142A RID: 5162 RVA: 0x00096579 File Offset: 0x00094779
		public bool IsLoggedIn
		{
			get
			{
				return !string.IsNullOrEmpty(SettingsManager.DropboxAccessToken);
			}
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x00096588 File Offset: 0x00094788
		public DropboxExternalContentProvider()
		{
			Program.HandleUri += this.HandleUri;
			Window.Activated += this.WindowActivated;
		}

		// Token: 0x0600142C RID: 5164 RVA: 0x000965B2 File Offset: 0x000947B2
		public void Dispose()
		{
			Program.HandleUri -= this.HandleUri;
			Window.Activated -= this.WindowActivated;
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x000965D8 File Offset: 0x000947D8
		public void Login(CancellableProgress progress, Action success, Action<Exception> failure)
		{
			try
			{
				if (this.m_loginProcessData != null)
				{
					throw new InvalidOperationException("Login already in progress.");
				}
				if (!WebManager.IsInternetConnectionAvailable())
				{
					throw new InvalidOperationException("Internet connection is unavailable.");
				}
				this.Logout();
				progress.Cancelled += delegate()
				{
					if (this.m_loginProcessData != null)
					{
						DropboxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
						this.m_loginProcessData = null;
						loginProcessData.Fail(this, null);
					}
				};
				this.m_loginProcessData = new DropboxExternalContentProvider.LoginProcessData();
				this.m_loginProcessData.Progress = progress;
				this.m_loginProcessData.Success = success;
				this.m_loginProcessData.Failure = failure;
				this.LoginLaunchBrowser();
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x00096674 File Offset: 0x00094874
		public void Logout()
		{
			SettingsManager.DropboxAccessToken = string.Empty;
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x00096680 File Offset: 0x00094880
		public void List(string path, CancellableProgress progress, Action<ExternalContentEntry> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Authorization", "Bearer " + SettingsManager.DropboxAccessToken);
				dictionary.Add("Content-Type", "application/json");
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", DropboxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("recursive", false);
				jsonObject.Add("include_media_info", false);
				jsonObject.Add("include_deleted", false);
				jsonObject.Add("include_has_explicit_shared_members", false);
				MemoryStream data = new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));
				WebManager.Post("https://api.dropboxapi.com/2/files/list_folder", null, dictionary, data, progress, delegate(byte[] result)
				{
					try
					{
						JsonObject jsonObject2 = (JsonObject)WebManager.JsonFromBytes(result);
						success(DropboxExternalContentProvider.JsonObjectToEntry(jsonObject2));
					}
					catch (Exception obj2)
					{
						failure(obj2);
					}
				}, delegate(Exception error)
				{
					failure(error);
				});
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x00096794 File Offset: 0x00094994
		public void Download(string path, CancellableProgress progress, Action<Stream> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", DropboxExternalContentProvider.NormalizePath(path));
				WebManager.Get("https://content.dropboxapi.com/2/files/download", null, new Dictionary<string, string>
				{
					{
						"Authorization",
						"Bearer " + SettingsManager.DropboxAccessToken
					},
					{
						"Dropbox-API-Arg",
						jsonObject.ToString()
					}
				}, progress, delegate(byte[] result)
				{
					success(new MemoryStream(result));
				}, delegate(Exception error)
				{
					failure(error);
				});
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x0009684C File Offset: 0x00094A4C
		public void Upload(string path, Stream stream, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", DropboxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("mode", "add");
				jsonObject.Add("autorename", true);
				jsonObject.Add("mute", false);
				WebManager.Post("https://content.dropboxapi.com/2/files/upload", null, new Dictionary<string, string>
				{
					{
						"Authorization",
						"Bearer " + SettingsManager.DropboxAccessToken
					},
					{
						"Content-Type",
						"application/octet-stream"
					},
					{
						"Dropbox-API-Arg",
						jsonObject.ToString()
					}
				}, stream, progress, delegate
				{
					success(null);
				}, delegate(Exception error)
				{
					failure(error);
				});
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x00096948 File Offset: 0x00094B48
		public void Link(string path, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Authorization", "Bearer " + SettingsManager.DropboxAccessToken);
				dictionary.Add("Content-Type", "application/json");
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", DropboxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("short_url", false);
				MemoryStream data = new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));
				WebManager.Post("https://api.dropboxapi.com/2/sharing/create_shared_link", null, dictionary, data, progress, delegate(byte[] result)
				{
					try
					{
						JsonObject jsonObject2 = (JsonObject)WebManager.JsonFromBytes(result);
						success(DropboxExternalContentProvider.JsonObjectToLinkAddress(jsonObject2));
					}
					catch (Exception obj2)
					{
						failure(obj2);
					}
				}, delegate(Exception error)
				{
					failure(error);
				});
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x00096A28 File Offset: 0x00094C28
		public void LoginLaunchBrowser()
		{
			try
			{
				this.m_loginProcessData.IsTokenFlow = true;
				WebBrowserManager.LaunchBrowser("https://www.dropbox.com/oauth2/authorize?" + WebManager.UrlParametersToString(new Dictionary<string, string>
				{
					{
						"response_type",
						"token"
					},
					{
						"client_id",
						"1unnzwkb8igx70k"
					},
					{
						"redirect_uri",
						"com.candyrufusgames.survivalcraft2://redirect"
					}
				}));
			}
			catch (Exception error)
			{
				this.m_loginProcessData.Fail(this, error);
			}
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x00096AB0 File Offset: 0x00094CB0
		public void WindowActivated()
		{
			if (this.m_loginProcessData != null && !this.m_loginProcessData.IsTokenFlow)
			{
				DropboxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
				this.m_loginProcessData = null;
				TextBoxDialog dialog = new TextBoxDialog("Enter Dropbox authorization code", "", 256, delegate(string s)
				{
					if (s != null)
					{
						try
						{
							WebManager.Post("https://api.dropboxapi.com/oauth2/token", new Dictionary<string, string>
							{
								{
									"code",
									s.Trim()
								},
								{
									"client_id",
									"1unnzwkb8igx70k"
								},
								{
									"client_secret",
									"3i5u3j3141php7u"
								},
								{
									"grant_type",
									"authorization_code"
								}
							}, null, new MemoryStream(), loginProcessData.Progress, delegate(byte[] result)
							{
								SettingsManager.DropboxAccessToken = ((IDictionary<string, object>)WebManager.JsonFromBytes(result))["access_token"].ToString();
								loginProcessData.Succeed(this);
							}, delegate(Exception error)
							{
								loginProcessData.Fail(this, error);
							});
							return;
						}
						catch (Exception error)
						{
							Exception error2;
							loginProcessData.Fail(this, error2);
							return;
						}
					}
					loginProcessData.Fail(this, null);
				});
				DialogsManager.ShowDialog(null, dialog);
			}
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x00096B1C File Offset: 0x00094D1C
		public void HandleUri(Uri uri)
		{
			if (this.m_loginProcessData == null)
			{
				this.m_loginProcessData = new DropboxExternalContentProvider.LoginProcessData();
				this.m_loginProcessData.IsTokenFlow = true;
			}
			DropboxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
			this.m_loginProcessData = null;
			if (loginProcessData.IsTokenFlow)
			{
				try
				{
					if (uri != null && !string.IsNullOrEmpty(uri.Fragment))
					{
						Dictionary<string, string> dictionary = WebManager.UrlParametersFromString(uri.Fragment.TrimStart(new char[]
						{
							'#'
						}));
						if (dictionary.ContainsKey("access_token"))
						{
							SettingsManager.DropboxAccessToken = dictionary["access_token"];
							loginProcessData.Succeed(this);
							return;
						}
						if (dictionary.ContainsKey("error"))
						{
							throw new Exception(dictionary["error"]);
						}
					}
					throw new Exception("Could not retrieve Dropbox access token.");
				}
				catch (Exception error)
				{
					loginProcessData.Fail(this, error);
				}
			}
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x00096C00 File Offset: 0x00094E00
		public void VerifyLoggedIn()
		{
			if (!this.IsLoggedIn)
			{
				throw new InvalidOperationException("Not logged in to Dropbox in this app.");
			}
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x00096C18 File Offset: 0x00094E18
		public static ExternalContentEntry JsonObjectToEntry(JsonObject jsonObject)
		{
			ExternalContentEntry externalContentEntry = new ExternalContentEntry();
			if (jsonObject.ContainsKey("entries"))
			{
				foreach (object obj in ((JsonArray)jsonObject["entries"]))
				{
					JsonObject jsonObject2 = (JsonObject)obj;
					ExternalContentEntry externalContentEntry2 = new ExternalContentEntry();
					externalContentEntry2.Path = jsonObject2["path_display"].ToString();
					externalContentEntry2.Type = ((jsonObject2[".tag"].ToString() == "folder") ? ExternalContentType.Directory : ExternalContentManager.ExtensionToType(Storage.GetExtension(externalContentEntry2.Path)));
					if (externalContentEntry2.Type != ExternalContentType.Directory)
					{
						externalContentEntry2.Time = (jsonObject2.ContainsKey("server_modified") ? DateTime.Parse(jsonObject2["server_modified"].ToString(), CultureInfo.InvariantCulture) : new DateTime(2000, 1, 1));
						externalContentEntry2.Size = (jsonObject2.ContainsKey("size") ? ((long)jsonObject2["size"]) : 0L);
					}
					externalContentEntry.ChildEntries.Add(externalContentEntry2);
				}
				return externalContentEntry;
			}
			return externalContentEntry;
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x00096D5C File Offset: 0x00094F5C
		public static string JsonObjectToLinkAddress(JsonObject jsonObject)
		{
			if (jsonObject.ContainsKey("url"))
			{
				return jsonObject["url"].ToString().Replace("www.dropbox.", "dl.dropbox.").Replace("?dl=0", "") + "?dl=1";
			}
			throw new InvalidOperationException("Share information not found.");
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x00096DB9 File Offset: 0x00094FB9
		public static string NormalizePath(string path)
		{
			if (path == "/")
			{
				return string.Empty;
			}
			if (path.Length > 0 && path[0] != '/')
			{
				return "/" + path;
			}
			return path;
		}

		// Token: 0x04000CC4 RID: 3268
		public const string m_appKey = "1unnzwkb8igx70k";

		// Token: 0x04000CC5 RID: 3269
		public const string m_appSecret = "3i5u3j3141php7u";

		// Token: 0x04000CC6 RID: 3270
		public const string m_redirectUri = "com.candyrufusgames.survivalcraft2://redirect";

		// Token: 0x04000CC7 RID: 3271
		public DropboxExternalContentProvider.LoginProcessData m_loginProcessData;

		// Token: 0x02000502 RID: 1282
		public class LoginProcessData
		{
			// Token: 0x0600219F RID: 8607 RVA: 0x000EADB1 File Offset: 0x000E8FB1
			public void Succeed(DropboxExternalContentProvider provider)
			{
				provider.m_loginProcessData = null;
				Action success = this.Success;
				if (success == null)
				{
					return;
				}
				success();
			}

			// Token: 0x060021A0 RID: 8608 RVA: 0x000EADCA File Offset: 0x000E8FCA
			public void Fail(DropboxExternalContentProvider provider, Exception error)
			{
				provider.m_loginProcessData = null;
				Action<Exception> failure = this.Failure;
				if (failure == null)
				{
					return;
				}
				failure(error);
			}

			// Token: 0x0400182C RID: 6188
			public bool IsTokenFlow;

			// Token: 0x0400182D RID: 6189
			public Action Success;

			// Token: 0x0400182E RID: 6190
			public Action<Exception> Failure;

			// Token: 0x0400182F RID: 6191
			public CancellableProgress Progress;
		}
	}
}
