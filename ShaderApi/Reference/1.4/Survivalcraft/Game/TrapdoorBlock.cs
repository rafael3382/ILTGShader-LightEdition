using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000112 RID: 274
	public abstract class TrapdoorBlock : Block, IElectricElementBlock
	{
		// Token: 0x0600055A RID: 1370 RVA: 0x0001F2EB File Offset: 0x0001D4EB
		public TrapdoorBlock(string modelName)
		{
			this.m_modelName = modelName;
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x0001F320 File Offset: 0x0001D520
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName, null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Trapdoor", true).ParentBone);
			for (int i = 0; i < 16; i++)
			{
				int rotation = TrapdoorBlock.GetRotation(i);
				bool open = TrapdoorBlock.GetOpen(i);
				bool upsideDown = TrapdoorBlock.GetUpsideDown(i);
				this.m_blockMeshesByData[i] = new BlockMesh();
				Matrix matrix = Matrix.Identity;
				matrix *= Matrix.CreateTranslation(0f, -0.0625f, 0.4375f) * Matrix.CreateRotationX(open ? -1.57079637f : 0f) * Matrix.CreateTranslation(0f, 0.0625f, -0.4375f);
				matrix *= Matrix.CreateRotationZ(upsideDown ? 3.14159274f : 0f);
				matrix *= Matrix.CreateRotationY((float)rotation * 3.14159274f / 2f);
				matrix *= Matrix.CreateTranslation(new Vector3(0.5f, (float)(upsideDown ? 1 : 0), 0.5f));
				this.m_blockMeshesByData[i].AppendModelMeshPart(model.FindMesh("Trapdoor", true).MeshParts[0], boneAbsoluteTransform * matrix, false, false, false, false, Color.White);
				this.m_blockMeshesByData[i].GenerateSidesData();
				this.m_collisionBoxesByData[i] = new BoundingBox[]
				{
					this.m_blockMeshesByData[i].CalculateBoundingBox()
				};
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Trapdoor", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x0001F4F8 File Offset: 0x0001D6F8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, null, geometry.SubsetAlphaTest);
			}
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x0001F541 File Offset: 0x0001D741
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x0001F558 File Offset: 0x0001D758
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int rotation;
			bool upsideDown;
			if (raycastResult.CellFace.Face < 4)
			{
				rotation = raycastResult.CellFace.Face;
				upsideDown = (raycastResult.HitPoint(0f).Y - (float)raycastResult.CellFace.Y > 0.5f);
			}
			else
			{
				Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
				float num = Vector3.Dot(forward, Vector3.UnitZ);
				float num2 = Vector3.Dot(forward, Vector3.UnitX);
				float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
				float num4 = Vector3.Dot(forward, -Vector3.UnitX);
				rotation = ((num == MathUtils.Max(num, num2, num3, num4)) ? 2 : ((num2 == MathUtils.Max(num, num2, num3, num4)) ? 3 : ((num3 != MathUtils.Max(num, num2, num3, num4)) ? ((num4 == MathUtils.Max(num, num2, num3, num4)) ? 1 : 0) : 0)));
				upsideDown = (raycastResult.CellFace.Face == 5);
			}
			int data = TrapdoorBlock.SetOpen(TrapdoorBlock.SetRotation(TrapdoorBlock.SetUpsideDown(0, upsideDown), rotation), false);
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, this.BlockIndex), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x0001F6B0 File Offset: 0x0001D8B0
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxesByData.Length)
			{
				return this.m_collisionBoxesByData[num];
			}
			return base.GetCustomCollisionBoxes(terrain, value);
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x0001F6E0 File Offset: 0x0001D8E0
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			return new TrapDoorElectricElement(subsystemElectricity, new CellFace(x, y, z, TrapdoorBlock.GetMountingFace(data)));
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x0001F70C File Offset: 0x0001D90C
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (face == TrapdoorBlock.GetMountingFace(data))
			{
				int rotation = TrapdoorBlock.GetRotation(data);
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(4, (4 - rotation) % 4, connectorFace);
				ElectricConnectorDirection electricConnectorDirection = ElectricConnectorDirection.Top;
				if (connectorDirection.GetValueOrDefault() == electricConnectorDirection & connectorDirection != null)
				{
					return new ElectricConnectorType?(ElectricConnectorType.Input);
				}
			}
			return null;
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x0001F765 File Offset: 0x0001D965
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x0001F76C File Offset: 0x0001D96C
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0001F771 File Offset: 0x0001D971
		public static bool GetOpen(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x0001F779 File Offset: 0x0001D979
		public static bool GetUpsideDown(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x0001F781 File Offset: 0x0001D981
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x0001F78B File Offset: 0x0001D98B
		public static int SetOpen(int data, bool open)
		{
			if (!open)
			{
				return data & -5;
			}
			return data | 4;
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x0001F798 File Offset: 0x0001D998
		public static int SetUpsideDown(int data, bool upsideDown)
		{
			if (!upsideDown)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x0001F7A5 File Offset: 0x0001D9A5
		public static int GetMountingFace(int data)
		{
			if (!TrapdoorBlock.GetUpsideDown(data))
			{
				return 4;
			}
			return 5;
		}

		// Token: 0x0400025F RID: 607
		public string m_modelName;

		// Token: 0x04000260 RID: 608
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000261 RID: 609
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[16];

		// Token: 0x04000262 RID: 610
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[16][];
	}
}
