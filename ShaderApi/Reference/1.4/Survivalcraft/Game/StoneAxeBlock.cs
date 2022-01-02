﻿using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000FF RID: 255
	public class StoneAxeBlock : Block
	{
		// Token: 0x06000518 RID: 1304 RVA: 0x0001D6B0 File Offset: 0x0001B8B0
		public override void Initialize()
		{
			int num = 47;
			int num2 = 1;
			Model model = ContentManager.Get<Model>("Models/StoneAxe", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Handle", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Head", true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("Handle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(num % 16) / 16f, (float)(num / 16) / 16f, 0f), -1);
			BlockMesh blockMesh2 = new BlockMesh();
			blockMesh2.AppendModelMeshPart(model.FindMesh("Head", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh2.TransformTextureCoordinates(Matrix.CreateTranslation((float)(num2 % 16) / 16f, (float)(num2 / 16) / 16f, 0f), -1);
			this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
			this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh2);
			base.Initialize();
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x0001D806 File Offset: 0x0001BA06
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0001D808 File Offset: 0x0001BA08
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0400023E RID: 574
		public const int Index = 29;

		// Token: 0x0400023F RID: 575
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
