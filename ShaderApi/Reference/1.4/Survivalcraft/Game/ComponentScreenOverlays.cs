using System;
using Engine;
using Engine.Graphics;
using Engine.Media;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200022B RID: 555
	public class ComponentScreenOverlays : Component, IDrawable, IUpdateable
	{
		// Token: 0x1700028B RID: 651
		// (get) Token: 0x060011C4 RID: 4548 RVA: 0x00083F37 File Offset: 0x00082137
		// (set) Token: 0x060011C5 RID: 4549 RVA: 0x00083F3F File Offset: 0x0008213F
		public float BlackoutFactor { get; set; }

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x060011C6 RID: 4550 RVA: 0x00083F48 File Offset: 0x00082148
		// (set) Token: 0x060011C7 RID: 4551 RVA: 0x00083F50 File Offset: 0x00082150
		public float RedoutFactor { get; set; }

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x060011C8 RID: 4552 RVA: 0x00083F59 File Offset: 0x00082159
		// (set) Token: 0x060011C9 RID: 4553 RVA: 0x00083F61 File Offset: 0x00082161
		public float GreenoutFactor { get; set; }

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x060011CA RID: 4554 RVA: 0x00083F6A File Offset: 0x0008216A
		// (set) Token: 0x060011CB RID: 4555 RVA: 0x00083F72 File Offset: 0x00082172
		public string FloatingMessage { get; set; }

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x060011CC RID: 4556 RVA: 0x00083F7B File Offset: 0x0008217B
		// (set) Token: 0x060011CD RID: 4557 RVA: 0x00083F83 File Offset: 0x00082183
		public float FloatingMessageFactor { get; set; }

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x060011CE RID: 4558 RVA: 0x00083F8C File Offset: 0x0008218C
		// (set) Token: 0x060011CF RID: 4559 RVA: 0x00083F94 File Offset: 0x00082194
		public string Message { get; set; }

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x060011D0 RID: 4560 RVA: 0x00083F9D File Offset: 0x0008219D
		// (set) Token: 0x060011D1 RID: 4561 RVA: 0x00083FA5 File Offset: 0x000821A5
		public float MessageFactor { get; set; }

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x060011D2 RID: 4562 RVA: 0x00083FAE File Offset: 0x000821AE
		// (set) Token: 0x060011D3 RID: 4563 RVA: 0x00083FB6 File Offset: 0x000821B6
		public float IceFactor { get; set; }

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x060011D4 RID: 4564 RVA: 0x00083FBF File Offset: 0x000821BF
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Reset;
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x060011D5 RID: 4565 RVA: 0x00083FC3 File Offset: 0x000821C3
		public int[] DrawOrders
		{
			get
			{
				return ComponentScreenOverlays.m_drawOrders;
			}
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x00083FCC File Offset: 0x000821CC
		public void Update(float dt)
		{
			bool flag = this.m_subsystemSky.ViewUnderWaterDepth > 0f;
			if (flag != this.m_isUnderWater)
			{
				this.m_isUnderWater = flag;
				this.m_waterSurfaceCrossTime = new double?(this.m_subsystemTime.GameTime);
			}
			this.BlackoutFactor = 0f;
			this.RedoutFactor = 0f;
			this.GreenoutFactor = 0f;
			this.IceFactor = 0f;
			this.FloatingMessage = null;
			this.FloatingMessageFactor = 0f;
			this.Message = null;
			this.MessageFactor = 0f;
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x00084064 File Offset: 0x00082264
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.m_componentPlayer.GameWidget != camera.GameWidget)
			{
				return;
			}
			if (this.m_waterSurfaceCrossTime != null)
			{
				float num = (float)(this.m_subsystemTime.GameTime - this.m_waterSurfaceCrossTime.Value);
				float num2 = 0.66f * MathUtils.Sqr(MathUtils.Saturate(1f - 0.75f * num));
				if (num2 > 0.01f)
				{
					Matrix matrix = default(Matrix);
					matrix.Translation = Vector3.Zero;
					matrix.Forward = camera.ViewDirection;
					matrix.Right = Vector3.Normalize(Vector3.Cross(camera.ViewUp, matrix.Forward));
					matrix.Up = Vector3.Normalize(Vector3.Cross(matrix.Right, matrix.Forward));
					Vector3 vector = matrix.ToYawPitchRoll();
					Vector2 zero = Vector2.Zero;
					zero.X -= 2f * vector.X / 3.14159274f + 0.05f * MathUtils.Sin(5f * num);
					zero.Y += 2f * vector.Y / 3.14159274f + (this.m_isUnderWater ? (0.75f * num) : (-0.75f * num));
					Texture2D texture = ContentManager.Get<Texture2D>("Textures/SplashOverlay", null);
					this.DrawTexturedOverlay(camera, texture, new Color(156, 206, 210), num2, num2, zero);
				}
			}
			if (this.IceFactor > 0f)
			{
				this.DrawIceOverlay(camera, this.IceFactor);
			}
			if (this.RedoutFactor > 0.01f)
			{
				this.DrawOverlay(camera, new Color(255, 64, 0), MathUtils.Saturate(2f * (this.RedoutFactor - 0.5f)), this.RedoutFactor);
			}
			if (this.BlackoutFactor > 0.01f)
			{
				this.DrawOverlay(camera, Color.Black, MathUtils.Saturate(2f * (this.BlackoutFactor - 0.5f)), this.BlackoutFactor);
			}
			if (this.GreenoutFactor > 0.01f)
			{
				this.DrawOverlay(camera, new Color(166, 175, 103), this.GreenoutFactor, MathUtils.Saturate(2f * this.GreenoutFactor));
			}
			if (!string.IsNullOrEmpty(this.FloatingMessage) && this.FloatingMessageFactor > 0.01f)
			{
				this.DrawFloatingMessage(camera, this.FloatingMessage, this.FloatingMessageFactor);
			}
			if (!string.IsNullOrEmpty(this.Message) && this.MessageFactor > 0.01f)
			{
				this.DrawMessage(camera, this.Message, this.MessageFactor);
			}
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x000842FC File Offset: 0x000824FC
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentGui = base.Entity.FindComponent<ComponentGui>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x00084364 File Offset: 0x00082564
		public virtual void DrawOverlay(Camera camera, Color color, float innerFactor, float outerFactor)
		{
			Vector2 viewportSize = camera.ViewportSize;
			Vector2 vector = new Vector2(0f, 0f);
			Vector2 vector2 = new Vector2(viewportSize.X, 0f);
			Vector2 vector3 = new Vector2(viewportSize.X, viewportSize.Y);
			Vector2 vector4 = new Vector2(0f, viewportSize.Y);
			Vector2 p = new Vector2(viewportSize.X / 2f, viewportSize.Y / 2f);
			Color color2 = color * outerFactor;
			Color color3 = color * innerFactor;
			FlatBatch2D flatBatch2D = this.m_primitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, BlendState.AlphaBlend);
			int count = flatBatch2D.TriangleVertices.Count;
			flatBatch2D.QueueTriangle(vector, vector2, p, 0f, color2, color2, color3);
			flatBatch2D.QueueTriangle(vector2, vector3, p, 0f, color2, color2, color3);
			flatBatch2D.QueueTriangle(vector3, vector4, p, 0f, color2, color2, color3);
			flatBatch2D.QueueTriangle(vector4, vector, p, 0f, color2, color2, color3);
			flatBatch2D.TransformTriangles(camera.ViewportMatrix, count, -1);
			flatBatch2D.Flush(true);
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x00084484 File Offset: 0x00082684
		public virtual void DrawTexturedOverlay(Camera camera, Texture2D texture, Color color, float innerFactor, float outerFactor, Vector2 offset)
		{
			Vector2 viewportSize = camera.ViewportSize;
			float num = viewportSize.X / viewportSize.Y;
			Vector2 vector = new Vector2(0f, 0f);
			Vector2 vector2 = new Vector2(viewportSize.X, 0f);
			Vector2 vector3 = new Vector2(viewportSize.X, viewportSize.Y);
			Vector2 vector4 = new Vector2(0f, viewportSize.Y);
			Vector2 p = new Vector2(viewportSize.X / 2f, viewportSize.Y / 2f);
			offset.X = MathUtils.Remainder(offset.X, 1f);
			offset.Y = MathUtils.Remainder(offset.Y, 1f);
			Vector2 vector5 = new Vector2(0f, 0f) + offset;
			Vector2 vector6 = new Vector2(num, 0f) + offset;
			Vector2 vector7 = new Vector2(num, 1f) + offset;
			Vector2 vector8 = new Vector2(0f, 1f) + offset;
			Vector2 texCoord = new Vector2(num / 2f, 0.5f) + offset;
			Color color2 = color * outerFactor;
			Color color3 = color * innerFactor;
			TexturedBatch2D texturedBatch2D = this.m_primitivesRenderer2D.TexturedBatch(texture, false, 0, DepthStencilState.None, null, BlendState.Additive, SamplerState.PointWrap);
			int count = texturedBatch2D.TriangleVertices.Count;
			texturedBatch2D.QueueTriangle(vector, vector2, p, 0f, vector5, vector6, texCoord, color2, color2, color3);
			texturedBatch2D.QueueTriangle(vector2, vector3, p, 0f, vector6, vector7, texCoord, color2, color2, color3);
			texturedBatch2D.QueueTriangle(vector3, vector4, p, 0f, vector7, vector8, texCoord, color2, color2, color3);
			texturedBatch2D.QueueTriangle(vector4, vector, p, 0f, vector8, vector5, texCoord, color2, color2, color3);
			texturedBatch2D.TransformTriangles(camera.ViewportMatrix, count, -1);
			texturedBatch2D.Flush(true);
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x00084670 File Offset: 0x00082870
		public virtual void DrawIceOverlay(Camera camera, float factor)
		{
			Vector2 viewportSize = camera.ViewportSize;
			float s = 1f;
			Vector2 one = Vector2.One;
			float num = one.Length();
			Point2 point = new Point2((int)MathUtils.Round(12f * viewportSize.X / viewportSize.Y), (int)MathUtils.Round(12f));
			if (this.m_iceVertices == null || this.m_cellsCount != point)
			{
				this.m_cellsCount = point;
				this.m_random.Seed(0);
				this.m_iceVertices = new Vector2[(point.X + 1) * (point.Y + 1)];
				for (int i = 0; i <= point.X; i++)
				{
					for (int j = 0; j <= point.Y; j++)
					{
						float num2 = (float)i;
						float num3 = (float)j;
						if (i != 0 && i != point.X)
						{
							num2 += this.m_random.Float(-0.4f, 0.4f);
						}
						if (j != 0 && j != point.Y)
						{
							num3 += this.m_random.Float(-0.4f, 0.4f);
						}
						float x = num2 / (float)point.X;
						float y = num3 / (float)point.Y;
						this.m_iceVertices[i + j * (point.X + 1)] = new Vector2(x, y);
					}
				}
			}
			Vector3 vector = Vector3.UnitX / camera.ProjectionMatrix.M11 * 2f * 0.2f * s;
			Vector3 vector2 = Vector3.UnitY / camera.ProjectionMatrix.M22 * 2f * 0.2f * s;
			Vector3 v = -0.2f * Vector3.UnitZ - 0.5f * (vector + vector2);
			if (this.m_light == null || Time.PeriodicEvent(0.05000000074505806, 0.0))
			{
				this.m_light = new float?(LightingManager.CalculateSmoothLight(this.m_subsystemTerrain, camera.ViewPosition) ?? (this.m_light ?? 1f));
			}
			Color color = Color.MultiplyColorOnly(Color.White, this.m_light.Value);
			this.m_random.Seed(0);
			Texture2D texture = ContentManager.Get<Texture2D>("Textures/IceOverlay", null);
			TexturedBatch3D texturedBatch3D = this.m_primitivesRenderer3D.TexturedBatch(texture, false, 0, DepthStencilState.None, RasterizerState.CullNoneScissor, BlendState.AlphaBlend, SamplerState.PointWrap);
			Vector2 v2 = new Vector2(viewportSize.X / viewportSize.Y, 1f);
			Vector2 vector3 = new Vector2((float)(point.X - 1), (float)(point.Y - 1));
			for (int k = 0; k < point.X; k++)
			{
				for (int l = 0; l < point.Y; l++)
				{
					float num4 = (new Vector2((float)(2 * k) / vector3.X - 1f, (float)(2 * l) / vector3.Y - 1f) * one).Length() / num;
					if (1f - num4 + this.m_random.Float(0f, 0.05f) < factor)
					{
						Vector2 vector4 = this.m_iceVertices[k + l * (point.X + 1)];
						Vector2 vector5 = this.m_iceVertices[k + 1 + l * (point.X + 1)];
						Vector2 vector6 = this.m_iceVertices[k + 1 + (l + 1) * (point.X + 1)];
						Vector2 vector7 = this.m_iceVertices[k + (l + 1) * (point.X + 1)];
						Vector3 vector8 = v + vector4.X * vector + vector4.Y * vector2;
						Vector3 p = v + vector5.X * vector + vector5.Y * vector2;
						Vector3 vector9 = v + vector6.X * vector + vector6.Y * vector2;
						Vector3 p2 = v + vector7.X * vector + vector7.Y * vector2;
						Vector2 vector10 = vector4 * v2;
						Vector2 texCoord = vector5 * v2;
						Vector2 vector11 = vector6 * v2;
						Vector2 texCoord2 = vector7 * v2;
						texturedBatch3D.QueueTriangle(vector8, p, vector9, vector10, texCoord, vector11, color);
						texturedBatch3D.QueueTriangle(vector9, p2, vector8, vector11, texCoord2, vector10, color);
					}
				}
			}
			texturedBatch3D.Flush(camera.ProjectionMatrix, true);
		}

		// Token: 0x060011DC RID: 4572 RVA: 0x00084B8C File Offset: 0x00082D8C
		public virtual void DrawFloatingMessage(Camera camera, string message, float factor)
		{
			BitmapFont bitmapFont = LabelWidget.BitmapFont;
			Vector2 position = camera.ViewportSize / 2f;
			position.X += 0.07f * camera.ViewportSize.X * (float)MathUtils.Sin(1.7300000190734863 * Time.FrameStartTime);
			position.Y += 0.07f * camera.ViewportSize.Y * (float)MathUtils.Cos(1.1200000047683716 * Time.FrameStartTime);
			FontBatch2D fontBatch2D = this.m_primitivesRenderer2D.FontBatch(bitmapFont, 1, DepthStencilState.None, null, BlendState.AlphaBlend, null);
			int count = fontBatch2D.TriangleVertices.Count;
			fontBatch2D.QueueText(message, position, 0f, Color.White * factor, TextAnchor.HorizontalCenter | TextAnchor.VerticalCenter, Vector2.One * camera.GameWidget.GlobalScale, Vector2.Zero, 0f);
			fontBatch2D.TransformTriangles(camera.ViewportMatrix, count, -1);
			fontBatch2D.Flush(true);
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x00084C88 File Offset: 0x00082E88
		public virtual void DrawMessage(Camera camera, string message, float factor)
		{
			BitmapFont bitmapFont = LabelWidget.BitmapFont;
			Vector2 position = new Vector2(camera.ViewportSize.X / 2f, camera.ViewportSize.Y - 25f);
			FontBatch2D fontBatch2D = this.m_primitivesRenderer2D.FontBatch(bitmapFont, 0, DepthStencilState.None, null, BlendState.AlphaBlend, null);
			int count = fontBatch2D.TriangleVertices.Count;
			fontBatch2D.QueueText(message, position, 0f, Color.Gray * factor, TextAnchor.HorizontalCenter | TextAnchor.Bottom, Vector2.One * camera.GameWidget.GlobalScale, Vector2.Zero, 0f);
			fontBatch2D.TransformTriangles(camera.ViewportMatrix, count, -1);
			fontBatch2D.Flush(true);
		}

		// Token: 0x04000ABC RID: 2748
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000ABD RID: 2749
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000ABE RID: 2750
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000ABF RID: 2751
		public ComponentGui m_componentGui;

		// Token: 0x04000AC0 RID: 2752
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000AC1 RID: 2753
		public PrimitivesRenderer2D m_primitivesRenderer2D = new PrimitivesRenderer2D();

		// Token: 0x04000AC2 RID: 2754
		public PrimitivesRenderer3D m_primitivesRenderer3D = new PrimitivesRenderer3D();

		// Token: 0x04000AC3 RID: 2755
		public Game.Random m_random = new Game.Random(0);

		// Token: 0x04000AC4 RID: 2756
		public Vector2[] m_iceVertices;

		// Token: 0x04000AC5 RID: 2757
		public Point2 m_cellsCount;

		// Token: 0x04000AC6 RID: 2758
		public float? m_light;

		// Token: 0x04000AC7 RID: 2759
		public double? m_waterSurfaceCrossTime;

		// Token: 0x04000AC8 RID: 2760
		public bool m_isUnderWater;

		// Token: 0x04000AC9 RID: 2761
		public static int[] m_drawOrders = new int[]
		{
			1101
		};
	}
}
