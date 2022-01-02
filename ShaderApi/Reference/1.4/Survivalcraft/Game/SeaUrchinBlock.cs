using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000EA RID: 234
	public class SeaUrchinBlock : BottomSuckerBlock
	{
		// Token: 0x06000489 RID: 1161 RVA: 0x00019B48 File Offset: 0x00017D48
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/SeaUrchin", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Urchin", true).ParentBone);
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
					Matrix m = Matrix.CreateRotationX(1.57079637f) * Matrix.CreateRotationZ(0.3f + 2f * (float)j) * Matrix.CreateTranslation(SeaUrchinBlock.m_offsets[j].X, SeaUrchinBlock.m_offsets[j].Y, -0.49f) * Matrix.CreateRotationX(zero.X) * Matrix.CreateRotationY(zero.Y) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f);
					int num = 4 * i + j;
					this.m_blockMeshes[num] = new BlockMesh();
					this.m_blockMeshes[num].AppendModelMeshPart(model.FindMesh("Urchin", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
					this.m_collisionBoxes[num] = new BoundingBox[]
					{
						this.m_blockMeshes[num].CalculateBoundingBox()
					};
				}
			}
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Urchin", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bottom", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00019D98 File Offset: 0x00017F98
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int face = BottomSuckerBlock.GetFace(data);
			int subvariant = BottomSuckerBlock.GetSubvariant(data);
			return this.m_collisionBoxes[4 * face + subvariant];
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x00019DC4 File Offset: 0x00017FC4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int face = BottomSuckerBlock.GetFace(data);
			int subvariant = BottomSuckerBlock.GetSubvariant(data);
			Color color = SeaUrchinBlock.m_colors[subvariant];
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[4 * face + subvariant], color, null, geometry.SubsetOpaque);
			base.GenerateTerrainVertices(generator, geometry, value, x, y, z);
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x00019E28 File Offset: 0x00018028
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color * new Color(40, 40, 40), 3f * size, ref matrix, environmentData);
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x00019E53 File Offset: 0x00018053
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, 0.75f * strength, this.DestructionDebrisScale, new Color(64, 64, 64), this.DefaultTextureSlot);
		}

		// Token: 0x04000206 RID: 518
		public new const int Index = 226;

		// Token: 0x04000207 RID: 519
		public BlockMesh[] m_blockMeshes = new BlockMesh[24];

		// Token: 0x04000208 RID: 520
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x04000209 RID: 521
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[24][];

		// Token: 0x0400020A RID: 522
		public static Color[] m_colors = new Color[]
		{
			new Color(20, 20, 20),
			new Color(50, 20, 20),
			new Color(80, 30, 30),
			new Color(20, 20, 40)
		};

		// Token: 0x0400020B RID: 523
		public static Vector2[] m_offsets = new Vector2[]
		{
			0.15f * new Vector2(-0.8f, -1f),
			0.15f * new Vector2(1f, -0.75f),
			0.15f * new Vector2(-0.65f, 1f),
			0.15f * new Vector2(0.9f, 0.7f)
		};
	}
}
