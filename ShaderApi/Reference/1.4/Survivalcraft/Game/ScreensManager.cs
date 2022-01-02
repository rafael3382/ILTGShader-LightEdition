using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000175 RID: 373
	public static class ScreensManager
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600085F RID: 2143 RVA: 0x00032D69 File Offset: 0x00030F69
		// (set) Token: 0x06000860 RID: 2144 RVA: 0x00032D70 File Offset: 0x00030F70
		public static ContainerWidget RootWidget { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000861 RID: 2145 RVA: 0x00032D78 File Offset: 0x00030F78
		public static bool IsAnimating
		{
			get
			{
				return ScreensManager.m_animationData != null;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000862 RID: 2146 RVA: 0x00032D82 File Offset: 0x00030F82
		// (set) Token: 0x06000863 RID: 2147 RVA: 0x00032D89 File Offset: 0x00030F89
		public static Screen CurrentScreen { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000864 RID: 2148 RVA: 0x00032D91 File Offset: 0x00030F91
		// (set) Token: 0x06000865 RID: 2149 RVA: 0x00032D98 File Offset: 0x00030F98
		public static Screen PreviousScreen { get; set; }

		// Token: 0x06000866 RID: 2150 RVA: 0x00032DA0 File Offset: 0x00030FA0
		public static T FindScreen<T>(string name) where T : Screen
		{
			Screen screen;
			ScreensManager.m_screens.TryGetValue(name, out screen);
			return (T)((object)screen);
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x00032DC1 File Offset: 0x00030FC1
		public static void AddScreen(string name, Screen screen)
		{
			ScreensManager.m_screens.Add(name, screen);
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x00032DCF File Offset: 0x00030FCF
		public static void SwitchScreen(string name, params object[] parameters)
		{
			ScreensManager.SwitchScreen(string.IsNullOrEmpty(name) ? null : ScreensManager.FindScreen<Screen>(name), parameters);
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x00032DE8 File Offset: 0x00030FE8
		public static void SwitchScreen(Screen screen, params object[] parameters)
		{
			if (ScreensManager.m_animationData != null)
			{
				ScreensManager.EndAnimation();
			}
			ScreensManager.m_animationData = new ScreensManager.AnimationData
			{
				NewScreen = screen,
				OldScreen = ScreensManager.CurrentScreen,
				Parameters = parameters,
				Speed = ((ScreensManager.CurrentScreen == null) ? float.MaxValue : 4f)
			};
			if (ScreensManager.CurrentScreen != null)
			{
				ScreensManager.RootWidget.IsUpdateEnabled = false;
				ScreensManager.CurrentScreen.Input.Clear();
			}
			ScreensManager.PreviousScreen = ScreensManager.CurrentScreen;
			ScreensManager.CurrentScreen = screen;
			ScreensManager.UpdateAnimation();
			if (ScreensManager.CurrentScreen != null)
			{
				Log.Verbose("Entered screen \"" + ScreensManager.GetScreenName(ScreensManager.CurrentScreen) + "\"");
			}
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x00032E98 File Offset: 0x00031098
		public static void Initialize()
		{
			ScreensManager.RootWidget = new CanvasWidget();
			ScreensManager.RootWidget.WidgetsHierarchyInput = new WidgetInput(WidgetInputDevice.All);
			ScreensManager.InitScreens();
			ScreensManager.SwitchScreen("Loading", Array.Empty<object>());
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x00032ECC File Offset: 0x000310CC
		public static void InitScreens()
		{
			LoadingScreen screen = new LoadingScreen();
			ScreensManager.AddScreen("Loading", screen);
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x00032EEA File Offset: 0x000310EA
		public static void Update()
		{
			if (ScreensManager.m_animationData != null)
			{
				ScreensManager.UpdateAnimation();
			}
			Widget.UpdateWidgetsHierarchy(ScreensManager.RootWidget);
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x00032F02 File Offset: 0x00031102
		public static void Draw()
		{
			Utilities.Dispose<RenderTarget2D>(ref ScreensManager.m_uiRenderTarget);
			ScreensManager.LayoutAndDrawWidgets();
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x00032F14 File Offset: 0x00031114
		public static void UpdateAnimation()
		{
			float num = MathUtils.Min(Time.FrameDuration, 0.1f);
			float factor = ScreensManager.m_animationData.Factor;
			ScreensManager.m_animationData.Factor = MathUtils.Min(ScreensManager.m_animationData.Factor + ScreensManager.m_animationData.Speed * num, 1f);
			if (ScreensManager.m_animationData.Factor < 0.5f)
			{
				if (ScreensManager.m_animationData.OldScreen != null)
				{
					float num2 = 2f * (0.5f - ScreensManager.m_animationData.Factor);
					float scale = 1f;
					ScreensManager.m_animationData.OldScreen.ColorTransform = new Color(num2, num2, num2, num2);
					ScreensManager.m_animationData.OldScreen.RenderTransform = Matrix.CreateTranslation((0f - ScreensManager.m_animationData.OldScreen.ActualSize.X) / 2f, (0f - ScreensManager.m_animationData.OldScreen.ActualSize.Y) / 2f, 0f) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(ScreensManager.m_animationData.OldScreen.ActualSize.X / 2f, ScreensManager.m_animationData.OldScreen.ActualSize.Y / 2f, 0f);
				}
			}
			else if (factor < 0.5f)
			{
				if (ScreensManager.m_animationData.OldScreen != null)
				{
					ScreensManager.m_animationData.OldScreen.Leave();
					ScreensManager.RootWidget.Children.Remove(ScreensManager.m_animationData.OldScreen);
				}
				if (ScreensManager.m_animationData.NewScreen != null)
				{
					ScreensManager.RootWidget.Children.Insert(0, ScreensManager.m_animationData.NewScreen);
					ScreensManager.m_animationData.NewScreen.Enter(ScreensManager.m_animationData.Parameters);
					ScreensManager.m_animationData.NewScreen.ColorTransform = Color.Transparent;
					ScreensManager.RootWidget.IsUpdateEnabled = true;
				}
			}
			else if (ScreensManager.m_animationData.NewScreen != null)
			{
				float num3 = 2f * (ScreensManager.m_animationData.Factor - 0.5f);
				float scale2 = 1f;
				ScreensManager.m_animationData.NewScreen.ColorTransform = new Color(num3, num3, num3, num3);
				ScreensManager.m_animationData.NewScreen.RenderTransform = Matrix.CreateTranslation((0f - ScreensManager.m_animationData.NewScreen.ActualSize.X) / 2f, (0f - ScreensManager.m_animationData.NewScreen.ActualSize.Y) / 2f, 0f) * Matrix.CreateScale(scale2) * Matrix.CreateTranslation(ScreensManager.m_animationData.NewScreen.ActualSize.X / 2f, ScreensManager.m_animationData.NewScreen.ActualSize.Y / 2f, 0f);
			}
			if (ScreensManager.m_animationData.Factor >= 1f)
			{
				ScreensManager.EndAnimation();
			}
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x00033218 File Offset: 0x00031418
		public static void EndAnimation()
		{
			if (ScreensManager.m_animationData.NewScreen != null)
			{
				ScreensManager.m_animationData.NewScreen.ColorTransform = Color.White;
				ScreensManager.m_animationData.NewScreen.RenderTransform = Matrix.CreateScale(1f);
			}
			ScreensManager.m_animationData = null;
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x00033264 File Offset: 0x00031464
		public static string GetScreenName(Screen screen)
		{
			string key = ScreensManager.m_screens.FirstOrDefault((KeyValuePair<string, Screen> kvp) => kvp.Value == screen).Key;
			if (key == null)
			{
				return string.Empty;
			}
			return key;
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x000332A8 File Offset: 0x000314A8
		public static void AnimateVrQuad()
		{
			if (Time.FrameIndex >= 5)
			{
				float num = 6f;
				Matrix identity = Matrix.Identity;
				Vector3 vector = identity.Translation + num * (Vector3.Normalize(identity.Forward * new Vector3(1f, 0f, 1f)) + new Vector3(0f, 0.1f, 0f));
				if (ScreensManager.m_vrQuadPosition == Vector3.Zero)
				{
					ScreensManager.m_vrQuadPosition = vector;
				}
				if (Vector3.Distance(ScreensManager.m_vrQuadPosition, vector) > 0f)
				{
					Vector3 v = vector * new Vector3(1f, 0f, 1f) - ScreensManager.m_vrQuadPosition * new Vector3(1f, 0f, 1f);
					Vector3 v2 = vector * new Vector3(0f, 1f, 0f) - ScreensManager.m_vrQuadPosition * new Vector3(0f, 1f, 0f);
					float num2 = v.Length();
					float num3 = v2.Length();
					ScreensManager.m_vrQuadPosition += v * MathUtils.Min(0.75f * MathUtils.Pow(MathUtils.Max(num2 - 0.15f * num, 0f), 0.33f) * Time.FrameDuration, 1f);
					ScreensManager.m_vrQuadPosition += v2 * MathUtils.Min(1.5f * MathUtils.Pow(MathUtils.Max(num3 - 0.05f * num, 0f), 0.33f) * Time.FrameDuration, 1f);
				}
				Vector2 vector2 = new Vector2((float)ScreensManager.m_uiRenderTarget.Width / (float)ScreensManager.m_uiRenderTarget.Height, 1f);
				vector2 /= MathUtils.Max(vector2.X, vector2.Y);
				vector2 *= 7.5f;
				ScreensManager.m_vrQuadMatrix.Forward = Vector3.Normalize(identity.Translation - ScreensManager.m_vrQuadPosition);
				ScreensManager.m_vrQuadMatrix.Right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, ScreensManager.m_vrQuadMatrix.Forward)) * vector2.X;
				ScreensManager.m_vrQuadMatrix.Up = Vector3.Normalize(Vector3.Cross(ScreensManager.m_vrQuadMatrix.Forward, ScreensManager.m_vrQuadMatrix.Right)) * vector2.Y;
				ScreensManager.m_vrQuadMatrix.Translation = ScreensManager.m_vrQuadPosition - 0.5f * (ScreensManager.m_vrQuadMatrix.Right + ScreensManager.m_vrQuadMatrix.Up);
				ScreensManager.RootWidget.WidgetsHierarchyInput.VrQuadMatrix = new Matrix?(ScreensManager.m_vrQuadMatrix);
			}
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x00033588 File Offset: 0x00031788
		public static void DrawVrQuad()
		{
			ScreensManager.QueueQuad(ScreensManager.m_pr3.TexturedBatch(ScreensManager.m_uiRenderTarget, false, 0, DepthStencilState.Default, RasterizerState.CullNoneScissor, BlendState.Opaque, SamplerState.LinearClamp), ScreensManager.m_vrQuadMatrix.Translation, ScreensManager.m_vrQuadMatrix.Right, ScreensManager.m_vrQuadMatrix.Up, Color.White);
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x000335E4 File Offset: 0x000317E4
		public static void DrawVrBackground()
		{
			Matrix identity = Matrix.Identity;
			TexturedBatch3D batch = ScreensManager.m_pr3.TexturedBatch(ContentManager.Get<Texture2D>("Textures/Star", null), false, 0, null, null, null, null);
			ScreensManager.Random.Seed(0);
			for (int i = 0; i < 1500; i++)
			{
				float f = MathUtils.Pow(ScreensManager.Random.Float(0f, 1f), 6f);
				Color rgb = (MathUtils.Lerp(0.05f, 0.4f, f) * Color.White).RGB;
				int num = 6;
				Vector3 vector = ScreensManager.Random.Vector3(500f);
				Vector3 vector2 = Vector3.Normalize(Vector3.Cross(vector, Vector3.UnitY)) * (float)num;
				Vector3 up = Vector3.Normalize(Vector3.Cross(vector2, vector)) * (float)num;
				ScreensManager.QueueQuad(batch, vector + identity.Translation, vector2, up, rgb);
			}
			TexturedBatch3D batch2 = ScreensManager.m_pr3.TexturedBatch(ContentManager.Get<Texture2D>("Textures/Blocks", null), true, 1, null, null, null, SamplerState.PointClamp);
			for (int j = -8; j <= 8; j++)
			{
				for (int k = -8; k <= 8; k++)
				{
					float num2 = 1f;
					float num3 = 1f;
					Vector3 vector3 = new Vector3(((float)j - 0.5f) * num2, 0f, ((float)k - 0.5f) * num2) + new Vector3(MathUtils.Round(identity.Translation.X), 0f, MathUtils.Round(identity.Translation.Z));
					float num4 = Vector3.Distance(vector3, identity.Translation);
					float num5 = MathUtils.Lerp(1f, 0f, MathUtils.Saturate(num4 / 7f));
					if (num5 > 0f)
					{
						ScreensManager.QueueQuad(batch2, vector3, new Vector3(num3, 0f, 0f), new Vector3(0f, 0f, num3), Color.Gray * num5, new Vector2(0.1875f, 0.25f), new Vector2(0.25f, 0.3125f));
					}
				}
			}
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x00033818 File Offset: 0x00031A18
		public static void LayoutAndDrawWidgets()
		{
			if (ScreensManager.m_animationData != null)
			{
				Display.Clear(new Color?(Color.Black), new float?(1f), new int?(0));
			}
			float num;
			switch (SettingsManager.GuiSize)
			{
			case GuiSize.Smallest:
				num = 1120f;
				break;
			case GuiSize.Smaller:
				num = 960f;
				break;
			case GuiSize.Normal:
				num = 850f;
				break;
			default:
				num = 850f;
				break;
			}
			num *= ScreensManager.DebugUiScale;
			Vector2 vector = new Vector2((float)Display.Viewport.Width, (float)Display.Viewport.Height);
			float num2 = vector.X / num;
			Vector2 availableSize = new Vector2(num, num / vector.X * vector.Y);
			float num3 = num * 9f / 16f;
			if (vector.Y / num2 < num3)
			{
				num2 = vector.Y / num3;
				availableSize = new Vector2(num3 / vector.Y * vector.X, num3);
			}
			ScreensManager.RootWidget.LayoutTransform = Matrix.CreateScale(num2, num2, 1f);
			if (SettingsManager.UpsideDownLayout)
			{
				ScreensManager.RootWidget.LayoutTransform *= new Matrix(-1f, 0f, 0f, 0f, 0f, -1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
			}
			Widget.LayoutWidgetsHierarchy(ScreensManager.RootWidget, availableSize);
			Widget.DrawWidgetsHierarchy(ScreensManager.RootWidget);
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x000339A4 File Offset: 0x00031BA4
		public static void QueueQuad(FlatBatch3D batch, Vector3 corner, Vector3 right, Vector3 up, Color color)
		{
			Vector3 p = corner + right;
			Vector3 p2 = corner + right + up;
			Vector3 p3 = corner + up;
			batch.QueueQuad(corner, p, p2, p3, color);
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x000339DB File Offset: 0x00031BDB
		public static void QueueQuad(TexturedBatch3D batch, Vector3 center, Vector3 right, Vector3 up, Color color)
		{
			ScreensManager.QueueQuad(batch, center, right, up, color, new Vector2(0f, 0f), new Vector2(1f, 1f));
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x00033A08 File Offset: 0x00031C08
		public static void QueueQuad(TexturedBatch3D batch, Vector3 corner, Vector3 right, Vector3 up, Color color, Vector2 tc1, Vector2 tc2)
		{
			Vector3 p = corner + right;
			Vector3 p2 = corner + right + up;
			Vector3 p3 = corner + up;
			batch.QueueQuad(corner, p, p2, p3, new Vector2(tc1.X, tc2.Y), new Vector2(tc2.X, tc2.Y), new Vector2(tc2.X, tc1.Y), new Vector2(tc1.X, tc1.Y), color);
		}

		// Token: 0x040003DF RID: 991
		public static Dictionary<string, Screen> m_screens = new Dictionary<string, Screen>();

		// Token: 0x040003E0 RID: 992
		public static ScreensManager.AnimationData m_animationData;

		// Token: 0x040003E1 RID: 993
		public static PrimitivesRenderer2D m_pr2 = new PrimitivesRenderer2D();

		// Token: 0x040003E2 RID: 994
		public static PrimitivesRenderer3D m_pr3 = new PrimitivesRenderer3D();

		// Token: 0x040003E3 RID: 995
		public static Game.Random Random = new Game.Random(0);

		// Token: 0x040003E4 RID: 996
		public static RenderTarget2D m_uiRenderTarget;

		// Token: 0x040003E5 RID: 997
		public static Vector3 m_vrQuadPosition;

		// Token: 0x040003E6 RID: 998
		public static Matrix m_vrQuadMatrix;

		// Token: 0x040003E7 RID: 999
		public static float DebugUiScale = 1f;

		// Token: 0x02000472 RID: 1138
		public class AnimationData
		{
			// Token: 0x04001644 RID: 5700
			public Screen OldScreen;

			// Token: 0x04001645 RID: 5701
			public Screen NewScreen;

			// Token: 0x04001646 RID: 5702
			public float Factor;

			// Token: 0x04001647 RID: 5703
			public float Speed;

			// Token: 0x04001648 RID: 5704
			public object[] Parameters;
		}
	}
}
