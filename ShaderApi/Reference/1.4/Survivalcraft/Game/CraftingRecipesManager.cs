using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000366 RID: 870
	public static class CraftingRecipesManager
	{
		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06001969 RID: 6505 RVA: 0x000C8D02 File Offset: 0x000C6F02
		public static ReadOnlyList<CraftingRecipe> Recipes
		{
			get
			{
				return new ReadOnlyList<CraftingRecipe>(CraftingRecipesManager.m_recipes);
			}
		}

		// Token: 0x0600196A RID: 6506 RVA: 0x000C8D10 File Offset: 0x000C6F10
		public static void Initialize()
		{
			CraftingRecipesManager.m_recipes.Clear();
			XElement item = null;
			foreach (ModEntity modEntity in ModsManager.ModList)
			{
				modEntity.LoadCr(ref item);
			}
			CraftingRecipesManager.LoadData(item);
			foreach (Block block in BlocksManager.Blocks)
			{
				CraftingRecipesManager.m_recipes.AddRange(block.GetProceduralCraftingRecipes());
			}
			CraftingRecipesManager.m_recipes.Sort(delegate(CraftingRecipe r1, CraftingRecipe r2)
			{
				int y = r1.Ingredients.Count((string s) => !string.IsNullOrEmpty(s));
				int x = r2.Ingredients.Count((string s) => !string.IsNullOrEmpty(s));
				return Comparer<int>.Default.Compare(x, y);
			});
		}

		// Token: 0x0600196B RID: 6507 RVA: 0x000C8DC8 File Offset: 0x000C6FC8
		public static void LoadData(XElement item)
		{
			XAttribute xattribute;
			if (!ModsManager.HasAttribute(item, (string name) => name == "Result", out xattribute))
			{
				foreach (XElement item2 in item.Elements())
				{
					CraftingRecipesManager.LoadData(item2);
				}
				return;
			}
			bool flag = false;
			ModsManager.HookAction("OnCraftingRecipeDecode", delegate(ModLoader modLoader)
			{
				modLoader.OnCraftingRecipeDecode(CraftingRecipesManager.m_recipes, item, out flag);
				return flag;
			});
			if (!flag)
			{
				CraftingRecipe item3 = CraftingRecipesManager.DecodeElementToCraftingRecipe(item, 3);
				CraftingRecipesManager.m_recipes.Add(item3);
			}
		}

		// Token: 0x0600196C RID: 6508 RVA: 0x000C8E90 File Offset: 0x000C7090
		public static CraftingRecipe DecodeElementToCraftingRecipe(XElement item, int HorizontalLen = 3)
		{
			CraftingRecipe craftingRecipe = new CraftingRecipe();
			string attributeValue = XmlUtils.GetAttributeValue<string>(item, "Result");
			string text = XmlUtils.GetAttributeValue<string>(item, "Description");
			string text2;
			if (text.StartsWith("[") && text.EndsWith("]") && LanguageControl.TryGetBlock(attributeValue, "CRDescription:" + text.Substring(1, text.Length - 2), out text2))
			{
				text = text2;
			}
			craftingRecipe.ResultValue = CraftingRecipesManager.DecodeResult(attributeValue);
			craftingRecipe.ResultCount = XmlUtils.GetAttributeValue<int>(item, "ResultCount");
			string attributeValue2 = XmlUtils.GetAttributeValue<string>(item, "Remains", string.Empty);
			if (!string.IsNullOrEmpty(attributeValue2))
			{
				craftingRecipe.RemainsValue = CraftingRecipesManager.DecodeResult(attributeValue2);
				craftingRecipe.RemainsCount = XmlUtils.GetAttributeValue<int>(item, "RemainsCount");
			}
			craftingRecipe.RequiredHeatLevel = XmlUtils.GetAttributeValue<float>(item, "RequiredHeatLevel");
			craftingRecipe.RequiredPlayerLevel = XmlUtils.GetAttributeValue<float>(item, "RequiredPlayerLevel", 1f);
			craftingRecipe.Description = text;
			craftingRecipe.Message = XmlUtils.GetAttributeValue<string>(item, "Message", null);
			if (craftingRecipe.ResultCount > BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.ResultValue)].GetMaxStacking(craftingRecipe.ResultValue))
			{
				throw new InvalidOperationException("In recipe for \"" + attributeValue + "\" ResultCount is larger than max stacking of result block.");
			}
			if (craftingRecipe.RemainsValue != 0 && craftingRecipe.RemainsCount > BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.RemainsValue)].GetMaxStacking(craftingRecipe.RemainsValue))
			{
				throw new InvalidOperationException("In Recipe for \"" + attributeValue2 + "\" RemainsCount is larger than max stacking of remains block.");
			}
			Dictionary<char, string> dictionary = new Dictionary<char, string>();
			foreach (XAttribute xattribute in from a in item.Attributes()
			where a.Name.LocalName.Length == 1 && char.IsLower(a.Name.LocalName[0])
			select a)
			{
				string craftingId;
				int? num;
				CraftingRecipesManager.DecodeIngredient(xattribute.Value, out craftingId, out num);
				if (BlocksManager.FindBlocksByCraftingId(craftingId).Length == 0)
				{
					throw new InvalidOperationException("Block with craftingId \"" + xattribute.Value + "\" not found.");
				}
				if (num != null && (num.Value < 0 || num.Value > 262143))
				{
					throw new InvalidOperationException("Data in recipe ingredient \"" + xattribute.Value + "\" must be between 0 and 0x3FFFF.");
				}
				dictionary.Add(xattribute.Name.LocalName[0], xattribute.Value);
			}
			string[] array = item.Value.Trim().Split(new string[]
			{
				"\n"
			}, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = array[i].IndexOf('"');
				int num3 = array[i].LastIndexOf('"');
				if (num2 < 0 || num3 < 0 || num3 <= num2)
				{
					throw new InvalidOperationException("Invalid recipe line.");
				}
				string text3 = array[i].Substring(num2 + 1, num3 - num2 - 1);
				for (int j = 0; j < text3.Length; j++)
				{
					char c = text3[j];
					if (char.IsLower(c))
					{
						string text4 = dictionary[c];
						craftingRecipe.Ingredients[j + i * HorizontalLen] = text4;
					}
				}
			}
			return craftingRecipe;
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x000C91E0 File Offset: 0x000C73E0
		public static CraftingRecipe FindMatchingRecipe(SubsystemTerrain terrain, string[] ingredients, float heatLevel, float playerLevel)
		{
			CraftingRecipe craftingRecipe = null;
			Block[] blocks = BlocksManager.Blocks;
			for (int i = 0; i < blocks.Length; i++)
			{
				CraftingRecipe adHocCraftingRecipe = blocks[i].GetAdHocCraftingRecipe(terrain, ingredients, heatLevel, playerLevel);
				if (adHocCraftingRecipe != null && CraftingRecipesManager.MatchRecipe(adHocCraftingRecipe.Ingredients, ingredients))
				{
					craftingRecipe = adHocCraftingRecipe;
					break;
				}
			}
			if (craftingRecipe == null)
			{
				foreach (CraftingRecipe craftingRecipe2 in CraftingRecipesManager.Recipes)
				{
					if (CraftingRecipesManager.MatchRecipe(craftingRecipe2.Ingredients, ingredients))
					{
						craftingRecipe = craftingRecipe2;
						break;
					}
				}
			}
			if (craftingRecipe != null)
			{
				if (heatLevel < craftingRecipe.RequiredHeatLevel)
				{
					CraftingRecipe craftingRecipe3;
					if (heatLevel > 0f)
					{
						(craftingRecipe3 = new CraftingRecipe()).Message = LanguageControl.Get(CraftingRecipesManager.fName, 1);
					}
					else
					{
						(craftingRecipe3 = new CraftingRecipe()).Message = LanguageControl.Get(CraftingRecipesManager.fName, 0);
					}
					craftingRecipe = craftingRecipe3;
				}
				else if (playerLevel < craftingRecipe.RequiredPlayerLevel)
				{
					CraftingRecipe craftingRecipe4;
					if (craftingRecipe.RequiredHeatLevel > 0f)
					{
						(craftingRecipe4 = new CraftingRecipe()).Message = string.Format(LanguageControl.Get(CraftingRecipesManager.fName, 3), craftingRecipe.RequiredPlayerLevel);
					}
					else
					{
						(craftingRecipe4 = new CraftingRecipe()).Message = string.Format(LanguageControl.Get(CraftingRecipesManager.fName, 2), craftingRecipe.RequiredPlayerLevel);
					}
					craftingRecipe = craftingRecipe4;
				}
			}
			return craftingRecipe;
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x000C9330 File Offset: 0x000C7530
		public static int DecodeResult(string result)
		{
			bool flag2 = false;
			int result2 = 0;
			ModsManager.HookAction("DecodeResult", delegate(ModLoader modLoader)
			{
				result2 = modLoader.DecodeResult(result, out flag2);
				return flag2;
			});
			if (flag2)
			{
				return result2;
			}
			if (!string.IsNullOrEmpty(result))
			{
				string[] array = result.Split(new char[]
				{
					':'
				}, StringSplitOptions.None);
				return Terrain.MakeBlockValue(BlocksManager.FindBlockByTypeName(array[0], true).BlockIndex, 0, (array.Length == 2) ? int.Parse(array[1], CultureInfo.InvariantCulture) : 0);
			}
			return 0;
		}

		// Token: 0x0600196F RID: 6511 RVA: 0x000C93D0 File Offset: 0x000C75D0
		public static void DecodeIngredient(string ingredient, out string craftingId, out int? data)
		{
			bool flag2 = false;
			string craftingId_R = string.Empty;
			int? data_R = null;
			ModsManager.HookAction("DecodeIngredient", delegate(ModLoader modLoader)
			{
				modLoader.DecodeIngredient(ingredient, out craftingId_R, out data_R, out flag2);
				return flag2;
			});
			if (flag2)
			{
				craftingId = craftingId_R;
				data = data_R;
				return;
			}
			string[] array = ingredient.Split(new char[]
			{
				':'
			}, StringSplitOptions.None);
			craftingId = array[0];
			data = ((array.Length >= 2) ? new int?(int.Parse(array[1], CultureInfo.InvariantCulture)) : null);
		}

		// Token: 0x06001970 RID: 6512 RVA: 0x000C9484 File Offset: 0x000C7684
		public static bool MatchRecipe(string[] requiredIngredients, string[] actualIngredients)
		{
			bool flag2 = false;
			bool result = false;
			ModsManager.HookAction("MatchRecipe", delegate(ModLoader modLoader)
			{
				result = modLoader.MatchRecipe(requiredIngredients, actualIngredients, out flag2);
				return flag2;
			});
			if (flag2)
			{
				return result;
			}
			if (actualIngredients.Length > 9)
			{
				return false;
			}
			string[] array = new string[9];
			for (int i = 0; i < 2; i++)
			{
				for (int j = -3; j <= 3; j++)
				{
					for (int k = -3; k <= 3; k++)
					{
						bool flip = i != 0;
						if (CraftingRecipesManager.TransformRecipe(array, requiredIngredients, k, j, flip))
						{
							bool flag = true;
							for (int l = 0; l < 9; l++)
							{
								if (l == actualIngredients.Length || !CraftingRecipesManager.CompareIngredients(array[l], actualIngredients[l]))
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06001971 RID: 6513 RVA: 0x000C9578 File Offset: 0x000C7778
		public static bool TransformRecipe(string[] transformedIngredients, string[] ingredients, int shiftX, int shiftY, bool flip)
		{
			for (int i = 0; i < 9; i++)
			{
				transformedIngredients[i] = null;
			}
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					int num = (flip ? (3 - k - 1) : k) + shiftX;
					int num2 = j + shiftY;
					string text = ingredients[k + j * 3];
					if (num >= 0 && num2 >= 0 && num < 3 && num2 < 3)
					{
						transformedIngredients[num + num2 * 3] = text;
					}
					else if (!string.IsNullOrEmpty(text))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x000C95F8 File Offset: 0x000C77F8
		public static bool CompareIngredients(string requiredIngredient, string actualIngredient)
		{
			if (requiredIngredient == null)
			{
				return actualIngredient == null;
			}
			if (actualIngredient == null)
			{
				return requiredIngredient == null;
			}
			string a;
			int? num;
			CraftingRecipesManager.DecodeIngredient(requiredIngredient, out a, out num);
			string b;
			int? num2;
			CraftingRecipesManager.DecodeIngredient(actualIngredient, out b, out num2);
			if (num2 == null)
			{
				throw new InvalidOperationException("Actual ingredient data not specified.");
			}
			return a == b && (num == null || num.Value == num2.Value);
		}

		// Token: 0x04001182 RID: 4482
		public static List<CraftingRecipe> m_recipes = new List<CraftingRecipe>();

		// Token: 0x04001183 RID: 4483
		public static string fName = "CraftingRecipesManager";
	}
}
