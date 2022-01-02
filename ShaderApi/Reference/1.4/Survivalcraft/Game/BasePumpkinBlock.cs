using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000018 RID: 24
	public abstract class BasePumpkinBlock : Block
	{
		// Token: 0x060000DA RID: 218 RVA: 0x00008875 File Offset: 0x00006A75
		public BasePumpkinBlock(bool isRotten)
		{
			this.m_isRotten = isRotten;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x000088A8 File Offset: 0x00006AA8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Pumpkins", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Pumpkin", true).ParentBone);
			for (int i = 0; i < 8; i++)
			{
				float num = MathUtils.Lerp(0.2f, 1f, (float)i / 7f);
				float num2 = MathUtils.Min(0.3f * num, 0.7f * (1f - num));
				Color color;
				if (this.m_isRotten)
				{
					color = Color.White;
				}
				else
				{
					color = Color.Lerp(new Color(0, 128, 128), new Color(80, 255, 255), (float)i / 7f);
					if (i == 7)
					{
						color.R = byte.MaxValue;
					}
				}
				this.m_blockMeshesBySize[i] = new BlockMesh();
				if (i >= 1)
				{
					this.m_blockMeshesBySize[i].AppendModelMeshPart(model.FindMesh("Pumpkin", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(num) * Matrix.CreateTranslation(0.5f + num2, 0f, 0.5f + num2), false, false, false, false, color);
				}
				if (this.m_isRotten)
				{
					this.m_blockMeshesBySize[i].TransformTextureCoordinates(Matrix.CreateTranslation(-0.375f, 0.25f, 0f), -1);
				}
				this.m_standaloneBlockMeshesBySize[i] = new BlockMesh();
				this.m_standaloneBlockMeshesBySize[i].AppendModelMeshPart(model.FindMesh("Pumpkin", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(num) * Matrix.CreateTranslation(0f, -0.23f, 0f), false, false, false, false, color);
				if (this.m_isRotten)
				{
					this.m_standaloneBlockMeshesBySize[i].TransformTextureCoordinates(Matrix.CreateTranslation(-0.375f, 0.25f, 0f), -1);
				}
			}
			for (int j = 0; j < 8; j++)
			{
				BoundingBox boundingBox = (this.m_blockMeshesBySize[j].Vertices.Count > 0) ? this.m_blockMeshesBySize[j].CalculateBoundingBox() : new BoundingBox(new Vector3(0.5f, 0f, 0.5f), new Vector3(0.5f, 0f, 0.5f));
				float num3 = boundingBox.Max.X - boundingBox.Min.X;
				if (num3 < 0.8f)
				{
					float num4 = (0.8f - num3) / 2f;
					boundingBox.Min.X = boundingBox.Min.X - num4;
					boundingBox.Min.Z = boundingBox.Min.Z - num4;
					boundingBox.Max.X = boundingBox.Max.X + num4;
					boundingBox.Max.Y = 0.4f;
					boundingBox.Max.Z = boundingBox.Max.Z + num4;
				}
				this.m_collisionBoxesBySize[j] = new BoundingBox[]
				{
					boundingBox
				};
			}
			base.Initialize();
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00008BA4 File Offset: 0x00006DA4
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int size = BasePumpkinBlock.GetSize(Terrain.ExtractData(value));
			return this.m_collisionBoxesBySize[size];
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00008BC8 File Offset: 0x00006DC8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int size = BasePumpkinBlock.GetSize(data);
			bool isDead = BasePumpkinBlock.GetIsDead(data);
			if (size >= 1)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesBySize[size], Color.White, null, geometry.SubsetOpaque);
			}
			if (size == 0)
			{
				generator.GenerateCrossfaceVertices(this, value, x, y, z, new Color(160, 160, 160), 11, geometry.SubsetAlphaTest);
				return;
			}
			if (size < 7 && !isDead)
			{
				generator.GenerateCrossfaceVertices(this, value, x, y, z, Color.White, 28, geometry.SubsetAlphaTest);
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00008C68 File Offset: 0x00006E68
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int size2 = BasePumpkinBlock.GetSize(Terrain.ExtractData(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshesBySize[size2], color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00008C9C File Offset: 0x00006E9C
		public override int GetShadowStrength(int value)
		{
			return BasePumpkinBlock.GetSize(Terrain.ExtractData(value));
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00008CA9 File Offset: 0x00006EA9
		public override float GetNutritionalValue(int value)
		{
			if (BasePumpkinBlock.GetSize(Terrain.ExtractData(value)) != 7)
			{
				return 0f;
			}
			return base.GetNutritionalValue(value);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00008CC8 File Offset: 0x00006EC8
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int size = BasePumpkinBlock.GetSize(Terrain.ExtractData(oldValue));
			if (size >= 1)
			{
				int value = this.SetDamage(Terrain.MakeBlockValue(this.BlockIndex, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, true), size)), this.GetDamage(oldValue));
				dropValues.Add(new BlockDropValue
				{
					Value = value,
					Count = 1
				});
			}
			showDebris = true;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00008D30 File Offset: 0x00006F30
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int size = BasePumpkinBlock.GetSize(Terrain.ExtractData(value));
			float num = MathUtils.Lerp(0.2f, 1f, (float)size / 7f);
			Color color = (size == 7) ? Color.White : new Color(0, 128, 128);
			return new BlockDebrisParticleSystem(subsystemTerrain, position, 1.5f * strength, this.DestructionDebrisScale * num, color, this.DefaultTextureSlot);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00008D9C File Offset: 0x00006F9C
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int size = BasePumpkinBlock.GetSize(Terrain.ExtractData(value));
			if (this.m_isRotten)
			{
				if (size >= 7)
				{
					return "腐烂的南瓜";
				}
				return "腐烂未成熟的南瓜";
			}
			else
			{
				if (size >= 7)
				{
					return "南瓜";
				}
				return "未成熟的南瓜";
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00008DDC File Offset: 0x00006FDC
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, true), 1));
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, true), 3));
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, true), 5));
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, true), 7));
			yield break;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00008DEC File Offset: 0x00006FEC
		public static int GetSize(int data)
		{
			return 7 - (data & 7);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00008DF3 File Offset: 0x00006FF3
		public static int SetSize(int data, int size)
		{
			return (data & -8) | 7 - (size & 7);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00008DFF File Offset: 0x00006FFF
		public static bool GetIsDead(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00008E07 File Offset: 0x00007007
		public static int SetIsDead(int data, bool isDead)
		{
			if (!isDead)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00008E14 File Offset: 0x00007014
		public override int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) & 16) >> 4;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00008E24 File Offset: 0x00007024
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, (num & -17) | (damage & 1) << 4);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00008E48 File Offset: 0x00007048
		public override int GetDamageDestructionValue(int value)
		{
			if (this.m_isRotten)
			{
				return 0;
			}
			int data = Terrain.ExtractData(value);
			return this.SetDamage(Terrain.MakeBlockValue(244, 0, data), 0);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00008E79 File Offset: 0x00007079
		public override int GetRotPeriod(int value)
		{
			if (!BasePumpkinBlock.GetIsDead(Terrain.ExtractData(value)))
			{
				return 0;
			}
			return this.DefaultRotPeriod;
		}

		// Token: 0x0400007A RID: 122
		public BlockMesh[] m_blockMeshesBySize = new BlockMesh[8];

		// Token: 0x0400007B RID: 123
		public BlockMesh[] m_standaloneBlockMeshesBySize = new BlockMesh[8];

		// Token: 0x0400007C RID: 124
		public BoundingBox[][] m_collisionBoxesBySize = new BoundingBox[8][];

		// Token: 0x0400007D RID: 125
		public bool m_isRotten;
	}
}
