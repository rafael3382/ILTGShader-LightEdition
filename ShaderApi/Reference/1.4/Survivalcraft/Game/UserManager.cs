using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200014E RID: 334
	public static class UserManager
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600070D RID: 1805 RVA: 0x00026E5F File Offset: 0x0002505F
		// (set) Token: 0x0600070E RID: 1806 RVA: 0x00026E79 File Offset: 0x00025079
		public static UserInfo ActiveUser
		{
			get
			{
				return UserManager.GetUser(SettingsManager.UserId) ?? UserManager.GetUsers().FirstOrDefault<UserInfo>();
			}
			set
			{
				SettingsManager.UserId = ((value != null) ? value.UniqueId : string.Empty);
			}
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x00026E90 File Offset: 0x00025090
		static UserManager()
		{
			string text;
			try
			{
				string path = "app:/UserId.dat";
				if (!Storage.FileExists(path))
				{
					text = Guid.NewGuid().ToString();
					Storage.WriteAllText(path, text);
				}
				else
				{
					text = Storage.ReadAllText(path);
				}
			}
			catch (Exception)
			{
				text = Guid.NewGuid().ToString();
			}
			UserManager.m_users.Add(new UserInfo(text.ToString(), "Windows User"));
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x00026F1C File Offset: 0x0002511C
		public static IEnumerable<UserInfo> GetUsers()
		{
			return new ReadOnlyList<UserInfo>(UserManager.m_users);
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x00026F30 File Offset: 0x00025130
		public static UserInfo GetUser(string uniqueId)
		{
			return UserManager.GetUsers().FirstOrDefault((UserInfo u) => u.UniqueId == uniqueId);
		}

		// Token: 0x04000321 RID: 801
		public static List<UserInfo> m_users = new List<UserInfo>();
	}
}
