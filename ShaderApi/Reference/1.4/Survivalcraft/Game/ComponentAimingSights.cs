using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E6 RID: 486
	public class ComponentAimingSights : Component, IUpdateable, IDrawable
	{
		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000D54 RID: 3412 RVA: 0x00060ACB File Offset: 0x0005ECCB
		// (set) Token: 0x06000D55 RID: 3413 RVA: 0x00060AD3 File Offset: 0x0005ECD3
		public bool IsSightsVisible { get; set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000D56 RID: 3414 RVA: 0x00060ADC File Offset: 0x0005ECDC
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Reset;
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000D57 RID: 3415 RVA: 0x00060AE0 File Offset: 0x0005ECE0
		public int[] DrawOrders
		{
			get
			{
				return ComponentAimingSights.m_drawOrders;
			}
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x00060AE7 File Offset: 0x0005ECE7
		public virtual void ShowAimingSights(Vector3 position, Vector3 direction)
		{
			this.IsSightsVisible = true;
			this.m_sightsPosition = position;
			this.m_sightsDirection = direction;
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x00060AFE File Offset: 0x0005ECFE
		public void Update(float dt)
		{
			this.IsSightsVisible = false;
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x00060B08 File Offset: 0x0005ED08
		public void Draw(Camera camera, int drawOrder)
		{
			if (camera.GameWidget != this.m_componentPlayer.GameWidget)
			{
				return;
			}
			if (this.m_componentPlayer.ComponentHealth.Health > 0f && this.m_componentPlayer.ComponentGui.ControlsContainerWidget.IsVisible)
			{
				if (this.IsSightsVisible)
				{
					Texture2D texture = ContentManager.Get<Texture2D>("Textures/Gui/Sights", null);
					float s = 8f;
					Vector3 v = this.m_sightsPosition + this.m_sightsDirection * 50f;
					Vector3 vector = Vector3.Normalize(Vector3.Cross(this.m_sightsDirection, Vector3.UnitY));
					Vector3 v2 = Vector3.Normalize(Vector3.Cross(this.m_sightsDirection, vector));
					Vector3 p = v + s * (-vector - v2);
					Vector3 p2 = v + s * (vector - v2);
					Vector3 p3 = v + s * (vector + v2);
					Vector3 p4 = v + s * (-vector + v2);
					TexturedBatch3D texturedBatch3D = this.m_primitivesRenderer3D.TexturedBatch(texture, false, 0, DepthStencilState.None, null, null, null);
					int count = texturedBatch3D.TriangleVertices.Count;
					texturedBatch3D.QueueQuad(p, p2, p3, p4, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), Color.White);
					texturedBatch3D.TransformTriangles(camera.ViewMatrix, count, -1);
				}
				if (!camera.UsesMovementControls && !this.IsSightsVisible && (SettingsManager.LookControlMode == LookControlMode.SplitTouch || !this.m_componentPlayer.ComponentInput.IsControlledByTouch))
				{
					Subtexture subtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Crosshair", null);
					float s2 = 1.25f;
					Vector3 v3 = camera.ViewPosition + camera.ViewDirection * 50f;
					Vector3 vector2 = Vector3.Normalize(Vector3.Cross(camera.ViewDirection, Vector3.UnitY));
					Vector3 v4 = Vector3.Normalize(Vector3.Cross(camera.ViewDirection, vector2));
					Vector3 p5 = v3 + s2 * (-vector2 - v4);
					Vector3 p6 = v3 + s2 * (vector2 - v4);
					Vector3 p7 = v3 + s2 * (vector2 + v4);
					Vector3 p8 = v3 + s2 * (-vector2 + v4);
					TexturedBatch3D texturedBatch3D2 = this.m_primitivesRenderer3D.TexturedBatch(subtexture.Texture, false, 0, DepthStencilState.None, null, null, null);
					int count2 = texturedBatch3D2.TriangleVertices.Count;
					texturedBatch3D2.QueueQuad(p5, p6, p7, p8, new Vector2(subtexture.TopLeft.X, subtexture.TopLeft.Y), new Vector2(subtexture.BottomRight.X, subtexture.TopLeft.Y), new Vector2(subtexture.BottomRight.X, subtexture.BottomRight.Y), new Vector2(subtexture.TopLeft.X, subtexture.BottomRight.Y), Color.White);
					texturedBatch3D2.TransformTriangles(camera.ViewMatrix, count2, -1);
				}
			}
			this.m_primitivesRenderer2D.Flush(true, int.MaxValue);
			this.m_primitivesRenderer3D.Flush(camera.ProjectionMatrix, true, int.MaxValue);
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x00060E76 File Offset: 0x0005F076
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
		}

		// Token: 0x040006DF RID: 1759
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040006E0 RID: 1760
		public readonly PrimitivesRenderer2D m_primitivesRenderer2D = new PrimitivesRenderer2D();

		// Token: 0x040006E1 RID: 1761
		public readonly PrimitivesRenderer3D m_primitivesRenderer3D = new PrimitivesRenderer3D();

		// Token: 0x040006E2 RID: 1762
		public Vector3 m_sightsPosition;

		// Token: 0x040006E3 RID: 1763
		public Vector3 m_sightsDirection;

		// Token: 0x040006E4 RID: 1764
		public static int[] m_drawOrders = new int[]
		{
			2000
		};
	}
}
