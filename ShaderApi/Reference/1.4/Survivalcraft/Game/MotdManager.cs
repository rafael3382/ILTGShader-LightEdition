using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Engine;
using SimpleJson;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000147 RID: 327
	public static class MotdManager
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000664 RID: 1636 RVA: 0x0002485E File Offset: 0x00022A5E
		// (set) Token: 0x06000665 RID: 1637 RVA: 0x00024865 File Offset: 0x00022A65
		public static MotdManager.Message MessageOfTheDay
		{
			get
			{
				return MotdManager.m_message;
			}
			set
			{
				MotdManager.m_message = value;
				Action messageOfTheDayUpdated = MotdManager.MessageOfTheDayUpdated;
				if (messageOfTheDayUpdated == null)
				{
					return;
				}
				messageOfTheDayUpdated();
			}
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000666 RID: 1638 RVA: 0x0002487C File Offset: 0x00022A7C
		// (remove) Token: 0x06000667 RID: 1639 RVA: 0x000248B0 File Offset: 0x00022AB0
		public static event Action MessageOfTheDayUpdated;

		// Token: 0x06000668 RID: 1640 RVA: 0x000248E3 File Offset: 0x00022AE3
		public static void ForceRedownload()
		{
			SettingsManager.MotdLastUpdateTime = DateTime.MinValue;
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x000248EF File Offset: 0x00022AEF
		public static void Initialize()
		{
			if (VersionsManager.Version != VersionsManager.LastLaunchedVersion)
			{
				MotdManager.ForceRedownload();
			}
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x00024908 File Offset: 0x00022B08
		public static void UpdateVersion()
		{
			WebManager.Get(string.Format(SettingsManager.MotdUpdateCheckUrl, new object[]
			{
				VersionsManager.SerializationVersion,
				VersionsManager.Platform,
				"1.40",
				LanguageControl.LName()
			}), null, null, new CancellableProgress(), delegate(byte[] data)
			{
				MotdManager.UpdateResult = SimpleJson.DeserializeObject<JsonObject>(Encoding.UTF8.GetString(data));
			}, delegate(Exception ex)
			{
				Log.Error("Failed processing Update check. Reason: " + ex.Message);
			});
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x00024994 File Offset: 0x00022B94
		public static void DownloadMotd()
		{
			Log.Information("Downloading MOTD");
			WebManager.Get(MotdManager.GetMotdUrl(), null, null, null, delegate(byte[] result)
			{
				try
				{
					string motdLastDownloadedData = MotdManager.UnpackMotd(result);
					MotdManager.MessageOfTheDay = null;
					SettingsManager.MotdLastDownloadedData = motdLastDownloadedData;
					Log.Information("Downloaded MOTD");
				}
				catch (Exception ex)
				{
					Log.Error("Failed processing MOTD string. Reason: " + ex.Message);
				}
			}, delegate(Exception error)
			{
				Log.Error("Failed downloading MOTD. Reason: {0}", new object[]
				{
					error.Message
				});
			});
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x000249F8 File Offset: 0x00022BF8
		public static void Update()
		{
			if (Time.PeriodicEvent(1.0, 0.0) && ModsManager.ConfigLoaded)
			{
				TimeSpan t = TimeSpan.FromHours(SettingsManager.MotdUpdatePeriodHours);
				DateTime now = DateTime.Now;
				if (now >= SettingsManager.MotdLastUpdateTime + t)
				{
					SettingsManager.MotdLastUpdateTime = now;
					MotdManager.DownloadMotd();
					MotdManager.UpdateVersion();
				}
			}
			if (MotdManager.MessageOfTheDay == null && !string.IsNullOrEmpty(SettingsManager.MotdLastDownloadedData))
			{
				MotdManager.MessageOfTheDay = MotdManager.ParseMotd(SettingsManager.MotdLastDownloadedData);
				if (MotdManager.MessageOfTheDay == null)
				{
					SettingsManager.MotdLastDownloadedData = string.Empty;
				}
			}
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x00024A8C File Offset: 0x00022C8C
		public static string UnpackMotd(byte[] data)
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				result = new StreamReader(memoryStream).ReadToEnd();
			}
			return result;
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x00024ACC File Offset: 0x00022CCC
		public static MotdManager.Message ParseMotd(string dataString)
		{
			try
			{
				int num = dataString.IndexOf("<Motd");
				if (num < 0)
				{
					throw new InvalidOperationException("Invalid MOTD data string.");
				}
				int num2 = dataString.IndexOf("</Motd>");
				if (num2 >= 0 && num2 > num)
				{
					num2 += 7;
				}
				XElement xelement = XmlUtils.LoadXmlFromString(dataString.Substring(num, num2 - num), true);
				SettingsManager.MotdUpdatePeriodHours = (double)XmlUtils.GetAttributeValue<int>(xelement, "UpdatePeriodHours", 24);
				SettingsManager.MotdUpdateUrl = XmlUtils.GetAttributeValue<string>(xelement, "UpdateUrl", SettingsManager.MotdUpdateUrl);
				MotdManager.Message message = new MotdManager.Message();
				foreach (XElement xelement2 in xelement.Elements())
				{
					if (Widget.IsNodeIncludedOnCurrentPlatform(xelement2))
					{
						MotdManager.Line item = new MotdManager.Line
						{
							Time = XmlUtils.GetAttributeValue<float>(xelement2, "Time"),
							Node = xelement2.Elements().FirstOrDefault<XElement>(),
							Text = xelement2.Value
						};
						message.Lines.Add(item);
					}
				}
				return message;
			}
			catch (Exception ex)
			{
				Log.Warning("Failed extracting MOTD string. Reason: " + ex.Message);
			}
			return null;
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x00024C00 File Offset: 0x00022E00
		public static string GetMotdUrl()
		{
			return string.Format(SettingsManager.MotdUpdateUrl, VersionsManager.SerializationVersion, ModsManager.Configs["Language"]);
		}

		// Token: 0x040002C6 RID: 710
		public static MotdManager.Message m_message;

		// Token: 0x040002C7 RID: 711
		public static JsonObject UpdateResult;

		// Token: 0x02000434 RID: 1076
		public class Message
		{
			// Token: 0x04001592 RID: 5522
			public List<MotdManager.Line> Lines = new List<MotdManager.Line>();
		}

		// Token: 0x02000435 RID: 1077
		public class Line
		{
			// Token: 0x04001593 RID: 5523
			public float Time;

			// Token: 0x04001594 RID: 5524
			public XElement Node;

			// Token: 0x04001595 RID: 5525
			public string Text;
		}
	}
}
