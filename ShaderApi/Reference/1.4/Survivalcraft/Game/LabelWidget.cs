using System;
using Engine.Media;

namespace Game
{
	// Token: 0x0200038F RID: 911
	public class LabelWidget : FontTextWidget
	{
		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06001B3C RID: 6972 RVA: 0x000D5A13 File Offset: 0x000D3C13
		// (set) Token: 0x06001B3D RID: 6973 RVA: 0x000D5A1C File Offset: 0x000D3C1C
		public override string Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				if (this.m_text != value && value != null)
				{
					if (value.StartsWith("[") && value.EndsWith("]"))
					{
						string[] array = value.Substring(1, value.Length - 2).Split(new char[]
						{
							':'
						});
						this.m_text = ((array.Length == 2) ? LanguageControl.GetContentWidgets(array[0], array[1]) : LanguageControl.Get(new string[]
						{
							"Usual",
							value
						}));
					}
					else
					{
						this.m_text = LanguageControl.Get(new string[]
						{
							"Usual",
							value
						});
					}
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x06001B3E RID: 6974 RVA: 0x000D5AD5 File Offset: 0x000D3CD5
		public LabelWidget()
		{
			base.Font = LabelWidget.BitmapFont;
		}

		// Token: 0x04001285 RID: 4741
		public static BitmapFont BitmapFont;
	}
}
