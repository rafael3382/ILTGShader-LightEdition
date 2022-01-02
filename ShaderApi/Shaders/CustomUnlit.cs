using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using Engine.Graphics;
using Game;

namespace ShaderApi
{
	// Token: 0x02000148 RID: 328
	public class CustomUnlitShader : Shader
	{
		// Token: 0x17000188 RID: 392
		// (set) Token: 0x06000C2E RID: 3118 RVA: 0x000356A1 File Offset: 0x000338A1
		public Texture2D Texture
		{
			set
			{
				this.m_textureParameter.SetValue(value);
			}
		}

		// Token: 0x17000189 RID: 393
		// (set) Token: 0x06000C2F RID: 3119 RVA: 0x000356AF File Offset: 0x000338AF
		public SamplerState SamplerState
		{
			set
			{
				this.m_samplerStateParameter.SetValue(value);
			}
		}

		// Token: 0x1700018A RID: 394
		// (set) Token: 0x06000C30 RID: 3120 RVA: 0x000356BD File Offset: 0x000338BD
		public Vector4 Color
		{
			set
			{
				this.m_colorParameter.SetValue(value);
			}
		}

		// Token: 0x1700018B RID: 395
		// (set) Token: 0x06000C31 RID: 3121 RVA: 0x000356CB File Offset: 0x000338CB
		public float AlphaThreshold
		{
			set
			{
				this.m_alphaThresholdParameter.SetValue(value);
			}
		}
        
		
		public CustomUnlitShader(string shadername, bool useVertexColor, bool useTexture, bool useAlphaThreshold) 
             : base(ShaderReader.GetShader(shadername+".vsh"), ShaderReader.GetShader(shadername+".psh"), PrepareShaderMacros(useVertexColor, useTexture, useAlphaThreshold))
		{
			this.m_worldViewProjectionMatrixParameter = base.GetParameter("u_worldViewProjectionMatrix", true);
			this.m_textureParameter = base.GetParameter("u_texture", true);
			this.m_samplerStateParameter = base.GetParameter("u_samplerState", true);
			this.m_colorParameter = base.GetParameter("u_color", true);
			this.m_alphaThresholdParameter = base.GetParameter("u_alphaThreshold", true);
			this.Transforms = new ShaderTransforms(1);
			this.Color = Vector4.One;
			
			
		}
        
		// Token: 0x06000C33 RID: 3123 RVA: 0x00035792 File Offset: 0x00033992
		protected override void PrepareForDrawingOverride()
		{
			this.Transforms.UpdateMatrices(1, false, false, true);
			this.m_worldViewProjectionMatrixParameter.SetValue(this.Transforms.WorldViewProjection, 1);
			
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x000357BC File Offset: 0x000339BC
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

		// Token: 0x040005A6 RID: 1446
		public ShaderParameter m_worldViewProjectionMatrixParameter;

		// Token: 0x040005A7 RID: 1447
		public ShaderParameter m_textureParameter;

		// Token: 0x040005A8 RID: 1448
		public ShaderParameter m_samplerStateParameter;

		// Token: 0x040005A9 RID: 1449
		public ShaderParameter m_colorParameter;

		// Token: 0x040005AA RID: 1450
		public ShaderParameter m_alphaThresholdParameter;

		// Token: 0x040005AB RID: 1451
		public readonly ShaderTransforms Transforms;
	}
}
