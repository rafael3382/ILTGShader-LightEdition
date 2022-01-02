using System;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000162 RID: 354
	public class GameScreen : Screen
	{
		// Token: 0x060007D6 RID: 2006 RVA: 0x0002E080 File Offset: 0x0002C280
		public GameScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/GameScreen", null);
			base.LoadContents(this, node);
			base.IsDrawRequired = true;
			Window.Deactivated += delegate()
			{
				GameManager.SaveProject(true, false);
			};
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x0002E0D2 File Offset: 0x0002C2D2
		public override void Enter(object[] parameters)
		{
			if (GameManager.Project != null)
			{
				GameManager.Project.FindSubsystem<SubsystemAudio>(true).Unmute();
			}
			MusicManager.CurrentMix = MusicManager.Mix.None;
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0002E0F1 File Offset: 0x0002C2F1
		public override void Leave()
		{
			if (GameManager.Project != null)
			{
				GameManager.Project.FindSubsystem<SubsystemAudio>(true).Mute();
				GameManager.SaveProject(true, true);
			}
			this.ShowHideCursors(true);
			MusicManager.CurrentMix = MusicManager.Mix.Menu;
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0002E120 File Offset: 0x0002C320
		public override void Update()
		{
			if (GameManager.Project != null)
			{
				double realTime = Time.RealTime;
				if (realTime - this.m_lastAutosaveTime > 120.0)
				{
					this.m_lastAutosaveTime = realTime;
					GameManager.SaveProject(false, true);
				}
				if (MarketplaceManager.IsTrialMode && GameManager.Project.FindSubsystem<SubsystemGameInfo>(true).TotalElapsedGameTime > 1140.0)
				{
					GameManager.SaveProject(true, false);
					GameManager.DisposeProject();
					ScreensManager.SwitchScreen("TrialEnded", Array.Empty<object>());
				}
				GameManager.UpdateProject();
			}
			this.ShowHideCursors(GameManager.Project == null || DialogsManager.HasDialogs(this) || DialogsManager.HasDialogs(base.RootWidget) || ScreensManager.CurrentScreen != this);
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x0002E1CD File Offset: 0x0002C3CD
		public override void Draw(Widget.DrawContext dc)
		{
			if (!ScreensManager.IsAnimating && SettingsManager.ResolutionMode == ResolutionMode.High)
			{
				Display.Clear(new Color?(Color.Black), new float?(1f), new int?(0));
			}
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x0002E1FD File Offset: 0x0002C3FD
		public void ShowHideCursors(bool show)
		{
			base.Input.IsMouseCursorVisible = show;
			base.Input.IsPadCursorVisible = show;
		}

		// Token: 0x0400036F RID: 879
		public double m_lastAutosaveTime;
	}
}
