using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000097 RID: 151
	public class JackOLanternBlock : Block
	{
		// Token: 0x06000308 RID: 776 RVA: 0x00013244 File Offset: 0x00011444
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Pumpkins", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("JackOLantern", true).ParentBone);
			for (int i = 0; i < 4; i++)
			{
				float radians = (float)i * 3.14159274f / 2f;
				BlockMesh blockMesh = new BlockMesh();
				blockMesh.AppendModelMeshPart(model.FindMesh("JackOLantern", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, new Color(232, 232, 232));
				blockMesh.AppendModelMeshPart(model.FindMesh("JackOLantern", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), true, true, false, false, Color.White);
				this.m_blockMeshesByData[i] = blockMesh;
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("JackOLantern", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.23f, 0f), false, false, false, false, new Color(232, 232, 232));
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("JackOLantern", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.23f, 0f), true, true, false, false, Color.White);
			this.m_collisionBoxes[0] = this.m_blockMeshesByData[0].CalculateBoundingBox();
			base.Initialize();
		}

		// Token: 0x06000309 RID: 777 RVA: 0x00013418 File Offset: 0x00011618
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int data = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				data = 0;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 1;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 2;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 3;
			}
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, 132), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600030A RID: 778 RVA: 0x000134F2 File Offset: 0x000116F2
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x0600030B RID: 779 RVA: 0x000134FC File Offset: 0x000116FC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetAlphaTest);
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00013544 File Offset: 0x00011744
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000162 RID: 354
		public const int Index = 132;

		// Token: 0x04000163 RID: 355
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		// Token: 0x04000164 RID: 356
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000165 RID: 357
		public BoundingBox[] m_collisionBoxes = new BoundingBox[1];
	}
}
