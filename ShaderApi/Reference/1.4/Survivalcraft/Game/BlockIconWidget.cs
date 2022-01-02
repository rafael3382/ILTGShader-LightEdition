using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200036F RID: 879
	public class BlockIconWidget : Widget
	{
		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x060019FF RID: 6655 RVA: 0x000CC34F File Offset: 0x000CA54F
		// (set) Token: 0x06001A00 RID: 6656 RVA: 0x000CC357 File Offset: 0x000CA557
		public DrawBlockEnvironmentData DrawBlockEnvironmentData { get; set; }

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06001A01 RID: 6657 RVA: 0x000CC360 File Offset: 0x000CA560
		// (set) Token: 0x06001A02 RID: 6658 RVA: 0x000CC368 File Offset: 0x000CA568
		public Vector2 Size { get; set; }

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06001A03 RID: 6659 RVA: 0x000CC371 File Offset: 0x000CA571
		// (set) Token: 0x06001A04 RID: 6660 RVA: 0x000CC379 File Offset: 0x000CA579
		public float Depth { get; set; }

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06001A05 RID: 6661 RVA: 0x000CC382 File Offset: 0x000CA582
		// (set) Token: 0x06001A06 RID: 6662 RVA: 0x000CC38A File Offset: 0x000CA58A
		public Color Color { get; set; }

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06001A07 RID: 6663 RVA: 0x000CC393 File Offset: 0x000CA593
		// (set) Token: 0x06001A08 RID: 6664 RVA: 0x000CC39B File Offset: 0x000CA59B
		public Matrix? CustomViewMatrix { get; set; }

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06001A09 RID: 6665 RVA: 0x000CC3A4 File Offset: 0x000CA5A4
		// (set) Token: 0x06001A0A RID: 6666 RVA: 0x000CC3AC File Offset: 0x000CA5AC
		public int Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				if (this.m_value == 0 || value != this.m_value)
				{
					this.m_value = value;
					Block block = BlocksManager.Blocks[this.Contents];
					this.m_viewMatrix = Matrix.CreateLookAt(block.GetIconViewOffset(this.Value, this.DrawBlockEnvironmentData), new Vector3(0f, 0f, 0f), Vector3.UnitY);
				}
			}
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06001A0B RID: 6667 RVA: 0x000CC414 File Offset: 0x000CA614
		// (set) Token: 0x06001A0C RID: 6668 RVA: 0x000CC421 File Offset: 0x000CA621
		public int Contents
		{
			get
			{
				return Terrain.ExtractContents(this.Value);
			}
			set
			{
				this.Value = Terrain.ReplaceContents(this.Value, value);
			}
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06001A0D RID: 6669 RVA: 0x000CC435 File Offset: 0x000CA635
		// (set) Token: 0x06001A0E RID: 6670 RVA: 0x000CC442 File Offset: 0x000CA642
		public int Light
		{
			get
			{
				return Terrain.ExtractLight(this.Value);
			}
			set
			{
				this.Value = Terrain.ReplaceLight(this.Value, value);
			}
		}

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06001A0F RID: 6671 RVA: 0x000CC456 File Offset: 0x000CA656
		// (set) Token: 0x06001A10 RID: 6672 RVA: 0x000CC463 File Offset: 0x000CA663
		public int Data
		{
			get
			{
				return Terrain.ExtractData(this.Value);
			}
			set
			{
				this.Value = Terrain.ReplaceData(this.Value, value);
			}
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06001A11 RID: 6673 RVA: 0x000CC477 File Offset: 0x000CA677
		// (set) Token: 0x06001A12 RID: 6674 RVA: 0x000CC47F File Offset: 0x000CA67F
		public float Scale { get; set; }

		// Token: 0x06001A13 RID: 6675 RVA: 0x000CC488 File Offset: 0x000CA688
		public BlockIconWidget()
		{
			this.DrawBlockEnvironmentData = new DrawBlockEnvironmentData();
			this.Size = new Vector2(float.PositiveInfinity);
			this.IsHitTestVisible = false;
			this.Light = 15;
			this.Depth = 1f;
			this.Color = Color.White;
			this.Scale = 1f;
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x000CC4E8 File Offset: 0x000CA6E8
		public override void Draw(Widget.DrawContext dc)
		{
			Block block = BlocksManager.Blocks[this.Contents];
			if (this.DrawBlockEnvironmentData.SubsystemTerrain == null)
			{
				Texture2D defaultBlocksTexture = BlocksTexturesManager.DefaultBlocksTexture;
			}
			else
			{
				Texture2D animatedBlocksTexture = this.DrawBlockEnvironmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture;
			}
			Viewport viewport = Display.Viewport;
			float num = MathUtils.Min(base.ActualSize.X, base.ActualSize.Y) * this.Scale;
			Matrix m = Matrix.CreateOrthographic(3.6f, 3.6f, -10f - 1f * this.Depth, 10f - 1f * this.Depth);
			Matrix m2 = MatrixUtils.CreateScaleTranslation(num, 0f - num, base.ActualSize.X / 2f, base.ActualSize.Y / 2f) * base.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)viewport.Width, -2f / (float)viewport.Height, -1f, 1f);
			this.DrawBlockEnvironmentData.ViewProjectionMatrix = new Matrix?(((this.CustomViewMatrix != null) ? this.CustomViewMatrix.Value : this.m_viewMatrix) * m * m2);
			float iconViewScale = BlocksManager.Blocks[this.Contents].GetIconViewScale(this.Value, this.DrawBlockEnvironmentData);
			Matrix matrix = (this.CustomViewMatrix != null) ? Matrix.Identity : Matrix.CreateTranslation(BlocksManager.Blocks[this.Contents].GetIconBlockOffset(this.Value, this.DrawBlockEnvironmentData));
			block.DrawBlock(dc.PrimitivesRenderer3D, this.Value, base.GlobalColorTransform, iconViewScale, ref matrix, this.DrawBlockEnvironmentData);
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x000CC6B2 File Offset: 0x000CA8B2
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.DesiredSize = this.Size;
		}

		// Token: 0x040011BC RID: 4540
		public Matrix m_viewMatrix;

		// Token: 0x040011BD RID: 4541
		public int m_value;
	}
}
