using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200009F RID: 159
	public class LedBlock : MountedElectricElementBlock
	{
		// Token: 0x06000328 RID: 808 RVA: 0x00013BB0 File Offset: 0x00011DB0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Leds", null);
			ModelMesh modelMesh = model.FindMesh("Led", true);
			ModelMesh modelMesh2 = model.FindMesh("LedBulb", true);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(modelMesh2.ParentBone);
			for (int i = 0; i < 8; i++)
			{
				Color color = LedBlock.LedColors[i];
				color *= 0.5f;
				color.A = byte.MaxValue;
				Matrix m = Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateRotationZ(1.57079637f);
				this.m_standaloneBlockMeshesByColor[i] = new BlockMesh();
				this.m_standaloneBlockMeshesByColor[i].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_standaloneBlockMeshesByColor[i].AppendModelMeshPart(modelMesh2.MeshParts[0], boneAbsoluteTransform2 * m, false, false, false, false, color);
				for (int j = 0; j < 6; j++)
				{
					int num = LedBlock.SetMountingFace(LedBlock.SetColor(0, i), j);
					Matrix m2 = (j >= 4) ? ((j != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)j * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
					this.m_blockMeshesByData[num] = new BlockMesh();
					this.m_blockMeshesByData[num].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
					this.m_blockMeshesByData[num].AppendModelMeshPart(modelMesh2.MeshParts[0], boneAbsoluteTransform2 * m2, false, false, false, false, color);
					this.m_collisionBoxesByData[num] = new BoundingBox[]
					{
						this.m_blockMeshesByData[num].CalculateBoundingBox()
					};
				}
			}
		}

		// Token: 0x06000329 RID: 809 RVA: 0x00013E0B File Offset: 0x0001200B
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			int num;
			for (int color = 0; color < 8; color = num)
			{
				CraftingRecipe craftingRecipe = new CraftingRecipe
				{
					ResultCount = 4,
					ResultValue = Terrain.MakeBlockValue(152, 0, LedBlock.SetColor(0, color)),
					RemainsCount = 1,
					RemainsValue = Terrain.MakeBlockValue(90),
					RequiredHeatLevel = 0f,
					Description = LanguageControl.Get(base.GetType().Name, 1)
				};
				craftingRecipe.Ingredients[1] = "glass";
				craftingRecipe.Ingredients[4] = "paintbucket:" + color.ToString(CultureInfo.InvariantCulture);
				craftingRecipe.Ingredients[6] = "copperingot";
				craftingRecipe.Ingredients[7] = "copperingot";
				craftingRecipe.Ingredients[8] = "copperingot";
				yield return craftingRecipe;
				num = color + 1;
			}
			yield break;
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00013E1B File Offset: 0x0001201B
		public override int GetFace(int value)
		{
			return LedBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00013E28 File Offset: 0x00012028
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int color = LedBlock.GetColor(data);
			return LanguageControl.Get("LedBlock", color) + LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, data.ToString()), "DisplayName");
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00013E79 File Offset: 0x00012079
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < 8; i = num)
			{
				yield return Terrain.MakeBlockValue(152, 0, LedBlock.SetColor(0, i));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00013E84 File Offset: 0x00012084
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = LedBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00013ED0 File Offset: 0x000120D0
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int color = LedBlock.GetColor(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(152, 0, LedBlock.SetColor(0, color)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x00013F20 File Offset: 0x00012120
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00013F4C File Offset: 0x0001214C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00013FB8 File Offset: 0x000121B8
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int color2 = LedBlock.GetColor(Terrain.ExtractData(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshesByColor[color2], color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00013FEC File Offset: 0x000121EC
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new LedElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00014008 File Offset: 0x00012208
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00014044 File Offset: 0x00012244
		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00014049 File Offset: 0x00012249
		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00014053 File Offset: 0x00012253
		public static int GetColor(int data)
		{
			return data >> 3 & 7;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0001405A File Offset: 0x0001225A
		public static int SetColor(int data, int color)
		{
			return (data & -57) | (color & 7) << 3;
		}

		// Token: 0x04000173 RID: 371
		public const int Index = 152;

		// Token: 0x04000174 RID: 372
		public static readonly Color[] LedColors = new Color[]
		{
			new Color(255, 255, 255),
			new Color(0, 255, 255),
			new Color(255, 0, 0),
			new Color(0, 0, 255),
			new Color(255, 240, 0),
			new Color(0, 255, 0),
			new Color(255, 120, 0),
			new Color(255, 0, 255)
		};

		// Token: 0x04000175 RID: 373
		public BlockMesh[] m_standaloneBlockMeshesByColor = new BlockMesh[8];

		// Token: 0x04000176 RID: 374
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[64];

		// Token: 0x04000177 RID: 375
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[64][];
	}
}
