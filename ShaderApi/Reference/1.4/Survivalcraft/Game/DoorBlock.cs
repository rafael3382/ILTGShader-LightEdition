using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200005C RID: 92
	public abstract class DoorBlock : Block, IElectricElementBlock
	{
		// Token: 0x060001CA RID: 458 RVA: 0x0000C3C5 File Offset: 0x0000A5C5
		public DoorBlock(string modelName, float pivotDistance)
		{
			this.m_modelName = modelName;
			this.m_pivotDistance = pivotDistance;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000C400 File Offset: 0x0000A600
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName, null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Door", true).ParentBone);
			for (int i = 0; i < 16; i++)
			{
				int rotation = DoorBlock.GetRotation(i);
				bool open = DoorBlock.GetOpen(i);
				bool rightHanded = DoorBlock.GetRightHanded(i);
				float num = (float)((!rightHanded) ? 1 : -1);
				this.m_blockMeshesByData[i] = new BlockMesh();
				Matrix matrix = Matrix.Identity;
				matrix *= Matrix.CreateScale(0f - num, 1f, 1f);
				matrix *= Matrix.CreateTranslation((0.5f - this.m_pivotDistance) * num, 0f, 0f) * Matrix.CreateRotationY(open ? (num * 3.14159274f / 2f) : 0f) * Matrix.CreateTranslation((0f - (0.5f - this.m_pivotDistance)) * num, 0f, 0f);
				matrix *= Matrix.CreateTranslation(0f, 0f, 0.5f - this.m_pivotDistance) * Matrix.CreateRotationY((float)rotation * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_blockMeshesByData[i].AppendModelMeshPart(model.FindMesh("Door", true).MeshParts[0], boneAbsoluteTransform * matrix, false, !rightHanded, false, false, Color.White);
				BoundingBox boundingBox = this.m_blockMeshesByData[i].CalculateBoundingBox();
				boundingBox.Max.Y = 1f;
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					boundingBox
				};
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Door", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -1f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000C628 File Offset: 0x0000A828
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (DoorBlock.IsBottomPart(generator.Terrain, x, y, z) && num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetAlphaTest);
			}
			Vector2 centerOffset = DoorBlock.GetRightHanded(num) ? new Vector2(-0.45f, 0f) : new Vector2(0.45f, 0f);
			generator.GenerateWireVertices(value, x, y, z, DoorBlock.GetHingeFace(num), 0.01f, centerOffset, geometry.SubsetOpaque);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000C6CB File Offset: 0x0000A8CB
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 0.75f * size, ref matrix, environmentData);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000C6E6 File Offset: 0x0000A8E6
		public override int GetShadowStrength(int value)
		{
			if (!DoorBlock.GetOpen(Terrain.ExtractData(value)))
			{
				return this.DefaultShadowStrength;
			}
			return 4;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000C700 File Offset: 0x0000A900
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int rotation = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 1;
			}
			Point3 point = CellFace.FaceToPoint3(raycastResult.CellFace.Face);
			int num5 = raycastResult.CellFace.X + point.X;
			int y = raycastResult.CellFace.Y + point.Y;
			int num6 = raycastResult.CellFace.Z + point.Z;
			bool rightHanded = true;
			switch (rotation)
			{
			case 0:
			{
				int cellValue = subsystemTerrain.Terrain.GetCellValue(num5 - 1, y, num6);
				rightHanded = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue);
				break;
			}
			case 1:
			{
				int cellValue = subsystemTerrain.Terrain.GetCellValue(num5, y, num6 + 1);
				rightHanded = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue);
				break;
			}
			case 2:
			{
				int cellValue = subsystemTerrain.Terrain.GetCellValue(num5 + 1, y, num6);
				rightHanded = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue);
				break;
			}
			case 3:
			{
				int cellValue = subsystemTerrain.Terrain.GetCellValue(num5, y, num6 - 1);
				rightHanded = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue);
				break;
			}
			}
			int data = DoorBlock.SetRightHanded(DoorBlock.SetOpen(DoorBlock.SetRotation(0, rotation), false), rightHanded);
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, this.BlockIndex), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000C91C File Offset: 0x0000AB1C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return null;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000C945 File Offset: 0x0000AB45
		public override bool ShouldAvoid(int value)
		{
			return !DoorBlock.GetOpen(Terrain.ExtractData(value));
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000C955 File Offset: 0x0000AB55
		public override bool IsHeatBlocker(int value)
		{
			return !DoorBlock.GetOpen(Terrain.ExtractData(value));
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000C968 File Offset: 0x0000AB68
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			return new DoorElectricElement(subsystemElectricity, new CellFace(x, y, z, DoorBlock.GetHingeFace(data)));
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000C994 File Offset: 0x0000AB94
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int hingeFace = DoorBlock.GetHingeFace(Terrain.ExtractData(value));
			if (face == hingeFace)
			{
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(hingeFace, 0, connectorFace);
				ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
				ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.Right;
				if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
				{
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.Left;
					if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
					{
						electricConnectorDirection = connectorDirection;
						electricConnectorDirection2 = ElectricConnectorDirection.In;
						if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
						{
							goto IL_69;
						}
					}
				}
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			IL_69:
			return null;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000CA14 File Offset: 0x0000AC14
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000CA1B File Offset: 0x0000AC1B
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000CA20 File Offset: 0x0000AC20
		public static bool GetOpen(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000CA28 File Offset: 0x0000AC28
		public static bool GetRightHanded(int data)
		{
			return (data & 8) == 0;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000CA30 File Offset: 0x0000AC30
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000CA3A File Offset: 0x0000AC3A
		public static int SetOpen(int data, bool open)
		{
			if (!open)
			{
				return data & -5;
			}
			return data | 4;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000CA47 File Offset: 0x0000AC47
		public static int SetRightHanded(int data, bool rightHanded)
		{
			if (rightHanded)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000CA54 File Offset: 0x0000AC54
		public static bool IsTopPart(Terrain terrain, int x, int y, int z)
		{
			return BlocksManager.Blocks[terrain.GetCellContents(x, y - 1, z)] is DoorBlock;
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000CA6F File Offset: 0x0000AC6F
		public static bool IsBottomPart(Terrain terrain, int x, int y, int z)
		{
			return BlocksManager.Blocks[terrain.GetCellContents(x, y + 1, z)] is DoorBlock;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000CA8C File Offset: 0x0000AC8C
		public static int GetHingeFace(int data)
		{
			int rotation = DoorBlock.GetRotation(data);
			int num = (rotation - 1 < 0) ? 3 : (rotation - 1);
			if (!DoorBlock.GetRightHanded(data))
			{
				num = CellFace.OppositeFace(num);
			}
			return num;
		}

		// Token: 0x040000E4 RID: 228
		public float m_pivotDistance;

		// Token: 0x040000E5 RID: 229
		public string m_modelName;

		// Token: 0x040000E6 RID: 230
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040000E7 RID: 231
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[16];

		// Token: 0x040000E8 RID: 232
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[16][];
	}
}
