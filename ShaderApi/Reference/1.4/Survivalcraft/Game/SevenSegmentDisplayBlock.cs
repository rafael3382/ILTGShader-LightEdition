using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000ED RID: 237
	public class SevenSegmentDisplayBlock : MountedElectricElementBlock
	{
		// Token: 0x06000497 RID: 1175 RVA: 0x0001A310 File Offset: 0x00018510
		public override void Initialize()
		{
			ModelMesh modelMesh = ContentManager.Get<Model>("Models/Gates", null).FindMesh("SevenSegmentDisplay", true);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			for (int i = 0; i < 4; i++)
			{
				Matrix m = Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f);
				this.m_blockMeshesByFace[i] = new BlockMesh();
				this.m_blockMeshesByFace[i].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_collisionBoxesByFace[i] = new BoundingBox[]
				{
					this.m_blockMeshesByFace[i].CalculateBoundingBox()
				};
			}
			Matrix m2 = Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateRotationZ(1.57079637f);
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneBlockMesh.AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0001A455 File Offset: 0x00018655
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			int num;
			for (int color = 0; color < 8; color = num)
			{
				CraftingRecipe craftingRecipe = new CraftingRecipe
				{
					ResultCount = 4,
					ResultValue = Terrain.MakeBlockValue(185, 0, SevenSegmentDisplayBlock.SetColor(0, color)),
					RemainsCount = 1,
					RemainsValue = Terrain.MakeBlockValue(90),
					RequiredHeatLevel = 0f,
					Description = "用铜玻璃和颜料来制作7段显示器"
				};
				craftingRecipe.Ingredients[0] = "glass";
				craftingRecipe.Ingredients[2] = "glass";
				craftingRecipe.Ingredients[4] = "paintbucket:" + color.ToString(CultureInfo.InvariantCulture);
				craftingRecipe.Ingredients[6] = "copperingot";
				craftingRecipe.Ingredients[7] = "copperingot";
				craftingRecipe.Ingredients[8] = "copperingot";
				yield return craftingRecipe;
				num = color + 1;
			}
			yield break;
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001A460 File Offset: 0x00018660
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int mountingFace = SevenSegmentDisplayBlock.GetMountingFace(Terrain.ExtractData(value));
			return face != CellFace.OppositeFace(mountingFace);
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0001A485 File Offset: 0x00018685
		public override int GetFace(int value)
		{
			return SevenSegmentDisplayBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0001A494 File Offset: 0x00018694
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			return LanguageControl.GetWorldPalette(SevenSegmentDisplayBlock.GetColor(data)) + LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, data.ToString()), "DisplayName");
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0001A4DE File Offset: 0x000186DE
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < 8; i = num)
			{
				yield return Terrain.MakeBlockValue(185, 0, SevenSegmentDisplayBlock.SetColor(0, i));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0001A4E8 File Offset: 0x000186E8
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			if (raycastResult.CellFace.Face < 4)
			{
				int data = SevenSegmentDisplayBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
				int value2 = Terrain.ReplaceData(value, data);
				return new BlockPlacementData
				{
					Value = value2,
					CellFace = raycastResult.CellFace
				};
			}
			return default(BlockPlacementData);
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x0001A550 File Offset: 0x00018750
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int color = SevenSegmentDisplayBlock.GetColor(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(185, 0, SevenSegmentDisplayBlock.SetColor(0, color)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x0001A5A0 File Offset: 0x000187A0
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int mountingFace = SevenSegmentDisplayBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace >= this.m_collisionBoxesByFace.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByFace[mountingFace];
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0001A5D0 File Offset: 0x000187D0
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int mountingFace = SevenSegmentDisplayBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace < this.m_blockMeshesByFace.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByFace[mountingFace], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, mountingFace, 1f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x0001A63B File Offset: 0x0001883B
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x0001A656 File Offset: 0x00018856
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new SevenSegmentDisplayElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0001A670 File Offset: 0x00018870
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0001A6AC File Offset: 0x000188AC
		public static int GetMountingFace(int data)
		{
			return data & 3;
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x0001A6B1 File Offset: 0x000188B1
		public static int SetMountingFace(int data, int face)
		{
			return (data & -4) | (face & 3);
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0001A6BB File Offset: 0x000188BB
		public static int GetColor(int data)
		{
			return data >> 3 & 7;
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x0001A6C2 File Offset: 0x000188C2
		public static int SetColor(int data, int color)
		{
			return (data & -57) | (color & 7) << 3;
		}

		// Token: 0x0400020E RID: 526
		public const int Index = 185;

		// Token: 0x0400020F RID: 527
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x04000210 RID: 528
		public BlockMesh[] m_blockMeshesByFace = new BlockMesh[4];

		// Token: 0x04000211 RID: 529
		public BoundingBox[][] m_collisionBoxesByFace = new BoundingBox[4][];
	}
}
