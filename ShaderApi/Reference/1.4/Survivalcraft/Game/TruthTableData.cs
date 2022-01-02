using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Game
{
	// Token: 0x02000335 RID: 821
	public class TruthTableData : IEditableItemData
	{
		// Token: 0x0600185B RID: 6235 RVA: 0x000C03B8 File Offset: 0x000BE5B8
		public IEditableItemData Copy()
		{
			return new TruthTableData
			{
				Data = (byte[])this.Data.Clone()
			};
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x000C03D8 File Offset: 0x000BE5D8
		public void LoadString(string data)
		{
			for (int i = 0; i < 16; i++)
			{
				int num = (i < data.Length) ? TruthTableData.m_hexChars.IndexOf(char.ToUpperInvariant(data[i])) : 0;
				if (num < 0)
				{
					num = 0;
				}
				this.Data[i] = (byte)num;
			}
		}

		// Token: 0x0600185D RID: 6237 RVA: 0x000C0428 File Offset: 0x000BE628
		public void LoadBinaryString(string data)
		{
			for (int i = 0; i < 16; i++)
			{
				this.Data[i] = ((i < data.Length && data[i] != '0') ? 15 : 0);
			}
		}

		// Token: 0x0600185E RID: 6238 RVA: 0x000C0464 File Offset: 0x000BE664
		public string SaveString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Data.Length; i++)
			{
				int index = MathUtils.Clamp((int)this.Data[i], 0, 15);
				stringBuilder.Append(TruthTableData.m_hexChars[index]);
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'0'
			});
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x000C04C4 File Offset: 0x000BE6C4
		public string SaveBinaryString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Data.Length; i++)
			{
				stringBuilder.Append((this.Data[i] != 0) ? '1' : '0');
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'0'
			});
		}

		// Token: 0x0400110D RID: 4365
		public static List<char> m_hexChars = new List<char>
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};

		// Token: 0x0400110E RID: 4366
		public byte[] Data = new byte[16];
	}
}
