using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000020 RID: 32
	public class BowBlock : Block
	{
		// Token: 0x06000109 RID: 265 RVA: 0x000093A0 File Offset: 0x000075A0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Bows", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("BowRelaxed", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StringRelaxed", true).ParentBone);
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("BowTensed", true).ParentBone);
			Matrix boneAbsoluteTransform4 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StringTensed", true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("BowRelaxed", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh.AppendModelMeshPart(model.FindMesh("StringRelaxed", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			BlockMesh blockMesh2 = new BlockMesh();
			blockMesh2.AppendModelMeshPart(model.FindMesh("BowTensed", true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh2.AppendModelMeshPart(model.FindMesh("StringTensed", true).MeshParts[0], boneAbsoluteTransform4 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			for (int i = 0; i < 16; i++)
			{
				float factor = (float)i / 15f;
				this.m_standaloneBlockMeshes[i] = new BlockMesh();
				this.m_standaloneBlockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_standaloneBlockMeshes[i].BlendBlockMesh(blockMesh2, factor);
			}
			base.Initialize();
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000958B File Offset: 0x0000778B
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00009590 File Offset: 0x00007790
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			int draw = BowBlock.GetDraw(data);
			ArrowBlock.ArrowType? arrowType = BowBlock.GetArrowType(data);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshes[draw], color, 2f * size, ref matrix, environmentData);
			if (arrowType != null)
			{
				float num = MathUtils.Lerp(0.14f, 0.68f, (float)draw / 15f);
				Matrix matrix2 = Matrix.CreateRotationX(-1.57079637f) * Matrix.CreateTranslation(0f, 0.4f * size, (-1f + 2f * num) * size) * matrix;
				int value2 = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, arrowType.Value));
				BlocksManager.Blocks[192].DrawBlock(primitivesRenderer, value2, color, size, ref matrix2, environmentData);
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00009661 File Offset: 0x00007861
		public override int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 8 & 255;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00009674 File Offset: 0x00007874
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num &= -65281;
			num |= MathUtils.Clamp(damage, 0, 255) << 8;
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000096A8 File Offset: 0x000078A8
		public override bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			int num = Terrain.ExtractContents(oldValue);
			int data = Terrain.ExtractData(oldValue);
			int data2 = Terrain.ExtractData(newValue);
			if (num == 191)
			{
				ArrowBlock.ArrowType? arrowType = BowBlock.GetArrowType(data);
				ArrowBlock.ArrowType? arrowType2 = BowBlock.GetArrowType(data2);
				if (arrowType.GetValueOrDefault() == arrowType2.GetValueOrDefault() & arrowType != null == (arrowType2 != null))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00009704 File Offset: 0x00007904
		public static ArrowBlock.ArrowType? GetArrowType(int data)
		{
			int num = data >> 4 & 15;
			if (num != 0)
			{
				return new ArrowBlock.ArrowType?((ArrowBlock.ArrowType)(num - 1));
			}
			return null;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00009730 File Offset: 0x00007930
		public static int SetArrowType(int data, ArrowBlock.ArrowType? arrowType)
		{
			int num = (int)((arrowType != null) ? (arrowType.Value + 1) : ArrowBlock.ArrowType.WoodenArrow);
			return (data & -241) | (num & 15) << 4;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00009761 File Offset: 0x00007961
		public static int GetDraw(int data)
		{
			return data & 15;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00009767 File Offset: 0x00007967
		public static int SetDraw(int data, int draw)
		{
			return (data & -16) | (draw & 15);
		}

		// Token: 0x04000089 RID: 137
		public const int Index = 191;

		// Token: 0x0400008A RID: 138
		public BlockMesh[] m_standaloneBlockMeshes = new BlockMesh[16];
	}
}
