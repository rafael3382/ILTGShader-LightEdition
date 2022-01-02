using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000133 RID: 307
	public class SkyPrimitiveRender
	{
		// Token: 0x060005C5 RID: 1477 RVA: 0x00020CF0 File Offset: 0x0001EEF0
		public void Textured_FlushWithCurrentStateAndShader(BaseTexturedBatch baseTexturedBatch, Shader shader, bool clearAfterFlush = true)
		{
			int num = 0;
			int num2;
			for (int i = baseTexturedBatch.TriangleIndices.Count; i > 0; i -= num2)
			{
				num2 = MathUtils.Min(i, 196605);
				Display.DrawUserIndexed<VertexPositionColorTexture>(PrimitiveType.TriangleList, shader, VertexPositionColorTexture.VertexDeclaration, baseTexturedBatch.TriangleVertices.Array, 0, baseTexturedBatch.TriangleVertices.Count, baseTexturedBatch.TriangleIndices.Array, num, num2);
				num += num2;
			}
			if (clearAfterFlush)
			{
				baseTexturedBatch.Clear();
			}
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x00020D60 File Offset: 0x0001EF60
		public void Flush(PrimitivesRenderer3D primitiveRend, Matrix matrix, bool clearAfterFlush = true, int maxLayer = 2147483647)
		{
			if (primitiveRend.m_sortNeeded)
			{
				primitiveRend.m_sortNeeded = false;
				primitiveRend.m_allBatches.Sort(delegate(BaseBatch b1, BaseBatch b2)
				{
					if (b1.Layer < b2.Layer)
					{
						return -1;
					}
					if (b1.Layer <= b2.Layer)
					{
						return 0;
					}
					return 1;
				});
			}
			foreach (BaseBatch baseBatch in primitiveRend.m_allBatches)
			{
				if (baseBatch.Layer > maxLayer)
				{
					break;
				}
				if (!baseBatch.IsEmpty() && baseBatch is TexturedBatch3D)
				{
					BaseTexturedBatch baseTexturedBatch = (BaseTexturedBatch)baseBatch;
					Display.DepthStencilState = baseTexturedBatch.DepthStencilState;
					Display.RasterizerState = baseTexturedBatch.RasterizerState;
					Display.BlendState = baseTexturedBatch.BlendState;
					if (baseTexturedBatch.UseAlphaTest)
					{
						this.ShaderAlphaTest.Texture = baseTexturedBatch.Texture;
						this.ShaderAlphaTest.SamplerState = baseTexturedBatch.SamplerState;
						this.ShaderAlphaTest.Transforms.World[0] = matrix;
						this.ShaderAlphaTest.AlphaThreshold = 0f;
						ModsManager.HookAction("SetShaderParameter", delegate(ModLoader modLoader)
						{
							modLoader.SetShaderParameter(this.ShaderAlphaTest, this.Camera);
							return true;
						});
						baseTexturedBatch.FlushWithCurrentStateAndShader(this.ShaderAlphaTest, clearAfterFlush);
					}
					else
					{
						this.Shader.Texture = baseTexturedBatch.Texture;
						this.Shader.SamplerState = baseTexturedBatch.SamplerState;
						this.Shader.Transforms.World[0] = matrix;
						ModsManager.HookAction("SetShaderParameter", delegate(ModLoader modLoader)
						{
							modLoader.SetShaderParameter(this.Shader, this.Camera);
							return true;
						});
						baseTexturedBatch.FlushWithCurrentStateAndShader(this.Shader, clearAfterFlush);
					}
				}
				else
				{
					baseBatch.Flush(matrix, true);
				}
			}
		}

		// Token: 0x0400028B RID: 651
		public SkyShader Shader;

		// Token: 0x0400028C RID: 652
		public SkyShader ShaderAlphaTest;

		// Token: 0x0400028D RID: 653
		public Camera Camera;
	}
}
