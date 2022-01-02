using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200002B RID: 43
	public class CampfireBlock : Block
	{
		// Token: 0x06000139 RID: 313 RVA: 0x00009ED8 File Offset: 0x000080D8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Campfire", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Wood", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Ashes", true).ParentBone);
			for (int i = 0; i < 16; i++)
			{
				this.m_meshesByData[i] = new BlockMesh();
				if (i == 0)
				{
					this.m_meshesByData[i].AppendModelMeshPart(model.FindMesh("Ashes", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateScale(3f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
				}
				else
				{
					float scale = MathUtils.Lerp(1.5f, 4f, (float)i / 15f);
					float radians = (float)i * 3.14159274f / 2f;
					this.m_meshesByData[i].AppendModelMeshPart(model.FindMesh("Wood", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(scale) * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
					this.m_meshesByData[i].AppendModelMeshPart(model.FindMesh("Ashes", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateScale(scale) * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
				}
				BoundingBox boundingBox = this.m_meshesByData[i].CalculateBoundingBox();
				boundingBox.Min.X = 0f;
				boundingBox.Min.Z = 0f;
				boundingBox.Max.X = 1f;
				boundingBox.Max.Z = 1f;
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					boundingBox
				};
			}
			this.m_standaloneMesh.AppendModelMeshPart(model.FindMesh("Wood", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(3f) * Matrix.CreateTranslation(0f, 0f, 0f), false, false, true, false, Color.White);
			this.m_standaloneMesh.AppendModelMeshPart(model.FindMesh("Ashes", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateScale(3f) * Matrix.CreateTranslation(0f, 0f, 0f), false, false, true, false, Color.White);
			base.Initialize();
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000A1B0 File Offset: 0x000083B0
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return null;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000A1DC File Offset: 0x000083DC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_meshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_meshesByData[num], Color.White, null, geometry.SubsetOpaque);
			}
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000A224 File Offset: 0x00008424
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000A23C File Offset: 0x0000843C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				CellFace = raycastResult.CellFace,
				Value = Terrain.MakeBlockValue(209, 0, 3)
			};
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000A273 File Offset: 0x00008473
		public override bool ShouldAvoid(int value)
		{
			return Terrain.ExtractData(value) > 0;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000A280 File Offset: 0x00008480
		public override int GetEmittedLightAmount(int value)
		{
			int num = Terrain.ExtractData(value);
			if (num > 0)
			{
				return MathUtils.Min(8 + num / 2, 15);
			}
			return 0;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000A2A6 File Offset: 0x000084A6
		public override float GetHeat(int value)
		{
			if (Terrain.ExtractData(value) <= 0)
			{
				return 0f;
			}
			return base.GetHeat(value);
		}

		// Token: 0x0400009F RID: 159
		public const int Index = 209;

		// Token: 0x040000A0 RID: 160
		public BlockMesh[] m_meshesByData = new BlockMesh[16];

		// Token: 0x040000A1 RID: 161
		public BlockMesh m_standaloneMesh = new BlockMesh();

		// Token: 0x040000A2 RID: 162
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[16][];
	}
}
