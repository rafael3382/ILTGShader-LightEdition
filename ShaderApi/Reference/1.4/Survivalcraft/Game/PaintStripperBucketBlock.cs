using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C2 RID: 194
	public class PaintStripperBucketBlock : BucketBlock
	{
		// Token: 0x060003D6 RID: 982 RVA: 0x000167C8 File Offset: 0x000149C8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/FullBucket", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Contents", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.8125f, 0.6875f, 0f), -1);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bucket", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x000168ED File Offset: 0x00014AED
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00016908 File Offset: 0x00014B08
		public override int GetDamageDestructionValue(int value)
		{
			return Terrain.MakeBlockValue(90);
		}

		// Token: 0x040001B9 RID: 441
		public const int Index = 128;

		// Token: 0x040001BA RID: 442
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
