using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200007B RID: 123
	public abstract class GunpowderKegBlock : Block, IElectricElementBlock
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x00011EB7 File Offset: 0x000100B7
		// (set) Token: 0x060002C1 RID: 705 RVA: 0x00011EBF File Offset: 0x000100BF
		public Vector3 FuseOffset { get; set; }

		// Token: 0x060002C2 RID: 706 RVA: 0x00011EC8 File Offset: 0x000100C8
		public GunpowderKegBlock(string modelName, bool isIncendiary)
		{
			this.m_modelName = modelName;
			this.m_isIncendiary = isIncendiary;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00011EF4 File Offset: 0x000100F4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName, null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Keg", true).ParentBone);
			this.FuseOffset = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Fuse", true).ParentBone).Translation + new Vector3(0.5f, 0f, 0.5f);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("Keg", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_blockMesh.AppendBlockMesh(blockMesh);
			if (this.m_isIncendiary)
			{
				this.m_blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-0.25f, 0f, 0f), -1);
			}
			this.m_collisionBoxes = new BoundingBox[]
			{
				blockMesh.CalculateBoundingBox()
			};
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Keg", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			if (this.m_isIncendiary)
			{
				this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-0.25f, 0f, 0f), -1);
			}
			base.Initialize();
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00012070 File Offset: 0x00010270
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMesh, Color.White, null, geometry.SubsetOpaque);
			generator.GenerateWireVertices(value, x, y, z, 4, 0.25f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x000120C2 File Offset: 0x000102C2
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x000120D7 File Offset: 0x000102D7
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x000120DF File Offset: 0x000102DF
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new GunpowderKegElectricElement(subsystemElectricity, new CellFace(x, y, z, 4));
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x000120F4 File Offset: 0x000102F4
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (face == 4)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x00012115 File Offset: 0x00010315
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x0400013A RID: 314
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400013B RID: 315
		public BlockMesh m_blockMesh = new BlockMesh();

		// Token: 0x0400013C RID: 316
		public BoundingBox[] m_collisionBoxes;

		// Token: 0x0400013D RID: 317
		public string m_modelName;

		// Token: 0x0400013E RID: 318
		public bool m_isIncendiary;
	}
}
