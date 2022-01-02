using System;
using Engine;
using Engine.Graphics;
using Engine.Input;
using Engine.Media;

namespace Game
{
	// Token: 0x020003A3 RID: 931
	public class TextBoxWidget : Widget
	{
		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06001C3B RID: 7227 RVA: 0x000DB520 File Offset: 0x000D9720
		// (set) Token: 0x06001C3C RID: 7228 RVA: 0x000DB540 File Offset: 0x000D9740
		public Vector2 Size
		{
			get
			{
				if (this.m_size == null)
				{
					return Vector2.Zero;
				}
				return this.m_size.Value;
			}
			set
			{
				this.m_size = new Vector2?(value);
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06001C3D RID: 7229 RVA: 0x000DB54E File Offset: 0x000D974E
		// (set) Token: 0x06001C3E RID: 7230 RVA: 0x000DB556 File Offset: 0x000D9756
		public string Title { get; set; }

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06001C3F RID: 7231 RVA: 0x000DB55F File Offset: 0x000D975F
		// (set) Token: 0x06001C40 RID: 7232 RVA: 0x000DB567 File Offset: 0x000D9767
		public string Description { get; set; }

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06001C41 RID: 7233 RVA: 0x000DB570 File Offset: 0x000D9770
		// (set) Token: 0x06001C42 RID: 7234 RVA: 0x000DB578 File Offset: 0x000D9778
		public string Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				string text = (value.Length > this.MaximumLength) ? value.Substring(0, this.MaximumLength) : value;
				if (text != this.m_text)
				{
					this.m_text = text;
					this.CaretPosition = this.CaretPosition;
					Action<TextBoxWidget> textChanged = this.TextChanged;
					if (textChanged == null)
					{
						return;
					}
					textChanged(this);
				}
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06001C43 RID: 7235 RVA: 0x000DB5D6 File Offset: 0x000D97D6
		// (set) Token: 0x06001C44 RID: 7236 RVA: 0x000DB5DE File Offset: 0x000D97DE
		public int MaximumLength
		{
			get
			{
				return this.m_maximumLength;
			}
			set
			{
				this.m_maximumLength = MathUtils.Max(value, 0);
				if (this.Text.Length > this.m_maximumLength)
				{
					this.Text = this.Text.Substring(0, this.m_maximumLength);
				}
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06001C45 RID: 7237 RVA: 0x000DB618 File Offset: 0x000D9818
		// (set) Token: 0x06001C46 RID: 7238 RVA: 0x000DB620 File Offset: 0x000D9820
		public bool OverwriteMode { get; set; }

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06001C47 RID: 7239 RVA: 0x000DB629 File Offset: 0x000D9829
		// (set) Token: 0x06001C48 RID: 7240 RVA: 0x000DB634 File Offset: 0x000D9834
		public bool HasFocus
		{
			get
			{
				return this.m_hasFocus;
			}
			set
			{
				if (value != this.m_hasFocus)
				{
					this.m_hasFocus = value;
					if (value)
					{
						if (this.m_hasFocus && this.Text == string.Empty)
						{
							KeyboardInput.GetInput();
						}
						this.CaretPosition = this.m_text.Length;
						Keyboard.ShowKeyboard(this.Title, this.Description, this.Text, false, delegate(string text)
						{
							this.Text = text;
						}, null);
						return;
					}
					Action<TextBoxWidget> focusLost = this.FocusLost;
					if (focusLost == null)
					{
						return;
					}
					focusLost(this);
				}
			}
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06001C49 RID: 7241 RVA: 0x000DB6BC File Offset: 0x000D98BC
		// (set) Token: 0x06001C4A RID: 7242 RVA: 0x000DB6C4 File Offset: 0x000D98C4
		public BitmapFont Font
		{
			get
			{
				return this.m_font;
			}
			set
			{
				this.m_font = value;
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06001C4B RID: 7243 RVA: 0x000DB6CD File Offset: 0x000D98CD
		// (set) Token: 0x06001C4C RID: 7244 RVA: 0x000DB6D5 File Offset: 0x000D98D5
		public float FontScale { get; set; }

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06001C4D RID: 7245 RVA: 0x000DB6DE File Offset: 0x000D98DE
		// (set) Token: 0x06001C4E RID: 7246 RVA: 0x000DB6E6 File Offset: 0x000D98E6
		public Vector2 FontSpacing { get; set; }

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06001C4F RID: 7247 RVA: 0x000DB6EF File Offset: 0x000D98EF
		// (set) Token: 0x06001C50 RID: 7248 RVA: 0x000DB6F7 File Offset: 0x000D98F7
		public Color Color { get; set; }

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06001C51 RID: 7249 RVA: 0x000DB700 File Offset: 0x000D9900
		// (set) Token: 0x06001C52 RID: 7250 RVA: 0x000DB708 File Offset: 0x000D9908
		public bool TextureLinearFilter { get; set; }

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06001C53 RID: 7251 RVA: 0x000DB711 File Offset: 0x000D9911
		// (set) Token: 0x06001C54 RID: 7252 RVA: 0x000DB719 File Offset: 0x000D9919
		public int CaretPosition
		{
			get
			{
				return this.m_caretPosition;
			}
			set
			{
				this.m_caretPosition = MathUtils.Clamp(value, 0, this.Text.Length);
				this.m_focusStartTime = Time.RealTime;
			}
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06001C55 RID: 7253 RVA: 0x000DB740 File Offset: 0x000D9940
		// (remove) Token: 0x06001C56 RID: 7254 RVA: 0x000DB778 File Offset: 0x000D9978
		public event Action<TextBoxWidget> TextChanged;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06001C57 RID: 7255 RVA: 0x000DB7B0 File Offset: 0x000D99B0
		// (remove) Token: 0x06001C58 RID: 7256 RVA: 0x000DB7E8 File Offset: 0x000D99E8
		public event Action<TextBoxWidget> Enter;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x06001C59 RID: 7257 RVA: 0x000DB820 File Offset: 0x000D9A20
		// (remove) Token: 0x06001C5A RID: 7258 RVA: 0x000DB858 File Offset: 0x000D9A58
		public event Action<TextBoxWidget> Escape;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06001C5B RID: 7259 RVA: 0x000DB890 File Offset: 0x000D9A90
		// (remove) Token: 0x06001C5C RID: 7260 RVA: 0x000DB8C8 File Offset: 0x000D9AC8
		public event Action<TextBoxWidget> FocusLost;

		// Token: 0x06001C5D RID: 7261 RVA: 0x000DB900 File Offset: 0x000D9B00
		public TextBoxWidget()
		{
			base.ClampToBounds = true;
			this.Color = Color.White;
			this.TextureLinearFilter = true;
			this.Font = ContentManager.Get<BitmapFont>("Fonts/Pericles", null);
			this.FontScale = 1f;
			this.Title = string.Empty;
			this.Description = string.Empty;
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x000DB974 File Offset: 0x000D9B74
		public override void Update()
		{
			if (this.HasFocus)
			{
				if (KeyboardInput.DeletePressed)
				{
					int num = this.CaretPosition;
					this.CaretPosition = num - 1;
					this.CaretPosition = Math.Max(0, this.CaretPosition);
					if (this.Text.Length > 0)
					{
						this.Text = this.Text.Remove(this.CaretPosition, 1);
					}
					float num2 = this.Font.CalculateCharacterPosition(this.Text, 0, new Vector2(this.FontScale), this.FontSpacing);
					this.m_scroll = num2 - base.ActualSize.X;
					this.m_scroll = MathUtils.Max(0f, this.m_scroll);
				}
				string input = KeyboardInput.GetInput();
				if (!string.IsNullOrEmpty(input))
				{
					this.EnterText(input);
				}
			}
			if (base.Input.Click != null)
			{
				this.HasFocus = (base.HitTestGlobal(base.Input.Click.Value.Start, null) == this && base.HitTestGlobal(base.Input.Click.Value.End, null) == this);
				if (GameManager.Project != null)
				{
					foreach (ComponentPlayer componentPlayer in GameManager.Project.FindSubsystem<SubsystemPlayers>().ComponentPlayers)
					{
						componentPlayer.ComponentInput.AllowHandleInput = !this.HasFocus;
					}
				}
			}
			if (this.HasFocus)
			{
				if (base.Input.IsKeyDown(Key.Control))
				{
					if (base.Input.IsKeyDownOnce(Key.V))
					{
						this.Text += ClipboardManager.ClipboardString;
					}
					else if (base.Input.IsKeyDownOnce(Key.C))
					{
						ClipboardManager.ClipboardString = this.Text;
					}
					else if (base.Input.IsKeyDownOnce(Key.X))
					{
						ClipboardManager.ClipboardString = this.Text;
						this.Text = string.Empty;
					}
				}
				if (base.Input.IsKeyDownOnce(Key.Tab))
				{
					this.MoveNext(ScreensManager.CurrentScreen.Children);
				}
				if (base.Input.IsKeyDownRepeat(Key.LeftArrow))
				{
					int x = 0;
					int num = this.CaretPosition - 1;
					this.CaretPosition = num;
					this.CaretPosition = MathUtils.Max(x, num);
				}
				if (base.Input.IsKeyDownRepeat(Key.RightArrow))
				{
					int length = this.Text.Length;
					int num = this.CaretPosition + 1;
					this.CaretPosition = num;
					this.CaretPosition = MathUtils.Max(length, num);
				}
				if (base.Input.IsKeyDownRepeat(Key.UpArrow))
				{
					this.CaretPosition = 0;
				}
				if (base.Input.IsKeyDownRepeat(Key.DownArrow))
				{
					this.CaretPosition = this.Text.Length;
				}
			}
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x000DBC44 File Offset: 0x000D9E44
		public void MoveNext(WidgetsList widgets)
		{
			foreach (Widget widget in widgets)
			{
				if (widget is TextBoxWidget)
				{
					if (!this.MoveNextFlag && widget == this)
					{
						this.MoveNextFlag = true;
					}
					else if (this.MoveNextFlag)
					{
						(widget as TextBoxWidget).HasFocus = true;
						this.HasFocus = false;
						this.MoveNextFlag = false;
					}
				}
				if (widget is ContainerWidget)
				{
					ContainerWidget containerWidget = widget as ContainerWidget;
					this.MoveNext(containerWidget.Children);
				}
			}
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x000DBCE8 File Offset: 0x000D9EE8
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			if (this.m_size != null)
			{
				base.DesiredSize = this.m_size.Value;
				return;
			}
			if (this.Text.Length == 0)
			{
				base.DesiredSize = this.Font.MeasureText(" ", new Vector2(this.FontScale), this.FontSpacing);
			}
			else
			{
				base.DesiredSize = this.Font.MeasureText(this.Text, new Vector2(this.FontScale), this.FontSpacing);
			}
			base.DesiredSize += new Vector2(1f * this.FontScale * this.Font.Scale, 0f);
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x000DBDAC File Offset: 0x000D9FAC
		public override void Draw(Widget.DrawContext dc)
		{
			Color color = this.Color * base.GlobalColorTransform;
			if (!string.IsNullOrEmpty(this.m_text))
			{
				Vector2 position = new Vector2(0f - this.m_scroll, base.ActualSize.Y / 2f);
				SamplerState samplerState = this.TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp;
				FontBatch2D fontBatch2D = dc.PrimitivesRenderer2D.FontBatch(this.Font, 1, DepthStencilState.None, null, null, samplerState);
				int count = fontBatch2D.TriangleVertices.Count;
				fontBatch2D.QueueText(this.Text, position, 0f, color, TextAnchor.VerticalCenter, new Vector2(this.FontScale), this.FontSpacing, 0f);
				fontBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
			}
			if (!this.m_hasFocus || MathUtils.Remainder(Time.RealTime - this.m_focusStartTime, 0.5) >= 0.25)
			{
				return;
			}
			float num = this.Font.CalculateCharacterPosition(this.Text, this.CaretPosition, new Vector2(this.FontScale), this.FontSpacing);
			Vector2 vector = new Vector2(0f, base.ActualSize.Y / 2f) + new Vector2(num - this.m_scroll, 0f);
			if (this.m_hasFocus)
			{
				if (vector.X < 0f)
				{
					this.m_scroll = MathUtils.Max(this.m_scroll + vector.X, 0f);
				}
				if (vector.X > base.ActualSize.X)
				{
					this.m_scroll += vector.X - base.ActualSize.X + 1f;
				}
			}
			FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(1, DepthStencilState.None, null, null);
			int count2 = flatBatch2D.TriangleVertices.Count;
			flatBatch2D.QueueQuad(vector - new Vector2(0f, this.Font.GlyphHeight / 2f * this.FontScale * this.Font.Scale), vector + new Vector2(1f, this.Font.GlyphHeight / 2f * this.FontScale * this.Font.Scale), 0f, color);
			flatBatch2D.TransformTriangles(base.GlobalTransform, count2, -1);
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x000DC00C File Offset: 0x000DA20C
		public void EnterText(string s)
		{
			if (this.OverwriteMode)
			{
				if (this.CaretPosition + s.Length <= this.MaximumLength)
				{
					if (this.CaretPosition < this.m_text.Length)
					{
						string text = this.Text;
						text = text.Remove(this.CaretPosition, s.Length);
						string text2 = this.Text = text.Insert(this.CaretPosition, s);
					}
					else
					{
						this.Text = this.m_text + s;
					}
					this.CaretPosition += s.Length;
					return;
				}
			}
			else if (this.m_text.Length + s.Length <= this.MaximumLength)
			{
				if (this.CaretPosition < this.m_text.Length)
				{
					this.Text = this.Text.Insert(this.CaretPosition, s);
				}
				else
				{
					this.Text = this.m_text + s;
				}
				this.CaretPosition += s.Length;
			}
		}

		// Token: 0x04001312 RID: 4882
		public BitmapFont m_font;

		// Token: 0x04001313 RID: 4883
		public string m_text = string.Empty;

		// Token: 0x04001314 RID: 4884
		public int m_maximumLength = 512;

		// Token: 0x04001315 RID: 4885
		public bool m_hasFocus;

		// Token: 0x04001316 RID: 4886
		public int m_caretPosition;

		// Token: 0x04001317 RID: 4887
		public double m_focusStartTime;

		// Token: 0x04001318 RID: 4888
		public float m_scroll;

		// Token: 0x04001319 RID: 4889
		public Vector2? m_size;

		// Token: 0x04001325 RID: 4901
		public bool MoveNextFlag;
	}
}
