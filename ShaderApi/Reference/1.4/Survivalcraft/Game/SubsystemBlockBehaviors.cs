using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000187 RID: 391
	public class SubsystemBlockBehaviors : Subsystem
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000905 RID: 2309 RVA: 0x00038963 File Offset: 0x00036B63
		public ReadOnlyList<SubsystemBlockBehavior> BlockBehaviors
		{
			get
			{
				return new ReadOnlyList<SubsystemBlockBehavior>(this.m_blockBehaviors);
			}
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x00038970 File Offset: 0x00036B70
		public SubsystemBlockBehavior[] GetBlockBehaviors(int contents)
		{
			return this.m_blockBehaviorsByContents[contents];
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x0003897C File Offset: 0x00036B7C
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_blockBehaviorsByContents = new SubsystemBlockBehavior[BlocksManager.Blocks.Length][];
			Dictionary<int, List<SubsystemBlockBehavior>> dictionary = new Dictionary<int, List<SubsystemBlockBehavior>>();
			for (int i = 0; i < this.m_blockBehaviorsByContents.Length; i++)
			{
				dictionary[i] = new List<SubsystemBlockBehavior>();
				foreach (string text in BlocksManager.Blocks[i].Behaviors.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					SubsystemBlockBehavior item = base.Project.FindSubsystem<SubsystemBlockBehavior>(text.Trim(), true);
					dictionary[i].Add(item);
				}
			}
			foreach (SubsystemBlockBehavior subsystemBlockBehavior in base.Project.FindSubsystems<SubsystemBlockBehavior>())
			{
				this.m_blockBehaviors.Add(subsystemBlockBehavior);
				foreach (int key in subsystemBlockBehavior.HandledBlocks)
				{
					dictionary[key].Add(subsystemBlockBehavior);
				}
			}
			for (int k = 0; k < this.m_blockBehaviorsByContents.Length; k++)
			{
				this.m_blockBehaviorsByContents[k] = dictionary[k].ToArray();
			}
		}

		// Token: 0x04000483 RID: 1155
		public SubsystemBlockBehavior[][] m_blockBehaviorsByContents;

		// Token: 0x04000484 RID: 1156
		public List<SubsystemBlockBehavior> m_blockBehaviors = new List<SubsystemBlockBehavior>();
	}
}
