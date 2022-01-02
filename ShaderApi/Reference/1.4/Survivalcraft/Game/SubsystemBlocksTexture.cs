using System;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200018A RID: 394
	public class SubsystemBlocksTexture : Subsystem
	{
		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000914 RID: 2324 RVA: 0x00038ED5 File Offset: 0x000370D5
		// (set) Token: 0x06000915 RID: 2325 RVA: 0x00038EDD File Offset: 0x000370DD
		public Texture2D BlocksTexture { get; set; }

		// Token: 0x06000916 RID: 2326 RVA: 0x00038EE6 File Offset: 0x000370E6
		public override void Load(ValuesDictionary valuesDictionary)
		{
			Display.DeviceReset += this.Display_DeviceReset;
			this.LoadBlocksTexture();
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x00038EFF File Offset: 0x000370FF
		public override void Dispose()
		{
			Display.DeviceReset -= this.Display_DeviceReset;
			this.DisposeBlocksTexture();
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x00038F18 File Offset: 0x00037118
		public void LoadBlocksTexture()
		{
			SubsystemGameInfo subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.BlocksTexture = BlocksTexturesManager.LoadTexture(subsystemGameInfo.WorldSettings.BlocksTextureName);
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x00038F48 File Offset: 0x00037148
		public void DisposeBlocksTexture()
		{
			if (this.BlocksTexture != null && !ContentManager.IsContent(this.BlocksTexture))
			{
				this.BlocksTexture.Dispose();
				this.BlocksTexture = null;
			}
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x00038F71 File Offset: 0x00037171
		public void Display_DeviceReset()
		{
			this.LoadBlocksTexture();
		}
	}
}
