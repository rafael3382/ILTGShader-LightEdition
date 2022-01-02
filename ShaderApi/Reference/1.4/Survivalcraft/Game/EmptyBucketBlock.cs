using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000060 RID: 96
	public class EmptyBucketBlock : BucketBlock
	{
		// Token: 0x060001F9 RID: 505 RVA: 0x0000D10C File Offset: 0x0000B30C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/EmptyBucket", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000D19E File Offset: 0x0000B39E
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040000EE RID: 238
		public const int Index = 90;

		// Token: 0x040000EF RID: 239
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
