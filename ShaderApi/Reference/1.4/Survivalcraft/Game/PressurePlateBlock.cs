using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000CA RID: 202
	public class PressurePlateBlock : MountedElectricElementBlock
	{
		// Token: 0x06000428 RID: 1064 RVA: 0x00018084 File Offset: 0x00016284
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/PressurePlate", null);
			for (int i = 0; i < 2; i++)
			{
				Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("PressurePlate", true).ParentBone);
				int num = this.m_textureSlotsByMaterial[i];
				for (int j = 0; j < 6; j++)
				{
					int num2 = PressurePlateBlock.SetMountingFace(PressurePlateBlock.SetMaterial(0, i), j);
					Matrix matrix = (j >= 4) ? ((j != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)j * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
					this.m_blockMeshesByData[num2] = new BlockMesh();
					this.m_blockMeshesByData[num2].AppendModelMeshPart(model.FindMesh("PressurePlate", true).MeshParts[0], boneAbsoluteTransform * matrix, false, false, false, false, Color.White);
					this.m_blockMeshesByData[num2].TransformTextureCoordinates(Matrix.CreateTranslation((float)(num % 16) / 16f, (float)(num / 16) / 16f, 0f), -1);
					this.m_blockMeshesByData[num2].GenerateSidesData();
					Vector3 vector = Vector3.Transform(new Vector3(-0.5f, 0f, -0.5f), matrix);
					Vector3 vector2 = Vector3.Transform(new Vector3(0.5f, 0.0625f, 0.5f), matrix);
					vector.X = MathUtils.Round(vector.X * 100f) / 100f;
					vector.Y = MathUtils.Round(vector.Y * 100f) / 100f;
					vector.Z = MathUtils.Round(vector.Z * 100f) / 100f;
					vector2.X = MathUtils.Round(vector2.X * 100f) / 100f;
					vector2.Y = MathUtils.Round(vector2.Y * 100f) / 100f;
					vector2.Z = MathUtils.Round(vector2.Z * 100f) / 100f;
					this.m_collisionBoxesByData[num2] = new BoundingBox[]
					{
						new BoundingBox(new Vector3(MathUtils.Min(vector.X, vector2.X), MathUtils.Min(vector.Y, vector2.Y), MathUtils.Min(vector.Z, vector2.Z)), new Vector3(MathUtils.Max(vector.X, vector2.X), MathUtils.Max(vector.Y, vector2.Y), MathUtils.Max(vector.Z, vector2.Z)))
					};
				}
				Matrix identity = Matrix.Identity;
				this.m_standaloneBlockMeshesByMaterial[i] = new BlockMesh();
				this.m_standaloneBlockMeshesByMaterial[i].AppendModelMeshPart(model.FindMesh("PressurePlate", true).MeshParts[0], boneAbsoluteTransform * identity, false, false, false, false, Color.White);
				this.m_standaloneBlockMeshesByMaterial[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(num % 16) / 16f, (float)(num / 16) / 16f, 0f), -1);
			}
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x0001841C File Offset: 0x0001661C
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int material = PressurePlateBlock.GetMaterial(Terrain.ExtractData(value));
			return this.m_displayNamesByMaterial[material];
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x0001843D File Offset: 0x0001663D
		public override IEnumerable<int> GetCreativeValues()
		{
			return this.m_creativeValuesByMaterial;
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00018448 File Offset: 0x00016648
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int material = PressurePlateBlock.GetMaterial(Terrain.ExtractData(value));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.m_textureSlotsByMaterial[material]);
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00018480 File Offset: 0x00016680
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = PressurePlateBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x000184CC File Offset: 0x000166CC
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int material = PressurePlateBlock.GetMaterial(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(144, 0, PressurePlateBlock.SetMaterial(0, material)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x0001851C File Offset: 0x0001671C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00018545 File Offset: 0x00016745
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return face != CellFace.OppositeFace(this.GetFace(value));
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x0001855C File Offset: 0x0001675C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length && this.m_blockMeshesByData[num] != null)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.8125f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x000185D4 File Offset: 0x000167D4
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int material = PressurePlateBlock.GetMaterial(Terrain.ExtractData(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshesByMaterial[material], color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00018608 File Offset: 0x00016808
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new PressurePlateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x00018624 File Offset: 0x00016824
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x00018660 File Offset: 0x00016860
		public static int GetMaterial(int data)
		{
			return data & 1;
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00018665 File Offset: 0x00016865
		public static int SetMaterial(int data, int material)
		{
			return (data & -2) | (material & 1);
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x0001866F File Offset: 0x0001686F
		public static int GetMountingFace(int data)
		{
			return data >> 1 & 7;
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x00018676 File Offset: 0x00016876
		public static int SetMountingFace(int data, int face)
		{
			return (data & -15) | (face & 7) << 1;
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x00018682 File Offset: 0x00016882
		public override int GetFace(int value)
		{
			return PressurePlateBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x040001D4 RID: 468
		public const int Index = 144;

		// Token: 0x040001D5 RID: 469
		public BlockMesh[] m_standaloneBlockMeshesByMaterial = new BlockMesh[2];

		// Token: 0x040001D6 RID: 470
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[16];

		// Token: 0x040001D7 RID: 471
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[16][];

		// Token: 0x040001D8 RID: 472
		public string[] m_displayNamesByMaterial = new string[]
		{
			"木质压力板",
			"石质压力板"
		};

		// Token: 0x040001D9 RID: 473
		public int[] m_creativeValuesByMaterial = new int[]
		{
			Terrain.MakeBlockValue(144, 0, 0),
			Terrain.MakeBlockValue(144, 0, 1)
		};

		// Token: 0x040001DA RID: 474
		public int[] m_textureSlotsByMaterial = new int[]
		{
			4,
			1
		};
	}
}
