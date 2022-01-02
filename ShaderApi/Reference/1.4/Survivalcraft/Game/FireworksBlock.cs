using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000066 RID: 102
	public class FireworksBlock : Block
	{
		// Token: 0x06000232 RID: 562 RVA: 0x0000F13C File Offset: 0x0000D33C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Fireworks", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Body", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Fins", true).ParentBone);
			for (int i = 0; i < 64; i++)
			{
				int num = i / 8;
				int num2 = i % 8;
				Color color = FireworksBlock.FireworksColors[num2];
				color *= 0.75f;
				color.A = byte.MaxValue;
				Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(FireworksBlock.HeadNames[num], true).ParentBone);
				this.m_headBlockMeshes[i] = new BlockMesh();
				this.m_headBlockMeshes[i].AppendModelMeshPart(model.FindMesh(FireworksBlock.HeadNames[num], true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, color);
			}
			for (int j = 0; j < 2; j++)
			{
				float num3 = 0.5f + (float)j * 0.5f;
				Matrix m = Matrix.CreateScale(new Vector3(num3, 1f, num3));
				this.m_bodyBlockMeshes[j] = new BlockMesh();
				this.m_bodyBlockMeshes[j].AppendModelMeshPart(model.FindMesh("Body", true).MeshParts[0], boneAbsoluteTransform * m * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, Color.White);
			}
			for (int k = 0; k < 2; k++)
			{
				this.m_finsBlockMeshes[k] = new BlockMesh();
				this.m_finsBlockMeshes[k].AppendModelMeshPart(model.FindMesh("Fins", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, (k == 0) ? Color.White : new Color(224, 0, 0));
			}
			base.Initialize();
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000F35B File Offset: 0x0000D55B
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000F360 File Offset: 0x0000D560
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			int color2 = FireworksBlock.GetColor(data);
			FireworksBlock.Shape shape = FireworksBlock.GetShape(data);
			int altitude = FireworksBlock.GetAltitude(data);
			bool flickering = FireworksBlock.GetFlickering(data);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_headBlockMeshes[(int)(shape * (FireworksBlock.Shape)8 + color2)], color, 2f * size, ref matrix, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_bodyBlockMeshes[altitude], color, 2f * size, ref matrix, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_finsBlockMeshes[flickering ? 1 : 0], color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000F3EC File Offset: 0x0000D5EC
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int color = FireworksBlock.GetColor(data);
			FireworksBlock.Shape shape = FireworksBlock.GetShape(data);
			int altitude = FireworksBlock.GetAltitude(data);
			bool flickering = FireworksBlock.GetFlickering(data);
			string fireworks = LanguageControl.GetFireworks("Other", "1");
			object[] array = new object[4];
			array[0] = LanguageControl.GetFireworks("FireworksColorDisplayNames", color.ToString());
			array[1] = (flickering ? LanguageControl.GetFireworks("Other", "2") : null);
			int num = 2;
			string name = "ShapeDisplayNames";
			int num2 = (int)shape;
			array[num] = LanguageControl.GetFireworks(name, num2.ToString());
			array[3] = ((altitude == 0) ? LanguageControl.GetFireworks("Other", "3") : LanguageControl.GetFireworks("Other", "4"));
			return string.Format(fireworks, array);
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000F49D File Offset: 0x0000D69D
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int color = 0; color < 8; color = num)
			{
				for (int altitude = 0; altitude < 2; altitude = num)
				{
					for (int flickering = 0; flickering < 2; flickering = num)
					{
						for (int shape = 0; shape < 8; shape = num)
						{
							yield return Terrain.MakeBlockValue(215, 0, FireworksBlock.SetColor(FireworksBlock.SetAltitude(FireworksBlock.SetShape(FireworksBlock.SetFlickering(0, flickering != 0), (FireworksBlock.Shape)shape), altitude), color));
							num = shape + 1;
						}
						num = flickering + 1;
					}
					num = altitude + 1;
				}
				num = color + 1;
			}
			yield break;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000F4A6 File Offset: 0x0000D6A6
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			int num;
			for (int shape = 0; shape < 8; shape = num)
			{
				for (int altitude = 0; altitude < 2; altitude = num)
				{
					for (int flickering = 0; flickering < 2; flickering = num)
					{
						for (int color = 0; color < 8; color = num)
						{
							CraftingRecipe craftingRecipe = new CraftingRecipe
							{
								ResultCount = 20,
								ResultValue = Terrain.MakeBlockValue(215, 0, FireworksBlock.SetColor(FireworksBlock.SetAltitude(FireworksBlock.SetShape(FireworksBlock.SetFlickering(0, flickering != 0), (FireworksBlock.Shape)shape), altitude), color)),
								RemainsCount = 1,
								RemainsValue = Terrain.MakeBlockValue(90),
								RequiredHeatLevel = 0f,
								Description = "制作烟花"
							};
							if (shape == 0)
							{
								craftingRecipe.Ingredients[0] = null;
								craftingRecipe.Ingredients[1] = "sulphurchunk";
								craftingRecipe.Ingredients[2] = null;
							}
							if (shape == 1)
							{
								craftingRecipe.Ingredients[0] = "sulphurchunk";
								craftingRecipe.Ingredients[1] = "coalchunk";
								craftingRecipe.Ingredients[2] = "sulphurchunk";
							}
							if (shape == 2)
							{
								craftingRecipe.Ingredients[0] = "sulphurchunk";
								craftingRecipe.Ingredients[1] = null;
								craftingRecipe.Ingredients[2] = "sulphurchunk";
							}
							if (shape == 3)
							{
								craftingRecipe.Ingredients[0] = "sulphurchunk";
								craftingRecipe.Ingredients[1] = "sulphurchunk";
								craftingRecipe.Ingredients[2] = "sulphurchunk";
							}
							if (shape == 4)
							{
								craftingRecipe.Ingredients[0] = "coalchunk";
								craftingRecipe.Ingredients[1] = "coalchunk";
								craftingRecipe.Ingredients[2] = "coalchunk";
							}
							if (shape == 5)
							{
								craftingRecipe.Ingredients[0] = null;
								craftingRecipe.Ingredients[1] = "saltpeterchunk";
								craftingRecipe.Ingredients[2] = null;
							}
							if (shape == 6)
							{
								craftingRecipe.Ingredients[0] = "sulphurchunk";
								craftingRecipe.Ingredients[1] = "saltpeterchunk";
								craftingRecipe.Ingredients[2] = "sulphurchunk";
							}
							if (shape == 7)
							{
								craftingRecipe.Ingredients[0] = "coalchunk";
								craftingRecipe.Ingredients[1] = "saltpeterchunk";
								craftingRecipe.Ingredients[2] = "coalchunk";
							}
							if (flickering == 0)
							{
								craftingRecipe.Ingredients[3] = "canvas";
								craftingRecipe.Ingredients[5] = "canvas";
							}
							if (flickering == 1)
							{
								craftingRecipe.Ingredients[3] = "gunpowder";
								craftingRecipe.Ingredients[5] = "gunpowder";
							}
							if (altitude == 0)
							{
								craftingRecipe.Ingredients[6] = "gunpowder";
								craftingRecipe.Ingredients[7] = null;
								craftingRecipe.Ingredients[8] = "gunpowder";
							}
							if (altitude == 1)
							{
								craftingRecipe.Ingredients[6] = "gunpowder";
								craftingRecipe.Ingredients[7] = "gunpowder";
								craftingRecipe.Ingredients[8] = "gunpowder";
							}
							craftingRecipe.Ingredients[4] = "paintbucket:" + ((color != 7) ? color : 10).ToString(CultureInfo.InvariantCulture);
							yield return craftingRecipe;
							num = color + 1;
						}
						num = flickering + 1;
					}
					num = altitude + 1;
				}
				num = shape + 1;
			}
			yield break;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000F4AF File Offset: 0x0000D6AF
		public static FireworksBlock.Shape GetShape(int data)
		{
			return (FireworksBlock.Shape)(data & 7);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000F4B4 File Offset: 0x0000D6B4
		public static int SetShape(int data, FireworksBlock.Shape shape)
		{
			return (data & -8) | (int)(shape & FireworksBlock.Shape.FlatTrails);
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000F4BE File Offset: 0x0000D6BE
		public static int GetAltitude(int data)
		{
			return data >> 3 & 1;
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000F4C5 File Offset: 0x0000D6C5
		public static int SetAltitude(int data, int altitude)
		{
			return (data & -9) | (altitude & 1) << 3;
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000F4D1 File Offset: 0x0000D6D1
		public static bool GetFlickering(int data)
		{
			return (data >> 4 & 1) != 0;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000F4DB File Offset: 0x0000D6DB
		public static int SetFlickering(int data, bool flickering)
		{
			return (data & -17) | (flickering ? 1 : 0) << 4;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000F4EB File Offset: 0x0000D6EB
		public static int GetColor(int data)
		{
			return data >> 5 & 7;
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000F4F2 File Offset: 0x0000D6F2
		public static int SetColor(int data, int color)
		{
			return (data & -225) | (color & 7) << 5;
		}

		// Token: 0x0400010B RID: 267
		public const int Index = 215;

		// Token: 0x0400010C RID: 268
		public BlockMesh[] m_headBlockMeshes = new BlockMesh[64];

		// Token: 0x0400010D RID: 269
		public BlockMesh[] m_bodyBlockMeshes = new BlockMesh[2];

		// Token: 0x0400010E RID: 270
		public BlockMesh[] m_finsBlockMeshes = new BlockMesh[2];

		// Token: 0x0400010F RID: 271
		public static readonly string[] HeadNames = new string[]
		{
			"HeadConeSmall",
			"HeadConeLarge",
			"HeadCylinderSmall",
			"HeadCylinderLarge",
			"HeadSphere",
			"HeadDiamondSmall",
			"HeadDiamondLarge",
			"HeadCylinderFlat"
		};

		// Token: 0x04000110 RID: 272
		public static readonly Color[] FireworksColors = new Color[]
		{
			new Color(255, 255, 255),
			new Color(85, 255, 255),
			new Color(255, 85, 85),
			new Color(85, 85, 255),
			new Color(255, 255, 85),
			new Color(85, 255, 85),
			new Color(255, 170, 0),
			new Color(255, 85, 255)
		};

		// Token: 0x020003F2 RID: 1010
		public enum Shape
		{
			// Token: 0x0400148A RID: 5258
			SmallBurst,
			// Token: 0x0400148B RID: 5259
			LargeBurst,
			// Token: 0x0400148C RID: 5260
			Circle,
			// Token: 0x0400148D RID: 5261
			Disc,
			// Token: 0x0400148E RID: 5262
			Ball,
			// Token: 0x0400148F RID: 5263
			ShortTrails,
			// Token: 0x04001490 RID: 5264
			LongTrails,
			// Token: 0x04001491 RID: 5265
			FlatTrails
		}
	}
}
