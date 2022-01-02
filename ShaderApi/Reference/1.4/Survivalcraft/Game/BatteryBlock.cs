using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000019 RID: 25
	public class BatteryBlock : Block, IElectricElementBlock
	{
		// Token: 0x060000ED RID: 237 RVA: 0x00008E90 File Offset: 0x00007090
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Battery", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Battery", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Battery", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_blockMesh.AppendModelMeshPart(model.FindMesh("Battery", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_collisionBoxes[0] = this.m_blockMesh.CalculateBoundingBox();
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00008F68 File Offset: 0x00007168
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			if (toolLevel >= this.RequiredToolLevel)
			{
				int data = Terrain.ExtractData(oldValue);
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(138, 0, data),
					Count = 1
				});
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00008FB5 File Offset: 0x000071B5
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00008FC0 File Offset: 0x000071C0
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMesh, Color.White, null, geometry.SubsetOpaque);
			generator.GenerateWireVertices(value, x, y, z, 4, 0.72f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00009012 File Offset: 0x00007212
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 1f * size, ref matrix, environmentData);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000902D File Offset: 0x0000722D
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new BatteryElectricElement(subsystemElectricity, new CellFace(x, y, z, 4));
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00009040 File Offset: 0x00007240
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (face == 4 && SubsystemElectricity.GetConnectorDirection(4, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00009074 File Offset: 0x00007274
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000907B File Offset: 0x0000727B
		public static int GetVoltageLevel(int data)
		{
			return 15 - (data & 15);
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00009084 File Offset: 0x00007284
		public static int SetVoltageLevel(int data, int voltageLevel)
		{
			return (data & -16) | 15 - (voltageLevel & 15);
		}

		// Token: 0x0400007E RID: 126
		public const int Index = 138;

		// Token: 0x0400007F RID: 127
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000080 RID: 128
		public BlockMesh m_blockMesh = new BlockMesh();

		// Token: 0x04000081 RID: 129
		public BoundingBox[] m_collisionBoxes = new BoundingBox[1];
	}
}
