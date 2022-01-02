using System;
using Engine;

namespace Game
{
	// Token: 0x020002E3 RID: 739
	public struct PlayerInput
	{
		// Token: 0x04000EAD RID: 3757
		public Vector2 Look;

		// Token: 0x04000EAE RID: 3758
		public Vector3 Move;

		// Token: 0x04000EAF RID: 3759
		public Vector3 SneakMove;

		// Token: 0x04000EB0 RID: 3760
		public Vector3? VrMove;

		// Token: 0x04000EB1 RID: 3761
		public Vector2? VrLook;

		// Token: 0x04000EB2 RID: 3762
		public Vector2 CameraLook;

		// Token: 0x04000EB3 RID: 3763
		public Vector3 CameraMove;

		// Token: 0x04000EB4 RID: 3764
		public Vector3 CameraSneakMove;

		// Token: 0x04000EB5 RID: 3765
		public bool ToggleCreativeFly;

		// Token: 0x04000EB6 RID: 3766
		public bool ToggleSneak;

		// Token: 0x04000EB7 RID: 3767
		public bool ToggleMount;

		// Token: 0x04000EB8 RID: 3768
		public bool EditItem;

		// Token: 0x04000EB9 RID: 3769
		public bool Jump;

		// Token: 0x04000EBA RID: 3770
		public int ScrollInventory;

		// Token: 0x04000EBB RID: 3771
		public bool ToggleInventory;

		// Token: 0x04000EBC RID: 3772
		public bool ToggleClothing;

		// Token: 0x04000EBD RID: 3773
		public bool TakeScreenshot;

		// Token: 0x04000EBE RID: 3774
		public bool SwitchCameraMode;

		// Token: 0x04000EBF RID: 3775
		public bool TimeOfDay;

		// Token: 0x04000EC0 RID: 3776
		public bool Lighting;

		// Token: 0x04000EC1 RID: 3777
		public bool KeyboardHelp;

		// Token: 0x04000EC2 RID: 3778
		public bool GamepadHelp;

		// Token: 0x04000EC3 RID: 3779
		public Ray3? Dig;

		// Token: 0x04000EC4 RID: 3780
		public Ray3? Hit;

		// Token: 0x04000EC5 RID: 3781
		public Ray3? Aim;

		// Token: 0x04000EC6 RID: 3782
		public Ray3? Interact;

		// Token: 0x04000EC7 RID: 3783
		public Ray3? PickBlockType;

		// Token: 0x04000EC8 RID: 3784
		public bool Drop;

		// Token: 0x04000EC9 RID: 3785
		public int? SelectInventorySlot;
	}
}
