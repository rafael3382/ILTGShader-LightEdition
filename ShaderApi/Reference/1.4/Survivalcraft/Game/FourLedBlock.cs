using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200006C RID: 108
	public class FourLedBlock : MountedElectricElementBlock
	{
		// Token: 0x06000264 RID: 612 RVA: 0x0000FCD4 File Offset: 0x0000DED4
		public override void Initialize()
		{
			ModelMesh modelMesh = ContentManager.Get<Model>("Models/Leds", null).FindMesh("FourLed", true);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			for (int i = 0; i < 6; i++)
			{
				Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
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

		// Token: 0x06000265 RID: 613 RVA: 0x0000FE5C File Offset: 0x0000E05C
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			int num;
			for (int color = 0; color < 8; color = num)
			{
				CraftingRecipe craftingRecipe = new CraftingRecipe
				{
					ResultCount = 4,
					ResultValue = Terrain.MakeBlockValue(182, 0, FourLedBlock.SetColor(0, color)),
					RemainsCount = 1,
					RemainsValue = Terrain.MakeBlockValue(90),
					RequiredHeatLevel = 0f,
					Description = LanguageControl.Get(base.GetType().Name, 1)
				};
				craftingRecipe.Ingredients[0] = "glass";
				craftingRecipe.Ingredients[1] = "glass";
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

		// Token: 0x06000266 RID: 614 RVA: 0x0000FE6C File Offset: 0x0000E06C
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int mountingFace = FourLedBlock.GetMountingFace(Terrain.ExtractData(value));
			return face != CellFace.OppositeFace(mountingFace);
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000FE91 File Offset: 0x0000E091
		public override int GetFace(int value)
		{
			return FourLedBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000FEA0 File Offset: 0x0000E0A0
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int color = FourLedBlock.GetColor(data);
			return LanguageControl.Get("LedBlock", color) + LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, data.ToString()), "DisplayName");
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000FEF1 File Offset: 0x0000E0F1
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < 8; i = num)
			{
				yield return Terrain.MakeBlockValue(182, 0, FourLedBlock.SetColor(0, i));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000FEFC File Offset: 0x0000E0FC
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = FourLedBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000FF48 File Offset: 0x0000E148
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int color = FourLedBlock.GetColor(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(182, 0, FourLedBlock.SetColor(0, color)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000FF98 File Offset: 0x0000E198
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int mountingFace = FourLedBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace >= this.m_collisionBoxesByFace.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByFace[mountingFace];
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000FFC8 File Offset: 0x0000E1C8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int mountingFace = FourLedBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace < this.m_blockMeshesByFace.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByFace[mountingFace], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, mountingFace, 1f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00010033 File Offset: 0x0000E233
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0001004E File Offset: 0x0000E24E
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new FourLedElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00010068 File Offset: 0x0000E268
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x000100A4 File Offset: 0x0000E2A4
		public static int GetColor(int data)
		{
			return data >> 3 & 7;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x000100AB File Offset: 0x0000E2AB
		public static int SetColor(int data, int color)
		{
			return (data & -57) | (color & 7) << 3;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x000100B7 File Offset: 0x0000E2B7
		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		// Token: 0x06000274 RID: 628 RVA: 0x000100BC File Offset: 0x0000E2BC
		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}

		// Token: 0x0400011C RID: 284
		public const int Index = 182;

		// Token: 0x0400011D RID: 285
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x0400011E RID: 286
		public BlockMesh[] m_blockMeshesByFace = new BlockMesh[6];

		// Token: 0x0400011F RID: 287
		public BoundingBox[][] m_collisionBoxesByFace = new BoundingBox[6][];
	}
}
