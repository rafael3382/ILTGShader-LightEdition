using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001CF RID: 463
	public class SubsystemShadows : Subsystem, IDrawable
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000C41 RID: 3137 RVA: 0x00056A6E File Offset: 0x00054C6E
		public int[] DrawOrders
		{
			get
			{
				return SubsystemShadows.m_drawOrders;
			}
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x00056A78 File Offset: 0x00054C78
		public void QueueShadow(Camera camera, Vector3 shadowPosition, float shadowDiameter, float alpha)
		{
			if (!SettingsManager.ObjectsShadowsEnabled)
			{
				return;
			}
			float num = Vector3.DistanceSquared(camera.ViewPosition, shadowPosition);
			if (num > 1024f)
			{
				return;
			}
			float num2 = MathUtils.Sqrt(num);
			float num3 = MathUtils.Saturate(4f * (1f - num2 / 32f));
			float num4 = shadowDiameter / 2f;
			int num5 = Terrain.ToCell(shadowPosition.X - num4);
			int num6 = Terrain.ToCell(shadowPosition.Z - num4);
			int num7 = Terrain.ToCell(shadowPosition.X + num4);
			int num8 = Terrain.ToCell(shadowPosition.Z + num4);
			for (int i = num5; i <= num7; i++)
			{
				for (int j = num6; j <= num8; j++)
				{
					int num9 = MathUtils.Min(Terrain.ToCell(shadowPosition.Y), 255);
					int num10 = MathUtils.Max(num9 - 2, 0);
					for (int k = num9; k >= num10; k--)
					{
						int cellValueFast = this.m_subsystemTerrain.Terrain.GetCellValueFast(i, k, j);
						int num11 = Terrain.ExtractContents(cellValueFast);
						Block block = BlocksManager.Blocks[num11];
						if (block.GetObjectShadowStrength(cellValueFast) > 0f)
						{
							foreach (BoundingBox boundingBox in block.GetCustomCollisionBoxes(this.m_subsystemTerrain, cellValueFast))
							{
								float num12 = boundingBox.Max.Y + (float)k;
								if (shadowPosition.Y - num12 > -0.5f)
								{
									float num13 = camera.ViewPosition.Y - num12;
									if (num13 > 0f)
									{
										float num14 = MathUtils.Max(num13 * 0.01f, 0.005f);
										float num15 = MathUtils.Saturate(1f - (shadowPosition.Y - num12) / 2f);
										Vector3 p = new Vector3(boundingBox.Min.X + (float)i, num12 + num14, boundingBox.Min.Z + (float)j);
										Vector3 p2 = new Vector3(boundingBox.Max.X + (float)i, num12 + num14, boundingBox.Min.Z + (float)j);
										Vector3 p3 = new Vector3(boundingBox.Max.X + (float)i, num12 + num14, boundingBox.Max.Z + (float)j);
										Vector3 p4 = new Vector3(boundingBox.Min.X + (float)i, num12 + num14, boundingBox.Max.Z + (float)j);
										this.DrawShadowOverQuad(p, p2, p3, p4, shadowPosition, shadowDiameter, 0.45f * block.GetObjectShadowStrength(cellValueFast) * alpha * num3 * num15);
									}
								}
							}
							break;
						}
						if (num11 == 18)
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x00056D2C File Offset: 0x00054F2C
		public void Draw(Camera camera, int drawOrder)
		{
			this.m_primitivesRenderer.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x00056D48 File Offset: 0x00054F48
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_batch = this.m_primitivesRenderer.TexturedBatch(ContentManager.Get<Texture2D>("Textures/Shadow", null), false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, BlendState.AlphaBlend, SamplerState.LinearClamp);
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x00056D9C File Offset: 0x00054F9C
		public void DrawShadowOverQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 shadowPosition, float shadowDiameter, float alpha)
		{
			if (alpha > 0.02f)
			{
				Vector2 texCoord = SubsystemShadows.CalculateShadowTextureCoordinate(p1, shadowPosition, shadowDiameter);
				Vector2 texCoord2 = SubsystemShadows.CalculateShadowTextureCoordinate(p2, shadowPosition, shadowDiameter);
				Vector2 texCoord3 = SubsystemShadows.CalculateShadowTextureCoordinate(p3, shadowPosition, shadowDiameter);
				Vector2 texCoord4 = SubsystemShadows.CalculateShadowTextureCoordinate(p4, shadowPosition, shadowDiameter);
				this.m_batch.QueueQuad(p1, p2, p3, p4, texCoord, texCoord2, texCoord3, texCoord4, new Color(0f, 0f, 0f, alpha));
			}
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x00056E09 File Offset: 0x00055009
		public static Vector2 CalculateShadowTextureCoordinate(Vector3 p, Vector3 shadowPosition, float shadowDiameter)
		{
			return new Vector2(0.5f + (p.X - shadowPosition.X) / shadowDiameter, 0.5f + (p.Z - shadowPosition.Z) / shadowDiameter);
		}

		// Token: 0x0400061C RID: 1564
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400061D RID: 1565
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x0400061E RID: 1566
		public TexturedBatch3D m_batch;

		// Token: 0x0400061F RID: 1567
		public static int[] m_drawOrders = new int[]
		{
			200
		};
	}
}
