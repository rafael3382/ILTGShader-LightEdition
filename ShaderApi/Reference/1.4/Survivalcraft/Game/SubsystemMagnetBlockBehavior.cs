using System;
using System.Linq;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B7 RID: 439
	public class SubsystemMagnetBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000B46 RID: 2886 RVA: 0x0004BA70 File Offset: 0x00049C70
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					167
				};
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000B47 RID: 2887 RVA: 0x0004BA80 File Offset: 0x00049C80
		public int MagnetsCount
		{
			get
			{
				return this.m_magnets.Count;
			}
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x0004BA90 File Offset: 0x00049C90
		public Vector3 FindNearestCompassTarget(Vector3 compassPosition)
		{
			if (this.m_magnets.Count > 0)
			{
				float num = float.MaxValue;
				Vector3 v = Vector3.Zero;
				int num2 = 0;
				while (num2 < this.m_magnets.Count && num2 < 8)
				{
					Vector3 vector = this.m_magnets.Array[num2];
					float num3 = Vector3.DistanceSquared(compassPosition, vector);
					if (num3 < num)
					{
						num = num3;
						v = vector;
					}
					num2++;
				}
				return v + new Vector3(0.5f);
			}
			float num4 = float.MaxValue;
			Vector3 v2 = Vector3.Zero;
			foreach (PlayerData playerData in this.m_subsystemPlayers.PlayersData)
			{
				Vector3 spawnPosition = playerData.SpawnPosition;
				float num5 = Vector3.DistanceSquared(compassPosition, spawnPosition);
				if (num5 < num4)
				{
					num4 = num5;
					v2 = spawnPosition;
				}
			}
			return v2 + new Vector3(0.5f);
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x0004BB90 File Offset: 0x00049D90
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			string value = valuesDictionary.GetValue<string>("Magnets");
			this.m_magnets = new DynamicArray<Vector3>(HumanReadableConverter.ValuesListFromString<Vector3>(';', value));
		}

		// Token: 0x06000B4A RID: 2890 RVA: 0x0004BBD8 File Offset: 0x00049DD8
		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			string value = HumanReadableConverter.ValuesListToString<Vector3>(';', this.m_magnets.ToArray<Vector3>());
			valuesDictionary.SetValue<string>("Magnets", value);
		}

		// Token: 0x06000B4B RID: 2891 RVA: 0x0004BC0B File Offset: 0x00049E0B
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.m_magnets.Add(new Vector3((float)x, (float)y, (float)z));
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x0004BC25 File Offset: 0x00049E25
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.m_magnets.Remove(new Vector3((float)x, (float)y, (float)z));
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x0004BC40 File Offset: 0x00049E40
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x04000577 RID: 1399
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x04000578 RID: 1400
		public DynamicArray<Vector3> m_magnets = new DynamicArray<Vector3>();

		// Token: 0x04000579 RID: 1401
		public const int MaxMagnets = 8;
	}
}
