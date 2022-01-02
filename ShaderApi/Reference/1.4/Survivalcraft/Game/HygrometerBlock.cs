using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200007D RID: 125
	public class HygrometerBlock : Block, IElectricElementBlock
	{
		// Token: 0x060002CE RID: 718 RVA: 0x000122BC File Offset: 0x000104BC
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Hygrometer", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Case", true).ParentBone);
			Matrix matrix = this.m_pointerMatrix = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Pointer", true).ParentBone);
			this.m_invPointerMatrix = Matrix.Invert(this.m_pointerMatrix);
			this.m_caseMesh.AppendModelMeshPart(model.FindMesh("Case", true).MeshParts[0], boneAbsoluteTransform, false, false, true, false, Color.White);
			this.m_pointerMesh.AppendModelMeshPart(model.FindMesh("Pointer", true).MeshParts[0], matrix, false, false, false, false, Color.White);
			for (int i = 0; i < 4; i++)
			{
				this.m_matricesByData[i] = Matrix.CreateScale(5f) * Matrix.CreateTranslation(0.95f, 0.15f, 0.5f) * Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationY((float)(i + 1) * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					this.m_caseMesh.CalculateBoundingBox(this.m_matricesByData[i])
				};
			}
			base.Initialize();
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00012444 File Offset: 0x00010644
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			return new HygrometerElectricElement(subsystemElectricity, new CellFace(x, y, z, num & 3));
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0001246C File Offset: 0x0001066C
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if ((Terrain.ExtractData(value) & 3) == face)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00012494 File Offset: 0x00010694
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0001249C File Offset: 0x0001069C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return null;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x000124C8 File Offset: 0x000106C8
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int value2 = 0;
			if (raycastResult.CellFace.Face == 0)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 121), 0);
			}
			if (raycastResult.CellFace.Face == 1)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 121), 1);
			}
			if (raycastResult.CellFace.Face == 2)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 121), 2);
			}
			if (raycastResult.CellFace.Face == 3)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 121), 3);
			}
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00012570 File Offset: 0x00010770
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_matricesByData.Length)
			{
				int humidity = generator.Terrain.GetHumidity(x, z);
				float radians = MathUtils.Lerp(1.5f, -1.5f, (float)humidity / 15f);
				Matrix matrix = this.m_matricesByData[num];
				Matrix value2 = this.m_invPointerMatrix * Matrix.CreateRotationX(radians) * this.m_pointerMatrix * matrix;
				generator.GenerateMeshVertices(this, x, y, z, this.m_caseMesh, Color.White, new Matrix?(matrix), geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_pointerMesh, Color.White, new Matrix?(value2), geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, num & 3, 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00012654 File Offset: 0x00010854
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
				float x = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalHumidity(num2, num3);
				float x2 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalHumidity(num2, num3 + 1);
				float x3 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalHumidity(num2 + 1, num3);
				float x4 = (float)environmentData.SubsystemTerrain.Terrain.GetSeasonalHumidity(num2 + 1, num3 + 1);
				float x5 = MathUtils.Lerp(x, x2, f2);
				float x6 = MathUtils.Lerp(x3, x4, f2);
				num = MathUtils.Lerp(x5, x6, f);
			}
			float radians = MathUtils.Lerp(1.5f, -1.5f, num / 15f);
			Matrix m = Matrix.CreateScale(7f * size) * Matrix.CreateTranslation(0f, -0.1f, 0f) * matrix;
			Matrix matrix2 = this.m_invPointerMatrix * Matrix.CreateRotationX(radians) * this.m_pointerMatrix * m;
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_caseMesh, color, 1f, ref m, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_pointerMesh, color, 1f, ref matrix2, environmentData);
		}

		// Token: 0x04000143 RID: 323
		public const int Index = 121;

		// Token: 0x04000144 RID: 324
		public BlockMesh m_caseMesh = new BlockMesh();

		// Token: 0x04000145 RID: 325
		public BlockMesh m_pointerMesh = new BlockMesh();

		// Token: 0x04000146 RID: 326
		public Matrix m_pointerMatrix;

		// Token: 0x04000147 RID: 327
		public Matrix m_invPointerMatrix;

		// Token: 0x04000148 RID: 328
		public Matrix[] m_matricesByData = new Matrix[4];

		// Token: 0x04000149 RID: 329
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[4][];
	}
}
