using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000179 RID: 377
	public class SettingsControlsScreen : Screen
	{
		// Token: 0x06000896 RID: 2198 RVA: 0x0003435C File Offset: 0x0003255C
		public SettingsControlsScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsControlsScreen", null);
			base.LoadContents(this, node);
			this.m_moveControlModeButton = this.Children.Find<ButtonWidget>("MoveControlMode", true);
			this.m_lookControlModeButton = this.Children.Find<ButtonWidget>("LookControlMode", true);
			this.m_leftHandedLayoutButton = this.Children.Find<ButtonWidget>("LeftHandedLayout", true);
			this.m_flipVerticalAxisButton = this.Children.Find<ButtonWidget>("FlipVerticalAxis", true);
			this.m_AllowInitialIntro = this.Children.Find<ButtonWidget>("AllowInitialIntro", true);
			this.m_autoJumpButton = this.Children.Find<ButtonWidget>("AutoJump", true);
			this.m_horizontalCreativeFlightButton = this.Children.Find<ButtonWidget>("HorizontalCreativeFlight", true);
			this.m_horizontalCreativeFlightPanel = this.Children.Find<ContainerWidget>("HorizontalCreativeFlightPanel", true);
			this.m_moveSensitivitySlider = this.Children.Find<SliderWidget>("MoveSensitivitySlider", true);
			this.m_lookSensitivitySlider = this.Children.Find<SliderWidget>("LookSensitivitySlider", true);
			this.m_gamepadCursorSpeedSlider = this.Children.Find<SliderWidget>("GamepadCursorSpeedSlider", true);
			this.m_gamepadDeadZoneSlider = this.Children.Find<SliderWidget>("GamepadDeadZoneSlider", true);
			this.m_creativeDigTimeSlider = this.Children.Find<SliderWidget>("CreativeDigTimeSlider", true);
			this.m_creativeReachSlider = this.Children.Find<SliderWidget>("CreativeReachSlider", true);
			this.m_holdDurationSlider = this.Children.Find<SliderWidget>("HoldDurationSlider", true);
			this.m_dragDistanceSlider = this.Children.Find<SliderWidget>("DragDistanceSlider", true);
			this.m_MemoryBankStyle = this.Children.Find<ButtonWidget>("MemoryBankStyle", true);
			this.m_horizontalCreativeFlightPanel.IsVisible = false;
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x00034518 File Offset: 0x00032718
		public override void Update()
		{
			if (this.m_moveControlModeButton.IsClicked)
			{
				SettingsManager.MoveControlMode = (SettingsManager.MoveControlMode + 1) % (MoveControlMode)EnumUtils.GetEnumValues(typeof(MoveControlMode)).Count;
			}
			if (this.m_lookControlModeButton.IsClicked)
			{
				SettingsManager.LookControlMode = (SettingsManager.LookControlMode + 1) % (LookControlMode)EnumUtils.GetEnumValues(typeof(LookControlMode)).Count;
			}
			if (this.m_leftHandedLayoutButton.IsClicked)
			{
				SettingsManager.LeftHandedLayout = !SettingsManager.LeftHandedLayout;
			}
			if (this.m_flipVerticalAxisButton.IsClicked)
			{
				SettingsManager.FlipVerticalAxis = !SettingsManager.FlipVerticalAxis;
			}
			if (this.m_autoJumpButton.IsClicked)
			{
				SettingsManager.AutoJump = !SettingsManager.AutoJump;
			}
			if (this.m_horizontalCreativeFlightButton.IsClicked)
			{
				SettingsManager.HorizontalCreativeFlight = !SettingsManager.HorizontalCreativeFlight;
			}
			if (this.m_moveSensitivitySlider.IsSliding)
			{
				SettingsManager.MoveSensitivity = this.m_moveSensitivitySlider.Value;
			}
			if (this.m_lookSensitivitySlider.IsSliding)
			{
				SettingsManager.LookSensitivity = this.m_lookSensitivitySlider.Value;
			}
			if (this.m_gamepadCursorSpeedSlider.IsSliding)
			{
				SettingsManager.GamepadCursorSpeed = this.m_gamepadCursorSpeedSlider.Value;
			}
			if (this.m_gamepadDeadZoneSlider.IsSliding)
			{
				SettingsManager.GamepadDeadZone = this.m_gamepadDeadZoneSlider.Value;
			}
			if (this.m_creativeDigTimeSlider.IsSliding)
			{
				SettingsManager.CreativeDigTime = this.m_creativeDigTimeSlider.Value;
			}
			if (this.m_creativeReachSlider.IsSliding)
			{
				SettingsManager.CreativeReach = this.m_creativeReachSlider.Value;
			}
			if (this.m_holdDurationSlider.IsSliding)
			{
				SettingsManager.MinimumHoldDuration = this.m_holdDurationSlider.Value;
			}
			if (this.m_dragDistanceSlider.IsSliding)
			{
				SettingsManager.MinimumDragDistance = this.m_dragDistanceSlider.Value;
			}
			if (this.m_MemoryBankStyle.IsClicked)
			{
				SettingsManager.UsePrimaryMemoryBank = !SettingsManager.UsePrimaryMemoryBank;
			}
			if (this.m_AllowInitialIntro.IsClicked)
			{
				SettingsManager.AllowInitialIntro = !SettingsManager.AllowInitialIntro;
			}
			this.m_moveControlModeButton.Text = LanguageControl.Get(new string[]
			{
				"MoveControlMode",
				SettingsManager.MoveControlMode.ToString()
			});
			this.m_lookControlModeButton.Text = LanguageControl.Get(new string[]
			{
				"LookControlMode",
				SettingsManager.LookControlMode.ToString()
			});
			this.m_leftHandedLayoutButton.Text = (SettingsManager.LeftHandedLayout ? LanguageControl.On : LanguageControl.Off);
			this.m_flipVerticalAxisButton.Text = (SettingsManager.FlipVerticalAxis ? LanguageControl.On : LanguageControl.Off);
			this.m_MemoryBankStyle.Text = (SettingsManager.UsePrimaryMemoryBank ? LanguageControl.Get(base.GetType().Name, 2) : LanguageControl.Get(base.GetType().Name, 3));
			this.m_AllowInitialIntro.Text = (SettingsManager.AllowInitialIntro ? LanguageControl.On : LanguageControl.Off);
			this.m_autoJumpButton.Text = (SettingsManager.AutoJump ? LanguageControl.On : LanguageControl.Off);
			this.m_horizontalCreativeFlightButton.Text = (SettingsManager.HorizontalCreativeFlight ? LanguageControl.On : LanguageControl.Off);
			this.m_moveSensitivitySlider.Value = SettingsManager.MoveSensitivity;
			this.m_moveSensitivitySlider.Text = MathUtils.Round(SettingsManager.MoveSensitivity * 10f).ToString();
			this.m_lookSensitivitySlider.Value = SettingsManager.LookSensitivity;
			this.m_lookSensitivitySlider.Text = MathUtils.Round(SettingsManager.LookSensitivity * 10f).ToString();
			this.m_gamepadCursorSpeedSlider.Value = SettingsManager.GamepadCursorSpeed;
			this.m_gamepadCursorSpeedSlider.Text = string.Format("{0:0.0}x", SettingsManager.GamepadCursorSpeed);
			this.m_gamepadDeadZoneSlider.Value = SettingsManager.GamepadDeadZone;
			this.m_gamepadDeadZoneSlider.Text = string.Format("{0:0}%", SettingsManager.GamepadDeadZone * 100f);
			this.m_creativeDigTimeSlider.Value = SettingsManager.CreativeDigTime;
			this.m_creativeDigTimeSlider.Text = string.Format("{0}ms", MathUtils.Round(1000f * SettingsManager.CreativeDigTime));
			this.m_creativeReachSlider.Value = SettingsManager.CreativeReach;
			this.m_creativeReachSlider.Text = string.Format(LanguageControl.Get(base.GetType().Name, 0), string.Format("{0:0.0} ", SettingsManager.CreativeReach));
			this.m_holdDurationSlider.Value = SettingsManager.MinimumHoldDuration;
			this.m_holdDurationSlider.Text = string.Format("{0}ms", MathUtils.Round(1000f * SettingsManager.MinimumHoldDuration));
			this.m_dragDistanceSlider.Value = SettingsManager.MinimumDragDistance;
			this.m_dragDistanceSlider.Text = string.Format("{0} ", MathUtils.Round(SettingsManager.MinimumDragDistance)) + LanguageControl.Get(base.GetType().Name, 1);
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x04000403 RID: 1027
		public ButtonWidget m_moveControlModeButton;

		// Token: 0x04000404 RID: 1028
		public ButtonWidget m_lookControlModeButton;

		// Token: 0x04000405 RID: 1029
		public ButtonWidget m_leftHandedLayoutButton;

		// Token: 0x04000406 RID: 1030
		public ButtonWidget m_flipVerticalAxisButton;

		// Token: 0x04000407 RID: 1031
		public ButtonWidget m_autoJumpButton;

		// Token: 0x04000408 RID: 1032
		public ButtonWidget m_AllowInitialIntro;

		// Token: 0x04000409 RID: 1033
		public ButtonWidget m_MemoryBankStyle;

		// Token: 0x0400040A RID: 1034
		public ButtonWidget m_horizontalCreativeFlightButton;

		// Token: 0x0400040B RID: 1035
		public ContainerWidget m_horizontalCreativeFlightPanel;

		// Token: 0x0400040C RID: 1036
		public SliderWidget m_moveSensitivitySlider;

		// Token: 0x0400040D RID: 1037
		public SliderWidget m_lookSensitivitySlider;

		// Token: 0x0400040E RID: 1038
		public SliderWidget m_gamepadCursorSpeedSlider;

		// Token: 0x0400040F RID: 1039
		public SliderWidget m_gamepadDeadZoneSlider;

		// Token: 0x04000410 RID: 1040
		public SliderWidget m_creativeDigTimeSlider;

		// Token: 0x04000411 RID: 1041
		public SliderWidget m_creativeReachSlider;

		// Token: 0x04000412 RID: 1042
		public SliderWidget m_holdDurationSlider;

		// Token: 0x04000413 RID: 1043
		public SliderWidget m_dragDistanceSlider;
	}
}
