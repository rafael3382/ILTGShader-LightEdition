using System;

namespace Game
{
	// Token: 0x020002AD RID: 685
	public interface IEditableItemData
	{
		// Token: 0x06001531 RID: 5425
		IEditableItemData Copy();

		// Token: 0x06001532 RID: 5426
		void LoadString(string data);

		// Token: 0x06001533 RID: 5427
		string SaveString();
	}
}
