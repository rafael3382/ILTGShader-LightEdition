using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020D RID: 525
	public class ComponentGlowingEyes : Component, IDrawable
	{
		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000F82 RID: 3970 RVA: 0x00072C14 File Offset: 0x00070E14
		// (set) Token: 0x06000F83 RID: 3971 RVA: 0x00072C1C File Offset: 0x00070E1C
		public Vector3 GlowingEyesOffset { get; set; }

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x00072C25 File Offset: 0x00070E25
		// (set) Token: 0x06000F85 RID: 3973 RVA: 0x00072C2D File Offset: 0x00070E2D
		public Color GlowingEyesColor { get; set; }

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x00072C36 File Offset: 0x00070E36
		public int[] DrawOrders
		{
			get
			{
				return ComponentGlowingEyes.m_drawOrders;
			}
		}

		// Token: 0x06000F87 RID: 3975 RVA: 0x00072C40 File Offset: 0x00070E40
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGlow = base.Project.FindSubsystem<SubsystemGlow>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreatureModel = base.Entity.FindComponent<ComponentCreatureModel>(true);
			this.GlowingEyesOffset = valuesDictionary.GetValue<Vector3>("GlowingEyesOffset");
			this.GlowingEyesColor = valuesDictionary.GetValue<Color>("GlowingEyesColor");
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x00072CA8 File Offset: 0x00070EA8
		public override void OnEntityAdded()
		{
			for (int i = 0; i < this.m_eyeGlowPoints.Length; i++)
			{
				this.m_eyeGlowPoints[i] = this.m_subsystemGlow.AddGlowPoint();
			}
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x00072CDC File Offset: 0x00070EDC
		public override void OnEntityRemoved()
		{
			for (int i = 0; i < this.m_eyeGlowPoints.Length; i++)
			{
				this.m_subsystemGlow.RemoveGlowPoint(this.m_eyeGlowPoints[i]);
			}
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x00072D10 File Offset: 0x00070F10
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.m_eyeGlowPoints[0] == null || !this.m_componentCreatureModel.IsVisibleForCamera)
			{
				return;
			}
			this.m_eyeGlowPoints[0].Color = Color.Transparent;
			this.m_eyeGlowPoints[1].Color = Color.Transparent;
			ModelBone modelBone = this.m_componentCreatureModel.Model.FindBone("Head", false);
			if (modelBone == null)
			{
				return;
			}
			Matrix m = this.m_componentCreatureModel.AbsoluteBoneTransformsForCamera[modelBone.Index];
			m *= camera.InvertedViewMatrix;
			Vector3 vector = Vector3.Normalize(m.Up);
			float num = Vector3.Dot(m.Translation - camera.ViewPosition, camera.ViewDirection);
			if (num > 0f)
			{
				Vector3 translation = m.Translation;
				int cellLight = this.m_subsystemTerrain.Terrain.GetCellLight(Terrain.ToCell(translation.X), Terrain.ToCell(translation.Y), Terrain.ToCell(translation.Z));
				float num2 = LightingManager.LightIntensityByLightValue[cellLight];
				float num3 = (float)((0f - Vector3.Dot(vector, Vector3.Normalize(m.Translation - camera.ViewPosition)) > 0.7f) ? 1 : 0);
				num3 *= MathUtils.Saturate(1f * (num - 1f));
				num3 *= MathUtils.Saturate((1f - num2 - 0.5f) / 0.5f);
				if (num3 > 0.25f)
				{
					Vector3 vector2 = Vector3.Normalize(m.Right);
					Vector3 vector3 = -Vector3.Normalize(m.Forward);
					Color color = this.GlowingEyesColor * num3;
					this.m_eyeGlowPoints[0].Position = translation + vector2 * this.GlowingEyesOffset.X + vector3 * this.GlowingEyesOffset.Y + vector * this.GlowingEyesOffset.Z;
					this.m_eyeGlowPoints[0].Right = vector2;
					this.m_eyeGlowPoints[0].Up = vector3;
					this.m_eyeGlowPoints[0].Forward = vector;
					this.m_eyeGlowPoints[0].Size = 0.01f;
					this.m_eyeGlowPoints[0].FarSize = 0.06f;
					this.m_eyeGlowPoints[0].FarDistance = 14f;
					this.m_eyeGlowPoints[0].Color = color;
					this.m_eyeGlowPoints[1].Position = translation - vector2 * this.GlowingEyesOffset.X + vector3 * this.GlowingEyesOffset.Y + vector * this.GlowingEyesOffset.Z;
					this.m_eyeGlowPoints[1].Right = vector2;
					this.m_eyeGlowPoints[1].Up = vector3;
					this.m_eyeGlowPoints[1].Forward = vector;
					this.m_eyeGlowPoints[1].Size = 0.01f;
					this.m_eyeGlowPoints[1].FarSize = 0.06f;
					this.m_eyeGlowPoints[1].FarDistance = 14f;
					this.m_eyeGlowPoints[1].Color = color;
				}
			}
		}

		// Token: 0x040008E7 RID: 2279
		public SubsystemGlow m_subsystemGlow;

		// Token: 0x040008E8 RID: 2280
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040008E9 RID: 2281
		public ComponentCreatureModel m_componentCreatureModel;

		// Token: 0x040008EA RID: 2282
		public GlowPoint[] m_eyeGlowPoints = new GlowPoint[2];

		// Token: 0x040008EB RID: 2283
		public static int[] m_drawOrders = new int[1];
	}
}
