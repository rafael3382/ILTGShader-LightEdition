using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B6 RID: 438
	public class SubsystemMagmaBlockBehavior : SubsystemFluidBlockBehavior, IUpdateable
	{
		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000B3D RID: 2877 RVA: 0x0004B756 File Offset: 0x00049956
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					92
				};
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000B3E RID: 2878 RVA: 0x0004B763 File Offset: 0x00049963
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000B3F RID: 2879 RVA: 0x0004B766 File Offset: 0x00049966
		public SubsystemMagmaBlockBehavior() : base(BlocksManager.FluidBlocks[92], false)
		{
		}

		// Token: 0x06000B40 RID: 2880 RVA: 0x0004B784 File Offset: 0x00049984
		public void Update(float dt)
		{
			if (base.SubsystemTime.PeriodicGameTimeEvent(2.0, 0.0))
			{
				base.SpreadFluid();
			}
			if (base.SubsystemTime.PeriodicGameTimeEvent(1.0, 0.75))
			{
				float num = float.MaxValue;
				foreach (Vector3 p in base.SubsystemAudio.ListenerPositions)
				{
					float? num2 = base.CalculateDistanceToFluid(p, 8, false);
					if (num2 != null && num2.Value < num)
					{
						num = num2.Value;
					}
				}
				this.m_soundVolume = base.SubsystemAudio.CalculateVolume(num, 2f, 3.5f);
			}
			base.SubsystemAmbientSounds.MagmaSoundVolume = MathUtils.Max(base.SubsystemAmbientSounds.MagmaSoundVolume, this.m_soundVolume);
		}

		// Token: 0x06000B41 RID: 2881 RVA: 0x0004B888 File Offset: 0x00049A88
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			base.OnBlockAdded(value, oldValue, x, y, z);
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					for (int k = -1; k <= 1; k++)
					{
						this.ApplyMagmaNeighborhoodEffect(x + i, y + j, z + k);
					}
				}
			}
		}

		// Token: 0x06000B42 RID: 2882 RVA: 0x0004B8D7 File Offset: 0x00049AD7
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
			this.ApplyMagmaNeighborhoodEffect(neighborX, neighborY, neighborZ);
		}

		// Token: 0x06000B43 RID: 2883 RVA: 0x0004B8F4 File Offset: 0x00049AF4
		public override bool OnFluidInteract(int interactValue, int x, int y, int z, int fluidValue)
		{
			if (BlocksManager.Blocks[Terrain.ExtractContents(interactValue)] is WaterBlock)
			{
				base.SubsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.1f, 0.1f), new Vector3((float)x, (float)y, (float)z), 5f, true);
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				base.Set(x, y, z, 67);
				return true;
			}
			return base.OnFluidInteract(interactValue, x, y, z, fluidValue);
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x0004B97E File Offset: 0x00049B7E
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemFireBlockBehavior = base.Project.FindSubsystem<SubsystemFireBlockBehavior>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
		}

		// Token: 0x06000B45 RID: 2885 RVA: 0x0004B9AC File Offset: 0x00049BAC
		public void ApplyMagmaNeighborhoodEffect(int x, int y, int z)
		{
			this.m_subsystemFireBlockBehavior.SetCellOnFire(x, y, z, 1f);
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y, z);
			if (cellContents != 8)
			{
				if (cellContents - 61 <= 1)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
					this.m_subsystemParticles.AddParticleSystem(new BurntDebrisParticleSystem(base.SubsystemTerrain, new Vector3((float)x + 0.5f, (float)(y + 1), (float)z + 0.5f)));
					return;
				}
			}
			else
			{
				base.SubsystemTerrain.ChangeCell(x, y, z, 2, true);
				this.m_subsystemParticles.AddParticleSystem(new BurntDebrisParticleSystem(base.SubsystemTerrain, new Vector3((float)x + 0.5f, (float)(y + 1), (float)z + 0.5f)));
			}
		}

		// Token: 0x04000573 RID: 1395
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000574 RID: 1396
		public SubsystemFireBlockBehavior m_subsystemFireBlockBehavior;

		// Token: 0x04000575 RID: 1397
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000576 RID: 1398
		public float m_soundVolume;
	}
}
