using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C0 RID: 192
	public class PaintBucketBlock : BucketBlock
	{
		// Token: 0x060003BC RID: 956 RVA: 0x000162B8 File Offset: 0x000144B8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/FullBucket", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents", true).ParentBone);
			this.m_standaloneBucketBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			this.m_standalonePaintBlockMesh.AppendModelMeshPart(model.FindMesh("Contents", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			this.m_standalonePaintBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0f, 0f), -1);
			base.Initialize();
		}

		// Token: 0x060003BD RID: 957 RVA: 0x000163E0 File Offset: 0x000145E0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int color2 = PaintBucketBlock.GetColor(Terrain.ExtractData(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBucketBlockMesh, color, 2f * size, ref matrix, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standalonePaintBlockMesh, color * SubsystemPalette.GetColor(environmentData, new int?(color2)), 2f * size, ref matrix, environmentData);
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0001643D File Offset: 0x0001463D
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(129, 0, PaintBucketBlock.SetColor(0, i));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00016446 File Offset: 0x00014646
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			string[] additives = new string[]
			{
				BlocksManager.Blocks[43].CraftingId,
				BlocksManager.Blocks[24].CraftingId,
				BlocksManager.Blocks[103].CraftingId,
				BlocksManager.Blocks[22].CraftingId
			};
			int num2;
			for (int color = 0; color < 16; color = num2)
			{
				for (int additive = 0; additive < 4; additive = num2)
				{
					int num = PaintBucketBlock.CombineColors(color, 1 << additive);
					if (num != color)
					{
						CraftingRecipe craftingRecipe = new CraftingRecipe
						{
							Description = "制作 " + SubsystemPalette.GetName(null, new int?(num), null) + " 颜料",
							ResultValue = Terrain.MakeBlockValue(129, 0, num),
							ResultCount = 1,
							RequiredHeatLevel = 1f
						};
						craftingRecipe.Ingredients[0] = BlocksManager.Blocks[129].CraftingId + ":" + color.ToString(CultureInfo.InvariantCulture);
						craftingRecipe.Ingredients[1] = additives[additive];
						yield return craftingRecipe;
					}
					num2 = additive + 1;
				}
				num2 = color + 1;
			}
			yield break;
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00016450 File Offset: 0x00014650
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int color = PaintBucketBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, new int?(color), "颜料桶");
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0001647A File Offset: 0x0001467A
		public override int GetDamageDestructionValue(int value)
		{
			return Terrain.MakeBlockValue(90);
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x00016483 File Offset: 0x00014683
		public static int GetColor(int data)
		{
			return data & 15;
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00016489 File Offset: 0x00014689
		public static int SetColor(int data, int color)
		{
			return (data & -16) | (color & 15);
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00016494 File Offset: 0x00014694
		public static Vector4 ColorToCmyk(int color)
		{
			float num = (float)(color & 1);
			int num2 = color >> 1 & 1;
			int num3 = color >> 2 & 1;
			int num4 = color >> 3 & 1;
			return new Vector4(num, (float)num2, (float)num3, (float)num4);
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x000164C4 File Offset: 0x000146C4
		public static int CmykToColor(Vector4 cmyk)
		{
			if (cmyk.W <= 1f)
			{
				int num = (int)MathUtils.Round(MathUtils.Saturate(cmyk.X));
				int num2 = (int)MathUtils.Round(MathUtils.Saturate(cmyk.Y));
				int num3 = (int)MathUtils.Round(MathUtils.Saturate(cmyk.Z));
				int num4 = (int)MathUtils.Round(MathUtils.Saturate(cmyk.W));
				return num | num2 << 1 | num3 << 2 | num4 << 3;
			}
			return 15;
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x00016534 File Offset: 0x00014734
		public static int CombineColors(int color1, int color2)
		{
			return PaintBucketBlock.CmykToColor(PaintBucketBlock.ColorToCmyk(color1) + PaintBucketBlock.ColorToCmyk(color2));
		}

		// Token: 0x040001B5 RID: 437
		public const int Index = 129;

		// Token: 0x040001B6 RID: 438
		public BlockMesh m_standaloneBucketBlockMesh = new BlockMesh();

		// Token: 0x040001B7 RID: 439
		public BlockMesh m_standalonePaintBlockMesh = new BlockMesh();
	}
}
