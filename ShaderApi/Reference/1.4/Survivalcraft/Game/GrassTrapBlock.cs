using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000077 RID: 119
	public class GrassTrapBlock : Block
	{
		// Token: 0x060002AB RID: 683 RVA: 0x00011610 File Offset: 0x0000F810
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/GrassTrap", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("GrassTrap", true).ParentBone);
			Color color = BlockColorsMap.GrassColorsMap.Lookup(8, 15);
			this.m_blockMesh.AppendModelMeshPart(model.FindMesh("GrassTrap", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0.75f, 0.5f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("GrassTrap", true).MeshParts[0], boneAbsoluteTransform, false, false, false, false, color);
			this.m_collisionBoxes[0] = new BoundingBox(new Vector3(0f, 0.75f, 0f), new Vector3(1f, 0.95f, 1f));
			base.Initialize();
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00011700 File Offset: 0x0000F900
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMesh, BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x, y, z), null, null, geometry.SubsetAlphaTest);
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00011746 File Offset: 0x0000F946
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0001175C File Offset: 0x0000F95C
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			Color color = BlockColorsMap.GrassColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x060002AF RID: 687 RVA: 0x000117B3 File Offset: 0x0000F9B3
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x04000130 RID: 304
		public const int Index = 87;

		// Token: 0x04000131 RID: 305
		public BlockMesh m_blockMesh = new BlockMesh();

		// Token: 0x04000132 RID: 306
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000133 RID: 307
		public BoundingBox[] m_collisionBoxes = new BoundingBox[1];
	}
}
