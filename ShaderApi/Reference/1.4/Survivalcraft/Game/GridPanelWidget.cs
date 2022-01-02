using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x0200038C RID: 908
	public class GridPanelWidget : ContainerWidget
	{
		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06001B0C RID: 6924 RVA: 0x000D42D5 File Offset: 0x000D24D5
		// (set) Token: 0x06001B0D RID: 6925 RVA: 0x000D42E4 File Offset: 0x000D24E4
		public int ColumnsCount
		{
			get
			{
				return this.m_columns.Count;
			}
			set
			{
				this.m_columns = new List<GridPanelWidget.Column>(this.m_columns.GetRange(0, MathUtils.Min(this.m_columns.Count, value)));
				while (this.m_columns.Count < value)
				{
					this.m_columns.Add(new GridPanelWidget.Column());
				}
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06001B0E RID: 6926 RVA: 0x000D4339 File Offset: 0x000D2539
		// (set) Token: 0x06001B0F RID: 6927 RVA: 0x000D4348 File Offset: 0x000D2548
		public int RowsCount
		{
			get
			{
				return this.m_rows.Count;
			}
			set
			{
				this.m_rows = new List<GridPanelWidget.Row>(this.m_rows.GetRange(0, MathUtils.Min(this.m_rows.Count, value)));
				while (this.m_rows.Count < value)
				{
					this.m_rows.Add(new GridPanelWidget.Row());
				}
			}
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x000D439D File Offset: 0x000D259D
		public GridPanelWidget()
		{
			this.ColumnsCount = 1;
			this.RowsCount = 1;
		}

		// Token: 0x06001B11 RID: 6929 RVA: 0x000D43D4 File Offset: 0x000D25D4
		public Point2 GetWidgetCell(Widget widget)
		{
			Point2 result;
			this.m_cells.TryGetValue(widget, out result);
			return result;
		}

		// Token: 0x06001B12 RID: 6930 RVA: 0x000D43F1 File Offset: 0x000D25F1
		public void SetWidgetCell(Widget widget, Point2 cell)
		{
			this.m_cells[widget] = cell;
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x000D4400 File Offset: 0x000D2600
		public static void SetCell(Widget widget, Point2 cell)
		{
			GridPanelWidget gridPanelWidget = widget.ParentWidget as GridPanelWidget;
			if (gridPanelWidget == null)
			{
				return;
			}
			gridPanelWidget.SetWidgetCell(widget, cell);
		}

		// Token: 0x06001B14 RID: 6932 RVA: 0x000D4419 File Offset: 0x000D2619
		public override void WidgetRemoved(Widget widget)
		{
			this.m_cells.Remove(widget);
		}

		// Token: 0x06001B15 RID: 6933 RVA: 0x000D4428 File Offset: 0x000D2628
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			foreach (GridPanelWidget.Column column in this.m_columns)
			{
				column.ActualWidth = 0f;
			}
			foreach (GridPanelWidget.Row row in this.m_rows)
			{
				row.ActualHeight = 0f;
			}
			foreach (Widget widget in this.Children)
			{
				widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
				Point2 widgetCell = this.GetWidgetCell(widget);
				if (this.IsCellValid(widgetCell))
				{
					GridPanelWidget.Column column2 = this.m_columns[widgetCell.X];
					column2.ActualWidth = MathUtils.Max(column2.ActualWidth, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
					GridPanelWidget.Row row2 = this.m_rows[widgetCell.Y];
					row2.ActualHeight = MathUtils.Max(row2.ActualHeight, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
				}
			}
			Vector2 zero = Vector2.Zero;
			foreach (GridPanelWidget.Column column3 in this.m_columns)
			{
				column3.Position = zero.X;
				zero.X += column3.ActualWidth;
			}
			foreach (GridPanelWidget.Row row3 in this.m_rows)
			{
				row3.Position = zero.Y;
				zero.Y += row3.ActualHeight;
			}
			base.DesiredSize = zero;
		}

		// Token: 0x06001B16 RID: 6934 RVA: 0x000D4688 File Offset: 0x000D2888
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				Point2 widgetCell = this.GetWidgetCell(widget);
				if (this.IsCellValid(widgetCell))
				{
					GridPanelWidget.Column column = this.m_columns[widgetCell.X];
					GridPanelWidget.Row row = this.m_rows[widgetCell.Y];
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(column.Position, row.Position), new Vector2(column.Position + column.ActualWidth, row.Position + row.ActualHeight), widget);
				}
				else
				{
					ContainerWidget.ArrangeChildWidgetInCell(Vector2.Zero, base.ActualSize, widget);
				}
			}
		}

		// Token: 0x06001B17 RID: 6935 RVA: 0x000D475C File Offset: 0x000D295C
		public bool IsCellValid(Point2 cell)
		{
			return cell.X >= 0 && cell.X < this.m_columns.Count && cell.Y >= 0 && cell.Y < this.m_rows.Count;
		}

		// Token: 0x04001268 RID: 4712
		public List<GridPanelWidget.Column> m_columns = new List<GridPanelWidget.Column>();

		// Token: 0x04001269 RID: 4713
		public List<GridPanelWidget.Row> m_rows = new List<GridPanelWidget.Row>();

		// Token: 0x0400126A RID: 4714
		public Dictionary<Widget, Point2> m_cells = new Dictionary<Widget, Point2>();

		// Token: 0x02000582 RID: 1410
		public class Column
		{
			// Token: 0x040019E5 RID: 6629
			public float Position;

			// Token: 0x040019E6 RID: 6630
			public float ActualWidth;
		}

		// Token: 0x02000583 RID: 1411
		public class Row
		{
			// Token: 0x040019E7 RID: 6631
			public float Position;

			// Token: 0x040019E8 RID: 6632
			public float ActualHeight;
		}
	}
}
