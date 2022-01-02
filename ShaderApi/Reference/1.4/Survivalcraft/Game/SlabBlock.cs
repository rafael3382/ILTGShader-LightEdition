using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000F0 RID: 240
	public abstract class SlabBlock : Block, IPaintableBlock
	{
		// Token: 0x060004B0 RID: 1200 RVA: 0x0001A894 File Offset: 0x00018A94
		public SlabBlock(int coloredTextureSlot, int fullBlockIndex)
		{
			this.m_coloredTextureSlot = coloredTextureSlot;
			this.m_fullBlockIndex = fullBlockIndex;
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x0001A8F0 File Offset: 0x00018AF0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Slab", null);
			ModelMeshPart meshPart = model.FindMesh("Slab", true).MeshParts[0];
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Slab", true).ParentBone);
			for (int i = 0; i < 2; i++)
			{
				Matrix matrix = boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, (i == 0) ? 0f : 0.5f, 0.5f);
				this.m_uncoloredBlockMeshes[i] = new BlockMesh();
				this.m_uncoloredBlockMeshes[i].AppendModelMeshPart(meshPart, matrix, false, false, false, false, Color.White);
				this.m_uncoloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
				this.m_uncoloredBlockMeshes[i].GenerateSidesData();
				this.m_coloredBlockMeshes[i] = new BlockMesh();
				this.m_coloredBlockMeshes[i].AppendModelMeshPart(meshPart, matrix, false, false, false, false, Color.White);
				this.m_coloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
				this.m_coloredBlockMeshes[i].GenerateSidesData();
			}
			this.m_standaloneUncoloredBlockMesh.AppendModelMeshPart(meshPart, boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_standaloneUncoloredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneColoredBlockMesh.AppendModelMeshPart(meshPart, boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_standaloneColoredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
			this.m_collisionBoxes[0] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f))
			};
			this.m_collisionBoxes[1] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f))
			};
			base.Initialize();
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x0001AB9B File Offset: 0x00018D9B
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			if (SlabBlock.GetIsTop(Terrain.ExtractData(value)))
			{
				return face != 4;
			}
			return face != 5;
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x0001ABBC File Offset: 0x00018DBC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int num = SlabBlock.GetIsTop(data) ? 1 : 0;
			int? color = SlabBlock.GetColor(data);
			if (color != null)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_coloredBlockMeshes[num], SubsystemPalette.GetColor(generator, color), null, null, geometry.SubsetOpaque);
				return;
			}
			generator.GenerateShadedMeshVertices(this, x, y, z, this.m_uncoloredBlockMeshes[num], Color.White, null, null, geometry.SubsetOpaque);
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x0001AC44 File Offset: 0x00018E44
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int num = Terrain.ExtractContents(value);
			int data = Terrain.ExtractData(value);
			int num2 = Terrain.ExtractContents(raycastResult.Value);
			int data2 = Terrain.ExtractData(raycastResult.Value);
			if (num2 == num && ((SlabBlock.GetIsTop(data2) && raycastResult.CellFace.Face == 5) || (!SlabBlock.GetIsTop(data2) && raycastResult.CellFace.Face == 4)))
			{
				int value2 = Terrain.MakeBlockValue(this.m_fullBlockIndex, 0, 0);
				IPaintableBlock paintableBlock = BlocksManager.Blocks[this.m_fullBlockIndex] as IPaintableBlock;
				if (paintableBlock != null)
				{
					int? color = SlabBlock.GetColor(data);
					value2 = paintableBlock.Paint(subsystemTerrain, value2, color);
				}
				CellFace cellFace = raycastResult.CellFace;
				cellFace.Point -= CellFace.FaceToPoint3(cellFace.Face);
				return new BlockPlacementData
				{
					Value = value2,
					CellFace = cellFace
				};
			}
			bool isTop = (raycastResult.CellFace.Face >= 4) ? (raycastResult.CellFace.Face == 5) : (raycastResult.HitPoint(0f).Y - (float)raycastResult.CellFace.Y > 0.5f);
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, SlabBlock.SetIsTop(data, isTop)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x0001ADA8 File Offset: 0x00018FA8
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = SlabBlock.GetIsTop(Terrain.ExtractData(value)) ? 1 : 0;
			return this.m_collisionBoxes[num];
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x0001ADD0 File Offset: 0x00018FD0
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			if (Terrain.ExtractContents(newValue) != this.m_fullBlockIndex)
			{
				int data = Terrain.ExtractData(oldValue);
				int data2 = SlabBlock.SetColor(0, SlabBlock.GetColor(data));
				int value = Terrain.MakeBlockValue(this.BlockIndex, 0, data2);
				dropValues.Add(new BlockDropValue
				{
					Value = value,
					Count = 1
				});
				showDebris = true;
				return;
			}
			showDebris = false;
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x0001AE38 File Offset: 0x00019038
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int? color = SlabBlock.GetColor(Terrain.ExtractData(value));
			if (color != null)
			{
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, color), this.m_coloredTextureSlot);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.GetFaceTextureSlot(0, value));
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x0001AE94 File Offset: 0x00019094
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = SlabBlock.GetColor(Terrain.ExtractData(value));
			if (color2 != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneColoredBlockMesh, color * SubsystemPalette.GetColor(environmentData, color2), size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneUncoloredBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0001AEEC File Offset: 0x000190EC
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = SlabBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, base.GetDisplayName(subsystemTerrain, value));
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x0001AF14 File Offset: 0x00019114
		public override string GetCategory(int value)
		{
			if (SlabBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return "Painted";
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x0001AF43 File Offset: 0x00019143
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, SlabBlock.SetColor(0, null));
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(this.BlockIndex, 0, SlabBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x0001AF53 File Offset: 0x00019153
		public int? GetPaintColor(int value)
		{
			return SlabBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x0001AF60 File Offset: 0x00019160
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.MakeBlockValue(this.BlockIndex, 0, SlabBlock.SetColor(data, color));
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x0001AF87 File Offset: 0x00019187
		public static bool GetIsTop(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x0001AF8F File Offset: 0x0001918F
		public static int SetIsTop(int data, bool isTop)
		{
			return (data & -2) | (isTop ? 1 : 0);
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0001AFA0 File Offset: 0x000191A0
		public static int? GetColor(int data)
		{
			if ((data & 2) != 0)
			{
				return new int?(data >> 2 & 15);
			}
			return null;
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x0001AFC7 File Offset: 0x000191C7
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -63) | 2 | (color.Value & 15) << 2;
			}
			return data & -63;
		}

		// Token: 0x04000215 RID: 533
		public int m_coloredTextureSlot;

		// Token: 0x04000216 RID: 534
		public int m_fullBlockIndex;

		// Token: 0x04000217 RID: 535
		public BlockMesh m_standaloneColoredBlockMesh = new BlockMesh();

		// Token: 0x04000218 RID: 536
		public BlockMesh m_standaloneUncoloredBlockMesh = new BlockMesh();

		// Token: 0x04000219 RID: 537
		public BlockMesh[] m_coloredBlockMeshes = new BlockMesh[2];

		// Token: 0x0400021A RID: 538
		public BlockMesh[] m_uncoloredBlockMeshes = new BlockMesh[2];

		// Token: 0x0400021B RID: 539
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[2][];
	}
}
