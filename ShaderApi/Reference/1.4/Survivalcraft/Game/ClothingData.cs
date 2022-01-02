using System;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200025D RID: 605
	public class ClothingData
	{
		// Token: 0x1400000A RID: 10
		// (add) Token: 0x060013D7 RID: 5079 RVA: 0x00094650 File Offset: 0x00092850
		// (remove) Token: 0x060013D8 RID: 5080 RVA: 0x00094688 File Offset: 0x00092888
		public event Action Mount;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x060013D9 RID: 5081 RVA: 0x000946C0 File Offset: 0x000928C0
		// (remove) Token: 0x060013DA RID: 5082 RVA: 0x000946F8 File Offset: 0x000928F8
		public event Action Dismount;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x060013DB RID: 5083 RVA: 0x00094730 File Offset: 0x00092930
		// (remove) Token: 0x060013DC RID: 5084 RVA: 0x00094768 File Offset: 0x00092968
		public event Action Update;

		// Token: 0x060013DD RID: 5085 RVA: 0x0009479D File Offset: 0x0009299D
		public void OnMount()
		{
			Action mount = this.Mount;
			if (mount == null)
			{
				return;
			}
			mount();
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x000947AF File Offset: 0x000929AF
		public void OnDismount()
		{
			Action dismount = this.Dismount;
			if (dismount == null)
			{
				return;
			}
			dismount();
		}

		// Token: 0x060013DF RID: 5087 RVA: 0x000947C1 File Offset: 0x000929C1
		public void OnUpdate()
		{
			Action update = this.Update;
			if (update == null)
			{
				return;
			}
			update();
		}

		// Token: 0x04000C5F RID: 3167
		public int Index;

		// Token: 0x04000C60 RID: 3168
		public int DisplayIndex;

		// Token: 0x04000C61 RID: 3169
		public ClothingSlot Slot;

		// Token: 0x04000C62 RID: 3170
		public float ArmorProtection;

		// Token: 0x04000C63 RID: 3171
		public float Sturdiness;

		// Token: 0x04000C64 RID: 3172
		public float Insulation;

		// Token: 0x04000C65 RID: 3173
		public float MovementSpeedFactor;

		// Token: 0x04000C66 RID: 3174
		public float SteedMovementSpeedFactor;

		// Token: 0x04000C67 RID: 3175
		public float DensityModifier;

		// Token: 0x04000C68 RID: 3176
		public Texture2D Texture;

		// Token: 0x04000C69 RID: 3177
		public string DisplayName;

		// Token: 0x04000C6A RID: 3178
		public string Description;

		// Token: 0x04000C6B RID: 3179
		public string ImpactSoundsFolder;

		// Token: 0x04000C6C RID: 3180
		public bool IsOuter;

		// Token: 0x04000C6D RID: 3181
		public bool CanBeDyed;

		// Token: 0x04000C6E RID: 3182
		public int Layer;

		// Token: 0x04000C6F RID: 3183
		public int PlayerLevelRequired;
	}
}
