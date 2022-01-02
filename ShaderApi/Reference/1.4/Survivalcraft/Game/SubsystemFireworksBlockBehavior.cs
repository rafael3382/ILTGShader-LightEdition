using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A7 RID: 423
	public class SubsystemFireworksBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000AA9 RID: 2729 RVA: 0x00046F89 File Offset: 0x00045189
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000AAA RID: 2730 RVA: 0x00046F91 File Offset: 0x00045191
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x00046F94 File Offset: 0x00045194
		public void ExplodeFireworks(Vector3 position, int data)
		{
			for (int i = 0; i < 3; i++)
			{
				Vector3 v = new Vector3(this.m_random.Float(-3f, 3f), -15f, this.m_random.Float(-3f, 3f));
				if (this.m_subsystemTerrain.Raycast(position, position + v, false, true, null) != null)
				{
					return;
				}
			}
			FireworksBlock.Shape shape = FireworksBlock.GetShape(data);
			float flickering = FireworksBlock.GetFlickering(data) ? 0.66f : 0f;
			float particleSize = (FireworksBlock.GetAltitude(data) > 0) ? 1.1f : 1f;
			Color color = FireworksBlock.FireworksColors[FireworksBlock.GetColor(data)];
			this.m_subsystemParticles.AddParticleSystem(new FireworksParticleSystem(position, color, shape, flickering, particleSize));
			this.m_subsystemAudio.PlayRandomSound("Audio/FireworksPop", 1f, this.m_random.Float(-0.4f, 0f), position, 80f, true);
			this.m_subsystemNoise.MakeNoise(position, 1f, 60f);
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x000470AC File Offset: 0x000452AC
		public override void OnFiredAsProjectile(Projectile projectile)
		{
			int data = Terrain.ExtractData(projectile.Value);
			float num = (FireworksBlock.GetAltitude(data) == 0) ? 0.8f : 1.3f;
			this.m_subsystemProjectiles.AddTrail(projectile, Vector3.Zero, new FireworksTrailParticleSystem());
			this.m_subsystemAudio.PlayRandomSound("Audio/FireworksWhoosh", 1f, this.m_random.Float(-0.2f, 0.2f), projectile.Position, 8f, true);
			this.m_subsystemNoise.MakeNoise(projectile.Position, 1f, 10f);
			this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + (double)num, delegate
			{
				if (!projectile.ToRemove)
				{
					projectile.ToRemove = true;
					this.ExplodeFireworks(projectile.Position, data);
				}
			});
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x00047198 File Offset: 0x00045398
		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			return true;
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x0004719C File Offset: 0x0004539C
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x00047240 File Offset: 0x00045440
		public void Update(float dt)
		{
			ComponentPlayer componentPlayer = (this.m_subsystemPlayers.ComponentPlayers.Count > 0) ? this.m_subsystemPlayers.ComponentPlayers[0] : null;
			if (componentPlayer == null)
			{
				return;
			}
			if (this.m_newYearCelebrationTimeRemaining <= 0f && Time.PeriodicEvent(5.0, 0.0) && this.m_subsystemSky.SkyLightIntensity == 0f && !componentPlayer.ComponentSleep.IsSleeping)
			{
				DateTime now = DateTime.Now;
				if (now.Year > SettingsManager.NewYearCelebrationLastYear && now.Month == 1 && now.Day == 1 && now.Hour == 0 && now.Minute < 59)
				{
					SettingsManager.NewYearCelebrationLastYear = now.Year;
					this.m_newYearCelebrationTimeRemaining = 180f;
					componentPlayer.ComponentGui.DisplayLargeMessage("Happy New Year!", "--- Enjoy the fireworks ---", 15f, 3f);
				}
			}
			if (this.m_newYearCelebrationTimeRemaining <= 0f)
			{
				return;
			}
			this.m_newYearCelebrationTimeRemaining -= dt;
			float num = (this.m_newYearCelebrationTimeRemaining > 10f) ? MathUtils.Lerp(1f, 7f, 0.5f * MathUtils.Sin(0.25f * this.m_newYearCelebrationTimeRemaining) + 0.5f) : 20f;
			if (this.m_random.Float(0f, 1f) < num * dt)
			{
				Vector2 vector = this.m_random.Vector2(35f, 50f);
				Vector3 vector2 = componentPlayer.ComponentBody.Position + new Vector3(vector.X, 0f, vector.Y);
				TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(new Vector3(vector2.X, 120f, vector2.Z), new Vector3(vector2.X, 40f, vector2.Z), false, true, null);
				if (terrainRaycastResult != null)
				{
					int data = 0;
					data = FireworksBlock.SetShape(data, (FireworksBlock.Shape)this.m_random.Int(0, 7));
					data = FireworksBlock.SetColor(data, this.m_random.Int(0, 7));
					data = FireworksBlock.SetAltitude(data, this.m_random.Int(0, 1));
					data = FireworksBlock.SetFlickering(data, this.m_random.Float(0f, 1f) < 0.25f);
					int value = Terrain.MakeBlockValue(215, 0, data);
					TerrainRaycastResult value2 = terrainRaycastResult.Value;
					float x = (float)value2.CellFace.Point.X;
					value2 = terrainRaycastResult.Value;
					float y = (float)(value2.CellFace.Point.Y + 1);
					value2 = terrainRaycastResult.Value;
					Vector3 position = new Vector3(x, y, (float)value2.CellFace.Point.Z);
					this.m_subsystemProjectiles.FireProjectile(value, position, new Vector3(this.m_random.Float(-3f, 3f), 45f, this.m_random.Float(-3f, 3f)), Vector3.Zero, null);
				}
			}
		}

		// Token: 0x04000537 RID: 1335
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000538 RID: 1336
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x04000539 RID: 1337
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x0400053A RID: 1338
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400053B RID: 1339
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x0400053C RID: 1340
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400053D RID: 1341
		public SubsystemSky m_subsystemSky;

		// Token: 0x0400053E RID: 1342
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x0400053F RID: 1343
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000540 RID: 1344
		public float m_newYearCelebrationTimeRemaining;
	}
}
