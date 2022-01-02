using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002C7 RID: 711
	public class ModelShader : Shader
	{
		// Token: 0x1700033D RID: 829
		// (set) Token: 0x0600159F RID: 5535 RVA: 0x000A30E2 File Offset: 0x000A12E2
		public Texture2D Texture
		{
			set
			{
				this.m_textureParameter.SetValue(value);
			}
		}

		// Token: 0x1700033E RID: 830
		// (set) Token: 0x060015A0 RID: 5536 RVA: 0x000A30F0 File Offset: 0x000A12F0
		public SamplerState SamplerState
		{
			set
			{
				this.m_samplerStateParameter.SetValue(value);
			}
		}

		// Token: 0x1700033F RID: 831
		// (set) Token: 0x060015A1 RID: 5537 RVA: 0x000A30FE File Offset: 0x000A12FE
		public Vector4 MaterialColor
		{
			set
			{
				this.m_materialColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000340 RID: 832
		// (set) Token: 0x060015A2 RID: 5538 RVA: 0x000A310C File Offset: 0x000A130C
		public Vector4 EmissionColor
		{
			set
			{
				this.m_emissionColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000341 RID: 833
		// (set) Token: 0x060015A3 RID: 5539 RVA: 0x000A311A File Offset: 0x000A131A
		public float AlphaThreshold
		{
			set
			{
				this.m_alphaThresholdParameter.SetValue(value);
			}
		}

		// Token: 0x17000342 RID: 834
		// (set) Token: 0x060015A4 RID: 5540 RVA: 0x000A3128 File Offset: 0x000A1328
		public Vector3 AmbientLightColor
		{
			set
			{
				this.m_ambientLightColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000343 RID: 835
		// (set) Token: 0x060015A5 RID: 5541 RVA: 0x000A3136 File Offset: 0x000A1336
		public Vector3 DiffuseLightColor1
		{
			set
			{
				this.m_diffuseLightColor1Parameter.SetValue(value);
			}
		}

		// Token: 0x17000344 RID: 836
		// (set) Token: 0x060015A6 RID: 5542 RVA: 0x000A3144 File Offset: 0x000A1344
		public Vector3 DiffuseLightColor2
		{
			set
			{
				this.m_diffuseLightColor2Parameter.SetValue(value);
			}
		}

		// Token: 0x17000345 RID: 837
		// (set) Token: 0x060015A7 RID: 5543 RVA: 0x000A3152 File Offset: 0x000A1352
		public Vector3 LightDirection1
		{
			set
			{
				this.m_directionToLight1Parameter.SetValue(-value);
			}
		}

		// Token: 0x17000346 RID: 838
		// (set) Token: 0x060015A8 RID: 5544 RVA: 0x000A3165 File Offset: 0x000A1365
		public Vector3 LightDirection2
		{
			set
			{
				this.m_directionToLight2Parameter.SetValue(-value);
			}
		}

		// Token: 0x17000347 RID: 839
		// (set) Token: 0x060015A9 RID: 5545 RVA: 0x000A3178 File Offset: 0x000A1378
		public Vector3 FogColor
		{
			set
			{
				this.m_fogColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000348 RID: 840
		// (set) Token: 0x060015AA RID: 5546 RVA: 0x000A3186 File Offset: 0x000A1386
		public Vector2 FogStartInvLength
		{
			set
			{
				this.m_fogStartInvLengthParameter.SetValue(value);
			}
		}

		// Token: 0x17000349 RID: 841
		// (set) Token: 0x060015AB RID: 5547 RVA: 0x000A3194 File Offset: 0x000A1394
		public float FogYMultiplier
		{
			set
			{
				this.m_fogYMultiplierParameter.SetValue(value);
			}
		}

		// Token: 0x1700034A RID: 842
		// (set) Token: 0x060015AC RID: 5548 RVA: 0x000A31A2 File Offset: 0x000A13A2
		public Vector3 WorldUp
		{
			set
			{
				this.m_worldUpParameter.SetValue(value);
			}
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x060015AD RID: 5549 RVA: 0x000A31B0 File Offset: 0x000A13B0
		// (set) Token: 0x060015AE RID: 5550 RVA: 0x000A31B8 File Offset: 0x000A13B8
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

		// Token: 0x060015AF RID: 5551 RVA: 0x000A31E0 File Offset: 0x000A13E0
		public ModelShader(string vsc, string psc, bool useAlphaThreshold, int maxInstancesCount = 1) : base(vsc, psc, ModelShader.PrepareShaderMacros(useAlphaThreshold, maxInstancesCount))
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

		// Token: 0x060015B0 RID: 5552 RVA: 0x000A332C File Offset: 0x000A152C
		protected override void PrepareForDrawingOverride()
		{
			this.Transforms.UpdateMatrices(this.m_instancesCount, false, false, true);
			this.m_worldViewProjectionMatrixParameter.SetValue(this.Transforms.WorldViewProjection, this.InstancesCount);
			this.m_worldMatrixParameter.SetValue(this.Transforms.World, this.InstancesCount);
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x000A3388 File Offset: 0x000A1588
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

		// Token: 0x04000E25 RID: 3621
		public ShaderParameter m_worldMatrixParameter;

		// Token: 0x04000E26 RID: 3622
		public ShaderParameter m_worldViewProjectionMatrixParameter;

		// Token: 0x04000E27 RID: 3623
		public ShaderParameter m_textureParameter;

		// Token: 0x04000E28 RID: 3624
		public ShaderParameter m_samplerStateParameter;

		// Token: 0x04000E29 RID: 3625
		public ShaderParameter m_materialColorParameter;

		// Token: 0x04000E2A RID: 3626
		public ShaderParameter m_emissionColorParameter;

		// Token: 0x04000E2B RID: 3627
		public ShaderParameter m_alphaThresholdParameter;

		// Token: 0x04000E2C RID: 3628
		public ShaderParameter m_ambientLightColorParameter;

		// Token: 0x04000E2D RID: 3629
		public ShaderParameter m_diffuseLightColor1Parameter;

		// Token: 0x04000E2E RID: 3630
		public ShaderParameter m_directionToLight1Parameter;

		// Token: 0x04000E2F RID: 3631
		public ShaderParameter m_diffuseLightColor2Parameter;

		// Token: 0x04000E30 RID: 3632
		public ShaderParameter m_directionToLight2Parameter;

		// Token: 0x04000E31 RID: 3633
		public ShaderParameter m_fogColorParameter;

		// Token: 0x04000E32 RID: 3634
		public ShaderParameter m_fogStartInvLengthParameter;

		// Token: 0x04000E33 RID: 3635
		public ShaderParameter m_fogYMultiplierParameter;

		// Token: 0x04000E34 RID: 3636
		public ShaderParameter m_worldUpParameter;

		// Token: 0x04000E35 RID: 3637
		public int m_instancesCount;

		// Token: 0x04000E36 RID: 3638
		public readonly ShaderTransforms Transforms;
	}
}
