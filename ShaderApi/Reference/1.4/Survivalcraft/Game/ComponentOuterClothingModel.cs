using System;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000223 RID: 547
	public class ComponentOuterClothingModel : ComponentModel
	{
		// Token: 0x06001123 RID: 4387 RVA: 0x0007FB02 File Offset: 0x0007DD02
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentHumanModel = base.Entity.FindComponent<ComponentHumanModel>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x0007FB44 File Offset: 0x0007DD44
		public override void Animate()
		{
			base.Animate();
			if (this.Animated)
			{
				return;
			}
			base.Opacity = this.m_componentHumanModel.Opacity;
			foreach (ModelBone modelBone in base.Model.Bones)
			{
				ModelBone modelBone2 = this.m_componentHumanModel.Model.FindBone(modelBone.Name, true);
				this.SetBoneTransform(modelBone.Index, this.m_componentHumanModel.GetBoneTransform(modelBone2.Index));
			}
			if (base.Opacity != null && base.Opacity.Value < 1f)
			{
				bool flag = this.m_componentCreature.ComponentBody.ImmersionFactor >= 1f;
				bool flag2 = this.m_subsystemSky.ViewUnderWaterDepth > 0f;
				this.RenderingMode = ((flag == flag2) ? ModelRenderingMode.TransparentAfterWater : ModelRenderingMode.TransparentBeforeWater);
				return;
			}
			this.RenderingMode = ModelRenderingMode.AlphaThreshold;
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x0007FC5C File Offset: 0x0007DE5C
		public override void SetModel(Model model)
		{
			base.SetModel(model);
			if (this.IsSet)
			{
				return;
			}
			if (base.MeshDrawOrders.Length != 4)
			{
				throw new InvalidOperationException("Invalid number of meshes in OuterClothing model.");
			}
			base.MeshDrawOrders[0] = model.Meshes.IndexOf(model.FindMesh("Leg1", true));
			base.MeshDrawOrders[1] = model.Meshes.IndexOf(model.FindMesh("Leg2", true));
			base.MeshDrawOrders[2] = model.Meshes.IndexOf(model.FindMesh("Body", true));
			base.MeshDrawOrders[3] = model.Meshes.IndexOf(model.FindMesh("Head", true));
		}

		// Token: 0x04000A3D RID: 2621
		public ComponentHumanModel m_componentHumanModel;

		// Token: 0x04000A3E RID: 2622
		public ComponentCreature m_componentCreature;
	}
}
