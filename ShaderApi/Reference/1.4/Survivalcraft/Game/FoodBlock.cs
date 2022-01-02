using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200006B RID: 107
	public abstract class FoodBlock : Block
	{
		// Token: 0x0600025E RID: 606 RVA: 0x0000FBE0 File Offset: 0x0000DDE0
		public FoodBlock(string modelName, Matrix tcTransform, Color color, int rottenValue)
		{
			this.m_modelName = modelName;
			this.m_tcTransform = tcTransform;
			this.m_color = color;
			this.m_rottenValue = rottenValue;
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000FC10 File Offset: 0x0000DE10
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName, null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.Meshes[0].ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.Meshes[0].MeshParts[0], boneAbsoluteTransform, false, false, false, false, this.m_color);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(this.m_tcTransform, -1);
			base.Initialize();
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000FC8F File Offset: 0x0000DE8F
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000FC91 File Offset: 0x0000DE91
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000FCAC File Offset: 0x0000DEAC
		public override int GetDamageDestructionValue(int value)
		{
			return this.m_rottenValue;
		}

		// Token: 0x04000116 RID: 278
		public static int m_compostValue = Terrain.MakeBlockValue(168, 0, SoilBlock.SetHydration(SoilBlock.SetNitrogen(0, 1), false));

		// Token: 0x04000117 RID: 279
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000118 RID: 280
		public string m_modelName;

		// Token: 0x04000119 RID: 281
		public Matrix m_tcTransform;

		// Token: 0x0400011A RID: 282
		public Color m_color;

		// Token: 0x0400011B RID: 283
		public int m_rottenValue;
	}
}
