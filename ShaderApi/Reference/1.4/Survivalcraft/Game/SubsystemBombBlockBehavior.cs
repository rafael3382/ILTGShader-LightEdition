using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200018D RID: 397
	public class SubsystemBombBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600092E RID: 2350 RVA: 0x000397B2 File Offset: 0x000379B2
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600092F RID: 2351 RVA: 0x000397BA File Offset: 0x000379BA
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x000397C0 File Offset: 0x000379C0
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			SubsystemProjectiles subsystemProjectiles = this.m_subsystemProjectiles;
			subsystemProjectiles.ProjectileAdded = (Action<Projectile>)Delegate.Combine(subsystemProjectiles.ProjectileAdded, new Action<Projectile>(delegate(Projectile projectile)
			{
				this.ScanProjectile(projectile);
			}));
			SubsystemProjectiles subsystemProjectiles2 = this.m_subsystemProjectiles;
			subsystemProjectiles2.ProjectileRemoved = (Action<Projectile>)Delegate.Combine(subsystemProjectiles2.ProjectileRemoved, new Action<Projectile>(delegate(Projectile projectile)
			{
				this.m_projectiles.Remove(projectile);
			}));
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x0003987C File Offset: 0x00037A7C
		public void ScanProjectile(Projectile projectile)
		{
			if (!this.m_projectiles.ContainsKey(projectile))
			{
				int num = Terrain.ExtractContents(projectile.Value);
				if (this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(projectile.Value)).Contains(this))
				{
					this.m_projectiles.Add(projectile, true);
					projectile.ProjectileStoppedAction = ProjectileStoppedAction.DoNothing;
					Color color = (num == 228) ? new Color(255, 140, 192) : Color.White;
					this.m_subsystemProjectiles.AddTrail(projectile, new Vector3(0f, 0.25f, 0.1f), new SmokeTrailParticleSystem(20, 0.33f, float.MaxValue, color));
				}
			}
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x00039930 File Offset: 0x00037B30
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(0.1, 0.0))
			{
				foreach (Projectile projectile in this.m_projectiles.Keys)
				{
					if (this.m_subsystemGameInfo.TotalElapsedGameTime - projectile.CreationTime > 5.0)
					{
						this.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(projectile.Position.X), Terrain.ToCell(projectile.Position.Y), Terrain.ToCell(projectile.Position.Z), projectile.Value);
						projectile.ToRemove = true;
					}
				}
			}
		}

		// Token: 0x04000499 RID: 1177
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x0400049A RID: 1178
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400049B RID: 1179
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x0400049C RID: 1180
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x0400049D RID: 1181
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x0400049E RID: 1182
		public Dictionary<Projectile, bool> m_projectiles = new Dictionary<Projectile, bool>();
	}
}
