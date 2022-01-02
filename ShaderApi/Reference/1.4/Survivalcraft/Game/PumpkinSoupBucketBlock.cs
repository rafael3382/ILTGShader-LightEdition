using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000CC RID: 204
	public class PumpkinSoupBucketBlock : BucketBlock
	{
		// Token: 0x0600043C RID: 1084 RVA: 0x00018798 File Offset: 0x00016998
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/FullBucket", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Contents", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, new Color(200, 130, 35));
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), -1);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x000188C9 File Offset: 0x00016AC9
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x000188E4 File Offset: 0x00016AE4
		public override int GetDamageDestructionValue(int value)
		{
			return 252;
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x000188EB File Offset: 0x00016AEB
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			int num;
			for (int isDead = 0; isDead <= 1; isDead = num)
			{
				for (int rot = 0; rot <= 1; rot = num)
				{
					CraftingRecipe craftingRecipe = new CraftingRecipe
					{
						ResultCount = 1,
						ResultValue = 251,
						RequiredHeatLevel = 1f,
						Description = "烹饪南瓜粥"
					};
					int data = BasePumpkinBlock.SetIsDead(BasePumpkinBlock.SetSize(0, 7), isDead != 0);
					int value = this.SetDamage(Terrain.MakeBlockValue(131, 0, data), rot);
					craftingRecipe.Ingredients[0] = "pumpkin:" + Terrain.ExtractData(value).ToString(CultureInfo.InvariantCulture);
					craftingRecipe.Ingredients[1] = "waterbucket";
					yield return craftingRecipe;
					num = rot + 1;
				}
				num = isDead + 1;
			}
			yield break;
		}

		// Token: 0x040001DC RID: 476
		public const int Index = 251;

		// Token: 0x040001DD RID: 477
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
