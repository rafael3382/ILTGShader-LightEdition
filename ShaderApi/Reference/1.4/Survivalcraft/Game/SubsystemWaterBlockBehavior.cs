using System;
using Engine;

namespace Game
{
	// Token: 0x020001E1 RID: 481
	public class SubsystemWaterBlockBehavior : SubsystemFluidBlockBehavior, IUpdateable
	{
		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000D1E RID: 3358 RVA: 0x0005F1BD File Offset: 0x0005D3BD
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					18
				};
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000D1F RID: 3359 RVA: 0x0005F1CA File Offset: 0x0005D3CA
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0005F1CD File Offset: 0x0005D3CD
		public SubsystemWaterBlockBehavior() : base(BlocksManager.FluidBlocks[18], true)
		{
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x0005F1EC File Offset: 0x0005D3EC
		public void Update(float dt)
		{
			if (base.SubsystemTime.PeriodicGameTimeEvent(0.25, 0.0))
			{
				base.SpreadFluid();
			}
			if (base.SubsystemTime.PeriodicGameTimeEvent(1.0, 0.25))
			{
				float num = float.MaxValue;
				foreach (Vector3 p in base.SubsystemAudio.ListenerPositions)
				{
					float? num2 = base.CalculateDistanceToFluid(p, 8, true);
					if (num2 != null && num2.Value < num)
					{
						num = num2.Value;
					}
				}
				this.m_soundVolume = 0.5f * base.SubsystemAudio.CalculateVolume(num, 2f, 3.5f);
			}
			base.SubsystemAmbientSounds.WaterSoundVolume = MathUtils.Max(base.SubsystemAmbientSounds.WaterSoundVolume, this.m_soundVolume);
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x0005F2F8 File Offset: 0x0005D4F8
		public override bool OnFluidInteract(int interactValue, int x, int y, int z, int fluidValue)
		{
			if (BlocksManager.Blocks[Terrain.ExtractContents(interactValue)] is MagmaBlock)
			{
				base.SubsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.1f, 0.1f), new Vector3((float)x, (float)y, (float)z), 5f, true);
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				base.Set(x, y, z, 3);
				return true;
			}
			return base.OnFluidInteract(interactValue, x, y, z, fluidValue);
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x0005F381 File Offset: 0x0005D581
		public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			if (y > 80 && SubsystemWeather.IsPlaceFrozen(base.SubsystemTerrain.Terrain.GetSeasonalTemperature(x, z), y))
			{
				dropValue.Value = Terrain.MakeBlockValue(62);
				return;
			}
			base.OnItemHarvested(x, y, z, blockValue, ref dropValue, ref newBlockValue);
		}

		// Token: 0x040006BC RID: 1724
		public Game.Random m_random = new Game.Random();

		// Token: 0x040006BD RID: 1725
		public float m_soundVolume;
	}
}
