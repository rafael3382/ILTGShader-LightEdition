using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;
using ShaderApi;

namespace Game
{
	// Token: 0x02000282 RID: 642
	public class NModelShadowShader : Shader
	{
        public int m_instancesCount;
        public readonly ShaderTransforms Transforms;
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
		public NModelShadowShader(bool useAlphaThreshold, int maxInstancesCount = 1) : base(ShaderReader.GetShader("ModelShadow.vsh"), ShaderReader.GetShader("ModelShadow.psh"), ModelShader.PrepareShaderMacros(useAlphaThreshold, maxInstancesCount))
		{
			
			this.m_worldViewProjectionMatrixParameter = base.GetParameter("u_worldViewProjectionMatrix", false);
			this.m_worldMatrixParameter = base.GetParameter("u_worldMatrix", false);

			this.Transforms = new ShaderTransforms(maxInstancesCount);
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x00069BB8 File Offset: 0x00067DB8
		public override void PrepareForDrawingOverride()
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
        public ShaderParameter m_worldViewProjectionMatrixParameter;
		
	}
}
