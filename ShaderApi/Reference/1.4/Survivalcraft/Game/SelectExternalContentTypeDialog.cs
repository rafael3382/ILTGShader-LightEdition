using System;
using System.Linq;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x020002FF RID: 767
	public class SelectExternalContentTypeDialog : ListSelectionDialog
	{
		// Token: 0x060016B1 RID: 5809 RVA: 0x000AAC00 File Offset: 0x000A8E00
		public SelectExternalContentTypeDialog(string title, Action<ExternalContentType> selectionHandler) : base(title, from v in EnumUtils.GetEnumValues(typeof(ExternalContentType))
		where ExternalContentManager.IsEntryTypeDownloadSupported((ExternalContentType)v)
		select v, 64f, delegate(object item)
		{
			ExternalContentType type = (ExternalContentType)item;
			XElement node = ContentManager.Get<XElement>("Widgets/SelectExternalContentTypeItem", null);
			ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(null, node, null);
			containerWidget.Children.Find<RectangleWidget>("SelectExternalContentType.Icon", true).Subtexture = ExternalContentManager.GetEntryTypeIcon(type);
			containerWidget.Children.Find<LabelWidget>("SelectExternalContentType.Text", true).Text = ExternalContentManager.GetEntryTypeDescription(type);
			return containerWidget;
		}, delegate(object item)
		{
			selectionHandler((ExternalContentType)item);
		})
		{
		}
	}
}
