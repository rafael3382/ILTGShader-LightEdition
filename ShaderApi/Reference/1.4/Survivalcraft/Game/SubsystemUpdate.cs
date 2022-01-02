using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E0 RID: 480
	public class SubsystemUpdate : Subsystem
	{
		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000D14 RID: 3348 RVA: 0x0005EDFF File Offset: 0x0005CFFF
		public int UpdateablesCount
		{
			get
			{
				return this.m_updateables.Count;
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000D15 RID: 3349 RVA: 0x0005EE0C File Offset: 0x0005D00C
		// (set) Token: 0x06000D16 RID: 3350 RVA: 0x0005EE14 File Offset: 0x0005D014
		public int UpdatesPerFrame { get; set; }

		// Token: 0x06000D17 RID: 3351 RVA: 0x0005EE20 File Offset: 0x0005D020
		public void Update()
		{
			for (int i = 0; i < this.UpdatesPerFrame; i++)
			{
				this.m_subsystemTime.NextFrame();
				bool flag = false;
				foreach (KeyValuePair<IUpdateable, bool> keyValuePair in this.m_toAddOrRemove)
				{
					if (keyValuePair.Value)
					{
						this.m_updateables.Add(keyValuePair.Key, new SubsystemUpdate.UpdateableInfo
						{
							UpdateOrder = keyValuePair.Key.UpdateOrder
						});
						flag = true;
					}
					else
					{
						this.m_updateables.Remove(keyValuePair.Key);
						flag = true;
					}
				}
				this.m_toAddOrRemove.Clear();
				foreach (KeyValuePair<IUpdateable, SubsystemUpdate.UpdateableInfo> keyValuePair2 in this.m_updateables)
				{
					UpdateOrder updateOrder = keyValuePair2.Key.UpdateOrder;
					if (updateOrder != keyValuePair2.Value.UpdateOrder)
					{
						flag = true;
						keyValuePair2.Value.UpdateOrder = updateOrder;
					}
				}
				if (flag)
				{
					this.m_sortedUpdateables.Clear();
					foreach (IUpdateable item in this.m_updateables.Keys)
					{
						this.m_sortedUpdateables.Add(item);
					}
					this.m_sortedUpdateables.Sort(SubsystemUpdate.Comparer.Instance);
				}
				float dt = MathUtils.Clamp(this.m_subsystemTime.GameTimeDelta, 0f, 0.1f);
				foreach (IUpdateable updateable in this.m_sortedUpdateables)
				{
					try
					{
						updateable.Update(dt);
					}
					catch (Exception)
					{
					}
				}
				ModsManager.HookAction("SubsystemUpdate", delegate(ModLoader loader)
				{
					loader.SubsystemUpdate(dt);
					return false;
				});
			}
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x0005F058 File Offset: 0x0005D258
		public void AddUpdateable(IUpdateable updateable)
		{
			this.m_toAddOrRemove[updateable] = true;
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x0005F067 File Offset: 0x0005D267
		public void RemoveUpdateable(IUpdateable updateable)
		{
			this.m_toAddOrRemove[updateable] = false;
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x0005F078 File Offset: 0x0005D278
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			foreach (IUpdateable updateable in base.Project.FindSubsystems<IUpdateable>())
			{
				this.AddUpdateable(updateable);
			}
			this.UpdatesPerFrame = 1;
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x0005F0E4 File Offset: 0x0005D2E4
		public override void OnEntityAdded(Entity entity)
		{
			foreach (IUpdateable updateable in entity.FindComponents<IUpdateable>())
			{
				this.AddUpdateable(updateable);
			}
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x0005F13C File Offset: 0x0005D33C
		public override void OnEntityRemoved(Entity entity)
		{
			foreach (IUpdateable updateable in entity.FindComponents<IUpdateable>())
			{
				this.RemoveUpdateable(updateable);
			}
		}

		// Token: 0x040006B7 RID: 1719
		public SubsystemTime m_subsystemTime;

		// Token: 0x040006B8 RID: 1720
		public Dictionary<IUpdateable, SubsystemUpdate.UpdateableInfo> m_updateables = new Dictionary<IUpdateable, SubsystemUpdate.UpdateableInfo>();

		// Token: 0x040006B9 RID: 1721
		public Dictionary<IUpdateable, bool> m_toAddOrRemove = new Dictionary<IUpdateable, bool>();

		// Token: 0x040006BA RID: 1722
		public List<IUpdateable> m_sortedUpdateables = new List<IUpdateable>();

		// Token: 0x020004B7 RID: 1207
		public class UpdateableInfo
		{
			// Token: 0x0400176A RID: 5994
			public UpdateOrder UpdateOrder;
		}

		// Token: 0x020004B8 RID: 1208
		public class Comparer : IComparer<IUpdateable>
		{
			// Token: 0x060020E7 RID: 8423 RVA: 0x000E9708 File Offset: 0x000E7908
			public int Compare(IUpdateable u1, IUpdateable u2)
			{
				int num = u1.UpdateOrder - u2.UpdateOrder;
				if (num != 0)
				{
					return num;
				}
				return u1.GetHashCode() - u2.GetHashCode();
			}

			// Token: 0x0400176B RID: 5995
			public static SubsystemUpdate.Comparer Instance = new SubsystemUpdate.Comparer();
		}
	}
}
