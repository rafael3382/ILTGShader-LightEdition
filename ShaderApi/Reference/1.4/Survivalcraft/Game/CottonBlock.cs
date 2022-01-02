using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000047 RID: 71
	public class CottonBlock : CrossBlock
	{
		// Token: 0x0600017B RID: 379 RVA: 0x0000B22A File Offset: 0x0000942A
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(204, 0, CottonBlock.SetIsWild(CottonBlock.SetSize(0, 2), true));
			yield return Terrain.MakeBlockValue(204, 0, CottonBlock.SetIsWild(CottonBlock.SetSize(0, 1), false));
			yield return Terrain.MakeBlockValue(204, 0, CottonBlock.SetIsWild(CottonBlock.SetSize(0, 2), false));
			yield break;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000B233 File Offset: 0x00009433
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			if (!CottonBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				return "棉花";
			}
			return "野生棉花";
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000B250 File Offset: 0x00009450
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (CottonBlock.GetSize(data) == 2)
			{
				BlockDropValue item = new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(173, 0, 6),
					Count = this.Random.Int(1, 2)
				};
				dropValues.Add(item);
				if (!CottonBlock.GetIsWild(data))
				{
					int num = this.Random.Int(1, 2);
					for (int i = 0; i < num; i++)
					{
						item = new BlockDropValue
						{
							Value = Terrain.MakeBlockValue(205, 0, 0),
							Count = 1
						};
						dropValues.Add(item);
					}
					if (this.Random.Bool(0.5f))
					{
						item = new BlockDropValue
						{
							Value = Terrain.MakeBlockValue(248),
							Count = 1
						};
						dropValues.Add(item);
					}
				}
			}
			showDebris = true;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000B340 File Offset: 0x00009540
		public override int GetFaceTextureSlot(int face, int value)
		{
			int size = CottonBlock.GetSize(Terrain.ExtractData(value));
			if (size == 0)
			{
				return 11;
			}
			if (size != 1)
			{
				return 30;
			}
			return 29;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000B36C File Offset: 0x0000956C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if (CottonBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				color *= BlockColorsMap.GrassColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity);
				BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
				return;
			}
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000B3C8 File Offset: 0x000095C8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			if (CottonBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				Color color = BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x, y, z);
				generator.GenerateCrossfaceVertices(this, value, x, y, z, color, this.GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
				return;
			}
			generator.GenerateCrossfaceVertices(this, value, x, y, z, Color.White, this.GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000B438 File Offset: 0x00009638
		public override int GetShadowStrength(int value)
		{
			int size = CottonBlock.GetSize(Terrain.ExtractData(value));
			return 2 + size * 2;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000B458 File Offset: 0x00009658
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			if (CottonBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				Color color = BlockColorsMap.GrassColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(4, value));
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000B4D9 File Offset: 0x000096D9
		public static int GetSize(int data)
		{
			return data & 3;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000B4DE File Offset: 0x000096DE
		public static int SetSize(int data, int size)
		{
			size = MathUtils.Clamp(size, 0, 2);
			return (data & -4) | (size & 3);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000B4F2 File Offset: 0x000096F2
		public static bool GetIsWild(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000B4FA File Offset: 0x000096FA
		public static int SetIsWild(int data, bool isWild)
		{
			if (!isWild)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x040000CA RID: 202
		public const int Index = 204;
	}
}
