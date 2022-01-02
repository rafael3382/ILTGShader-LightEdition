using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000011 RID: 17
	public class ArrowBlock : Block
	{
		// Token: 0x060000B2 RID: 178 RVA: 0x00007A18 File Offset: 0x00005C18
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Arrows", null);
			foreach (int num in EnumUtils.GetEnumValues(typeof(ArrowBlock.ArrowType)))
			{
				if (num > 15)
				{
					throw new InvalidOperationException("Too many arrow types.");
				}
				Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(ArrowBlock.m_shaftNames[num], true).ParentBone);
				Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(ArrowBlock.m_stabilizerNames[num], true).ParentBone);
				Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(ArrowBlock.m_tipNames[num], true).ParentBone);
				BlockMesh blockMesh = new BlockMesh();
				blockMesh.AppendModelMeshPart(model.FindMesh(ArrowBlock.m_tipNames[num], true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, ArrowBlock.m_offsets[num], 0f), false, false, false, false, Color.White);
				blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(ArrowBlock.m_tipTextureSlots[num] % 16) / 16f, (float)(ArrowBlock.m_tipTextureSlots[num] / 16) / 16f, 0f), -1);
				BlockMesh blockMesh2 = new BlockMesh();
				blockMesh2.AppendModelMeshPart(model.FindMesh(ArrowBlock.m_shaftNames[num], true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, ArrowBlock.m_offsets[num], 0f), false, false, false, false, Color.White);
				blockMesh2.TransformTextureCoordinates(Matrix.CreateTranslation((float)(ArrowBlock.m_shaftTextureSlots[num] % 16) / 16f, (float)(ArrowBlock.m_shaftTextureSlots[num] / 16) / 16f, 0f), -1);
				BlockMesh blockMesh3 = new BlockMesh();
				blockMesh3.AppendModelMeshPart(model.FindMesh(ArrowBlock.m_stabilizerNames[num], true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, ArrowBlock.m_offsets[num], 0f), false, false, true, false, Color.White);
				blockMesh3.TransformTextureCoordinates(Matrix.CreateTranslation((float)(ArrowBlock.m_stabilizerTextureSlots[num] % 16) / 16f, (float)(ArrowBlock.m_stabilizerTextureSlots[num] / 16) / 16f, 0f), -1);
				BlockMesh blockMesh4 = new BlockMesh();
				blockMesh4.AppendBlockMesh(blockMesh);
				blockMesh4.AppendBlockMesh(blockMesh2);
				blockMesh4.AppendBlockMesh(blockMesh3);
				this.m_standaloneBlockMeshes.Add(blockMesh4);
			}
			base.Initialize();
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00007CA8 File Offset: 0x00005EA8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00007CAC File Offset: 0x00005EAC
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int arrowType = (int)ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (arrowType >= 0 && arrowType < this.m_standaloneBlockMeshes.Count)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshes[arrowType], color, 2f * size, ref matrix, environmentData);
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00007CF8 File Offset: 0x00005EF8
		public override float GetProjectilePower(int value)
		{
			int arrowType = (int)ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (arrowType < 0 || arrowType >= ArrowBlock.m_weaponPowers.Length)
			{
				return 0f;
			}
			return ArrowBlock.m_weaponPowers[arrowType];
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00007D2C File Offset: 0x00005F2C
		public override float GetExplosionPressure(int value)
		{
			int arrowType = (int)ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (arrowType < 0 || arrowType >= ArrowBlock.m_explosionPressures.Length)
			{
				return 0f;
			}
			return ArrowBlock.m_explosionPressures[arrowType];
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00007D60 File Offset: 0x00005F60
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			int arrowType = (int)ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (arrowType < 0 || arrowType >= ArrowBlock.m_iconViewScales.Length)
			{
				return 1f;
			}
			return ArrowBlock.m_iconViewScales[arrowType];
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00007D94 File Offset: 0x00005F94
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < ArrowBlock.m_order.Length; i = num)
			{
				yield return Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, (ArrowBlock.ArrowType)ArrowBlock.m_order[i]));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00007DA0 File Offset: 0x00005FA0
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int arrowType = (int)ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (arrowType < 0 || arrowType >= ArrowBlock.m_displayNames.Length)
			{
				return string.Empty;
			}
			return ArrowBlock.m_displayNames[arrowType];
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00007DD4 File Offset: 0x00005FD4
		public static ArrowBlock.ArrowType GetArrowType(int data)
		{
			return (ArrowBlock.ArrowType)(data & 15);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00007DDA File Offset: 0x00005FDA
		public static int SetArrowType(int data, ArrowBlock.ArrowType arrowType)
		{
			return (data & -16) | (int)(arrowType & (ArrowBlock.ArrowType)15);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00007DF8 File Offset: 0x00005FF8
		// Note: this type is marked as 'beforefieldinit'.
		static ArrowBlock()
		{
			float[] array = new float[9];
			array[7] = 40f;
			ArrowBlock.m_explosionPressures = array;
		}

		// Token: 0x0400005B RID: 91
		public const int Index = 192;

		// Token: 0x0400005C RID: 92
		public List<BlockMesh> m_standaloneBlockMeshes = new List<BlockMesh>();

		// Token: 0x0400005D RID: 93
		public static int[] m_order = new int[]
		{
			0,
			1,
			8,
			2,
			3,
			4,
			5,
			6,
			7
		};

		// Token: 0x0400005E RID: 94
		public static string[] m_tipNames = new string[]
		{
			"ArrowTip",
			"ArrowTip",
			"ArrowTip",
			"ArrowTip",
			"ArrowFireTip",
			"BoltTip",
			"BoltTip",
			"BoltExplosiveTip",
			"ArrowTip"
		};

		// Token: 0x0400005F RID: 95
		public static int[] m_tipTextureSlots = new int[]
		{
			47,
			1,
			63,
			182,
			62,
			63,
			182,
			183,
			79
		};

		// Token: 0x04000060 RID: 96
		public static string[] m_shaftNames = new string[]
		{
			"ArrowShaft",
			"ArrowShaft",
			"ArrowShaft",
			"ArrowShaft",
			"ArrowShaft",
			"BoltShaft",
			"BoltShaft",
			"BoltShaft",
			"ArrowShaft"
		};

		// Token: 0x04000061 RID: 97
		public static int[] m_shaftTextureSlots = new int[]
		{
			4,
			4,
			4,
			4,
			4,
			63,
			63,
			63,
			4
		};

		// Token: 0x04000062 RID: 98
		public static string[] m_stabilizerNames = new string[]
		{
			"ArrowStabilizer",
			"ArrowStabilizer",
			"ArrowStabilizer",
			"ArrowStabilizer",
			"ArrowStabilizer",
			"BoltStabilizer",
			"BoltStabilizer",
			"BoltStabilizer",
			"ArrowStabilizer"
		};

		// Token: 0x04000063 RID: 99
		public static int[] m_stabilizerTextureSlots = new int[]
		{
			15,
			15,
			15,
			15,
			15,
			63,
			63,
			63,
			15
		};

		// Token: 0x04000064 RID: 100
		public static string[] m_displayNames = new string[]
		{
			"木尖箭头",
			"石尖箭头",
			"铁尖箭头",
			"钻石尖箭头",
			"火尖箭头",
			"铁螺栓",
			"钻石尖螺栓",
			"爆炸螺栓",
			"铜尖箭头"
		};

		// Token: 0x04000065 RID: 101
		public static float[] m_offsets = new float[]
		{
			-0.5f,
			-0.5f,
			-0.5f,
			-0.5f,
			-0.5f,
			-0.3f,
			-0.3f,
			-0.3f,
			-0.5f
		};

		// Token: 0x04000066 RID: 102
		public static float[] m_weaponPowers = new float[]
		{
			5f,
			7f,
			14f,
			18f,
			4f,
			28f,
			36f,
			8f,
			10f
		};

		// Token: 0x04000067 RID: 103
		public static float[] m_iconViewScales = new float[]
		{
			0.8f,
			0.8f,
			0.8f,
			0.8f,
			0.8f,
			1.1f,
			1.1f,
			1.1f,
			0.8f
		};

		// Token: 0x04000068 RID: 104
		public static float[] m_explosionPressures;

		// Token: 0x020003E2 RID: 994
		public enum ArrowType
		{
			// Token: 0x0400143F RID: 5183
			WoodenArrow,
			// Token: 0x04001440 RID: 5184
			StoneArrow,
			// Token: 0x04001441 RID: 5185
			IronArrow,
			// Token: 0x04001442 RID: 5186
			DiamondArrow,
			// Token: 0x04001443 RID: 5187
			FireArrow,
			// Token: 0x04001444 RID: 5188
			IronBolt,
			// Token: 0x04001445 RID: 5189
			DiamondBolt,
			// Token: 0x04001446 RID: 5190
			ExplosiveBolt,
			// Token: 0x04001447 RID: 5191
			CopperArrow
		}
	}
}
