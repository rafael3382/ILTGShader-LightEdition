using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001BF RID: 447
	public class SubsystemNoise : Subsystem
	{
		// Token: 0x06000B9F RID: 2975 RVA: 0x0004FD3F File Offset: 0x0004DF3F
		public void MakeNoise(Vector3 position, float loudness, float range)
		{
			this.MakeNoisepublic(null, position, loudness, range);
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x0004FD4B File Offset: 0x0004DF4B
		public void MakeNoise(ComponentBody sourceBody, float loudness, float range)
		{
			this.MakeNoisepublic(sourceBody, sourceBody.Position, loudness, range);
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x0004FD5C File Offset: 0x0004DF5C
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x0004FD70 File Offset: 0x0004DF70
		public void MakeNoisepublic(ComponentBody sourceBody, Vector3 position, float loudness, float range)
		{
			float num = range * range;
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), range, this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentBody componentBody = this.m_componentBodies.Array[i];
				if (componentBody != sourceBody && Vector3.DistanceSquared(componentBody.Position, position) < num)
				{
					foreach (INoiseListener noiseListener in componentBody.Entity.FindComponents<INoiseListener>())
					{
						noiseListener.HearNoise(sourceBody, position, loudness);
					}
				}
			}
		}

		// Token: 0x040005B7 RID: 1463
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040005B8 RID: 1464
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();
	}
}
