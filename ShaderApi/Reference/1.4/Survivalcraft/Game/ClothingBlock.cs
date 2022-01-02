using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000368 RID: 872
	public class ClothingBlock : Block
	{
		// Token: 0x0600197D RID: 6525 RVA: 0x000C9868 File Offset: 0x000C7A68
		public void LoadClothingData(XElement item)
		{
			if (item.Name.LocalName == "ClothingData")
			{
				int num;
				int.TryParse(item.Attribute("Index").Value, out num);
				XAttribute xattribute = item.Attribute("Description");
				string text = (xattribute != null) ? xattribute.Value : null;
				XAttribute xattribute2 = item.Attribute("DisplayName");
				string text2 = (xattribute2 != null) ? xattribute2.Value : null;
				string text3;
				if (text != null && text.StartsWith("[") && text.EndsWith("]") && LanguageControl.TryGetBlock(string.Format("{0}:{1}", base.GetType().Name, num), "Description", out text3))
				{
					text = text3;
				}
				string text4;
				if (text2 != null && text2.StartsWith("[") && text2.EndsWith("]") && LanguageControl.TryGetBlock(string.Format("{0}:{1}", base.GetType().Name, num), "DisplayName", out text4))
				{
					text2 = text4;
				}
				ClothingData value = new ClothingData
				{
					Index = num,
					DisplayIndex = this.num,
					DisplayName = text2,
					Slot = XmlUtils.GetAttributeValue<ClothingSlot>(item, "Slot"),
					ArmorProtection = XmlUtils.GetAttributeValue<float>(item, "ArmorProtection"),
					Sturdiness = XmlUtils.GetAttributeValue<float>(item, "Sturdiness"),
					Insulation = XmlUtils.GetAttributeValue<float>(item, "Insulation"),
					MovementSpeedFactor = XmlUtils.GetAttributeValue<float>(item, "MovementSpeedFactor"),
					SteedMovementSpeedFactor = XmlUtils.GetAttributeValue<float>(item, "SteedMovementSpeedFactor"),
					DensityModifier = XmlUtils.GetAttributeValue<float>(item, "DensityModifier"),
					IsOuter = XmlUtils.GetAttributeValue<bool>(item, "IsOuter"),
					CanBeDyed = XmlUtils.GetAttributeValue<bool>(item, "CanBeDyed"),
					Layer = XmlUtils.GetAttributeValue<int>(item, "Layer"),
					PlayerLevelRequired = XmlUtils.GetAttributeValue<int>(item, "PlayerLevelRequired"),
					Texture = ContentManager.Get<Texture2D>(XmlUtils.GetAttributeValue<string>(item, "TextureName"), null),
					ImpactSoundsFolder = XmlUtils.GetAttributeValue<string>(item, "ImpactSoundsFolder"),
					Description = text
				};
				if (num >= this.m_clothingData.Count)
				{
					this.m_clothingData.Count = num + 1;
				}
				this.m_clothingData[num] = value;
			}
			this.num++;
			foreach (XElement item2 in item.Elements())
			{
				this.LoadClothingData(item2);
			}
		}

		// Token: 0x0600197E RID: 6526 RVA: 0x000C9B04 File Offset: 0x000C7D04
		public override void Initialize()
		{
			this.num = 0;
			XElement xElement = null;
			ModsManager.ModListAllDo(delegate(ModEntity modEntity)
			{
				modEntity.LoadClo(this, ref xElement);
			});
			this.LoadClothingData(xElement);
			Model playerModel = CharacterSkinsManager.GetPlayerModel(PlayerClass.Male);
			Matrix[] array = new Matrix[playerModel.Bones.Count];
			playerModel.CopyAbsoluteBoneTransformsTo(array);
			int index = playerModel.FindBone("Hand1", true).Index;
			int index2 = playerModel.FindBone("Hand2", true).Index;
			array[index] = Matrix.CreateRotationY(0.1f) * array[index];
			array[index2] = Matrix.CreateRotationY(-0.1f) * array[index2];
			this.m_innerMesh = new BlockMesh();
			foreach (ModelMesh modelMesh in playerModel.Meshes)
			{
				Matrix matrix = array[modelMesh.ParentBone.Index];
				foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
				{
					Color color = Color.White * 0.8f;
					color.A = byte.MaxValue;
					this.m_innerMesh.AppendModelMeshPart(meshPart, matrix, false, false, false, false, Color.White);
					this.m_innerMesh.AppendModelMeshPart(meshPart, matrix, false, true, false, true, color);
				}
			}
			Model outerClothingModel = CharacterSkinsManager.GetOuterClothingModel(PlayerClass.Male);
			Matrix[] array2 = new Matrix[outerClothingModel.Bones.Count];
			outerClothingModel.CopyAbsoluteBoneTransformsTo(array2);
			int index3 = outerClothingModel.FindBone("Leg1", true).Index;
			int index4 = outerClothingModel.FindBone("Leg2", true).Index;
			array2[index3] = Matrix.CreateTranslation(-0.02f, 0f, 0f) * array2[index3];
			array2[index4] = Matrix.CreateTranslation(0.02f, 0f, 0f) * array2[index4];
			this.m_outerMesh = new BlockMesh();
			foreach (ModelMesh modelMesh2 in outerClothingModel.Meshes)
			{
				Matrix matrix2 = array2[modelMesh2.ParentBone.Index];
				foreach (ModelMeshPart meshPart2 in modelMesh2.MeshParts)
				{
					Color color2 = Color.White * 0.8f;
					color2.A = byte.MaxValue;
					this.m_outerMesh.AppendModelMeshPart(meshPart2, matrix2, false, false, false, false, Color.White);
					this.m_outerMesh.AppendModelMeshPart(meshPart2, matrix2, false, true, false, true, color2);
				}
			}
			base.Initialize();
		}

		// Token: 0x0600197F RID: 6527 RVA: 0x000C9E58 File Offset: 0x000C8058
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			ClothingData clothingData = this.GetClothingData(value);
			int clothingColor = ClothingBlock.GetClothingColor(data);
			string displayName = clothingData.DisplayName;
			if (clothingColor != 0)
			{
				return SubsystemPalette.GetName(subsystemTerrain, new int?(clothingColor), displayName);
			}
			return displayName;
		}

		// Token: 0x06001980 RID: 6528 RVA: 0x000C9E92 File Offset: 0x000C8092
		public override string GetDescription(int value)
		{
			Terrain.ExtractData(value);
			return this.GetClothingData(value).Description;
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x000C9EA7 File Offset: 0x000C80A7
		public override string GetCategory(int value)
		{
			if (ClothingBlock.GetClothingColor(Terrain.ExtractData(value)) == 0)
			{
				return base.GetCategory(value);
			}
			return "Dyed";
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x000C9EC3 File Offset: 0x000C80C3
		public override int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 8 & 15;
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x000C9ED0 File Offset: 0x000C80D0
		public override int GetDisplayOrder(int value)
		{
			return this.GetClothingData(value).DisplayIndex;
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x000C9EE0 File Offset: 0x000C80E0
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num = ((num & -3841) | (damage & 15) << 8);
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x000C9F0A File Offset: 0x000C810A
		public override bool CanWear(int value)
		{
			return true;
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x000C9F10 File Offset: 0x000C8110
		public override ClothingData GetClothingData(int value)
		{
			int clothingIndex = ClothingBlock.GetClothingIndex(Terrain.ExtractData(value));
			return this.m_clothingData[clothingIndex];
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x000C9F35 File Offset: 0x000C8135
		public override IEnumerable<int> GetCreativeValues()
		{
			IEnumerable<ClothingData> enumerable = from cd in this.m_clothingData
			orderby cd.DisplayIndex
			select cd;
			foreach (ClothingData clothingData in enumerable)
			{
				if (clothingData != null)
				{
					int colorsCount = (!clothingData.CanBeDyed) ? 1 : 16;
					int num;
					for (int color = 0; color < colorsCount; color = num)
					{
						int data = ClothingBlock.SetClothingColor(ClothingBlock.SetClothingIndex(0, clothingData.Index), color);
						yield return Terrain.MakeBlockValue(203, 0, data);
						num = color + 1;
					}
					clothingData = null;
				}
			}
			IEnumerator<ClothingData> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x000C9F48 File Offset: 0x000C8148
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain terrain, string[] ingredients, float heatLevel, float playerLevel)
		{
			if (heatLevel < 1f)
			{
				return null;
			}
			List<string> list = (from i in ingredients
			where !string.IsNullOrEmpty(i)
			select i).ToList<string>();
			if (list.Count == 2)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (string ingredient in list)
				{
					string a;
					int? num4;
					CraftingRecipesManager.DecodeIngredient(ingredient, out a, out num4);
					if (a == BlocksManager.Blocks[203].CraftingId)
					{
						num3 = Terrain.MakeBlockValue(203, 0, (num4 != null) ? num4.Value : 0);
					}
					else if (a == BlocksManager.Blocks[129].CraftingId)
					{
						num = Terrain.MakeBlockValue(129, 0, (num4 != null) ? num4.Value : 0);
					}
					else if (a == BlocksManager.Blocks[128].CraftingId)
					{
						num2 = Terrain.MakeBlockValue(128, 0, (num4 != null) ? num4.Value : 0);
					}
				}
				if (num != 0 && num3 != 0)
				{
					int num5 = Terrain.ExtractData(num3);
					int clothingColor = ClothingBlock.GetClothingColor(num5);
					int clothingIndex = ClothingBlock.GetClothingIndex(num5);
					bool canBeDyed = this.GetClothingData(num5).CanBeDyed;
					int damage = BlocksManager.Blocks[203].GetDamage(num3);
					int color = PaintBucketBlock.GetColor(Terrain.ExtractData(num));
					int damage2 = BlocksManager.Blocks[129].GetDamage(num);
					Block block = BlocksManager.Blocks[129];
					Block block2 = BlocksManager.Blocks[203];
					if (!canBeDyed)
					{
						return null;
					}
					int num6 = PaintBucketBlock.CombineColors(clothingColor, color);
					if (num6 != clothingColor)
					{
						return new CraftingRecipe
						{
							ResultCount = 1,
							ResultValue = block2.SetDamage(Terrain.MakeBlockValue(203, 0, ClothingBlock.SetClothingIndex(ClothingBlock.SetClothingColor(0, num6), clothingIndex)), damage),
							RemainsCount = 1,
							RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(129, 0, color), damage2 + MathUtils.Max(block.Durability / 4, 1)),
							RequiredHeatLevel = 1f,
							Description = LanguageControl.Get(new string[]
							{
								"BlocksManager",
								"Dyed"
							}) + " " + SubsystemPalette.GetName(terrain, new int?(color), null),
							Ingredients = (string[])ingredients.Clone()
						};
					}
				}
				if (num2 != 0 && num3 != 0)
				{
					int num7 = Terrain.ExtractData(num3);
					int clothingColor2 = ClothingBlock.GetClothingColor(num7);
					int clothingIndex2 = ClothingBlock.GetClothingIndex(num7);
					bool canBeDyed2 = this.GetClothingData(num7).CanBeDyed;
					int damage3 = BlocksManager.Blocks[203].GetDamage(num3);
					int damage4 = BlocksManager.Blocks[128].GetDamage(num2);
					Block block3 = BlocksManager.Blocks[128];
					Block block4 = BlocksManager.Blocks[203];
					if (!canBeDyed2)
					{
						return null;
					}
					if (clothingColor2 != 0)
					{
						return new CraftingRecipe
						{
							ResultCount = 1,
							ResultValue = block4.SetDamage(Terrain.MakeBlockValue(203, 0, ClothingBlock.SetClothingIndex(ClothingBlock.SetClothingColor(0, 0), clothingIndex2)), damage3),
							RemainsCount = 1,
							RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(128, 0, 0), damage4 + MathUtils.Max(block3.Durability / 4, 1)),
							RequiredHeatLevel = 1f,
							Description = LanguageControl.Get(new string[]
							{
								"BlocksManager",
								"Not Dyed"
							}) + " " + LanguageControl.Get(new string[]
							{
								"BlocksManager",
								"Clothes"
							}),
							Ingredients = (string[])ingredients.Clone()
						};
					}
				}
			}
			return null;
		}

		// Token: 0x06001989 RID: 6537 RVA: 0x000CA344 File Offset: 0x000C8544
		public static int GetClothingIndex(int data)
		{
			return data & 255;
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x000CA34D File Offset: 0x000C854D
		public static int SetClothingIndex(int data, int clothingIndex)
		{
			return (data & -256) | (clothingIndex & 255);
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x000CA35E File Offset: 0x000C855E
		public static int GetClothingColor(int data)
		{
			return data >> 12 & 15;
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x000CA367 File Offset: 0x000C8567
		public static int SetClothingColor(int data, int color)
		{
			return (data & -61441) | (color & 15) << 12;
		}

		// Token: 0x0600198D RID: 6541 RVA: 0x000CA378 File Offset: 0x000C8578
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x000CA37C File Offset: 0x000C857C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int clothingColor = ClothingBlock.GetClothingColor(Terrain.ExtractData(value));
			ClothingData clothingData = this.GetClothingData(value);
			Matrix matrix2 = ClothingBlock.m_slotTransforms[(int)clothingData.Slot] * Matrix.CreateScale(size) * matrix;
			if (clothingData.IsOuter)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_outerMesh, clothingData.Texture, color * SubsystemPalette.GetFabricColor(environmentData, new int?(clothingColor)), 1f, ref matrix2, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_innerMesh, clothingData.Texture, color * SubsystemPalette.GetFabricColor(environmentData, new int?(clothingColor)), 1f, ref matrix2, environmentData);
		}

		// Token: 0x04001187 RID: 4487
		public const int Index = 203;

		// Token: 0x04001188 RID: 4488
		public DynamicArray<ClothingData> m_clothingData = new DynamicArray<ClothingData>();

		// Token: 0x04001189 RID: 4489
		public BlockMesh m_innerMesh;

		// Token: 0x0400118A RID: 4490
		public int num;

		// Token: 0x0400118B RID: 4491
		public BlockMesh m_outerMesh;

		// Token: 0x0400118C RID: 4492
		public static Matrix[] m_slotTransforms = new Matrix[]
		{
			Matrix.CreateTranslation(0f, -1.5f, 0f) * Matrix.CreateScale(2.7f),
			Matrix.CreateTranslation(0f, -1.1f, 0f) * Matrix.CreateScale(2.7f),
			Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateScale(2.7f),
			Matrix.CreateTranslation(0f, -0.1f, 0f) * Matrix.CreateScale(2.7f)
		};
	}
}
