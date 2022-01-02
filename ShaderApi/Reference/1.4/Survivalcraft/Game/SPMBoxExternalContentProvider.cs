using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Engine;
using SimpleJson;

namespace Game
{
	// Token: 0x0200023A RID: 570
	public class SPMBoxExternalContentProvider : IExternalContentProvider, IDisposable
	{
		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06001293 RID: 4755 RVA: 0x0008A42F File Offset: 0x0008862F
		public string DisplayName
		{
			get
			{
				return "SPMBox中国社区";
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06001294 RID: 4756 RVA: 0x0008A436 File Offset: 0x00088636
		public string Description
		{
			get
			{
				if (!this.IsLoggedIn)
				{
					return "未登录";
				}
				return "登陆";
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06001295 RID: 4757 RVA: 0x0008A44B File Offset: 0x0008864B
		public bool SupportsListing
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06001296 RID: 4758 RVA: 0x0008A44E File Offset: 0x0008864E
		public bool SupportsLinks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06001297 RID: 4759 RVA: 0x0008A451 File Offset: 0x00088651
		public bool RequiresLogin
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06001298 RID: 4760 RVA: 0x0008A454 File Offset: 0x00088654
		public bool IsLoggedIn
		{
			get
			{
				return !string.IsNullOrEmpty(SettingsManager.ScpboxAccessToken);
			}
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x0008A463 File Offset: 0x00088663
		public SPMBoxExternalContentProvider()
		{
			Program.HandleUri += this.HandleUri;
			Window.Activated += this.WindowActivated;
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x0008A48D File Offset: 0x0008868D
		public void Dispose()
		{
			Program.HandleUri -= this.HandleUri;
			Window.Activated -= this.WindowActivated;
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x0008A4B4 File Offset: 0x000886B4
		public void Login(CancellableProgress progress, Action success, Action<Exception> failure)
		{
			try
			{
				if (this.m_loginProcessData != null)
				{
					throw new InvalidOperationException("登陆已经在进程中");
				}
				if (!WebManager.IsInternetConnectionAvailable())
				{
					throw new InvalidOperationException("网络连接错误");
				}
				this.Logout();
				progress.Cancelled += delegate()
				{
					if (this.m_loginProcessData != null)
					{
						SPMBoxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
						this.m_loginProcessData = null;
						loginProcessData.Fail(this, null);
					}
				};
				this.m_loginProcessData = new SPMBoxExternalContentProvider.LoginProcessData();
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

		// Token: 0x0600129C RID: 4764 RVA: 0x0008A550 File Offset: 0x00088750
		public void Logout()
		{
			this.m_loginProcessData = null;
			SettingsManager.ScpboxAccessToken = string.Empty;
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x0008A564 File Offset: 0x00088764
		public void List(string path, CancellableProgress progress, Action<ExternalContentEntry> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Authorization", "Bearer " + SettingsManager.ScpboxAccessToken);
				dictionary.Add("Content-Type", "application/json");
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", SPMBoxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("recursive", false);
				jsonObject.Add("include_media_info", false);
				jsonObject.Add("include_deleted", false);
				jsonObject.Add("include_has_explicit_shared_members", false);
				MemoryStream data = new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));
				WebManager.Post("https://m.schub.top/com/files/list_folder", null, dictionary, data, progress, delegate(byte[] result)
				{
					try
					{
						JsonObject jsonObject2 = (JsonObject)WebManager.JsonFromBytes(result);
						success(SPMBoxExternalContentProvider.JsonObjectToEntry(jsonObject2));
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

		// Token: 0x0600129E RID: 4766 RVA: 0x0008A678 File Offset: 0x00088878
		public void Download(string path, CancellableProgress progress, Action<Stream> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", SPMBoxExternalContentProvider.NormalizePath(path));
				WebManager.Get("https://m.schub.top/com/files/download", null, new Dictionary<string, string>
				{
					{
						"Authorization",
						"Bearer " + SettingsManager.ScpboxAccessToken
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

		// Token: 0x0600129F RID: 4767 RVA: 0x0008A730 File Offset: 0x00088930
		public void Upload(string path, Stream stream, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", SPMBoxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("mode", "add");
				jsonObject.Add("autorename", true);
				jsonObject.Add("mute", false);
				WebManager.Post("https://m.schub.top/com/files/upload", null, new Dictionary<string, string>
				{
					{
						"Authorization",
						"Bearer " + SettingsManager.ScpboxAccessToken
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

		// Token: 0x060012A0 RID: 4768 RVA: 0x0008A82C File Offset: 0x00088A2C
		public void Link(string path, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Authorization", "Bearer " + SettingsManager.ScpboxAccessToken);
				dictionary.Add("Content-Type", "application/json");
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", SPMBoxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("short_url", false);
				MemoryStream data = new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));
				WebManager.Post("https://m.schub.top/com/sharing/create_shared_link", null, dictionary, data, progress, delegate(byte[] result)
				{
					try
					{
						JsonObject jsonObject2 = (JsonObject)WebManager.JsonFromBytes(result);
						success(SPMBoxExternalContentProvider.JsonObjectToLinkAddress(jsonObject2));
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

		// Token: 0x060012A1 RID: 4769 RVA: 0x0008A90C File Offset: 0x00088B0C
		public void LoginLaunchBrowser()
		{
			try
			{
				LoginDialog login = new LoginDialog();
				login.succ = delegate(byte[] a)
				{
					JsonObject jsonObject = (JsonObject)SimpleJson.DeserializeObject(new StreamReader(new MemoryStream(a)).ReadToEnd());
					int num = int.Parse(jsonObject["code"].ToString());
					string text = jsonObject["msg"].ToString();
					if (num == 200)
					{
						SettingsManager.ScpboxAccessToken = ((JsonObject)jsonObject["data"])["accessToken"].ToString();
						DialogsManager.HideAllDialogs();
						return;
					}
					login.tip.Text = text;
				};
				login.fail = delegate(Exception e)
				{
					login.tip.Text = e.ToString();
				};
				DialogsManager.ShowDialog(null, login);
			}
			catch (Exception error)
			{
				this.m_loginProcessData.Fail(this, error);
			}
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x0008A988 File Offset: 0x00088B88
		public void WindowActivated()
		{
			if (this.m_loginProcessData != null && !this.m_loginProcessData.IsTokenFlow)
			{
				SPMBoxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
				this.m_loginProcessData = null;
				TextBoxDialog dialog = new TextBoxDialog("输入用户登录Token:", "", 256, delegate(string s)
				{
					if (s != null)
					{
						try
						{
							WebManager.Post("https://m.schub.top/com/oauth2/token", new Dictionary<string, string>
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
								SettingsManager.ScpboxAccessToken = ((IDictionary<string, object>)WebManager.JsonFromBytes(result))["access_token"].ToString();
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

		// Token: 0x060012A3 RID: 4771 RVA: 0x0008A9F4 File Offset: 0x00088BF4
		public void HandleUri(Uri uri)
		{
			if (this.m_loginProcessData == null)
			{
				this.m_loginProcessData = new SPMBoxExternalContentProvider.LoginProcessData();
				this.m_loginProcessData.IsTokenFlow = true;
			}
			SPMBoxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
			this.m_loginProcessData = null;
			if (loginProcessData.IsTokenFlow)
			{
				try
				{
					if (!(uri != null) || string.IsNullOrEmpty(uri.Fragment))
					{
						throw new Exception("不能接收来自SPMBox的身份验证信息");
					}
					Dictionary<string, string> dictionary = WebManager.UrlParametersFromString(uri.Fragment.TrimStart(new char[]
					{
						'#'
					}));
					if (!dictionary.ContainsKey("access_token"))
					{
						if (dictionary.ContainsKey("error"))
						{
							throw new Exception(dictionary["error"]);
						}
						throw new Exception("不能接收来自SPMBox的身份验证信息");
					}
					else
					{
						SettingsManager.ScpboxAccessToken = dictionary["access_token"];
						loginProcessData.Succeed(this);
					}
				}
				catch (Exception error)
				{
					loginProcessData.Fail(this, error);
				}
			}
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x0008AAE0 File Offset: 0x00088CE0
		public void VerifyLoggedIn()
		{
			if (!this.IsLoggedIn)
			{
				throw new InvalidOperationException("这个应用未登录到SPMBox中国社区");
			}
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x0008AAF8 File Offset: 0x00088CF8
		internal static ExternalContentEntry JsonObjectToEntry(JsonObject jsonObject)
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

		// Token: 0x060012A6 RID: 4774 RVA: 0x0008AC3C File Offset: 0x00088E3C
		internal static string JsonObjectToLinkAddress(JsonObject jsonObject)
		{
			if (jsonObject.ContainsKey("url"))
			{
				return jsonObject["url"].ToString();
			}
			throw new InvalidOperationException("没有分享链接信息");
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x0008AC66 File Offset: 0x00088E66
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

		// Token: 0x04000B73 RID: 2931
		public const string m_appKey = "1uGA5aADX43p";

		// Token: 0x04000B74 RID: 2932
		public const string m_appSecret = "9aux67wg5z";

		// Token: 0x04000B75 RID: 2933
		public const string m_redirectUri = "https://m.schub.top";

		// Token: 0x04000B76 RID: 2934
		public SPMBoxExternalContentProvider.LoginProcessData m_loginProcessData;

		// Token: 0x020004EA RID: 1258
		public class LoginProcessData
		{
			// Token: 0x06002157 RID: 8535 RVA: 0x000EA64E File Offset: 0x000E884E
			public void Succeed(SPMBoxExternalContentProvider provider)
			{
				provider.m_loginProcessData = null;
				Action success = this.Success;
				if (success == null)
				{
					return;
				}
				success();
			}

			// Token: 0x06002158 RID: 8536 RVA: 0x000EA667 File Offset: 0x000E8867
			public void Fail(SPMBoxExternalContentProvider provider, Exception error)
			{
				provider.m_loginProcessData = null;
				Action<Exception> failure = this.Failure;
				if (failure == null)
				{
					return;
				}
				failure(error);
			}

			// Token: 0x040017E8 RID: 6120
			public bool IsTokenFlow;

			// Token: 0x040017E9 RID: 6121
			public Action Success;

			// Token: 0x040017EA RID: 6122
			public Action<Exception> Failure;

			// Token: 0x040017EB RID: 6123
			public CancellableProgress Progress;
		}
	}
}
