using System;
using Engine;

namespace Game
{
	// Token: 0x02000384 RID: 900
	public class FixedSizePanelWidget : ContainerWidget
	{
		// Token: 0x06001AB9 RID: 6841 RVA: 0x000D02DC File Offset: 0x000CE4DC
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			Vector2 zero = Vector2.Zero;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
					if (widget.ParentDesiredSize.X != float.PositiveInfinity)
					{
						zero.X = MathUtils.Max(zero.X, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
					}
					if (widget.ParentDesiredSize.Y != float.PositiveInfinity)
					{
						zero.Y = MathUtils.Max(zero.Y, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
					}
				}
			}
			base.DesiredSize = zero;
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x000D03F0 File Offset: 0x000CE5F0
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				ContainerWidget.ArrangeChildWidgetInCell(Vector2.Zero, base.ActualSize, widget);
			}
		}
	}
}
