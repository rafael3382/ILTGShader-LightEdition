using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine;
using Engine.Graphics;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200005F RID: 95
	public class EggBlock : Block
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x0000CAFF File Offset: 0x0000ACFF
		public ReadOnlyList<EggBlock.EggType> EggTypes
		{
			get
			{
				return new ReadOnlyList<EggBlock.EggType>(this.m_eggTypes);
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000CB0C File Offset: 0x0000AD0C
		public override void Initialize()
		{
			this.m_eggTypes.Clear();
			DatabaseObjectType parameterSetType = DatabaseManager.GameDatabase.ParameterSetType;
			Guid eggParameterSetGuid = new Guid("300ff557-775f-4c7c-a88a-26655369f00b");
			IEnumerable<DatabaseObject> explicitNestingChildren = DatabaseManager.GameDatabase.Database.Root.GetExplicitNestingChildren(parameterSetType, false);
			Func<DatabaseObject, bool> <>9__0;
			Func<DatabaseObject, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((DatabaseObject o) => o.EffectiveInheritanceRoot.Guid == eggParameterSetGuid));
			}
			foreach (DatabaseObject databaseObject in explicitNestingChildren.Where(predicate))
			{
				int nestedValue = databaseObject.GetNestedValue<int>("EggTypeIndex");
				if (nestedValue >= 0)
				{
					string text = databaseObject.GetNestedValue<string>("DisplayName");
					if (text.StartsWith("[") && text.EndsWith("]"))
					{
						string[] array = text.Substring(1, text.Length - 2).Split(new string[]
						{
							":"
						}, StringSplitOptions.RemoveEmptyEntries);
						text = LanguageControl.GetDatabase("DisplayName", array[1]);
					}
					if (nestedValue >= this.m_eggTypes.Count)
					{
						this.m_eggTypes.Count = nestedValue + 1;
					}
					this.m_eggTypes[nestedValue] = new EggBlock.EggType
					{
						EggTypeIndex = nestedValue,
						ShowEgg = databaseObject.GetNestedValue<bool>("ShowEgg"),
						DisplayName = text,
						TemplateName = databaseObject.NestingParent.Name,
						NutritionalValue = databaseObject.GetNestedValue<float>("NutritionalValue"),
						Color = databaseObject.GetNestedValue<Color>("Color"),
						ScaleUV = databaseObject.GetNestedValue<Vector2>("ScaleUV"),
						SwapUV = databaseObject.GetNestedValue<bool>("SwapUV"),
						Scale = databaseObject.GetNestedValue<float>("Scale"),
						TextureSlot = databaseObject.GetNestedValue<int>("TextureSlot")
					};
				}
			}
			Model model = ContentManager.Get<Model>("Models/Egg", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Egg", true).ParentBone);
			foreach (EggBlock.EggType eggType in this.m_eggTypes)
			{
				if (eggType != null)
				{
					eggType.BlockMesh = new BlockMesh();
					eggType.BlockMesh.AppendModelMeshPart(model.FindMesh("Egg", true).MeshParts[0], boneAbsoluteTransform, false, false, false, false, eggType.Color);
					Matrix matrix = Matrix.Identity;
					if (eggType.SwapUV)
					{
						matrix.M11 = 0f;
						matrix.M12 = 1f;
						matrix.M21 = 1f;
						matrix.M22 = 0f;
					}
					matrix *= Matrix.CreateScale(0.0625f * eggType.ScaleUV.X, 0.0625f * eggType.ScaleUV.Y, 1f);
					matrix *= Matrix.CreateTranslation((float)(eggType.TextureSlot % 16) / 16f, (float)(eggType.TextureSlot / 16) / 16f, 0f);
					eggType.BlockMesh.TransformTextureCoordinates(matrix, -1);
				}
			}
			base.Initialize();
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000CE8C File Offset: 0x0000B08C
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			EggBlock.EggType eggType = this.GetEggType(Terrain.ExtractData(value));
			int data = Terrain.ExtractData(value);
			bool isCooked = EggBlock.GetIsCooked(data);
			bool isLaid = EggBlock.GetIsLaid(data);
			if (isCooked)
			{
				return LanguageControl.Get(EggBlock.fName, 1) + eggType.DisplayName;
			}
			if (!isLaid)
			{
				return eggType.DisplayName;
			}
			return LanguageControl.Get(EggBlock.fName, 2) + eggType.DisplayName;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000CEF3 File Offset: 0x0000B0F3
		public override string GetCategory(int value)
		{
			return "Spawner Eggs";
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000CEFC File Offset: 0x0000B0FC
		public override string GetDescription(int value)
		{
			EggBlock.EggType eggType = this.GetEggType(Terrain.ExtractData(value));
			string displayName = this.m_eggTypes[eggType.EggTypeIndex].DisplayName;
			return LanguageControl.Get(EggBlock.fName, 3) + displayName.Substring(0, displayName.Length - 1);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000CF4C File Offset: 0x0000B14C
		public override float GetNutritionalValue(int value)
		{
			EggBlock.EggType eggType = this.GetEggType(Terrain.ExtractData(value));
			if (!EggBlock.GetIsCooked(Terrain.ExtractData(value)))
			{
				return eggType.NutritionalValue;
			}
			return 1.5f * eggType.NutritionalValue;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000CF86 File Offset: 0x0000B186
		public override float GetSicknessProbability(int value)
		{
			if (!EggBlock.GetIsCooked(Terrain.ExtractData(value)))
			{
				return this.DefaultSicknessProbability;
			}
			return 0f;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000CFA1 File Offset: 0x0000B1A1
		public override int GetRotPeriod(int value)
		{
			if (this.GetNutritionalValue(value) > 0f)
			{
				return base.GetRotPeriod(value);
			}
			return 0;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000CFBA File Offset: 0x0000B1BA
		public override int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 16 & 1;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000CFC8 File Offset: 0x0000B1C8
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num = ((num & -65537) | (damage & 1) << 16);
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000CFF2 File Offset: 0x0000B1F2
		public override int GetDamageDestructionValue(int value)
		{
			return 246;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000CFF9 File Offset: 0x0000B1F9
		public override IEnumerable<int> GetCreativeValues()
		{
			foreach (EggBlock.EggType eggType in this.m_eggTypes)
			{
				if (eggType != null)
				{
					if (eggType.ShowEgg)
					{
						yield return Terrain.MakeBlockValue(118, 0, EggBlock.SetEggType(0, eggType.EggTypeIndex));
						if (eggType.NutritionalValue > 0f)
						{
							yield return Terrain.MakeBlockValue(118, 0, EggBlock.SetIsCooked(EggBlock.SetEggType(0, eggType.EggTypeIndex), true));
						}
					}
					eggType = null;
				}
			}
			DynamicArray<EggBlock.EggType>.Enumerator enumerator = default(DynamicArray<EggBlock.EggType>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000D009 File Offset: 0x0000B209
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			foreach (EggBlock.EggType eggType in this.EggTypes)
			{
				if (eggType != null)
				{
					if (eggType.NutritionalValue > 0f)
					{
						int num;
						for (int rot = 0; rot <= 1; rot = num)
						{
							CraftingRecipe craftingRecipe = new CraftingRecipe
							{
								ResultCount = 1,
								ResultValue = Terrain.MakeBlockValue(118, 0, EggBlock.SetEggType(EggBlock.SetIsCooked(0, true), eggType.EggTypeIndex)),
								RemainsCount = 1,
								RemainsValue = Terrain.MakeBlockValue(91),
								RequiredHeatLevel = 1f,
								Description = LanguageControl.Get(EggBlock.fName, 4)
							};
							int data = EggBlock.SetEggType(EggBlock.SetIsLaid(0, true), eggType.EggTypeIndex);
							int value = this.SetDamage(Terrain.MakeBlockValue(118, 0, data), rot);
							craftingRecipe.Ingredients[0] = "egg:" + Terrain.ExtractData(value).ToString(CultureInfo.InvariantCulture);
							craftingRecipe.Ingredients[1] = "waterbucket";
							yield return craftingRecipe;
							num = rot + 1;
						}
					}
					eggType = null;
				}
			}
			ReadOnlyList<EggBlock.EggType>.Enumerator enumerator = default(ReadOnlyList<EggBlock.EggType>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000D019 File Offset: 0x0000B219
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000D01C File Offset: 0x0000B21C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			EggBlock.EggType eggType = this.GetEggType(data);
			BlocksManager.DrawMeshBlock(primitivesRenderer, eggType.BlockMesh, color, eggType.Scale * size, ref matrix, environmentData);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000D054 File Offset: 0x0000B254
		public EggBlock.EggType GetEggType(int data)
		{
			int index = data >> 4 & 4095;
			return this.m_eggTypes[index];
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000D078 File Offset: 0x0000B278
		public EggBlock.EggType GetEggTypeByCreatureTemplateName(string templateName)
		{
			return this.m_eggTypes.FirstOrDefault((EggBlock.EggType e) => e.TemplateName == templateName);
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000D0A9 File Offset: 0x0000B2A9
		public static bool GetIsCooked(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000D0B1 File Offset: 0x0000B2B1
		public static int SetIsCooked(int data, bool isCooked)
		{
			if (!isCooked)
			{
				return data & -2;
			}
			return data | 1;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000D0BE File Offset: 0x0000B2BE
		public static bool GetIsLaid(int data)
		{
			return (data & 2) != 0;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000D0C6 File Offset: 0x0000B2C6
		public static int SetIsLaid(int data, bool isLaid)
		{
			if (!isLaid)
			{
				return data & -3;
			}
			return data | 2;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000D0D3 File Offset: 0x0000B2D3
		public static int SetEggType(int data, int eggTypeIndex)
		{
			data &= -65521;
			data |= (eggTypeIndex & 4095) << 4;
			return data;
		}

		// Token: 0x040000EB RID: 235
		public new static string fName = "EggBlock";

		// Token: 0x040000EC RID: 236
		public const int Index = 118;

		// Token: 0x040000ED RID: 237
		public DynamicArray<EggBlock.EggType> m_eggTypes = new DynamicArray<EggBlock.EggType>();

		// Token: 0x020003EB RID: 1003
		public class EggType
		{
			// Token: 0x04001464 RID: 5220
			public int EggTypeIndex;

			// Token: 0x04001465 RID: 5221
			public bool ShowEgg;

			// Token: 0x04001466 RID: 5222
			public string DisplayName;

			// Token: 0x04001467 RID: 5223
			public string TemplateName;

			// Token: 0x04001468 RID: 5224
			public float NutritionalValue;

			// Token: 0x04001469 RID: 5225
			public int TextureSlot;

			// Token: 0x0400146A RID: 5226
			public Color Color;

			// Token: 0x0400146B RID: 5227
			public Vector2 ScaleUV;

			// Token: 0x0400146C RID: 5228
			public bool SwapUV;

			// Token: 0x0400146D RID: 5229
			public float Scale;

			// Token: 0x0400146E RID: 5230
			public BlockMesh BlockMesh;
		}
	}
}
