using System;
using Engine;

namespace Game
{
	// Token: 0x020003A5 RID: 933
	public class UniformSpacingPanelWidget : ContainerWidget
	{
		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06001C69 RID: 7273 RVA: 0x000DC4CB File Offset: 0x000DA6CB
		// (set) Token: 0x06001C6A RID: 7274 RVA: 0x000DC4D3 File Offset: 0x000DA6D3
		public LayoutDirection Direction
		{
			get
			{
				return this.m_direction;
			}
			set
			{
				this.m_direction = value;
			}
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x000DC4DC File Offset: 0x000DA6DC
		public override void ArrangeOverride()
		{
			Vector2 zero = Vector2.Zero;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					if (this.m_direction == LayoutDirection.Horizontal)
					{
						float num = (this.m_count > 0) ? (base.ActualSize.X / (float)this.m_count) : 0f;
						ContainerWidget.ArrangeChildWidgetInCell(zero, new Vector2(zero.X + num, zero.Y + base.ActualSize.Y), widget);
						zero.X += num;
					}
					else
					{
						float num2 = (this.m_count > 0) ? (base.ActualSize.Y / (float)this.m_count) : 0f;
						ContainerWidget.ArrangeChildWidgetInCell(zero, new Vector2(zero.X + base.ActualSize.X, zero.Y + num2), widget);
						zero.Y += num2;
					}
				}
			}
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x000DC5F8 File Offset: 0x000DA7F8
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			this.m_count = 0;
			using (WidgetsList.Enumerator enumerator = this.Children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsVisible)
					{
						this.m_count++;
					}
				}
			}
			parentAvailableSize = ((this.m_direction != LayoutDirection.Horizontal) ? Vector2.Min(parentAvailableSize, new Vector2(parentAvailableSize.X, parentAvailableSize.Y / (float)this.m_count)) : Vector2.Min(parentAvailableSize, new Vector2(parentAvailableSize.X / (float)this.m_count, parentAvailableSize.Y)));
			float num = 0f;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
					num = ((this.m_direction != LayoutDirection.Horizontal) ? MathUtils.Max(num, widget.ParentDesiredSize.X + 2f * widget.Margin.X) : MathUtils.Max(num, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y));
				}
			}
			base.DesiredSize = ((this.m_direction == LayoutDirection.Horizontal) ? new Vector2(float.PositiveInfinity, num) : new Vector2(num, float.PositiveInfinity));
		}

		// Token: 0x0400132F RID: 4911
		public LayoutDirection m_direction;

		// Token: 0x04001330 RID: 4912
		public int m_count;
	}
}
