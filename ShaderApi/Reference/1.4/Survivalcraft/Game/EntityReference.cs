using System;
using System.Globalization;
using GameEntitySystem;

namespace Game
{
	// Token: 0x02000284 RID: 644
	public struct EntityReference
	{
		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06001473 RID: 5235 RVA: 0x0009917C File Offset: 0x0009737C
		public string ReferenceString
		{
			get
			{
				if (this.m_referenceType == EntityReference.ReferenceType.Null)
				{
					return "null:";
				}
				if (this.m_referenceType == EntityReference.ReferenceType.Local)
				{
					return "local:" + this.m_componentReference;
				}
				if (this.m_referenceType == EntityReference.ReferenceType.ByEntityId)
				{
					return "id:" + this.m_entityReference + ":" + this.m_componentReference;
				}
				if (this.m_referenceType == EntityReference.ReferenceType.ByEntityName)
				{
					return "name:" + this.m_entityReference + ":" + this.m_componentReference;
				}
				throw new Exception("Unknown entity reference type.");
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06001474 RID: 5236 RVA: 0x00099208 File Offset: 0x00097408
		public static EntityReference Null
		{
			get
			{
				return default(EntityReference);
			}
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x00099220 File Offset: 0x00097420
		public Entity GetEntity(Entity localEntity, IdToEntityMap idToEntityMap, bool throwIfNotFound)
		{
			Entity entity;
			if (this.m_referenceType == EntityReference.ReferenceType.Null)
			{
				entity = null;
			}
			else if (this.m_referenceType == EntityReference.ReferenceType.Local)
			{
				entity = localEntity;
			}
			else if (this.m_referenceType == EntityReference.ReferenceType.ByEntityId)
			{
				int id = int.Parse(this.m_entityReference, CultureInfo.InvariantCulture);
				entity = idToEntityMap.FindEntity(id);
			}
			else
			{
				if (this.m_referenceType != EntityReference.ReferenceType.ByEntityName)
				{
					throw new Exception("Unknown entity reference type.");
				}
				entity = localEntity.Project.FindSubsystem<SubsystemNames>(true).FindEntityByName(this.m_entityReference);
			}
			if (entity != null)
			{
				return entity;
			}
			if (throwIfNotFound)
			{
				throw new Exception("Required entity \"" + this.ReferenceString + "\" not found.");
			}
			return null;
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x000992BC File Offset: 0x000974BC
		public T GetComponent<T>(Entity localEntity, IdToEntityMap idToEntityMap, bool throwIfNotFound) where T : class
		{
			Entity entity = this.GetEntity(localEntity, idToEntityMap, throwIfNotFound);
			if (entity == null)
			{
				return default(T);
			}
			return entity.FindComponent<T>(this.m_componentReference, throwIfNotFound);
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x000992F0 File Offset: 0x000974F0
		public bool IsNullOrEmpty()
		{
			return this.m_referenceType == EntityReference.ReferenceType.Null || (this.m_referenceType == EntityReference.ReferenceType.Local && string.IsNullOrEmpty(this.m_componentReference)) || (this.m_referenceType == EntityReference.ReferenceType.ByEntityId && this.m_entityReference == "0") || (this.m_referenceType == EntityReference.ReferenceType.ByEntityName && string.IsNullOrEmpty(this.m_entityReference));
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x00099350 File Offset: 0x00097550
		public static EntityReference Local(Component component)
		{
			return new EntityReference
			{
				m_referenceType = EntityReference.ReferenceType.Local,
				m_componentReference = ((component != null) ? component.ValuesDictionary.DatabaseObject.Name : string.Empty)
			};
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x00099390 File Offset: 0x00097590
		public static EntityReference FromId(Component component, EntityToIdMap entityToIdMap)
		{
			int num = entityToIdMap.FindId((component != null) ? component.Entity : null);
			return new EntityReference
			{
				m_referenceType = EntityReference.ReferenceType.ByEntityId,
				m_entityReference = num.ToString(CultureInfo.InvariantCulture),
				m_componentReference = ((component != null) ? component.ValuesDictionary.DatabaseObject.Name : string.Empty)
			};
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x000993F8 File Offset: 0x000975F8
		public static EntityReference FromId(Entity entity, EntityToIdMap entityToIdMap)
		{
			int num = entityToIdMap.FindId(entity);
			return new EntityReference
			{
				m_referenceType = EntityReference.ReferenceType.ByEntityId,
				m_entityReference = num.ToString(CultureInfo.InvariantCulture),
				m_componentReference = string.Empty
			};
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x00099440 File Offset: 0x00097640
		public static EntityReference FromName(Component component)
		{
			string entityReference = (component != null) ? component.Entity.FindComponent<ComponentName>(null, true).Name : string.Empty;
			return new EntityReference
			{
				m_referenceType = EntityReference.ReferenceType.ByEntityName,
				m_entityReference = entityReference,
				m_componentReference = ((component != null) ? component.ValuesDictionary.DatabaseObject.Name : string.Empty)
			};
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x000994A4 File Offset: 0x000976A4
		public static EntityReference FromName(Entity entity)
		{
			string entityReference = (entity != null) ? entity.FindComponent<ComponentName>(null, true).Name : string.Empty;
			return new EntityReference
			{
				m_referenceType = EntityReference.ReferenceType.ByEntityName,
				m_entityReference = entityReference,
				m_componentReference = string.Empty
			};
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x000994F0 File Offset: 0x000976F0
		public static EntityReference FromReferenceString(string referenceString)
		{
			EntityReference result = default(EntityReference);
			if (string.IsNullOrEmpty(referenceString))
			{
				result.m_referenceType = EntityReference.ReferenceType.Null;
				result.m_entityReference = string.Empty;
				result.m_componentReference = string.Empty;
			}
			else
			{
				string[] array = referenceString.Split(new char[]
				{
					':'
				});
				if (array.Length == 1)
				{
					result.m_referenceType = EntityReference.ReferenceType.Local;
					result.m_entityReference = string.Empty;
					result.m_componentReference = array[0];
				}
				else
				{
					if (array.Length != 2 && array.Length != 3)
					{
						throw new Exception("Invalid entity reference. Too many tokens.");
					}
					if (array[0] == "null" && array.Length == 2)
					{
						result.m_referenceType = EntityReference.ReferenceType.Null;
						result.m_entityReference = string.Empty;
						result.m_componentReference = string.Empty;
					}
					else if (array[0] == "local" && array.Length == 2)
					{
						result.m_referenceType = EntityReference.ReferenceType.Local;
						result.m_componentReference = array[1];
					}
					else if (array[0] == "id")
					{
						result.m_referenceType = EntityReference.ReferenceType.ByEntityId;
						result.m_entityReference = array[1];
						result.m_componentReference = ((array.Length == 3) ? array[2] : string.Empty);
					}
					else
					{
						if (!(array[0] == "name"))
						{
							throw new Exception("Unknown entity reference type.");
						}
						result.m_referenceType = EntityReference.ReferenceType.ByEntityId;
						result.m_entityReference = array[1];
						result.m_componentReference = ((array.Length == 3) ? array[2] : string.Empty);
					}
				}
			}
			return result;
		}

		// Token: 0x04000D3E RID: 3390
		public EntityReference.ReferenceType m_referenceType;

		// Token: 0x04000D3F RID: 3391
		public string m_entityReference;

		// Token: 0x04000D40 RID: 3392
		public string m_componentReference;

		// Token: 0x02000509 RID: 1289
		public enum ReferenceType
		{
			// Token: 0x0400183D RID: 6205
			Null,
			// Token: 0x0400183E RID: 6206
			Local,
			// Token: 0x0400183F RID: 6207
			ByEntityId,
			// Token: 0x04001840 RID: 6208
			ByEntityName
		}
	}
}
