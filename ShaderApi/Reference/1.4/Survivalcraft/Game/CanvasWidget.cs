using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000373 RID: 883
	public class CanvasWidget : ContainerWidget
	{
		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06001A2C RID: 6700 RVA: 0x000CCA69 File Offset: 0x000CAC69
		// (set) Token: 0x06001A2D RID: 6701 RVA: 0x000CCA71 File Offset: 0x000CAC71
		public Vector2 Size { get; set; } = new Vector2(-1f);

		// Token: 0x06001A2E RID: 6702 RVA: 0x000CCA7A File Offset: 0x000CAC7A
		public static void SetPosition(Widget widget, Vector2 position)
		{
			CanvasWidget canvasWidget = widget.ParentWidget as CanvasWidget;
			if (canvasWidget == null)
			{
				return;
			}
			canvasWidget.SetWidgetPosition(widget, new Vector2?(position));
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x000CCA98 File Offset: 0x000CAC98
		public Vector2? GetWidgetPosition(Widget widget)
		{
			Vector2 value;
			if (this.m_positions.TryGetValue(widget, out value))
			{
				return new Vector2?(value);
			}
			return null;
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x000CCAC5 File Offset: 0x000CACC5
		public void SetWidgetPosition(Widget widget, Vector2? position)
		{
			if (position != null)
			{
				this.m_positions[widget] = position.Value;
				return;
			}
			this.m_positions.Remove(widget);
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x000CCAF1 File Offset: 0x000CACF1
		public override void WidgetRemoved(Widget widget)
		{
			this.m_positions.Remove(widget);
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x000CCB00 File Offset: 0x000CAD00
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			Vector2 vector = Vector2.Zero;
			if (this.Size.X >= 0f)
			{
				parentAvailableSize.X = MathUtils.Min(parentAvailableSize.X, this.Size.X);
			}
			if (this.Size.Y >= 0f)
			{
				parentAvailableSize.Y = MathUtils.Min(parentAvailableSize.Y, this.Size.Y);
			}
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					Vector2? widgetPosition = this.GetWidgetPosition(widget);
					Vector2 vector2 = (widgetPosition != null) ? widgetPosition.Value : Vector2.Zero;
					widget.Measure(Vector2.Max(parentAvailableSize - vector2 - 2f * widget.Margin, Vector2.Zero));
					vector = new Vector2
					{
						X = MathUtils.Max(vector.X, vector2.X + widget.ParentDesiredSize.X + 2f * widget.Margin.X),
						Y = MathUtils.Max(vector.Y, vector2.Y + widget.ParentDesiredSize.Y + 2f * widget.Margin.Y)
					};
				}
			}
			if (this.Size.X >= 0f)
			{
				vector.X = this.Size.X;
			}
			if (this.Size.Y >= 0f)
			{
				vector.Y = this.Size.Y;
			}
			base.DesiredSize = vector;
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x000CCCD8 File Offset: 0x000CAED8
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					Vector2? widgetPosition = this.GetWidgetPosition(widget);
					if (widgetPosition != null)
					{
						Vector2 zero = Vector2.Zero;
						zero.X = ((!float.IsPositiveInfinity(widget.ParentDesiredSize.X)) ? widget.ParentDesiredSize.X : MathUtils.Max(base.ActualSize.X - widgetPosition.Value.X, 0f));
						zero.Y = ((!float.IsPositiveInfinity(widget.ParentDesiredSize.Y)) ? widget.ParentDesiredSize.Y : MathUtils.Max(base.ActualSize.Y - widgetPosition.Value.Y, 0f));
						widget.Arrange(widgetPosition.Value, zero);
					}
					else
					{
						ContainerWidget.ArrangeChildWidgetInCell(Vector2.Zero, base.ActualSize, widget);
					}
				}
			}
		}

		// Token: 0x040011D1 RID: 4561
		public Dictionary<Widget, Vector2> m_positions = new Dictionary<Widget, Vector2>();
	}
}
