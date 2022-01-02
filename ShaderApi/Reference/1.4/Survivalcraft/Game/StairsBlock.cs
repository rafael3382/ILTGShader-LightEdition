using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000FC RID: 252
	public abstract class StairsBlock : Block, IPaintableBlock
	{
		// Token: 0x060004F5 RID: 1269 RVA: 0x0001BB38 File Offset: 0x00019D38
		public StairsBlock(int coloredTextureSlot)
		{
			this.m_coloredTextureSlot = coloredTextureSlot;
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0001BB90 File Offset: 0x00019D90
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Stairs", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Stairs", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StairsOuterCorner", true).ParentBone);
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StairsInnerCorner", true).ParentBone);
			for (int i = 0; i < 24; i++)
			{
				int rotation = StairsBlock.GetRotation(i);
				bool isUpsideDown = StairsBlock.GetIsUpsideDown(i);
				StairsBlock.CornerType cornerType = StairsBlock.GetCornerType(i);
				Matrix m = (!isUpsideDown) ? (Matrix.CreateRotationY((float)rotation * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationY((float)rotation * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, -0.5f, 0.5f) * Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, 0.5f, 0f));
				BlockMesh blockMesh = new BlockMesh();
				switch (cornerType)
				{
				case StairsBlock.CornerType.None:
					blockMesh.AppendModelMeshPart(model.FindMesh("Stairs", true).MeshParts[0], boneAbsoluteTransform * m, false, isUpsideDown, false, false, Color.White);
					break;
				case StairsBlock.CornerType.OneQuarter:
					blockMesh.AppendModelMeshPart(model.FindMesh("StairsOuterCorner", true).MeshParts[0], boneAbsoluteTransform2 * m, false, isUpsideDown, false, false, Color.White);
					break;
				case StairsBlock.CornerType.ThreeQuarters:
					blockMesh.AppendModelMeshPart(model.FindMesh("StairsInnerCorner", true).MeshParts[0], boneAbsoluteTransform3 * m, false, isUpsideDown, false, false, Color.White);
					break;
				}
				float num = (float)(isUpsideDown ? rotation : (-(float)rotation));
				blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-0.03125f, -0.03125f, 0f) * Matrix.CreateRotationZ(num * 3.14159274f / 2f) * Matrix.CreateTranslation(0.03125f, 0.03125f, 0f), 16);
				blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-0.03125f, -0.03125f, 0f) * Matrix.CreateRotationZ((0f - num) * 3.14159274f / 2f) * Matrix.CreateTranslation(0.03125f, 0.03125f, 0f), 32);
				if (isUpsideDown)
				{
					blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-0.03125f, -0.03125f, 0f) * Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0.03125f, 0.03125f, 0f), -1);
				}
				this.m_coloredBlockMeshes[i] = new BlockMesh();
				this.m_coloredBlockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_coloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
				this.m_coloredBlockMeshes[i].GenerateSidesData();
				this.m_uncoloredBlockMeshes[i] = new BlockMesh();
				this.m_uncoloredBlockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_uncoloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
				this.m_uncoloredBlockMeshes[i].GenerateSidesData();
			}
			this.m_standaloneUncoloredBlockMesh.AppendModelMeshPart(model.FindMesh("Stairs", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_standaloneUncoloredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneColoredBlockMesh.AppendModelMeshPart(model.FindMesh("Stairs", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_standaloneColoredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
			this.m_collisionBoxes[0] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(1f, 0.5f, 1f))
			};
			this.m_collisionBoxes[1] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0f), new Vector3(1f, 0.5f, 1f))
			};
			this.m_collisionBoxes[2] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[3] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[4] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[5] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[6] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[7] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[8] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 0.5f))
			};
			this.m_collisionBoxes[9] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 1f, 0.5f))
			};
			this.m_collisionBoxes[10] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0.5f), new Vector3(0.5f, 1f, 1f))
			};
			this.m_collisionBoxes[11] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[12] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0f), new Vector3(1f, 0.5f, 0.5f))
			};
			this.m_collisionBoxes[13] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 0.5f, 0.5f))
			};
			this.m_collisionBoxes[14] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(0.5f, 0.5f, 1f))
			};
			this.m_collisionBoxes[15] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0.5f), new Vector3(1f, 0.5f, 1f))
			};
			this.m_collisionBoxes[16] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 0.5f)),
				new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[17] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0.5f), new Vector3(0.5f, 1f, 1f))
			};
			this.m_collisionBoxes[18] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 1f, 0.5f))
			};
			this.m_collisionBoxes[19] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 0.5f))
			};
			this.m_collisionBoxes[20] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 0.5f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0.5f), new Vector3(1f, 0.5f, 1f))
			};
			this.m_collisionBoxes[21] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(0.5f, 0.5f, 1f))
			};
			this.m_collisionBoxes[22] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 0.5f, 0.5f))
			};
			this.m_collisionBoxes[23] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0f), new Vector3(1f, 0.5f, 0.5f))
			};
			base.Initialize();
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x0001CD02 File Offset: 0x0001AF02
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, StairsBlock.SetColor(0, null));
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(this.BlockIndex, 0, StairsBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0001CD14 File Offset: 0x0001AF14
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int data = Terrain.ExtractData(value);
			bool isUpsideDown = StairsBlock.GetIsUpsideDown(data);
			if (face == 4)
			{
				return !isUpsideDown;
			}
			if (face == 5)
			{
				return isUpsideDown;
			}
			StairsBlock.CornerType cornerType = StairsBlock.GetCornerType(data);
			if (cornerType == StairsBlock.CornerType.None)
			{
				int rotation = StairsBlock.GetRotation(data);
				return face != (rotation + 2 & 3);
			}
			if (cornerType != StairsBlock.CornerType.OneQuarter)
			{
				int rotation2 = StairsBlock.GetRotation(data);
				return face != (rotation2 + 1 & 3) && face != (rotation2 + 2 & 3);
			}
			return true;
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x0001CD84 File Offset: 0x0001AF84
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int? color = StairsBlock.GetColor(data);
			if (color != null)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_coloredBlockMeshes[StairsBlock.GetVariant(data)], SubsystemPalette.GetColor(generator, color), null, null, geometry.SubsetOpaque);
				return;
			}
			generator.GenerateShadedMeshVertices(this, x, y, z, this.m_uncoloredBlockMeshes[StairsBlock.GetVariant(data)], Color.White, null, null, geometry.SubsetOpaque);
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x0001CE0C File Offset: 0x0001B00C
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
			bool isUpsideDown = raycastResult.CellFace.Face == 5;
			int data = Terrain.ExtractData(value);
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, StairsBlock.SetIsUpsideDown(StairsBlock.SetRotation(data, rotation), isUpsideDown)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x0001CF0C File Offset: 0x0001B10C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int data = Terrain.ExtractData(value);
			return this.m_collisionBoxes[StairsBlock.GetVariant(data)];
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x0001CF30 File Offset: 0x0001B130
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = StairsBlock.GetColor(Terrain.ExtractData(value));
			if (color2 != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneColoredBlockMesh, color * SubsystemPalette.GetColor(environmentData, color2), size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneUncoloredBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x0001CF88 File Offset: 0x0001B188
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = StairsBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, base.GetDisplayName(subsystemTerrain, value));
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x0001CFB0 File Offset: 0x0001B1B0
		public override string GetCategory(int value)
		{
			if (StairsBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return "Painted";
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x0001CFE0 File Offset: 0x0001B1E0
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int data = Terrain.ExtractData(oldValue);
			int data2 = StairsBlock.SetColor(0, StairsBlock.GetColor(data));
			int value = Terrain.MakeBlockValue(this.BlockIndex, 0, data2);
			dropValues.Add(new BlockDropValue
			{
				Value = value,
				Count = 1
			});
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x0001D034 File Offset: 0x0001B234
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int? color = StairsBlock.GetColor(Terrain.ExtractData(value));
			if (color != null)
			{
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, color), this.m_coloredTextureSlot);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, base.GetFaceTextureSlot(0, value));
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0001D08F File Offset: 0x0001B28F
		public int? GetPaintColor(int value)
		{
			return StairsBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0001D09C File Offset: 0x0001B29C
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			return Terrain.MakeBlockValue(this.BlockIndex, 0, StairsBlock.SetColor(Terrain.ExtractData(value), color));
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0001D0B6 File Offset: 0x0001B2B6
		public static Point3 RotationToDirection(int rotation)
		{
			return CellFace.FaceToPoint3((rotation + 2) % 4);
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0001D0C2 File Offset: 0x0001B2C2
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x0001D0C7 File Offset: 0x0001B2C7
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0001D0D1 File Offset: 0x0001B2D1
		public static bool GetIsUpsideDown(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0001D0D9 File Offset: 0x0001B2D9
		public static int SetIsUpsideDown(int data, bool isUpsideDown)
		{
			if (isUpsideDown)
			{
				return data | 4;
			}
			return data & -5;
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x0001D0E6 File Offset: 0x0001B2E6
		public static StairsBlock.CornerType GetCornerType(int data)
		{
			return (StairsBlock.CornerType)(data >> 3 & 3);
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x0001D0ED File Offset: 0x0001B2ED
		public static int SetCornerType(int data, StairsBlock.CornerType cornerType)
		{
			return (data & -25) | (int)((int)(cornerType & (StairsBlock.CornerType)3) << 3);
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x0001D0FC File Offset: 0x0001B2FC
		public static int? GetColor(int data)
		{
			if ((data & 32) != 0)
			{
				return new int?(data >> 6 & 15);
			}
			return null;
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x0001D124 File Offset: 0x0001B324
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -993) | 32 | (color.Value & 15) << 6;
			}
			return data & -993;
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x0001D14E File Offset: 0x0001B34E
		public static int GetVariant(int data)
		{
			return data & 31;
		}

		// Token: 0x04000230 RID: 560
		public BlockMesh m_standaloneUncoloredBlockMesh = new BlockMesh();

		// Token: 0x04000231 RID: 561
		public BlockMesh m_standaloneColoredBlockMesh = new BlockMesh();

		// Token: 0x04000232 RID: 562
		public BlockMesh[] m_uncoloredBlockMeshes = new BlockMesh[24];

		// Token: 0x04000233 RID: 563
		public BlockMesh[] m_coloredBlockMeshes = new BlockMesh[24];

		// Token: 0x04000234 RID: 564
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[24][];

		// Token: 0x04000235 RID: 565
		public int m_coloredTextureSlot;

		// Token: 0x0200040D RID: 1037
		public enum CornerType
		{
			// Token: 0x0400150B RID: 5387
			None,
			// Token: 0x0400150C RID: 5388
			OneQuarter,
			// Token: 0x0400150D RID: 5389
			ThreeQuarters
		}
	}
}
