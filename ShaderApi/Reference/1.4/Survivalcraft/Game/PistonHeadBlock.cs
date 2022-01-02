using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C7 RID: 199
	public class PistonHeadBlock : Block
	{
		// Token: 0x06000401 RID: 1025 RVA: 0x000173B0 File Offset: 0x000155B0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Pistons", null);
			for (int i = 0; i < 2; i++)
			{
				string name = (i == 0) ? "PistonHead" : "PistonShaft";
				Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(name, true).ParentBone);
				for (PistonMode pistonMode = PistonMode.Pushing; pistonMode <= PistonMode.StrictPulling; pistonMode++)
				{
					for (int j = 0; j < 6; j++)
					{
						int num = PistonHeadBlock.SetFace(PistonHeadBlock.SetMode(PistonHeadBlock.SetIsShaft(0, i != 0), pistonMode), j);
						Matrix m = (j < 4) ? (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationY((float)j * 3.14159274f / 2f + 3.14159274f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)) : ((j != 4) ? (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationX(-1.57079637f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)) : (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)));
						this.m_blockMeshesByData[num] = new BlockMesh();
						this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh(name, true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
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
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x000175C4 File Offset: 0x000157C4
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int data = Terrain.ExtractData(value);
			return face != PistonHeadBlock.GetFace(data);
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x000175E4 File Offset: 0x000157E4
		public override int GetShadowStrength(int value)
		{
			if (!PistonHeadBlock.GetIsShaft(Terrain.ExtractData(value)))
			{
				return base.GetShadowStrength(value);
			}
			return 0;
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x000175FC File Offset: 0x000157FC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length && this.m_blockMeshesByData[num] != null)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, null, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0001764F File Offset: 0x0001584F
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00017651 File Offset: 0x00015851
		public static PistonMode GetMode(int data)
		{
			return (PistonMode)(data & 3);
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00017656 File Offset: 0x00015856
		public static int SetMode(int data, PistonMode mode)
		{
			return (data & -4) | (int)(mode & (PistonMode)3);
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00017660 File Offset: 0x00015860
		public static bool GetIsShaft(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x00017668 File Offset: 0x00015868
		public static int SetIsShaft(int data, bool isShaft)
		{
			return (data & -5) | (isShaft ? 4 : 0);
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00017676 File Offset: 0x00015876
		public static int GetFace(int data)
		{
			return data >> 3 & 7;
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x0001767D File Offset: 0x0001587D
		public static int SetFace(int data, int face)
		{
			return (data & -57) | (face & 7) << 3;
		}

		// Token: 0x040001C6 RID: 454
		public const int Index = 238;

		// Token: 0x040001C7 RID: 455
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[48];
	}
}
