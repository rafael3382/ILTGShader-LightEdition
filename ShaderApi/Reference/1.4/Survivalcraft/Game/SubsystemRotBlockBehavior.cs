using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001CC RID: 460
	public class SubsystemRotBlockBehavior : SubsystemPollableBlockBehavior
	{
		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000C28 RID: 3112 RVA: 0x00055EA3 File Offset: 0x000540A3
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x00055EAC File Offset: 0x000540AC
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemItemsScanner = base.Project.FindSubsystem<SubsystemItemsScanner>(true);
			this.m_lastRotTime = valuesDictionary.GetValue<double>("LastRotTime");
			this.m_rotStep = valuesDictionary.GetValue<int>("RotStep");
			SubsystemItemsScanner subsystemItemsScanner = this.m_subsystemItemsScanner;
			subsystemItemsScanner.ItemsScanned = (Action<ReadOnlyList<ScannedItemData>>)Delegate.Combine(subsystemItemsScanner.ItemsScanned, new Action<ReadOnlyList<ScannedItemData>>(this.ItemsScanned));
			this.m_isRotEnabled = (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Adventure);
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x00055F5E File Offset: 0x0005415E
		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			valuesDictionary.SetValue<double>("LastRotTime", this.m_lastRotTime);
			valuesDictionary.SetValue<int>("RotStep", this.m_rotStep);
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x00055F8C File Offset: 0x0005418C
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (this.m_isRotEnabled)
			{
				int num = Terrain.ExtractContents(value);
				Block block = BlocksManager.Blocks[num];
				int rotPeriod = block.GetRotPeriod(value);
				if (rotPeriod > 0 && pollPass % rotPeriod == 0)
				{
					int num2 = block.GetDamage(value) + 1;
					value = ((num2 > 1) ? block.GetDamageDestructionValue(value) : block.SetDamage(value, num2));
					base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				}
			}
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x00055FF4 File Offset: 0x000541F4
		public void ItemsScanned(ReadOnlyList<ScannedItemData> items)
		{
			int num = (int)((this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_lastRotTime) / 60.0);
			if (num > 0)
			{
				if (this.m_isRotEnabled)
				{
					foreach (ScannedItemData scannedItemData in items)
					{
						int num2 = Terrain.ExtractContents(scannedItemData.Value);
						Block block = BlocksManager.Blocks[num2];
						int rotPeriod = block.GetRotPeriod(scannedItemData.Value);
						if (rotPeriod > 0)
						{
							int num3 = block.GetDamage(scannedItemData.Value);
							int num4 = 0;
							while (num4 < num && num3 <= 1)
							{
								if ((num4 + this.m_rotStep) % rotPeriod == 0)
								{
									num3++;
								}
								num4++;
							}
							if (num3 <= 1)
							{
								this.m_subsystemItemsScanner.TryModifyItem(scannedItemData, block.SetDamage(scannedItemData.Value, num3));
							}
							else
							{
								this.m_subsystemItemsScanner.TryModifyItem(scannedItemData, block.GetDamageDestructionValue(scannedItemData.Value));
							}
						}
					}
				}
				this.m_rotStep += num;
				this.m_lastRotTime += (double)((float)num * 60f);
			}
		}

		// Token: 0x0400060E RID: 1550
		public const int MaxRot = 1;

		// Token: 0x0400060F RID: 1551
		public SubsystemItemsScanner m_subsystemItemsScanner;

		// Token: 0x04000610 RID: 1552
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000611 RID: 1553
		public double m_lastRotTime;

		// Token: 0x04000612 RID: 1554
		public int m_rotStep;

		// Token: 0x04000613 RID: 1555
		public const float m_rotPeriod = 60f;

		// Token: 0x04000614 RID: 1556
		public bool m_isRotEnabled;
	}
}
