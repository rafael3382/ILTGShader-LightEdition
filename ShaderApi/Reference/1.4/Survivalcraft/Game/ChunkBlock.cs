using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000032 RID: 50
	public abstract class ChunkBlock : Block
	{
		// Token: 0x06000160 RID: 352 RVA: 0x0000ADFE File Offset: 0x00008FFE
		public ChunkBlock(Matrix transform, Matrix tcTransform, Color color, bool smooth)
		{
			this.m_transform = transform;
			this.m_tcTransform = tcTransform;
			this.m_color = color;
			this.m_smooth = smooth;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000AE30 File Offset: 0x00009030
		public override void Initialize()
		{
			Model model = this.m_smooth ? ContentManager.Get<Model>("Models/ChunkSmooth", null) : ContentManager.Get<Model>("Models/Chunk", null);
			Matrix matrix = BlockMesh.GetBoneAbsoluteTransform(model.Meshes[0].ParentBone) * this.m_transform;
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.Meshes[0].MeshParts[0], matrix, false, false, false, false, this.m_color);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(this.m_tcTransform, -1);
			base.Initialize();
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000AECE File Offset: 0x000090CE
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000AED0 File Offset: 0x000090D0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040000AF RID: 175
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040000B0 RID: 176
		public Matrix m_transform;

		// Token: 0x040000B1 RID: 177
		public Matrix m_tcTransform;

		// Token: 0x040000B2 RID: 178
		public Color m_color;

		// Token: 0x040000B3 RID: 179
		public bool m_smooth;
	}
}
