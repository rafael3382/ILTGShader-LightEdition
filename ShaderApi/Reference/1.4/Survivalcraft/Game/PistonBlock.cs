using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C6 RID: 198
	public class PistonBlock : Block, IElectricElementBlock
	{
		// Token: 0x060003E9 RID: 1001 RVA: 0x00016E18 File Offset: 0x00015018
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Pistons", null);
			for (PistonMode pistonMode = PistonMode.Pushing; pistonMode <= PistonMode.StrictPulling; pistonMode++)
			{
				for (int i = 0; i < 2; i++)
				{
					string name = (i == 0) ? "PistonRetracted" : "PistonExtended";
					Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(name, true).ParentBone);
					for (int j = 0; j < 6; j++)
					{
						int num = PistonBlock.SetFace(PistonBlock.SetIsExtended(PistonBlock.SetMode(0, pistonMode), i != 0), j);
						Matrix m = (j < 4) ? (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationY((float)j * 3.14159274f / 2f + 3.14159274f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)) : ((j != 4) ? (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationX(-1.57079637f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)) : (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)));
						this.m_blockMeshesByData[num] = new BlockMesh();
						this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh(name, true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
						if (i == 0)
						{
							if (pistonMode != PistonMode.Pulling)
							{
								if (pistonMode == PistonMode.StrictPulling)
								{
									this.m_blockMeshesByData[num].TransformTextureCoordinates(Matrix.CreateTranslation(0f, 0.125f, 0f), 1 << j);
								}
							}
							else
							{
								this.m_blockMeshesByData[num].TransformTextureCoordinates(Matrix.CreateTranslation(0f, 0.0625f, 0f), 1 << j);
							}
						}
					}
				}
				Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("PistonRetracted", true).ParentBone);
				this.m_standaloneBlockMeshes[(int)pistonMode] = new BlockMesh();
				this.m_standaloneBlockMeshes[(int)pistonMode].AppendModelMeshPart(model.FindMesh("PistonRetracted", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
				if (pistonMode != PistonMode.Pulling)
				{
					if (pistonMode == PistonMode.StrictPulling)
					{
						this.m_standaloneBlockMeshes[(int)pistonMode].TransformTextureCoordinates(Matrix.CreateTranslation(0f, 0.125f, 0f), 4);
					}
				}
				else
				{
					this.m_standaloneBlockMeshes[(int)pistonMode].TransformTextureCoordinates(Matrix.CreateTranslation(0f, 0.0625f, 0f), 4);
				}
			}
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x000170EC File Offset: 0x000152EC
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int data = Terrain.ExtractData(value);
			int face2 = PistonBlock.GetFace(data);
			return PistonBlock.GetIsExtended(data) && face != face2 && face != CellFace.OppositeFace(face2);
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00017124 File Offset: 0x00015324
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value) & 63;
			if (num < this.m_blockMeshesByData.Length && this.m_blockMeshesByData[num] != null)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, null, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x0001717C File Offset: 0x0001537C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int mode = (int)PistonBlock.GetMode(Terrain.ExtractData(value));
			if (mode < this.m_standaloneBlockMeshes.Length && this.m_standaloneBlockMeshes[mode] != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshes[mode], color, 1f * size, ref matrix, environmentData);
			}
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x000171C5 File Offset: 0x000153C5
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new PistonElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x000171D7 File Offset: 0x000153D7
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return new ElectricConnectorType?(ElectricConnectorType.Input);
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x000171DF File Offset: 0x000153DF
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x000171E6 File Offset: 0x000153E6
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(237, 0, PistonBlock.SetMode(PistonBlock.SetMaxExtension(0, 7), PistonMode.Pushing));
			yield return Terrain.MakeBlockValue(237, 0, PistonBlock.SetMode(PistonBlock.SetMaxExtension(0, 7), PistonMode.Pulling));
			yield return Terrain.MakeBlockValue(237, 0, PistonBlock.SetMode(PistonBlock.SetMaxExtension(0, 7), PistonMode.StrictPulling));
			yield break;
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x000171F0 File Offset: 0x000153F0
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			PistonMode mode = PistonBlock.GetMode(Terrain.ExtractData(value));
			if (mode == PistonMode.Pulling)
			{
				return "粘性活塞";
			}
			if (mode != PistonMode.StrictPulling)
			{
				return "活塞";
			}
			return "严格粘性活塞";
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00017224 File Offset: 0x00015424
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = float.PositiveInfinity;
			int face = 0;
			for (int i = 0; i < 6; i++)
			{
				float num2 = Vector3.Dot(CellFace.FaceToVector3(i), forward);
				if (num2 < num)
				{
					num = num2;
					face = i;
				}
			}
			int data = Terrain.ExtractData(value);
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, PistonBlock.SetFace(data, face)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x000172BC File Offset: 0x000154BC
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(237, 0, PistonBlock.SetFace(PistonBlock.SetIsExtended(data, false), 0)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0001730B File Offset: 0x0001550B
		public static bool GetIsExtended(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x00017313 File Offset: 0x00015513
		public static int SetIsExtended(int data, bool isExtended)
		{
			return (data & -2) | (isExtended ? 1 : 0);
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00017321 File Offset: 0x00015521
		public static PistonMode GetMode(int data)
		{
			return (PistonMode)(data >> 1 & 3);
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00017328 File Offset: 0x00015528
		public static int SetMode(int data, PistonMode mode)
		{
			return (data & -7) | (int)((int)(mode & (PistonMode)3) << 1);
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x00017334 File Offset: 0x00015534
		public static int GetFace(int data)
		{
			return data >> 3 & 7;
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0001733B File Offset: 0x0001553B
		public static int SetFace(int data, int face)
		{
			return (data & -57) | (face & 7) << 3;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00017347 File Offset: 0x00015547
		public static int GetMaxExtension(int data)
		{
			return data >> 6 & 7;
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0001734E File Offset: 0x0001554E
		public static int SetMaxExtension(int data, int maxExtension)
		{
			return (data & -449) | (maxExtension & 7) << 6;
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x0001735D File Offset: 0x0001555D
		public static int GetPullCount(int data)
		{
			return data >> 9 & 7;
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x00017365 File Offset: 0x00015565
		public static int SetPullCount(int data, int pullCount)
		{
			return (data & -3585) | (pullCount & 7) << 9;
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x00017375 File Offset: 0x00015575
		public static int GetSpeed(int data)
		{
			return data >> 12 & 7;
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x0001737D File Offset: 0x0001557D
		public static int SetSpeed(int data, int speed)
		{
			return (data & -12289) | (speed & 3) << 12;
		}

		// Token: 0x040001C3 RID: 451
		public const int Index = 237;

		// Token: 0x040001C4 RID: 452
		public BlockMesh[] m_standaloneBlockMeshes = new BlockMesh[4];

		// Token: 0x040001C5 RID: 453
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[48];
	}
}
