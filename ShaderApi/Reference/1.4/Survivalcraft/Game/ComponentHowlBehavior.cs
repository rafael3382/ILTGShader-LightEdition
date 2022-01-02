using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000211 RID: 529
	public class ComponentHowlBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x00075F0F File Offset: 0x0007410F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x00075F12 File Offset: 0x00074112
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x00075F1A File Offset: 0x0007411A
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x06000FF3 RID: 4083 RVA: 0x00075F28 File Offset: 0x00074128
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_howlSoundName = valuesDictionary.GetValue<string>("HowlSoundName");
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
			}, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Howl");
				}
				if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
				{
					if (this.m_random.Float(0f, 1f) < 0.015f * this.m_subsystemTime.GameTimeDelta)
					{
						this.m_importanceLevel = this.m_random.Float(1f, 3f);
						return;
					}
				}
				else
				{
					this.m_importanceLevel = 0f;
				}
			}, null);
			this.m_stateMachine.AddState("Howl", delegate
			{
				this.m_howlTime = 0f;
				this.m_howlDuration = this.m_random.Float(5f, 6f);
				this.m_componentPathfinding.Stop();
				this.m_importanceLevel = 10f;
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				this.m_componentCreature.ComponentLocomotion.LookOrder = new Vector2(this.m_componentCreature.ComponentLocomotion.LookOrder.X, 2f);
				float num = this.m_howlTime + this.m_subsystemTime.GameTimeDelta;
				if (this.m_howlTime <= 0.5f && num > 0.5f)
				{
					this.m_subsystemAudio.PlayRandomSound(this.m_howlSoundName, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, 10f, true);
				}
				this.m_howlTime = num;
				if (this.m_howlTime >= this.m_howlDuration)
				{
					this.m_importanceLevel = 0f;
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x0400094C RID: 2380
		public SubsystemSky m_subsystemSky;

		// Token: 0x0400094D RID: 2381
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400094E RID: 2382
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400094F RID: 2383
		public ComponentCreature m_componentCreature;

		// Token: 0x04000950 RID: 2384
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000951 RID: 2385
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000952 RID: 2386
		public float m_importanceLevel;

		// Token: 0x04000953 RID: 2387
		public string m_howlSoundName;

		// Token: 0x04000954 RID: 2388
		public float m_howlTime;

		// Token: 0x04000955 RID: 2389
		public float m_howlDuration;

		// Token: 0x04000956 RID: 2390
		public Game.Random m_random = new Game.Random();
	}
}
