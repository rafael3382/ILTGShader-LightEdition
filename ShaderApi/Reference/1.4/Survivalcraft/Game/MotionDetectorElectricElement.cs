using System;
using Engine;

namespace Game
{
	// Token: 0x020002C9 RID: 713
	public class MotionDetectorElectricElement : MountedElectricElement
	{
		// Token: 0x060015B4 RID: 5556 RVA: 0x000A361C File Offset: 0x000A181C
		public MotionDetectorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemBodies = subsystemElectricity.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_center = new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z) + new Vector3(0.5f) - 0.25f * this.m_direction;
			this.m_direction = CellFace.FaceToVector3(cellFace.Face);
			Vector3 vector = Vector3.One - new Vector3(MathUtils.Abs(this.m_direction.X), MathUtils.Abs(this.m_direction.Y), MathUtils.Abs(this.m_direction.Z));
			Vector3 vector2 = this.m_center - 8f * vector;
			Vector3 vector3 = this.m_center + 8f * (vector + this.m_direction);
			this.m_corner1 = new Vector2(vector2.X, vector2.Z);
			this.m_corner2 = new Vector2(vector3.X, vector3.Z);
		}

		// Token: 0x060015B5 RID: 5557 RVA: 0x000A374A File Offset: 0x000A194A
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060015B6 RID: 5558 RVA: 0x000A3754 File Offset: 0x000A1954
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			this.m_voltage = this.CalculateMotionVoltage();
			if (this.m_voltage > 0f && voltage == 0f)
			{
				base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/MotionDetectorClick", 1f, 0f, this.m_center, 1f, true);
			}
			float num = 0.5f * (0.9f + 0.000200000009f * (float)(this.GetHashCode() % 1000));
			base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + MathUtils.Max((int)(num / 0.01f), 1));
			return this.m_voltage != voltage;
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x000A3808 File Offset: 0x000A1A08
		public float CalculateMotionVoltage()
		{
			float num = 0f;
			this.m_bodies.Clear();
			this.m_subsystemBodies.FindBodiesInArea(this.m_corner1, this.m_corner2, this.m_bodies);
			for (int i = 0; i < this.m_bodies.Count; i++)
			{
				ComponentBody componentBody = this.m_bodies.Array[i];
				if (componentBody.Velocity.LengthSquared() > 0.0625f)
				{
					Vector3 vector = componentBody.Position + new Vector3(0f, 0.5f * componentBody.BoxSize.Y, 0f);
					float num2 = Vector3.DistanceSquared(vector, this.m_center);
					if (num2 < 64f && Vector3.Dot(Vector3.Normalize(vector - (this.m_center - 0.75f * this.m_direction)), this.m_direction) > 0.5f)
					{
						if (base.SubsystemElectricity.SubsystemTerrain.Raycast(this.m_center, vector, false, true, delegate(int value, float d)
						{
							Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
							return block.IsCollidable_(value) && block.BlockIndex != 15 && block.BlockIndex != 60 && block.BlockIndex != 44 && block.BlockIndex != 18;
						}) == null)
						{
							num = MathUtils.Max(num, MathUtils.Saturate(1f - MathUtils.Sqrt(num2) / 8f));
						}
					}
				}
			}
			if (num <= 0f)
			{
				return 0f;
			}
			return MathUtils.Lerp(0.51f, 1f, MathUtils.Saturate(num * 1.1f));
		}

		// Token: 0x04000E3D RID: 3645
		public const float m_range = 8f;

		// Token: 0x04000E3E RID: 3646
		public const float m_speedThreshold = 0.25f;

		// Token: 0x04000E3F RID: 3647
		public const float m_pollingPeriod = 0.5f;

		// Token: 0x04000E40 RID: 3648
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000E41 RID: 3649
		public float m_voltage;

		// Token: 0x04000E42 RID: 3650
		public Vector3 m_center;

		// Token: 0x04000E43 RID: 3651
		public Vector3 m_direction;

		// Token: 0x04000E44 RID: 3652
		public Vector2 m_corner1;

		// Token: 0x04000E45 RID: 3653
		public Vector2 m_corner2;

		// Token: 0x04000E46 RID: 3654
		public DynamicArray<ComponentBody> m_bodies = new DynamicArray<ComponentBody>();
	}
}
