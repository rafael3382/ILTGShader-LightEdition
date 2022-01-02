using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000FD RID: 253
	public class StarfishBlock : BottomSuckerBlock
	{
		// Token: 0x0600050D RID: 1293 RVA: 0x0001D154 File Offset: 0x0001B354
		public override void Initialize()
		{
			int num = 63;
			Model model = ContentManager.Get<Model>("Models/Starfish", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Starfish", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bottom", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Vector2 zero = Vector2.Zero;
					if (i < 4)
					{
						zero.Y = (float)i * 3.14159274f / 2f;
					}
					else
					{
						zero.X = ((i == 4) ? -1.57079637f : 1.57079637f);
					}
					Matrix m = Matrix.CreateRotationX(1.57079637f) * Matrix.CreateRotationZ(0.3f + 2f * (float)j) * Matrix.CreateTranslation(StarfishBlock.m_offsets[j].X, StarfishBlock.m_offsets[j].Y, -0.49f) * Matrix.CreateRotationX(zero.X) * Matrix.CreateRotationY(zero.Y) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f);
					int num2 = 4 * i + j;
					this.m_blockMeshes[num2] = new BlockMesh();
					this.m_blockMeshes[num2].AppendModelMeshPart(model.FindMesh("Starfish", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
					this.m_blockMeshes[num2].TransformTextureCoordinates(Matrix.CreateTranslation((float)(num % 16) / 16f, (float)(num / 16) / 16f, 0f), -1);
					this.m_collisionBoxes[num2] = new BoundingBox[]
					{
						this.m_blockMeshes[num2].CalculateBoundingBox()
					};
				}
			}
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Starfish", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bottom", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(num % 16) / 16f, (float)(num / 16) / 16f, 0f), -1);
			base.Initialize();
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x0001D40C File Offset: 0x0001B60C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int face = BottomSuckerBlock.GetFace(data);
			int subvariant = BottomSuckerBlock.GetSubvariant(data);
			return this.m_collisionBoxes[4 * face + subvariant];
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x0001D438 File Offset: 0x0001B638
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int face = BottomSuckerBlock.GetFace(data);
			int subvariant = BottomSuckerBlock.GetSubvariant(data);
			Color color = StarfishBlock.m_colors[subvariant];
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[4 * face + subvariant], color, null, geometry.SubsetOpaque);
			base.GenerateTerrainVertices(generator, geometry, value, x, y, z);
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0001D49C File Offset: 0x0001B69C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color * StarfishBlock.m_colors[0], 3f * size, ref matrix, environmentData);
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x0001D4C7 File Offset: 0x0001B6C7
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, 0.75f * strength, this.DestructionDebrisScale, new Color(64, 64, 64), this.DefaultTextureSlot);
		}

		// Token: 0x04000236 RID: 566
		public new const int Index = 229;

		// Token: 0x04000237 RID: 567
		public BlockMesh[] m_blockMeshes = new BlockMesh[24];

		// Token: 0x04000238 RID: 568
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x04000239 RID: 569
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[24][];

		// Token: 0x0400023A RID: 570
		public static Color[] m_colors = new Color[]
		{
			new Color(100, 40, 20),
			new Color(90, 30, 20),
			new Color(100, 30, 30),
			new Color(80, 20, 10)
		};

		// Token: 0x0400023B RID: 571
		public static Vector2[] m_offsets = new Vector2[]
		{
			0.15f * new Vector2(-0.8f, -1f),
			0.15f * new Vector2(1f, -0.75f),
			0.15f * new Vector2(-0.65f, 1f),
			0.15f * new Vector2(0.9f, 0.7f)
		};
	}
}
