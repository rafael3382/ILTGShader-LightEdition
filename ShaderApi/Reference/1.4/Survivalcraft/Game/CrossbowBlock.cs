using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200004C RID: 76
	public class CrossbowBlock : Block
	{
		// Token: 0x06000194 RID: 404 RVA: 0x0000B79C File Offset: 0x0000999C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Crossbows", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Body", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("BowRelaxed", true).ParentBone);
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StringRelaxed", true).ParentBone);
			Matrix boneAbsoluteTransform4 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("BowTensed", true).ParentBone);
			Matrix boneAbsoluteTransform5 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StringTensed", true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("Body", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			blockMesh.AppendModelMeshPart(model.FindMesh("BowRelaxed", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			blockMesh.AppendModelMeshPart(model.FindMesh("StringRelaxed", true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			BlockMesh blockMesh2 = new BlockMesh();
			blockMesh2.AppendModelMeshPart(model.FindMesh("Body", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			blockMesh2.AppendModelMeshPart(model.FindMesh("BowTensed", true).MeshParts[0], boneAbsoluteTransform4 * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			blockMesh2.AppendModelMeshPart(model.FindMesh("StringTensed", true).MeshParts[0], boneAbsoluteTransform5 * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			for (int i = 0; i < 16; i++)
			{
				float factor = (float)i / 15f;
				this.m_standaloneBlockMeshes[i] = new BlockMesh();
				this.m_standaloneBlockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_standaloneBlockMeshes[i].BlendBlockMesh(blockMesh2, factor);
			}
			base.Initialize();
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000BA2A File Offset: 0x00009C2A
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000BA2C File Offset: 0x00009C2C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			int draw = CrossbowBlock.GetDraw(data);
			ArrowBlock.ArrowType? arrowType = CrossbowBlock.GetArrowType(data);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshes[draw], color, 2f * size, ref matrix, environmentData);
			if (arrowType != null)
			{
				Matrix matrix2 = Matrix.CreateRotationX(-1.57079637f) * Matrix.CreateTranslation(0f, 0.2f * size, -0.09f * size) * matrix;
				int value2 = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, arrowType.Value));
				BlocksManager.Blocks[192].DrawBlock(primitivesRenderer, value2, color, size, ref matrix2, environmentData);
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000BAD8 File Offset: 0x00009CD8
		public override int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 8 & 255;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000BAE8 File Offset: 0x00009CE8
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num &= -65281;
			num |= MathUtils.Clamp(damage, 0, 255) << 8;
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000BB1C File Offset: 0x00009D1C
		public override bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			int num = Terrain.ExtractContents(oldValue);
			int data = Terrain.ExtractData(oldValue);
			int data2 = Terrain.ExtractData(newValue);
			if (num == 200)
			{
				ArrowBlock.ArrowType? arrowType = CrossbowBlock.GetArrowType(data);
				ArrowBlock.ArrowType? arrowType2 = CrossbowBlock.GetArrowType(data2);
				if (arrowType.GetValueOrDefault() == arrowType2.GetValueOrDefault() & arrowType != null == (arrowType2 != null))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000BB78 File Offset: 0x00009D78
		public static ArrowBlock.ArrowType? GetArrowType(int data)
		{
			int num = data >> 4 & 15;
			if (num != 0)
			{
				return new ArrowBlock.ArrowType?((ArrowBlock.ArrowType)(num - 1));
			}
			return null;
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000BBA4 File Offset: 0x00009DA4
		public static int SetArrowType(int data, ArrowBlock.ArrowType? arrowType)
		{
			int num = (int)((arrowType != null) ? (arrowType.Value + 1) : ArrowBlock.ArrowType.WoodenArrow);
			return (data & -241) | (num & 15) << 4;
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000BBD5 File Offset: 0x00009DD5
		public static int GetDraw(int data)
		{
			return data & 15;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000BBDB File Offset: 0x00009DDB
		public static int SetDraw(int data, int draw)
		{
			return (data & -16) | (draw & 15);
		}

		// Token: 0x040000D0 RID: 208
		public const int Index = 200;

		// Token: 0x040000D1 RID: 209
		public BlockMesh[] m_standaloneBlockMeshes = new BlockMesh[16];
	}
}
