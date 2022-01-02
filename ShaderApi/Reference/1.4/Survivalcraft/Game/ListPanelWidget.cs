using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000391 RID: 913
	public class ListPanelWidget : ScrollPanelWidget
	{
		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06001B53 RID: 6995 RVA: 0x000D5C4F File Offset: 0x000D3E4F
		// (set) Token: 0x06001B54 RID: 6996 RVA: 0x000D5C57 File Offset: 0x000D3E57
		public Func<object, Widget> ItemWidgetFactory { get; set; }

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06001B55 RID: 6997 RVA: 0x000D5C60 File Offset: 0x000D3E60
		// (set) Token: 0x06001B56 RID: 6998 RVA: 0x000D5C68 File Offset: 0x000D3E68
		public override LayoutDirection Direction
		{
			get
			{
				return base.Direction;
			}
			set
			{
				if (value != this.Direction)
				{
					base.Direction = value;
					this.m_widgetsDirty = true;
				}
			}
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06001B57 RID: 6999 RVA: 0x000D5C81 File Offset: 0x000D3E81
		// (set) Token: 0x06001B58 RID: 7000 RVA: 0x000D5C89 File Offset: 0x000D3E89
		public override float ScrollPosition
		{
			get
			{
				return base.ScrollPosition;
			}
			set
			{
				if (value != this.ScrollPosition)
				{
					base.ScrollPosition = value;
					this.m_widgetsDirty = true;
				}
			}
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06001B59 RID: 7001 RVA: 0x000D5CA2 File Offset: 0x000D3EA2
		// (set) Token: 0x06001B5A RID: 7002 RVA: 0x000D5CAA File Offset: 0x000D3EAA
		public float ItemSize
		{
			get
			{
				return this.m_itemSize;
			}
			set
			{
				if (value != this.m_itemSize)
				{
					this.m_itemSize = value;
					this.m_widgetsDirty = true;
				}
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06001B5B RID: 7003 RVA: 0x000D5CC3 File Offset: 0x000D3EC3
		// (set) Token: 0x06001B5C RID: 7004 RVA: 0x000D5CCC File Offset: 0x000D3ECC
		public int? SelectedIndex
		{
			get
			{
				return this.m_selectedItemIndex;
			}
			set
			{
				if (value != null && (value.Value < 0 || value.Value >= this.m_items.Count))
				{
					value = null;
				}
				int? num = value;
				int? selectedItemIndex = this.m_selectedItemIndex;
				if (!(num.GetValueOrDefault() == selectedItemIndex.GetValueOrDefault() & num != null == (selectedItemIndex != null)))
				{
					this.m_selectedItemIndex = value;
					Action selectionChanged = this.SelectionChanged;
					if (selectionChanged == null)
					{
						return;
					}
					selectionChanged();
				}
			}
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06001B5D RID: 7005 RVA: 0x000D5D4B File Offset: 0x000D3F4B
		// (set) Token: 0x06001B5E RID: 7006 RVA: 0x000D5D74 File Offset: 0x000D3F74
		public object SelectedItem
		{
			get
			{
				if (this.m_selectedItemIndex == null)
				{
					return null;
				}
				return this.m_items[this.m_selectedItemIndex.Value];
			}
			set
			{
				int num = this.m_items.IndexOf(value);
				this.SelectedIndex = ((num >= 0) ? new int?(num) : null);
			}
		}

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06001B5F RID: 7007 RVA: 0x000D5DA9 File Offset: 0x000D3FA9
		public ReadOnlyList<object> Items
		{
			get
			{
				return new ReadOnlyList<object>(this.m_items);
			}
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06001B60 RID: 7008 RVA: 0x000D5DB6 File Offset: 0x000D3FB6
		// (set) Token: 0x06001B61 RID: 7009 RVA: 0x000D5DBE File Offset: 0x000D3FBE
		public Color SelectionColor { get; set; }

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06001B62 RID: 7010 RVA: 0x000D5DC7 File Offset: 0x000D3FC7
		// (set) Token: 0x06001B63 RID: 7011 RVA: 0x000D5DCF File Offset: 0x000D3FCF
		public virtual Action<object> ItemClicked { get; set; }

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06001B64 RID: 7012 RVA: 0x000D5DD8 File Offset: 0x000D3FD8
		// (set) Token: 0x06001B65 RID: 7013 RVA: 0x000D5DE0 File Offset: 0x000D3FE0
		public virtual Action SelectionChanged { get; set; }

		// Token: 0x06001B66 RID: 7014 RVA: 0x000D5DEC File Offset: 0x000D3FEC
		public ListPanelWidget()
		{
			this.SelectionColor = Color.Gray;
			this.ItemWidgetFactory = ((object item) => new LabelWidget
			{
				Text = ((item != null) ? item.ToString() : string.Empty),
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center
			});
			this.ItemSize = 48f;
		}

		// Token: 0x06001B67 RID: 7015 RVA: 0x000D5E67 File Offset: 0x000D4067
		public void AddItem(object item)
		{
			this.m_items.Add(item);
			this.m_widgetsDirty = true;
		}

		// Token: 0x06001B68 RID: 7016 RVA: 0x000D5E7C File Offset: 0x000D407C
		public void RemoveItem(object item)
		{
			int num = this.m_items.IndexOf(item);
			if (num >= 0)
			{
				this.RemoveItemAt(num);
			}
		}

		// Token: 0x06001B69 RID: 7017 RVA: 0x000D5EA4 File Offset: 0x000D40A4
		public void RemoveItemAt(int index)
		{
			object obj = this.m_items[index];
			this.m_items.RemoveAt(index);
			this.m_widgetsByIndex.Clear();
			this.m_widgetsDirty = true;
			int? selectedIndex = this.SelectedIndex;
			if (index == selectedIndex.GetValueOrDefault() & selectedIndex != null)
			{
				this.SelectedIndex = null;
			}
		}

		// Token: 0x06001B6A RID: 7018 RVA: 0x000D5F08 File Offset: 0x000D4108
		public void ClearItems()
		{
			this.m_items.Clear();
			this.m_widgetsByIndex.Clear();
			this.m_widgetsDirty = true;
			this.SelectedIndex = null;
		}

		// Token: 0x06001B6B RID: 7019 RVA: 0x000D5F44 File Offset: 0x000D4144
		public override float CalculateScrollAreaLength()
		{
			return (float)this.Items.Count * this.ItemSize;
		}

		// Token: 0x06001B6C RID: 7020 RVA: 0x000D5F68 File Offset: 0x000D4168
		public void ScrollToItem(object item)
		{
			int num = this.m_items.IndexOf(item);
			if (num >= 0)
			{
				float num2 = (float)num * this.ItemSize;
				float num3 = (this.Direction == LayoutDirection.Horizontal) ? base.ActualSize.X : base.ActualSize.Y;
				if (num2 < this.ScrollPosition)
				{
					this.ScrollPosition = num2;
					return;
				}
				if (num2 > this.ScrollPosition + num3 - this.ItemSize)
				{
					this.ScrollPosition = num2 - num3 + this.ItemSize;
				}
			}
		}

		// Token: 0x06001B6D RID: 7021 RVA: 0x000D5FE4 File Offset: 0x000D41E4
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			foreach (Widget widget in this.Children)
			{
				if (widget.IsVisible)
				{
					if (this.Direction == LayoutDirection.Horizontal)
					{
						widget.Measure(new Vector2(this.ItemSize, MathUtils.Max(parentAvailableSize.Y - 2f * widget.Margin.Y, 0f)));
					}
					else
					{
						widget.Measure(new Vector2(MathUtils.Max(parentAvailableSize.X - 2f * widget.Margin.X, 0f), this.ItemSize));
					}
				}
			}
			if (this.m_widgetsDirty)
			{
				this.m_widgetsDirty = false;
				this.CreateListWidgets((this.Direction == LayoutDirection.Horizontal) ? base.ActualSize.X : base.ActualSize.Y);
			}
		}

		// Token: 0x06001B6E RID: 7022 RVA: 0x000D60EC File Offset: 0x000D42EC
		public override void ArrangeOverride()
		{
			if (base.ActualSize != this.lastActualSize)
			{
				this.m_widgetsDirty = true;
			}
			this.lastActualSize = base.ActualSize;
			int num = this.m_firstVisibleIndex;
			foreach (Widget widget in this.Children)
			{
				if (this.Direction == LayoutDirection.Horizontal)
				{
					Vector2 vector = new Vector2((float)num * this.ItemSize - this.ScrollPosition, 0f);
					ContainerWidget.ArrangeChildWidgetInCell(vector, vector + new Vector2(this.ItemSize, base.ActualSize.Y), widget);
				}
				else
				{
					Vector2 vector2 = new Vector2(0f, (float)num * this.ItemSize - this.ScrollPosition);
					ContainerWidget.ArrangeChildWidgetInCell(vector2, vector2 + new Vector2(base.ActualSize.X, this.ItemSize), widget);
				}
				num++;
			}
		}

		// Token: 0x06001B6F RID: 7023 RVA: 0x000D61F4 File Offset: 0x000D43F4
		public override void Update()
		{
			bool flag = this.ScrollSpeed != 0f;
			base.Update();
			if (base.Input.Tap != null && base.HitTestPanel(base.Input.Tap.Value))
			{
				this.m_clickAllowed = !flag;
			}
			if (base.Input.Click != null && this.m_clickAllowed && base.HitTestPanel(base.Input.Click.Value.Start) && base.HitTestPanel(base.Input.Click.Value.End))
			{
				int num = this.PositionToItemIndex(base.Input.Click.Value.End);
				if (this.ItemClicked != null && num >= 0 && num < this.m_items.Count)
				{
					this.ItemClicked(this.Items[num]);
				}
				this.SelectedIndex = new int?(num);
				if (this.SelectedIndex != null && this.PlayClickSound)
				{
					AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				}
			}
		}

		// Token: 0x06001B70 RID: 7024 RVA: 0x000D6350 File Offset: 0x000D4550
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.SelectedIndex != null && this.SelectedIndex.Value >= this.m_firstVisibleIndex && this.SelectedIndex.Value <= this.m_lastVisibleIndex)
			{
				Vector2 vector = (this.Direction == LayoutDirection.Horizontal) ? new Vector2((float)this.SelectedIndex.Value * this.ItemSize - this.ScrollPosition, 0f) : new Vector2(0f, (float)this.SelectedIndex.Value * this.ItemSize - this.ScrollPosition);
				FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
				int count = flatBatch2D.TriangleVertices.Count;
				Vector2 v = (this.Direction == LayoutDirection.Horizontal) ? new Vector2(this.ItemSize, base.ActualSize.Y) : new Vector2(base.ActualSize.X, this.ItemSize);
				flatBatch2D.QueueQuad(vector, vector + v, 0f, this.SelectionColor * base.GlobalColorTransform);
				flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
			}
			base.Draw(dc);
		}

		// Token: 0x06001B71 RID: 7025 RVA: 0x000D648C File Offset: 0x000D468C
		public int PositionToItemIndex(Vector2 position)
		{
			Vector2 vector = base.ScreenToWidget(position);
			if (this.Direction == LayoutDirection.Horizontal)
			{
				return (int)((vector.X + this.ScrollPosition) / this.ItemSize);
			}
			return (int)((vector.Y + this.ScrollPosition) / this.ItemSize);
		}

		// Token: 0x06001B72 RID: 7026 RVA: 0x000D64D4 File Offset: 0x000D46D4
		public void CreateListWidgets(float size)
		{
			this.Children.Clear();
			if (this.m_items.Count <= 0)
			{
				return;
			}
			int x = (int)MathUtils.Floor(this.ScrollPosition / this.ItemSize);
			int x2 = (int)MathUtils.Floor((this.ScrollPosition + size) / this.ItemSize);
			this.m_firstVisibleIndex = MathUtils.Max(x, 0);
			this.m_lastVisibleIndex = MathUtils.Min(x2, this.m_items.Count - 1);
			for (int i = this.m_firstVisibleIndex; i <= this.m_lastVisibleIndex; i++)
			{
				object obj = this.m_items[i];
				Widget widget;
				if (!this.m_widgetsByIndex.TryGetValue(i, out widget))
				{
					widget = this.ItemWidgetFactory(obj);
					widget.Tag = obj;
					this.m_widgetsByIndex.Add(i, widget);
				}
				this.Children.Add(widget);
			}
		}

		// Token: 0x04001289 RID: 4745
		public List<object> m_items = new List<object>();

		// Token: 0x0400128A RID: 4746
		public int? m_selectedItemIndex;

		// Token: 0x0400128B RID: 4747
		public Dictionary<int, Widget> m_widgetsByIndex = new Dictionary<int, Widget>();

		// Token: 0x0400128C RID: 4748
		public int m_firstVisibleIndex;

		// Token: 0x0400128D RID: 4749
		public int m_lastVisibleIndex;

		// Token: 0x0400128E RID: 4750
		public bool PlayClickSound = true;

		// Token: 0x0400128F RID: 4751
		public float m_itemSize;

		// Token: 0x04001290 RID: 4752
		public bool m_widgetsDirty;

		// Token: 0x04001291 RID: 4753
		public bool m_clickAllowed;

		// Token: 0x04001292 RID: 4754
		public Vector2 lastActualSize = new Vector2(-1f);
	}
}
