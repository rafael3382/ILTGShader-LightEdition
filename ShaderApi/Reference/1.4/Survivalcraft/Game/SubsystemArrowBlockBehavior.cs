using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000183 RID: 387
	public class SubsystemArrowBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060008CD RID: 2253 RVA: 0x0003803F File Offset: 0x0003623F
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x00038048 File Offset: 0x00036248
		public override void OnFiredAsProjectile(Projectile projectile)
		{
			if (ArrowBlock.GetArrowType(Terrain.ExtractData(projectile.Value)) == ArrowBlock.ArrowType.FireArrow)
			{
				this.m_subsystemProjectiles.AddTrail(projectile, Vector3.Zero, new SmokeTrailParticleSystem(20, 0.5f, float.MaxValue, Color.White));
				projectile.ProjectileStoppedAction = ProjectileStoppedAction.Disappear;
				projectile.IsIncendiary = true;
			}
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x000380A0 File Offset: 0x000362A0
		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			ArrowBlock.ArrowType arrowType = ArrowBlock.GetArrowType(Terrain.ExtractData(worldItem.Value));
			if (worldItem.Velocity.Length() > 10f)
			{
				float num = 0.1f;
				if (arrowType == ArrowBlock.ArrowType.FireArrow)
				{
					num = 0.5f;
				}
				if (arrowType == ArrowBlock.ArrowType.WoodenArrow)
				{
					num = 0.2f;
				}
				if (arrowType == ArrowBlock.ArrowType.DiamondArrow)
				{
					num = 0f;
				}
				if (arrowType == ArrowBlock.ArrowType.IronBolt)
				{
					num = 0.05f;
				}
				if (arrowType == ArrowBlock.ArrowType.DiamondBolt)
				{
					num = 0f;
				}
				if (this.m_random.Float(0f, 1f) < num)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x00038122 File Offset: 0x00036322
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
		}

		// Token: 0x04000477 RID: 1143
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x04000478 RID: 1144
		public Game.Random m_random = new Game.Random();
	}
}
