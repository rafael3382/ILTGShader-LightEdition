using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000367 RID: 871
	public static class DatabaseManager
	{
		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06001974 RID: 6516 RVA: 0x000C9679 File Offset: 0x000C7879
		public static GameDatabase GameDatabase
		{
			get
			{
				if (DatabaseManager.m_gameDatabase != null)
				{
					return DatabaseManager.m_gameDatabase;
				}
				throw new InvalidOperationException("Database not loaded.");
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06001975 RID: 6517 RVA: 0x000C9692 File Offset: 0x000C7892
		public static ICollection<ValuesDictionary> EntitiesValuesDictionaries
		{
			get
			{
				return DatabaseManager.m_valueDictionaries.Values;
			}
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x000C969E File Offset: 0x000C789E
		public static void Initialize()
		{
			DatabaseManager.DatabaseNode = null;
			DatabaseManager.m_valueDictionaries.Clear();
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x000C96B0 File Offset: 0x000C78B0
		public static void LoadDataBaseFromXml(XElement node)
		{
			DatabaseManager.m_gameDatabase = new GameDatabase(XmlDatabaseSerializer.LoadDatabase(node));
			foreach (DatabaseObject databaseObject in DatabaseManager.GameDatabase.Database.Root.GetExplicitNestingChildren(DatabaseManager.GameDatabase.EntityTemplateType, false))
			{
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				valuesDictionary.PopulateFromDatabaseObject(databaseObject);
				DatabaseManager.m_valueDictionaries.Add(databaseObject.Name, valuesDictionary);
			}
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x000C9740 File Offset: 0x000C7940
		public static ValuesDictionary FindEntityValuesDictionary(string entityTemplateName, bool throwIfNotFound)
		{
			ValuesDictionary result;
			if (!DatabaseManager.m_valueDictionaries.TryGetValue(entityTemplateName, out result) && throwIfNotFound)
			{
				throw new InvalidOperationException("EntityTemplate \"" + entityTemplateName + "\" not found.");
			}
			return result;
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x000C9778 File Offset: 0x000C7978
		public static ValuesDictionary FindValuesDictionaryForComponent(ValuesDictionary entityVd, Type componentType)
		{
			foreach (ValuesDictionary valuesDictionary in entityVd.Values.OfType<ValuesDictionary>())
			{
				if (valuesDictionary.DatabaseObject.Type == DatabaseManager.GameDatabase.MemberComponentTemplateType)
				{
					Type type = TypeCache.FindType(valuesDictionary.GetValue<string>("Class"), true, true);
					if (componentType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
					{
						return valuesDictionary;
					}
				}
			}
			return null;
		}

		// Token: 0x0600197A RID: 6522 RVA: 0x000C9808 File Offset: 0x000C7A08
		public static Entity CreateEntity(Project project, string entityTemplateName, bool throwIfNotFound)
		{
			ValuesDictionary valuesDictionary = DatabaseManager.FindEntityValuesDictionary(entityTemplateName, throwIfNotFound);
			if (valuesDictionary == null)
			{
				return null;
			}
			return project.CreateEntity(valuesDictionary);
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x000C982C File Offset: 0x000C7A2C
		public static Entity CreateEntity(Project project, string entityTemplateName, ValuesDictionary overrides, bool throwIfNotFound)
		{
			ValuesDictionary valuesDictionary = DatabaseManager.FindEntityValuesDictionary(entityTemplateName, throwIfNotFound);
			if (valuesDictionary != null)
			{
				valuesDictionary.ApplyOverrides(overrides);
				return project.CreateEntity(valuesDictionary);
			}
			return null;
		}

		// Token: 0x04001184 RID: 4484
		public static GameDatabase m_gameDatabase;

		// Token: 0x04001185 RID: 4485
		public static Dictionary<string, ValuesDictionary> m_valueDictionaries = new Dictionary<string, ValuesDictionary>();

		// Token: 0x04001186 RID: 4486
		public static XElement DatabaseNode = null;
	}
}
