using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000282 RID: 642
	public class NModelShader : Shader
	{
		// Token: 0x17000252 RID: 594
		// (set) Token: 0x06000F29 RID: 3881 RVA: 0x0006995E File Offset: 0x00067B5E
		public Texture2D Texture
		{
			set
			{
				this.m_textureParameter.SetValue(value);
			}
		}

		// Token: 0x17000253 RID: 595
		// (set) Token: 0x06000F2A RID: 3882 RVA: 0x0006996C File Offset: 0x00067B6C
		public SamplerState SamplerState
		{
			set
			{
				this.m_samplerStateParameter.SetValue(value);
			}
		}

		// Token: 0x17000254 RID: 596
		// (set) Token: 0x06000F2B RID: 3883 RVA: 0x0006997A File Offset: 0x00067B7A
		public Vector4 MaterialColor
		{
			set
			{
				this.m_materialColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000255 RID: 597
		// (set) Token: 0x06000F2C RID: 3884 RVA: 0x00069988 File Offset: 0x00067B88
		public Vector4 EmissionColor
		{
			set
			{
				this.m_emissionColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000256 RID: 598
		// (set) Token: 0x06000F2D RID: 3885 RVA: 0x00069996 File Offset: 0x00067B96
		public float AlphaThreshold
		{
			set
			{
				this.m_alphaThresholdParameter.SetValue(value);
			}
		}

		// Token: 0x17000257 RID: 599
		// (set) Token: 0x06000F2E RID: 3886 RVA: 0x000699A4 File Offset: 0x00067BA4
		public Vector3 AmbientLightColor
		{
			set
			{
				this.m_ambientLightColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000258 RID: 600
		// (set) Token: 0x06000F2F RID: 3887 RVA: 0x000699B2 File Offset: 0x00067BB2
		public Vector3 DiffuseLightColor1
		{
			set
			{
				this.m_diffuseLightColor1Parameter.SetValue(value);
			}
		}

		// Token: 0x17000259 RID: 601
		// (set) Token: 0x06000F30 RID: 3888 RVA: 0x000699C0 File Offset: 0x00067BC0
		public Vector3 DiffuseLightColor2
		{
			set
			{
				this.m_diffuseLightColor2Parameter.SetValue(value);
			}
		}

		// Token: 0x1700025A RID: 602
		// (set) Token: 0x06000F31 RID: 3889 RVA: 0x000699CE File Offset: 0x00067BCE
		public Vector3 LightDirection1
		{
			set
			{
				this.m_directionToLight1Parameter.SetValue(-value);
			}
		}

		// Token: 0x1700025B RID: 603
		// (set) Token: 0x06000F32 RID: 3890 RVA: 0x000699E1 File Offset: 0x00067BE1
		public Vector3 LightDirection2
		{
			set
			{
				this.m_directionToLight2Parameter.SetValue(-value);
			}
		}

		// Token: 0x1700025C RID: 604
		// (set) Token: 0x06000F33 RID: 3891 RVA: 0x000699F4 File Offset: 0x00067BF4
		public Vector3 FogColor
		{
			set
			{
				this.m_fogColorParameter.SetValue(value);
			}
		}

		// Token: 0x1700025D RID: 605
		// (set) Token: 0x06000F34 RID: 3892 RVA: 0x00069A02 File Offset: 0x00067C02
		public Vector2 FogStartInvLength
		{
			set
			{
				this.m_fogStartInvLengthParameter.SetValue(value);
			}
		}

		// Token: 0x1700025E RID: 606
		// (set) Token: 0x06000F35 RID: 3893 RVA: 0x00069A10 File Offset: 0x00067C10
		public float FogYMultiplier
		{
			set
			{
				this.m_fogYMultiplierParameter.SetValue(value);
			}
		}

		// Token: 0x1700025F RID: 607
		// (set) Token: 0x06000F36 RID: 3894 RVA: 0x00069A1E File Offset: 0x00067C1E
		public Vector3 WorldUp
		{
			set
			{
				this.m_worldUpParameter.SetValue(value);
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06000F37 RID: 3895 RVA: 0x00069A2C File Offset: 0x00067C2C
		// (set) Token: 0x06000F38 RID: 3896 RVA: 0x00069A34 File Offset: 0x00067C34
		public int InstancesCount
		{
			get
			{
				return this.m_instancesCount;
			}
			set
			{
				if (value < 0 || value > this.Transforms.MaxWorldMatrices)
				{
					throw new InvalidOperationException("Invalid instances count.");
				}
				this.m_instancesCount = value;
			}
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x00069A5C File Offset: 0x00067C5C
		public NModelShader(bool useAlphaThreshold, int maxInstancesCount = 1) : base(ShaderReader.GetShader("Model.vsh"), ShaderReader.GetShader("Model.psh"), ModelShader.PrepareShaderMacros(useAlphaThreshold, maxInstancesCount))
		{
			this.m_worldMatrixParameter = base.GetParameter("u_worldMatrix", false);
			this.m_worldViewProjectionMatrixParameter = base.GetParameter("u_worldViewProjectionMatrix", false);
			this.m_textureParameter = base.GetParameter("u_texture", false);
			this.m_samplerStateParameter = base.GetParameter("u_samplerState", false);
			this.m_materialColorParameter = base.GetParameter("u_materialColor", false);
			this.m_emissionColorParameter = base.GetParameter("u_emissionColor", false);
			this.m_alphaThresholdParameter = base.GetParameter("u_alphaThreshold", true);
			this.m_ambientLightColorParameter = base.GetParameter("u_ambientLightColor", false);
			this.m_diffuseLightColor1Parameter = base.GetParameter("u_diffuseLightColor1", false);
			this.m_directionToLight1Parameter = base.GetParameter("u_directionToLight1", false);
			this.m_diffuseLightColor2Parameter = base.GetParameter("u_diffuseLightColor2", false);
			this.m_directionToLight2Parameter = base.GetParameter("u_directionToLight2", false);
			this.m_fogColorParameter = base.GetParameter("u_fogColor", false);
			this.m_fogStartInvLengthParameter = base.GetParameter("u_fogStartInvLength", false);
			this.m_fogYMultiplierParameter = base.GetParameter("u_fogYMultiplier", false);
			this.m_worldUpParameter = base.GetParameter("u_worldUp", false);
			this.Transforms = new ShaderTransforms(maxInstancesCount);
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x00069BB8 File Offset: 0x00067DB8
		protected override void PrepareForDrawingOverride()
		{
			this.Transforms.UpdateMatrices(this.m_instancesCount, false, false, true);
			this.m_worldViewProjectionMatrixParameter.SetValue(this.Transforms.WorldViewProjection, this.InstancesCount);
			this.m_worldMatrixParameter.SetValue(this.Transforms.World, this.InstancesCount);
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x00069C14 File Offset: 0x00067E14
		public static ShaderMacro[] PrepareShaderMacros(bool useAlphaThreshold, int maxInstancesCount)
		{
			List<ShaderMacro> list = new List<ShaderMacro>();
			if (useAlphaThreshold)
			{
				list.Add(new ShaderMacro("ALPHATESTED"));
			}
			list.Add(new ShaderMacro("MAX_INSTANCES_COUNT", maxInstancesCount.ToString(CultureInfo.InvariantCulture)));
			return list.ToArray();
		}

		// Token: 0x040011FE RID: 4606
		public ShaderParameter m_worldMatrixParameter;

		// Token: 0x040011FF RID: 4607
		public ShaderParameter m_worldViewProjectionMatrixParameter;

		// Token: 0x04001200 RID: 4608
		public ShaderParameter m_textureParameter;

		// Token: 0x04001201 RID: 4609
		public ShaderParameter m_samplerStateParameter;

		// Token: 0x04001202 RID: 4610
		public ShaderParameter m_materialColorParameter;

		// Token: 0x04001203 RID: 4611
		public ShaderParameter m_emissionColorParameter;

		// Token: 0x04001204 RID: 4612
		public ShaderParameter m_alphaThresholdParameter;

		// Token: 0x04001205 RID: 4613
		public ShaderParameter m_ambientLightColorParameter;

		// Token: 0x04001206 RID: 4614
		public ShaderParameter m_diffuseLightColor1Parameter;

		// Token: 0x04001207 RID: 4615
		public ShaderParameter m_directionToLight1Parameter;

		// Token: 0x04001208 RID: 4616
		public ShaderParameter m_diffuseLightColor2Parameter;

		// Token: 0x04001209 RID: 4617
		public ShaderParameter m_directionToLight2Parameter;

		// Token: 0x0400120A RID: 4618
		public ShaderParameter m_fogColorParameter;

		// Token: 0x0400120B RID: 4619
		public ShaderParameter m_fogStartInvLengthParameter;

		// Token: 0x0400120C RID: 4620
		public ShaderParameter m_fogYMultiplierParameter;

		// Token: 0x0400120D RID: 4621
		public ShaderParameter m_worldUpParameter;

		// Token: 0x0400120E RID: 4622
		public int m_instancesCount;

		// Token: 0x0400120F RID: 4623
		public readonly ShaderTransforms Transforms;
	}
}
