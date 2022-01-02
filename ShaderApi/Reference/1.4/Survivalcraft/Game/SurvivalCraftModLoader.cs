using System;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x0200015B RID: 347
	public class SurvivalCraftModLoader : ModLoader
	{
		// Token: 0x060007A5 RID: 1957 RVA: 0x0002B57B File Offset: 0x0002977B
		public override void __ModInitialize()
		{
			ModsManager.RegisterHook("OnCameraChange", this);
			ModsManager.RegisterHook("OnPlayerDead", this);
			ModsManager.RegisterHook("OnModelRendererDrawExtra", this);
			ModsManager.RegisterHook("GetMaxInstancesCount", this);
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x0002B5AC File Offset: 0x000297AC
		public override void OnCameraChange(ComponentPlayer m_componentPlayer, ComponentGui componentGui)
		{
			GameWidget gameWidget = m_componentPlayer.GameWidget;
			if (gameWidget.ActiveCamera is FppCamera)
			{
				gameWidget.ActiveCamera = gameWidget.FindCamera<TppCamera>(true);
				componentGui.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 9), Color.White, false, false);
				return;
			}
			if (gameWidget.ActiveCamera is TppCamera)
			{
				gameWidget.ActiveCamera = gameWidget.FindCamera<OrbitCamera>(true);
				componentGui.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 10), Color.White, false, false);
				return;
			}
			if (gameWidget.ActiveCamera is OrbitCamera)
			{
				gameWidget.ActiveCamera = gameWidget.FindCamera<FixedCamera>(true);
				componentGui.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 11), Color.White, false, false);
				return;
			}
			gameWidget.ActiveCamera = gameWidget.FindCamera<FppCamera>(true);
			componentGui.DisplaySmallMessage(LanguageControl.Get(ComponentGui.fName, 12), Color.White, false, false);
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x0002B684 File Offset: 0x00029884
		public override void OnPlayerDead(PlayerData playerData)
		{
			playerData.GameWidget.ActiveCamera = playerData.GameWidget.FindCamera<DeathCamera>(true);
			if (playerData.ComponentPlayer != null)
			{
				string text = playerData.ComponentPlayer.ComponentHealth.CauseOfDeath;
				if (string.IsNullOrEmpty(text))
				{
					text = LanguageControl.Get(PlayerData.fName, 12);
				}
				string arg = string.Format(LanguageControl.Get(PlayerData.fName, 13), text);
				if (playerData.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Cruel)
				{
					playerData.ComponentPlayer.ComponentGui.DisplayLargeMessage(LanguageControl.Get(PlayerData.fName, 6), string.Format(LanguageControl.Get(PlayerData.fName, 7), arg, LanguageControl.Get(new string[]
					{
						"GameMode",
						playerData.m_subsystemGameInfo.WorldSettings.GameMode.ToString()
					})), 30f, 1.5f);
				}
				else if (playerData.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure && !playerData.m_subsystemGameInfo.WorldSettings.IsAdventureRespawnAllowed)
				{
					playerData.ComponentPlayer.ComponentGui.DisplayLargeMessage(LanguageControl.Get(PlayerData.fName, 6), string.Format(LanguageControl.Get(PlayerData.fName, 8), arg), 30f, 1.5f);
				}
				else
				{
					playerData.ComponentPlayer.ComponentGui.DisplayLargeMessage(LanguageControl.Get(PlayerData.fName, 6), string.Format(LanguageControl.Get(PlayerData.fName, 9), arg), 30f, 1.5f);
				}
			}
			playerData.Level = MathUtils.Max(MathUtils.Floor(playerData.Level / 2f), 1f);
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x0002B820 File Offset: 0x00029A20
		public override void OnModelRendererDrawExtra(SubsystemModelsRenderer modelsRenderer, ComponentModel componentModel, Camera camera, float? alphaThreshold)
		{
			if (componentModel is ComponentHumanModel)
			{
				ComponentPlayer componentPlayer = componentModel.Entity.FindComponent<ComponentPlayer>();
				if (componentPlayer != null && camera.GameWidget.PlayerData != componentPlayer.PlayerData)
				{
					ComponentCreature componentCreature = componentPlayer.ComponentMiner.ComponentCreature;
					Vector3 vector = Vector3.Transform(componentCreature.ComponentBody.Position + 1.02f * Vector3.UnitY * componentCreature.ComponentBody.BoxSize.Y, camera.ViewMatrix);
					if (vector.Z < 0f)
					{
						Color color = Color.Lerp(Color.White, Color.Transparent, MathUtils.Saturate((vector.Length() - 4f) / 3f));
						if (color.A > 8)
						{
							Vector3 right = Vector3.TransformNormal(0.005f * Vector3.Normalize(Vector3.Cross(camera.ViewDirection, Vector3.UnitY)), camera.ViewMatrix);
							Vector3 down = Vector3.TransformNormal(-0.005f * Vector3.UnitY, camera.ViewMatrix);
							BitmapFont bitmapFont = LabelWidget.BitmapFont;
							modelsRenderer.PrimitivesRenderer.FontBatch(bitmapFont, 1, DepthStencilState.DepthRead, RasterizerState.CullNoneScissor, BlendState.AlphaBlend, SamplerState.LinearClamp).QueueText(componentPlayer.PlayerData.Name, vector, right, down, color, TextAnchor.HorizontalCenter | TextAnchor.Bottom);
						}
					}
				}
			}
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x0002B97A File Offset: 0x00029B7A
		public override int GetMaxInstancesCount()
		{
			return 7;
		}
	}
}
