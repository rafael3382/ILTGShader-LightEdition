using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000A0 RID: 160
	public class LightbulbBlock : MountedElectricElementBlock, IPaintableBlock
	{
		// Token: 0x0600033A RID: 826 RVA: 0x0001415C File Offset: 0x0001235C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Lightbulbs", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Top", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Sides", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_bulbBlockMeshes[i] = new BlockMesh();
				this.m_bulbBlockMeshes[i].AppendModelMeshPart(model.FindMesh("Top", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_bulbBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation(0.1875f, 0.25f, 0f), -1);
				this.m_bulbBlockMeshesLit[i] = new BlockMesh();
				this.m_bulbBlockMeshesLit[i].AppendModelMeshPart(model.FindMesh("Top", true).MeshParts[0], boneAbsoluteTransform * m, true, false, false, false, new Color(255, 255, 230));
				this.m_bulbBlockMeshesLit[i].TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0f, 0f), -1);
				this.m_sidesBlockMeshes[i] = new BlockMesh();
				this.m_sidesBlockMeshes[i].AppendModelMeshPart(model.FindMesh("Sides", true).MeshParts[0], boneAbsoluteTransform2 * m, false, false, true, false, Color.White);
				this.m_sidesBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), -1);
				this.m_collisionBoxes[i] = new BoundingBox[]
				{
					this.m_sidesBlockMeshes[i].CalculateBoundingBox()
				};
			}
			Matrix m2 = Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateRotationZ(1.57079637f);
			this.m_standaloneBulbBlockMesh.AppendModelMeshPart(model.FindMesh("Top", true).MeshParts[0], boneAbsoluteTransform * m2, false, false, true, false, Color.White);
			this.m_standaloneBulbBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.1875f, 0.25f, 0f), -1);
			this.m_standaloneSidesBlockMesh.AppendModelMeshPart(model.FindMesh("Sides", true).MeshParts[0], boneAbsoluteTransform2 * m2, false, false, true, false, Color.White);
			this.m_standaloneSidesBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), -1);
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0001448F File Offset: 0x0001268F
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(139);
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(139, 0, LightbulbBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x0600033C RID: 828 RVA: 0x00014498 File Offset: 0x00012698
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = LightbulbBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, LanguageControl.Get(LightbulbBlock.fName, 1));
		}

		// Token: 0x0600033D RID: 829 RVA: 0x000144C4 File Offset: 0x000126C4
		public override string GetCategory(int value)
		{
			if (LightbulbBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return "Painted";
		}

		// Token: 0x0600033E RID: 830 RVA: 0x000144F3 File Offset: 0x000126F3
		public override int GetFace(int value)
		{
			return LightbulbBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00014500 File Offset: 0x00012700
		public override int GetEmittedLightAmount(int value)
		{
			return LightbulbBlock.GetLightIntensity(Terrain.ExtractData(value));
		}

		// Token: 0x06000340 RID: 832 RVA: 0x00014510 File Offset: 0x00012710
		public override int GetShadowStrength(int value)
		{
			int lightIntensity = LightbulbBlock.GetLightIntensity(Terrain.ExtractData(value));
			return this.DefaultShadowStrength - 10 * lightIntensity;
		}

		// Token: 0x06000341 RID: 833 RVA: 0x00014534 File Offset: 0x00012734
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(139, 0, LightbulbBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000342 RID: 834 RVA: 0x00014584 File Offset: 0x00012784
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int? color = LightbulbBlock.GetColor(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(139, 0, LightbulbBlock.SetColor(0, color)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x06000343 RID: 835 RVA: 0x000145D4 File Offset: 0x000127D4
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int mountingFace = LightbulbBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace >= this.m_collisionBoxes.Length)
			{
				return null;
			}
			return this.m_collisionBoxes[mountingFace];
		}

		// Token: 0x06000344 RID: 836 RVA: 0x00014604 File Offset: 0x00012804
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int mountingFace = LightbulbBlock.GetMountingFace(data);
			int lightIntensity = LightbulbBlock.GetLightIntensity(data);
			int? color = LightbulbBlock.GetColor(data);
			Color color2 = (color != null) ? SubsystemPalette.GetColor(generator, color) : this.m_copperColor;
			if (mountingFace < this.m_bulbBlockMeshes.Length)
			{
				if (lightIntensity <= 0)
				{
					generator.GenerateMeshVertices(this, x, y, z, this.m_bulbBlockMeshes[mountingFace], Color.White, null, geometry.SubsetAlphaTest);
				}
				else
				{
					byte r = (byte)(195 + lightIntensity * 4);
					byte g = (byte)(180 + lightIntensity * 5);
					byte b = (byte)(165 + lightIntensity * 6);
					generator.GenerateMeshVertices(this, x, y, z, this.m_bulbBlockMeshesLit[mountingFace], new Color(r, g, b), null, geometry.SubsetOpaque);
				}
				generator.GenerateMeshVertices(this, x, y, z, this.m_sidesBlockMeshes[mountingFace], color2, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, mountingFace, 0.875f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0001471C File Offset: 0x0001291C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = LightbulbBlock.GetColor(Terrain.ExtractData(value));
			Color c = (color2 != null) ? SubsystemPalette.GetColor(environmentData, color2) : this.m_copperColor;
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneSidesBlockMesh, color * c, 2f * size, ref matrix, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBulbBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000346 RID: 838 RVA: 0x00014787 File Offset: 0x00012987
		public int? GetPaintColor(int value)
		{
			return LightbulbBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x06000347 RID: 839 RVA: 0x00014794 File Offset: 0x00012994
		public int Paint(SubsystemTerrain subsystemTerrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, LightbulbBlock.SetColor(data, color));
		}

		// Token: 0x06000348 RID: 840 RVA: 0x000147B5 File Offset: 0x000129B5
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new LightBulbElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)), value);
		}

		// Token: 0x06000349 RID: 841 RVA: 0x000147D0 File Offset: 0x000129D0
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0001480C File Offset: 0x00012A0C
		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00014811 File Offset: 0x00012A11
		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0001481B File Offset: 0x00012A1B
		public static int GetLightIntensity(int data)
		{
			return data >> 3 & 15;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00014823 File Offset: 0x00012A23
		public static int SetLightIntensity(int data, int intensity)
		{
			return (data & -121) | (intensity & 15) << 3;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x00014830 File Offset: 0x00012A30
		public static int? GetColor(int data)
		{
			if ((data & 128) != 0)
			{
				return new int?(data >> 8 & 15);
			}
			return null;
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0001485B File Offset: 0x00012A5B
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -3969) | 128 | (color.Value & 15) << 8;
			}
			return data & -3969;
		}

		// Token: 0x04000178 RID: 376
		public const int Index = 139;

		// Token: 0x04000179 RID: 377
		public BlockMesh m_standaloneBulbBlockMesh = new BlockMesh();

		// Token: 0x0400017A RID: 378
		public BlockMesh m_standaloneSidesBlockMesh = new BlockMesh();

		// Token: 0x0400017B RID: 379
		public BlockMesh[] m_bulbBlockMeshes = new BlockMesh[6];

		// Token: 0x0400017C RID: 380
		public BlockMesh[] m_bulbBlockMeshesLit = new BlockMesh[6];

		// Token: 0x0400017D RID: 381
		public BlockMesh[] m_sidesBlockMeshes = new BlockMesh[6];

		// Token: 0x0400017E RID: 382
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[6][];

		// Token: 0x0400017F RID: 383
		public new static string fName = "LightbulbBlock";

		// Token: 0x04000180 RID: 384
		public Color m_copperColor = new Color(118, 56, 32);
	}
}
