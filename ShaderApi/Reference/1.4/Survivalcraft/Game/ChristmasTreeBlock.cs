using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000031 RID: 49
	public class ChristmasTreeBlock : Block, IElectricElementBlock
	{
		// Token: 0x06000154 RID: 340 RVA: 0x0000A91C File Offset: 0x00008B1C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/ChristmasTree", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StandTrunk", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Leaves", true).ParentBone);
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Decorations", true).ParentBone);
			Color color = BlockColorsMap.SpruceLeavesColorsMap.Lookup(4, 15);
			this.m_leavesBlockMesh.AppendModelMeshPart(model.FindMesh("Leaves", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, true, false, Color.White);
			this.m_standTrunkBlockMesh.AppendModelMeshPart(model.FindMesh("StandTrunk", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_decorationsBlockMesh.AppendModelMeshPart(model.FindMesh("Decorations", true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_litDecorationsBlockMesh.AppendModelMeshPart(model.FindMesh("Decorations", true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0.5f, 0f, 0.5f), true, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("StandTrunk", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -1f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Leaves", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -1f, 0f), false, false, true, false, color);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Decorations", true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, -1f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000AB8C File Offset: 0x00008D8C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color color = BlockColorsMap.SpruceLeavesColorsMap.Lookup(generator.Terrain, x, y, z);
			if (ChristmasTreeBlock.GetLightState(Terrain.ExtractData(value)))
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_standTrunkBlockMesh, Color.White, null, geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_litDecorationsBlockMesh, Color.White, null, geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_leavesBlockMesh, color, null, geometry.SubsetAlphaTest);
			}
			else
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_standTrunkBlockMesh, Color.White, null, geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_decorationsBlockMesh, Color.White, null, geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_leavesBlockMesh, color, null, geometry.SubsetAlphaTest);
			}
			generator.GenerateWireVertices(value, x, y, z, 4, 0.01f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000ACBF File Offset: 0x00008EBF
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000ACD4 File Offset: 0x00008ED4
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			Color color = BlockColorsMap.SpruceLeavesColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.DefaultTextureSlot);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000AD29 File Offset: 0x00008F29
		public override int GetEmittedLightAmount(int value)
		{
			if (!ChristmasTreeBlock.GetLightState(Terrain.ExtractData(value)))
			{
				return 0;
			}
			return this.DefaultEmittedLightAmount;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000AD40 File Offset: 0x00008F40
		public override int GetShadowStrength(int value)
		{
			if (!ChristmasTreeBlock.GetLightState(Terrain.ExtractData(value)))
			{
				return this.DefaultShadowStrength;
			}
			return -99;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000AD58 File Offset: 0x00008F58
		public static bool GetLightState(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000AD60 File Offset: 0x00008F60
		public static int SetLightState(int data, bool state)
		{
			if (!state)
			{
				return data & -2;
			}
			return data | 1;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000AD6D File Offset: 0x00008F6D
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new ChristmasTreeElectricElement(subsystemElectricity, new CellFace(x, y, z, 4), value);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000AD84 File Offset: 0x00008F84
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (face == 4 && SubsystemElectricity.GetConnectorDirection(4, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000ADB8 File Offset: 0x00008FB8
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x040000A9 RID: 169
		public const int Index = 63;

		// Token: 0x040000AA RID: 170
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040000AB RID: 171
		public BlockMesh m_leavesBlockMesh = new BlockMesh();

		// Token: 0x040000AC RID: 172
		public BlockMesh m_standTrunkBlockMesh = new BlockMesh();

		// Token: 0x040000AD RID: 173
		public BlockMesh m_decorationsBlockMesh = new BlockMesh();

		// Token: 0x040000AE RID: 174
		public BlockMesh m_litDecorationsBlockMesh = new BlockMesh();
	}
}
