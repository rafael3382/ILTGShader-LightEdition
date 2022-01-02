using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Engine.Serialization;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200014C RID: 332
	public static class SettingsManager
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000690 RID: 1680 RVA: 0x00026235 File Offset: 0x00024435
		// (set) Token: 0x06000691 RID: 1681 RVA: 0x0002623C File Offset: 0x0002443C
		public static bool UsePrimaryMemoryBank { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000692 RID: 1682 RVA: 0x00026244 File Offset: 0x00024444
		// (set) Token: 0x06000693 RID: 1683 RVA: 0x0002624B File Offset: 0x0002444B
		public static bool AllowInitialIntro { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000694 RID: 1684 RVA: 0x00026253 File Offset: 0x00024453
		// (set) Token: 0x06000695 RID: 1685 RVA: 0x0002625A File Offset: 0x0002445A
		public static float SoundsVolume
		{
			get
			{
				return SettingsManager.m_soundsVolume;
			}
			set
			{
				SettingsManager.m_soundsVolume = MathUtils.Saturate(value);
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000696 RID: 1686 RVA: 0x00026267 File Offset: 0x00024467
		// (set) Token: 0x06000697 RID: 1687 RVA: 0x0002626E File Offset: 0x0002446E
		public static float MusicVolume
		{
			get
			{
				return SettingsManager.m_musicVolume;
			}
			set
			{
				SettingsManager.m_musicVolume = MathUtils.Saturate(value);
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000698 RID: 1688 RVA: 0x0002627B File Offset: 0x0002447B
		// (set) Token: 0x06000699 RID: 1689 RVA: 0x00026282 File Offset: 0x00024482
		public static int VisibilityRange { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x0002628A File Offset: 0x0002448A
		// (set) Token: 0x0600069B RID: 1691 RVA: 0x00026291 File Offset: 0x00024491
		public static bool UseVr { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600069C RID: 1692 RVA: 0x00026299 File Offset: 0x00024499
		// (set) Token: 0x0600069D RID: 1693 RVA: 0x000262A0 File Offset: 0x000244A0
		public static ResolutionMode ResolutionMode
		{
			get
			{
				return SettingsManager.m_resolutionMode;
			}
			set
			{
				if (value != SettingsManager.m_resolutionMode)
				{
					SettingsManager.m_resolutionMode = value;
					Action<string> settingChanged = SettingsManager.SettingChanged;
					if (settingChanged == null)
					{
						return;
					}
					settingChanged("ResolutionMode");
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x000262C4 File Offset: 0x000244C4
		// (set) Token: 0x0600069F RID: 1695 RVA: 0x000262CB File Offset: 0x000244CB
		public static ViewAngleMode ViewAngleMode { get; set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x000262D3 File Offset: 0x000244D3
		// (set) Token: 0x060006A1 RID: 1697 RVA: 0x000262DA File Offset: 0x000244DA
		public static SkyRenderingMode SkyRenderingMode { get; set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x000262E2 File Offset: 0x000244E2
		// (set) Token: 0x060006A3 RID: 1699 RVA: 0x000262E9 File Offset: 0x000244E9
		public static bool TerrainMipmapsEnabled { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x000262F1 File Offset: 0x000244F1
		// (set) Token: 0x060006A5 RID: 1701 RVA: 0x000262F8 File Offset: 0x000244F8
		public static bool ObjectsShadowsEnabled { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060006A6 RID: 1702 RVA: 0x00026300 File Offset: 0x00024500
		// (set) Token: 0x060006A7 RID: 1703 RVA: 0x00026307 File Offset: 0x00024507
		public static float Brightness
		{
			get
			{
				return SettingsManager.m_brightness;
			}
			set
			{
				value = MathUtils.Clamp(value, 0f, 1f);
				if (value != SettingsManager.m_brightness)
				{
					SettingsManager.m_brightness = value;
					Action<string> settingChanged = SettingsManager.SettingChanged;
					if (settingChanged == null)
					{
						return;
					}
					settingChanged("Brightness");
				}
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060006A8 RID: 1704 RVA: 0x0002633D File Offset: 0x0002453D
		// (set) Token: 0x060006A9 RID: 1705 RVA: 0x00026344 File Offset: 0x00024544
		public static int PresentationInterval { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060006AA RID: 1706 RVA: 0x0002634C File Offset: 0x0002454C
		// (set) Token: 0x060006AB RID: 1707 RVA: 0x00026353 File Offset: 0x00024553
		public static bool ShowGuiInScreenshots { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x0002635B File Offset: 0x0002455B
		// (set) Token: 0x060006AD RID: 1709 RVA: 0x00026362 File Offset: 0x00024562
		public static bool ShowLogoInScreenshots { get; set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060006AE RID: 1710 RVA: 0x0002636A File Offset: 0x0002456A
		// (set) Token: 0x060006AF RID: 1711 RVA: 0x00026371 File Offset: 0x00024571
		public static ScreenshotSize ScreenshotSize { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060006B0 RID: 1712 RVA: 0x00026379 File Offset: 0x00024579
		// (set) Token: 0x060006B1 RID: 1713 RVA: 0x00026380 File Offset: 0x00024580
		public static WindowMode WindowMode
		{
			get
			{
				return SettingsManager.m_windowMode;
			}
			set
			{
				if (value != SettingsManager.m_windowMode)
				{
					if (value == WindowMode.Borderless)
					{
						SettingsManager.m_resizableWindowSize = Window.Size;
						Window.Position = Point2.Zero;
						Window.Size = Window.ScreenSize;
					}
					else if (value == WindowMode.Resizable)
					{
						Window.Position = SettingsManager.m_resizableWindowPosition;
						Window.Size = SettingsManager.m_resizableWindowSize;
					}
					Window.WindowMode = value;
					SettingsManager.m_windowMode = value;
				}
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060006B2 RID: 1714 RVA: 0x000263DC File Offset: 0x000245DC
		// (set) Token: 0x060006B3 RID: 1715 RVA: 0x000263E3 File Offset: 0x000245E3
		public static GuiSize GuiSize { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060006B4 RID: 1716 RVA: 0x000263EB File Offset: 0x000245EB
		// (set) Token: 0x060006B5 RID: 1717 RVA: 0x000263F2 File Offset: 0x000245F2
		public static bool HideMoveLookPads { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x000263FA File Offset: 0x000245FA
		// (set) Token: 0x060006B7 RID: 1719 RVA: 0x00026401 File Offset: 0x00024601
		public static string BlocksTextureFileName { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x00026409 File Offset: 0x00024609
		// (set) Token: 0x060006B9 RID: 1721 RVA: 0x00026410 File Offset: 0x00024610
		public static MoveControlMode MoveControlMode { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060006BA RID: 1722 RVA: 0x00026418 File Offset: 0x00024618
		// (set) Token: 0x060006BB RID: 1723 RVA: 0x0002641F File Offset: 0x0002461F
		public static LookControlMode LookControlMode { get; set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060006BC RID: 1724 RVA: 0x00026427 File Offset: 0x00024627
		// (set) Token: 0x060006BD RID: 1725 RVA: 0x0002642E File Offset: 0x0002462E
		public static bool LeftHandedLayout { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x00026436 File Offset: 0x00024636
		// (set) Token: 0x060006BF RID: 1727 RVA: 0x0002643D File Offset: 0x0002463D
		public static bool FlipVerticalAxis { get; set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x00026445 File Offset: 0x00024645
		// (set) Token: 0x060006C1 RID: 1729 RVA: 0x0002644C File Offset: 0x0002464C
		public static float MoveSensitivity { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00026454 File Offset: 0x00024654
		// (set) Token: 0x060006C3 RID: 1731 RVA: 0x0002645B File Offset: 0x0002465B
		public static float LookSensitivity { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00026463 File Offset: 0x00024663
		// (set) Token: 0x060006C5 RID: 1733 RVA: 0x0002646A File Offset: 0x0002466A
		public static float GamepadDeadZone { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x00026472 File Offset: 0x00024672
		// (set) Token: 0x060006C7 RID: 1735 RVA: 0x00026479 File Offset: 0x00024679
		public static float GamepadCursorSpeed { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x00026481 File Offset: 0x00024681
		// (set) Token: 0x060006C9 RID: 1737 RVA: 0x00026488 File Offset: 0x00024688
		public static float CreativeDigTime { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060006CA RID: 1738 RVA: 0x00026490 File Offset: 0x00024690
		// (set) Token: 0x060006CB RID: 1739 RVA: 0x00026497 File Offset: 0x00024697
		public static float CreativeReach { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060006CC RID: 1740 RVA: 0x0002649F File Offset: 0x0002469F
		// (set) Token: 0x060006CD RID: 1741 RVA: 0x000264A6 File Offset: 0x000246A6
		public static float MinimumHoldDuration { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060006CE RID: 1742 RVA: 0x000264AE File Offset: 0x000246AE
		// (set) Token: 0x060006CF RID: 1743 RVA: 0x000264B5 File Offset: 0x000246B5
		public static float MinimumDragDistance { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060006D0 RID: 1744 RVA: 0x000264BD File Offset: 0x000246BD
		// (set) Token: 0x060006D1 RID: 1745 RVA: 0x000264C4 File Offset: 0x000246C4
		public static bool AutoJump { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060006D2 RID: 1746 RVA: 0x000264CC File Offset: 0x000246CC
		// (set) Token: 0x060006D3 RID: 1747 RVA: 0x000264D3 File Offset: 0x000246D3
		public static bool HorizontalCreativeFlight { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060006D4 RID: 1748 RVA: 0x000264DB File Offset: 0x000246DB
		// (set) Token: 0x060006D5 RID: 1749 RVA: 0x000264E2 File Offset: 0x000246E2
		public static string DropboxAccessToken { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060006D6 RID: 1750 RVA: 0x000264EA File Offset: 0x000246EA
		// (set) Token: 0x060006D7 RID: 1751 RVA: 0x000264F1 File Offset: 0x000246F1
		public static string MotdUpdateUrl { get; set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x000264F9 File Offset: 0x000246F9
		// (set) Token: 0x060006D9 RID: 1753 RVA: 0x00026500 File Offset: 0x00024700
		public static string MotdUpdateCheckUrl { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x00026508 File Offset: 0x00024708
		// (set) Token: 0x060006DB RID: 1755 RVA: 0x0002650F File Offset: 0x0002470F
		public static string ScpboxAccessToken { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x00026517 File Offset: 0x00024717
		// (set) Token: 0x060006DD RID: 1757 RVA: 0x0002651E File Offset: 0x0002471E
		public static bool MotdUseBackupUrl { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x00026526 File Offset: 0x00024726
		// (set) Token: 0x060006DF RID: 1759 RVA: 0x0002652D File Offset: 0x0002472D
		public static double MotdUpdatePeriodHours { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x00026535 File Offset: 0x00024735
		// (set) Token: 0x060006E1 RID: 1761 RVA: 0x0002653C File Offset: 0x0002473C
		public static DateTime MotdLastUpdateTime { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060006E2 RID: 1762 RVA: 0x00026544 File Offset: 0x00024744
		// (set) Token: 0x060006E3 RID: 1763 RVA: 0x0002654B File Offset: 0x0002474B
		public static string MotdLastDownloadedData { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x00026553 File Offset: 0x00024753
		// (set) Token: 0x060006E5 RID: 1765 RVA: 0x0002655A File Offset: 0x0002475A
		public static string UserId { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x00026562 File Offset: 0x00024762
		// (set) Token: 0x060006E7 RID: 1767 RVA: 0x00026569 File Offset: 0x00024769
		public static string LastLaunchedVersion { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x00026571 File Offset: 0x00024771
		// (set) Token: 0x060006E9 RID: 1769 RVA: 0x00026578 File Offset: 0x00024778
		public static CommunityContentMode CommunityContentMode { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060006EA RID: 1770 RVA: 0x00026580 File Offset: 0x00024780
		// (set) Token: 0x060006EB RID: 1771 RVA: 0x00026587 File Offset: 0x00024787
		public static bool MultithreadedTerrainUpdate { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060006EC RID: 1772 RVA: 0x0002658F File Offset: 0x0002478F
		// (set) Token: 0x060006ED RID: 1773 RVA: 0x00026596 File Offset: 0x00024796
		public static bool EnableAndroidAudioTrackCaching { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060006EE RID: 1774 RVA: 0x0002659E File Offset: 0x0002479E
		// (set) Token: 0x060006EF RID: 1775 RVA: 0x000265A5 File Offset: 0x000247A5
		public static bool UseReducedZRange { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060006F0 RID: 1776 RVA: 0x000265AD File Offset: 0x000247AD
		// (set) Token: 0x060006F1 RID: 1777 RVA: 0x000265B4 File Offset: 0x000247B4
		public static int IsolatedStorageMigrationCounter { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060006F2 RID: 1778 RVA: 0x000265BC File Offset: 0x000247BC
		// (set) Token: 0x060006F3 RID: 1779 RVA: 0x000265C3 File Offset: 0x000247C3
		public static bool DisplayFpsCounter { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060006F4 RID: 1780 RVA: 0x000265CB File Offset: 0x000247CB
		// (set) Token: 0x060006F5 RID: 1781 RVA: 0x000265D2 File Offset: 0x000247D2
		public static bool DisplayFpsRibbon { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060006F6 RID: 1782 RVA: 0x000265DA File Offset: 0x000247DA
		// (set) Token: 0x060006F7 RID: 1783 RVA: 0x000265E1 File Offset: 0x000247E1
		public static int NewYearCelebrationLastYear { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060006F8 RID: 1784 RVA: 0x000265E9 File Offset: 0x000247E9
		// (set) Token: 0x060006F9 RID: 1785 RVA: 0x000265F0 File Offset: 0x000247F0
		public static ScreenLayout ScreenLayout1 { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060006FA RID: 1786 RVA: 0x000265F8 File Offset: 0x000247F8
		// (set) Token: 0x060006FB RID: 1787 RVA: 0x000265FF File Offset: 0x000247FF
		public static ScreenLayout ScreenLayout2 { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060006FC RID: 1788 RVA: 0x00026607 File Offset: 0x00024807
		// (set) Token: 0x060006FD RID: 1789 RVA: 0x0002660E File Offset: 0x0002480E
		public static ScreenLayout ScreenLayout3 { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060006FE RID: 1790 RVA: 0x00026616 File Offset: 0x00024816
		// (set) Token: 0x060006FF RID: 1791 RVA: 0x0002661D File Offset: 0x0002481D
		public static ScreenLayout ScreenLayout4 { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000700 RID: 1792 RVA: 0x00026625 File Offset: 0x00024825
		// (set) Token: 0x06000701 RID: 1793 RVA: 0x0002662C File Offset: 0x0002482C
		public static bool UpsideDownLayout { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000702 RID: 1794 RVA: 0x00026634 File Offset: 0x00024834
		// (set) Token: 0x06000703 RID: 1795 RVA: 0x0002663E File Offset: 0x0002483E
		public static bool FullScreenMode
		{
			get
			{
				return Window.WindowMode == WindowMode.Fullscreen;
			}
			set
			{
				Window.WindowMode = (value ? WindowMode.Fullscreen : WindowMode.Resizable);
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000704 RID: 1796 RVA: 0x0002664C File Offset: 0x0002484C
		// (set) Token: 0x06000705 RID: 1797 RVA: 0x00026653 File Offset: 0x00024853
		public static bool DisplayLog { get; set; }

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000706 RID: 1798 RVA: 0x0002665C File Offset: 0x0002485C
		// (remove) Token: 0x06000707 RID: 1799 RVA: 0x00026690 File Offset: 0x00024890
		public static event Action<string> SettingChanged;

		// Token: 0x06000708 RID: 1800 RVA: 0x000266C4 File Offset: 0x000248C4
		public static void Initialize()
		{
			SettingsManager.DisplayLog = false;
			SettingsManager.VisibilityRange = 128;
			SettingsManager.m_resolutionMode = ResolutionMode.High;
			SettingsManager.ViewAngleMode = ViewAngleMode.Normal;
			SettingsManager.SkyRenderingMode = SkyRenderingMode.Full;
			SettingsManager.TerrainMipmapsEnabled = false;
			SettingsManager.ObjectsShadowsEnabled = true;
			SettingsManager.m_soundsVolume = 0.5f;
			SettingsManager.m_musicVolume = 0.5f;
			SettingsManager.m_brightness = 0.5f;
			SettingsManager.PresentationInterval = 1;
			SettingsManager.ShowGuiInScreenshots = false;
			SettingsManager.ShowLogoInScreenshots = true;
			SettingsManager.ScreenshotSize = ScreenshotSize.ScreenSize;
			SettingsManager.MoveControlMode = MoveControlMode.Pad;
			SettingsManager.HideMoveLookPads = false;
			SettingsManager.AllowInitialIntro = true;
			SettingsManager.BlocksTextureFileName = string.Empty;
			SettingsManager.LookControlMode = LookControlMode.EntireScreen;
			SettingsManager.FlipVerticalAxis = false;
			SettingsManager.GuiSize = GuiSize.Smallest;
			SettingsManager.EnableAndroidAudioTrackCaching = false;
			SettingsManager.MoveSensitivity = 0.5f;
			SettingsManager.LookSensitivity = 0.5f;
			SettingsManager.GamepadDeadZone = 0.16f;
			SettingsManager.GamepadCursorSpeed = 1f;
			SettingsManager.CreativeDigTime = 0.2f;
			SettingsManager.CreativeReach = 7.5f;
			SettingsManager.MinimumHoldDuration = 0.5f;
			SettingsManager.MinimumDragDistance = 10f;
			SettingsManager.AutoJump = true;
			SettingsManager.HorizontalCreativeFlight = false;
			SettingsManager.DropboxAccessToken = string.Empty;
			SettingsManager.ScpboxAccessToken = string.Empty;
			SettingsManager.MotdUpdateUrl = "https://m.schub.top/com/motd?v={0}&l={1}";
			SettingsManager.MotdUpdateCheckUrl = "https://m.schub.top/com/motd?v={0}&cmd=version_check&platform={1}&apiv={2}&l={3}";
			SettingsManager.MotdUpdatePeriodHours = 12.0;
			SettingsManager.MotdLastUpdateTime = DateTime.MinValue;
			SettingsManager.MotdLastDownloadedData = string.Empty;
			SettingsManager.UserId = string.Empty;
			SettingsManager.LastLaunchedVersion = string.Empty;
			SettingsManager.CommunityContentMode = CommunityContentMode.Normal;
			SettingsManager.MultithreadedTerrainUpdate = true;
			SettingsManager.NewYearCelebrationLastYear = 2015;
			SettingsManager.ScreenLayout1 = ScreenLayout.Single;
			SettingsManager.ScreenLayout2 = (((float)Window.ScreenSize.X / (float)Window.ScreenSize.Y > 1.33333337f) ? ScreenLayout.DoubleVertical : ScreenLayout.DoubleHorizontal);
			SettingsManager.ScreenLayout3 = (((float)Window.ScreenSize.X / (float)Window.ScreenSize.Y > 1.33333337f) ? ScreenLayout.TripleVertical : ScreenLayout.TripleHorizontal);
			SettingsManager.ScreenLayout4 = ScreenLayout.Quadruple;
			SettingsManager.HorizontalCreativeFlight = true;
			SettingsManager.LoadSettings();
			VersionsManager.CompareVersions(SettingsManager.LastLaunchedVersion, "1.29");
			if (VersionsManager.CompareVersions(SettingsManager.LastLaunchedVersion, "2.1") < 0)
			{
				SettingsManager.MinimumDragDistance = 10f;
			}
			if (VersionsManager.CompareVersions(SettingsManager.LastLaunchedVersion, "2.2") < 0)
			{
				if (Utilities.GetTotalAvailableMemory() < 524288000)
				{
					SettingsManager.VisibilityRange = MathUtils.Min(64, SettingsManager.VisibilityRange);
				}
				else if (Utilities.GetTotalAvailableMemory() < 1048576000)
				{
					SettingsManager.VisibilityRange = MathUtils.Min(112, SettingsManager.VisibilityRange);
				}
			}
			Window.Deactivated += delegate()
			{
				SettingsManager.SaveSettings();
			};
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x0002693C File Offset: 0x00024B3C
		public static void LoadSettings()
		{
			try
			{
				if (Storage.FileExists("app:/Settings.xml"))
				{
					using (Stream stream = Storage.OpenFile("app:/Settings.xml", OpenFileMode.Read))
					{
						ModsManager.DisabledMods.Clear();
						XElement xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
						ModsManager.LoadSettings(xelement);
						foreach (XElement xelement2 in xelement.Elements())
						{
							string name = "<unknown>";
							try
							{
								if (xelement2.Name.LocalName == "Setting")
								{
									name = XmlUtils.GetAttributeValue<string>(xelement2, "Name");
									string attributeValue = XmlUtils.GetAttributeValue<string>(xelement2, "Value");
									PropertyInfo propertyInfo = (from pi in typeof(SettingsManager).GetRuntimeProperties()
									where pi.Name == name && pi.GetMethod.IsStatic && pi.GetMethod.IsPublic && pi.SetMethod.IsPublic
									select pi).FirstOrDefault<PropertyInfo>();
									if (propertyInfo != null)
									{
										object value = HumanReadableConverter.ConvertFromString(propertyInfo.PropertyType, attributeValue);
										propertyInfo.SetValue(null, value, null);
									}
								}
								else if (xelement2.Name.LocalName == "DisableMods")
								{
									foreach (XElement xelement3 in xelement2.Elements())
									{
										ModInfo modInfo = new ModInfo();
										modInfo.PackageName = xelement3.Attribute("PackageName").Value;
										modInfo.Version = xelement3.Attribute("Version").Value;
										ModsManager.DisabledMods.Add(modInfo);
									}
								}
							}
							catch (Exception ex)
							{
								Log.Warning(string.Format("Setting \"{0}\" could not be loaded. Reason: {1}", new object[]
								{
									name,
									ex.Message
								}));
							}
						}
					}
					Log.Information("Loaded settings.");
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Loading settings failed.", e);
			}
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x00026BA4 File Offset: 0x00024DA4
		public static void SaveSettings()
		{
			try
			{
				XElement xelement = new XElement("Settings");
				foreach (PropertyInfo propertyInfo in from pi in typeof(SettingsManager).GetRuntimeProperties()
				where pi.GetMethod.IsStatic && pi.GetMethod.IsPublic && pi.SetMethod.IsPublic
				select pi)
				{
					try
					{
						string value = HumanReadableConverter.ConvertToString(propertyInfo.GetValue(null, null));
						XElement node = XmlUtils.AddElement(xelement, "Setting");
						XmlUtils.SetAttributeValue(node, "Name", propertyInfo.Name);
						XmlUtils.SetAttributeValue(node, "Value", value);
					}
					catch (Exception ex)
					{
						Log.Warning(string.Format("Setting \"{0}\" could not be saved. Reason: {1}", new object[]
						{
							propertyInfo.Name,
							ex.Message
						}));
					}
				}
				XElement xelement2 = new XElement("DisableMods");
				XElement xelement3 = new XElement("ModSettings");
				foreach (ModEntity modEntity in ModsManager.ModListAll)
				{
					if (ModsManager.DisabledMods.Contains(modEntity.modInfo))
					{
						XElement xelement4 = new XElement("Mod");
						xelement4.SetAttributeValue("PackageName", modEntity.modInfo.PackageName);
						xelement4.SetAttributeValue("Version", modEntity.modInfo.Version);
						xelement2.Add(xelement4);
					}
				}
				xelement.Add(xelement2);
				ModsManager.SaveSettings(xelement);
				ModsManager.SaveModSettings(xelement3);
				using (Stream stream = Storage.OpenFile("app:/Settings.xml", OpenFileMode.Create))
				{
					XmlUtils.SaveXmlToStream(xelement, stream, null, true);
				}
				using (Stream stream2 = Storage.OpenFile("app:/ModSettings.xml", OpenFileMode.Create))
				{
					XmlUtils.SaveXmlToStream(xelement3, stream2, null, true);
				}
				Log.Information("Saved settings");
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Saving settings failed.", e);
			}
		}

		// Token: 0x040002E4 RID: 740
		public static float m_soundsVolume;

		// Token: 0x040002E5 RID: 741
		public static float m_musicVolume;

		// Token: 0x040002E6 RID: 742
		public static float m_brightness;

		// Token: 0x040002E7 RID: 743
		public static ResolutionMode m_resolutionMode;

		// Token: 0x040002E8 RID: 744
		public static WindowMode m_windowMode;

		// Token: 0x040002E9 RID: 745
		public static Point2 m_resizableWindowPosition;

		// Token: 0x040002EA RID: 746
		public static Point2 m_resizableWindowSize;
	}
}
