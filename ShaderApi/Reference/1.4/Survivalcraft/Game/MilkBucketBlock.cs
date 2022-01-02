using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000B0 RID: 176
	public class MilkBucketBlock : BucketBlock
	{
		// Token: 0x06000371 RID: 881 RVA: 0x0001508C File Offset: 0x0001328C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/FullBucket", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Contents", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0f, 0f), -1);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000372 RID: 882 RVA: 0x000151B1 File Offset: 0x000133B1
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000373 RID: 883 RVA: 0x000151CC File Offset: 0x000133CC
		public override int GetDamageDestructionValue(int value)
		{
			return 245;
		}

		// Token: 0x04000197 RID: 407
		public const int Index = 110;

		// Token: 0x04000198 RID: 408
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
