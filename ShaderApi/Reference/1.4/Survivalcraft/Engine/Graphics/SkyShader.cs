using System;
using System.Collections.Generic;

namespace Engine.Graphics
{
	// Token: 0x0200000B RID: 11
	public class SkyShader : Shader
	{
		// Token: 0x17000009 RID: 9
		// (set) Token: 0x0600009A RID: 154 RVA: 0x0000758D File Offset: 0x0000578D
		public Texture2D Texture
		{
			set
			{
				this.m_textureParameter.SetValue(value);
			}
		}

		// Token: 0x1700000A RID: 10
		// (set) Token: 0x0600009B RID: 155 RVA: 0x0000759B File Offset: 0x0000579B
		public SamplerState SamplerState
		{
			set
			{
				this.m_samplerStateParameter.SetValue(value);
			}
		}

		// Token: 0x1700000B RID: 11
		// (set) Token: 0x0600009C RID: 156 RVA: 0x000075A9 File Offset: 0x000057A9
		public Vector4 Color
		{
			set
			{
				this.m_colorParameter.SetValue(value);
			}
		}

		// Token: 0x1700000C RID: 12
		// (set) Token: 0x0600009D RID: 157 RVA: 0x000075B7 File Offset: 0x000057B7
		public float AlphaThreshold
		{
			set
			{
				this.m_alphaThresholdParameter.SetValue(value);
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000075C8 File Offset: 0x000057C8
		public SkyShader(string vsc, string psc, bool useVertexColor, bool useTexture, bool useAlphaThreshold) : base(vsc, psc, SkyShader.PrepareShaderMacros(useVertexColor, useTexture, useAlphaThreshold))
		{
			this.m_worldViewProjectionMatrixParameter = base.GetParameter("u_worldViewProjectionMatrix", true);
			this.m_textureParameter = base.GetParameter("u_texture", true);
			this.m_samplerStateParameter = base.GetParameter("u_samplerState", true);
			this.m_colorParameter = base.GetParameter("u_color", true);
			this.m_alphaThresholdParameter = base.GetParameter("u_alphaThreshold", true);
			this.Transforms = new ShaderTransforms(1);
			this.Color = Vector4.One;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00007658 File Offset: 0x00005858
		protected override void PrepareForDrawingOverride()
		{
			this.Transforms.UpdateMatrices(1, false, false, true);
			this.m_worldViewProjectionMatrixParameter.SetValue(this.Transforms.WorldViewProjection, 1);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00007680 File Offset: 0x00005880
		public static ShaderMacro[] PrepareShaderMacros(bool useVertexColor, bool useTexture, bool useAlphaThreshold)
		{
			List<ShaderMacro> list = new List<ShaderMacro>();
			if (useVertexColor)
			{
				list.Add(new ShaderMacro("USE_VERTEXCOLOR"));
			}
			if (useTexture)
			{
				list.Add(new ShaderMacro("USE_TEXTURE"));
			}
			if (useAlphaThreshold)
			{
				list.Add(new ShaderMacro("USE_ALPHATHRESHOLD"));
			}
			return list.ToArray();
		}

		// Token: 0x04000051 RID: 81
		public ShaderParameter m_worldViewProjectionMatrixParameter;

		// Token: 0x04000052 RID: 82
		public ShaderParameter m_textureParameter;

		// Token: 0x04000053 RID: 83
		public ShaderParameter m_samplerStateParameter;

		// Token: 0x04000054 RID: 84
		public ShaderParameter m_colorParameter;

		// Token: 0x04000055 RID: 85
		public ShaderParameter m_alphaThresholdParameter;

		// Token: 0x04000056 RID: 86
		public readonly ShaderTransforms Transforms;
	}
}
