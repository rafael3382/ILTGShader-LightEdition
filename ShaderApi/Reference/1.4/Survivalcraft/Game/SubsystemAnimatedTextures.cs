using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000182 RID: 386
	public class SubsystemAnimatedTextures : Subsystem, IUpdateable
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060008C2 RID: 2242 RVA: 0x00037084 File Offset: 0x00035284
		public Texture2D AnimatedBlocksTexture
		{
			get
			{
				if (this.DisableTextureAnimation || this.m_animatedBlocksTexture == null)
				{
					return this.m_subsystemBlocksTexture.BlocksTexture;
				}
				return this.m_animatedBlocksTexture;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060008C3 RID: 2243 RVA: 0x000370A8 File Offset: 0x000352A8
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x000370AC File Offset: 0x000352AC
		public void Update(float dt)
		{
			if (!this.DisableTextureAnimation && this.m_subsystemTime.FixedTimeStep == null)
			{
				float dt2 = (float)MathUtils.Min(this.m_subsystemTime.GameTime - this.m_lastAnimateGameTime, 1.0);
				this.m_lastAnimateGameTime = this.m_subsystemTime.GameTime;
				Texture2D blocksTexture = this.m_subsystemBlocksTexture.BlocksTexture;
				if (this.m_animatedBlocksTexture == null || this.m_animatedBlocksTexture.Width != blocksTexture.Width || this.m_animatedBlocksTexture.Height != blocksTexture.Height || this.m_animatedBlocksTexture.MipLevelsCount > 1 != SettingsManager.TerrainMipmapsEnabled)
				{
					Utilities.Dispose<RenderTarget2D>(ref this.m_animatedBlocksTexture);
					this.m_animatedBlocksTexture = new RenderTarget2D(blocksTexture.Width, blocksTexture.Height, (!SettingsManager.TerrainMipmapsEnabled) ? 1 : 4, ColorFormat.Rgba8888, DepthFormat.None);
				}
				Rectangle scissorRectangle = Display.ScissorRectangle;
				RenderTarget2D renderTarget = Display.RenderTarget;
				Display.RenderTarget = this.m_animatedBlocksTexture;
				try
				{
					Display.Clear(new Vector4?(new Vector4(Color.Transparent)), null, null);
					this.m_primitivesRenderer.TexturedBatch(blocksTexture, false, -1, DepthStencilState.None, RasterizerState.CullNone, BlendState.Opaque, SamplerState.PointClamp).QueueQuad(new Vector2(0f, 0f), new Vector2((float)this.m_animatedBlocksTexture.Width, (float)this.m_animatedBlocksTexture.Height), 0f, Vector2.Zero, Vector2.One, Color.White);
					this.AnimateWaterBlocksTexture();
					this.AnimateMagmaBlocksTexture();
					this.m_primitivesRenderer.Flush(true, int.MaxValue);
					Display.ScissorRectangle = this.AnimateFireBlocksTexture(dt2);
					this.m_primitivesRenderer.Flush(true, int.MaxValue);
				}
				finally
				{
					Display.RenderTarget = renderTarget;
					Display.ScissorRectangle = scissorRectangle;
				}
				if (SettingsManager.TerrainMipmapsEnabled && Time.FrameIndex % 2 == 0)
				{
					this.m_animatedBlocksTexture.GenerateMipMaps();
				}
			}
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x000372A8 File Offset: 0x000354A8
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemBlocksTexture = base.Project.FindSubsystem<SubsystemBlocksTexture>(true);
			Display.DeviceReset += this.Display_DeviceReset;
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x000372DF File Offset: 0x000354DF
		public override void Dispose()
		{
			Utilities.Dispose<RenderTarget2D>(ref this.m_animatedBlocksTexture);
			Display.DeviceReset -= this.Display_DeviceReset;
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x000372FD File Offset: 0x000354FD
		public void Display_DeviceReset()
		{
			this.m_animatedBlocksTexture = null;
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x00037308 File Offset: 0x00035508
		public void AnimateWaterBlocksTexture()
		{
			TexturedBatch2D batch = this.m_primitivesRenderer.TexturedBatch(this.m_subsystemBlocksTexture.BlocksTexture, false, 0, DepthStencilState.None, null, BlendState.AlphaBlend, SamplerState.PointClamp);
			int num = BlocksManager.Blocks[18].DefaultTextureSlot % 16;
			int num2 = BlocksManager.Blocks[18].DefaultTextureSlot / 16;
			double num3 = 1.0 * this.m_subsystemTime.GameTime;
			double num4 = 1.0 * (this.m_subsystemTime.GameTime - (double)this.m_subsystemTime.GameTimeDelta);
			float num5 = MathUtils.Min((float)MathUtils.Remainder(num3, 2.0), 1f);
			float num6 = MathUtils.Min((float)MathUtils.Remainder(num3 + 1.0, 2.0), 1f);
			byte b = (byte)(255f * num5);
			byte b2 = (byte)(255f * num6);
			if (MathUtils.Remainder(num3, 2.0) >= 1.0 && MathUtils.Remainder(num4, 2.0) < 1.0)
			{
				this.m_waterOrder = true;
				this.m_waterOffset2 = new Vector2(this.m_random.Float(0f, 1f), this.m_random.Float(0f, 1f));
			}
			else if (MathUtils.Remainder(num3 + 1.0, 2.0) >= 1.0 && MathUtils.Remainder(num4 + 1.0, 2.0) < 1.0)
			{
				this.m_waterOrder = false;
				this.m_waterOffset1 = new Vector2(this.m_random.Float(0f, 1f), this.m_random.Float(0f, 1f));
			}
			Vector2 tcOffset = new Vector2((float)num, (float)num2) - (this.m_waterOrder ? this.m_waterOffset1 : this.m_waterOffset2);
			Vector2 tcOffset2 = new Vector2((float)num, (float)num2) - (this.m_waterOrder ? this.m_waterOffset2 : this.m_waterOffset1);
			Color color = this.m_waterOrder ? new Color(b, b, b, b) : new Color(b2, b2, b2, b2);
			Color color2 = this.m_waterOrder ? new Color(b2, b2, b2, b2) : new Color(b, b, b, b);
			float num7 = MathUtils.Floor((float)MathUtils.Remainder(1.75 * this.m_subsystemTime.GameTime, 1.0) * 16f) / 16f;
			float num8 = 0f - num7 + 1f;
			float num9 = MathUtils.Floor((float)MathUtils.Remainder((double)(1.75f / MathUtils.Sqrt(2f)) * this.m_subsystemTime.GameTime, 1.0) * 16f) / 16f;
			float num10 = 0f - num9 + 1f;
			Vector2 tc = new Vector2(0f, 0f);
			Vector2 tc2 = new Vector2(1f, 1f);
			this.DrawBlocksTextureSlot(batch, num, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num7, 0f);
			tc2 = new Vector2(num7 + 1f, 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num8, 0f);
			tc2 = new Vector2(num8 + 1f, 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(0f, num7);
			tc2 = new Vector2(1f, num7 + 1f);
			this.DrawBlocksTextureSlot(batch, num, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2 - 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(0f, num8);
			tc2 = new Vector2(1f, num8 + 1f);
			this.DrawBlocksTextureSlot(batch, num, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num9, num10);
			tc2 = new Vector2(num9 + 1f, num10 + 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num10, num10);
			tc2 = new Vector2(num10 + 1f, num10 + 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num9, num9);
			tc2 = new Vector2(num9 + 1f, num9 + 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 - 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num10, num9);
			tc2 = new Vector2(num10 + 1f, num9 + 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 - 1, tc, tc2, tcOffset2, color2);
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x000378B0 File Offset: 0x00035AB0
		public void AnimateMagmaBlocksTexture()
		{
			TexturedBatch2D batch = this.m_primitivesRenderer.TexturedBatch(this.m_subsystemBlocksTexture.BlocksTexture, false, 0, DepthStencilState.None, null, BlendState.AlphaBlend, SamplerState.PointClamp);
			int num = BlocksManager.Blocks[92].DefaultTextureSlot % 16;
			int num2 = BlocksManager.Blocks[92].DefaultTextureSlot / 16;
			double num3 = 0.5 * this.m_subsystemTime.GameTime;
			double num4 = 0.5 * (this.m_subsystemTime.GameTime - (double)this.m_subsystemTime.GameTimeDelta);
			float num5 = MathUtils.Min((float)MathUtils.Remainder(num3, 2.0), 1f);
			float num6 = MathUtils.Min((float)MathUtils.Remainder(num3 + 1.0, 2.0), 1f);
			byte b = (byte)(255f * num5);
			byte b2 = (byte)(255f * num6);
			if (MathUtils.Remainder(num3, 2.0) >= 1.0 && MathUtils.Remainder(num4, 2.0) < 1.0)
			{
				this.m_magmaOrder = true;
				this.m_magmaOffset2 = new Vector2(this.m_random.Float(0f, 1f), this.m_random.Float(0f, 1f));
			}
			else if (MathUtils.Remainder(num3 + 1.0, 2.0) >= 1.0 && MathUtils.Remainder(num4 + 1.0, 2.0) < 1.0)
			{
				this.m_magmaOrder = false;
				this.m_magmaOffset1 = new Vector2(this.m_random.Float(0f, 1f), this.m_random.Float(0f, 1f));
			}
			Vector2 tcOffset = new Vector2((float)num, (float)num2) - (this.m_magmaOrder ? this.m_magmaOffset1 : this.m_magmaOffset2);
			Vector2 tcOffset2 = new Vector2((float)num, (float)num2) - (this.m_magmaOrder ? this.m_magmaOffset2 : this.m_magmaOffset1);
			Color color = this.m_magmaOrder ? new Color(b, b, b, b) : new Color(b2, b2, b2, b2);
			Color color2 = this.m_magmaOrder ? new Color(b2, b2, b2, b2) : new Color(b, b, b, b);
			float num7 = MathUtils.Floor((float)MathUtils.Remainder(0.40000000596046448 * this.m_subsystemTime.GameTime, 1.0) * 16f) / 16f;
			float num8 = 0f - num7 + 1f;
			float num9 = MathUtils.Floor((float)MathUtils.Remainder((double)(0.4f / MathUtils.Sqrt(2f)) * this.m_subsystemTime.GameTime, 1.0) * 16f) / 16f;
			float num10 = 0f - num9 + 1f;
			Vector2 tc = new Vector2(0f, 0f);
			Vector2 tc2 = new Vector2(1f, 1f);
			this.DrawBlocksTextureSlot(batch, num, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num7, 0f);
			tc2 = new Vector2(num7 + 1f, 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num8, 0f);
			tc2 = new Vector2(num8 + 1f, 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(0f, num7);
			tc2 = new Vector2(1f, num7 + 1f);
			this.DrawBlocksTextureSlot(batch, num, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2 - 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(0f, num8);
			tc2 = new Vector2(1f, num8 + 1f);
			this.DrawBlocksTextureSlot(batch, num, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num9, num10);
			tc2 = new Vector2(num9 + 1f, num10 + 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num10, num10);
			tc2 = new Vector2(num10 + 1f, num10 + 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num9, num9);
			tc2 = new Vector2(num9 + 1f, num9 + 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 - 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num10, num9);
			tc2 = new Vector2(num10 + 1f, num9 + 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 - 1, tc, tc2, tcOffset2, color2);
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x00037E58 File Offset: 0x00036058
		public Rectangle AnimateFireBlocksTexture(float dt)
		{
			int defaultTextureSlot = BlocksManager.Blocks[104].DefaultTextureSlot;
			float num = (float)(this.m_animatedBlocksTexture.Width / 16);
			int num2 = defaultTextureSlot % 16;
			int num3 = defaultTextureSlot / 16;
			this.m_screenSpaceFireRenderer.ParticleSize = 1f * num;
			this.m_screenSpaceFireRenderer.ParticleSpeed = 1.9f * num;
			this.m_screenSpaceFireRenderer.ParticlesPerSecond = 24f;
			this.m_screenSpaceFireRenderer.MinTimeToLive = float.PositiveInfinity;
			this.m_screenSpaceFireRenderer.MaxTimeToLive = float.PositiveInfinity;
			this.m_screenSpaceFireRenderer.ParticleAnimationOffset = 1f;
			this.m_screenSpaceFireRenderer.ParticleAnimationPeriod = 3f;
			this.m_screenSpaceFireRenderer.Origin = new Vector2((float)num2, (float)(num3 + 3)) * num + new Vector2(0f, 0.5f * this.m_screenSpaceFireRenderer.ParticleSize);
			this.m_screenSpaceFireRenderer.Width = num;
			this.m_screenSpaceFireRenderer.CutoffPosition = (float)num3 * num;
			this.m_screenSpaceFireRenderer.Update(dt);
			this.m_screenSpaceFireRenderer.Draw(this.m_primitivesRenderer, 0f, Matrix.Identity, Color.White);
			return new Rectangle((int)((float)num2 * num), (int)((float)num3 * num), (int)num, (int)(num * 3f));
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x00037F9C File Offset: 0x0003619C
		public void DrawBlocksTextureSlot(TexturedBatch2D batch, int slotX, int slotY, Vector2 tc1, Vector2 tc2, Vector2 tcOffset, Color color)
		{
			float s = (float)this.m_animatedBlocksTexture.Width / 16f;
			batch.QueueQuad(new Vector2((float)slotX, (float)slotY) * s, new Vector2((float)(slotX + 1), (float)(slotY + 1)) * s, 0f, (tc1 + tcOffset) / 16f, (tc2 + tcOffset) / 16f, color);
		}

		// Token: 0x04000468 RID: 1128
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000469 RID: 1129
		public SubsystemBlocksTexture m_subsystemBlocksTexture;

		// Token: 0x0400046A RID: 1130
		public RenderTarget2D m_animatedBlocksTexture;

		// Token: 0x0400046B RID: 1131
		public PrimitivesRenderer2D m_primitivesRenderer = new PrimitivesRenderer2D();

		// Token: 0x0400046C RID: 1132
		public ScreenSpaceFireRenderer m_screenSpaceFireRenderer = new ScreenSpaceFireRenderer(200);

		// Token: 0x0400046D RID: 1133
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400046E RID: 1134
		public bool m_waterOrder;

		// Token: 0x0400046F RID: 1135
		public Vector2 m_waterOffset1;

		// Token: 0x04000470 RID: 1136
		public Vector2 m_waterOffset2;

		// Token: 0x04000471 RID: 1137
		public bool m_magmaOrder;

		// Token: 0x04000472 RID: 1138
		public Vector2 m_magmaOffset1;

		// Token: 0x04000473 RID: 1139
		public Vector2 m_magmaOffset2;

		// Token: 0x04000474 RID: 1140
		public double m_lastAnimateGameTime;

		// Token: 0x04000475 RID: 1141
		public bool DisableTextureAnimation;

		// Token: 0x04000476 RID: 1142
		public bool ShowAnimatedTexture;
	}
}
