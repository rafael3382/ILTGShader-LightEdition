using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E4 RID: 484
	public class SubsystemWhistleBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000D47 RID: 3399 RVA: 0x000603CE File Offset: 0x0005E5CE
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					160
				};
			}
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x000603E0 File Offset: 0x0005E5E0
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			this.m_subsystemAudio.PlayRandomSound("Audio/Whistle", 1f, this.m_random.Float(-0.2f, 0f), ray.Position, 4f, true);
			this.m_subsystemNoise.MakeNoise(componentMiner.ComponentCreature.ComponentBody, 0.5f, 30f);
			DynamicArray<ComponentBody> dynamicArray = new DynamicArray<ComponentBody>();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(componentMiner.ComponentCreature.ComponentBody.Position.X, componentMiner.ComponentCreature.ComponentBody.Position.Z), 64f, dynamicArray);
			float num = float.PositiveInfinity;
			List<ComponentBody> list = new List<ComponentBody>();
			foreach (ComponentBody componentBody in dynamicArray)
			{
				ComponentSummonBehavior componentSummonBehavior = componentBody.Entity.FindComponent<ComponentSummonBehavior>();
				if (componentSummonBehavior != null && componentSummonBehavior.IsEnabled)
				{
					float num2 = Vector3.Distance(componentBody.Position, componentMiner.ComponentCreature.ComponentBody.Position);
					if (num2 > 4f && componentSummonBehavior.SummonTarget == null)
					{
						list.Add(componentBody);
						num = MathUtils.Min(num, num2);
					}
					else
					{
						componentSummonBehavior.SummonTarget = componentMiner.ComponentCreature.ComponentBody;
					}
				}
			}
			foreach (ComponentBody componentBody2 in list)
			{
				ComponentSummonBehavior componentSummonBehavior2 = componentBody2.Entity.FindComponent<ComponentSummonBehavior>();
				if (componentSummonBehavior2 != null && Vector3.Distance(componentBody2.Position, componentMiner.ComponentCreature.ComponentBody.Position) < num + 4f)
				{
					componentSummonBehavior2.SummonTarget = componentMiner.ComponentCreature.ComponentBody;
				}
			}
			componentMiner.DamageActiveTool(1);
			return true;
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x000605CC File Offset: 0x0005E7CC
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
		}

		// Token: 0x040006D6 RID: 1750
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040006D7 RID: 1751
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040006D8 RID: 1752
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x040006D9 RID: 1753
		public Game.Random m_random = new Game.Random();
	}
}
