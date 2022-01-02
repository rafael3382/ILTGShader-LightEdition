using System;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x020002FE RID: 766
	public class SelectExternalContentProviderDialog : ListSelectionDialog
	{
		// Token: 0x060016B0 RID: 5808 RVA: 0x000AAB70 File Offset: 0x000A8D70
		public SelectExternalContentProviderDialog(string title, bool listingSupportRequired, Action<IExternalContentProvider> selectionHandler) : base(title, from p in ExternalContentManager.Providers
		where !listingSupportRequired || p.SupportsListing
		select p, 100f, delegate(object item)
		{
			IExternalContentProvider externalContentProvider = (IExternalContentProvider)item;
			XElement node = ContentManager.Get<XElement>("Widgets/SelectExternalContentProviderItem", null);
			ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(null, node, null);
			containerWidget.Children.Find<LabelWidget>("SelectExternalContentProvider.Text", true).Text = externalContentProvider.DisplayName;
			containerWidget.Children.Find<LabelWidget>("SelectExternalContentProvider.Details", true).Text = externalContentProvider.Description;
			return containerWidget;
		}, delegate(object item)
		{
			selectionHandler((IExternalContentProvider)item);
		})
		{
			base.ContentSize = new Vector2(700f, base.ContentSize.Y);
		}
	}
}
