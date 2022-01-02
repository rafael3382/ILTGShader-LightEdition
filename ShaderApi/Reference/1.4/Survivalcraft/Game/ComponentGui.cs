using System;
using Engine;
using Engine.Input;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020E RID: 526
	public class ComponentGui : Component, IUpdateable, IDrawable
	{
		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000F8D RID: 3981 RVA: 0x00073062 File Offset: 0x00071262
		// (set) Token: 0x06000F8E RID: 3982 RVA: 0x0007306A File Offset: 0x0007126A
		public ContainerWidget ControlsContainerWidget { get; set; }

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000F8F RID: 3983 RVA: 0x00073073 File Offset: 0x00071273
		// (set) Token: 0x06000F90 RID: 3984 RVA: 0x0007307B File Offset: 0x0007127B
		public TouchInputWidget ViewWidget { get; set; }

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000F91 RID: 3985 RVA: 0x00073084 File Offset: 0x00071284
		// (set) Token: 0x06000F92 RID: 3986 RVA: 0x0007308C File Offset: 0x0007128C
		public TouchInputWidget MoveWidget { get; set; }

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000F93 RID: 3987 RVA: 0x00073095 File Offset: 0x00071295
		// (set) Token: 0x06000F94 RID: 3988 RVA: 0x0007309D File Offset: 0x0007129D
		public MoveRoseWidget MoveRoseWidget { get; set; }

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000F95 RID: 3989 RVA: 0x000730A6 File Offset: 0x000712A6
		// (set) Token: 0x06000F96 RID: 3990 RVA: 0x000730AE File Offset: 0x000712AE
		public TouchInputWidget LookWidget { get; set; }

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000F97 RID: 3991 RVA: 0x000730B7 File Offset: 0x000712B7
		// (set) Token: 0x06000F98 RID: 3992 RVA: 0x000730BF File Offset: 0x000712BF
		public ShortInventoryWidget ShortInventoryWidget { get; set; }

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000F99 RID: 3993 RVA: 0x000730C8 File Offset: 0x000712C8
		// (set) Token: 0x06000F9A RID: 3994 RVA: 0x000730D0 File Offset: 0x000712D0
		public ValueBarWidget HealthBarWidget { get; set; }

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000F9B RID: 3995 RVA: 0x000730D9 File Offset: 0x000712D9
		// (set) Token: 0x06000F9C RID: 3996 RVA: 0x000730E1 File Offset: 0x000712E1
		public ValueBarWidget FoodBarWidget { get; set; }

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000F9D RID: 3997 RVA: 0x000730EA File Offset: 0x000712EA
		// (set) Token: 0x06000F9E RID: 3998 RVA: 0x000730F2 File Offset: 0x000712F2
		public ValueBarWidget TemperatureBarWidget { get; set; }

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000F9F RID: 3999 RVA: 0x000730FB File Offset: 0x000712FB
		// (set) Token: 0x06000FA0 RID: 4000 RVA: 0x00073103 File Offset: 0x00071303
		public LabelWidget LevelLabelWidget { get; set; }

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000FA1 RID: 4001 RVA: 0x0007310C File Offset: 0x0007130C
		// (set) Token: 0x06000FA2 RID: 4002 RVA: 0x00073134 File Offset: 0x00071334
		public Widget ModalPanelWidget
		{
			get
			{
				if (this.m_modalPanelContainerWidget.Children.Count <= 0)
				{
					return null;
				}
				return this.m_modalPanelContainerWidget.Children[0];
			}
			set
			{
				if (value != this.ModalPanelWidget)
				{
					if (this.m_modalPanelAnimationData != null)
					{
						this.EndModalPanelAnimation();
					}
					this.m_modalPanelAnimationData = new ComponentGui.ModalPanelAnimationData
					{
						OldWidget = this.ModalPanelWidget,
						NewWidget = value
					};
					if (value != null)
					{
						value.HorizontalAlignment = WidgetAlignment.Center;
						this.m_modalPanelContainerWidget.Children.Insert(0, value);
					}
					this.UpdateModalPanelAnimation();
					this.m_componentPlayer.GameWidget.Input.Clear();
					this.m_componentPlayer.ComponentInput.SetSplitSourceInventoryAndSlot(null, -1);
				}
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000FA3 RID: 4003 RVA: 0x000731BF File Offset: 0x000713BF
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000FA4 RID: 4004 RVA: 0x000731C2 File Offset: 0x000713C2
		public int[] DrawOrders
		{
			get
			{
				return new int[]
				{
					9
				};
			}
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x000731CF File Offset: 0x000713CF
		public virtual void DisplayLargeMessage(string largeText, string smallText, float duration, float delay)
		{
			this.m_message = new ComponentGui.Message
			{
				LargeText = largeText,
				SmallText = smallText,
				Duration = duration,
				StartTime = Time.RealTime + (double)delay
			};
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x00073200 File Offset: 0x00071400
		public virtual void DisplaySmallMessage(string text, Color color, bool blinking, bool playNotificationSound)
		{
			this.m_messageWidget.DisplayMessage(text, color, blinking);
			if (playNotificationSound)
			{
				this.m_subsystemAudio.PlaySound("Audio/UI/Message", 1f, 0f, 0f, 0f);
			}
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x00073238 File Offset: 0x00071438
		public virtual bool IsGameMenuDialogVisible()
		{
			foreach (Dialog dialog in DialogsManager.Dialogs)
			{
				if (dialog.ParentWidget == this.m_componentPlayer.GuiWidget && dialog is GameMenuDialog)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x000732A8 File Offset: 0x000714A8
		public void Update(float dt)
		{
			this.HandleInput();
			this.UpdateWidgets();
			ModsManager.HookAction("GuiUpdate", delegate(ModLoader modLoader)
			{
				modLoader.GuiUpdate(this);
				return false;
			});
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x000732CC File Offset: 0x000714CC
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			ContainerWidget guiWidget = this.m_componentPlayer.GuiWidget;
			this.m_backButtonWidget = guiWidget.Children.Find<ButtonWidget>("BackButton", true);
			this.m_inventoryButtonWidget = guiWidget.Children.Find<ButtonWidget>("InventoryButton", true);
			this.m_clothingButtonWidget = guiWidget.Children.Find<ButtonWidget>("ClothingButton", true);
			this.m_moreButtonWidget = guiWidget.Children.Find<ButtonWidget>("MoreButton", true);
			this.m_moreContentsWidget = guiWidget.Children.Find<Widget>("MoreContents", true);
			this.m_helpButtonWidget = guiWidget.Children.Find<ButtonWidget>("HelpButton", true);
			this.m_photoButtonWidget = guiWidget.Children.Find<ButtonWidget>("PhotoButton", true);
			this.m_lightningButtonWidget = guiWidget.Children.Find<ButtonWidget>("LightningButton", true);
			this.m_timeOfDayButtonWidget = guiWidget.Children.Find<ButtonWidget>("TimeOfDayButton", true);
			this.m_cameraButtonWidget = guiWidget.Children.Find<ButtonWidget>("CameraButton", true);
			this.m_creativeFlyButtonWidget = guiWidget.Children.Find<ButtonWidget>("CreativeFlyButton", true);
			this.m_sneakButtonWidget = guiWidget.Children.Find<ButtonWidget>("SneakButton", true);
			this.m_mountButtonWidget = guiWidget.Children.Find<ButtonWidget>("MountButton", true);
			this.m_editItemButton = guiWidget.Children.Find<ButtonWidget>("EditItemButton", true);
			this.MoveWidget = guiWidget.Children.Find<TouchInputWidget>("Move", true);
			this.MoveRoseWidget = guiWidget.Children.Find<MoveRoseWidget>("MoveRose", true);
			this.LookWidget = guiWidget.Children.Find<TouchInputWidget>("Look", true);
			this.ViewWidget = this.m_componentPlayer.ViewWidget;
			this.HealthBarWidget = guiWidget.Children.Find<ValueBarWidget>("HealthBar", true);
			this.FoodBarWidget = guiWidget.Children.Find<ValueBarWidget>("FoodBar", true);
			this.TemperatureBarWidget = guiWidget.Children.Find<ValueBarWidget>("TemperatureBar", true);
			this.LevelLabelWidget = guiWidget.Children.Find<LabelWidget>("LevelLabel", true);
			this.m_modalPanelContainerWidget = guiWidget.Children.Find<ContainerWidget>("ModalPanelContainer", true);
			this.ControlsContainerWidget = guiWidget.Children.Find<ContainerWidget>("ControlsContainer", true);
			this.m_leftControlsContainerWidget = guiWidget.Children.Find<ContainerWidget>("LeftControlsContainer", true);
			this.m_rightControlsContainerWidget = guiWidget.Children.Find<ContainerWidget>("RightControlsContainer", true);
			this.m_moveContainerWidget = guiWidget.Children.Find<ContainerWidget>("MoveContainer", true);
			this.m_lookContainerWidget = guiWidget.Children.Find<ContainerWidget>("LookContainer", true);
			this.m_moveRectangleWidget = guiWidget.Children.Find<RectangleWidget>("MoveRectangle", true);
			this.m_lookRectangleWidget = guiWidget.Children.Find<RectangleWidget>("LookRectangle", true);
			this.m_moveRectangleContainerWidget = guiWidget.Children.Find<ContainerWidget>("MoveRectangleContainer", true);
			this.m_lookRectangleContainerWidget = guiWidget.Children.Find<ContainerWidget>("LookRectangleContainer", true);
			this.m_moveRectangleWidget = guiWidget.Children.Find<RectangleWidget>("MoveRectangle", true);
			this.m_lookRectangleWidget = guiWidget.Children.Find<RectangleWidget>("LookRectangle", true);
			this.m_movePadContainerWidget = guiWidget.Children.Find<ContainerWidget>("MovePadContainer", true);
			this.m_lookPadContainerWidget = guiWidget.Children.Find<ContainerWidget>("LookPadContainer", true);
			this.m_moveButtonsContainerWidget = guiWidget.Children.Find<ContainerWidget>("MoveButtonsContainer", true);
			this.ShortInventoryWidget = guiWidget.Children.Find<ShortInventoryWidget>("ShortInventory", true);
			this.m_largeMessageWidget = guiWidget.Children.Find<ContainerWidget>("LargeMessage", true);
			this.m_messageWidget = guiWidget.Children.Find<MessageWidget>("Message", true);
			this.m_keyboardHelpMessageShown = valuesDictionary.GetValue<bool>("KeyboardHelpMessageShown");
			this.m_gamepadHelpMessageShown = valuesDictionary.GetValue<bool>("GamepadHelpMessageShown");
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x00073705 File Offset: 0x00071905
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<bool>("KeyboardHelpMessageShown", this.m_keyboardHelpMessageShown);
			valuesDictionary.SetValue<bool>("GamepadHelpMessageShown", this.m_gamepadHelpMessageShown);
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x00073729 File Offset: 0x00071929
		public override void OnEntityAdded()
		{
			this.ShortInventoryWidget.AssignComponents(this.m_componentPlayer.ComponentMiner.Inventory);
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x00073746 File Offset: 0x00071946
		public override void OnEntityRemoved()
		{
			this.ShortInventoryWidget.AssignComponents(null);
			this.m_message = null;
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0007375B File Offset: 0x0007195B
		public override void Dispose()
		{
			this.ModalPanelWidget = null;
			this.m_keyboardHelpDialog = null;
			if (this.ShortInventoryWidget != null)
			{
				this.ShortInventoryWidget.AssignComponents(null);
			}
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x00073780 File Offset: 0x00071980
		public virtual void UpdateSidePanelsAnimation()
		{
			float num = MathUtils.Min(Time.FrameDuration, 0.1f);
			bool flag = this.ModalPanelWidget != null && (this.m_modalPanelAnimationData == null || this.m_modalPanelAnimationData.NewWidget != null);
			float num2 = (float)((!this.m_componentPlayer.ComponentInput.IsControlledByTouch && !flag) ? 1 : 0);
			float x = num2 - this.m_sidePanelsFactor;
			if (MathUtils.Abs(x) > 0.01f)
			{
				this.m_sidePanelsFactor += MathUtils.Clamp(12f * MathUtils.PowSign(x, 0.75f) * num, 0f - MathUtils.Abs(x), MathUtils.Abs(x));
			}
			else
			{
				this.m_sidePanelsFactor = num2;
			}
			this.m_leftControlsContainerWidget.RenderTransform = Matrix.CreateTranslation(this.m_leftControlsContainerWidget.ActualSize.X * (0f - this.m_sidePanelsFactor), 0f, 0f);
			this.m_rightControlsContainerWidget.RenderTransform = Matrix.CreateTranslation(this.m_rightControlsContainerWidget.ActualSize.X * this.m_sidePanelsFactor, 0f, 0f);
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x0007389C File Offset: 0x00071A9C
		public virtual void UpdateModalPanelAnimation()
		{
			this.m_modalPanelAnimationData.Factor += 6f * MathUtils.Min(Time.FrameDuration, 0.1f);
			if (this.m_modalPanelAnimationData.Factor < 1f)
			{
				float factor = this.m_modalPanelAnimationData.Factor;
				float num = 0.5f + 0.5f * MathUtils.Pow(1f - factor, 0.1f);
				float num2 = 0.5f + 0.5f * MathUtils.Pow(factor, 0.1f);
				float s = 1f - factor;
				float s2 = factor;
				if (this.m_modalPanelAnimationData.OldWidget != null)
				{
					Vector2 actualSize = this.m_modalPanelAnimationData.OldWidget.ActualSize;
					this.m_modalPanelAnimationData.OldWidget.ColorTransform = Color.White * s;
					this.m_modalPanelAnimationData.OldWidget.RenderTransform = Matrix.CreateTranslation((0f - actualSize.X) / 2f, (0f - actualSize.Y) / 2f, 0f) * Matrix.CreateScale(num, num, 1f) * Matrix.CreateTranslation(actualSize.X / 2f, actualSize.Y / 2f, 0f);
				}
				if (this.m_modalPanelAnimationData.NewWidget != null)
				{
					Vector2 actualSize2 = this.m_modalPanelAnimationData.NewWidget.ActualSize;
					this.m_modalPanelAnimationData.NewWidget.ColorTransform = Color.White * s2;
					this.m_modalPanelAnimationData.NewWidget.RenderTransform = Matrix.CreateTranslation((0f - actualSize2.X) / 2f, (0f - actualSize2.Y) / 2f, 0f) * Matrix.CreateScale(num2, num2, 1f) * Matrix.CreateTranslation(actualSize2.X / 2f, actualSize2.Y / 2f, 0f);
					return;
				}
			}
			else
			{
				this.EndModalPanelAnimation();
			}
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x00073AA8 File Offset: 0x00071CA8
		public virtual void EndModalPanelAnimation()
		{
			if (this.m_modalPanelAnimationData.OldWidget != null)
			{
				this.m_modalPanelContainerWidget.Children.Remove(this.m_modalPanelAnimationData.OldWidget);
			}
			if (this.m_modalPanelAnimationData.NewWidget != null)
			{
				this.m_modalPanelAnimationData.NewWidget.ColorTransform = Color.White;
				this.m_modalPanelAnimationData.NewWidget.RenderTransform = Matrix.Identity;
			}
			this.m_modalPanelAnimationData = null;
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x00073B1C File Offset: 0x00071D1C
		public virtual void UpdateWidgets()
		{
			ComponentRider componentRider = this.m_componentPlayer.ComponentRider;
			ComponentSleep componentSleep = this.m_componentPlayer.ComponentSleep;
			ComponentInput componentInput = this.m_componentPlayer.ComponentInput;
			WorldSettings worldSettings = this.m_subsystemGameInfo.WorldSettings;
			GameMode gameMode = worldSettings.GameMode;
			this.UpdateSidePanelsAnimation();
			if (this.m_modalPanelAnimationData != null)
			{
				this.UpdateModalPanelAnimation();
			}
			if (this.m_message != null)
			{
				double realTime = Time.RealTime;
				this.m_largeMessageWidget.IsVisible = true;
				LabelWidget labelWidget = this.m_largeMessageWidget.Children.Find<LabelWidget>("LargeLabel", true);
				LabelWidget labelWidget2 = this.m_largeMessageWidget.Children.Find<LabelWidget>("SmallLabel", true);
				labelWidget.Text = this.m_message.LargeText;
				labelWidget2.Text = this.m_message.SmallText;
				labelWidget.IsVisible = !string.IsNullOrEmpty(this.m_message.LargeText);
				labelWidget2.IsVisible = !string.IsNullOrEmpty(this.m_message.SmallText);
				float num = (float)MathUtils.Min(MathUtils.Saturate(2.0 * (realTime - this.m_message.StartTime)), MathUtils.Saturate(2.0 * (this.m_message.StartTime + (double)this.m_message.Duration - realTime)));
				labelWidget.Color = new Color(num, num, num, num);
				labelWidget2.Color = new Color(num, num, num, num);
				if (Time.RealTime > this.m_message.StartTime + (double)this.m_message.Duration)
				{
					this.m_message = null;
				}
			}
			else
			{
				this.m_largeMessageWidget.IsVisible = false;
			}
			this.ControlsContainerWidget.IsVisible = (this.m_componentPlayer.PlayerData.IsReadyForPlaying && this.m_componentPlayer.GameWidget.ActiveCamera.IsEntityControlEnabled && componentSleep.SleepFactor <= 0f);
			this.m_moveRectangleContainerWidget.IsVisible = (!SettingsManager.HideMoveLookPads && componentInput.IsControlledByTouch);
			bool isVisible = false;
			if (!SettingsManager.HideMoveLookPads && componentInput.IsControlledByTouch)
			{
				if (SettingsManager.MoveControlMode == MoveControlMode.Buttons && SettingsManager.LookControlMode == LookControlMode.Pad)
				{
					isVisible = true;
				}
				else if (SettingsManager.MoveControlMode == MoveControlMode.Pad)
				{
					isVisible = true;
				}
			}
			this.m_lookRectangleContainerWidget.IsVisible = isVisible;
			this.m_lookPadContainerWidget.IsVisible = (!SettingsManager.HideMoveLookPads && componentInput.IsControlledByTouch);
			this.MoveRoseWidget.IsVisible = componentInput.IsControlledByTouch;
			this.m_moreContentsWidget.IsVisible = this.m_moreButtonWidget.IsChecked;
			this.HealthBarWidget.IsVisible = (gameMode > GameMode.Creative);
			this.FoodBarWidget.IsVisible = (gameMode != GameMode.Creative && worldSettings.AreAdventureSurvivalMechanicsEnabled);
			this.TemperatureBarWidget.IsVisible = (gameMode != GameMode.Creative && worldSettings.AreAdventureSurvivalMechanicsEnabled);
			this.LevelLabelWidget.IsVisible = (gameMode != GameMode.Creative && worldSettings.AreAdventureSurvivalMechanicsEnabled);
			this.m_creativeFlyButtonWidget.IsVisible = (gameMode == GameMode.Creative);
			this.m_timeOfDayButtonWidget.IsVisible = (gameMode == GameMode.Creative);
			this.m_lightningButtonWidget.IsVisible = (gameMode == GameMode.Creative);
			this.m_moveButtonsContainerWidget.IsVisible = (SettingsManager.MoveControlMode == MoveControlMode.Buttons);
			this.m_movePadContainerWidget.IsVisible = (SettingsManager.MoveControlMode == MoveControlMode.Pad);
			if (SettingsManager.LeftHandedLayout)
			{
				this.m_moveContainerWidget.HorizontalAlignment = WidgetAlignment.Far;
				this.m_lookContainerWidget.HorizontalAlignment = WidgetAlignment.Near;
				this.m_moveRectangleWidget.FlipHorizontal = true;
				this.m_lookRectangleWidget.FlipHorizontal = false;
			}
			else
			{
				this.m_moveContainerWidget.HorizontalAlignment = WidgetAlignment.Near;
				this.m_lookContainerWidget.HorizontalAlignment = WidgetAlignment.Far;
				this.m_moveRectangleWidget.FlipHorizontal = false;
				this.m_lookRectangleWidget.FlipHorizontal = true;
			}
			this.m_sneakButtonWidget.IsChecked = this.m_componentPlayer.ComponentBody.IsSneaking;
			this.m_creativeFlyButtonWidget.IsChecked = this.m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled;
			this.m_inventoryButtonWidget.IsChecked = this.IsInventoryVisible();
			this.m_clothingButtonWidget.IsChecked = this.IsClothingVisible();
			if (this.IsActiveSlotEditable() || this.m_componentPlayer.ComponentBlockHighlight.NearbyEditableCell != null)
			{
				this.m_sneakButtonWidget.IsVisible = false;
				this.m_mountButtonWidget.IsVisible = false;
				this.m_editItemButton.IsVisible = true;
			}
			else if (componentRider != null && componentRider.Mount != null)
			{
				this.m_sneakButtonWidget.IsVisible = false;
				this.m_mountButtonWidget.IsChecked = true;
				this.m_mountButtonWidget.IsVisible = true;
				this.m_editItemButton.IsVisible = false;
			}
			else
			{
				this.m_mountButtonWidget.IsChecked = false;
				if (componentRider != null && Time.FrameStartTime - this.m_lastMountableCreatureSearchTime > 0.5)
				{
					this.m_lastMountableCreatureSearchTime = Time.FrameStartTime;
					if (componentRider.FindNearestMount() != null)
					{
						this.m_sneakButtonWidget.IsVisible = false;
						this.m_mountButtonWidget.IsVisible = true;
						this.m_editItemButton.IsVisible = false;
					}
					else
					{
						this.m_sneakButtonWidget.IsVisible = true;
						this.m_mountButtonWidget.IsVisible = false;
						this.m_editItemButton.IsVisible = false;
					}
				}
			}
			if (!this.m_componentPlayer.IsAddedToProject || this.m_componentPlayer.ComponentHealth.Health == 0f || componentSleep.IsSleeping || this.m_componentPlayer.ComponentSickness.IsPuking)
			{
				this.ModalPanelWidget = null;
			}
			if (this.m_componentPlayer.ComponentSickness.IsSick)
			{
				this.m_componentPlayer.ComponentGui.HealthBarWidget.LitBarColor = new Color(166, 175, 103);
				return;
			}
			this.m_componentPlayer.ComponentGui.HealthBarWidget.LitBarColor = (this.m_componentPlayer.ComponentFlu.HasFlu ? new Color(0, 48, 255) : new Color(224, 24, 0));
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x000740E0 File Offset: 0x000722E0
		public virtual void HandleInput()
		{
			WidgetInput input = this.m_componentPlayer.GameWidget.Input;
			PlayerInput playerInput = this.m_componentPlayer.ComponentInput.PlayerInput;
			ComponentRider componentRider = this.m_componentPlayer.ComponentRider;
			if (this.m_componentPlayer.GameWidget.ActiveCamera.IsEntityControlEnabled)
			{
				if (!this.m_keyboardHelpMessageShown && (this.m_componentPlayer.PlayerData.InputDevice & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Time.PeriodicEvent(7.0, 0.0))
				{
					this.m_keyboardHelpMessageShown = true;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 1), Color.White, true, true);
				}
				else if (!this.m_gamepadHelpMessageShown && (this.m_componentPlayer.PlayerData.InputDevice & WidgetInputDevice.Gamepads) != WidgetInputDevice.None && Time.PeriodicEvent(7.0, 0.0))
				{
					this.m_gamepadHelpMessageShown = true;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 2), Color.White, true, true);
				}
			}
			if (playerInput.KeyboardHelp)
			{
				if (this.m_keyboardHelpDialog == null)
				{
					this.m_keyboardHelpDialog = new KeyboardHelpDialog();
				}
				if (this.m_keyboardHelpDialog.ParentWidget != null)
				{
					DialogsManager.HideDialog(this.m_keyboardHelpDialog);
				}
				else
				{
					DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, this.m_keyboardHelpDialog);
				}
			}
			if (playerInput.GamepadHelp)
			{
				if (this.m_gamepadHelpDialog == null)
				{
					this.m_gamepadHelpDialog = new GamepadHelpDialog();
				}
				if (this.m_gamepadHelpDialog.ParentWidget != null)
				{
					DialogsManager.HideDialog(this.m_gamepadHelpDialog);
				}
				else
				{
					DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, this.m_gamepadHelpDialog);
				}
			}
			if (this.m_helpButtonWidget.IsClicked)
			{
				ScreensManager.SwitchScreen("Help", Array.Empty<object>());
			}
			if (playerInput.ToggleInventory || this.m_inventoryButtonWidget.IsClicked)
			{
				if (this.IsInventoryVisible())
				{
					this.ModalPanelWidget = null;
				}
				else
				{
					this.ModalPanelWidget = ((this.m_componentPlayer.ComponentMiner.Inventory is ComponentCreativeInventory) ? new CreativeInventoryWidget(this.m_componentPlayer.Entity) : new FullInventoryWidget(this.m_componentPlayer.ComponentMiner.Inventory, this.m_componentPlayer.Entity.FindComponent<ComponentCraftingTable>(true)));
				}
			}
			if (playerInput.ToggleClothing || this.m_clothingButtonWidget.IsClicked)
			{
				if (this.IsClothingVisible())
				{
					this.ModalPanelWidget = null;
				}
				else
				{
					ClothingWidget clothingWidget = new ClothingWidget(this.m_componentPlayer);
					ModsManager.HookAction("ClothingWidgetOpen", delegate(ModLoader modLoader)
					{
						modLoader.ClothingWidgetOpen(this, clothingWidget);
						return false;
					});
					this.ModalPanelWidget = clothingWidget;
				}
			}
			if (this.m_sneakButtonWidget.IsClicked || playerInput.ToggleSneak)
			{
				bool isSneaking = this.m_componentPlayer.ComponentBody.IsSneaking;
				this.m_componentPlayer.ComponentBody.IsSneaking = !isSneaking;
				if (this.m_componentPlayer.ComponentBody.IsSneaking != isSneaking)
				{
					if (this.m_componentPlayer.ComponentBody.IsSneaking)
					{
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 3), Color.White, false, false);
					}
					else
					{
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 4), Color.White, false, false);
					}
				}
			}
			if (componentRider != null && (this.m_mountButtonWidget.IsClicked || playerInput.ToggleMount))
			{
				bool flag = componentRider.Mount != null;
				if (flag)
				{
					componentRider.StartDismounting();
				}
				else
				{
					ComponentMount componentMount = componentRider.FindNearestMount();
					if (componentMount != null)
					{
						componentRider.StartMounting(componentMount);
					}
				}
				if (componentRider.Mount != null != flag)
				{
					if (componentRider.Mount != null)
					{
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 5), Color.White, false, false);
					}
					else
					{
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 6), Color.White, false, false);
					}
				}
			}
			if ((this.m_editItemButton.IsClicked || playerInput.EditItem) && this.m_componentPlayer.ComponentBlockHighlight.NearbyEditableCell != null)
			{
				Point3 value = this.m_componentPlayer.ComponentBlockHighlight.NearbyEditableCell.Value;
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(value.X, value.Y, value.Z);
				SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(cellValue));
				for (int i = 0; i < blockBehaviors.Length; i++)
				{
					if (blockBehaviors[i].OnEditBlock(value.X, value.Y, value.Z, cellValue, this.m_componentPlayer))
					{
						break;
					}
				}
			}
			else if ((this.m_editItemButton.IsClicked || playerInput.EditItem) && this.IsActiveSlotEditable())
			{
				IInventory inventory = this.m_componentPlayer.ComponentMiner.Inventory;
				if (inventory != null)
				{
					int activeSlotIndex = inventory.ActiveSlotIndex;
					int slotValue = inventory.GetSlotValue(activeSlotIndex);
					int num = Terrain.ExtractContents(slotValue);
					if (BlocksManager.Blocks[num].IsEditable_(slotValue))
					{
						SubsystemBlockBehavior[] blockBehaviors2 = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(num));
						int num2 = 0;
						while (num2 < blockBehaviors2.Length && !blockBehaviors2[num2].OnEditInventoryItem(inventory, activeSlotIndex, this.m_componentPlayer))
						{
							num2++;
						}
					}
				}
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && (this.m_creativeFlyButtonWidget.IsClicked || playerInput.ToggleCreativeFly) && componentRider.Mount == null)
			{
				bool isCreativeFlyEnabled = this.m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled;
				this.m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled = !isCreativeFlyEnabled;
				if (this.m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled != isCreativeFlyEnabled)
				{
					if (this.m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled)
					{
						this.m_componentPlayer.ComponentLocomotion.JumpOrder = 1f;
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 7), Color.White, false, false);
					}
					else
					{
						this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 8), Color.White, false, false);
					}
				}
			}
			if (this.m_cameraButtonWidget.IsClicked || playerInput.SwitchCameraMode || input.IsKeyDownOnce(Key.V) || input.IsPadButtonDownOnce(GamePadButton.RightThumb) || input.IsPadButtonDownOnce(GamePadButton.DPadDown))
			{
				ModsManager.HookAction("OnCameraChange", delegate(ModLoader modLoader)
				{
					modLoader.OnCameraChange(this.m_componentPlayer, this);
					return false;
				});
			}
			if (this.m_photoButtonWidget.IsClicked || playerInput.TakeScreenshot)
			{
				ScreenCaptureManager.CapturePhoto(delegate
				{
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 13), Color.White, false, false);
				}, delegate
				{
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 14), Color.White, false, false);
				});
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && (this.m_lightningButtonWidget.IsClicked || playerInput.Lighting))
			{
				Matrix matrix = Matrix.CreateFromQuaternion(this.m_componentPlayer.ComponentCreatureModel.EyeRotation);
				base.Project.FindSubsystem<SubsystemWeather>(true).ManualLightingStrike(this.m_componentPlayer.ComponentCreatureModel.EyePosition, matrix.Forward);
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && (this.m_timeOfDayButtonWidget.IsClicked || playerInput.TimeOfDay))
			{
				float num3 = MathUtils.Remainder(0.25f, 1f);
				float num4 = MathUtils.Remainder(0.5f, 1f);
				float num5 = MathUtils.Remainder(0.75f, 1f);
				float num6 = MathUtils.Remainder(1f, 1f);
				float num7 = MathUtils.Remainder(num3 - this.m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num8 = MathUtils.Remainder(num4 - this.m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num9 = MathUtils.Remainder(num5 - this.m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num10 = MathUtils.Remainder(num6 - this.m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num11 = MathUtils.Min(num7, num8, num9, num10);
				if (num7 == num11)
				{
					this.m_subsystemTimeOfDay.TimeOfDayOffset += (double)num7;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 15), Color.White, false, false);
				}
				else if (num8 == num11)
				{
					this.m_subsystemTimeOfDay.TimeOfDayOffset += (double)num8;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 16), Color.White, false, false);
				}
				else if (num9 == num11)
				{
					this.m_subsystemTimeOfDay.TimeOfDayOffset += (double)num9;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 17), Color.White, false, false);
				}
				else if (num10 == num11)
				{
					this.m_subsystemTimeOfDay.TimeOfDayOffset += (double)num10;
					this.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 18), Color.White, false, false);
				}
			}
			if (this.ModalPanelWidget != null)
			{
				if (input.Cancel || input.Back || this.m_backButtonWidget.IsClicked)
				{
					this.ModalPanelWidget = null;
					return;
				}
			}
			else if (input.Back || this.m_backButtonWidget.IsClicked)
			{
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new GameMenuDialog(this.m_componentPlayer));
			}
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x000749CE File Offset: 0x00072BCE
		public virtual bool IsClothingVisible()
		{
			return this.ModalPanelWidget is ClothingWidget;
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x000749DE File Offset: 0x00072BDE
		public virtual bool IsInventoryVisible()
		{
			return this.ModalPanelWidget != null && !this.IsClothingVisible();
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x000749F4 File Offset: 0x00072BF4
		public virtual bool IsActiveSlotEditable()
		{
			IInventory inventory = this.m_componentPlayer.ComponentMiner.Inventory;
			if (inventory != null)
			{
				int activeSlotIndex = inventory.ActiveSlotIndex;
				int slotValue = inventory.GetSlotValue(activeSlotIndex);
				int num = Terrain.ExtractContents(slotValue);
				if (BlocksManager.Blocks[num].IsEditable_(slotValue))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x00074A40 File Offset: 0x00072C40
		public void Draw(Camera camera, int drawOrder)
		{
			ModsManager.HookAction("GuiDraw", delegate(ModLoader modloader)
			{
				modloader.GuiDraw(this, camera, drawOrder);
				return false;
			});
		}

		// Token: 0x040008EE RID: 2286
		public static string fName = "ComponentGui";

		// Token: 0x040008EF RID: 2287
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040008F0 RID: 2288
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040008F1 RID: 2289
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x040008F2 RID: 2290
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040008F3 RID: 2291
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x040008F4 RID: 2292
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040008F5 RID: 2293
		public ContainerWidget m_leftControlsContainerWidget;

		// Token: 0x040008F6 RID: 2294
		public ContainerWidget m_rightControlsContainerWidget;

		// Token: 0x040008F7 RID: 2295
		public ContainerWidget m_moveContainerWidget;

		// Token: 0x040008F8 RID: 2296
		public ContainerWidget m_lookContainerWidget;

		// Token: 0x040008F9 RID: 2297
		public RectangleWidget m_moveRectangleWidget;

		// Token: 0x040008FA RID: 2298
		public RectangleWidget m_lookRectangleWidget;

		// Token: 0x040008FB RID: 2299
		public ContainerWidget m_moveRectangleContainerWidget;

		// Token: 0x040008FC RID: 2300
		public ContainerWidget m_lookRectangleContainerWidget;

		// Token: 0x040008FD RID: 2301
		public ContainerWidget m_movePadContainerWidget;

		// Token: 0x040008FE RID: 2302
		public ContainerWidget m_lookPadContainerWidget;

		// Token: 0x040008FF RID: 2303
		public ContainerWidget m_moveButtonsContainerWidget;

		// Token: 0x04000900 RID: 2304
		public ContainerWidget m_modalPanelContainerWidget;

		// Token: 0x04000901 RID: 2305
		public ContainerWidget m_largeMessageWidget;

		// Token: 0x04000902 RID: 2306
		public MessageWidget m_messageWidget;

		// Token: 0x04000903 RID: 2307
		public ButtonWidget m_backButtonWidget;

		// Token: 0x04000904 RID: 2308
		public ButtonWidget m_inventoryButtonWidget;

		// Token: 0x04000905 RID: 2309
		public ButtonWidget m_clothingButtonWidget;

		// Token: 0x04000906 RID: 2310
		public ButtonWidget m_moreButtonWidget;

		// Token: 0x04000907 RID: 2311
		public Widget m_moreContentsWidget;

		// Token: 0x04000908 RID: 2312
		public ButtonWidget m_lightningButtonWidget;

		// Token: 0x04000909 RID: 2313
		public ButtonWidget m_photoButtonWidget;

		// Token: 0x0400090A RID: 2314
		public ButtonWidget m_helpButtonWidget;

		// Token: 0x0400090B RID: 2315
		public ButtonWidget m_timeOfDayButtonWidget;

		// Token: 0x0400090C RID: 2316
		public ButtonWidget m_cameraButtonWidget;

		// Token: 0x0400090D RID: 2317
		public ButtonWidget m_creativeFlyButtonWidget;

		// Token: 0x0400090E RID: 2318
		public ButtonWidget m_sneakButtonWidget;

		// Token: 0x0400090F RID: 2319
		public ButtonWidget m_mountButtonWidget;

		// Token: 0x04000910 RID: 2320
		public ButtonWidget m_editItemButton;

		// Token: 0x04000911 RID: 2321
		public float m_sidePanelsFactor;

		// Token: 0x04000912 RID: 2322
		public ComponentGui.ModalPanelAnimationData m_modalPanelAnimationData;

		// Token: 0x04000913 RID: 2323
		public ComponentGui.Message m_message;

		// Token: 0x04000914 RID: 2324
		public KeyboardHelpDialog m_keyboardHelpDialog;

		// Token: 0x04000915 RID: 2325
		public GamepadHelpDialog m_gamepadHelpDialog;

		// Token: 0x04000916 RID: 2326
		public double m_lastMountableCreatureSearchTime;

		// Token: 0x04000917 RID: 2327
		public bool m_keyboardHelpMessageShown;

		// Token: 0x04000918 RID: 2328
		public bool m_gamepadHelpMessageShown;

		// Token: 0x04000919 RID: 2329
		public static Func<Widget> OpenClothingWidget;

		// Token: 0x020004CF RID: 1231
		public class ModalPanelAnimationData
		{
			// Token: 0x0400179E RID: 6046
			public Widget NewWidget;

			// Token: 0x0400179F RID: 6047
			public Widget OldWidget;

			// Token: 0x040017A0 RID: 6048
			public float Factor;
		}

		// Token: 0x020004D0 RID: 1232
		public class Message
		{
			// Token: 0x040017A1 RID: 6049
			public string LargeText;

			// Token: 0x040017A2 RID: 6050
			public string SmallText;

			// Token: 0x040017A3 RID: 6051
			public double StartTime;

			// Token: 0x040017A4 RID: 6052
			public float Duration;
		}
	}
}
