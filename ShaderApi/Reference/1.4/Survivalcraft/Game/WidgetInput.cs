using System;
using Engine;
using Engine.Graphics;
using Engine.Input;

namespace Game
{
	// Token: 0x020003AB RID: 939
	public class WidgetInput
	{
		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06001CED RID: 7405 RVA: 0x000DEF89 File Offset: 0x000DD189
		// (set) Token: 0x06001CEE RID: 7406 RVA: 0x000DEF91 File Offset: 0x000DD191
		public bool Any { get; set; }

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06001CEF RID: 7407 RVA: 0x000DEF9A File Offset: 0x000DD19A
		// (set) Token: 0x06001CF0 RID: 7408 RVA: 0x000DEFA2 File Offset: 0x000DD1A2
		public bool Ok { get; set; }

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06001CF1 RID: 7409 RVA: 0x000DEFAB File Offset: 0x000DD1AB
		// (set) Token: 0x06001CF2 RID: 7410 RVA: 0x000DEFB3 File Offset: 0x000DD1B3
		public bool Cancel { get; set; }

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06001CF3 RID: 7411 RVA: 0x000DEFBC File Offset: 0x000DD1BC
		// (set) Token: 0x06001CF4 RID: 7412 RVA: 0x000DEFC4 File Offset: 0x000DD1C4
		public bool Back { get; set; }

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06001CF5 RID: 7413 RVA: 0x000DEFCD File Offset: 0x000DD1CD
		// (set) Token: 0x06001CF6 RID: 7414 RVA: 0x000DEFD5 File Offset: 0x000DD1D5
		public bool Left { get; set; }

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06001CF7 RID: 7415 RVA: 0x000DEFDE File Offset: 0x000DD1DE
		// (set) Token: 0x06001CF8 RID: 7416 RVA: 0x000DEFE6 File Offset: 0x000DD1E6
		public bool Right { get; set; }

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06001CF9 RID: 7417 RVA: 0x000DEFEF File Offset: 0x000DD1EF
		// (set) Token: 0x06001CFA RID: 7418 RVA: 0x000DEFF7 File Offset: 0x000DD1F7
		public bool Up { get; set; }

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06001CFB RID: 7419 RVA: 0x000DF000 File Offset: 0x000DD200
		// (set) Token: 0x06001CFC RID: 7420 RVA: 0x000DF008 File Offset: 0x000DD208
		public bool Down { get; set; }

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06001CFD RID: 7421 RVA: 0x000DF011 File Offset: 0x000DD211
		// (set) Token: 0x06001CFE RID: 7422 RVA: 0x000DF019 File Offset: 0x000DD219
		public Vector2? Press { get; set; }

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06001CFF RID: 7423 RVA: 0x000DF022 File Offset: 0x000DD222
		// (set) Token: 0x06001D00 RID: 7424 RVA: 0x000DF02A File Offset: 0x000DD22A
		public Vector2? Tap { get; set; }

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06001D01 RID: 7425 RVA: 0x000DF033 File Offset: 0x000DD233
		// (set) Token: 0x06001D02 RID: 7426 RVA: 0x000DF03B File Offset: 0x000DD23B
		public Segment2? Click { get; set; }

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06001D03 RID: 7427 RVA: 0x000DF044 File Offset: 0x000DD244
		// (set) Token: 0x06001D04 RID: 7428 RVA: 0x000DF04C File Offset: 0x000DD24C
		public Segment2? SpecialClick { get; set; }

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06001D05 RID: 7429 RVA: 0x000DF055 File Offset: 0x000DD255
		// (set) Token: 0x06001D06 RID: 7430 RVA: 0x000DF05D File Offset: 0x000DD25D
		public Vector2? Drag { get; set; }

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06001D07 RID: 7431 RVA: 0x000DF066 File Offset: 0x000DD266
		// (set) Token: 0x06001D08 RID: 7432 RVA: 0x000DF06E File Offset: 0x000DD26E
		public DragMode DragMode { get; set; }

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06001D09 RID: 7433 RVA: 0x000DF077 File Offset: 0x000DD277
		// (set) Token: 0x06001D0A RID: 7434 RVA: 0x000DF07F File Offset: 0x000DD27F
		public Vector2? Hold { get; set; }

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06001D0B RID: 7435 RVA: 0x000DF088 File Offset: 0x000DD288
		// (set) Token: 0x06001D0C RID: 7436 RVA: 0x000DF090 File Offset: 0x000DD290
		public float HoldTime { get; set; }

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06001D0D RID: 7437 RVA: 0x000DF099 File Offset: 0x000DD299
		// (set) Token: 0x06001D0E RID: 7438 RVA: 0x000DF0A1 File Offset: 0x000DD2A1
		public Vector3? Scroll { get; set; }

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06001D0F RID: 7439 RVA: 0x000DF0AC File Offset: 0x000DD2AC
		public Key? LastKey
		{
			get
			{
				if (this.m_isCleared || (this.Devices & WidgetInputDevice.Keyboard) == WidgetInputDevice.None)
				{
					return null;
				}
				return Keyboard.LastKey;
			}
		}

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06001D10 RID: 7440 RVA: 0x000DF0DC File Offset: 0x000DD2DC
		public char? LastChar
		{
			get
			{
				if (this.m_isCleared || (this.Devices & WidgetInputDevice.Keyboard) == WidgetInputDevice.None)
				{
					return null;
				}
				return Keyboard.LastChar;
			}
		}

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06001D11 RID: 7441 RVA: 0x000DF10A File Offset: 0x000DD30A
		// (set) Token: 0x06001D12 RID: 7442 RVA: 0x000DF112 File Offset: 0x000DD312
		public bool UseSoftMouseCursor
		{
			get
			{
				return this.m_useSoftMouseCursor;
			}
			set
			{
				this.m_useSoftMouseCursor = value;
			}
		}

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06001D13 RID: 7443 RVA: 0x000DF11B File Offset: 0x000DD31B
		// (set) Token: 0x06001D14 RID: 7444 RVA: 0x000DF12F File Offset: 0x000DD32F
		public bool IsMouseCursorVisible
		{
			get
			{
				return (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None && this.m_isMouseCursorVisible;
			}
			set
			{
				this.m_isMouseCursorVisible = value;
			}
		}

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06001D15 RID: 7445 RVA: 0x000DF138 File Offset: 0x000DD338
		// (set) Token: 0x06001D16 RID: 7446 RVA: 0x000DF1A8 File Offset: 0x000DD3A8
		public Vector2? MousePosition
		{
			get
			{
				if (this.m_isCleared || (this.Devices & WidgetInputDevice.Mouse) == WidgetInputDevice.None)
				{
					return null;
				}
				if (this.m_useSoftMouseCursor)
				{
					return new Vector2?(this.m_softMouseCursorPosition);
				}
				if (Mouse.MousePosition == null)
				{
					return null;
				}
				return new Vector2?(new Vector2(Mouse.MousePosition.Value));
			}
			set
			{
				if ((this.Devices & WidgetInputDevice.Mouse) == WidgetInputDevice.None || value == null)
				{
					return;
				}
				if (this.m_useSoftMouseCursor)
				{
					Vector2 vector;
					Vector2 max;
					if (this.Widget != null)
					{
						vector = this.Widget.GlobalBounds.Min;
						max = this.Widget.GlobalBounds.Max;
					}
					else
					{
						vector = Vector2.Zero;
						max = new Vector2(Window.Size);
					}
					this.m_softMouseCursorPosition = new Vector2(MathUtils.Clamp(value.Value.X, vector.X, max.X - 1f), MathUtils.Clamp(value.Value.Y, vector.Y, max.Y - 1f));
					return;
				}
				Mouse.SetMousePosition((int)value.Value.X, (int)value.Value.Y);
			}
		}

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06001D17 RID: 7447 RVA: 0x000DF283 File Offset: 0x000DD483
		public Point2 MouseMovement
		{
			get
			{
				if (!this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
				{
					return Mouse.MouseMovement;
				}
				return Point2.Zero;
			}
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06001D18 RID: 7448 RVA: 0x000DF2A2 File Offset: 0x000DD4A2
		public int MouseWheelMovement
		{
			get
			{
				if (!this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
				{
					return Mouse.MouseWheelMovement;
				}
				return 0;
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06001D19 RID: 7449 RVA: 0x000DF2C0 File Offset: 0x000DD4C0
		// (set) Token: 0x06001D1A RID: 7450 RVA: 0x000DF324 File Offset: 0x000DD524
		public bool IsPadCursorVisible
		{
			get
			{
				return this.m_isPadCursorVisible && (((this.Devices & WidgetInputDevice.GamePad1) != WidgetInputDevice.None && GamePad.IsConnected(0)) || ((this.Devices & WidgetInputDevice.GamePad2) != WidgetInputDevice.None && GamePad.IsConnected(1)) || ((this.Devices & WidgetInputDevice.GamePad3) != WidgetInputDevice.None && GamePad.IsConnected(2)) || ((this.Devices & WidgetInputDevice.GamePad4) != WidgetInputDevice.None && GamePad.IsConnected(3)));
			}
			set
			{
				this.m_isPadCursorVisible = value;
			}
		}

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06001D1B RID: 7451 RVA: 0x000DF32D File Offset: 0x000DD52D
		// (set) Token: 0x06001D1C RID: 7452 RVA: 0x000DF338 File Offset: 0x000DD538
		public Vector2 PadCursorPosition
		{
			get
			{
				return this.m_padCursorPosition;
			}
			set
			{
				Vector2 vector;
				Vector2 max;
				if (this.Widget != null)
				{
					vector = this.Widget.GlobalBounds.Min;
					max = this.Widget.GlobalBounds.Max;
				}
				else
				{
					vector = Vector2.Zero;
					max = new Vector2(Window.Size);
				}
				value.X = MathUtils.Clamp(value.X, vector.X, max.X - 1f);
				value.Y = MathUtils.Clamp(value.Y, vector.Y, max.Y - 1f);
				this.m_padCursorPosition = value;
			}
		}

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06001D1D RID: 7453 RVA: 0x000DF3D2 File Offset: 0x000DD5D2
		public ReadOnlyList<TouchLocation> TouchLocations
		{
			get
			{
				if (!this.m_isCleared && (this.Devices & WidgetInputDevice.Touch) != WidgetInputDevice.None)
				{
					return Touch.TouchLocations;
				}
				return ReadOnlyList<TouchLocation>.Empty;
			}
		}

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06001D1E RID: 7454 RVA: 0x000DF3F1 File Offset: 0x000DD5F1
		// (set) Token: 0x06001D1F RID: 7455 RVA: 0x000DF3F9 File Offset: 0x000DD5F9
		public Matrix? VrQuadMatrix { get; set; }

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06001D20 RID: 7456 RVA: 0x000DF402 File Offset: 0x000DD602
		// (set) Token: 0x06001D21 RID: 7457 RVA: 0x000DF40A File Offset: 0x000DD60A
		public Vector2? VrCursorPosition { get; set; }

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06001D22 RID: 7458 RVA: 0x000DF413 File Offset: 0x000DD613
		public static WidgetInput EmptyInput { get; } = new WidgetInput(WidgetInputDevice.None);

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06001D23 RID: 7459 RVA: 0x000DF41A File Offset: 0x000DD61A
		public Widget Widget
		{
			get
			{
				return this.m_widget;
			}
		}

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06001D24 RID: 7460 RVA: 0x000DF422 File Offset: 0x000DD622
		// (set) Token: 0x06001D25 RID: 7461 RVA: 0x000DF42A File Offset: 0x000DD62A
		public WidgetInputDevice Devices { get; set; }

		// Token: 0x06001D26 RID: 7462 RVA: 0x000DF433 File Offset: 0x000DD633
		public bool IsKeyDown(Key key)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Keyboard.IsKeyDown(key);
		}

		// Token: 0x06001D27 RID: 7463 RVA: 0x000DF44F File Offset: 0x000DD64F
		public bool IsKeyDownOnce(Key key)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Keyboard.IsKeyDownOnce(key);
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x000DF46B File Offset: 0x000DD66B
		public bool IsKeyDownRepeat(Key key)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Keyboard.IsKeyDownRepeat(key);
		}

		// Token: 0x06001D29 RID: 7465 RVA: 0x000DF488 File Offset: 0x000DD688
		public void EnterText(ContainerWidget parentWidget, string title, string text, int maxLength, Action<string> handler)
		{
			Keyboard.ShowKeyboard(title, string.Empty, text, false, delegate(string s)
			{
				if (s.Length > maxLength)
				{
					s = s.Substring(0, maxLength);
				}
				handler(s);
			}, delegate
			{
				handler(null);
			});
		}

		// Token: 0x06001D2A RID: 7466 RVA: 0x000DF4D0 File Offset: 0x000DD6D0
		public bool IsMouseButtonDown(MouseButton button)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None && Mouse.IsMouseButtonDown(button);
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x000DF4EC File Offset: 0x000DD6EC
		public bool IsMouseButtonDownOnce(MouseButton button)
		{
			return !this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None && Mouse.IsMouseButtonDownOnce(button);
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x000DF508 File Offset: 0x000DD708
		public Vector2 GetPadStickPosition(GamePadStick stick, float deadZone = 0f)
		{
			if (this.m_isCleared)
			{
				return Vector2.Zero;
			}
			Vector2 vector = Vector2.Zero;
			for (int i = 0; i < 4; i++)
			{
				if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None)
				{
					vector += GamePad.GetStickPosition(i, stick, deadZone);
				}
			}
			if (vector.LengthSquared() <= 1f)
			{
				return vector;
			}
			return Vector2.Normalize(vector);
		}

		// Token: 0x06001D2D RID: 7469 RVA: 0x000DF56C File Offset: 0x000DD76C
		public float GetPadTriggerPosition(GamePadTrigger trigger, float deadZone = 0f)
		{
			if (this.m_isCleared)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < 4; i++)
			{
				if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None)
				{
					num += GamePad.GetTriggerPosition(i, trigger, deadZone);
				}
			}
			return MathUtils.Min(num, 1f);
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x000DF5C0 File Offset: 0x000DD7C0
		public bool IsPadButtonDown(GamePadButton button)
		{
			if (this.m_isCleared)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None && GamePad.IsButtonDown(i, button))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D2F RID: 7471 RVA: 0x000DF600 File Offset: 0x000DD800
		public bool IsPadButtonDownOnce(GamePadButton button)
		{
			if (this.m_isCleared)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None && GamePad.IsButtonDownOnce(i, button))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D30 RID: 7472 RVA: 0x000DF640 File Offset: 0x000DD840
		public bool IsPadButtonDownRepeat(GamePadButton button)
		{
			if (this.m_isCleared)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None && GamePad.IsButtonDownRepeat(i, button))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x000DF67E File Offset: 0x000DD87E
		public WidgetInput(WidgetInputDevice devices = WidgetInputDevice.All)
		{
			this.Devices = devices;
		}

		// Token: 0x06001D32 RID: 7474 RVA: 0x000DF6A4 File Offset: 0x000DD8A4
		public void Clear()
		{
			this.m_isCleared = true;
			this.m_mouseDownPoint = null;
			this.m_mouseDragInProgress = false;
			this.m_touchCleared = true;
			this.m_padDownPoint = null;
			this.m_padDragInProgress = false;
			this.m_vrDownPoint = null;
			this.m_vrDragInProgress = false;
			this.ClearInput();
		}

		// Token: 0x06001D33 RID: 7475 RVA: 0x000DF700 File Offset: 0x000DD900
		public void Update()
		{
			this.m_isCleared = false;
			this.ClearInput();
			if (Window.IsActive)
			{
				if ((this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None)
				{
					this.UpdateInputFromKeyboard();
				}
				if ((this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
				{
					this.UpdateInputFromMouse();
				}
				if ((this.Devices & WidgetInputDevice.Gamepads) != WidgetInputDevice.None)
				{
					this.UpdateInputFromGamepads();
				}
				if ((this.Devices & WidgetInputDevice.Touch) != WidgetInputDevice.None)
				{
					this.UpdateInputFromTouch();
				}
			}
		}

		// Token: 0x06001D34 RID: 7476 RVA: 0x000DF764 File Offset: 0x000DD964
		public void Draw(Widget.DrawContext dc)
		{
			if (this.IsMouseCursorVisible && this.UseSoftMouseCursor && this.MousePosition != null)
			{
				Texture2D texture2D = this.m_mouseDragInProgress ? ContentManager.Get<Texture2D>("Textures/Gui/PadCursorDrag", null) : ((this.m_mouseDownPoint == null) ? ContentManager.Get<Texture2D>("Textures/Gui/PadCursor", null) : ContentManager.Get<Texture2D>("Textures/Gui/PadCursorDown", null));
				TexturedBatch2D texturedBatch2D = dc.CursorPrimitivesRenderer2D.TexturedBatch(texture2D, false, 0, null, null, null, null);
				Vector2 corner2;
				Vector2 corner = (corner2 = Vector2.Transform(this.MousePosition.Value, this.Widget.InvertedGlobalTransform)) + new Vector2((float)texture2D.Width, (float)texture2D.Height) * 0.8f;
				int count = texturedBatch2D.TriangleVertices.Count;
				texturedBatch2D.QueueQuad(corner2, corner, 0f, Vector2.Zero, Vector2.One, Color.White);
				texturedBatch2D.TransformTriangles(this.Widget.GlobalTransform, count, -1);
			}
			if (this.IsPadCursorVisible)
			{
				Texture2D texture2D2 = this.m_padDragInProgress ? ContentManager.Get<Texture2D>("Textures/Gui/PadCursorDrag", null) : ((this.m_padDownPoint == null) ? ContentManager.Get<Texture2D>("Textures/Gui/PadCursor", null) : ContentManager.Get<Texture2D>("Textures/Gui/PadCursorDown", null));
				TexturedBatch2D texturedBatch2D2 = dc.CursorPrimitivesRenderer2D.TexturedBatch(texture2D2, false, 0, null, null, null, null);
				Vector2 corner4;
				Vector2 corner3 = (corner4 = Vector2.Transform(this.PadCursorPosition, this.Widget.InvertedGlobalTransform)) + new Vector2((float)texture2D2.Width, (float)texture2D2.Height) * 0.8f;
				int count2 = texturedBatch2D2.TriangleVertices.Count;
				texturedBatch2D2.QueueQuad(corner4, corner3, 0f, Vector2.Zero, Vector2.One, Color.White);
				texturedBatch2D2.TransformTriangles(this.Widget.GlobalTransform, count2, -1);
			}
		}

		// Token: 0x06001D35 RID: 7477 RVA: 0x000DF940 File Offset: 0x000DDB40
		public void ClearInput()
		{
			this.Any = false;
			this.Ok = false;
			this.Cancel = false;
			this.Back = false;
			this.Left = false;
			this.Right = false;
			this.Up = false;
			this.Down = false;
			this.Press = null;
			this.Tap = null;
			this.Click = null;
			this.SpecialClick = null;
			this.Drag = null;
			this.DragMode = DragMode.AllItems;
			this.Hold = null;
			this.HoldTime = 0f;
			this.Scroll = null;
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x000DFA00 File Offset: 0x000DDC00
		public void UpdateInputFromKeyboard()
		{
			if (this.LastKey != null)
			{
				Key? lastKey = this.LastKey;
				Key key = Key.Escape;
				if (!(lastKey.GetValueOrDefault() == key & lastKey != null))
				{
					this.Any = true;
				}
			}
			if (this.IsKeyDownOnce(Key.Escape))
			{
				this.Back = true;
				this.Cancel = true;
			}
			if (this.IsKeyDownRepeat(Key.LeftArrow))
			{
				this.Left = true;
			}
			if (this.IsKeyDownRepeat(Key.RightArrow))
			{
				this.Right = true;
			}
			if (this.IsKeyDownRepeat(Key.UpArrow))
			{
				this.Up = true;
			}
			if (this.IsKeyDownRepeat(Key.DownArrow))
			{
				this.Down = true;
			}
			this.Back |= Keyboard.IsKeyDownOnce(Key.Back);
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x000DFAB4 File Offset: 0x000DDCB4
		public void UpdateInputFromMouse()
		{
			if (this.IsMouseButtonDownOnce(MouseButton.Left))
			{
				this.Any = true;
			}
			if (this.IsMouseCursorVisible && this.MousePosition != null)
			{
				Vector2 value = this.MousePosition.Value;
				if (this.IsMouseButtonDown(MouseButton.Left) || this.IsMouseButtonDown(MouseButton.Right))
				{
					this.Press = new Vector2?(value);
				}
				if (this.IsMouseButtonDownOnce(MouseButton.Left) || this.IsMouseButtonDownOnce(MouseButton.Right))
				{
					this.Tap = new Vector2?(value);
					this.m_mouseDownPoint = new Vector2?(value);
					this.m_mouseDownButton = ((!this.IsMouseButtonDownOnce(MouseButton.Left)) ? MouseButton.Right : MouseButton.Left);
					this.m_mouseDragTime = Time.FrameStartTime;
				}
				if (!this.IsMouseButtonDown(MouseButton.Left) && this.m_mouseDownPoint != null && this.m_mouseDownButton == MouseButton.Left)
				{
					if (this.IsKeyDown(Key.Shift))
					{
						this.SpecialClick = new Segment2?(new Segment2(this.m_mouseDownPoint.Value, value));
					}
					else
					{
						this.Click = new Segment2?(new Segment2(this.m_mouseDownPoint.Value, value));
					}
				}
				if (!this.IsMouseButtonDown(MouseButton.Right) && this.m_mouseDownPoint != null && this.m_mouseDownButton == MouseButton.Right)
				{
					this.SpecialClick = new Segment2?(new Segment2(this.m_mouseDownPoint.Value, value));
				}
				if (this.MouseWheelMovement != 0)
				{
					this.Scroll = new Vector3?(new Vector3(value, (float)this.MouseWheelMovement / 120f));
				}
				if (this.m_mouseHoldInProgress && this.m_mouseDownPoint != null)
				{
					this.Hold = new Vector2?(this.m_mouseDownPoint.Value);
					this.HoldTime = (float)(Time.FrameStartTime - this.m_mouseDragTime);
				}
				if (this.m_mouseDragInProgress)
				{
					this.Drag = new Vector2?(value);
				}
				else if ((this.IsMouseButtonDown(MouseButton.Left) || this.IsMouseButtonDown(MouseButton.Right)) && this.m_mouseDownPoint != null)
				{
					if (Vector2.Distance(this.m_mouseDownPoint.Value, value) > SettingsManager.MinimumDragDistance * this.Widget.GlobalScale)
					{
						this.m_mouseDragInProgress = true;
						this.DragMode = ((!this.IsMouseButtonDown(MouseButton.Left)) ? DragMode.SingleItem : DragMode.AllItems);
						this.Drag = new Vector2?(this.m_mouseDownPoint.Value);
					}
					else if (Time.FrameStartTime - this.m_mouseDragTime > (double)SettingsManager.MinimumHoldDuration)
					{
						this.m_mouseHoldInProgress = true;
					}
				}
			}
			if (!this.IsMouseButtonDown(MouseButton.Left) && !this.IsMouseButtonDown(MouseButton.Right))
			{
				this.m_mouseDragInProgress = false;
				this.m_mouseHoldInProgress = false;
				this.m_mouseDownPoint = null;
			}
			if (this.m_useSoftMouseCursor && this.IsMouseCursorVisible)
			{
				this.MousePosition = new Vector2?((this.MousePosition ?? Vector2.Zero) + new Vector2(this.MouseMovement));
			}
		}

		// Token: 0x06001D38 RID: 7480 RVA: 0x000DFD84 File Offset: 0x000DDF84
		public void UpdateInputFromGamepads()
		{
			if (this.IsPadButtonDownRepeat(GamePadButton.DPadLeft))
			{
				this.Left = true;
			}
			if (this.IsPadButtonDownRepeat(GamePadButton.DPadRight))
			{
				this.Right = true;
			}
			if (this.IsPadButtonDownRepeat(GamePadButton.DPadUp))
			{
				this.Up = true;
			}
			if (this.IsPadButtonDownRepeat(GamePadButton.DPadDown))
			{
				this.Down = true;
			}
			if (this.IsPadCursorVisible)
			{
				if (this.IsPadButtonDownRepeat(GamePadButton.DPadUp))
				{
					this.Scroll = new Vector3?(new Vector3(this.PadCursorPosition, 1f));
				}
				if (this.IsPadButtonDownRepeat(GamePadButton.DPadDown))
				{
					this.Scroll = new Vector3?(new Vector3(this.PadCursorPosition, -1f));
				}
				if (this.IsPadButtonDown(GamePadButton.A))
				{
					this.Press = new Vector2?(this.PadCursorPosition);
				}
				if (this.IsPadButtonDownOnce(GamePadButton.A))
				{
					this.Ok = true;
					this.Tap = new Vector2?(this.PadCursorPosition);
					this.m_padDownPoint = new Vector2?(this.PadCursorPosition);
					this.m_padDragTime = Time.FrameStartTime;
				}
				if (!this.IsPadButtonDown(GamePadButton.A) && this.m_padDownPoint != null)
				{
					if (this.GetPadTriggerPosition(GamePadTrigger.Left, 0f) > 0.5f)
					{
						this.SpecialClick = new Segment2?(new Segment2(this.m_padDownPoint.Value, this.PadCursorPosition));
					}
					else
					{
						this.Click = new Segment2?(new Segment2(this.m_padDownPoint.Value, this.PadCursorPosition));
					}
				}
			}
			if (this.IsPadButtonDownOnce(GamePadButton.A) || this.IsPadButtonDownOnce(GamePadButton.B) || this.IsPadButtonDownOnce(GamePadButton.X) || this.IsPadButtonDownOnce(GamePadButton.Y))
			{
				this.Any = true;
			}
			if (!this.IsPadButtonDown(GamePadButton.A))
			{
				this.m_padDragInProgress = false;
				this.m_padDownPoint = null;
			}
			if (this.IsPadButtonDownOnce(GamePadButton.B))
			{
				this.Cancel = true;
			}
			if (this.IsPadButtonDownOnce(GamePadButton.Back))
			{
				this.Back = true;
			}
			if (this.m_padDragInProgress)
			{
				this.Drag = new Vector2?(this.PadCursorPosition);
			}
			else if (this.IsPadButtonDown(GamePadButton.A) && this.m_padDownPoint != null)
			{
				if (Vector2.Distance(this.m_padDownPoint.Value, this.PadCursorPosition) > SettingsManager.MinimumDragDistance * this.Widget.GlobalScale)
				{
					this.m_padDragInProgress = true;
					this.Drag = new Vector2?(this.m_padDownPoint.Value);
					this.DragMode = DragMode.AllItems;
				}
				else if (Time.FrameStartTime - this.m_padDragTime > (double)SettingsManager.MinimumHoldDuration)
				{
					this.Hold = new Vector2?(this.m_padDownPoint.Value);
					this.HoldTime = (float)(Time.FrameStartTime - this.m_padDragTime);
				}
			}
			if (this.IsPadCursorVisible)
			{
				Vector2 vector = Vector2.Transform(this.PadCursorPosition, this.Widget.InvertedGlobalTransform);
				Vector2 padStickPosition = this.GetPadStickPosition(GamePadStick.Left, SettingsManager.GamepadDeadZone);
				Vector2 vector2 = new Vector2(padStickPosition.X, 0f - padStickPosition.Y);
				vector2 = 1200f * SettingsManager.GamepadCursorSpeed * vector2.LengthSquared() * Vector2.Normalize(vector2) * Time.FrameDuration;
				vector += vector2;
				this.PadCursorPosition = Vector2.Transform(vector, this.Widget.GlobalTransform);
			}
		}

		// Token: 0x06001D39 RID: 7481 RVA: 0x000E00AC File Offset: 0x000DE2AC
		public void UpdateInputFromTouch()
		{
			foreach (TouchLocation touchLocation in this.TouchLocations)
			{
				if (touchLocation.State == TouchLocationState.Pressed)
				{
					if (this.Widget.HitTest(touchLocation.Position))
					{
						this.Any = true;
						this.Tap = new Vector2?(touchLocation.Position);
						this.Press = new Vector2?(touchLocation.Position);
						this.m_touchStartPoint = touchLocation.Position;
						this.m_touchId = new int?(touchLocation.Id);
						this.m_touchCleared = false;
						this.m_touchStartTime = Time.FrameStartTime;
						this.m_touchDragInProgress = false;
						this.m_touchHoldInProgress = false;
					}
				}
				else if (touchLocation.State == TouchLocationState.Moved)
				{
					int? touchId = this.m_touchId;
					int id = touchLocation.Id;
					if (touchId.GetValueOrDefault() == id & touchId != null)
					{
						this.Press = new Vector2?(touchLocation.Position);
						if (!this.m_touchCleared)
						{
							if (this.m_touchDragInProgress)
							{
								this.Drag = new Vector2?(touchLocation.Position);
							}
							else if (Vector2.Distance(touchLocation.Position, this.m_touchStartPoint) > SettingsManager.MinimumDragDistance * this.Widget.GlobalScale)
							{
								this.m_touchDragInProgress = true;
								this.Drag = new Vector2?(this.m_touchStartPoint);
							}
							if (!this.m_touchDragInProgress)
							{
								if (this.m_touchHoldInProgress)
								{
									this.Hold = new Vector2?(this.m_touchStartPoint);
									this.HoldTime = (float)(Time.FrameStartTime - this.m_touchStartTime);
								}
								else if (Time.FrameStartTime - this.m_touchStartTime > (double)SettingsManager.MinimumHoldDuration)
								{
									this.m_touchHoldInProgress = true;
								}
							}
						}
					}
				}
				else if (touchLocation.State == TouchLocationState.Released)
				{
					int? touchId = this.m_touchId;
					int id = touchLocation.Id;
					if (touchId.GetValueOrDefault() == id & touchId != null)
					{
						if (!this.m_touchCleared)
						{
							this.Click = new Segment2?(new Segment2(this.m_touchStartPoint, touchLocation.Position));
						}
						this.m_touchId = null;
						this.m_touchCleared = false;
						this.m_touchDragInProgress = false;
						this.m_touchHoldInProgress = false;
					}
				}
			}
		}

		// Token: 0x04001385 RID: 4997
		public bool m_isCleared;

		// Token: 0x04001386 RID: 4998
		public Widget m_widget;

		// Token: 0x04001387 RID: 4999
		public Vector2 m_softMouseCursorPosition;

		// Token: 0x04001388 RID: 5000
		public Vector2? m_mouseDownPoint;

		// Token: 0x04001389 RID: 5001
		public MouseButton m_mouseDownButton;

		// Token: 0x0400138A RID: 5002
		public double m_mouseDragTime;

		// Token: 0x0400138B RID: 5003
		public bool m_mouseDragInProgress;

		// Token: 0x0400138C RID: 5004
		public bool m_mouseHoldInProgress;

		// Token: 0x0400138D RID: 5005
		public bool m_isMouseCursorVisible = true;

		// Token: 0x0400138E RID: 5006
		public bool m_useSoftMouseCursor;

		// Token: 0x0400138F RID: 5007
		public int? m_touchId;

		// Token: 0x04001390 RID: 5008
		public bool m_touchCleared;

		// Token: 0x04001391 RID: 5009
		public Vector2 m_touchStartPoint;

		// Token: 0x04001392 RID: 5010
		public double m_touchStartTime;

		// Token: 0x04001393 RID: 5011
		public bool m_touchDragInProgress;

		// Token: 0x04001394 RID: 5012
		public bool m_touchHoldInProgress;

		// Token: 0x04001395 RID: 5013
		public Vector2 m_padCursorPosition;

		// Token: 0x04001396 RID: 5014
		public Vector2? m_padDownPoint;

		// Token: 0x04001397 RID: 5015
		public double m_padDragTime;

		// Token: 0x04001398 RID: 5016
		public bool m_padDragInProgress;

		// Token: 0x04001399 RID: 5017
		public bool m_isPadCursorVisible = true;

		// Token: 0x0400139A RID: 5018
		public Vector2? m_vrDownPoint;

		// Token: 0x0400139B RID: 5019
		public double m_vrDragTime;

		// Token: 0x0400139C RID: 5020
		public bool m_vrDragInProgress;

		// Token: 0x0400139D RID: 5021
		public bool m_isVrCursorVisible = true;
	}
}
