using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000B7 RID: 183
	public class MulticoloredLedBlock : MountedElectricElementBlock
	{
		// Token: 0x06000386 RID: 902 RVA: 0x00015550 File Offset: 0x00013750
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Leds", null);
			ModelMesh modelMesh = model.FindMesh("Led", true);
			ModelMesh modelMesh2 = model.FindMesh("LedBulb", true);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(modelMesh2.ParentBone);
			Matrix m = Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateRotationZ(1.57079637f);
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneBlockMesh.AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(modelMesh2.MeshParts[0], boneAbsoluteTransform2 * m, false, false, false, false, new Color(48, 48, 48));
			for (int i = 0; i < 6; i++)
			{
				int num = MulticoloredLedBlock.SetMountingFace(0, i);
				Matrix m2 = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_blockMeshesByData[num] = new BlockMesh();
				this.m_blockMeshesByData[num].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
				this.m_blockMeshesByData[num].AppendModelMeshPart(modelMesh2.MeshParts[0], boneAbsoluteTransform2 * m2, false, false, false, false, new Color(48, 48, 48));
				this.m_collisionBoxesByData[num] = new BoundingBox[]
				{
					this.m_blockMeshesByData[num].CalculateBoundingBox()
				};
			}
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0001576F File Offset: 0x0001396F
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			CraftingRecipe craftingRecipe = new CraftingRecipe
			{
				ResultCount = 4,
				ResultValue = Terrain.MakeBlockValue(254, 0, 0),
				RequiredHeatLevel = 0f,
				Description = LanguageControl.Get(base.GetType().Name, 1)
			};
			craftingRecipe.Ingredients[1] = "glass";
			craftingRecipe.Ingredients[4] = "wire";
			craftingRecipe.Ingredients[6] = "copperingot";
			craftingRecipe.Ingredients[7] = "copperingot";
			craftingRecipe.Ingredients[8] = "copperingot";
			yield return craftingRecipe;
			yield break;
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0001577F File Offset: 0x0001397F
		public override int GetFace(int value)
		{
			return MulticoloredLedBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0001578C File Offset: 0x0001398C
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(254, 0, 0);
			yield break;
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00015798 File Offset: 0x00013998
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = MulticoloredLedBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600038B RID: 907 RVA: 0x000157E4 File Offset: 0x000139E4
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00015810 File Offset: 0x00013A10
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0001587C File Offset: 0x00013A7C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0600038E RID: 910 RVA: 0x00015897 File Offset: 0x00013A97
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new MulticoloredLedElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x0600038F RID: 911 RVA: 0x000158B0 File Offset: 0x00013AB0
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x000158EC File Offset: 0x00013AEC
		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x000158F1 File Offset: 0x00013AF1
		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}

		// Token: 0x040001A4 RID: 420
		public const int Index = 254;

		// Token: 0x040001A5 RID: 421
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x040001A6 RID: 422
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];

		// Token: 0x040001A7 RID: 423
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[6][];
	}
}
