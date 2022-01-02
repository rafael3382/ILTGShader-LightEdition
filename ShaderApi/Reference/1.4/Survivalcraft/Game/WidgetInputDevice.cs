using System;

namespace Game
{
	// Token: 0x020003AC RID: 940
	[Flags]
	public enum WidgetInputDevice
	{
		// Token: 0x040013B4 RID: 5044
		None = 0,
		// Token: 0x040013B5 RID: 5045
		Keyboard = 1,
		// Token: 0x040013B6 RID: 5046
		Mouse = 2,
		// Token: 0x040013B7 RID: 5047
		Touch = 4,
		// Token: 0x040013B8 RID: 5048
		GamePad1 = 8,
		// Token: 0x040013B9 RID: 5049
		GamePad2 = 16,
		// Token: 0x040013BA RID: 5050
		GamePad3 = 32,
		// Token: 0x040013BB RID: 5051
		GamePad4 = 64,
		// Token: 0x040013BC RID: 5052
		Gamepads = 120,
		// Token: 0x040013BD RID: 5053
		VrControllers = 128,
		// Token: 0x040013BE RID: 5054
		All = 255
	}
}
