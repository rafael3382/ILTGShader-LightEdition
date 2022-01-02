using System;
using System.Collections.Generic;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200019C RID: 412
	public class SubsystemDrawing : Subsystem
	{
		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000A23 RID: 2595 RVA: 0x000414DD File Offset: 0x0003F6DD
		public int DrawablesCount
		{
			get
			{
				return this.m_drawables.Count;
			}
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x000414EA File Offset: 0x0003F6EA
		public void AddDrawable(IDrawable drawable)
		{
			this.m_drawables.Add(drawable, true);
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x000414F9 File Offset: 0x0003F6F9
		public void RemoveDrawable(IDrawable drawable)
		{
			this.m_drawables.Remove(drawable);
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00041508 File Offset: 0x0003F708
		public void Draw(Camera camera)
		{
			this.m_sortedDrawables.Clear();
			foreach (IDrawable drawable in this.m_drawables.Keys)
			{
				foreach (int key in drawable.DrawOrders)
				{
					this.m_sortedDrawables.Add(key, drawable);
				}
			}
			for (int j = 0; j < this.m_sortedDrawables.Count; j++)
			{
				try
				{
					KeyValuePair<int, IDrawable> keyValuePair = this.m_sortedDrawables[j];
					keyValuePair.Value.Draw(camera, keyValuePair.Key);
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x000415DC File Offset: 0x0003F7DC
		public override void Load(ValuesDictionary valuesDictionary)
		{
			foreach (IDrawable drawable in base.Project.FindSubsystems<IDrawable>())
			{
				this.AddDrawable(drawable);
			}
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x00041630 File Offset: 0x0003F830
		public override void OnEntityAdded(Entity entity)
		{
			foreach (IDrawable drawable in entity.FindComponents<IDrawable>())
			{
				this.AddDrawable(drawable);
			}
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x00041688 File Offset: 0x0003F888
		public override void OnEntityRemoved(Entity entity)
		{
			foreach (IDrawable drawable in entity.FindComponents<IDrawable>())
			{
				this.RemoveDrawable(drawable);
			}
		}

		// Token: 0x040004E4 RID: 1252
		public Dictionary<IDrawable, bool> m_drawables = new Dictionary<IDrawable, bool>();

		// Token: 0x040004E5 RID: 1253
		public SortedMultiCollection<int, IDrawable> m_sortedDrawables = new SortedMultiCollection<int, IDrawable>();
	}
}
