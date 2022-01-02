using System;
using System.IO;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000273 RID: 627
	public class DownloadContentFromLinkDialog : Dialog
	{
		// Token: 0x0600141F RID: 5151 RVA: 0x000961A0 File Offset: 0x000943A0
		public DownloadContentFromLinkDialog()
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/DownloadContentFromLinkDialog", null);
			base.LoadContents(this, node);
			this.m_linkTextBoxWidget = this.Children.Find<TextBoxWidget>("DownloadContentFromLinkDialog.Link", true);
			this.m_nameTextBoxWidget = this.Children.Find<TextBoxWidget>("DownloadContentFromLinkDialog.Name", true);
			this.m_typeIconWidget = this.Children.Find<RectangleWidget>("DownloadContentFromLinkDialog.TypeIcon", true);
			this.m_typeLabelWidget = this.Children.Find<LabelWidget>("DownloadContentFromLinkDialog.Type", true);
			this.m_changeTypeButtonWidget = this.Children.Find<ButtonWidget>("DownloadContentFromLinkDialog.ChangeType", true);
			this.m_downloadButtonWidget = this.Children.Find<ButtonWidget>("DownloadContentFromLinkDialog.Download", true);
			this.m_cancelButtonWidget = this.Children.Find<ButtonWidget>("DownloadContentFromLinkDialog.Cancel", true);
			this.m_linkTextBoxWidget.TextChanged += delegate(TextBoxWidget <p0>)
			{
				this.m_updateContentName = true;
				this.m_updateContentType = true;
			};
		}

		// Token: 0x06001420 RID: 5152 RVA: 0x00096280 File Offset: 0x00094480
		public override void Update()
		{
			string text = this.m_linkTextBoxWidget.Text.Trim();
			string name = this.m_nameTextBoxWidget.Text.Trim();
			this.m_typeLabelWidget.Text = ExternalContentManager.GetEntryTypeDescription(this.m_type);
			this.m_typeIconWidget.Subtexture = ExternalContentManager.GetEntryTypeIcon(this.m_type);
			if (ExternalContentManager.DoesEntryTypeRequireName(this.m_type))
			{
				this.m_nameTextBoxWidget.IsEnabled = true;
				this.m_downloadButtonWidget.IsEnabled = (text.Length > 0 && name.Length > 0 && this.m_type > ExternalContentType.Unknown);
				if (this.m_updateContentName)
				{
					this.m_nameTextBoxWidget.Text = DownloadContentFromLinkDialog.GetNameFromLink(this.m_linkTextBoxWidget.Text);
					this.m_updateContentName = false;
				}
			}
			else
			{
				this.m_nameTextBoxWidget.IsEnabled = false;
				this.m_nameTextBoxWidget.Text = string.Empty;
				this.m_downloadButtonWidget.IsEnabled = (text.Length > 0 && this.m_type > ExternalContentType.Unknown);
			}
			if (this.m_updateContentType)
			{
				this.m_type = DownloadContentFromLinkDialog.GetTypeFromLink(this.m_linkTextBoxWidget.Text);
				this.m_updateContentType = false;
			}
			if (this.m_changeTypeButtonWidget.IsClicked)
			{
				DialogsManager.ShowDialog(base.ParentWidget, new SelectExternalContentTypeDialog("Select Content Type", delegate(ExternalContentType item)
				{
					this.m_type = item;
					this.m_updateContentName = true;
				}));
				return;
			}
			if (base.Input.Cancel || this.m_cancelButtonWidget.IsClicked)
			{
				DialogsManager.HideDialog(this);
				return;
			}
			if (this.m_downloadButtonWidget.IsClicked)
			{
				CancellableBusyDialog busyDialog = new CancellableBusyDialog("Downloading", false);
				DialogsManager.ShowDialog(base.ParentWidget, busyDialog);
				WebManager.Get(text, null, null, busyDialog.Progress, delegate(byte[] data)
				{
					ExternalContentManager.ImportExternalContent(new MemoryStream(data), this.m_type, name, delegate
					{
						DialogsManager.HideDialog(busyDialog);
						DialogsManager.HideDialog(this);
					}, delegate(Exception error)
					{
						DialogsManager.HideDialog(busyDialog);
						DialogsManager.ShowDialog(this.ParentWidget, new MessageDialog("Error", error.Message, "OK", null, null));
					});
				}, delegate(Exception error)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(this.ParentWidget, new MessageDialog("Error", error.Message, "OK", null, null));
				});
			}
		}

		// Token: 0x06001421 RID: 5153 RVA: 0x00096478 File Offset: 0x00094678
		public static string UnclutterLink(string address)
		{
			string result;
			try
			{
				string text = address;
				int num = text.IndexOf('&');
				if (num > 0)
				{
					text = text.Remove(num);
				}
				int num2 = text.IndexOf('?');
				if (num2 > 0)
				{
					text = text.Remove(num2);
				}
				result = Uri.UnescapeDataString(text);
			}
			catch (Exception)
			{
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x06001422 RID: 5154 RVA: 0x000964D4 File Offset: 0x000946D4
		public static string GetNameFromLink(string address)
		{
			string result;
			try
			{
				result = Storage.GetFileNameWithoutExtension(DownloadContentFromLinkDialog.UnclutterLink(address));
			}
			catch (Exception)
			{
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x06001423 RID: 5155 RVA: 0x0009650C File Offset: 0x0009470C
		public static ExternalContentType GetTypeFromLink(string address)
		{
			ExternalContentType result;
			try
			{
				result = ExternalContentManager.ExtensionToType(Storage.GetExtension(DownloadContentFromLinkDialog.UnclutterLink(address)));
			}
			catch (Exception)
			{
				result = ExternalContentType.Unknown;
			}
			return result;
		}

		// Token: 0x04000CB7 RID: 3255
		public TextBoxWidget m_linkTextBoxWidget;

		// Token: 0x04000CB8 RID: 3256
		public TextBoxWidget m_nameTextBoxWidget;

		// Token: 0x04000CB9 RID: 3257
		public RectangleWidget m_typeIconWidget;

		// Token: 0x04000CBA RID: 3258
		public LabelWidget m_typeLabelWidget;

		// Token: 0x04000CBB RID: 3259
		public ButtonWidget m_changeTypeButtonWidget;

		// Token: 0x04000CBC RID: 3260
		public ButtonWidget m_downloadButtonWidget;

		// Token: 0x04000CBD RID: 3261
		public ButtonWidget m_cancelButtonWidget;

		// Token: 0x04000CBE RID: 3262
		public bool m_updateContentName;

		// Token: 0x04000CBF RID: 3263
		public bool m_updateContentType;

		// Token: 0x04000CC0 RID: 3264
		public ExternalContentType m_type;
	}
}
