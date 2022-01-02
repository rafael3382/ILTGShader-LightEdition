using System;
using System.Collections.Generic;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C1 RID: 449
	public class SubsystemParticles : Subsystem, IDrawable, IUpdateable
	{
		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000BB2 RID: 2994 RVA: 0x000500B0 File Offset: 0x0004E2B0
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000BB3 RID: 2995 RVA: 0x000500B3 File Offset: 0x0004E2B3
		public int[] DrawOrders
		{
			get
			{
				return this.m_drawOrders;
			}
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x000500BB File Offset: 0x0004E2BB
		public void AddParticleSystem(ParticleSystemBase particleSystem)
		{
			if (particleSystem.SubsystemParticles == null)
			{
				this.m_particleSystems.Add(particleSystem, true);
				particleSystem.SubsystemParticles = this;
				particleSystem.OnAdded();
				return;
			}
			throw new InvalidOperationException("Particle system is already added.");
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x000500EA File Offset: 0x0004E2EA
		public void RemoveParticleSystem(ParticleSystemBase particleSystem)
		{
			if (particleSystem.SubsystemParticles == this)
			{
				particleSystem.OnRemoved();
				this.m_particleSystems.Remove(particleSystem);
				particleSystem.SubsystemParticles = null;
				return;
			}
			throw new InvalidOperationException("Particle system is not added.");
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x0005011A File Offset: 0x0004E31A
		public bool ContainsParticleSystem(ParticleSystemBase particleSystem)
		{
			return particleSystem.SubsystemParticles == this;
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x00050125 File Offset: 0x0004E325
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
		}

		// Token: 0x06000BB8 RID: 3000 RVA: 0x0005013C File Offset: 0x0004E33C
		public void Update(float dt)
		{
			if (this.ParticleSystemsSimulate)
			{
				this.m_endedParticleSystems.Clear();
				foreach (ParticleSystemBase particleSystemBase in this.m_particleSystems.Keys)
				{
					if (particleSystemBase.Simulate(this.m_subsystemTime.GameTimeDelta))
					{
						this.m_endedParticleSystems.Add(particleSystemBase);
					}
				}
				foreach (ParticleSystemBase particleSystem in this.m_endedParticleSystems)
				{
					this.RemoveParticleSystem(particleSystem);
				}
			}
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x00050204 File Offset: 0x0004E404
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.ParticleSystemsDraw)
			{
				foreach (ParticleSystemBase particleSystemBase in this.m_particleSystems.Keys)
				{
					particleSystemBase.Draw(camera);
				}
				this.PrimitivesRenderer.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
			}
		}

		// Token: 0x040005BD RID: 1469
		public SubsystemTime m_subsystemTime;

		// Token: 0x040005BE RID: 1470
		public Dictionary<ParticleSystemBase, bool> m_particleSystems = new Dictionary<ParticleSystemBase, bool>();

		// Token: 0x040005BF RID: 1471
		public PrimitivesRenderer3D PrimitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x040005C0 RID: 1472
		public bool ParticleSystemsDraw = true;

		// Token: 0x040005C1 RID: 1473
		public bool ParticleSystemsSimulate = true;

		// Token: 0x040005C2 RID: 1474
		public int[] m_drawOrders = new int[]
		{
			300
		};

		// Token: 0x040005C3 RID: 1475
		public List<ParticleSystemBase> m_endedParticleSystems = new List<ParticleSystemBase>();
	}
}
