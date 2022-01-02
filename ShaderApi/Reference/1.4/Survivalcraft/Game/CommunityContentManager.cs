using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200013D RID: 317
	public static class CommunityContentManager
	{
		// Token: 0x0600060D RID: 1549 RVA: 0x00022550 File Offset: 0x00020750
		public static void Initialize()
		{
			CommunityContentManager.Load();
			WorldsManager.WorldDeleted += delegate(string path)
			{
				CommunityContentManager.m_idToAddressMap.Remove(CommunityContentManager.MakeContentIdString(ExternalContentType.World, path));
			};
			BlocksTexturesManager.BlocksTextureDeleted += delegate(string path)
			{
				CommunityContentManager.m_idToAddressMap.Remove(CommunityContentManager.MakeContentIdString(ExternalContentType.BlocksTexture, path));
			};
			CharacterSkinsManager.CharacterSkinDeleted += delegate(string path)
			{
				CommunityContentManager.m_idToAddressMap.Remove(CommunityContentManager.MakeContentIdString(ExternalContentType.CharacterSkin, path));
			};
			FurniturePacksManager.FurniturePackDeleted += delegate(string path)
			{
				CommunityContentManager.m_idToAddressMap.Remove(CommunityContentManager.MakeContentIdString(ExternalContentType.FurniturePack, path));
			};
			Window.Deactivated += delegate()
			{
				CommunityContentManager.Save();
			};
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x00022618 File Offset: 0x00020818
		public static string GetDownloadedContentAddress(ExternalContentType type, string name)
		{
			string result;
			CommunityContentManager.m_idToAddressMap.TryGetValue(CommunityContentManager.MakeContentIdString(type, name), out result);
			return result;
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x0002263C File Offset: 0x0002083C
		public static bool IsContentRated(string address, string userId)
		{
			string key = CommunityContentManager.MakeFeedbackCacheKey(address, "Rating", userId);
			return CommunityContentManager.m_feedbackCache.ContainsKey(key);
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x00022664 File Offset: 0x00020864
		public static void List(string cursor, string userFilter, string typeFilter, string moderationFilter, string sortOrder, string keySearch, CancellableProgress progress, Action<List<CommunityContentEntry>, string> success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			if (!WebManager.IsInternetConnectionAvailable())
			{
				failure(new InvalidOperationException("Internet connection is unavailable."));
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2.Add("Content-Type", "application/x-www-form-urlencoded");
			dictionary.Add("Action", "list");
			dictionary.Add("Cursor", cursor ?? string.Empty);
			dictionary.Add("UserId", userFilter ?? string.Empty);
			dictionary.Add("Type", typeFilter ?? string.Empty);
			dictionary.Add("Moderation", moderationFilter ?? string.Empty);
			dictionary.Add("SortOrder", sortOrder ?? string.Empty);
			dictionary.Add("Platform", VersionsManager.Platform.ToString());
			dictionary.Add("Version", VersionsManager.Version);
			dictionary.Add("Apiv", 3.ToString());
			dictionary.Add("key", keySearch);
			WebManager.Post("https://m.schub.top/com/list", null, dictionary2, WebManager.UrlParametersToStream(dictionary), progress, delegate(byte[] result)
			{
				try
				{
					XElement xelement = XmlUtils.LoadXmlFromString(Encoding.UTF8.GetString(result, 0, result.Length), true);
					string attributeValue = XmlUtils.GetAttributeValue<string>(xelement, "NextCursor");
					List<CommunityContentEntry> list = new List<CommunityContentEntry>();
					foreach (XElement node in xelement.Elements())
					{
						try
						{
							list.Add(new CommunityContentEntry
							{
								Type = XmlUtils.GetAttributeValue<ExternalContentType>(node, "Type", ExternalContentType.Unknown),
								Name = XmlUtils.GetAttributeValue<string>(node, "Name"),
								Address = XmlUtils.GetAttributeValue<string>(node, "Url"),
								UserId = XmlUtils.GetAttributeValue<string>(node, "UserId"),
								Size = XmlUtils.GetAttributeValue<long>(node, "Size"),
								ExtraText = XmlUtils.GetAttributeValue<string>(node, "ExtraText", string.Empty),
								RatingsAverage = XmlUtils.GetAttributeValue<float>(node, "RatingsAverage", 0f)
							});
						}
						catch (Exception)
						{
						}
					}
					success(list, attributeValue);
				}
				catch (Exception obj)
				{
					failure(obj);
				}
			}, delegate(Exception error)
			{
				failure(error);
			});
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x000227C4 File Offset: 0x000209C4
		public static void Download(string address, string name, ExternalContentType type, string userId, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			if (!WebManager.IsInternetConnectionAvailable())
			{
				failure(new InvalidOperationException("Internet connection is unavailable."));
				return;
			}
			WebManager.Get(address, null, null, progress, delegate(byte[] data)
			{
				string hash = CommunityContentManager.CalculateContentHashString(data);
				ExternalContentManager.ImportExternalContent(new MemoryStream(data), type, name, delegate(string downloadedName)
				{
					CommunityContentManager.m_idToAddressMap[CommunityContentManager.MakeContentIdString(type, downloadedName)] = address;
					CommunityContentManager.Feedback(address, "Success", null, hash, (long)data.Length, userId, progress, delegate
					{
					}, delegate
					{
					});
					success();
				}, delegate(Exception error)
				{
					CommunityContentManager.Feedback(address, "ImportFailure", null, hash, (long)data.Length, userId, null, delegate
					{
					}, delegate
					{
					});
					failure(error);
				});
			}, delegate(Exception error)
			{
				CommunityContentManager.Feedback(address, "DownloadFailure", null, null, 0L, userId, null, delegate
				{
				}, delegate
				{
				});
				failure(error);
			});
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x00022868 File Offset: 0x00020A68
		public static void Publish(string address, string name, ExternalContentType type, string userId, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			if (MarketplaceManager.IsTrialMode)
			{
				failure(new InvalidOperationException("Cannot publish links in trial mode."));
				return;
			}
			if (!WebManager.IsInternetConnectionAvailable())
			{
				failure(new InvalidOperationException("Internet connection is unavailable."));
				return;
			}
			CommunityContentManager.VerifyLinkContent(address, name, type, progress, delegate(byte[] data)
			{
				string value = CommunityContentManager.CalculateContentHashString(data);
				WebManager.Post("https://m.schub.top/com/list", null, null, WebManager.UrlParametersToStream(new Dictionary<string, string>
				{
					{
						"Action",
						"publish"
					},
					{
						"UserId",
						userId
					},
					{
						"Name",
						name
					},
					{
						"Url",
						address
					},
					{
						"Type",
						type.ToString()
					},
					{
						"Hash",
						value
					},
					{
						"Size",
						data.Length.ToString(CultureInfo.InvariantCulture)
					},
					{
						"Platform",
						VersionsManager.Platform.ToString()
					},
					{
						"Version",
						VersionsManager.Version
					}
				}), progress, delegate
				{
					success();
				}, delegate(Exception error)
				{
					failure(error);
				});
			}, failure);
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x00022930 File Offset: 0x00020B30
		public static void Delete(string address, string userId, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			if (!WebManager.IsInternetConnectionAvailable())
			{
				failure(new InvalidOperationException("Internet connection is unavailable."));
				return;
			}
			WebManager.Post("https://m.schub.top/com/list", null, null, WebManager.UrlParametersToStream(new Dictionary<string, string>
			{
				{
					"Action",
					"delete"
				},
				{
					"UserId",
					userId
				},
				{
					"Url",
					address
				},
				{
					"Platform",
					VersionsManager.Platform.ToString()
				},
				{
					"Version",
					VersionsManager.Version
				}
			}), progress, delegate
			{
				success();
			}, delegate(Exception error)
			{
				failure(error);
			});
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00022A04 File Offset: 0x00020C04
		public static void Rate(string address, string userId, int rating, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			rating = MathUtils.Clamp(rating, 1, 5);
			CommunityContentManager.Feedback(address, "Rating", rating.ToString(CultureInfo.InvariantCulture), null, 0L, userId, progress, success, failure);
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00022A3C File Offset: 0x00020C3C
		public static void Report(string address, string userId, string report, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			CommunityContentManager.Feedback(address, "Report", report, null, 0L, userId, progress, success, failure);
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00022A60 File Offset: 0x00020C60
		public static void SendPlayTime(string address, string userId, double time, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			CommunityContentManager.Feedback(address, "PlayTime", MathUtils.Round(time).ToString(CultureInfo.InvariantCulture), null, 0L, userId, progress, success, failure);
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x00022A94 File Offset: 0x00020C94
		public static void VerifyLinkContent(string address, string name, ExternalContentType type, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			WebManager.Get(address, null, null, progress, delegate(byte[] data)
			{
				ExternalContentManager.ImportExternalContent(new MemoryStream(data), type, "__Temp", delegate(string downloadedName)
				{
					ExternalContentManager.DeleteExternalContent(type, downloadedName);
					success(data);
				}, failure);
			}, failure);
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00022AE8 File Offset: 0x00020CE8
		public static void Feedback(string address, string feedback, string feedbackParameter, string hash, long size, string userId, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			if (!WebManager.IsInternetConnectionAvailable())
			{
				failure(new InvalidOperationException("Internet connection is unavailable."));
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Action", "feedback");
			dictionary.Add("Feedback", feedback);
			if (feedbackParameter != null)
			{
				dictionary.Add("FeedbackParameter", feedbackParameter);
			}
			dictionary.Add("UserId", userId);
			if (address != null)
			{
				dictionary.Add("Url", address);
			}
			if (hash != null)
			{
				dictionary.Add("Hash", hash);
			}
			if (size > 0L)
			{
				dictionary.Add("Size", size.ToString(CultureInfo.InvariantCulture));
			}
			dictionary.Add("Platform", VersionsManager.Platform.ToString());
			dictionary.Add("Version", VersionsManager.Version);
			WebManager.Post("https://m.schub.top/com/list", null, null, WebManager.UrlParametersToStream(dictionary), progress, delegate
			{
				string key = CommunityContentManager.MakeFeedbackCacheKey(address, feedback, userId);
				if (CommunityContentManager.m_feedbackCache.ContainsKey(key))
				{
					Task.Run(delegate()
					{
						Task.Delay(1500).Wait();
						failure(new InvalidOperationException("Duplicate feedback."));
					});
					return;
				}
				CommunityContentManager.m_feedbackCache[key] = true;
				success();
			}, delegate(Exception error)
			{
				failure(error);
			});
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00022C34 File Offset: 0x00020E34
		public static string CalculateContentHashString(byte[] data)
		{
			string result;
			using (SHA1Managed sha1Managed = new SHA1Managed())
			{
				result = Convert.ToBase64String(sha1Managed.ComputeHash(data));
			}
			return result;
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x00022C74 File Offset: 0x00020E74
		public static string MakeFeedbackCacheKey(string address, string feedback, string userId)
		{
			return string.Concat(new string[]
			{
				address,
				"\n",
				feedback,
				"\n",
				userId
			});
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x00022C9D File Offset: 0x00020E9D
		public static string MakeContentIdString(ExternalContentType type, string name)
		{
			return type.ToString() + ":" + name;
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x00022CB8 File Offset: 0x00020EB8
		public static void Load()
		{
			try
			{
				if (Storage.FileExists("app:CommunityContentCache.xml"))
				{
					using (Stream stream = Storage.OpenFile("app:CommunityContentCache.xml", OpenFileMode.Read))
					{
						XElement xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
						foreach (XElement node in xelement.Element("Feedback").Elements())
						{
							string attributeValue = XmlUtils.GetAttributeValue<string>(node, "Key");
							CommunityContentManager.m_feedbackCache[attributeValue] = true;
						}
						foreach (XElement node2 in xelement.Element("Content").Elements())
						{
							string attributeValue2 = XmlUtils.GetAttributeValue<string>(node2, "Path");
							string attributeValue3 = XmlUtils.GetAttributeValue<string>(node2, "Address");
							CommunityContentManager.m_idToAddressMap[attributeValue2] = attributeValue3;
						}
					}
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Loading Community Content cache failed.", e);
			}
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x00022DE8 File Offset: 0x00020FE8
		public static void Save()
		{
			try
			{
				XElement xelement = new XElement("Cache");
				XElement xelement2 = new XElement("Feedback");
				xelement.Add(xelement2);
				foreach (string value in CommunityContentManager.m_feedbackCache.Keys)
				{
					XElement xelement3 = new XElement("Item");
					XmlUtils.SetAttributeValue(xelement3, "Key", value);
					xelement2.Add(xelement3);
				}
				XElement xelement4 = new XElement("Content");
				xelement.Add(xelement4);
				foreach (KeyValuePair<string, string> keyValuePair in CommunityContentManager.m_idToAddressMap)
				{
					XElement xelement5 = new XElement("Item");
					XmlUtils.SetAttributeValue(xelement5, "Path", keyValuePair.Key);
					XmlUtils.SetAttributeValue(xelement5, "Address", keyValuePair.Value);
					xelement4.Add(xelement5);
				}
				using (Stream stream = Storage.OpenFile("app:CommunityContentCache.xml", OpenFileMode.Create))
				{
					XmlUtils.SaveXmlToStream(xelement, stream, null, true);
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Saving Community Content cache failed.", e);
			}
		}

		// Token: 0x040002AD RID: 685
		public const string m_scResDirAddress = "https://m.schub.top/com/list";

		// Token: 0x040002AE RID: 686
		public static Dictionary<string, string> m_idToAddressMap = new Dictionary<string, string>();

		// Token: 0x040002AF RID: 687
		public static Dictionary<string, bool> m_feedbackCache = new Dictionary<string, bool>();
	}
}
