using System;

namespace Game
{
	// Token: 0x0200038D RID: 909
	public interface IDragTargetWidget
	{
		// Token: 0x06001B18 RID: 6936
		void DragOver(Widget dragWidget, object data);

		// Token: 0x06001B19 RID: 6937
		void DragDrop(Widget dragWidget, object data);
	}
}
