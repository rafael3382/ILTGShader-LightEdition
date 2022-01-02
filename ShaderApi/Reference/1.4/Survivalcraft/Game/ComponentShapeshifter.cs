using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200022C RID: 556
	public class ComponentShapeshifter : Component, IUpdateable
	{
		// Token: 0x17000295 RID: 661
		// (get) Token: 0x060011E0 RID: 4576 RVA: 0x00084D75 File Offset: 0x00082F75
		// (set) Token: 0x060011E1 RID: 4577 RVA: 0x00084D7D File Offset: 0x00082F7D
		public bool IsEnabled { get; set; }

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x060011E2 RID: 4578 RVA: 0x00084D86 File Offset: 0x00082F86
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x00084D8C File Offset: 0x00082F8C
		public void Update(float dt)
		{
			bool areSupernaturalCreaturesEnabled = this.m_subsystemGameInfo.WorldSettings.AreSupernaturalCreaturesEnabled;
			if (this.IsEnabled && !this.m_componentSpawn.IsDespawning && this.m_componentHealth.Health > 0f)
			{
				if (!areSupernaturalCreaturesEnabled && !string.IsNullOrEmpty(this.m_dayEntityTemplateName))
				{
					this.ShapeshiftTo(this.m_dayEntityTemplateName);
				}
				else if (this.m_subsystemSky.SkyLightIntensity > 0.25f && !string.IsNullOrEmpty(this.m_dayEntityTemplateName))
				{
					this.m_timeToSwitch -= 2f * dt;
					if (this.m_timeToSwitch <= 0f)
					{
						this.ShapeshiftTo(this.m_dayEntityTemplateName);
					}
				}
				else if (areSupernaturalCreaturesEnabled && this.m_subsystemSky.SkyLightIntensity < 0.1f && (this.m_subsystemSky.MoonPhase == 0 || this.m_subsystemSky.MoonPhase == 4) && !string.IsNullOrEmpty(this.m_nightEntityTemplateName))
				{
					this.m_timeToSwitch -= dt;
					if (this.m_timeToSwitch <= 0f)
					{
						this.ShapeshiftTo(this.m_nightEntityTemplateName);
					}
				}
			}
			if (!string.IsNullOrEmpty(this.m_spawnEntityTemplateName))
			{
				if (this.m_particleSystem == null)
				{
					this.m_particleSystem = new ShapeshiftParticleSystem();
					this.m_subsystemParticles.AddParticleSystem(this.m_particleSystem);
				}
				this.m_particleSystem.BoundingBox = this.m_componentBody.BoundingBox;
			}
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x00084EF4 File Offset: 0x000830F4
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentSpawn = base.Entity.FindComponent<ComponentSpawn>(true);
			this.m_componentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.m_componentHealth = base.Entity.FindComponent<ComponentHealth>(true);
			this.m_dayEntityTemplateName = valuesDictionary.GetValue<string>("DayEntityTemplateName");
			this.m_nightEntityTemplateName = valuesDictionary.GetValue<string>("NightEntityTemplateName");
			float value = valuesDictionary.GetValue<float>("Probability");
			if (!string.IsNullOrEmpty(this.m_dayEntityTemplateName))
			{
				DatabaseManager.FindEntityValuesDictionary(this.m_dayEntityTemplateName, true);
			}
			if (!string.IsNullOrEmpty(this.m_nightEntityTemplateName))
			{
				DatabaseManager.FindEntityValuesDictionary(this.m_nightEntityTemplateName, true);
			}
			this.m_timeToSwitch = ComponentShapeshifter.s_random.Float(3f, 15f);
			this.IsEnabled = (ComponentShapeshifter.s_random.Float(0f, 1f) < value);
			ComponentSpawn componentSpawn = this.m_componentSpawn;
			componentSpawn.Despawned = (Action<ComponentSpawn>)Delegate.Combine(componentSpawn.Despawned, new Action<ComponentSpawn>(this.ComponentSpawn_Despawned));
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x00085040 File Offset: 0x00083240
		public virtual void ShapeshiftTo(string entityTemplateName)
		{
			if (string.IsNullOrEmpty(this.m_spawnEntityTemplateName))
			{
				this.m_spawnEntityTemplateName = entityTemplateName;
				this.m_componentSpawn.DespawnDuration = 3f;
				this.m_componentSpawn.Despawn();
				this.m_subsystemAudio.PlaySound("Audio/Shapeshift", 1f, 0f, this.m_componentBody.Position, 3f, true);
			}
		}

		// Token: 0x060011E6 RID: 4582 RVA: 0x000850A8 File Offset: 0x000832A8
		public virtual void ComponentSpawn_Despawned(ComponentSpawn componentSpawn)
		{
			if (this.m_componentHealth.Health > 0f && !string.IsNullOrEmpty(this.m_spawnEntityTemplateName))
			{
				Entity entity = DatabaseManager.CreateEntity(base.Project, this.m_spawnEntityTemplateName, true);
				ComponentBody componentBody = entity.FindComponent<ComponentBody>(true);
				componentBody.Position = this.m_componentBody.Position;
				componentBody.Rotation = this.m_componentBody.Rotation;
				componentBody.Velocity = this.m_componentBody.Velocity;
				entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0.5f;
				ModsManager.HookAction("OnDespawned", delegate(ModLoader modLoader)
				{
					modLoader.OnDespawned(entity, componentSpawn);
					return false;
				});
				base.Project.AddEntity(entity);
			}
			if (this.m_particleSystem != null)
			{
				this.m_particleSystem.Stopped = true;
			}
		}

		// Token: 0x04000AD2 RID: 2770
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000AD3 RID: 2771
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000AD4 RID: 2772
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000AD5 RID: 2773
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000AD6 RID: 2774
		public ComponentSpawn m_componentSpawn;

		// Token: 0x04000AD7 RID: 2775
		public ComponentBody m_componentBody;

		// Token: 0x04000AD8 RID: 2776
		public ComponentHealth m_componentHealth;

		// Token: 0x04000AD9 RID: 2777
		public ShapeshiftParticleSystem m_particleSystem;

		// Token: 0x04000ADA RID: 2778
		public string m_nightEntityTemplateName;

		// Token: 0x04000ADB RID: 2779
		public string m_dayEntityTemplateName;

		// Token: 0x04000ADC RID: 2780
		public float m_timeToSwitch;

		// Token: 0x04000ADD RID: 2781
		public string m_spawnEntityTemplateName;

		// Token: 0x04000ADE RID: 2782
		public static Random s_random = new Random();
	}
}
