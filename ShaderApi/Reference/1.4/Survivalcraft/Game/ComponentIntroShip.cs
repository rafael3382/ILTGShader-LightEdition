using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000215 RID: 533
	public class ComponentIntroShip : Component, IUpdateable
	{
		// Token: 0x1700020B RID: 523
		// (get) Token: 0x0600101E RID: 4126 RVA: 0x00079418 File Offset: 0x00077618
		// (set) Token: 0x0600101F RID: 4127 RVA: 0x00079420 File Offset: 0x00077620
		public float Heading { get; set; }

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06001020 RID: 4128 RVA: 0x00079429 File Offset: 0x00077629
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x0007942C File Offset: 0x0007762C
		public void Update(float dt)
		{
			float s = 3.5f * MathUtils.Saturate(0.07f * ((float)this.m_subsystemGameInfo.TotalElapsedGameTime - 6f));
			Matrix matrix = this.m_componentFrame.Matrix;
			Vector3 vector = Quaternion.CreateFromRotationMatrix(matrix).ToYawPitchRoll();
			vector.X = this.Heading;
			vector.Y = 0.05f * MathUtils.Sin((float)MathUtils.NormalizeAngle(0.77 * this.m_subsystemTime.GameTime + 1.0));
			vector.Z = 0.12f * MathUtils.Sin((float)MathUtils.NormalizeAngle(1.12 * this.m_subsystemTime.GameTime + 2.0));
			matrix = Matrix.CreateFromYawPitchRoll(vector.X, vector.Y, vector.Z) * Matrix.CreateTranslation(matrix.Translation);
			matrix.Translation += s * matrix.Forward * new Vector3(1f, 0f, 1f) * dt;
			this.m_componentFrame.Position = matrix.Translation;
			this.m_componentFrame.Rotation = Quaternion.CreateFromRotationMatrix(matrix);
			this.m_componentModel.SetBoneTransform(this.m_componentModel.Model.RootBone.Index, new Matrix?(matrix));
			if (this.m_subsystemTime.GameTime - this.m_creationTime > 10.0 && this.m_subsystemViews.CalculateDistanceFromNearestView(matrix.Translation) > this.m_subsystemSky.VisibilityRange + 30f)
			{
				base.Project.RemoveEntity(base.Entity, true);
			}
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x000795F8 File Offset: 0x000777F8
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentFrame = base.Entity.FindComponent<ComponentFrame>(true);
			this.m_componentModel = base.Entity.FindComponent<ComponentModel>(true);
			this.m_creationTime = this.m_subsystemTime.GameTime;
			this.Heading = valuesDictionary.GetValue<float>("Heading");
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x00079693 File Offset: 0x00077893
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("Heading", this.Heading);
		}

		// Token: 0x0400098F RID: 2447
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000990 RID: 2448
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x04000991 RID: 2449
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000992 RID: 2450
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000993 RID: 2451
		public ComponentFrame m_componentFrame;

		// Token: 0x04000994 RID: 2452
		public ComponentModel m_componentModel;

		// Token: 0x04000995 RID: 2453
		public double m_creationTime;
	}
}
