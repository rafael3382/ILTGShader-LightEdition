using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000DF RID: 223
	public class RottenPumpkinSoupBucketBlock : BucketBlock
	{
		// Token: 0x06000467 RID: 1127 RVA: 0x000193D0 File Offset: 0x000175D0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/FullBucket", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Contents", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, new Color(255, 160, 64));
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0f, 0.625f, 0f), -1);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x00019501 File Offset: 0x00017701
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040001F9 RID: 505
		public const int Index = 252;

		// Token: 0x040001FA RID: 506
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
