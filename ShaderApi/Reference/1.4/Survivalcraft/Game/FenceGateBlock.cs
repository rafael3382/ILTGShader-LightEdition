using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000064 RID: 100
	public abstract class FenceGateBlock : Block, IElectricElementBlock, IPaintableBlock
	{
		// Token: 0x06000212 RID: 530 RVA: 0x0000DD44 File Offset: 0x0000BF44
		public FenceGateBlock(string modelName, float pivotDistance, bool doubleSided, bool useAlphaTest, int coloredTextureSlot, Color postColor, Color unpaintedColor)
		{
			this.m_modelName = modelName;
			this.m_pivotDistance = pivotDistance;
			this.m_doubleSided = doubleSided;
			this.m_useAlphaTest = useAlphaTest;
			this.m_coloredTextureSlot = coloredTextureSlot;
			this.m_postColor = postColor;
			this.m_unpaintedColor = unpaintedColor;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000DDCC File Offset: 0x0000BFCC
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName, null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Post", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Planks", true).ParentBone);
			for (int i = 0; i < 16; i++)
			{
				int rotation = FenceGateBlock.GetRotation(i);
				bool open = FenceGateBlock.GetOpen(i);
				bool rightHanded = FenceGateBlock.GetRightHanded(i);
				float num = (float)((!rightHanded) ? 1 : -1);
				Matrix matrix = Matrix.Identity;
				matrix *= Matrix.CreateScale(0f - num, 1f, 1f);
				matrix *= Matrix.CreateTranslation((0.5f - this.m_pivotDistance) * num, 0f, 0f) * Matrix.CreateRotationY(open ? (num * 3.14159274f / 2f) : 0f) * Matrix.CreateTranslation((0f - (0.5f - this.m_pivotDistance)) * num, 0f, 0f);
				matrix *= Matrix.CreateTranslation(0f, 0f, 0f) * Matrix.CreateRotationY((float)rotation * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_blockMeshes[i] = new BlockMesh();
				this.m_blockMeshes[i].AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform * matrix, false, !rightHanded, false, false, this.m_postColor);
				this.m_blockMeshes[i].AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * matrix, false, !rightHanded, false, false, Color.White);
				if (this.m_doubleSided)
				{
					this.m_blockMeshes[i].AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * matrix, false, rightHanded, false, true, Color.White);
				}
				this.m_coloredBlockMeshes[i] = new BlockMesh();
				this.m_coloredBlockMeshes[i].AppendBlockMesh(this.m_blockMeshes[i]);
				this.m_blockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
				this.m_coloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
				BoundingBox boundingBox = this.m_blockMeshes[i].CalculateBoundingBox();
				boundingBox.Min.X = MathUtils.Saturate(boundingBox.Min.X);
				boundingBox.Min.Y = MathUtils.Saturate(boundingBox.Min.Y);
				boundingBox.Min.Z = MathUtils.Saturate(boundingBox.Min.Z);
				boundingBox.Max.X = MathUtils.Saturate(boundingBox.Max.X);
				boundingBox.Max.Y = MathUtils.Saturate(boundingBox.Max.Y);
				boundingBox.Max.Z = MathUtils.Saturate(boundingBox.Max.Z);
				this.m_collisionBoxes[i] = new BoundingBox[]
				{
					boundingBox
				};
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, this.m_postColor);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			if (this.m_doubleSided)
			{
				this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, true, false, true, Color.White);
			}
			this.m_standaloneColoredBlockMesh.AppendBlockMesh(this.m_standaloneBlockMesh);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneColoredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
			base.Initialize();
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000E2CC File Offset: 0x0000C4CC
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = FenceGateBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, base.GetDisplayName(subsystemTerrain, value));
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000E2F4 File Offset: 0x0000C4F4
		public override string GetCategory(int value)
		{
			if (FenceGateBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return "Painted";
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000E323 File Offset: 0x0000C523
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, FenceGateBlock.SetColor(0, null));
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(this.BlockIndex, 0, FenceGateBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000E334 File Offset: 0x0000C534
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int data = FenceGateBlock.SetVariant(Terrain.ExtractData(oldValue), 0);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, data),
				Count = 1
			});
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000E380 File Offset: 0x0000C580
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int num5 = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				num5 = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				num5 = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				num5 = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				num5 = 1;
			}
			Point3 point = CellFace.FaceToPoint3(raycastResult.CellFace.Face);
			int num6 = raycastResult.CellFace.X + point.X;
			int y = raycastResult.CellFace.Y + point.Y;
			int num7 = raycastResult.CellFace.Z + point.Z;
			int num8 = 0;
			int num9 = 0;
			switch (num5)
			{
			case 0:
				num8 = -1;
				break;
			case 1:
				num9 = 1;
				break;
			case 2:
				num8 = 1;
				break;
			default:
				num9 = -1;
				break;
			}
			int cellValue = subsystemTerrain.Terrain.GetCellValue(num6 + num8, y, num7 + num9);
			int cellValue2 = subsystemTerrain.Terrain.GetCellValue(num6 - num8, y, num7 - num9);
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
			Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)];
			int data = Terrain.ExtractData(cellValue);
			int data2 = Terrain.ExtractData(cellValue2);
			bool rightHanded = (block is FenceGateBlock && FenceGateBlock.GetRotation(data) == num5) || ((!(block2 is FenceGateBlock) || FenceGateBlock.GetRotation(data2) != num5) && !block.IsCollidable_(cellValue));
			int data3 = FenceGateBlock.SetRightHanded(FenceGateBlock.SetOpen(FenceGateBlock.SetRotation(Terrain.ExtractData(value), num5), false), rightHanded);
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, this.BlockIndex), data3),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000E598 File Offset: 0x0000C798
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int? color = FenceGateBlock.GetColor(Terrain.ExtractData(value));
			if (color != null)
			{
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, color), this.m_coloredTextureSlot);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.DefaultTextureSlot);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000E5F4 File Offset: 0x0000C7F4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int variant = FenceGateBlock.GetVariant(data);
			int? color = FenceGateBlock.GetColor(data);
			if (color != null)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_coloredBlockMeshes[variant], SubsystemPalette.GetColor(generator, color), null, this.m_useAlphaTest ? geometry.SubsetAlphaTest : geometry.SubsetOpaque);
			}
			else
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[variant], this.m_unpaintedColor, null, this.m_useAlphaTest ? geometry.SubsetAlphaTest : geometry.SubsetOpaque);
			}
			generator.GenerateWireVertices(value, x, y, z, FenceGateBlock.GetHingeFace(data), this.m_pivotDistance * 2f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000E6C0 File Offset: 0x0000C8C0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = FenceGateBlock.GetColor(Terrain.ExtractData(value));
			if (color2 != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneColoredBlockMesh, color * SubsystemPalette.GetColor(environmentData, color2), size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color * this.m_unpaintedColor, size, ref matrix, environmentData);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000E721 File Offset: 0x0000C921
		public int? GetPaintColor(int value)
		{
			return FenceGateBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000E730 File Offset: 0x0000C930
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, FenceGateBlock.SetColor(data, color));
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000E754 File Offset: 0x0000C954
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int variant = FenceGateBlock.GetVariant(Terrain.ExtractData(value));
			return this.m_collisionBoxes[variant];
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000E778 File Offset: 0x0000C978
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			return new FenceGateElectricElement(subsystemElectricity, new CellFace(x, y, z, FenceGateBlock.GetHingeFace(data)));
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000E7A4 File Offset: 0x0000C9A4
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int hingeFace = FenceGateBlock.GetHingeFace(Terrain.ExtractData(value));
			if (face == hingeFace)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000E7D1 File Offset: 0x0000C9D1
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000E7D8 File Offset: 0x0000C9D8
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000E7DD File Offset: 0x0000C9DD
		public static bool GetOpen(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000E7E5 File Offset: 0x0000C9E5
		public static bool GetRightHanded(int data)
		{
			return (data & 8) == 0;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000E7ED File Offset: 0x0000C9ED
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000E7F7 File Offset: 0x0000C9F7
		public static int SetOpen(int data, bool open)
		{
			if (!open)
			{
				return data & -5;
			}
			return data | 4;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000E804 File Offset: 0x0000CA04
		public static int SetRightHanded(int data, bool rightHanded)
		{
			if (rightHanded)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000E814 File Offset: 0x0000CA14
		public static int GetHingeFace(int data)
		{
			int rotation = FenceGateBlock.GetRotation(data);
			int num = (rotation - 1 < 0) ? 3 : (rotation - 1);
			if (!FenceGateBlock.GetRightHanded(data))
			{
				num = CellFace.OppositeFace(num);
			}
			return num;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000E845 File Offset: 0x0000CA45
		public static int GetVariant(int data)
		{
			return data & 15;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000E84B File Offset: 0x0000CA4B
		public static int SetVariant(int data, int variant)
		{
			return (data & -16) | (variant & 15);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000E858 File Offset: 0x0000CA58
		public static int? GetColor(int data)
		{
			if ((data & 16) != 0)
			{
				return new int?(data >> 5 & 15);
			}
			return null;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000E880 File Offset: 0x0000CA80
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -497) | 16 | (color.Value & 15) << 5;
			}
			return data & -497;
		}

		// Token: 0x040000FE RID: 254
		public float m_pivotDistance;

		// Token: 0x040000FF RID: 255
		public string m_modelName;

		// Token: 0x04000100 RID: 256
		public bool m_doubleSided;

		// Token: 0x04000101 RID: 257
		public bool m_useAlphaTest;

		// Token: 0x04000102 RID: 258
		public int m_coloredTextureSlot;

		// Token: 0x04000103 RID: 259
		public Color m_postColor;

		// Token: 0x04000104 RID: 260
		public Color m_unpaintedColor;

		// Token: 0x04000105 RID: 261
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000106 RID: 262
		public BlockMesh m_standaloneColoredBlockMesh = new BlockMesh();

		// Token: 0x04000107 RID: 263
		public BlockMesh[] m_blockMeshes = new BlockMesh[16];

		// Token: 0x04000108 RID: 264
		public BlockMesh[] m_coloredBlockMeshes = new BlockMesh[16];

		// Token: 0x04000109 RID: 265
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[16][];
	}
}
