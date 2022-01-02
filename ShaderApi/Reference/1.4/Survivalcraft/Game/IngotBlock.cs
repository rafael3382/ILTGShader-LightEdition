using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000083 RID: 131
	public abstract class IngotBlock : Block
	{
		// Token: 0x060002E8 RID: 744 RVA: 0x000128D1 File Offset: 0x00010AD1
		public IngotBlock(string meshName)
		{
			this.m_meshName = meshName;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x000128EC File Offset: 0x00010AEC
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Ingots", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(this.m_meshName, true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh(this.m_meshName, true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0001296C File Offset: 0x00010B6C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0001296E File Offset: 0x00010B6E
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0400014D RID: 333
		public string m_meshName;

		// Token: 0x0400014E RID: 334
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
