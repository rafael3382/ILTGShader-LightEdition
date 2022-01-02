using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000A6 RID: 166
	public class MagnetBlock : Block
	{
		// Token: 0x06000360 RID: 864 RVA: 0x00014C60 File Offset: 0x00012E60
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Magnet", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Magnet", true).ParentBone);
			for (int i = 0; i < 2; i++)
			{
				this.m_meshesByData[i] = new BlockMesh();
				this.m_meshesByData[i].AppendModelMeshPart(model.FindMesh("Magnet", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(1.57079637f * (float)i) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, true, false, Color.White);
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					this.m_meshesByData[i].CalculateBoundingBox()
				};
			}
			this.m_standaloneMesh.AppendModelMeshPart(model.FindMesh("Magnet", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(1.5f) * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, true, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000361 RID: 865 RVA: 0x00014D8C File Offset: 0x00012F8C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return null;
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00014DB8 File Offset: 0x00012FB8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_meshesByData[num], Color.White, null, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00014E00 File Offset: 0x00013000
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00014E18 File Offset: 0x00013018
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			if (componentMiner.Project.FindSubsystem<SubsystemMagnetBlockBehavior>(true).MagnetsCount < 8)
			{
				Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
				int data = (MathUtils.Abs(forward.X) <= MathUtils.Abs(forward.Z)) ? 1 : 0;
				return new BlockPlacementData
				{
					CellFace = raycastResult.CellFace,
					Value = Terrain.ReplaceData(value, data)
				};
			}
			ComponentPlayer componentPlayer = componentMiner.ComponentPlayer;
			if (componentPlayer != null)
			{
				componentPlayer.ComponentGui.DisplaySmallMessage("Too many magnets", Color.White, true, false);
			}
			return default(BlockPlacementData);
		}

		// Token: 0x0400018A RID: 394
		public const int Index = 167;

		// Token: 0x0400018B RID: 395
		public BlockMesh[] m_meshesByData = new BlockMesh[2];

		// Token: 0x0400018C RID: 396
		public BlockMesh m_standaloneMesh = new BlockMesh();

		// Token: 0x0400018D RID: 397
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[2][];
	}
}
