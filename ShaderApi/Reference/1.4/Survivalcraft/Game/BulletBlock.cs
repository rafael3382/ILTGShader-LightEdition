using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000028 RID: 40
	public class BulletBlock : FlatBlock
	{
		// Token: 0x06000120 RID: 288 RVA: 0x000098C5 File Offset: 0x00007AC5
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000121 RID: 289 RVA: 0x000098C8 File Offset: 0x00007AC8
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int bulletType = (int)BulletBlock.GetBulletType(Terrain.ExtractData(value));
			float size2 = (bulletType >= 0 && bulletType < BulletBlock.m_sizes.Length) ? (size * BulletBlock.m_sizes[bulletType]) : size;
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size2, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00009910 File Offset: 0x00007B10
		public override float GetProjectilePower(int value)
		{
			int bulletType = (int)BulletBlock.GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= BulletBlock.m_weaponPowers.Length)
			{
				return 0f;
			}
			return BulletBlock.m_weaponPowers[bulletType];
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00009944 File Offset: 0x00007B44
		public override float GetExplosionPressure(int value)
		{
			int bulletType = (int)BulletBlock.GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= BulletBlock.m_explosionPressures.Length)
			{
				return 0f;
			}
			return BulletBlock.m_explosionPressures[bulletType];
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00009978 File Offset: 0x00007B78
		public override IEnumerable<int> GetCreativeValues()
		{
			foreach (int bulletType in EnumUtils.GetEnumValues(typeof(BulletBlock.BulletType)))
			{
				yield return Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, (BulletBlock.BulletType)bulletType));
			}
			IEnumerator<int> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00009984 File Offset: 0x00007B84
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int bulletType = (int)BulletBlock.GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= BulletBlock.m_displayNames.Length)
			{
				return string.Empty;
			}
			return BulletBlock.m_displayNames[bulletType];
		}

		// Token: 0x06000126 RID: 294 RVA: 0x000099B8 File Offset: 0x00007BB8
		public override int GetFaceTextureSlot(int face, int value)
		{
			int bulletType = (int)BulletBlock.GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= BulletBlock.m_textureSlots.Length)
			{
				return 229;
			}
			return BulletBlock.m_textureSlots[bulletType];
		}

		// Token: 0x06000127 RID: 295 RVA: 0x000099EC File Offset: 0x00007BEC
		public static BulletBlock.BulletType GetBulletType(int data)
		{
			return (BulletBlock.BulletType)(data & 15);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x000099F2 File Offset: 0x00007BF2
		public static int SetBulletType(int data, BulletBlock.BulletType bulletType)
		{
			return (data & -16) | (int)(bulletType & (BulletBlock.BulletType)15);
		}

		// Token: 0x04000092 RID: 146
		public const int Index = 214;

		// Token: 0x04000093 RID: 147
		public static string[] m_displayNames = new string[]
		{
			"枪弹",
			"铅弹",
			"铅弹球"
		};

		// Token: 0x04000094 RID: 148
		public static float[] m_sizes = new float[]
		{
			1f,
			1f,
			0.33f
		};

		// Token: 0x04000095 RID: 149
		public static int[] m_textureSlots = new int[]
		{
			229,
			231,
			229
		};

		// Token: 0x04000096 RID: 150
		public static float[] m_weaponPowers = new float[]
		{
			80f,
			0f,
			3.6f
		};

		// Token: 0x04000097 RID: 151
		public static float[] m_explosionPressures = new float[3];

		// Token: 0x020003E5 RID: 997
		public enum BulletType
		{
			// Token: 0x04001451 RID: 5201
			MusketBall,
			// Token: 0x04001452 RID: 5202
			Buckshot,
			// Token: 0x04001453 RID: 5203
			BuckshotBall
		}
	}
}
