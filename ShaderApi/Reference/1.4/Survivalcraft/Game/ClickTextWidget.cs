using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000378 RID: 888
	public class ClickTextWidget : CanvasWidget
	{
		// Token: 0x06001A69 RID: 6761 RVA: 0x000CD4B4 File Offset: 0x000CB6B4
		public ClickTextWidget(Vector2 vector2, string text, Action click, bool box = false)
		{
			base.Size = vector2;
			this.HorizontalAlignment = WidgetAlignment.Center;
			this.VerticalAlignment = WidgetAlignment.Center;
			this.labelWidget = new LabelWidget
			{
				Text = text,
				FontScale = 0.8f,
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center
			};
			this.Children.Add(this.labelWidget);
			base.IsDrawEnabled = true;
			base.IsDrawRequired = true;
			base.IsUpdateEnabled = true;
			this.click = click;
		}

		// Token: 0x06001A6A RID: 6762 RVA: 0x000CD54C File Offset: 0x000CB74C
		public override void Draw(Widget.DrawContext dc)
		{
			Matrix globalTransform = base.GlobalTransform;
			Vector2 zero = Vector2.Zero;
			Vector2 vector = new Vector2(base.ActualSize.X, 0f);
			Vector2 actualSize = base.ActualSize;
			Vector2 vector2 = new Vector2(0f, base.ActualSize.Y);
			Vector2 vector3;
			Vector2.Transform(ref zero, ref globalTransform, out vector3);
			Vector2 vector4;
			Vector2.Transform(ref vector, ref globalTransform, out vector4);
			Vector2 vector5;
			Vector2.Transform(ref actualSize, ref globalTransform, out vector5);
			Vector2 vector6;
			Vector2.Transform(ref vector2, ref globalTransform, out vector6);
			FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(1, DepthStencilState.DepthWrite, null, null);
			Vector2 vector7 = Vector2.Normalize(base.GlobalTransform.Right.XY);
			Vector2 v = -Vector2.Normalize(base.GlobalTransform.Up.XY);
			for (int i = 0; i < 1; i++)
			{
				flatBatch2D.QueueLine(vector3, vector4, 1f, this.BorderColor);
				flatBatch2D.QueueLine(vector4, vector5, 1f, this.BorderColor);
				flatBatch2D.QueueLine(vector5, vector6, 1f, this.BorderColor);
				flatBatch2D.QueueLine(vector6, vector3, 1f, this.BorderColor);
				vector3 += vector7 - v;
				vector4 += -vector7 - v;
				vector5 += -vector7 + v;
				vector6 += vector7 + v;
			}
		}

		// Token: 0x06001A6B RID: 6763 RVA: 0x000CD6E0 File Offset: 0x000CB8E0
		public override void Update()
		{
			if (base.Input.Click != null && this.HitTest(base.Input.Click.Value.Start) && this.HitTest(base.Input.Click.Value.End))
			{
				Action action = this.click;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x040011EA RID: 4586
		public LabelWidget labelWidget;

		// Token: 0x040011EB RID: 4587
		public Action click;

		// Token: 0x040011EC RID: 4588
		public RectangleWidget rectangleWidget;

		// Token: 0x040011ED RID: 4589
		public Color pressColor = Color.Red;

		// Token: 0x040011EE RID: 4590
		public Color BorderColor = Color.Transparent;
	}
}
