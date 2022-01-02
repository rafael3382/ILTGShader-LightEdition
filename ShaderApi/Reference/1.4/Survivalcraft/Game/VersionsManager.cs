using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Engine.Serialization;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200014F RID: 335
	public static class VersionsManager
	{
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000712 RID: 1810 RVA: 0x00026F60 File Offset: 0x00025160
		public static Platform Platform
		{
			get
			{
				return Platform.Desktop;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000713 RID: 1811 RVA: 0x00026F63 File Offset: 0x00025163
		public static BuildConfiguration BuildConfiguration
		{
			get
			{
				return BuildConfiguration.Release;
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000714 RID: 1812 RVA: 0x00026F66 File Offset: 0x00025166
		// (set) Token: 0x06000715 RID: 1813 RVA: 0x00026F6D File Offset: 0x0002516D
		public static string Version { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000716 RID: 1814 RVA: 0x00026F75 File Offset: 0x00025175
		// (set) Token: 0x06000717 RID: 1815 RVA: 0x00026F7C File Offset: 0x0002517C
		public static string SerializationVersion { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000718 RID: 1816 RVA: 0x00026F84 File Offset: 0x00025184
		// (set) Token: 0x06000719 RID: 1817 RVA: 0x00026F8B File Offset: 0x0002518B
		public static string LastLaunchedVersion { get; set; }

		// Token: 0x0600071A RID: 1818 RVA: 0x00026F94 File Offset: 0x00025194
		static VersionsManager()
		{
			AssemblyName assemblyName = new AssemblyName(typeof(VersionsManager).GetTypeInfo().Assembly.FullName);
			VersionsManager.Version = string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				assemblyName.Version.Major,
				assemblyName.Version.Minor,
				assemblyName.Version.Build,
				assemblyName.Version.Revision
			});
			VersionsManager.SerializationVersion = string.Format("{0}.{1}", assemblyName.Version.Major, assemblyName.Version.Minor);
			Assembly[] array = TypeCache.LoadedAssemblies.ToArray<Assembly>();
			for (int i = 0; i < array.Length; i++)
			{
				foreach (TypeInfo typeInfo in array[i].DefinedTypes)
				{
					if (!typeInfo.IsAbstract && !typeInfo.IsInterface && typeof(VersionConverter).GetTypeInfo().IsAssignableFrom(typeInfo))
					{
						VersionConverter item = (VersionConverter)Activator.CreateInstance(typeInfo.AsType());
						VersionsManager.m_versionConverters.Add(item);
					}
				}
			}
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00027100 File Offset: 0x00025300
		public static void Initialize()
		{
			VersionsManager.LastLaunchedVersion = SettingsManager.LastLaunchedVersion;
			SettingsManager.LastLaunchedVersion = VersionsManager.Version;
			VersionsManager.Version != VersionsManager.LastLaunchedVersion;
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x00027128 File Offset: 0x00025328
		public static void UpgradeProjectXml(XElement projectNode)
		{
			string attributeValue = XmlUtils.GetAttributeValue<string>(projectNode, "Version", "1.0");
			if (attributeValue != VersionsManager.SerializationVersion)
			{
				List<VersionConverter> list = VersionsManager.FindTransform(attributeValue, VersionsManager.SerializationVersion, VersionsManager.m_versionConverters, 0);
				if (list == null)
				{
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Cannot find conversion path from version \"",
						attributeValue,
						"\" to version \"",
						VersionsManager.SerializationVersion,
						"\""
					}));
				}
				foreach (VersionConverter versionConverter in list)
				{
					Log.Information(string.Concat(new string[]
					{
						"Upgrading world version \"",
						versionConverter.SourceVersion,
						"\" to \"",
						versionConverter.TargetVersion,
						"\"."
					}));
					versionConverter.ConvertProjectXml(projectNode);
				}
				string attributeValue2 = XmlUtils.GetAttributeValue<string>(projectNode, "Version", "1.0");
				if (attributeValue2 != VersionsManager.SerializationVersion)
				{
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Upgrade produced invalid project version. Expected \"",
						VersionsManager.SerializationVersion,
						"\", found \"",
						attributeValue2,
						"\"."
					}));
				}
			}
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x00027270 File Offset: 0x00025470
		public static void UpgradeWorld(string directoryName)
		{
			WorldInfo worldInfo = WorldsManager.GetWorldInfo(directoryName);
			if (worldInfo == null)
			{
				throw new InvalidOperationException("Cannot determine version of world at \"" + directoryName + "\"");
			}
			if (worldInfo.SerializationVersion != VersionsManager.SerializationVersion)
			{
				ProgressManager.UpdateProgress("Upgrading World To " + VersionsManager.SerializationVersion, 0f);
				List<VersionConverter> list = VersionsManager.FindTransform(worldInfo.SerializationVersion, VersionsManager.SerializationVersion, VersionsManager.m_versionConverters, 0);
				if (list == null)
				{
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Cannot find conversion path from version \"",
						worldInfo.SerializationVersion,
						"\" to version \"",
						VersionsManager.SerializationVersion,
						"\""
					}));
				}
				foreach (VersionConverter versionConverter in list)
				{
					Log.Information(string.Concat(new string[]
					{
						"Upgrading world version \"",
						versionConverter.SourceVersion,
						"\" to \"",
						versionConverter.TargetVersion,
						"\"."
					}));
					versionConverter.ConvertWorld(directoryName);
				}
				WorldInfo worldInfo2 = WorldsManager.GetWorldInfo(directoryName);
				if (worldInfo2.SerializationVersion != VersionsManager.SerializationVersion)
				{
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Upgrade produced invalid project version. Expected \"",
						VersionsManager.SerializationVersion,
						"\", found \"",
						worldInfo2.SerializationVersion,
						"\"."
					}));
				}
			}
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x000273EC File Offset: 0x000255EC
		public static int CompareVersions(string v1, string v2)
		{
			string[] array = v1.Split(new char[]
			{
				'.'
			});
			string[] array2 = v2.Split(new char[]
			{
				'.'
			});
			for (int i = 0; i < MathUtils.Min(array.Length, array2.Length); i++)
			{
				int num2;
				int num3;
				int num = (!int.TryParse(array[i], out num2) || !int.TryParse(array2[i], out num3)) ? string.CompareOrdinal(array[i], array2[i]) : (num2 - num3);
				if (num != 0)
				{
					return num;
				}
			}
			return array.Length - array2.Length;
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x0002746C File Offset: 0x0002566C
		public static List<VersionConverter> FindTransform(string sourceVersion, string targetVersion, IEnumerable<VersionConverter> converters, int depth)
		{
			if (depth > 100)
			{
				throw new InvalidOperationException("Too deep recursion when searching for version converters. Check for possible loops in transforms.");
			}
			if (sourceVersion == targetVersion)
			{
				return new List<VersionConverter>();
			}
			List<VersionConverter> result = null;
			int num = int.MaxValue;
			foreach (VersionConverter versionConverter in converters)
			{
				if (versionConverter.SourceVersion == sourceVersion)
				{
					List<VersionConverter> list = VersionsManager.FindTransform(versionConverter.TargetVersion, targetVersion, converters, depth + 1);
					if (list != null && list.Count < num)
					{
						num = list.Count;
						list.Insert(0, versionConverter);
						result = list;
					}
				}
			}
			return result;
		}

		// Token: 0x04000322 RID: 802
		public static List<VersionConverter> m_versionConverters = new List<VersionConverter>();
	}
}
