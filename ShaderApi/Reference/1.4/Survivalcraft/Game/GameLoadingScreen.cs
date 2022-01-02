using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000161 RID: 353
	public class GameLoadingScreen : Screen
	{
		// Token: 0x060007CF RID: 1999 RVA: 0x0002DEA4 File Offset: 0x0002C0A4
		public GameLoadingScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/GameLoadingScreen", null);
			base.LoadContents(this, node);
			this.m_stateMachine.AddState("WaitingForFadeIn", null, delegate
			{
				if (!ScreensManager.IsAnimating)
				{
					if (string.IsNullOrEmpty(this.m_worldSnapshotName))
					{
						this.m_stateMachine.TransitionTo("Loading");
						return;
					}
					this.m_stateMachine.TransitionTo("RestoringSnapshot");
				}
			}, null);
			this.m_stateMachine.AddState("Loading", null, delegate
			{
				ContainerWidget gamesWidget = ScreensManager.FindScreen<GameScreen>("Game").Children.Find<ContainerWidget>("GamesWidget", true);
				GameManager.LoadProject(this.m_worldInfo, gamesWidget);
				ScreensManager.SwitchScreen("Game", Array.Empty<object>());
			}, null);
			this.m_stateMachine.AddState("RestoringSnapshot", null, delegate
			{
				GameManager.DisposeProject();
				WorldsManager.RestoreWorldFromSnapshot(this.m_worldInfo.DirectoryName, this.m_worldSnapshotName);
				this.m_stateMachine.TransitionTo("Loading");
			}, null);
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x0002DF30 File Offset: 0x0002C130
		public override void Update()
		{
			try
			{
				this.m_stateMachine.Update();
			}
			catch (Exception e)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(GameLoadingScreen.fName, 1), ExceptionManager.MakeFullErrorMessage(e), LanguageControl.Ok, null, null));
			}
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x0002DF90 File Offset: 0x0002C190
		public override void Enter(object[] parameters)
		{
			this.m_worldInfo = (WorldInfo)parameters[0];
			this.m_worldSnapshotName = (string)parameters[1];
			this.m_stateMachine.TransitionTo("WaitingForFadeIn");
			ProgressManager.UpdateProgress("Loading World", 0f);
		}

		// Token: 0x0400036B RID: 875
		public WorldInfo m_worldInfo;

		// Token: 0x0400036C RID: 876
		public string m_worldSnapshotName;

		// Token: 0x0400036D RID: 877
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x0400036E RID: 878
		public static string fName = "GameLoadingScreen";
	}
}
