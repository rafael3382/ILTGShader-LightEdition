using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F2 RID: 498
	public class ComponentCetaceanBreatheBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000E03 RID: 3587 RVA: 0x00066AE8 File Offset: 0x00064CE8
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000E04 RID: 3588 RVA: 0x00066AEB File Offset: 0x00064CEB
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000E05 RID: 3589 RVA: 0x00066AF3 File Offset: 0x00064CF3
		public void Update(float dt)
		{
			if (!this.IsActive)
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
			this.m_stateMachine.Update();
		}

		// Token: 0x06000E06 RID: 3590 RVA: 0x00066B18 File Offset: 0x00064D18
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				this.m_importanceLevel = MathUtils.Lerp(0f, 400f, MathUtils.Saturate((0.75f - this.m_componentCreature.ComponentHealth.Air) / 0.75f));
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Surface");
				}
			}, null);
			this.m_stateMachine.AddState("Surface", delegate
			{
				this.m_componentPathfinding.Stop();
			}, delegate
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				if (this.m_componentPathfinding.Destination == null)
				{
					Vector3? destination = this.FindSurfaceDestination();
					if (destination != null)
					{
						float speed = (this.m_componentCreature.ComponentHealth.Air < 0.25f) ? 1f : this.m_random.Float(0.4f, 0.6f);
						this.m_componentPathfinding.SetDestination(destination, speed, 1f, 0, false, false, false, null);
					}
				}
				else if (this.m_componentPathfinding.IsStuck)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_componentCreature.ComponentHealth.Air > 0.9f)
				{
					this.m_stateMachine.TransitionTo("Breathe");
				}
			}, null);
			this.m_stateMachine.AddState("Breathe", delegate
			{
				Vector3 forward = this.m_componentCreature.ComponentBody.Matrix.Forward;
				Vector3 value = this.m_componentCreature.ComponentBody.Matrix.Translation + 10f * forward + new Vector3(0f, 2f, 0f);
				this.m_componentPathfinding.SetDestination(new Vector3?(value), 0.6f, 1f, 0, false, false, false, null);
				this.m_particleSystem = new WhalePlumeParticleSystem(this.m_subsystemTerrain, this.m_random.Float(0.8f, 1.1f), this.m_random.Float(1f, 1.3f));
				this.m_subsystemParticles.AddParticleSystem(this.m_particleSystem);
				this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/WhaleBlow", 1f, this.m_random.Float(-0.2f, 0.2f), this.m_componentCreature.ComponentBody.Position, 10f, true);
			}, delegate
			{
				this.m_particleSystem.Position = this.m_componentCreature.ComponentBody.Position + new Vector3(0f, 0.8f * this.m_componentCreature.ComponentBody.BoxSize.Y, 0f);
				if (!this.m_subsystemParticles.ContainsParticleSystem(this.m_particleSystem))
				{
					this.m_importanceLevel = 0f;
				}
			}, delegate
			{
				this.m_particleSystem.IsStopped = true;
				this.m_particleSystem = null;
			});
		}

		// Token: 0x06000E07 RID: 3591 RVA: 0x00066BFC File Offset: 0x00064DFC
		public virtual Vector3? FindSurfaceDestination()
		{
			Vector3 vector = 0.5f * (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max);
			Vector3 forward = this.m_componentCreature.ComponentBody.Matrix.Forward;
			float s = 2f * this.m_componentCreature.ComponentBody.ImmersionDepth;
			for (int i = 0; i < 16; i++)
			{
				Vector2 vector2 = (i < 4) ? (new Vector2(forward.X, forward.Z) + this.m_random.Vector2(0f, 0.25f)) : this.m_random.Vector2(0.5f, 1f);
				Vector3 v = Vector3.Normalize(new Vector3(vector2.X, 1f, vector2.Y));
				Vector3 end = vector + s * v;
				TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(vector, end, false, false, (int value, float d) => Terrain.ExtractContents(value) != 18);
				if (terrainRaycastResult != null && Terrain.ExtractContents(terrainRaycastResult.Value.Value) == 0)
				{
					return new Vector3?(new Vector3((float)terrainRaycastResult.Value.CellFace.X + 0.5f, (float)terrainRaycastResult.Value.CellFace.Y, (float)terrainRaycastResult.Value.CellFace.Z + 0.5f));
				}
			}
			return null;
		}

		// Token: 0x0400076A RID: 1898
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x0400076B RID: 1899
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400076C RID: 1900
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400076D RID: 1901
		public ComponentCreature m_componentCreature;

		// Token: 0x0400076E RID: 1902
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x0400076F RID: 1903
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000770 RID: 1904
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000771 RID: 1905
		public WhalePlumeParticleSystem m_particleSystem;

		// Token: 0x04000772 RID: 1906
		public float m_importanceLevel;
	}
}
