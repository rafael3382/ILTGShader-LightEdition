using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000099 RID: 153
	public abstract class LadderBlock : Block
	{
		// Token: 0x06000313 RID: 787 RVA: 0x00013685 File Offset: 0x00011885
		public LadderBlock(string modelName, float offset)
		{
			this.m_modelName = modelName;
			this.m_offset = offset;
		}

		// Token: 0x06000314 RID: 788 RVA: 0x000136C0 File Offset: 0x000118C0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName, null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Ladder", true).ParentBone);
			for (int i = 0; i < 4; i++)
			{
				this.m_blockMeshesByData[i] = new BlockMesh();
				Matrix m = Matrix.CreateTranslation(0f, 0f, 0f - (0.5f - this.m_offset)) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_blockMeshesByData[i].AppendModelMeshPart(model.FindMesh("Ladder", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_blockMeshesByData[i].GenerateSidesData();
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					this.m_blockMeshesByData[i].CalculateBoundingBox()
				};
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Ladder", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0001381C File Offset: 0x00011A1C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00013864 File Offset: 0x00011A64
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0001387C File Offset: 0x00011A7C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = ((raycastResult.CellFace.Face < 4) ? Terrain.MakeBlockValue(this.BlockIndex, 0, LadderBlock.SetFace(0, raycastResult.CellFace.Face)) : 0),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000318 RID: 792 RVA: 0x000138D8 File Offset: 0x00011AD8
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return base.GetCustomCollisionBoxes(terrain, value);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00013908 File Offset: 0x00011B08
		public static int GetFace(int data)
		{
			return data & 3;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0001390D File Offset: 0x00011B0D
		public static int SetFace(int data, int face)
		{
			return (data & -4) | (face & 3);
		}

		// Token: 0x04000167 RID: 359
		public string m_modelName;

		// Token: 0x04000168 RID: 360
		public float m_offset;

		// Token: 0x04000169 RID: 361
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400016A RID: 362
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		// Token: 0x0400016B RID: 363
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[4][];
	}
}
