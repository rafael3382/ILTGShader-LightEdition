using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x020002F4 RID: 756
	public class RateCommunityContentDialog : Dialog
	{
		// Token: 0x06001697 RID: 5783 RVA: 0x000A9CFC File Offset: 0x000A7EFC
		public RateCommunityContentDialog(string address, string displayName, string userId)
		{
			this.m_address = address;
			this.m_displayName = displayName;
			this.m_userId = userId;
			XElement node = ContentManager.Get<XElement>("Dialogs/RateCommunityContentDialog", null);
			base.LoadContents(this, node);
			this.m_nameLabel = this.Children.Find<LabelWidget>("RateCommunityContentDialog.Name", true);
			this.m_starRating = this.Children.Find<StarRatingWidget>("RateCommunityContentDialog.StarRating", true);
			this.m_rateButton = this.Children.Find<ButtonWidget>("RateCommunityContentDialog.Rate", true);
			this.m_reportLink = this.Children.Find<LinkWidget>("RateCommunityContentDialog.Report", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("RateCommunityContentDialog.Cancel", true);
			this.m_nameLabel.Text = displayName;
			this.m_rateButton.IsEnabled = false;
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x000A9DC4 File Offset: 0x000A7FC4
		public override void Update()
		{
			this.m_rateButton.IsEnabled = (this.m_starRating.Rating != 0f);
			if (this.m_rateButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
				CancellableBusyDialog busyDialog = new CancellableBusyDialog("Sending Rating", false);
				DialogsManager.ShowDialog(base.ParentWidget, busyDialog);
				CommunityContentManager.Rate(this.m_address, this.m_userId, (int)this.m_starRating.Rating, busyDialog.Progress, delegate
				{
					DialogsManager.HideDialog(busyDialog);
				}, delegate
				{
					DialogsManager.HideDialog(busyDialog);
				});
			}
			if (this.m_reportLink.IsClicked && UserManager.ActiveUser != null)
			{
				DialogsManager.HideDialog(this);
				DialogsManager.ShowDialog(base.ParentWidget, new ReportCommunityContentDialog(this.m_address, this.m_displayName, UserManager.ActiveUser.UniqueId));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04000F47 RID: 3911
		public string m_address;

		// Token: 0x04000F48 RID: 3912
		public string m_displayName;

		// Token: 0x04000F49 RID: 3913
		public string m_userId;

		// Token: 0x04000F4A RID: 3914
		public LabelWidget m_nameLabel;

		// Token: 0x04000F4B RID: 3915
		public StarRatingWidget m_starRating;

		// Token: 0x04000F4C RID: 3916
		public ButtonWidget m_rateButton;

		// Token: 0x04000F4D RID: 3917
		public LinkWidget m_reportLink;

		// Token: 0x04000F4E RID: 3918
		public ButtonWidget m_cancelButton;
	}
}
