using System;
using System.Collections.Generic;
using Game;
using Engine.Graphics;


namespace Game
{


public static class ShaderBase
{
public static Dictionary<string, Texture2D> Images = new Dictionary<string, Texture2D>();

public static SamplerState m_samplerState = new SamplerState
		{
			AddressModeU = TextureAddressMode.Clamp,
			AddressModeV = TextureAddressMode.Clamp,
			FilterMode = TextureFilterMode.Point,
			MaxLod = 0f
		};

		// Token: 0x04001AFA RID: 6906
public static SamplerState m_samplerStateMips = new SamplerState
		{
			AddressModeU = TextureAddressMode.Clamp,
			AddressModeV = TextureAddressMode.Clamp,
			FilterMode = TextureFilterMode.PointMipLinear,
			MaxLod = 4f
		};

public static void PrepareUniforms(Shader shader) {
foreach (var Image in ShaderBase.Images) {
try {
shader.GetParameter(Image.Key).SetValue(Image.Value);
shader.GetParameter(Image.Key+"Sampler").SetValue(SettingsManager.TerrainMipmapsEnabled ? m_samplerStateMips : m_samplerState);
} catch {}
}
}




}




}