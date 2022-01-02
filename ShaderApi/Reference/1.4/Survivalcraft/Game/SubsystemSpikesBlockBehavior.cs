using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D6 RID: 470
	public class SubsystemSpikesBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000CAF RID: 3247 RVA: 0x0005BAF4 File Offset: 0x00059CF4
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					86
				};
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000CB0 RID: 3248 RVA: 0x0005BB01 File Offset: 0x00059D01
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x0005BB04 File Offset: 0x00059D04
		public void Update(float dt)
		{
			if (this.m_closestSoundToPlay != null)
			{
				this.m_subsystemAudio.PlaySound("Audio/Spikes", 0.7f, SubsystemSpikesBlockBehavior.m_random.Float(-0.1f, 0.1f), this.m_closestSoundToPlay.Value, 4f, true);
				this.m_closestSoundToPlay = null;
			}
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x0005BB64 File Offset: 0x00059D64
		public bool RetractExtendSpikes(int x, int y, int z, bool extend)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num] is SpikedPlankBlock)
			{
				int data = SpikedPlankBlock.SetSpikesState(Terrain.ExtractData(cellValue), extend);
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				Vector3 vector = new Vector3((float)x, (float)y, (float)z);
				float num2 = this.m_subsystemAudio.CalculateListenerDistance(vector);
				if (this.m_closestSoundToPlay == null || num2 < this.m_subsystemAudio.CalculateListenerDistance(this.m_closestSoundToPlay.Value))
				{
					this.m_closestSoundToPlay = new Vector3?(vector);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x0005BC14 File Offset: 0x00059E14
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			int data = Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			if (!SpikedPlankBlock.GetSpikesState(data))
			{
				return;
			}
			int mountingFace = SpikedPlankBlock.GetMountingFace(data);
			if (cellFace.Face != mountingFace)
			{
				return;
			}
			ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
			if (componentCreature != null)
			{
				double num;
				this.m_lastInjuryTimes.TryGetValue(componentCreature, out num);
				if (this.m_subsystemTime.GameTime - num > 1.0)
				{
					this.m_lastInjuryTimes[componentCreature] = this.m_subsystemTime.GameTime;
					componentCreature.ComponentHealth.Injure(0.1f, null, false, "Spiked by a trap");
				}
			}
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x0005BCC7 File Offset: 0x00059EC7
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x0005BCF4 File Offset: 0x00059EF4
		public override void OnEntityRemoved(Entity entity)
		{
			ComponentCreature componentCreature = entity.FindComponent<ComponentCreature>();
			if (componentCreature != null)
			{
				this.m_lastInjuryTimes.Remove(componentCreature);
			}
		}

		// Token: 0x04000683 RID: 1667
		public static Game.Random m_random = new Game.Random();

		// Token: 0x04000684 RID: 1668
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000685 RID: 1669
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000686 RID: 1670
		public Vector3? m_closestSoundToPlay;

		// Token: 0x04000687 RID: 1671
		public Dictionary<ComponentCreature, double> m_lastInjuryTimes = new Dictionary<ComponentCreature, double>();
	}
}
