using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000394 RID: 916
	public class ModelWidget : Widget
	{
		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06001B7A RID: 7034 RVA: 0x000D6BF9 File Offset: 0x000D4DF9
		// (set) Token: 0x06001B7B RID: 7035 RVA: 0x000D6C01 File Offset: 0x000D4E01
		public Vector2 Size { get; set; }

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06001B7C RID: 7036 RVA: 0x000D6C0A File Offset: 0x000D4E0A
		// (set) Token: 0x06001B7D RID: 7037 RVA: 0x000D6C12 File Offset: 0x000D4E12
		public Color Color { get; set; }

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06001B7E RID: 7038 RVA: 0x000D6C1B File Offset: 0x000D4E1B
		// (set) Token: 0x06001B7F RID: 7039 RVA: 0x000D6C23 File Offset: 0x000D4E23
		public bool UseAlphaThreshold { get; set; }

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06001B80 RID: 7040 RVA: 0x000D6C2C File Offset: 0x000D4E2C
		// (set) Token: 0x06001B81 RID: 7041 RVA: 0x000D6C34 File Offset: 0x000D4E34
		public bool IsPerspective { get; set; }

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06001B82 RID: 7042 RVA: 0x000D6C3D File Offset: 0x000D4E3D
		// (set) Token: 0x06001B83 RID: 7043 RVA: 0x000D6C45 File Offset: 0x000D4E45
		public Vector3 OrthographicFrustumSize { get; set; }

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06001B84 RID: 7044 RVA: 0x000D6C4E File Offset: 0x000D4E4E
		// (set) Token: 0x06001B85 RID: 7045 RVA: 0x000D6C56 File Offset: 0x000D4E56
		public Vector3 ViewPosition { get; set; }

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06001B86 RID: 7046 RVA: 0x000D6C5F File Offset: 0x000D4E5F
		// (set) Token: 0x06001B87 RID: 7047 RVA: 0x000D6C67 File Offset: 0x000D4E67
		public Vector3 ViewTarget { get; set; }

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06001B88 RID: 7048 RVA: 0x000D6C70 File Offset: 0x000D4E70
		// (set) Token: 0x06001B89 RID: 7049 RVA: 0x000D6C78 File Offset: 0x000D4E78
		public float ViewFov { get; set; }

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06001B8A RID: 7050 RVA: 0x000D6C81 File Offset: 0x000D4E81
		// (set) Token: 0x06001B8B RID: 7051 RVA: 0x000D6C89 File Offset: 0x000D4E89
		public Matrix ModelMatrix { get; set; } = Matrix.Identity;

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06001B8C RID: 7052 RVA: 0x000D6C92 File Offset: 0x000D4E92
		// (set) Token: 0x06001B8D RID: 7053 RVA: 0x000D6C9A File Offset: 0x000D4E9A
		public Vector3 AutoRotationVector { get; set; }

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06001B8E RID: 7054 RVA: 0x000D6CA3 File Offset: 0x000D4EA3
		// (set) Token: 0x06001B8F RID: 7055 RVA: 0x000D6CAC File Offset: 0x000D4EAC
		public Model Model
		{
			get
			{
				return this.m_model;
			}
			set
			{
				if (value != this.m_model)
				{
					this.m_model = value;
					if (this.m_model != null)
					{
						this.m_boneTransforms = new Matrix?[this.m_model.Bones.Count];
						this.m_absoluteBoneTransforms = new Matrix[this.m_model.Bones.Count];
						return;
					}
					this.m_boneTransforms = null;
					this.m_absoluteBoneTransforms = null;
				}
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06001B90 RID: 7056 RVA: 0x000D6D1C File Offset: 0x000D4F1C
		// (set) Token: 0x06001B91 RID: 7057 RVA: 0x000D6D24 File Offset: 0x000D4F24
		public Texture2D TextureOverride { get; set; }

		// Token: 0x06001B92 RID: 7058 RVA: 0x000D6D30 File Offset: 0x000D4F30
		public ModelWidget()
		{
			this.Size = new Vector2(float.PositiveInfinity);
			this.IsHitTestVisible = false;
			this.Color = Color.White;
			this.UseAlphaThreshold = false;
			this.IsPerspective = true;
			this.ViewPosition = new Vector3(0f, 0f, -5f);
			this.ViewTarget = new Vector3(0f, 0f, 0f);
			this.ViewFov = 1f;
			this.OrthographicFrustumSize = new Vector3(0f, 10f, 10f);
		}

		// Token: 0x06001B93 RID: 7059 RVA: 0x000D6DD7 File Offset: 0x000D4FD7
		public Matrix? GetBoneTransform(int boneIndex)
		{
			return this.m_boneTransforms[boneIndex];
		}

		// Token: 0x06001B94 RID: 7060 RVA: 0x000D6DE5 File Offset: 0x000D4FE5
		public void SetBoneTransform(int boneIndex, Matrix? transformation)
		{
			this.m_boneTransforms[boneIndex] = transformation;
		}

		// Token: 0x06001B95 RID: 7061 RVA: 0x000D6DF4 File Offset: 0x000D4FF4
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.Model == null)
			{
				return;
			}
			LitShader litShader = this.UseAlphaThreshold ? ModelWidget.m_shaderAlpha : ModelWidget.m_shader;
			litShader.Texture = this.TextureOverride;
			litShader.SamplerState = SamplerState.PointClamp;
			litShader.MaterialColor = new Vector4(this.Color * base.GlobalColorTransform);
			litShader.AmbientLightColor = new Vector3(0.66f, 0.66f, 0.66f);
			litShader.DiffuseLightColor1 = new Vector3(1f, 1f, 1f);
			litShader.LightDirection1 = Vector3.Normalize(new Vector3(1f, 1f, 1f));
			if (this.UseAlphaThreshold)
			{
				litShader.AlphaThreshold = 0f;
			}
			litShader.Transforms.View = Matrix.CreateLookAt(this.ViewPosition, this.ViewTarget, Vector3.UnitY);
			Viewport viewport = Display.Viewport;
			float num = base.ActualSize.X / base.ActualSize.Y;
			if (this.IsPerspective)
			{
				litShader.Transforms.Projection = Matrix.CreatePerspectiveFieldOfView(this.ViewFov, num, 0.1f, 100f) * MatrixUtils.CreateScaleTranslation(0.5f * base.ActualSize.X, -0.5f * base.ActualSize.Y, base.ActualSize.X / 2f, base.ActualSize.Y / 2f) * base.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)viewport.Width, -2f / (float)viewport.Height, -1f, 1f);
			}
			else
			{
				Vector3 orthographicFrustumSize = this.OrthographicFrustumSize;
				if (orthographicFrustumSize.X < 0f)
				{
					orthographicFrustumSize.X = orthographicFrustumSize.Y / num;
				}
				else if (orthographicFrustumSize.Y < 0f)
				{
					orthographicFrustumSize.Y = orthographicFrustumSize.X * num;
				}
				litShader.Transforms.Projection = Matrix.CreateOrthographic(orthographicFrustumSize.X, orthographicFrustumSize.Y, 0f, this.OrthographicFrustumSize.Z) * MatrixUtils.CreateScaleTranslation(0.5f * base.ActualSize.X, -0.5f * base.ActualSize.Y, base.ActualSize.X / 2f, base.ActualSize.Y / 2f) * base.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)viewport.Width, -2f / (float)viewport.Height, -1f, 1f);
			}
			Display.DepthStencilState = DepthStencilState.Default;
			Display.BlendState = BlendState.AlphaBlend;
			Display.RasterizerState = RasterizerState.CullNoneScissor;
			this.ProcessBoneHierarchy(this.Model.RootBone, Matrix.Identity, this.m_absoluteBoneTransforms);
			float num2 = (float)Time.RealTime + (float)(this.GetHashCode() % 1000) / 100f;
			Matrix m = (this.AutoRotationVector.LengthSquared() > 0f) ? Matrix.CreateFromAxisAngle(Vector3.Normalize(this.AutoRotationVector), this.AutoRotationVector.Length() * num2) : Matrix.Identity;
			foreach (ModelMesh modelMesh in this.Model.Meshes)
			{
				litShader.Transforms.World[0] = this.m_absoluteBoneTransforms[modelMesh.ParentBone.Index] * this.ModelMatrix * m;
				foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
				{
					if (modelMeshPart.IndicesCount > 0)
					{
						Display.DrawIndexed(PrimitiveType.TriangleList, litShader, modelMeshPart.VertexBuffer, modelMeshPart.IndexBuffer, modelMeshPart.StartIndex, modelMeshPart.IndicesCount);
					}
				}
			}
		}

		// Token: 0x06001B96 RID: 7062 RVA: 0x000D7238 File Offset: 0x000D5438
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = (this.Model != null);
			base.DesiredSize = this.Size;
		}

		// Token: 0x06001B97 RID: 7063 RVA: 0x000D7258 File Offset: 0x000D5458
		public void ProcessBoneHierarchy(ModelBone modelBone, Matrix currentTransform, Matrix[] transforms)
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

		// Token: 0x040012A7 RID: 4775
		public static LitShader m_shader = new LitShader(1, false, false, true, false, false, 1);

		// Token: 0x040012A8 RID: 4776
		public static LitShader m_shaderAlpha = new LitShader(1, false, false, true, false, true, 1);

		// Token: 0x040012A9 RID: 4777
		public Model m_model;

		// Token: 0x040012AA RID: 4778
		public Matrix?[] m_boneTransforms;

		// Token: 0x040012AB RID: 4779
		public Matrix[] m_absoluteBoneTransforms;
	}
}
