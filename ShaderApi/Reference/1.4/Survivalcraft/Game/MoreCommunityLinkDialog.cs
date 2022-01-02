using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x020002C8 RID: 712
	public class MoreCommunityLinkDialog : Dialog
	{
		// Token: 0x060015B2 RID: 5554 RVA: 0x000A33D0 File Offset: 0x000A15D0
		public MoreCommunityLinkDialog()
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/MoreCommunityLinkDialog", null);
			base.LoadContents(this, node);
			this.m_userLabel = this.Children.Find<LabelWidget>("MoreCommunityLinkDialog.User", true);
			this.m_changeUserButton = this.Children.Find<ButtonWidget>("MoreCommunityLinkDialog.ChangeUser", true);
			this.m_userIdLabel = this.Children.Find<LabelWidget>("MoreCommunityLinkDialog.UserId", true);
			this.m_copyUserIdButton = this.Children.Find<ButtonWidget>("MoreCommunityLinkDialog.CopyUserId", true);
			this.m_publishButton = this.Children.Find<ButtonWidget>("MoreCommunityLinkDialog.Publish", true);
			this.m_closeButton = this.Children.Find<ButtonWidget>("MoreCommunityLinkDialog.Close", true);
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x000A3484 File Offset: 0x000A1684
		public override void Update()
		{
			string text = (UserManager.ActiveUser != null) ? UserManager.ActiveUser.DisplayName : "No User";
			if (text.Length > 15)
			{
				text = text.Substring(0, 15) + "...";
			}
			this.m_userLabel.Text = text;
			string text2 = (UserManager.ActiveUser != null) ? UserManager.ActiveUser.UniqueId : "No User";
			if (text2.Length > 15)
			{
				text2 = text2.Substring(0, 15) + "...";
			}
			this.m_userIdLabel.Text = text2;
			this.m_publishButton.IsEnabled = (UserManager.ActiveUser != null);
			this.m_copyUserIdButton.IsEnabled = (UserManager.ActiveUser != null);
			if (this.m_changeUserButton.IsClicked)
			{
				DialogsManager.ShowDialog(base.ParentWidget, new ListSelectionDialog("Select Active User", UserManager.GetUsers(), 60f, (object item) => ((UserInfo)item).DisplayName, delegate(object item)
				{
					UserManager.ActiveUser = (UserInfo)item;
				}));
			}
			if (this.m_copyUserIdButton.IsClicked && UserManager.ActiveUser != null)
			{
				ClipboardManager.ClipboardString = UserManager.ActiveUser.UniqueId;
			}
			if (this.m_publishButton.IsClicked && UserManager.ActiveUser != null)
			{
				DialogsManager.ShowDialog(base.ParentWidget, new PublishCommunityLinkDialog(UserManager.ActiveUser.UniqueId, null, null));
			}
			if (base.Input.Cancel || this.m_closeButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04000E37 RID: 3639
		public LabelWidget m_userLabel;

		// Token: 0x04000E38 RID: 3640
		public ButtonWidget m_changeUserButton;

		// Token: 0x04000E39 RID: 3641
		public LabelWidget m_userIdLabel;

		// Token: 0x04000E3A RID: 3642
		public ButtonWidget m_copyUserIdButton;

		// Token: 0x04000E3B RID: 3643
		public ButtonWidget m_publishButton;

		// Token: 0x04000E3C RID: 3644
		public ButtonWidget m_closeButton;
	}
}
