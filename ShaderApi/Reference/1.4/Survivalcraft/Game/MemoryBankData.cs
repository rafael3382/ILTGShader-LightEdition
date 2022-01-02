using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Game
{
	// Token: 0x020002C2 RID: 706
	public class MemoryBankData : IEditableItemData
	{
		// Token: 0x1700033B RID: 827
		// (get) Token: 0x0600158C RID: 5516 RVA: 0x000A283E File Offset: 0x000A0A3E
		// (set) Token: 0x0600158D RID: 5517 RVA: 0x000A2846 File Offset: 0x000A0A46
		public byte LastOutput { get; set; }

		// Token: 0x0600158E RID: 5518 RVA: 0x000A284F File Offset: 0x000A0A4F
		public byte Read(int address)
		{
			if (address >= 0 && address < this.Data.Count)
			{
				return this.Data.Array[address];
			}
			return 0;
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x000A2874 File Offset: 0x000A0A74
		public void Write(int address, byte data)
		{
			if (address >= 0 && address < this.Data.Count)
			{
				this.Data.Array[address] = data;
				return;
			}
			if (address >= 0 && address < 256 && data != 0)
			{
				this.Data.Count = MathUtils.Max(this.Data.Count, address + 1);
				this.Data.Array[address] = data;
			}
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x000A28DD File Offset: 0x000A0ADD
		public IEditableItemData Copy()
		{
			return new MemoryBankData
			{
				Data = new DynamicArray<byte>(this.Data),
				LastOutput = this.LastOutput
			};
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x000A2904 File Offset: 0x000A0B04
		public void LoadString(string data)
		{
			string[] array = data.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length >= 1)
			{
				string text = array[0];
				text = text.TrimEnd(new char[]
				{
					'0'
				});
				this.Data.Clear();
				for (int i = 0; i < MathUtils.Min(text.Length, 256); i++)
				{
					int num = MemoryBankData.m_hexChars.IndexOf(char.ToUpperInvariant(text[i]));
					if (num < 0)
					{
						num = 0;
					}
					this.Data.Add((byte)num);
				}
			}
			if (array.Length >= 2)
			{
				string text2 = array[1];
				int num2 = MemoryBankData.m_hexChars.IndexOf(char.ToUpperInvariant(text2[0]));
				if (num2 < 0)
				{
					num2 = 0;
				}
				this.LastOutput = (byte)num2;
			}
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x000A29C5 File Offset: 0x000A0BC5
		public string SaveString()
		{
			return this.SaveString(true);
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x000A29D0 File Offset: 0x000A0BD0
		public string SaveString(bool saveLastOutput)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int i = 0; i < this.Data.Count; i++)
			{
				if (this.Data.Array[i] != 0)
				{
					num = i + 1;
				}
			}
			for (int j = 0; j < num; j++)
			{
				int index = MathUtils.Clamp((int)this.Data.Array[j], 0, 15);
				stringBuilder.Append(MemoryBankData.m_hexChars[index]);
			}
			if (saveLastOutput)
			{
				stringBuilder.Append(';');
				stringBuilder.Append(MemoryBankData.m_hexChars[MathUtils.Clamp((int)this.LastOutput, 0, 15)]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000E10 RID: 3600
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

		// Token: 0x04000E11 RID: 3601
		public DynamicArray<byte> Data = new DynamicArray<byte>();
	}
}
