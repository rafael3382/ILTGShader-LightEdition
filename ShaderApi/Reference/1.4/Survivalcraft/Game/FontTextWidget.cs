using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x02000385 RID: 901
	public class FontTextWidget : Widget
	{
		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06001ABC RID: 6844 RVA: 0x000D0458 File Offset: 0x000CE658
		// (set) Token: 0x06001ABD RID: 6845 RVA: 0x000D0460 File Offset: 0x000CE660
		public Vector2 Size { get; set; }

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06001ABE RID: 6846 RVA: 0x000D0469 File Offset: 0x000CE669
		// (set) Token: 0x06001ABF RID: 6847 RVA: 0x000D0471 File Offset: 0x000CE671
		public virtual string Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				if (this.m_text != value && value != null)
				{
					this.m_text = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06001AC0 RID: 6848 RVA: 0x000D0497 File Offset: 0x000CE697
		// (set) Token: 0x06001AC1 RID: 6849 RVA: 0x000D049F File Offset: 0x000CE69F
		public TextAnchor TextAnchor { get; set; }

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06001AC2 RID: 6850 RVA: 0x000D04A8 File Offset: 0x000CE6A8
		// (set) Token: 0x06001AC3 RID: 6851 RVA: 0x000D04B0 File Offset: 0x000CE6B0
		public TextOrientation TextOrientation
		{
			get
			{
				return this.m_textOrientation;
			}
			set
			{
				if (value != this.m_textOrientation)
				{
					this.m_textOrientation = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06001AC4 RID: 6852 RVA: 0x000D04CE File Offset: 0x000CE6CE
		// (set) Token: 0x06001AC5 RID: 6853 RVA: 0x000D04E3 File Offset: 0x000CE6E3
		public BitmapFont Font
		{
			get
			{
				if (LabelWidget.BitmapFont != null)
				{
					return LabelWidget.BitmapFont;
				}
				return this.m_font;
			}
			set
			{
				if (value != this.m_font)
				{
					this.m_font = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06001AC6 RID: 6854 RVA: 0x000D0501 File Offset: 0x000CE701
		// (set) Token: 0x06001AC7 RID: 6855 RVA: 0x000D0509 File Offset: 0x000CE709
		public float FontScale
		{
			get
			{
				return this.m_fontScale;
			}
			set
			{
				if (value != this.m_fontScale)
				{
					this.m_fontScale = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06001AC8 RID: 6856 RVA: 0x000D0527 File Offset: 0x000CE727
		// (set) Token: 0x06001AC9 RID: 6857 RVA: 0x000D052F File Offset: 0x000CE72F
		public Vector2 FontSpacing
		{
			get
			{
				return this.m_fontSpacing;
			}
			set
			{
				if (value != this.m_fontSpacing)
				{
					this.m_fontSpacing = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06001ACA RID: 6858 RVA: 0x000D0552 File Offset: 0x000CE752
		// (set) Token: 0x06001ACB RID: 6859 RVA: 0x000D055A File Offset: 0x000CE75A
		public bool WordWrap
		{
			get
			{
				return this.m_wordWrap;
			}
			set
			{
				if (value != this.m_wordWrap)
				{
					this.m_wordWrap = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06001ACC RID: 6860 RVA: 0x000D0578 File Offset: 0x000CE778
		// (set) Token: 0x06001ACD RID: 6861 RVA: 0x000D0580 File Offset: 0x000CE780
		public bool Ellipsis
		{
			get
			{
				return this.m_ellipsis;
			}
			set
			{
				if (value != this.m_ellipsis)
				{
					this.m_ellipsis = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06001ACE RID: 6862 RVA: 0x000D059E File Offset: 0x000CE79E
		// (set) Token: 0x06001ACF RID: 6863 RVA: 0x000D05A6 File Offset: 0x000CE7A6
		public int MaxLines
		{
			get
			{
				return this.m_maxLines;
			}
			set
			{
				if (value != this.m_maxLines)
				{
					this.m_maxLines = value;
					this.m_linesSize = null;
				}
			}
		}

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06001AD0 RID: 6864 RVA: 0x000D05C4 File Offset: 0x000CE7C4
		// (set) Token: 0x06001AD1 RID: 6865 RVA: 0x000D05CC File Offset: 0x000CE7CC
		public Color Color { get; set; }

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06001AD2 RID: 6866 RVA: 0x000D05D5 File Offset: 0x000CE7D5
		// (set) Token: 0x06001AD3 RID: 6867 RVA: 0x000D05DD File Offset: 0x000CE7DD
		public bool DropShadow { get; set; }

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06001AD4 RID: 6868 RVA: 0x000D05E6 File Offset: 0x000CE7E6
		// (set) Token: 0x06001AD5 RID: 6869 RVA: 0x000D05EE File Offset: 0x000CE7EE
		public bool TextureLinearFilter { get; set; }

		// Token: 0x06001AD6 RID: 6870 RVA: 0x000D05F8 File Offset: 0x000CE7F8
		public FontTextWidget()
		{
			this.IsHitTestVisible = false;
			this.Text = string.Empty;
			this.FontScale = 1f;
			this.Color = Color.White;
			this.Font = BitmapFont.DebugFont;
			this.TextureLinearFilter = true;
			this.Size = new Vector2(-1f);
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x000D066C File Offset: 0x000CE86C
		public override void Draw(Widget.DrawContext dc)
		{
			if (!string.IsNullOrEmpty(this.Text) && this.Color.A != 0)
			{
				SamplerState samplerState = this.TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp;
				FontBatch2D fontBatch2D = dc.PrimitivesRenderer2D.FontBatch(this.Font, 1, DepthStencilState.None, null, null, samplerState);
				int count = fontBatch2D.TriangleVertices.Count;
				float num = 0f;
				if ((this.TextAnchor & TextAnchor.VerticalCenter) != TextAnchor.Default)
				{
					float num2 = this.Font.GlyphHeight * this.FontScale * this.Font.Scale + (float)(this.m_lines.Count - 1) * ((this.Font.GlyphHeight + this.Font.Spacing.Y) * this.FontScale * this.Font.Scale + this.FontSpacing.Y);
					num = (base.ActualSize.Y - num2) / 2f;
				}
				else if ((this.TextAnchor & TextAnchor.Bottom) != TextAnchor.Default)
				{
					float num3 = this.Font.GlyphHeight * this.FontScale * this.Font.Scale + (float)(this.m_lines.Count - 1) * ((this.Font.GlyphHeight + this.Font.Spacing.Y) * this.FontScale * this.Font.Scale + this.FontSpacing.Y);
					num = base.ActualSize.Y - num3;
				}
				TextAnchor anchor = this.TextAnchor & ~(TextAnchor.VerticalCenter | TextAnchor.Bottom);
				Color color = this.Color * base.GlobalColorTransform;
				float num4 = this.CalculateLineHeight();
				foreach (string text in this.m_lines)
				{
					float x = 0f;
					if ((this.TextAnchor & TextAnchor.HorizontalCenter) != TextAnchor.Default)
					{
						x = base.ActualSize.X / 2f;
					}
					else if ((this.TextAnchor & TextAnchor.Right) != TextAnchor.Default)
					{
						x = base.ActualSize.X;
					}
					bool flag = true;
					Vector2 zero = Vector2.Zero;
					float angle = 0f;
					if (this.TextOrientation == TextOrientation.Horizontal)
					{
						zero = new Vector2(x, num);
						angle = 0f;
						Rectangle scissorRectangle = Display.ScissorRectangle;
						flag = true;
					}
					else if (this.TextOrientation == TextOrientation.VerticalLeft)
					{
						zero = new Vector2(x, base.ActualSize.Y + num);
						angle = MathUtils.DegToRad(-90f);
						flag = true;
					}
					if (flag)
					{
						if (this.DropShadow)
						{
							fontBatch2D.QueueText(text, zero + 1f * new Vector2(this.FontScale), 0f, new Color(0, 0, 0, color.A), anchor, new Vector2(this.FontScale), this.FontSpacing, angle);
						}
						fontBatch2D.QueueText(text, zero, 0f, color, anchor, new Vector2(this.FontScale), this.FontSpacing, angle);
					}
					num += num4;
				}
				fontBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
			}
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x000D09A8 File Offset: 0x000CEBA8
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = (!string.IsNullOrEmpty(this.Text) && this.Color.A > 0);
			if (this.TextOrientation == TextOrientation.Horizontal)
			{
				this.UpdateLines(parentAvailableSize.X, parentAvailableSize.Y);
				base.DesiredSize = new Vector2((this.Size.X < 0f) ? this.m_linesSize.Value.X : this.Size.X, (this.Size.Y < 0f) ? this.m_linesSize.Value.Y : this.Size.Y);
				return;
			}
			if (this.TextOrientation == TextOrientation.VerticalLeft)
			{
				this.UpdateLines(parentAvailableSize.Y, parentAvailableSize.X);
				base.DesiredSize = new Vector2((this.Size.X < 0f) ? this.m_linesSize.Value.Y : this.Size.X, (this.Size.Y < 0f) ? this.m_linesSize.Value.X : this.Size.Y);
			}
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x000D0AE4 File Offset: 0x000CECE4
		public float CalculateLineHeight()
		{
			return (this.Font.GlyphHeight + this.Font.Spacing.Y + this.FontSpacing.Y) * this.FontScale * this.Font.Scale;
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x000D0B24 File Offset: 0x000CED24
		public void UpdateLines(float availableWidth, float availableHeight)
		{
			if (this.m_linesAvailableHeight != null)
			{
				float? linesAvailableHeight = this.m_linesAvailableHeight;
				if ((linesAvailableHeight.GetValueOrDefault() == availableHeight & linesAvailableHeight != null) && this.m_linesAvailableWidth != null && this.m_linesSize != null)
				{
					float num = MathUtils.Min(this.m_linesSize.Value.X, this.m_linesAvailableWidth.Value) - 0.1f;
					float num2 = MathUtils.Max(this.m_linesSize.Value.X, this.m_linesAvailableWidth.Value) + 0.1f;
					if (availableWidth >= num && availableWidth <= num2)
					{
						return;
					}
				}
			}
			availableWidth += 0.1f;
			this.m_lines.Clear();
			string[] array = (this.Text ?? string.Empty).Split(new string[]
			{
				"\n"
			}, StringSplitOptions.None);
			string text = "...";
			float x = this.Font.MeasureText(text, new Vector2(this.FontScale), this.FontSpacing).X;
			if (this.WordWrap)
			{
				int num3 = (int)MathUtils.Min(MathUtils.Floor(availableHeight / this.CalculateLineHeight()), (float)this.MaxLines);
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = array[i].TrimEnd(Array.Empty<char>());
					if (text2.Length == 0)
					{
						this.m_lines.Add(string.Empty);
					}
					else
					{
						while (text2.Length > 0)
						{
							int num4;
							bool flag;
							if (this.Ellipsis && this.m_lines.Count + 1 >= num3)
							{
								num4 = this.Font.FitText(MathUtils.Max(availableWidth - x, 0f), text2, 0, text2.Length, this.FontScale, this.FontSpacing.X);
								flag = true;
							}
							else
							{
								num4 = this.Font.FitText(availableWidth, text2, 0, text2.Length, this.FontScale, this.FontSpacing.X);
								num4 = MathUtils.Max(num4, 1);
								flag = false;
								if (num4 < text2.Length)
								{
									int num5 = num4;
									int num6 = num5 - 2;
									while (num6 >= 0 && !char.IsWhiteSpace(text2[num6]) && !char.IsPunctuation(text2[num6]))
									{
										num6--;
									}
									if (num6 < 0)
									{
										num6 = num5 - 1;
									}
									num4 = num6 + 1;
								}
							}
							string text3;
							if (num4 == text2.Length)
							{
								text3 = text2;
								text2 = string.Empty;
							}
							else
							{
								text3 = text2.Substring(0, num4).TrimEnd(Array.Empty<char>());
								if (flag)
								{
									text3 += text;
								}
								text2 = text2.Substring(num4, text2.Length - num4).TrimStart(Array.Empty<char>());
							}
							this.m_lines.Add(text3);
							if (flag && this.m_lines.Count > this.MaxLines)
							{
								this.m_lines = this.m_lines.Take(this.MaxLines).ToList<string>();
							}
						}
					}
				}
			}
			else if (this.Ellipsis)
			{
				for (int j = 0; j < array.Length; j++)
				{
					string text4 = array[j].TrimEnd(Array.Empty<char>());
					int num7 = this.Font.FitText(MathUtils.Max(availableWidth - x, 0f), text4, 0, text4.Length, this.FontScale, this.FontSpacing.X);
					if (num7 < text4.Length)
					{
						this.m_lines.Add(text4.Substring(0, num7).TrimEnd(Array.Empty<char>()) + text);
					}
					else
					{
						this.m_lines.Add(text4);
					}
				}
			}
			else
			{
				this.m_lines.AddRange(array);
			}
			if (this.m_lines.Count > this.MaxLines)
			{
				this.m_lines = this.m_lines.Take(this.MaxLines).ToList<string>();
			}
			Vector2 zero = Vector2.Zero;
			for (int k = 0; k < this.m_lines.Count; k++)
			{
				Vector2 vector = this.Font.MeasureText(this.m_lines[k], new Vector2(this.FontScale), this.FontSpacing);
				zero.X = MathUtils.Max(zero.X, vector.X);
				if (k < this.m_lines.Count - 1)
				{
					zero.Y += (this.Font.GlyphHeight + this.Font.Spacing.Y + this.FontSpacing.Y) * this.FontScale * this.Font.Scale;
				}
				else
				{
					zero.Y += this.Font.GlyphHeight * this.FontScale * this.Font.Scale;
				}
			}
			this.m_linesSize = new Vector2?(zero);
			this.m_linesAvailableWidth = new float?(availableWidth);
			this.m_linesAvailableHeight = new float?(availableHeight);
		}

		// Token: 0x04001233 RID: 4659
		public string m_text;

		// Token: 0x04001234 RID: 4660
		public TextOrientation m_textOrientation;

		// Token: 0x04001235 RID: 4661
		public BitmapFont m_font;

		// Token: 0x04001236 RID: 4662
		public Vector2 m_fontSpacing;

		// Token: 0x04001237 RID: 4663
		public float m_fontScale;

		// Token: 0x04001238 RID: 4664
		public int m_maxLines = int.MaxValue;

		// Token: 0x04001239 RID: 4665
		public bool m_wordWrap;

		// Token: 0x0400123A RID: 4666
		public bool m_ellipsis;

		// Token: 0x0400123B RID: 4667
		public List<string> m_lines = new List<string>();

		// Token: 0x0400123C RID: 4668
		public Vector2? m_linesSize;

		// Token: 0x0400123D RID: 4669
		public float? m_linesAvailableWidth;

		// Token: 0x0400123E RID: 4670
		public float? m_linesAvailableHeight;

		// Token: 0x04001244 RID: 4676
		public bool m_usedebug;
	}
}
