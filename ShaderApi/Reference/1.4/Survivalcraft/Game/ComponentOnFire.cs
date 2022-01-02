using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000222 RID: 546
	public class ComponentOnFire : Component, IUpdateable
	{
		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06001113 RID: 4371 RVA: 0x0007F448 File Offset: 0x0007D648
		// (set) Token: 0x06001114 RID: 4372 RVA: 0x0007F450 File Offset: 0x0007D650
		public ComponentBody ComponentBody { get; set; }

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06001115 RID: 4373 RVA: 0x0007F459 File Offset: 0x0007D659
		public bool IsOnFire
		{
			get
			{
				return this.m_fireDuration > 0f;
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06001116 RID: 4374 RVA: 0x0007F468 File Offset: 0x0007D668
		// (set) Token: 0x06001117 RID: 4375 RVA: 0x0007F470 File Offset: 0x0007D670
		public bool TouchesFire { get; set; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06001118 RID: 4376 RVA: 0x0007F479 File Offset: 0x0007D679
		// (set) Token: 0x06001119 RID: 4377 RVA: 0x0007F481 File Offset: 0x0007D681
		public ComponentCreature Attacker { get; set; }

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x0600111A RID: 4378 RVA: 0x0007F48A File Offset: 0x0007D68A
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x0007F48D File Offset: 0x0007D68D
		public virtual void SetOnFire(ComponentCreature attacker, float duration)
		{
			if (!this.IsOnFire)
			{
				this.Attacker = attacker;
			}
			this.m_fireDuration = MathUtils.Max(this.m_fireDuration, duration);
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x0007F4B0 File Offset: 0x0007D6B0
		public virtual void Extinguish()
		{
			this.Attacker = null;
			this.m_fireDuration = 0f;
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x0007F4C4 File Offset: 0x0007D6C4
		public void Update(float dt)
		{
			if (!base.IsAddedToProject)
			{
				return;
			}
			if (this.IsOnFire)
			{
				this.m_fireDuration = MathUtils.Max(this.m_fireDuration - dt, 0f);
				if (this.m_onFireParticleSystem == null)
				{
					this.m_onFireParticleSystem = new OnFireParticleSystem();
					this.m_subsystemParticles.AddParticleSystem(this.m_onFireParticleSystem);
				}
				BoundingBox boundingBox = this.ComponentBody.BoundingBox;
				this.m_onFireParticleSystem.Position = 0.5f * (boundingBox.Min + boundingBox.Max);
				this.m_onFireParticleSystem.Radius = 0.5f * MathUtils.Min(boundingBox.Max.X - boundingBox.Min.X, boundingBox.Max.Z - boundingBox.Min.Z);
				if (this.ComponentBody.ImmersionFactor > 0.5f && this.ComponentBody.ImmersionFluidBlock is WaterBlock)
				{
					this.Extinguish();
					this.m_subsystemAudio.PlaySound("Audio/SizzleLong", 1f, 0f, this.m_onFireParticleSystem.Position, 4f, true);
				}
				if (Time.PeriodicEvent(0.5, 0.0))
				{
					float distance = this.m_subsystemAudio.CalculateListenerDistance(this.ComponentBody.Position);
					this.m_soundVolume = this.m_subsystemAudio.CalculateVolume(distance, 2f, 5f);
				}
				this.m_subsystemAmbientSounds.FireSoundVolume = MathUtils.Max(this.m_subsystemAmbientSounds.FireSoundVolume, this.m_soundVolume);
			}
			else
			{
				if (this.m_onFireParticleSystem != null)
				{
					this.m_onFireParticleSystem.IsStopped = true;
					this.m_onFireParticleSystem = null;
				}
				this.m_soundVolume = 0f;
			}
			if (this.m_subsystemTime.GameTime <= this.m_nextCheckTime)
			{
				return;
			}
			this.m_nextCheckTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(0.9f, 1.1f);
			this.TouchesFire = this.CheckIfBodyTouchesFire();
			if (this.TouchesFire)
			{
				this.m_fireTouchCount++;
				if (this.m_fireTouchCount >= 5)
				{
					this.SetOnFire(null, this.m_random.Float(12f, 15f));
				}
			}
			else
			{
				this.m_fireTouchCount = 0;
			}
			if (this.ComponentBody.ImmersionFactor > 0.2f && this.ComponentBody.ImmersionFluidBlock is MagmaBlock)
			{
				this.SetOnFire(null, this.m_random.Float(12f, 15f));
			}
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x0007F750 File Offset: 0x0007D950
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemAmbientSounds = base.Project.FindSubsystem<SubsystemAmbientSounds>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.ComponentBody = base.Entity.FindComponent<ComponentBody>();
			float value = valuesDictionary.GetValue<float>("FireDuration");
			if (value > 0f)
			{
				this.SetOnFire(null, value);
			}
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x0007F7E4 File Offset: 0x0007D9E4
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("FireDuration", this.m_fireDuration);
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x0007F7F7 File Offset: 0x0007D9F7
		public override void OnEntityRemoved()
		{
			if (this.m_onFireParticleSystem != null)
			{
				this.m_onFireParticleSystem.IsStopped = true;
			}
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x0007F810 File Offset: 0x0007DA10
		public virtual bool CheckIfBodyTouchesFire()
		{
			BoundingBox boundingBox = this.ComponentBody.BoundingBox;
			boundingBox.Min -= new Vector3(0.25f);
			boundingBox.Max += new Vector3(0.25f);
			int num = Terrain.ToCell(boundingBox.Min.X);
			int num2 = Terrain.ToCell(boundingBox.Min.Y);
			int num3 = Terrain.ToCell(boundingBox.Min.Z);
			int num4 = Terrain.ToCell(boundingBox.Max.X);
			int num5 = Terrain.ToCell(boundingBox.Max.Y);
			int num6 = Terrain.ToCell(boundingBox.Max.Z);
			for (int i = num; i <= num4; i++)
			{
				for (int j = num2; j <= num5; j++)
				{
					for (int k = num3; k <= num6; k++)
					{
						int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(i, j, k);
						int num7 = Terrain.ExtractContents(cellValue);
						int num8 = Terrain.ExtractData(cellValue);
						if (num7 != 104)
						{
							if (num7 == 209)
							{
								if (num8 > 0)
								{
									BoundingBox box = new BoundingBox(new Vector3((float)i, (float)j, (float)k) + new Vector3(0.2f), new Vector3((float)(i + 1), (float)(j + 1), (float)(k + 1)) - new Vector3(0.2f));
									if (boundingBox.Intersection(box))
									{
										return true;
									}
								}
							}
						}
						else if (num8 == 0)
						{
							BoundingBox box2 = new BoundingBox(new Vector3((float)i, (float)j, (float)k), new Vector3((float)(i + 1), (float)(j + 1), (float)(k + 1)));
							if (boundingBox.Intersection(box2))
							{
								return true;
							}
						}
						else
						{
							if ((num8 & 1) != 0)
							{
								BoundingBox box3 = new BoundingBox(new Vector3((float)i, (float)j, (float)k + 0.5f), new Vector3((float)(i + 1), (float)(j + 1), (float)(k + 1)));
								if (boundingBox.Intersection(box3))
								{
									return true;
								}
							}
							if ((num8 & 2) != 0)
							{
								BoundingBox box4 = new BoundingBox(new Vector3((float)i + 0.5f, (float)j, (float)k), new Vector3((float)(i + 1), (float)(j + 1), (float)(k + 1)));
								if (boundingBox.Intersection(box4))
								{
									return true;
								}
							}
							if ((num8 & 4) != 0)
							{
								BoundingBox box5 = new BoundingBox(new Vector3((float)i, (float)j, (float)k), new Vector3((float)(i + 1), (float)(j + 1), (float)k + 0.5f));
								if (boundingBox.Intersection(box5))
								{
									return true;
								}
							}
							if ((num8 & 8) != 0)
							{
								BoundingBox box6 = new BoundingBox(new Vector3((float)i, (float)j, (float)k), new Vector3((float)i + 0.5f, (float)(j + 1), (float)(k + 1)));
								if (boundingBox.Intersection(box6))
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x04000A2F RID: 2607
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A30 RID: 2608
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000A31 RID: 2609
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000A32 RID: 2610
		public SubsystemAmbientSounds m_subsystemAmbientSounds;

		// Token: 0x04000A33 RID: 2611
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000A34 RID: 2612
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000A35 RID: 2613
		public double m_nextCheckTime;

		// Token: 0x04000A36 RID: 2614
		public int m_fireTouchCount;

		// Token: 0x04000A37 RID: 2615
		public OnFireParticleSystem m_onFireParticleSystem;

		// Token: 0x04000A38 RID: 2616
		public float m_soundVolume;

		// Token: 0x04000A39 RID: 2617
		public float m_fireDuration;
	}
}
