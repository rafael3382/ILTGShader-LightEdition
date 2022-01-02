using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x0200037A RID: 890
	public abstract class ContainerWidget : Widget
	{
		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06001A6E RID: 6766 RVA: 0x000CDA44 File Offset: 0x000CBC44
		public IEnumerable<Widget> AllChildren
		{
			get
			{
				foreach (Widget childWidget in this.Children)
				{
					yield return childWidget;
					ContainerWidget containerWidget = childWidget as ContainerWidget;
					if (containerWidget != null)
					{
						foreach (Widget widget in containerWidget.AllChildren)
						{
							yield return widget;
						}
						IEnumerator<Widget> enumerator2 = null;
					}
					childWidget = null;
				}
				WidgetsList.Enumerator enumerator = default(WidgetsList.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x000CDA54 File Offset: 0x000CBC54
		public ContainerWidget()
		{
			this.Children = new WidgetsList(this);
		}

		// Token: 0x06001A70 RID: 6768 RVA: 0x000CDA68 File Offset: 0x000CBC68
		public override void UpdateCeases()
		{
			foreach (Widget widget in this.Children)
			{
				widget.UpdateCeases();
			}
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x000CDAB8 File Offset: 0x000CBCB8
		public void AddChildren(Widget widget)
		{
			if (this.Children.IndexOf(widget) < 0)
			{
				this.Children.Add(widget);
			}
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x000CDAD5 File Offset: 0x000CBCD5
		public void RemoveChildren(Widget widget)
		{
			if (this.Children.IndexOf(widget) >= 0)
			{
				this.Children.Remove(widget);
			}
		}

		// Token: 0x06001A73 RID: 6771 RVA: 0x000CDAF2 File Offset: 0x000CBCF2
		public void ClearChildren()
		{
			this.Children.Clear();
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x000CDAFF File Offset: 0x000CBCFF
		public virtual void WidgetAdded(Widget widget)
		{
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x000CDB01 File Offset: 0x000CBD01
		public virtual void WidgetRemoved(Widget widget)
		{
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x000CDB04 File Offset: 0x000CBD04
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			foreach (Widget widget in this.Children)
			{
				widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
			}
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x000CDB78 File Offset: 0x000CBD78
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				ContainerWidget.ArrangeChildWidgetInCell(Vector2.Zero, base.ActualSize, widget);
			}
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x000CDBD8 File Offset: 0x000CBDD8
		public static void ArrangeChildWidgetInCell(Vector2 c1, Vector2 c2, Widget widget)
		{
			Vector2 zero = Vector2.Zero;
			Vector2 zero2 = Vector2.Zero;
			Vector2 vector = c2 - c1;
			Vector2 margin = widget.Margin;
			Vector2 parentDesiredSize = widget.ParentDesiredSize;
			if (float.IsPositiveInfinity(parentDesiredSize.X) || parentDesiredSize.X > vector.X - 2f * margin.X)
			{
				parentDesiredSize.X = MathUtils.Max(vector.X - 2f * margin.X, 0f);
			}
			if (float.IsPositiveInfinity(parentDesiredSize.Y) || parentDesiredSize.Y > vector.Y - 2f * margin.Y)
			{
				parentDesiredSize.Y = MathUtils.Max(vector.Y - 2f * margin.Y, 0f);
			}
			if (widget.HorizontalAlignment == WidgetAlignment.Near)
			{
				zero.X = c1.X + margin.X;
				zero2.X = parentDesiredSize.X;
			}
			else if (widget.HorizontalAlignment == WidgetAlignment.Center)
			{
				zero.X = c1.X + (vector.X - parentDesiredSize.X) / 2f;
				zero2.X = parentDesiredSize.X;
			}
			else if (widget.HorizontalAlignment == WidgetAlignment.Far)
			{
				zero.X = c2.X - parentDesiredSize.X - margin.X;
				zero2.X = parentDesiredSize.X;
			}
			else if (widget.HorizontalAlignment == WidgetAlignment.Stretch)
			{
				zero.X = c1.X + margin.X;
				zero2.X = MathUtils.Max(vector.X - 2f * margin.X, 0f);
			}
			if (widget.VerticalAlignment == WidgetAlignment.Near)
			{
				zero.Y = c1.Y + margin.Y;
				zero2.Y = parentDesiredSize.Y;
			}
			else if (widget.VerticalAlignment == WidgetAlignment.Center)
			{
				zero.Y = c1.Y + (vector.Y - parentDesiredSize.Y) / 2f;
				zero2.Y = parentDesiredSize.Y;
			}
			else if (widget.VerticalAlignment == WidgetAlignment.Far)
			{
				zero.Y = c2.Y - parentDesiredSize.Y - margin.Y;
				zero2.Y = parentDesiredSize.Y;
			}
			else if (widget.VerticalAlignment == WidgetAlignment.Stretch)
			{
				zero.Y = c1.Y + margin.Y;
				zero2.Y = MathUtils.Max(vector.Y - 2f * margin.Y, 0f);
			}
			widget.Arrange(zero, zero2);
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x000CDE6C File Offset: 0x000CC06C
		public override void Dispose()
		{
			foreach (Widget widget in this.Children)
			{
				widget.Dispose();
			}
		}

		// Token: 0x040011F6 RID: 4598
		public readonly WidgetsList Children;
	}
}
