using System;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000110 RID: 272
	public class ThermometerBlock : Block, IElectricElementBlock
	{
		// Token: 0x0600054B RID: 1355 RVA: 0x0001E798 File Offset: 0x0001C998
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Thermometer", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Case", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Fluid", true).ParentBone);
			this.m_caseMesh.AppendModelMeshPart(model.FindMesh("Case", true).MeshParts[0], boneAbsoluteTransform, false, false, true, false, Color.White);
			this.m_fluidMesh.AppendModelMeshPart(model.FindMesh("Fluid", true).MeshParts[0], boneAbsoluteTransform2, false, false, false, false, Color.White);
			for (int i = 0; i < 4; i++)
			{
				this.m_matricesByData[i] = Matrix.CreateScale(1.5f) * Matrix.CreateTranslation(0.95f, 0.15f, 0.5f) * Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationY((float)(i + 1) * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					this.m_caseMesh.CalculateBoundingBox(this.m_matricesByData[i])
				};
			}
			this.m_fluidBottomPosition = this.m_fluidMesh.Vertices.Min((BlockMeshVertex v) => v.Position.Y);
			base.Initialize();
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0001E93C File Offset: 0x0001CB3C
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			return new ThermometerElectricElement(subsystemElectricity, new CellFace(x, y, z, num & 3));
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0001E964 File Offset: 0x0001CB64
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if ((Terrain.ExtractData(value) & 3) == face)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0001E98C File Offset: 0x0001CB8C
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0001E994 File Offset: 0x0001CB94
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return null;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x0001E9C0 File Offset: 0x0001CBC0
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int value2 = 0;
			if (raycastResult.CellFace.Face == 0)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 120), 0);
			}
			if (raycastResult.CellFace.Face == 1)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 120), 1);
			}
			if (raycastResult.CellFace.Face == 2)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 120), 2);
			}
			if (raycastResult.CellFace.Face == 3)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 120), 3);
			}
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x0001EA68 File Offset: 0x0001CC68
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_matricesByData.Length)
			{
				int num2 = (generator.SubsystemMetersBlockBehavior != null) ? generator.SubsystemMetersBlockBehavior.GetThermometerReading(x, y, z) : 8;
				float y2 = MathUtils.Lerp(1f, 4f, (float)num2 / 15f);
				Matrix matrix = this.m_matricesByData[num];
				Matrix value2 = Matrix.CreateTranslation(0f, 0f - this.m_fluidBottomPosition, 0f) * Matrix.CreateScale(1f, y2, 1f) * Matrix.CreateTranslation(0f, this.m_fluidBottomPosition, 0f) * matrix;
				generator.GenerateMeshVertices(this, x, y, z, this.m_caseMesh, Color.White, new Matrix?(matrix), geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_fluidMesh, Color.White, new Matrix?(value2), geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, num & 3, 0.2f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0001EB88 File Offset: 0x0001CD88
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			float num = 8f;
			if (environmentData != null && environmentData.SubsystemTerrain != null)
			{
				Vector3 translation = environmentData.InWorldMatrix.Translation;
				int num2 = Terrain.ToCell(translation.X);
				int num3 = Terrain.ToCell(translation.Z);
				float f = translation.X - (float)num2;
				float f2 = translation.Z - (float)num3;
				float x = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalTemperature(num2, num3);
				float x2 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalTemperature(num2, num3 + 1);
				float x3 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalTemperature(num2 + 1, num3);
				float x4 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalTemperature(num2 + 1, num3 + 1);
				float x5 = MathUtils.Lerp(x, x2, f2);
				float x6 = MathUtils.Lerp(x3, x4, f2);
				num = MathUtils.Lerp(x5, x6, f);
			}
			float y = MathUtils.Lerp(1f, 4f, num / 15f);
			Matrix m = Matrix.CreateScale(3f * size) * Matrix.CreateTranslation(0f, -0.15f, 0f) * matrix;
			Matrix matrix2 = Matrix.CreateTranslation(0f, 0f - this.m_fluidBottomPosition, 0f) * Matrix.CreateScale(1f, y, 1f) * Matrix.CreateTranslation(0f, this.m_fluidBottomPosition, 0f) * m;
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_caseMesh, color, 1f, ref m, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_fluidMesh, color, 1f, ref matrix2, environmentData);
		}

		// Token: 0x04000255 RID: 597
		public const int Index = 120;

		// Token: 0x04000256 RID: 598
		public BlockMesh m_caseMesh = new BlockMesh();

		// Token: 0x04000257 RID: 599
		public BlockMesh m_fluidMesh = new BlockMesh();

		// Token: 0x04000258 RID: 600
		public Matrix[] m_matricesByData = new Matrix[4];

		// Token: 0x04000259 RID: 601
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[4][];

		// Token: 0x0400025A RID: 602
		public float m_fluidBottomPosition;
	}
}
