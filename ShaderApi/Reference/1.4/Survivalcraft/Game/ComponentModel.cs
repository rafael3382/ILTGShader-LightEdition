using System;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200021E RID: 542
	public class ComponentModel : Component
	{
		// Token: 0x17000244 RID: 580
		// (get) Token: 0x060010DB RID: 4315 RVA: 0x0007EB94 File Offset: 0x0007CD94
		// (set) Token: 0x060010DC RID: 4316 RVA: 0x0007EB9C File Offset: 0x0007CD9C
		public float? Opacity { get; set; }

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x060010DD RID: 4317 RVA: 0x0007EBA5 File Offset: 0x0007CDA5
		// (set) Token: 0x060010DE RID: 4318 RVA: 0x0007EBAD File Offset: 0x0007CDAD
		public Vector3? DiffuseColor { get; set; }

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060010DF RID: 4319 RVA: 0x0007EBB6 File Offset: 0x0007CDB6
		// (set) Token: 0x060010E0 RID: 4320 RVA: 0x0007EBBE File Offset: 0x0007CDBE
		public Vector4? EmissionColor { get; set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060010E1 RID: 4321 RVA: 0x0007EBC7 File Offset: 0x0007CDC7
		// (set) Token: 0x060010E2 RID: 4322 RVA: 0x0007EBCF File Offset: 0x0007CDCF
		public Model Model
		{
			get
			{
				return this.m_model;
			}
			set
			{
				this.SetModel(value);
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060010E3 RID: 4323 RVA: 0x0007EBD8 File Offset: 0x0007CDD8
		// (set) Token: 0x060010E4 RID: 4324 RVA: 0x0007EBE0 File Offset: 0x0007CDE0
		public Texture2D TextureOverride { get; set; }

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x060010E5 RID: 4325 RVA: 0x0007EBE9 File Offset: 0x0007CDE9
		// (set) Token: 0x060010E6 RID: 4326 RVA: 0x0007EBF1 File Offset: 0x0007CDF1
		public virtual Func<bool> OnAnimate { get; set; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x060010E7 RID: 4327 RVA: 0x0007EBFA File Offset: 0x0007CDFA
		// (set) Token: 0x060010E8 RID: 4328 RVA: 0x0007EC02 File Offset: 0x0007CE02
		public bool CastsShadow { get; set; }

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x060010E9 RID: 4329 RVA: 0x0007EC0B File Offset: 0x0007CE0B
		// (set) Token: 0x060010EA RID: 4330 RVA: 0x0007EC13 File Offset: 0x0007CE13
		public int PrepareOrder { get; set; }

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x060010EB RID: 4331 RVA: 0x0007EC1C File Offset: 0x0007CE1C
		// (set) Token: 0x060010EC RID: 4332 RVA: 0x0007EC24 File Offset: 0x0007CE24
		public virtual ModelRenderingMode RenderingMode { get; set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x060010ED RID: 4333 RVA: 0x0007EC2D File Offset: 0x0007CE2D
		// (set) Token: 0x060010EE RID: 4334 RVA: 0x0007EC35 File Offset: 0x0007CE35
		public int[] MeshDrawOrders { get; set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x060010EF RID: 4335 RVA: 0x0007EC3E File Offset: 0x0007CE3E
		// (set) Token: 0x060010F0 RID: 4336 RVA: 0x0007EC46 File Offset: 0x0007CE46
		public bool IsVisibleForCamera { get; set; }

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x060010F1 RID: 4337 RVA: 0x0007EC4F File Offset: 0x0007CE4F
		// (set) Token: 0x060010F2 RID: 4338 RVA: 0x0007EC57 File Offset: 0x0007CE57
		public Matrix[] AbsoluteBoneTransformsForCamera { get; set; }

		// Token: 0x060010F3 RID: 4339 RVA: 0x0007EC60 File Offset: 0x0007CE60
		public virtual Matrix? GetBoneTransform(int boneIndex)
		{
			return this.m_boneTransforms[boneIndex];
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x0007EC6E File Offset: 0x0007CE6E
		public virtual void SetBoneTransform(int boneIndex, Matrix? transformation)
		{
			this.m_boneTransforms[boneIndex] = transformation;
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x0007EC7D File Offset: 0x0007CE7D
		public virtual void CalculateAbsoluteBonesTransforms(Camera camera)
		{
			this.ProcessBoneHierarchy(this.Model.RootBone, camera.ViewMatrix, this.AbsoluteBoneTransformsForCamera);
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x0007EC9C File Offset: 0x0007CE9C
		public virtual void CalculateIsVisible(Camera camera)
		{
			if (camera.GameWidget.IsEntityFirstPersonTarget(base.Entity))
			{
				this.IsVisibleForCamera = false;
				return;
			}
			float num = MathUtils.Sqr(this.m_subsystemSky.VisibilityRange);
			Vector3 vector = this.m_componentFrame.Position - camera.ViewPosition;
			vector.Y *= this.m_subsystemSky.VisibilityRangeYMultiplier;
			if (vector.LengthSquared() < num)
			{
				BoundingSphere sphere = new BoundingSphere(this.m_componentFrame.Position, this.m_boundingSphereRadius);
				this.IsVisibleForCamera = camera.ViewFrustum.Intersection(sphere);
				return;
			}
			this.IsVisibleForCamera = false;
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x0007ED3E File Offset: 0x0007CF3E
		public virtual void Animate()
		{
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x0007ED40 File Offset: 0x0007CF40
		public virtual void DrawExtras(Camera camera)
		{
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x0007ED44 File Offset: 0x0007CF44
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentFrame = base.Entity.FindComponent<ComponentFrame>(true);
			string value = valuesDictionary.GetValue<string>("ModelName");
			Type type = TypeCache.FindType(valuesDictionary.GetValue<string>("ModelType", "Engine.Graphics.Model"), true, true);
			this.Model = (Model)ContentManager.Get(type, value, null);
			this.CastsShadow = valuesDictionary.GetValue<bool>("CastsShadow");
			string value2 = valuesDictionary.GetValue<string>("TextureOverride");
			this.TextureOverride = (string.IsNullOrEmpty(value2) ? null : ContentManager.Get<Texture2D>(value2, null));
			this.PrepareOrder = valuesDictionary.GetValue<int>("PrepareOrder");
			this.m_boundingSphereRadius = valuesDictionary.GetValue<float>("BoundingSphereRadius");
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x0007EE04 File Offset: 0x0007D004
		public virtual void SetModel(Model model)
		{
			this.IsSet = false;
			ModsManager.HookAction("OnSetModel", delegate(ModLoader modLoader)
			{
				modLoader.OnSetModel(this, model, out this.IsSet);
				return this.IsSet;
			});
			if (this.IsSet)
			{
				return;
			}
			this.m_model = model;
			if (this.m_model != null)
			{
				this.m_boneTransforms = new Matrix?[this.m_model.Bones.Count];
				this.AbsoluteBoneTransformsForCamera = new Matrix[this.m_model.Bones.Count];
				this.MeshDrawOrders = Enumerable.Range(0, this.m_model.Meshes.Count).ToArray<int>();
				return;
			}
			this.m_boneTransforms = null;
			this.AbsoluteBoneTransformsForCamera = null;
			this.MeshDrawOrders = null;
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x0007EED8 File Offset: 0x0007D0D8
		public virtual void ProcessBoneHierarchy(ModelBone modelBone, Matrix currentTransform, Matrix[] transforms)
		{
			Matrix m = modelBone.Transform;
			if (this.m_boneTransforms[modelBone.Index] != null)
			{
				Vector3 translation = m.Translation;
				m.Translation = Vector3.Zero;
				m *= this.m_boneTransforms[modelBone.Index].Value;
				m.Translation += translation;
				Matrix.MultiplyRestricted(ref m, ref currentTransform, out transforms[modelBone.Index]);
			}
			else
			{
				Matrix.MultiplyRestricted(ref m, ref currentTransform, out transforms[modelBone.Index]);
			}
			foreach (ModelBone modelBone2 in modelBone.ChildBones)
			{
				this.ProcessBoneHierarchy(modelBone2, transforms[modelBone.Index], transforms);
			}
		}

		// Token: 0x04000A12 RID: 2578
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000A13 RID: 2579
		public bool IsSet;

		// Token: 0x04000A14 RID: 2580
		public bool Animated;

		// Token: 0x04000A15 RID: 2581
		public ComponentFrame m_componentFrame;

		// Token: 0x04000A16 RID: 2582
		public Model m_model;

		// Token: 0x04000A17 RID: 2583
		public Matrix?[] m_boneTransforms;

		// Token: 0x04000A18 RID: 2584
		public float m_boundingSphereRadius;
	}
}
