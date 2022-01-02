using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Game
{
	// Token: 0x02000382 RID: 898
	public class EditMemoryBankDialogAPI : Dialog
	{
		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06001A9F RID: 6815 RVA: 0x000CF4E4 File Offset: 0x000CD6E4
		// (set) Token: 0x06001AA0 RID: 6816 RVA: 0x000CF4EC File Offset: 0x000CD6EC
		public byte LastOutput { get; set; }

		// Token: 0x06001AA1 RID: 6817 RVA: 0x000CF4F8 File Offset: 0x000CD6F8
		public EditMemoryBankDialogAPI(MemoryBankData memoryBankData, Action onCancel)
		{
			this.memory = memoryBankData;
			this.Data.Clear();
			this.Data.AddRange(this.memory.Data);
			CanvasWidget canvasWidget = new CanvasWidget
			{
				Size = new Vector2(600f, 500f),
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center
			};
			RectangleWidget widget = new RectangleWidget
			{
				FillColor = new Color(0, 0, 0, 255),
				OutlineColor = new Color(128, 128, 128, 128),
				OutlineThickness = 2f
			};
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical
			};
			LabelWidget widget2 = new LabelWidget
			{
				Text = LanguageControl.GetContentWidgets(base.GetType().Name, 0),
				HorizontalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(0f, 10f)
			};
			StackPanelWidget stackPanelWidget2 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Near,
				VerticalAlignment = WidgetAlignment.Near,
				Margin = new Vector2(10f, 10f)
			};
			this.Children.Add(canvasWidget);
			canvasWidget.Children.Add(widget);
			canvasWidget.Children.Add(stackPanelWidget);
			stackPanelWidget.Children.Add(widget2);
			stackPanelWidget.Children.Add(stackPanelWidget2);
			stackPanelWidget2.Children.Add(this.initData());
			stackPanelWidget2.Children.Add(this.initButton());
			this.MainView = stackPanelWidget;
			this.onCancel = onCancel;
			this.lastvalue = (int)this.memory.Read(0);
		}

		// Token: 0x06001AA2 RID: 6818 RVA: 0x000CF6B4 File Offset: 0x000CD8B4
		public byte Read(int address)
		{
			if (address >= 0 && address < this.Data.Count)
			{
				return this.Data.Array[address];
			}
			return 0;
		}

		// Token: 0x06001AA3 RID: 6819 RVA: 0x000CF6D8 File Offset: 0x000CD8D8
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

		// Token: 0x06001AA4 RID: 6820 RVA: 0x000CF744 File Offset: 0x000CD944
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

		// Token: 0x06001AA5 RID: 6821 RVA: 0x000CF805 File Offset: 0x000CDA05
		public string SaveString()
		{
			return this.SaveString(true);
		}

		// Token: 0x06001AA6 RID: 6822 RVA: 0x000CF810 File Offset: 0x000CDA10
		public string SaveString(bool saveLastOutput)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = this.Data.Count;
			for (int i = 0; i < count; i++)
			{
				int index = MathUtils.Clamp((int)this.Data.Array[i], 0, 15);
				stringBuilder.Append(MemoryBankData.m_hexChars[index]);
			}
			if (saveLastOutput)
			{
				stringBuilder.Append(';');
				stringBuilder.Append(MemoryBankData.m_hexChars[MathUtils.Clamp((int)this.LastOutput, 0, 15)]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001AA7 RID: 6823 RVA: 0x000CF894 File Offset: 0x000CDA94
		public Widget initData()
		{
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				VerticalAlignment = WidgetAlignment.Center,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(10f, 0f)
			};
			for (int i = 0; i < 17; i++)
			{
				StackPanelWidget stackPanelWidget2 = new StackPanelWidget
				{
					Direction = LayoutDirection.Horizontal
				};
				for (int j = 0; j < 17; j++)
				{
					int addr = (i - 1) * 16 + (j - 1);
					if (j > 0 && i > 0)
					{
						ClickTextWidget clickTextWidget = new ClickTextWidget(new Vector2(22f), string.Format("{0}", MemoryBankData.m_hexChars[(int)this.Read(addr)]), delegate()
						{
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							this.clickpos = addr;
							this.isclick = true;
						}, false);
						this.list.Add(clickTextWidget);
						stackPanelWidget2.Children.Add(clickTextWidget);
					}
					else
					{
						int index;
						if (i == 0 && j > 0)
						{
							index = j - 1;
						}
						else
						{
							if (j != 0 || i <= 0)
							{
								ClickTextWidget widget = new ClickTextWidget(new Vector2(22f), "", null, false);
								stackPanelWidget2.Children.Add(widget);
								goto IL_181;
							}
							index = i - 1;
						}
						ClickTextWidget clickTextWidget2 = new ClickTextWidget(new Vector2(22f), MemoryBankData.m_hexChars[index].ToString(), delegate()
						{
						}, false);
						clickTextWidget2.labelWidget.Color = Color.DarkGray;
						stackPanelWidget2.Children.Add(clickTextWidget2);
					}
					IL_181:;
				}
				stackPanelWidget.Children.Add(stackPanelWidget2);
			}
			return stackPanelWidget;
		}

		// Token: 0x06001AA8 RID: 6824 RVA: 0x000CFA48 File Offset: 0x000CDC48
		public Widget makeFuncButton(string txt, Action func)
		{
			return new ClickTextWidget(new Vector2(40f), txt, func, true)
			{
				BorderColor = Color.White,
				Margin = new Vector2(2f),
				labelWidget = 
				{
					FontScale = ((txt.Length > 1) ? 0.7f : 1f),
					Color = Color.White
				}
			};
		}

		// Token: 0x06001AA9 RID: 6825 RVA: 0x000CFAB4 File Offset: 0x000CDCB4
		public Widget initButton()
		{
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				VerticalAlignment = WidgetAlignment.Center,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(10f, 10f)
			};
			for (int i = 0; i < 6; i++)
			{
				StackPanelWidget stackPanelWidget2 = new StackPanelWidget
				{
					Direction = LayoutDirection.Horizontal
				};
				for (int j = 0; j < 3; j++)
				{
					int num = i * 3 + j;
					if (num < 15)
					{
						int pp = num + 1;
						stackPanelWidget2.Children.Add(this.makeFuncButton(string.Format("{0}", MemoryBankData.m_hexChars[pp]), delegate
						{
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							if (!this.isSetPos)
							{
								this.Write(this.clickpos, (byte)pp);
								this.lastvalue = pp;
								this.clickpos++;
								if (this.clickpos >= 255)
								{
									this.clickpos = 0;
								}
								this.isclick = true;
								return;
							}
							if (this.setPosN == 0)
							{
								this.clickpos = 16 * pp;
							}
							else if (this.setPosN == 1)
							{
								this.clickpos += pp;
							}
							this.setPosN++;
							if (this.setPosN == 2)
							{
								if (this.clickpos > 255)
								{
									this.clickpos = 0;
								}
								this.setPosN = 0;
								this.isclick = true;
								this.isSetPos = false;
							}
						}));
					}
					else if (num == 15)
					{
						stackPanelWidget2.Children.Add(this.makeFuncButton(string.Format("{0}", MemoryBankData.m_hexChars[0]), delegate
						{
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							if (!this.isSetPos)
							{
								this.Write(this.clickpos, 0);
								this.lastvalue = 0;
								this.clickpos++;
								if (this.clickpos >= 255)
								{
									this.clickpos = 0;
								}
								this.isclick = true;
								return;
							}
							if (this.setPosN == 0)
							{
								this.clickpos = 0;
							}
							else if (this.setPosN == 1)
							{
								this.clickpos = this.clickpos;
							}
							this.setPosN++;
							if (this.setPosN == 2)
							{
								if (this.clickpos > 255)
								{
									this.clickpos = 0;
								}
								this.setPosN = 0;
								this.isclick = true;
								this.isSetPos = false;
							}
						}));
					}
					else if (num == 16)
					{
						stackPanelWidget2.Children.Add(this.makeFuncButton(LanguageControl.GetContentWidgets(base.GetType().Name, 1), delegate
						{
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							for (int k = 0; k < this.Data.Count; k++)
							{
								this.Write(k, 0);
							}
							this.isclick = true;
						}));
					}
					else if (num == 17)
					{
						stackPanelWidget2.Children.Add(this.makeFuncButton(LanguageControl.GetContentWidgets(base.GetType().Name, 2), delegate
						{
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							DynamicArray<byte> dynamicArray = new DynamicArray<byte>();
							dynamicArray.AddRange(this.Data);
							dynamicArray.Count = 256;
							for (int k = 0; k < 16; k++)
							{
								for (int l = 0; l < 16; l++)
								{
									this.Write(k + l * 16, dynamicArray[k * 16 + l]);
								}
							}
							this.clickpos = 0;
							this.isclick = true;
						}));
					}
				}
				stackPanelWidget.Children.Add(stackPanelWidget2);
			}
			LabelWidget widget = new LabelWidget
			{
				FontScale = 0.8f,
				Text = LanguageControl.GetContentWidgets(base.GetType().Name, 3),
				HorizontalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(0f, 10f),
				Color = Color.DarkGray
			};
			stackPanelWidget.Children.Add(widget);
			stackPanelWidget.Children.Add(this.makeTextBox(delegate(TextBoxWidget textBoxWidget)
			{
				this.LoadString(textBoxWidget.Text);
				this.isclick = true;
			}, this.memory.SaveString(false)));
			stackPanelWidget.Children.Add(this.makeButton(LanguageControl.GetContentWidgets(base.GetType().Name, 4), delegate
			{
				for (int k = 0; k < this.Data.Count; k++)
				{
					this.memory.Write(k, this.Data[k]);
				}
				Action action = this.onCancel;
				if (action != null)
				{
					action();
				}
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				DialogsManager.HideDialog(this);
			}));
			stackPanelWidget.Children.Add(this.makeButton(LanguageControl.GetContentWidgets(base.GetType().Name, 5), delegate
			{
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				DialogsManager.HideDialog(this);
				this.isclick = true;
			}));
			return stackPanelWidget;
		}

		// Token: 0x06001AAA RID: 6826 RVA: 0x000CFD28 File Offset: 0x000CDF28
		public Widget makeTextBox(Action<TextBoxWidget> ac, string text = "")
		{
			CanvasWidget canvasWidget = new CanvasWidget();
			canvasWidget.HorizontalAlignment = WidgetAlignment.Center;
			RectangleWidget widget = new RectangleWidget
			{
				FillColor = Color.Black,
				OutlineColor = Color.White,
				Size = new Vector2(120f, 30f)
			};
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical
			};
			TextBoxWidget textBoxWidget = new TextBoxWidget
			{
				VerticalAlignment = WidgetAlignment.Center,
				Color = new Color(255, 255, 255),
				Margin = new Vector2(4f, 0f),
				Size = new Vector2(120f, 30f),
				MaximumLength = 256
			};
			textBoxWidget.FontScale = 0.7f;
			textBoxWidget.Text = text;
			textBoxWidget.TextChanged += ac;
			stackPanelWidget.Children.Add(textBoxWidget);
			canvasWidget.Children.Add(widget);
			canvasWidget.Children.Add(stackPanelWidget);
			return canvasWidget;
		}

		// Token: 0x06001AAB RID: 6827 RVA: 0x000CFE18 File Offset: 0x000CE018
		public Widget makeButton(string txt, Action tas)
		{
			return new ClickTextWidget(new Vector2(120f, 30f), txt, tas, false)
			{
				BorderColor = Color.White,
				Margin = new Vector2(0f, 3f),
				labelWidget = 
				{
					FontScale = 0.7f,
					Color = Color.Green
				}
			};
		}

		// Token: 0x06001AAC RID: 6828 RVA: 0x000CFE7C File Offset: 0x000CE07C
		public override void Update()
		{
			if (base.Input.Back || base.Input.Cancel)
			{
				DialogsManager.HideDialog(this);
			}
			if (this.isSetPos)
			{
				this.list[this.clickpos].BorderColor = Color.Red;
				return;
			}
			if (!this.isclick)
			{
				return;
			}
			for (int i = 0; i < this.list.Count; i++)
			{
				if (i == this.clickpos)
				{
					this.list[i].BorderColor = Color.Yellow;
				}
				else
				{
					this.list[i].BorderColor = Color.Transparent;
				}
				this.list[i].labelWidget.Text = string.Format("{0}", MemoryBankData.m_hexChars[(int)this.Read(i)]);
			}
			this.isclick = false;
		}

		// Token: 0x04001227 RID: 4647
		public MemoryBankData memory;

		// Token: 0x04001228 RID: 4648
		public DynamicArray<byte> Data = new DynamicArray<byte>();

		// Token: 0x04001229 RID: 4649
		public StackPanelWidget MainView;

		// Token: 0x0400122A RID: 4650
		public Action onCancel;

		// Token: 0x0400122B RID: 4651
		public int clickpos;

		// Token: 0x0400122C RID: 4652
		public bool isSetPos;

		// Token: 0x0400122D RID: 4653
		public int setPosN;

		// Token: 0x0400122E RID: 4654
		public int lastvalue;

		// Token: 0x0400122F RID: 4655
		public bool isclick = true;

		// Token: 0x04001230 RID: 4656
		public List<ClickTextWidget> list = new List<ClickTextWidget>();
	}
}
