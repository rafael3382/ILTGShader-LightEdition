using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
	// Token: 0x02000392 RID: 914
	public class LoginDialog : Dialog
	{
		// Token: 0x06001B73 RID: 7027 RVA: 0x000D65B0 File Offset: 0x000D47B0
		public LoginDialog()
		{
			CanvasWidget canvasWidget = new CanvasWidget
			{
				Size = new Vector2(600f, 240f),
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center
			};
			RectangleWidget widget = new RectangleWidget
			{
				FillColor = new Color(0, 0, 0, 255),
				OutlineColor = new Color(128, 128, 128, 128),
				OutlineThickness = 2f
			};
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Near,
				Margin = new Vector2(10f, 10f)
			};
			this.Children.Add(canvasWidget);
			canvasWidget.Children.Add(widget);
			canvasWidget.Children.Add(stackPanelWidget);
			this.MainView = stackPanelWidget;
			this.MainView.Children.Add(this.tip);
			this.MainView.Children.Add(this.makeTextBox("用户名:"));
			this.MainView.Children.Add(this.makeTextBox("密  码:"));
			this.MainView.Children.Add(this.makeButton());
		}

		// Token: 0x06001B74 RID: 7028 RVA: 0x000D6730 File Offset: 0x000D4930
		public Widget makeTextBox(string title)
		{
			CanvasWidget canvasWidget = new CanvasWidget();
			canvasWidget.Margin = new Vector2(10f, 0f);
			RectangleWidget widget = new RectangleWidget
			{
				FillColor = Color.Black,
				OutlineColor = Color.White,
				Size = new Vector2(float.PositiveInfinity, 80f)
			};
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal
			};
			LabelWidget widget2 = new LabelWidget
			{
				HorizontalAlignment = WidgetAlignment.Near,
				VerticalAlignment = WidgetAlignment.Near,
				Text = title,
				Margin = new Vector2(1f, 1f)
			};
			TextBoxWidget widget3 = new TextBoxWidget
			{
				VerticalAlignment = WidgetAlignment.Center,
				HorizontalAlignment = WidgetAlignment.Stretch,
				Color = new Color(255, 255, 255),
				Margin = new Vector2(4f, 0f),
				Size = new Vector2(float.PositiveInfinity, 80f)
			};
			if (title == "用户名:")
			{
				this.txa = widget3;
			}
			if (title == "密  码:")
			{
				this.txb = widget3;
			}
			stackPanelWidget.Children.Add(widget2);
			stackPanelWidget.Children.Add(widget3);
			canvasWidget.Children.Add(widget);
			canvasWidget.Children.Add(stackPanelWidget);
			return canvasWidget;
		}

		// Token: 0x06001B75 RID: 7029 RVA: 0x000D6878 File Offset: 0x000D4A78
		public Widget makeButton()
		{
			StackPanelWidget stackPanelWidget = new StackPanelWidget();
			stackPanelWidget.Direction = LayoutDirection.Horizontal;
			BevelledButtonWidget widget = new BevelledButtonWidget
			{
				Size = new Vector2(160f, 60f),
				Margin = new Vector2(4f, 0f),
				Text = "登陆"
			};
			BevelledButtonWidget widget2 = new BevelledButtonWidget
			{
				Size = new Vector2(160f, 60f),
				Margin = new Vector2(4f, 0f),
				Text = "注册"
			};
			BevelledButtonWidget widget3 = new BevelledButtonWidget
			{
				Size = new Vector2(160f, 60f),
				Margin = new Vector2(4f, 0f),
				Text = "取消"
			};
			stackPanelWidget.Children.Add(widget);
			stackPanelWidget.Children.Add(widget2);
			stackPanelWidget.Children.Add(widget3);
			this.btna = widget;
			this.btnb = widget2;
			this.btnc = widget3;
			return stackPanelWidget;
		}

		// Token: 0x06001B76 RID: 7030 RVA: 0x000D697C File Offset: 0x000D4B7C
		public override void Update()
		{
			if (this.btna.IsClicked)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("user", this.txa.Text);
				dictionary.Add("pass", this.txb.Text);
				DialogsManager.ShowDialog(this, this.busyDialog);
				WebManager.Post("https://m.schub.top/com/api/login", dictionary, null, new MemoryStream(), new CancellableProgress(), this.succ, this.fail);
			}
			if (this.btnb.IsClicked)
			{
				WebBrowserManager.LaunchBrowser("https://m.schub.top/com/reg");
			}
			if (this.btnc.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04001297 RID: 4759
		public Action<byte[]> succ;

		// Token: 0x04001298 RID: 4760
		public Action<Exception> fail;

		// Token: 0x04001299 RID: 4761
		public StackPanelWidget MainView;

		// Token: 0x0400129A RID: 4762
		public BevelledButtonWidget btna;

		// Token: 0x0400129B RID: 4763
		public BevelledButtonWidget btnb;

		// Token: 0x0400129C RID: 4764
		public BevelledButtonWidget btnc;

		// Token: 0x0400129D RID: 4765
		public TextBoxWidget txa;

		// Token: 0x0400129E RID: 4766
		public TextBoxWidget txb;

		// Token: 0x0400129F RID: 4767
		public BusyDialog busyDialog = new BusyDialog("提示", "登陆中");

		// Token: 0x040012A0 RID: 4768
		public LabelWidget tip = new LabelWidget
		{
			HorizontalAlignment = WidgetAlignment.Near,
			VerticalAlignment = WidgetAlignment.Near,
			Margin = new Vector2(1f, 1f)
		};
	}
}
