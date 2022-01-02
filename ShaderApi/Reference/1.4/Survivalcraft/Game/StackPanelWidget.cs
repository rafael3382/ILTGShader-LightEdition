using System;
using Engine;

namespace Game
{
	// Token: 0x020003A0 RID: 928
	public class StackPanelWidget : ContainerWidget
	{
		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06001C17 RID: 7191 RVA: 0x000DAAD4 File Offset: 0x000D8CD4
		// (set) Token: 0x06001C18 RID: 7192 RVA: 0x000DAADC File Offset: 0x000D8CDC
		public LayoutDirection Direction { get; set; }

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06001C19 RID: 7193 RVA: 0x000DAAE5 File Offset: 0x000D8CE5
		// (set) Token: 0x06001C1A RID: 7194 RVA: 0x000DAAED File Offset: 0x000D8CED
		public bool IsInverted { get; set; }

		// Token: 0x06001C1B RID: 7195 RVA: 0x000DAAF8 File Offset: 0x000D8CF8
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			this.m_fixedSize = 0f;
			this.m_fillCount = 0;
			float num = 0f;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
					if (this.Direction == LayoutDirection.Horizontal)
					{
						if (widget.ParentDesiredSize.X != float.PositiveInfinity)
						{
							this.m_fixedSize += widget.ParentDesiredSize.X + 2f * widget.Margin.X;
							parentAvailableSize.X = MathUtils.Max(parentAvailableSize.X - (widget.ParentDesiredSize.X + 2f * widget.Margin.X), 0f);
						}
						else
						{
							this.m_fillCount++;
						}
						num = MathUtils.Max(num, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
					}
					else
					{
						if (widget.ParentDesiredSize.Y != float.PositiveInfinity)
						{
							this.m_fixedSize += widget.ParentDesiredSize.Y + 2f * widget.Margin.Y;
							parentAvailableSize.Y = MathUtils.Max(parentAvailableSize.Y - (widget.ParentDesiredSize.Y + 2f * widget.Margin.Y), 0f);
						}
						else
						{
							this.m_fillCount++;
						}
						num = MathUtils.Max(num, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
					}
				}
			}
			if (this.Direction == LayoutDirection.Horizontal)
			{
				base.DesiredSize = ((this.m_fillCount == 0) ? new Vector2(this.m_fixedSize, num) : new Vector2(float.PositiveInfinity, num));
				return;
			}
			base.DesiredSize = ((this.m_fillCount == 0) ? new Vector2(num, this.m_fixedSize) : new Vector2(num, float.PositiveInfinity));
		}

		// Token: 0x06001C1C RID: 7196 RVA: 0x000DAD50 File Offset: 0x000D8F50
		public override void ArrangeOverride()
		{
			float num = 0f;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					if (this.Direction == LayoutDirection.Horizontal)
					{
						float num2 = (widget.ParentDesiredSize.X == float.PositiveInfinity) ? ((this.m_fillCount > 0) ? (MathUtils.Max(base.ActualSize.X - this.m_fixedSize, 0f) / (float)this.m_fillCount) : 0f) : (widget.ParentDesiredSize.X + 2f * widget.Margin.X);
						Vector2 c;
						Vector2 c2;
						if (!this.IsInverted)
						{
							c = new Vector2(num, 0f);
							c2 = new Vector2(num + num2, base.ActualSize.Y);
						}
						else
						{
							c = new Vector2(base.ActualSize.X - (num + num2), 0f);
							c2 = new Vector2(base.ActualSize.X - num, base.ActualSize.Y);
						}
						ContainerWidget.ArrangeChildWidgetInCell(c, c2, widget);
						num += num2;
					}
					else
					{
						float num3 = (widget.ParentDesiredSize.Y == float.PositiveInfinity) ? ((this.m_fillCount > 0) ? (MathUtils.Max(base.ActualSize.Y - this.m_fixedSize, 0f) / (float)this.m_fillCount) : 0f) : (widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
						Vector2 c3;
						Vector2 c4;
						if (!this.IsInverted)
						{
							c3 = new Vector2(0f, num);
							c4 = new Vector2(base.ActualSize.X, num + num3);
						}
						else
						{
							c3 = new Vector2(0f, base.ActualSize.Y - (num + num3));
							c4 = new Vector2(base.ActualSize.X, base.ActualSize.Y - num);
						}
						ContainerWidget.ArrangeChildWidgetInCell(c3, c4, widget);
						num += num3;
					}
				}
			}
		}

		// Token: 0x04001305 RID: 4869
		public float m_fixedSize;

		// Token: 0x04001306 RID: 4870
		public int m_fillCount;
	}
}
