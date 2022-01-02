using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000BE RID: 190
	public class OneLedBlock : MountedElectricElementBlock
	{
		// Token: 0x060003AB RID: 939 RVA: 0x00015E48 File Offset: 0x00014048
		public override void Initialize()
		{
			ModelMesh modelMesh = ContentManager.Get<Model>("Models/Leds", null).FindMesh("OneLed", true);
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

		// Token: 0x060003AC RID: 940 RVA: 0x00015FD0 File Offset: 0x000141D0
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			CraftingRecipe craftingRecipe = new CraftingRecipe
			{
				ResultCount = 4,
				ResultValue = Terrain.MakeBlockValue(253, 0, 0),
				RequiredHeatLevel = 0f,
				Description = LanguageControl.Get(base.GetType().Name, 1)
			};
			craftingRecipe.Ingredients[0] = "glass";
			craftingRecipe.Ingredients[1] = "glass";
			craftingRecipe.Ingredients[2] = "glass";
			craftingRecipe.Ingredients[4] = "wire";
			craftingRecipe.Ingredients[6] = "copperingot";
			craftingRecipe.Ingredients[7] = "copperingot";
			craftingRecipe.Ingredients[8] = "copperingot";
			yield return craftingRecipe;
			yield break;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00015FE0 File Offset: 0x000141E0
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int mountingFace = OneLedBlock.GetMountingFace(Terrain.ExtractData(value));
			return face != CellFace.OppositeFace(mountingFace);
		}

		// Token: 0x060003AE RID: 942 RVA: 0x00016005 File Offset: 0x00014205
		public override int GetFace(int value)
		{
			return OneLedBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00016014 File Offset: 0x00014214
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = OneLedBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00016060 File Offset: 0x00014260
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(253, 0, 0),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0001609C File Offset: 0x0001429C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int mountingFace = OneLedBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace >= this.m_collisionBoxesByFace.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByFace[mountingFace];
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x000160CC File Offset: 0x000142CC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int mountingFace = OneLedBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace < this.m_blockMeshesByFace.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByFace[mountingFace], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, mountingFace, 1f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00016137 File Offset: 0x00014337
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x00016152 File Offset: 0x00014352
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new OneLedElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0001616C File Offset: 0x0001436C
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x000161A8 File Offset: 0x000143A8
		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x000161AD File Offset: 0x000143AD
		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}

		// Token: 0x040001B0 RID: 432
		public const int Index = 253;

		// Token: 0x040001B1 RID: 433
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x040001B2 RID: 434
		public BlockMesh[] m_blockMeshesByFace = new BlockMesh[6];

		// Token: 0x040001B3 RID: 435
		public BoundingBox[][] m_collisionBoxesByFace = new BoundingBox[6][];
	}
}
