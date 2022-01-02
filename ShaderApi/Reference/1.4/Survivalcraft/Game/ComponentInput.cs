using System;
using Engine;
using Engine.Input;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000213 RID: 531
	public class ComponentInput : Component, IUpdateable
	{
		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06001001 RID: 4097 RVA: 0x00077982 File Offset: 0x00075B82
		public PlayerInput PlayerInput
		{
			get
			{
				return this.m_playerInput;
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06001002 RID: 4098 RVA: 0x0007798A File Offset: 0x00075B8A
		// (set) Token: 0x06001003 RID: 4099 RVA: 0x00077992 File Offset: 0x00075B92
		public bool IsControlledByTouch { get; set; }

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06001004 RID: 4100 RVA: 0x0007799B File Offset: 0x00075B9B
		// (set) Token: 0x06001005 RID: 4101 RVA: 0x000779A3 File Offset: 0x00075BA3
		public bool AllowHandleInput { get; set; } = true;

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06001006 RID: 4102 RVA: 0x000779AC File Offset: 0x00075BAC
		// (set) Token: 0x06001007 RID: 4103 RVA: 0x000779B4 File Offset: 0x00075BB4
		public IInventory SplitSourceInventory { get; set; }

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06001008 RID: 4104 RVA: 0x000779BD File Offset: 0x00075BBD
		// (set) Token: 0x06001009 RID: 4105 RVA: 0x000779C5 File Offset: 0x00075BC5
		public int SplitSourceSlotIndex { get; set; }

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x0600100A RID: 4106 RVA: 0x000779CE File Offset: 0x00075BCE
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Input;
			}
		}

		// Token: 0x0600100B RID: 4107 RVA: 0x000779D2 File Offset: 0x00075BD2
		public virtual void SetSplitSourceInventoryAndSlot(IInventory inventory, int slotIndex)
		{
			this.SplitSourceInventory = inventory;
			this.SplitSourceSlotIndex = slotIndex;
		}

		// Token: 0x0600100C RID: 4108 RVA: 0x000779E4 File Offset: 0x00075BE4
		public virtual Ray3? CalculateVrHandRay()
		{
			return null;
		}

		// Token: 0x0600100D RID: 4109 RVA: 0x000779FC File Offset: 0x00075BFC
		public void Update(float dt)
		{
			this.m_playerInput = default(PlayerInput);
			this.UpdateInputFromMouseAndKeyboard(this.m_componentPlayer.GameWidget.Input);
			this.UpdateInputFromGamepad(this.m_componentPlayer.GameWidget.Input);
			this.UpdateInputFromWidgets(this.m_componentPlayer.GameWidget.Input);
			if (this.m_playerInput.Jump)
			{
				if (Time.RealTime - this.m_lastJumpTime < 0.3)
				{
					this.m_playerInput.ToggleCreativeFly = true;
					this.m_lastJumpTime = 0.0;
				}
				else
				{
					this.m_lastJumpTime = Time.RealTime;
				}
			}
			this.m_playerInput.CameraMove = this.m_playerInput.Move;
			this.m_playerInput.CameraSneakMove = this.m_playerInput.SneakMove;
			this.m_playerInput.CameraLook = this.m_playerInput.Look;
			if (!Window.IsActive || !this.m_componentPlayer.PlayerData.IsReadyForPlaying)
			{
				this.m_playerInput = default(PlayerInput);
			}
			else if (this.m_componentPlayer.ComponentHealth.Health <= 0f || this.m_componentPlayer.ComponentSleep.SleepFactor > 0f || !this.m_componentPlayer.GameWidget.ActiveCamera.IsEntityControlEnabled)
			{
				this.m_playerInput = new PlayerInput
				{
					CameraMove = this.m_playerInput.CameraMove,
					CameraSneakMove = this.m_playerInput.CameraSneakMove,
					CameraLook = this.m_playerInput.CameraLook,
					TimeOfDay = this.m_playerInput.TimeOfDay,
					TakeScreenshot = this.m_playerInput.TakeScreenshot,
					KeyboardHelp = this.m_playerInput.KeyboardHelp
				};
			}
			else if (this.m_componentPlayer.GameWidget.ActiveCamera.UsesMovementControls)
			{
				this.m_playerInput.Move = Vector3.Zero;
				this.m_playerInput.SneakMove = Vector3.Zero;
				this.m_playerInput.Look = Vector2.Zero;
				this.m_playerInput.Jump = false;
				this.m_playerInput.ToggleSneak = false;
				this.m_playerInput.ToggleCreativeFly = false;
			}
			if (this.m_playerInput.Move.LengthSquared() > 1f)
			{
				this.m_playerInput.Move = Vector3.Normalize(this.m_playerInput.Move);
			}
			if (this.m_playerInput.SneakMove.LengthSquared() > 1f)
			{
				this.m_playerInput.SneakMove = Vector3.Normalize(this.m_playerInput.SneakMove);
			}
			if (this.SplitSourceInventory != null && this.SplitSourceInventory.GetSlotCount(this.SplitSourceSlotIndex) == 0)
			{
				this.SetSplitSourceInventoryAndSlot(null, -1);
			}
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x00077CC3 File Offset: 0x00075EC3
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentGui = base.Entity.FindComponent<ComponentGui>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
		}

		// Token: 0x0600100F RID: 4111 RVA: 0x00077CFC File Offset: 0x00075EFC
		public virtual void UpdateInputFromMouseAndKeyboard(WidgetInput input)
		{
			Vector3 viewPosition = this.m_componentPlayer.GameWidget.ActiveCamera.ViewPosition;
			Vector3 viewDirection = this.m_componentPlayer.GameWidget.ActiveCamera.ViewDirection;
			if (this.m_componentGui.ModalPanelWidget != null || DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget))
			{
				if (!input.IsMouseCursorVisible)
				{
					ViewWidget viewWidget = this.m_componentPlayer.ViewWidget;
					Vector2 value = viewWidget.WidgetToScreen(viewWidget.ActualSize / 2f);
					input.IsMouseCursorVisible = true;
					input.MousePosition = new Vector2?(value);
				}
			}
			else
			{
				input.IsMouseCursorVisible = false;
				Vector2 zero = Vector2.Zero;
				int num = 0;
				if (Window.IsActive && Time.FrameDuration > 0f)
				{
					Point2 mouseMovement = input.MouseMovement;
					int mouseWheelMovement = input.MouseWheelMovement;
					zero.X = 0.02f * (float)mouseMovement.X / Time.FrameDuration / 60f;
					zero.Y = -0.02f * (float)mouseMovement.Y / Time.FrameDuration / 60f;
					num = mouseWheelMovement / 120;
					if (mouseMovement != Point2.Zero)
					{
						this.IsControlledByTouch = false;
					}
				}
				Vector3 vector = default(Vector3) + Vector3.UnitX * (float)(input.IsKeyDown(Key.D) ? 1 : 0);
				vector += -Vector3.UnitZ * (float)(input.IsKeyDown(Key.S) ? 1 : 0);
				vector += Vector3.UnitZ * (float)(input.IsKeyDown(Key.W) ? 1 : 0);
				vector += -Vector3.UnitX * (float)(input.IsKeyDown(Key.A) ? 1 : 0);
				vector += Vector3.UnitY * (float)(input.IsKeyDown(Key.Space) ? 1 : 0);
				vector += -Vector3.UnitY * (float)(input.IsKeyDown(Key.Shift) ? 1 : 0);
				this.m_playerInput.Look = this.m_playerInput.Look + new Vector2(MathUtils.Clamp(zero.X, -15f, 15f), MathUtils.Clamp(zero.Y, -15f, 15f));
				this.m_playerInput.Move = this.m_playerInput.Move + vector;
				this.m_playerInput.SneakMove = this.m_playerInput.SneakMove + vector;
				this.m_playerInput.Jump = (this.m_playerInput.Jump | input.IsKeyDownOnce(Key.Space));
				this.m_playerInput.ScrollInventory = this.m_playerInput.ScrollInventory - num;
				this.m_playerInput.Dig = (input.IsMouseButtonDown(MouseButton.Left) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Dig);
				this.m_playerInput.Hit = (input.IsMouseButtonDownOnce(MouseButton.Left) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Hit);
				this.m_playerInput.Aim = (input.IsMouseButtonDown(MouseButton.Right) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Aim);
				this.m_playerInput.Interact = (input.IsMouseButtonDownOnce(MouseButton.Right) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Interact);
				this.m_playerInput.ToggleSneak = (this.m_playerInput.ToggleSneak | input.IsKeyDownOnce(Key.Shift));
				this.m_playerInput.ToggleMount = (this.m_playerInput.ToggleMount | input.IsKeyDownOnce(Key.R));
				this.m_playerInput.ToggleCreativeFly = (this.m_playerInput.ToggleCreativeFly | input.IsKeyDownOnce(Key.F));
				this.m_playerInput.PickBlockType = (input.IsMouseButtonDownOnce(MouseButton.Middle) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.PickBlockType);
			}
			if (!DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget) && this.AllowHandleInput)
			{
				this.m_playerInput.ToggleInventory = (this.m_playerInput.ToggleInventory | input.IsKeyDownOnce(Key.E));
				this.m_playerInput.ToggleClothing = (this.m_playerInput.ToggleClothing | input.IsKeyDownOnce(Key.C));
				this.m_playerInput.TakeScreenshot = (this.m_playerInput.TakeScreenshot | input.IsKeyDownOnce(Key.P));
				this.m_playerInput.SwitchCameraMode = (this.m_playerInput.SwitchCameraMode | input.IsKeyDownOnce(Key.V));
				this.m_playerInput.TimeOfDay = (this.m_playerInput.TimeOfDay | input.IsKeyDownOnce(Key.T));
				this.m_playerInput.Lighting = (this.m_playerInput.Lighting | input.IsKeyDownOnce(Key.L));
				this.m_playerInput.Drop = (this.m_playerInput.Drop | input.IsKeyDownOnce(Key.Q));
				this.m_playerInput.EditItem = (this.m_playerInput.EditItem | input.IsKeyDownOnce(Key.G));
				this.m_playerInput.KeyboardHelp = (this.m_playerInput.KeyboardHelp | input.IsKeyDownOnce(Key.H));
				if (input.IsKeyDownOnce(Key.Number1))
				{
					this.m_playerInput.SelectInventorySlot = new int?(0);
				}
				if (input.IsKeyDownOnce(Key.Number2))
				{
					this.m_playerInput.SelectInventorySlot = new int?(1);
				}
				if (input.IsKeyDownOnce(Key.Number3))
				{
					this.m_playerInput.SelectInventorySlot = new int?(2);
				}
				if (input.IsKeyDownOnce(Key.Number4))
				{
					this.m_playerInput.SelectInventorySlot = new int?(3);
				}
				if (input.IsKeyDownOnce(Key.Number5))
				{
					this.m_playerInput.SelectInventorySlot = new int?(4);
				}
				if (input.IsKeyDownOnce(Key.Number6))
				{
					this.m_playerInput.SelectInventorySlot = new int?(5);
				}
				if (input.IsKeyDownOnce(Key.Number7))
				{
					this.m_playerInput.SelectInventorySlot = new int?(6);
				}
				if (input.IsKeyDownOnce(Key.Number8))
				{
					this.m_playerInput.SelectInventorySlot = new int?(7);
				}
				if (input.IsKeyDownOnce(Key.Number9))
				{
					this.m_playerInput.SelectInventorySlot = new int?(8);
				}
				if (input.IsKeyDownOnce(Key.Number0))
				{
					this.m_playerInput.SelectInventorySlot = new int?(9);
				}
			}
		}

		// Token: 0x06001010 RID: 4112 RVA: 0x000782D8 File Offset: 0x000764D8
		public virtual void UpdateInputFromGamepad(WidgetInput input)
		{
			Vector3 viewPosition = this.m_componentPlayer.GameWidget.ActiveCamera.ViewPosition;
			Vector3 viewDirection = this.m_componentPlayer.GameWidget.ActiveCamera.ViewDirection;
			if (this.m_componentGui.ModalPanelWidget != null || DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget))
			{
				if (!input.IsPadCursorVisible)
				{
					ViewWidget viewWidget = this.m_componentPlayer.ViewWidget;
					Vector2 padCursorPosition = viewWidget.WidgetToScreen(viewWidget.ActualSize / 2f);
					input.IsPadCursorVisible = true;
					input.PadCursorPosition = padCursorPosition;
				}
			}
			else
			{
				input.IsPadCursorVisible = false;
				Vector3 vector = Vector3.Zero;
				Vector2 padStickPosition = input.GetPadStickPosition(GamePadStick.Left, SettingsManager.GamepadDeadZone);
				Vector2 padStickPosition2 = input.GetPadStickPosition(GamePadStick.Right, SettingsManager.GamepadDeadZone);
				float padTriggerPosition = input.GetPadTriggerPosition(GamePadTrigger.Left, 0f);
				float padTriggerPosition2 = input.GetPadTriggerPosition(GamePadTrigger.Right, 0f);
				vector += new Vector3(2f * padStickPosition.X, 0f, 2f * padStickPosition.Y);
				vector += Vector3.UnitY * (float)(input.IsPadButtonDown(GamePadButton.A) ? 1 : 0);
				vector += -Vector3.UnitY * (float)(input.IsPadButtonDown(GamePadButton.RightShoulder) ? 1 : 0);
				this.m_playerInput.Move = this.m_playerInput.Move + vector;
				this.m_playerInput.SneakMove = this.m_playerInput.SneakMove + vector;
				this.m_playerInput.Look = this.m_playerInput.Look + 0.75f * padStickPosition2 * MathUtils.Pow(padStickPosition2.LengthSquared(), 0.25f);
				this.m_playerInput.Jump = (this.m_playerInput.Jump | input.IsPadButtonDownOnce(GamePadButton.A));
				this.m_playerInput.Dig = ((padTriggerPosition2 >= 0.5f) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Dig);
				this.m_playerInput.Hit = ((padTriggerPosition2 >= 0.5f && this.m_lastRightTrigger < 0.5f) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Hit);
				this.m_playerInput.Aim = ((padTriggerPosition >= 0.5f) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Aim);
				this.m_playerInput.Interact = ((padTriggerPosition >= 0.5f && this.m_lastLeftTrigger < 0.5f) ? new Ray3?(new Ray3(viewPosition, viewDirection)) : this.m_playerInput.Interact);
				this.m_playerInput.Drop = (this.m_playerInput.Drop | input.IsPadButtonDownOnce(GamePadButton.B));
				this.m_playerInput.ToggleMount = (this.m_playerInput.ToggleMount | (input.IsPadButtonDownOnce(GamePadButton.LeftThumb) || input.IsPadButtonDownOnce(GamePadButton.DPadUp)));
				this.m_playerInput.EditItem = (this.m_playerInput.EditItem | input.IsPadButtonDownOnce(GamePadButton.LeftShoulder));
				this.m_playerInput.ToggleSneak = (this.m_playerInput.ToggleSneak | input.IsPadButtonDownOnce(GamePadButton.RightShoulder));
				this.m_playerInput.SwitchCameraMode = (this.m_playerInput.SwitchCameraMode | (input.IsPadButtonDownOnce(GamePadButton.RightThumb) || input.IsPadButtonDownOnce(GamePadButton.DPadDown)));
				if (input.IsPadButtonDownRepeat(GamePadButton.DPadLeft))
				{
					this.m_playerInput.ScrollInventory = this.m_playerInput.ScrollInventory - 1;
				}
				if (input.IsPadButtonDownRepeat(GamePadButton.DPadRight))
				{
					this.m_playerInput.ScrollInventory = this.m_playerInput.ScrollInventory + 1;
				}
				if (padStickPosition != Vector2.Zero || padStickPosition2 != Vector2.Zero)
				{
					this.IsControlledByTouch = false;
				}
				this.m_lastLeftTrigger = padTriggerPosition;
				this.m_lastRightTrigger = padTriggerPosition2;
			}
			if (!DialogsManager.HasDialogs(this.m_componentPlayer.GuiWidget) && this.AllowHandleInput)
			{
				this.m_playerInput.ToggleInventory = (this.m_playerInput.ToggleInventory | input.IsPadButtonDownOnce(GamePadButton.X));
				this.m_playerInput.ToggleClothing = (this.m_playerInput.ToggleClothing | input.IsPadButtonDownOnce(GamePadButton.Y));
				this.m_playerInput.GamepadHelp = (this.m_playerInput.GamepadHelp | input.IsPadButtonDownOnce(GamePadButton.Start));
			}
		}

		// Token: 0x06001011 RID: 4113 RVA: 0x000786C8 File Offset: 0x000768C8
		public virtual void UpdateInputFromWidgets(WidgetInput input)
		{
			float num = MathUtils.Pow(1.25f, 10f * (SettingsManager.MoveSensitivity - 0.5f));
			float num2 = MathUtils.Pow(1.25f, 10f * (SettingsManager.LookSensitivity - 0.5f));
			float num3 = MathUtils.Clamp(this.m_subsystemTime.GameTimeDelta, 0f, 0.1f);
			ViewWidget viewWidget = this.m_componentPlayer.ViewWidget;
			this.m_componentGui.MoveWidget.Radius = 30f / num * this.m_componentGui.MoveWidget.GlobalScale;
			if (this.m_componentGui.ModalPanelWidget != null || this.m_subsystemTime.GameTimeFactor <= 0f || num3 <= 0f)
			{
				return;
			}
			Vector2 vector = new Vector2((float)(SettingsManager.LeftHandedLayout ? 96 : -96), -96f);
			vector = Vector2.TransformNormal(vector, input.Widget.GlobalTransform);
			if (this.m_componentGui.ViewWidget != null && this.m_componentGui.ViewWidget.TouchInput != null)
			{
				this.IsControlledByTouch = true;
				TouchInput value = this.m_componentGui.ViewWidget.TouchInput.Value;
				Camera activeCamera = this.m_componentPlayer.GameWidget.ActiveCamera;
				Vector3 viewPosition = activeCamera.ViewPosition;
				Vector3 viewDirection = activeCamera.ViewDirection;
				Vector3 direction = Vector3.Normalize(activeCamera.ScreenToWorld(new Vector3(value.Position, 1f), Matrix.Identity) - viewPosition);
				Vector3 direction2 = Vector3.Normalize(activeCamera.ScreenToWorld(new Vector3(value.Position + vector, 1f), Matrix.Identity) - viewPosition);
				if (value.InputType == TouchInputType.Tap)
				{
					if (SettingsManager.LookControlMode == LookControlMode.SplitTouch)
					{
						this.m_playerInput.Interact = new Ray3?(new Ray3(viewPosition, viewDirection));
						this.m_playerInput.Hit = new Ray3?(new Ray3(viewPosition, viewDirection));
					}
					else
					{
						this.m_playerInput.Interact = new Ray3?(new Ray3(viewPosition, direction));
						this.m_playerInput.Hit = new Ray3?(new Ray3(viewPosition, direction));
					}
				}
				else if (value.InputType == TouchInputType.Hold && value.DurationFrames > 1 && value.Duration > 0.2f)
				{
					if (SettingsManager.LookControlMode == LookControlMode.SplitTouch)
					{
						this.m_playerInput.Dig = new Ray3?(new Ray3(viewPosition, viewDirection));
						this.m_playerInput.Aim = new Ray3?(new Ray3(viewPosition, direction2));
					}
					else
					{
						this.m_playerInput.Dig = new Ray3?(new Ray3(viewPosition, direction));
						this.m_playerInput.Aim = new Ray3?(new Ray3(viewPosition, direction2));
					}
					this.m_isViewHoldStarted = true;
				}
				else if (value.InputType == TouchInputType.Move)
				{
					if (SettingsManager.LookControlMode == LookControlMode.EntireScreen || SettingsManager.LookControlMode == LookControlMode.SplitTouch)
					{
						Vector2 v = Vector2.TransformNormal(value.Move, this.m_componentGui.ViewWidget.InvertedGlobalTransform);
						Vector2 v2 = num2 / num3 * new Vector2(0.0006f, -0.0006f) * v * MathUtils.Pow(v.LengthSquared(), 0.125f);
						this.m_playerInput.Look = this.m_playerInput.Look + v2;
					}
					if (this.m_isViewHoldStarted)
					{
						if (SettingsManager.LookControlMode == LookControlMode.SplitTouch)
						{
							this.m_playerInput.Dig = new Ray3?(new Ray3(viewPosition, viewDirection));
							this.m_playerInput.Aim = new Ray3?(new Ray3(viewPosition, direction2));
						}
						else
						{
							this.m_playerInput.Dig = new Ray3?(new Ray3(viewPosition, direction));
							this.m_playerInput.Aim = new Ray3?(new Ray3(viewPosition, direction2));
						}
					}
				}
			}
			else
			{
				this.m_isViewHoldStarted = false;
			}
			if (this.m_componentGui.MoveWidget != null && this.m_componentGui.MoveWidget.TouchInput != null)
			{
				this.IsControlledByTouch = true;
				float radius = this.m_componentGui.MoveWidget.Radius;
				TouchInput value2 = this.m_componentGui.MoveWidget.TouchInput.Value;
				if (value2.InputType == TouchInputType.Tap)
				{
					this.m_playerInput.Jump = true;
				}
				else if (value2.InputType == TouchInputType.Move || value2.InputType == TouchInputType.Hold)
				{
					Vector2 v3 = Vector2.TransformNormal(value2.Move, this.m_componentGui.ViewWidget.InvertedGlobalTransform);
					Vector2 vector2 = num / num3 * new Vector2(0.003f, -0.003f) * v3 * MathUtils.Pow(v3.LengthSquared(), 0.175f);
					this.m_playerInput.SneakMove.X = this.m_playerInput.SneakMove.X + vector2.X;
					this.m_playerInput.SneakMove.Z = this.m_playerInput.SneakMove.Z + vector2.Y;
					Vector2 vector3 = Vector2.TransformNormal(value2.TotalMoveLimited, this.m_componentGui.ViewWidget.InvertedGlobalTransform);
					this.m_playerInput.Move.X = this.m_playerInput.Move.X + ComponentInput.ProcessInputValue(vector3.X * viewWidget.GlobalScale, 0.2f * radius, radius);
					this.m_playerInput.Move.Z = this.m_playerInput.Move.Z + ComponentInput.ProcessInputValue((0f - vector3.Y) * viewWidget.GlobalScale, 0.2f * radius, radius);
				}
			}
			if (this.m_componentGui.MoveRoseWidget != null)
			{
				if (this.m_componentGui.MoveRoseWidget.Direction != Vector3.Zero || this.m_componentGui.MoveRoseWidget.Jump)
				{
					this.IsControlledByTouch = true;
				}
				this.m_playerInput.Move = this.m_playerInput.Move + this.m_componentGui.MoveRoseWidget.Direction;
				this.m_playerInput.SneakMove = this.m_playerInput.SneakMove + this.m_componentGui.MoveRoseWidget.Direction;
				this.m_playerInput.Jump = (this.m_playerInput.Jump | this.m_componentGui.MoveRoseWidget.Jump);
			}
			if (this.m_componentGui.LookWidget != null && this.m_componentGui.LookWidget.TouchInput != null)
			{
				this.IsControlledByTouch = true;
				TouchInput value3 = this.m_componentGui.LookWidget.TouchInput.Value;
				if (value3.InputType == TouchInputType.Tap)
				{
					this.m_playerInput.Jump = true;
					return;
				}
				if (value3.InputType == TouchInputType.Move)
				{
					Vector2 v4 = Vector2.TransformNormal(value3.Move, this.m_componentGui.ViewWidget.InvertedGlobalTransform);
					Vector2 v5 = num2 / num3 * new Vector2(0.0006f, -0.0006f) * v4 * MathUtils.Pow(v4.LengthSquared(), 0.125f);
					this.m_playerInput.Look = this.m_playerInput.Look + v5;
				}
			}
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x00078DE8 File Offset: 0x00076FE8
		public static float ProcessInputValue(float value, float deadZone, float saturationZone)
		{
			return MathUtils.Sign(value) * MathUtils.Clamp((MathUtils.Abs(value) - deadZone) / (saturationZone - deadZone), 0f, 1f);
		}

		// Token: 0x0400097B RID: 2427
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400097C RID: 2428
		public ComponentGui m_componentGui;

		// Token: 0x0400097D RID: 2429
		public ComponentPlayer m_componentPlayer;

		// Token: 0x0400097E RID: 2430
		public PlayerInput m_playerInput;

		// Token: 0x0400097F RID: 2431
		public bool m_isViewHoldStarted;

		// Token: 0x04000980 RID: 2432
		public double m_lastJumpTime;

		// Token: 0x04000981 RID: 2433
		public float m_lastLeftTrigger;

		// Token: 0x04000982 RID: 2434
		public float m_lastRightTrigger;

		// Token: 0x04000983 RID: 2435
		public Vector2 m_vrSmoothLook;
	}
}
