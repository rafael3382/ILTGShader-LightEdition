using System;
using System.Collections;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x020002BD RID: 701
	public class ListSelectionDialog : Dialog
	{
		// Token: 0x17000338 RID: 824
		// (get) Token: 0x0600157D RID: 5501 RVA: 0x000A1FB0 File Offset: 0x000A01B0
		// (set) Token: 0x0600157E RID: 5502 RVA: 0x000A1FBD File Offset: 0x000A01BD
		public Vector2 ContentSize
		{
			get
			{
				return this.m_contentWidget.Size;
			}
			set
			{
				this.m_contentWidget.Size = value;
			}
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x000A1FCC File Offset: 0x000A01CC
		public ListSelectionDialog(string title, IEnumerable items, float itemSize, Func<object, Widget> itemWidgetFactory, Action<object> selectionHandler)
		{
			this.m_selectionHandler = selectionHandler;
			XElement node = ContentManager.Get<XElement>("Dialogs/ListSelectionDialog", null);
			base.LoadContents(this, node);
			this.m_titleLabelWidget = this.Children.Find<LabelWidget>("ListSelectionDialog.Title", true);
			this.m_listWidget = this.Children.Find<ListPanelWidget>("ListSelectionDialog.List", true);
			this.m_contentWidget = this.Children.Find<CanvasWidget>("ListSelectionDialog.Content", true);
			this.m_titleLabelWidget.Text = title;
			this.m_titleLabelWidget.IsVisible = !string.IsNullOrEmpty(title);
			this.m_listWidget.ItemSize = itemSize;
			if (itemWidgetFactory != null)
			{
				this.m_listWidget.ItemWidgetFactory = itemWidgetFactory;
			}
			foreach (object item in items)
			{
				this.m_listWidget.AddItem(item);
			}
			for (int i = this.m_listWidget.Items.Count; i >= 0; i--)
			{
				float num = MathUtils.Min((float)i + 0.5f, (float)this.m_listWidget.Items.Count);
				if (num * itemSize <= this.m_contentWidget.Size.Y)
				{
					this.m_contentWidget.Size = new Vector2(this.m_contentWidget.Size.X, num * itemSize);
					return;
				}
			}
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x000A2144 File Offset: 0x000A0344
		public ListSelectionDialog(string title, IEnumerable items, float itemSize, Func<object, string> itemToStringConverter, Action<object> selectionHandler) : this(title, items, itemSize, (object item) => new LabelWidget
		{
			Text = itemToStringConverter(item),
			HorizontalAlignment = WidgetAlignment.Center,
			VerticalAlignment = WidgetAlignment.Center
		}, selectionHandler)
		{
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x000A2178 File Offset: 0x000A0378
		public override void Update()
		{
			if (base.Input.Back || base.Input.Cancel)
			{
				this.m_dismissTime = new double?(0.0);
			}
			else if (base.Input.Tap != null && !this.m_listWidget.HitTest(base.Input.Tap.Value))
			{
				this.m_dismissTime = new double?(0.0);
			}
			else if (this.m_dismissTime == null && this.m_listWidget.SelectedItem != null)
			{
				this.m_dismissTime = new double?(Time.FrameStartTime + 0.05000000074505806);
			}
			if (this.m_dismissTime != null && Time.FrameStartTime >= this.m_dismissTime.Value)
			{
				this.Dismiss(this.m_listWidget.SelectedItem);
			}
		}

		// Token: 0x06001582 RID: 5506 RVA: 0x000A2264 File Offset: 0x000A0464
		public void Dismiss(object result)
		{
			if (!this.m_isDismissed)
			{
				this.m_isDismissed = true;
				DialogsManager.HideDialog(this);
				if (this.m_selectionHandler != null && result != null)
				{
					this.m_selectionHandler(result);
				}
			}
		}

		// Token: 0x04000E02 RID: 3586
		public Action<object> m_selectionHandler;

		// Token: 0x04000E03 RID: 3587
		public LabelWidget m_titleLabelWidget;

		// Token: 0x04000E04 RID: 3588
		public ListPanelWidget m_listWidget;

		// Token: 0x04000E05 RID: 3589
		public CanvasWidget m_contentWidget;

		// Token: 0x04000E06 RID: 3590
		public double? m_dismissTime;

		// Token: 0x04000E07 RID: 3591
		public bool m_isDismissed;
	}
}
