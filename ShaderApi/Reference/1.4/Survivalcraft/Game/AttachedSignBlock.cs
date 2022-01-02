using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000012 RID: 18
	public abstract class AttachedSignBlock : SignBlock, IElectricElementBlock, IPaintableBlock
	{
		// Token: 0x060000BE RID: 190 RVA: 0x0000800C File Offset: 0x0000620C
		public AttachedSignBlock(string modelName, int coloredTextureSlot, int postedSignBlockIndex)
		{
			this.m_modelName = modelName;
			this.m_coloredTextureSlot = coloredTextureSlot;
			this.m_postedSignBlockIndex = postedSignBlockIndex;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00008088 File Offset: 0x00006288
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName, null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Sign", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Surface", true).ParentBone);
			for (int i = 0; i < 4; i++)
			{
				float radians = 1.57079637f * (float)i;
				Matrix m = Matrix.CreateTranslation(0f, 0f, -0.46875f) * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(0.5f, -0.3125f, 0.5f);
				BlockMesh blockMesh = new BlockMesh();
				blockMesh.AppendModelMeshPart(model.FindMesh("Sign", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_blockMeshes[i] = new BlockMesh();
				this.m_blockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_coloredBlockMeshes[i] = new BlockMesh();
				this.m_coloredBlockMeshes[i].AppendBlockMesh(this.m_blockMeshes[i]);
				this.m_blockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
				this.m_coloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
				this.m_collisionBoxes[i] = new BoundingBox[1];
				this.m_collisionBoxes[i][0] = blockMesh.CalculateBoundingBox();
				this.m_surfaceMeshes[i] = new BlockMesh();
				this.m_surfaceMeshes[i].AppendModelMeshPart(model.FindMesh("Surface", true).MeshParts[0], boneAbsoluteTransform2 * m, false, false, false, false, Color.White);
				this.m_surfaceNormals[i] = -m.Forward;
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Sign", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.6f, 0f), false, false, false, false, Color.White);
			this.m_standaloneColoredBlockMesh.AppendBlockMesh(this.m_standaloneBlockMesh);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneColoredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
			base.Initialize();
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x0000835C File Offset: 0x0000655C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int? color = AttachedSignBlock.GetColor(Terrain.ExtractData(oldValue));
			int data = PostedSignBlock.SetColor(0, color);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(this.m_postedSignBlockIndex, 0, data),
				Count = 1
			});
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000083B0 File Offset: 0x000065B0
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int? color = AttachedSignBlock.GetColor(Terrain.ExtractData(value));
			if (color != null)
			{
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, color), this.m_coloredTextureSlot);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.DefaultTextureSlot);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000840C File Offset: 0x0000660C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int face = AttachedSignBlock.GetFace(data);
			int? color = AttachedSignBlock.GetColor(data);
			if (color != null)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_coloredBlockMeshes[face], SubsystemPalette.GetColor(generator, color), null, geometry.SubsetOpaque);
			}
			else
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[face], Color.White, null, geometry.SubsetOpaque);
			}
			generator.GenerateWireVertices(value, x, y, z, AttachedSignBlock.GetFace(data), 0.375f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000084B0 File Offset: 0x000066B0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = AttachedSignBlock.GetColor(Terrain.ExtractData(value));
			if (color2 != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneColoredBlockMesh, color * SubsystemPalette.GetColor(environmentData, color2), 1.25f * size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 1.25f * size, ref matrix, environmentData);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00008512 File Offset: 0x00006712
		public int? GetPaintColor(int value)
		{
			return AttachedSignBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00008520 File Offset: 0x00006720
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, AttachedSignBlock.SetColor(data, color));
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00008544 File Offset: 0x00006744
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int face = AttachedSignBlock.GetFace(Terrain.ExtractData(value));
			return this.m_collisionBoxes[face];
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00008568 File Offset: 0x00006768
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return default(BlockPlacementData);
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000857E File Offset: 0x0000677E
		public override BlockMesh GetSignSurfaceBlockMesh(int data)
		{
			return this.m_surfaceMeshes[AttachedSignBlock.GetFace(data)];
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000858D File Offset: 0x0000678D
		public override Vector3 GetSignSurfaceNormal(int data)
		{
			return this.m_surfaceNormals[AttachedSignBlock.GetFace(data)];
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000085A0 File Offset: 0x000067A0
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			return new SignElectricElement(subsystemElectricity, new CellFace(x, y, z, AttachedSignBlock.GetFace(data)));
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000085CC File Offset: 0x000067CC
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (face != AttachedSignBlock.GetFace(data) || SubsystemElectricity.GetConnectorDirection(face, 0, connectorFace) == null)
			{
				return null;
			}
			return new ElectricConnectorType?(ElectricConnectorType.Input);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000860C File Offset: 0x0000680C
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00008613 File Offset: 0x00006813
		public static int GetFace(int data)
		{
			return data & 3;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00008618 File Offset: 0x00006818
		public static int SetFace(int data, int face)
		{
			return (data & -4) | (face & 3);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00008624 File Offset: 0x00006824
		public static int? GetColor(int data)
		{
			if ((data & 4) != 0)
			{
				return new int?(data >> 3 & 15);
			}
			return null;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000864B File Offset: 0x0000684B
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -125) | 4 | (color.Value & 15) << 3;
			}
			return data & -125;
		}

		// Token: 0x04000069 RID: 105
		public string m_modelName;

		// Token: 0x0400006A RID: 106
		public int m_coloredTextureSlot;

		// Token: 0x0400006B RID: 107
		public int m_postedSignBlockIndex;

		// Token: 0x0400006C RID: 108
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400006D RID: 109
		public BlockMesh m_standaloneColoredBlockMesh = new BlockMesh();

		// Token: 0x0400006E RID: 110
		public BlockMesh[] m_blockMeshes = new BlockMesh[4];

		// Token: 0x0400006F RID: 111
		public BlockMesh[] m_coloredBlockMeshes = new BlockMesh[4];

		// Token: 0x04000070 RID: 112
		public BlockMesh[] m_surfaceMeshes = new BlockMesh[4];

		// Token: 0x04000071 RID: 113
		public Vector3[] m_surfaceNormals = new Vector3[4];

		// Token: 0x04000072 RID: 114
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[4][];
	}
}
