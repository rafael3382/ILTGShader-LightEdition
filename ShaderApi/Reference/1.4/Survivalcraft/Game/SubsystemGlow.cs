using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001AC RID: 428
	public class SubsystemGlow : Subsystem, IDrawable
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000B07 RID: 2823 RVA: 0x0004A36C File Offset: 0x0004856C
		public int[] DrawOrders
		{
			get
			{
				return SubsystemGlow.m_drawOrders;
			}
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x0004A374 File Offset: 0x00048574
		public GlowPoint AddGlowPoint()
		{
			GlowPoint glowPoint = new GlowPoint();
			this.m_glowPoints.Add(glowPoint, true);
			return glowPoint;
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x0004A395 File Offset: 0x00048595
		public void RemoveGlowPoint(GlowPoint glowPoint)
		{
			this.m_glowPoints.Remove(glowPoint);
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x0004A3A4 File Offset: 0x000485A4
		public void Draw(Camera camera, int drawOrder)
		{
			foreach (GlowPoint glowPoint in this.m_glowPoints.Keys)
			{
				if (glowPoint.Color.A > 0)
				{
					Vector3 vector = glowPoint.Position - camera.ViewPosition;
					float num = Vector3.Dot(vector, camera.ViewDirection);
					if (num > 0.01f)
					{
						float num2 = vector.Length();
						if (num2 < this.m_subsystemSky.ViewFogRange.Y)
						{
							float num3 = glowPoint.Size;
							if (glowPoint.FarDistance > 0f)
							{
								num3 += (glowPoint.FarSize - glowPoint.Size) * MathUtils.Saturate(num2 / glowPoint.FarDistance);
							}
							Vector3 v = (0f - (0.01f + 0.02f * num)) / num2 * vector;
							Vector3 p = glowPoint.Position + num3 * (-glowPoint.Right - glowPoint.Up) + v;
							Vector3 p2 = glowPoint.Position + num3 * (glowPoint.Right - glowPoint.Up) + v;
							Vector3 p3 = glowPoint.Position + num3 * (glowPoint.Right + glowPoint.Up) + v;
							Vector3 p4 = glowPoint.Position + num3 * (-glowPoint.Right + glowPoint.Up) + v;
							this.m_batchesByType[(int)glowPoint.Type].QueueQuad(p, p2, p3, p4, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), glowPoint.Color);
						}
					}
				}
			}
			this.m_primitivesRenderer.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x0004A5E4 File Offset: 0x000487E4
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_batchesByType[0] = this.m_primitivesRenderer.TexturedBatch(ContentManager.Get<Texture2D>("Textures/RoundGlow", null), false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, BlendState.AlphaBlend, SamplerState.LinearClamp);
			this.m_batchesByType[1] = this.m_primitivesRenderer.TexturedBatch(ContentManager.Get<Texture2D>("Textures/SquareGlow", null), false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, BlendState.AlphaBlend, SamplerState.LinearClamp);
			this.m_batchesByType[2] = this.m_primitivesRenderer.TexturedBatch(ContentManager.Get<Texture2D>("Textures/HorizontalRectGlow", null), false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, BlendState.AlphaBlend, SamplerState.LinearClamp);
			this.m_batchesByType[3] = this.m_primitivesRenderer.TexturedBatch(ContentManager.Get<Texture2D>("Textures/VerticalRectGlow", null), false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, BlendState.AlphaBlend, SamplerState.LinearClamp);
		}

		// Token: 0x0400055F RID: 1375
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000560 RID: 1376
		public Dictionary<GlowPoint, bool> m_glowPoints = new Dictionary<GlowPoint, bool>();

		// Token: 0x04000561 RID: 1377
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x04000562 RID: 1378
		public TexturedBatch3D[] m_batchesByType = new TexturedBatch3D[4];

		// Token: 0x04000563 RID: 1379
		public static int[] m_drawOrders = new int[]
		{
			110
		};
	}
}
