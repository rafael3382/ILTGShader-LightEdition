using System;
using Engine;
using Engine.Graphics;
using Engine.Input;

namespace Game
{
	// Token: 0x02000396 RID: 918
	public class MoveRoseWidget : Widget
	{
		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06001B9F RID: 7071 RVA: 0x000D793A File Offset: 0x000D5B3A
		public Vector3 Direction
		{
			get
			{
				if (base.IsEnabledGlobal && base.IsVisibleGlobal)
				{
					return this.m_direction;
				}
				return Vector3.Zero;
			}
		}

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06001BA0 RID: 7072 RVA: 0x000D7958 File Offset: 0x000D5B58
		public bool Jump
		{
			get
			{
				return base.IsEnabledGlobal && base.IsVisibleGlobal && this.m_jump;
			}
		}

		// Token: 0x06001BA1 RID: 7073 RVA: 0x000D7974 File Offset: 0x000D5B74
		public override void Update()
		{
			this.m_direction = Vector3.Zero;
			this.m_jump = false;
			Vector2 v = base.ActualSize / 2f;
			float num = base.ActualSize.X / 2f;
			float num2 = num / 3.5f;
			float num3 = MathUtils.DegToRad(35f);
			foreach (TouchLocation touchLocation in base.Input.TouchLocations)
			{
				if (base.HitTestGlobal(touchLocation.Position, null) == this)
				{
					if (touchLocation.State == TouchLocationState.Pressed && Vector2.Distance(base.ScreenToWidget(touchLocation.Position), v) <= num2)
					{
						this.m_jump = true;
						this.m_jumpTouchId = new int?(touchLocation.Id);
					}
					if (touchLocation.State == TouchLocationState.Released && this.m_jumpTouchId != null && touchLocation.Id == this.m_jumpTouchId.Value)
					{
						this.m_jumpTouchId = null;
					}
					if (touchLocation.State == TouchLocationState.Moved || touchLocation.State == TouchLocationState.Pressed)
					{
						Vector2 v2 = base.ScreenToWidget(touchLocation.Position);
						float num4 = Vector2.Distance(v2, v);
						if (num4 > num2 && num4 <= num)
						{
							float num5 = Vector2.Angle(v2 - v, -Vector2.UnitY);
							if (MathUtils.Abs(MathUtils.NormalizeAngle(num5 - 0f)) < num3)
							{
								this.m_direction = ((this.m_jumpTouchId != null) ? new Vector3(0f, 1f, 0f) : new Vector3(0f, 0f, 1f));
							}
							else if (MathUtils.Abs(MathUtils.NormalizeAngle(num5 - 1.57079637f)) < num3)
							{
								this.m_direction = new Vector3(-1f, 0f, 0f);
							}
							else if (MathUtils.Abs(MathUtils.NormalizeAngle(num5 - 3.14159274f)) < num3)
							{
								this.m_direction = ((this.m_jumpTouchId != null) ? new Vector3(0f, -1f, 0f) : new Vector3(0f, 0f, -1f));
							}
							else if (MathUtils.Abs(MathUtils.NormalizeAngle(num5 - 4.712389f)) < num3)
							{
								this.m_direction = new Vector3(1f, 0f, 0f);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001BA2 RID: 7074 RVA: 0x000D7C10 File Offset: 0x000D5E10
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
		}

		// Token: 0x06001BA3 RID: 7075 RVA: 0x000D7C1C File Offset: 0x000D5E1C
		public override void Draw(Widget.DrawContext dc)
		{
			Subtexture subtexture = ContentManager.Get<Subtexture>("Textures/Atlas/MoveRose", null);
			Subtexture subtexture2 = ContentManager.Get<Subtexture>("Textures/Atlas/MoveRose_Pressed", null);
			TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(subtexture.Texture, false, 0, null, null, null, null);
			TexturedBatch2D texturedBatch2D2 = dc.PrimitivesRenderer2D.TexturedBatch(subtexture2.Texture, false, 0, null, null, null, null);
			int count = texturedBatch2D.TriangleVertices.Count;
			int count2 = texturedBatch2D2.TriangleVertices.Count;
			Vector2 p = base.ActualSize / 2f;
			Vector2 vector = new Vector2(0f, 0f);
			Vector2 vector2 = new Vector2(base.ActualSize.X, 0f);
			Vector2 vector3 = new Vector2(base.ActualSize.X, base.ActualSize.Y);
			Vector2 vector4 = new Vector2(0f, base.ActualSize.Y);
			if (this.m_direction.Z > 0f)
			{
				Vector2 subtextureCoords = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(0f, 0f));
				Vector2 subtextureCoords2 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(1f, 0f));
				Vector2 subtextureCoords3 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(0.5f, 0.5f));
				texturedBatch2D2.QueueTriangle(vector, vector2, p, 0f, subtextureCoords, subtextureCoords2, subtextureCoords3, base.GlobalColorTransform);
			}
			else
			{
				Vector2 subtextureCoords4 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(0f, 0f));
				Vector2 subtextureCoords5 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(1f, 0f));
				Vector2 subtextureCoords6 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(0.5f, 0.5f));
				texturedBatch2D.QueueTriangle(vector, vector2, p, 0f, subtextureCoords4, subtextureCoords5, subtextureCoords6, base.GlobalColorTransform);
			}
			if (this.m_direction.X > 0f)
			{
				Vector2 subtextureCoords7 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(1f, 0f));
				Vector2 subtextureCoords8 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(1f, 1f));
				Vector2 subtextureCoords9 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(0.5f, 0.5f));
				texturedBatch2D2.QueueTriangle(vector2, vector3, p, 0f, subtextureCoords7, subtextureCoords8, subtextureCoords9, base.GlobalColorTransform);
			}
			else
			{
				Vector2 subtextureCoords10 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(1f, 0f));
				Vector2 subtextureCoords11 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(1f, 1f));
				Vector2 subtextureCoords12 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(0.5f, 0.5f));
				texturedBatch2D.QueueTriangle(vector2, vector3, p, 0f, subtextureCoords10, subtextureCoords11, subtextureCoords12, base.GlobalColorTransform);
			}
			if (this.m_direction.Z < 0f)
			{
				Vector2 subtextureCoords13 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(1f, 1f));
				Vector2 subtextureCoords14 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(0f, 1f));
				Vector2 subtextureCoords15 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(0.5f, 0.5f));
				texturedBatch2D2.QueueTriangle(vector3, vector4, p, 0f, subtextureCoords13, subtextureCoords14, subtextureCoords15, base.GlobalColorTransform);
			}
			else
			{
				Vector2 subtextureCoords16 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(1f, 1f));
				Vector2 subtextureCoords17 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(0f, 1f));
				Vector2 subtextureCoords18 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(0.5f, 0.5f));
				texturedBatch2D.QueueTriangle(vector3, vector4, p, 0f, subtextureCoords16, subtextureCoords17, subtextureCoords18, base.GlobalColorTransform);
			}
			if (this.m_direction.X < 0f)
			{
				Vector2 subtextureCoords19 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(0f, 1f));
				Vector2 subtextureCoords20 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(0f, 0f));
				Vector2 subtextureCoords21 = MoveRoseWidget.GetSubtextureCoords(subtexture2, new Vector2(0.5f, 0.5f));
				texturedBatch2D2.QueueTriangle(vector4, vector, p, 0f, subtextureCoords19, subtextureCoords20, subtextureCoords21, base.GlobalColorTransform);
			}
			else
			{
				Vector2 subtextureCoords22 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(0f, 1f));
				Vector2 subtextureCoords23 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(0f, 0f));
				Vector2 subtextureCoords24 = MoveRoseWidget.GetSubtextureCoords(subtexture, new Vector2(0.5f, 0.5f));
				texturedBatch2D.QueueTriangle(vector4, vector, p, 0f, subtextureCoords22, subtextureCoords23, subtextureCoords24, base.GlobalColorTransform);
			}
			if (texturedBatch2D == texturedBatch2D2)
			{
				texturedBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
				return;
			}
			texturedBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
			texturedBatch2D2.TransformTriangles(base.GlobalTransform, count2, -1);
		}

		// Token: 0x06001BA4 RID: 7076 RVA: 0x000D808C File Offset: 0x000D628C
		public static Vector2 GetSubtextureCoords(Subtexture subtexture, Vector2 texCoords)
		{
			return new Vector2(MathUtils.Lerp(subtexture.TopLeft.X, subtexture.BottomRight.X, texCoords.X), MathUtils.Lerp(subtexture.TopLeft.Y, subtexture.BottomRight.Y, texCoords.Y));
		}

		// Token: 0x040012BD RID: 4797
		public Vector3 m_direction;

		// Token: 0x040012BE RID: 4798
		public bool m_jump;

		// Token: 0x040012BF RID: 4799
		public int? m_jumpTouchId;
	}
}
