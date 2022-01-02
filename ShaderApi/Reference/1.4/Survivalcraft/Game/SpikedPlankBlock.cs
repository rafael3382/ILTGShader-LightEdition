using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000F8 RID: 248
	public class SpikedPlankBlock : MountedElectricElementBlock
	{
		// Token: 0x060004E1 RID: 1249 RVA: 0x0001B658 File Offset: 0x00019858
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/SpikedPlanks", null);
			string[] array = new string[]
			{
				"SpikedPlankRetracted",
				"SpikedPlank"
			};
			for (int i = 0; i < 2; i++)
			{
				string name = array[i];
				Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(name, true).ParentBone);
				for (int j = 0; j < 6; j++)
				{
					int num = SpikedPlankBlock.SetMountingFace(SpikedPlankBlock.SetSpikesState(0, i != 0), j);
					Matrix m = (j >= 4) ? ((j != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)j * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
					this.m_blockMeshesByData[num] = new BlockMesh();
					this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh(name, true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
					this.m_collisionBoxesByData[num] = new BoundingBox[]
					{
						this.m_blockMeshesByData[num].CalculateBoundingBox()
					};
				}
				Matrix identity = Matrix.Identity;
				this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh(name, true).MeshParts[0], boneAbsoluteTransform * identity, false, false, false, false, Color.White);
			}
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x0001B81C File Offset: 0x00019A1C
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int mountingFace = SpikedPlankBlock.GetMountingFace(Terrain.ExtractData(value));
			return face != CellFace.OppositeFace(mountingFace);
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x0001B841 File Offset: 0x00019A41
		public override bool ShouldAvoid(int value)
		{
			return SpikedPlankBlock.GetSpikesState(Terrain.ExtractData(value));
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x0001B84E File Offset: 0x00019A4E
		public static bool GetSpikesState(int data)
		{
			return (data & 1) == 0;
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x0001B856 File Offset: 0x00019A56
		public static int SetSpikesState(int data, bool spikesState)
		{
			if (spikesState)
			{
				return data & -2;
			}
			return data | 1;
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x0001B863 File Offset: 0x00019A63
		public static int GetMountingFace(int data)
		{
			return ((data >> 1) + 4) % 6;
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x0001B86C File Offset: 0x00019A6C
		public static int SetMountingFace(int data, int face)
		{
			data &= -15;
			data |= ((face + 2) % 6 & 7) << 1;
			return data;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0001B882 File Offset: 0x00019A82
		public override int GetFace(int value)
		{
			return SpikedPlankBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x0001B890 File Offset: 0x00019A90
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = SpikedPlankBlock.SetMountingFace(SpikedPlankBlock.SetSpikesState(Terrain.ExtractData(value), true), raycastResult.CellFace.Face);
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0001B8E0 File Offset: 0x00019AE0
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return base.GetCustomCollisionBoxes(terrain, value);
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0001B910 File Offset: 0x00019B10
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length && this.m_blockMeshesByData[num] != null)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 1f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0001B987 File Offset: 0x00019B87
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 1f * size, ref matrix, environmentData);
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0001B9A2 File Offset: 0x00019BA2
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new SpikedPlankElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0001B9BC File Offset: 0x00019BBC
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x04000229 RID: 553
		public const int Index = 86;

		// Token: 0x0400022A RID: 554
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400022B RID: 555
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[12];

		// Token: 0x0400022C RID: 556
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[12][];
	}
}
