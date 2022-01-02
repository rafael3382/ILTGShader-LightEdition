using System;
using Engine;

namespace Game
{
	// Token: 0x02000381 RID: 897
	public class DragHostWidget : ContainerWidget
	{
		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06001A98 RID: 6808 RVA: 0x000CF227 File Offset: 0x000CD427
		public bool IsDragInProgress
		{
			get
			{
				return this.m_dragWidget != null;
			}
		}

		// Token: 0x06001A99 RID: 6809 RVA: 0x000CF232 File Offset: 0x000CD432
		public DragHostWidget()
		{
			this.IsHitTestVisible = false;
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x000CF241 File Offset: 0x000CD441
		public void BeginDrag(Widget dragWidget, object dragData, Action dragEndedHandler)
		{
			if (this.m_dragWidget == null)
			{
				this.m_dragWidget = dragWidget;
				this.m_dragData = dragData;
				this.m_dragEndedHandler = dragEndedHandler;
				this.Children.Add(this.m_dragWidget);
				this.UpdateDragPosition();
			}
		}

		// Token: 0x06001A9B RID: 6811 RVA: 0x000CF278 File Offset: 0x000CD478
		public void EndDrag()
		{
			if (this.m_dragWidget != null)
			{
				this.Children.Remove(this.m_dragWidget);
				this.m_dragWidget = null;
				this.m_dragData = null;
				if (this.m_dragEndedHandler != null)
				{
					this.m_dragEndedHandler();
					this.m_dragEndedHandler = null;
				}
			}
		}

		// Token: 0x06001A9C RID: 6812 RVA: 0x000CF2C8 File Offset: 0x000CD4C8
		public override void Update()
		{
			if (this.m_dragWidget != null)
			{
				this.UpdateDragPosition();
				IDragTargetWidget dragTargetWidget = base.HitTestGlobal(this.m_dragPosition, (Widget w) => w is IDragTargetWidget) as IDragTargetWidget;
				if (base.Input.Drag != null)
				{
					if (dragTargetWidget != null)
					{
						dragTargetWidget.DragOver(this.m_dragWidget, this.m_dragData);
						return;
					}
				}
				else
				{
					try
					{
						if (dragTargetWidget != null)
						{
							dragTargetWidget.DragDrop(this.m_dragWidget, this.m_dragData);
						}
					}
					finally
					{
						this.EndDrag();
					}
				}
			}
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x000CF374 File Offset: 0x000CD574
		public override void ArrangeOverride()
		{
			foreach (Widget widget in this.Children)
			{
				Vector2 parentDesiredSize = widget.ParentDesiredSize;
				parentDesiredSize.X = MathUtils.Min(parentDesiredSize.X, base.ActualSize.X);
				parentDesiredSize.Y = MathUtils.Min(parentDesiredSize.Y, base.ActualSize.Y);
				widget.Arrange(base.ScreenToWidget(this.m_dragPosition) - 0.5f * parentDesiredSize, parentDesiredSize);
			}
		}

		// Token: 0x06001A9E RID: 6814 RVA: 0x000CF424 File Offset: 0x000CD624
		public void UpdateDragPosition()
		{
			if (base.Input.Drag != null)
			{
				this.m_dragPosition = base.Input.Drag.Value;
				this.m_dragPosition.X = MathUtils.Clamp(this.m_dragPosition.X, base.GlobalBounds.Min.X, base.GlobalBounds.Max.X - 1f);
				this.m_dragPosition.Y = MathUtils.Clamp(this.m_dragPosition.Y, base.GlobalBounds.Min.Y, base.GlobalBounds.Max.Y - 1f);
			}
		}

		// Token: 0x04001223 RID: 4643
		public Widget m_dragWidget;

		// Token: 0x04001224 RID: 4644
		public object m_dragData;

		// Token: 0x04001225 RID: 4645
		public Action m_dragEndedHandler;

		// Token: 0x04001226 RID: 4646
		public Vector2 m_dragPosition;
	}
}
