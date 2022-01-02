using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000214 RID: 532
	public class ComponentIntro : Component, IUpdateable
	{
		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06001014 RID: 4116 RVA: 0x00078E1B File Offset: 0x0007701B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x00078E20 File Offset: 0x00077020
		public static Vector2 FindOceanDirection(ITerrainContentsGenerator generator, Vector2 position)
		{
			float num = float.MaxValue;
			Vector2 result = Vector2.Zero;
			for (int i = 0; i < 36; i++)
			{
				Vector2 vector = Vector2.CreateFromAngle((float)i / 36f * 2f * 3.14159274f);
				Vector2 vector2 = position + 50f * vector;
				float num2 = generator.CalculateOceanShoreDistance(vector2.X, vector2.Y);
				if (num2 < num)
				{
					result = vector;
					num = num2;
				}
			}
			return result;
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x00078E94 File Offset: 0x00077094
		public void Update(float dt)
		{
			if (this.m_playIntro)
			{
				this.m_playIntro = false;
				this.m_stateMachine.TransitionTo("ShipView");
			}
			this.m_stateMachine.Update();
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x00078EC0 File Offset: 0x000770C0
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_playIntro = (valuesDictionary.GetValue<bool>("PlayIntro") && SettingsManager.AllowInitialIntro);
			this.m_stateMachine.AddState("ShipView", new Action(this.ShipView_Enter), new Action(this.ShipView_Update), null);
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x00078F5B File Offset: 0x0007715B
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<bool>("PlayIntro", this.m_playIntro);
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x00078F70 File Offset: 0x00077170
		public virtual void ShipView_Enter()
		{
			ComponentBody componentBody = this.m_componentPlayer.Entity.FindComponent<ComponentBody>(true);
			Vector2 vector = ComponentIntro.FindOceanDirection(this.m_subsystemTerrain.TerrainContentsGenerator, componentBody.Position.XZ);
			Vector2 vector2 = componentBody.Position.XZ + 25f * vector;
			bool isPlayerMounted = this.m_componentPlayer.ComponentRider.Mount != null;
			Vector2 vector3 = vector2;
			float num = float.MinValue;
			for (int i = Terrain.ToCell(vector2.Y) - 15; i < Terrain.ToCell(vector2.Y) + 15; i++)
			{
				for (int j = Terrain.ToCell(vector2.X) - 15; j < Terrain.ToCell(vector2.X) + 15; j++)
				{
					float num2 = this.ScoreShipPosition(componentBody.Position.XZ, j, i);
					if (num2 > num)
					{
						num = num2;
						vector3 = new Vector2((float)j, (float)i);
					}
				}
			}
			DatabaseObject databaseObject = base.Project.GameDatabase.Database.FindDatabaseObject("IntroShip", base.Project.GameDatabase.EntityTemplateType, true);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(databaseObject);
			Entity entity = base.Project.CreateEntity(valuesDictionary);
			Vector3 vector4 = new Vector3(vector3.X, (float)this.m_subsystemTerrain.TerrainContentsGenerator.OceanLevel + 0.5f, vector3.Y);
			entity.FindComponent<ComponentFrame>(true).Position = vector4;
			entity.FindComponent<ComponentIntroShip>(true).Heading = Vector2.Angle(vector, -Vector2.UnitY);
			base.Project.AddEntity(entity);
			this.m_subsystemTime.QueueGameTimeDelayedExecution(2.0, delegate
			{
				this.m_componentPlayer.ComponentGui.DisplayLargeMessage(null, LanguageControl.Get(ComponentIntro.fName, 1), 5f, 0f);
			});
			this.m_subsystemTime.QueueGameTimeDelayedExecution(7.0, delegate
			{
				if (isPlayerMounted)
				{
					this.m_componentPlayer.ComponentGui.DisplayLargeMessage(null, LanguageControl.Get(ComponentIntro.fName, 2), 5f, 0f);
					return;
				}
				this.m_componentPlayer.ComponentGui.DisplayLargeMessage(null, LanguageControl.Get(ComponentIntro.fName, 3), 5f, 0f);
			});
			this.m_subsystemTime.QueueGameTimeDelayedExecution(12.0, delegate
			{
				this.m_componentPlayer.ComponentGui.DisplayLargeMessage(null, LanguageControl.Get(ComponentIntro.fName, 4), 5f, 0f);
			});
			IntroCamera introCamera = this.m_componentPlayer.GameWidget.FindCamera<IntroCamera>(true);
			this.m_componentPlayer.GameWidget.ActiveCamera = introCamera;
			introCamera.CameraPosition = vector4 + new Vector3(12f * vector.X, 8f, 12f * vector.Y) + new Vector3(-5f * vector.Y, 0f, 5f * vector.X);
			introCamera.TargetPosition = this.m_componentPlayer.ComponentCreatureModel.EyePosition + 2.5f * new Vector3(vector.X, 0f, vector.Y);
			introCamera.Speed = 0f;
			introCamera.TargetCameraPosition = this.m_componentPlayer.ComponentCreatureModel.EyePosition;
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x00079274 File Offset: 0x00077474
		public virtual void ShipView_Update()
		{
			IntroCamera introCamera = this.m_componentPlayer.GameWidget.FindCamera<IntroCamera>(true);
			introCamera.Speed = MathUtils.Lerp(0f, 8f, MathUtils.Saturate(((float)this.m_subsystemGameInfo.TotalElapsedGameTime - 6f) / 3f));
			if (Vector3.Distance(introCamera.TargetCameraPosition, introCamera.CameraPosition) < 0.3f)
			{
				this.m_componentPlayer.GameWidget.ActiveCamera = this.m_componentPlayer.GameWidget.FindCamera<FppCamera>(true);
				this.m_stateMachine.TransitionTo(null);
			}
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x0007930C File Offset: 0x0007750C
		public virtual float ScoreShipPosition(Vector2 playerPosition, int x, int z)
		{
			float num = 0f;
			float num2 = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)x, (float)z);
			if (num2 > -8f)
			{
				num -= 100f;
			}
			num -= 0.25f * num2;
			float num3 = Vector2.Distance(playerPosition, new Vector2((float)x, (float)z));
			num -= MathUtils.Abs(num3 - 20f);
			int num4 = 0;
			TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(x, z);
			if (chunkAtCell != null && chunkAtCell.State >= TerrainChunkState.InvalidLight)
			{
				int oceanLevel = this.m_subsystemTerrain.TerrainContentsGenerator.OceanLevel;
				int i = oceanLevel;
				while (i >= oceanLevel - 5)
				{
					if (i < 0)
					{
						break;
					}
					int cellContentsFast = chunkAtCell.GetCellContentsFast(x & 15, i, z & 15);
					if (cellContentsFast != 18 && cellContentsFast != 92)
					{
						break;
					}
					i--;
					num4++;
				}
			}
			else
			{
				num4 = 2;
			}
			if (num4 < 2)
			{
				num -= 100f;
			}
			return num + 2f * (float)num4;
		}

		// Token: 0x04000988 RID: 2440
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000989 RID: 2441
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400098A RID: 2442
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x0400098B RID: 2443
		public ComponentPlayer m_componentPlayer;

		// Token: 0x0400098C RID: 2444
		public bool m_playIntro;

		// Token: 0x0400098D RID: 2445
		public static string fName = "ComponentIntro";

		// Token: 0x0400098E RID: 2446
		public StateMachine m_stateMachine = new StateMachine();
	}
}
