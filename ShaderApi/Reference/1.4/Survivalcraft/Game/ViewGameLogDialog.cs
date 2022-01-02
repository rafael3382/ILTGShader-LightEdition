using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Engine;
using Engine.Media;
using SimpleJson;

namespace Game
{
	// Token: 0x02000358 RID: 856
	public class ViewGameLogDialog : Dialog
	{
		// Token: 0x0600190E RID: 6414 RVA: 0x000C49B0 File Offset: 0x000C2BB0
		public ViewGameLogDialog()
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/ViewGameLogDialog", null);
			base.LoadContents(this, node);
			this.m_listPanel = this.Children.Find<ListPanelWidget>("ViewGameLogDialog.ListPanel", true);
			this.m_copyButton = this.Children.Find<ButtonWidget>("ViewGameLogDialog.CopyButton", true);
			this.m_filterButton = this.Children.Find<ButtonWidget>("ViewGameLogDialog.FilterButton", true);
			this.m_filterButton.Style = ContentManager.Get<XElement>("Styles/ButtonStyle_160x60", null);
			this.m_closeButton = this.Children.Find<ButtonWidget>("ViewGameLogDialog.CloseButton", true);
			this.m_uploadButton = this.Children.Find<ButtonWidget>("ViewGameLogDialog.UploadButton", true);
			ListPanelWidget listPanel = this.m_listPanel;
			listPanel.ItemClicked = (Action<object>)Delegate.Combine(listPanel.ItemClicked, new Action<object>(delegate(object item)
			{
				if (this.m_listPanel.SelectedItem == item)
				{
					DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog("Log Item", item.ToString(), "OK", null, null));
				}
			}));
			this.PopulateList();
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x000C4A90 File Offset: 0x000C2C90
		public override void Update()
		{
			if (this.m_copyButton.IsClicked)
			{
				ClipboardManager.ClipboardString = GameLogSink.GetRecentLog(131072);
			}
			if (this.m_filterButton.IsClicked)
			{
				if (this.m_filter < LogType.Warning)
				{
					this.m_filter = LogType.Warning;
				}
				else
				{
					this.m_filter = ((this.m_filter < LogType.Error) ? LogType.Error : LogType.Debug);
				}
				this.PopulateList();
			}
			if (base.Input.Cancel || this.m_closeButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
			if (this.m_filter == LogType.Debug)
			{
				this.m_filterButton.Text = "All";
			}
			else if (this.m_filter == LogType.Warning)
			{
				this.m_filterButton.Text = "Warnings";
			}
			else if (this.m_filter == LogType.Error)
			{
				this.m_filterButton.Text = "Errors";
			}
			if (this.m_uploadButton.IsClicked)
			{
				if (string.IsNullOrEmpty(SettingsManager.ScpboxAccessToken))
				{
					MessageDialog dialog2 = new MessageDialog(LanguageControl.Get(ViewGameLogDialog.fName, 1), LanguageControl.Get(ViewGameLogDialog.fName, 2), LanguageControl.Get(ViewGameLogDialog.fName, 3), LanguageControl.Get(ViewGameLogDialog.fName, 4), delegate(MessageDialogButton btn)
					{
						DialogsManager.HideAllDialogs();
					});
					DialogsManager.ShowDialog(this, dialog2);
					return;
				}
				CancellableProgress progress = new CancellableProgress();
				CancellableBusyDialog dialog = new CancellableBusyDialog(LanguageControl.Get(ViewGameLogDialog.fName, 5), true);
				DialogsManager.ShowDialog(this, dialog);
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", "/GameLog/" + DateTime.Now.Ticks.ToString() + ".log");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Authorization", "Bearer " + SettingsManager.ScpboxAccessToken);
				dictionary.Add("Content-Type", "application/octet-stream");
				dictionary.Add("Dropbox-API-Arg", jsonObject.ToString());
				MemoryStream memoryStream = new MemoryStream();
				GameLogSink.m_stream.Seek(0L, SeekOrigin.Begin);
				GameLogSink.m_stream.CopyTo(memoryStream);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				WebManager.Post("https://m.schub.top/com/files/upload", null, dictionary, memoryStream, progress, delegate
				{
					dialog.LargeMessage = LanguageControl.Get(ViewGameLogDialog.fName, 6);
					dialog.m_cancelButtonWidget.Text = "OK";
					GameLogSink.m_writer.BaseStream.SetLength(0L);
					GameLogSink.m_writer.Flush();
					this.PopulateList();
				}, delegate(Exception error)
				{
					dialog.LargeMessage = LanguageControl.Get(ViewGameLogDialog.fName, 7);
					dialog.SmallMessage = error.Message;
				});
			}
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x000C4CDC File Offset: 0x000C2EDC
		public void PopulateList()
		{
			this.m_listPanel.ItemWidgetFactory = delegate(object item)
			{
				string text2 = (item != null) ? item.ToString() : string.Empty;
				Color color = Color.Gray;
				if (text2.Contains("ERROR:"))
				{
					color = Color.Red;
				}
				else if (text2.Contains("WARNING:"))
				{
					color = Color.DarkYellow;
				}
				else if (text2.Contains("INFO:"))
				{
					color = Color.LightGray;
				}
				return new LabelWidget
				{
					Text = text2,
					Font = BitmapFont.DebugFont,
					HorizontalAlignment = WidgetAlignment.Near,
					VerticalAlignment = WidgetAlignment.Center,
					FontScale = 0.6f,
					Color = color
				};
			};
			List<string> recentLogLines = GameLogSink.GetRecentLogLines(131072);
			this.m_listPanel.ClearItems();
			if (recentLogLines.Count > 1000)
			{
				recentLogLines.RemoveRange(0, recentLogLines.Count - 1000);
			}
			foreach (string text in recentLogLines)
			{
				if (this.m_filter == LogType.Warning)
				{
					if (!text.Contains("WARNING:") && !text.Contains("ERROR:"))
					{
						continue;
					}
				}
				else if (this.m_filter == LogType.Error && !text.Contains("ERROR:"))
				{
					continue;
				}
				this.m_listPanel.AddItem(text);
			}
			this.m_listPanel.ScrollPosition = (float)this.m_listPanel.Items.Count * this.m_listPanel.ItemSize;
		}

		// Token: 0x04001122 RID: 4386
		public ListPanelWidget m_listPanel;

		// Token: 0x04001123 RID: 4387
		public ButtonWidget m_copyButton;

		// Token: 0x04001124 RID: 4388
		public ButtonWidget m_filterButton;

		// Token: 0x04001125 RID: 4389
		public ButtonWidget m_closeButton;

		// Token: 0x04001126 RID: 4390
		public ButtonWidget m_uploadButton;

		// Token: 0x04001127 RID: 4391
		public LogType m_filter;

		// Token: 0x04001128 RID: 4392
		public static string fName = "ViewGameLogDialog";
	}
}
