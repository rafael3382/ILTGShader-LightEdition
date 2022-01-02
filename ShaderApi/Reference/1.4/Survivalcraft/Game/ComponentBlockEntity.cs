using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001ED RID: 493
	public class ComponentBlockEntity : Component
	{
		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000D8C RID: 3468 RVA: 0x00062B65 File Offset: 0x00060D65
		// (set) Token: 0x06000D8D RID: 3469 RVA: 0x00062B6D File Offset: 0x00060D6D
		public Point3 Coordinates { get; set; }

		// Token: 0x06000D8E RID: 3470 RVA: 0x00062B76 File Offset: 0x00060D76
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.Coordinates = valuesDictionary.GetValue<Point3>("Coordinates");
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x00062B89 File Offset: 0x00060D89
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<Point3>("Coordinates", this.Coordinates);
		}
	}
}
