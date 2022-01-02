using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000070 RID: 112
	public class FurnitureBlock : Block, IPaintableBlock, IElectricElementBlock
	{
		// Token: 0x06000282 RID: 642 RVA: 0x0001047C File Offset: 0x0000E67C
		public override void Initialize()
		{
			for (int i = 0; i < 4; i++)
			{
				this.m_matrices[i] = Matrix.CreateTranslation(new Vector3(-0.5f, 0f, -0.5f)) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(new Vector3(0.5f, 0f, 0.5f));
			}
			base.Initialize();
		}

		// Token: 0x06000283 RID: 643 RVA: 0x000104F8 File Offset: 0x0000E6F8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			if (generator.SubsystemFurnitureBlockBehavior == null)
			{
				return;
			}
			int data = Terrain.ExtractData(value);
			int designIndex = FurnitureBlock.GetDesignIndex(data);
			int rotation = FurnitureBlock.GetRotation(data);
			FurnitureDesign design = generator.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design == null)
			{
				return;
			}
			FurnitureGeometry geometry2 = design.Geometry;
			int mountingFacesMask = design.MountingFacesMask;
			for (int i = 0; i < 6; i++)
			{
				int num = CellFace.OppositeFace((i < 4) ? ((i + rotation) % 4) : i);
				byte b = (byte)(LightingManager.LightIntensityByLightValueAndFace[15 + 16 * num] * 255f);
				Color color = new Color(b, b, b);
				if (geometry2.SubsetOpaqueByFace[i] != null)
				{
					generator.GenerateShadedMeshVertices(this, x, y, z, geometry2.SubsetOpaqueByFace[i], color, new Matrix?(this.m_matrices[rotation]), this.m_facesMaps[rotation], geometry.OpaqueSubsetsByFace[num]);
				}
				if (geometry2.SubsetAlphaTestByFace[i] != null)
				{
					generator.GenerateShadedMeshVertices(this, x, y, z, geometry2.SubsetAlphaTestByFace[i], color, new Matrix?(this.m_matrices[rotation]), this.m_facesMaps[rotation], geometry.AlphaTestSubsetsByFace[num]);
				}
				int num2 = CellFace.OppositeFace((i < 4) ? ((i - rotation + 4) % 4) : i);
				if ((mountingFacesMask & 1 << num2) != 0)
				{
					generator.GenerateWireVertices(value, x, y, z, i, 0f, Vector2.Zero, geometry.SubsetOpaque);
				}
			}
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00010660 File Offset: 0x0000E860
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if (environmentData.SubsystemTerrain == null)
			{
				return;
			}
			int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
			FurnitureDesign design = environmentData.SubsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design == null)
			{
				return;
			}
			Matrix matrix2 = Matrix.CreateTranslation(new Vector3
			{
				X = -0.5f * (float)(design.Box.Left + design.Box.Right) / (float)design.Resolution,
				Y = -0.5f * (float)(design.Box.Top + design.Box.Bottom) / (float)design.Resolution,
				Z = -0.5f * (float)(design.Box.Near + design.Box.Far) / (float)design.Resolution
			} * size) * matrix;
			FurnitureGeometry geometry = design.Geometry;
			for (int i = 0; i < 6; i++)
			{
				float s = LightingManager.LightIntensityByLightValueAndFace[environmentData.Light + 16 * CellFace.OppositeFace(i)];
				Color color2 = Color.MultiplyColorOnly(color, s);
				if (geometry.SubsetOpaqueByFace[i] != null)
				{
					BlocksManager.DrawMeshBlock(primitivesRenderer, geometry.SubsetOpaqueByFace[i], color2, size, ref matrix2, environmentData);
				}
				if (geometry.SubsetAlphaTestByFace[i] != null)
				{
					BlocksManager.DrawMeshBlock(primitivesRenderer, geometry.SubsetAlphaTestByFace[i], color2, size, ref matrix2, environmentData);
				}
			}
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000107D4 File Offset: 0x0000E9D4
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			if (subsystemTerrain != null)
			{
				int data = Terrain.ExtractData(value);
				int rotation = FurnitureBlock.GetRotation(data);
				int designIndex = FurnitureBlock.GetDesignIndex(data);
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					return (1 << this.m_reverseFacesMaps[rotation][face] & design.TransparentFacesMask) != 0;
				}
			}
			return false;
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00010824 File Offset: 0x0000EA24
		public override int GetShadowStrength(int value)
		{
			int data = Terrain.ExtractData(value);
			if (FurnitureBlock.GetIsLightEmitter(data))
			{
				return -99;
			}
			return FurnitureBlock.GetShadowStrengthFactor(data) * 3 + 1;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0001084D File Offset: 0x0000EA4D
		public override int GetEmittedLightAmount(int value)
		{
			if (!FurnitureBlock.GetIsLightEmitter(Terrain.ExtractData(value)))
			{
				return 0;
			}
			return 15;
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00010860 File Offset: 0x0000EA60
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			if (subsystemTerrain != null)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					if (!string.IsNullOrEmpty(design.Name))
					{
						return design.Name;
					}
					return design.GetDefaultName();
				}
			}
			return "Furniture";
		}

		// Token: 0x06000289 RID: 649 RVA: 0x000108AC File Offset: 0x0000EAAC
		public override bool IsInteractive(SubsystemTerrain subsystemTerrain, int value)
		{
			if (subsystemTerrain != null)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					return design.InteractionMode == FurnitureInteractionMode.Multistate || design.InteractionMode == FurnitureInteractionMode.ElectricButton || design.InteractionMode == FurnitureInteractionMode.ElectricSwitch || design.InteractionMode == FurnitureInteractionMode.ConnectedMultistate;
				}
			}
			return base.IsInteractive(subsystemTerrain, value);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00010908 File Offset: 0x0000EB08
		public override string GetSoundMaterialName(SubsystemTerrain subsystemTerrain, int value)
		{
			if (subsystemTerrain != null)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					int mainValue = design.MainValue;
					int num = Terrain.ExtractContents(mainValue);
					return BlocksManager.Blocks[num].GetSoundMaterialName(subsystemTerrain, mainValue);
				}
			}
			return base.GetSoundMaterialName(subsystemTerrain, value);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0001095C File Offset: 0x0000EB5C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain subsystemTerrain, int value)
		{
			if (subsystemTerrain != null)
			{
				int data = Terrain.ExtractData(value);
				int designIndex = FurnitureBlock.GetDesignIndex(data);
				int rotation = FurnitureBlock.GetRotation(data);
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					return design.GetCollisionBoxes(rotation);
				}
			}
			return base.GetCustomCollisionBoxes(subsystemTerrain, value);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x000109A0 File Offset: 0x0000EBA0
		public override BoundingBox[] GetCustomInteractionBoxes(SubsystemTerrain subsystemTerrain, int value)
		{
			if (subsystemTerrain != null)
			{
				int data = Terrain.ExtractData(value);
				int designIndex = FurnitureBlock.GetDesignIndex(data);
				int rotation = FurnitureBlock.GetRotation(data);
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					return design.GetInteractionBoxes(rotation);
				}
			}
			return base.GetCustomInteractionBoxes(subsystemTerrain, value);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x000109E4 File Offset: 0x0000EBE4
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int faceTextureSlot = this.GetFaceTextureSlot(4, value);
			int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
			FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design != null)
			{
				int mainValue = design.MainValue;
				int num = Terrain.ExtractContents(mainValue);
				return BlocksManager.Blocks[num].CreateDebrisParticleSystem(subsystemTerrain, position, mainValue, strength);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, faceTextureSlot);
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00010A50 File Offset: 0x0000EC50
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int rotation = 0;
			if (raycastResult.CellFace.Face < 4)
			{
				rotation = CellFace.OppositeFace(raycastResult.CellFace.Face);
			}
			else
			{
				Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
				float num = Vector3.Dot(forward, Vector3.UnitZ);
				float num2 = Vector3.Dot(forward, Vector3.UnitX);
				float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
				float num4 = Vector3.Dot(forward, -Vector3.UnitX);
				if (num == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 0;
				}
				else if (num2 == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 1;
				}
				else if (num3 == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 2;
				}
				else if (num4 == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 3;
				}
			}
			int data = FurnitureBlock.SetRotation(Terrain.ExtractData(value), rotation);
			return new BlockPlacementData
			{
				CellFace = raycastResult.CellFace,
				Value = Terrain.ReplaceData(value, data)
			};
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00010B60 File Offset: 0x0000ED60
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int data = Terrain.ExtractData(oldValue);
			data = FurnitureBlock.SetRotation(data, 0);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(227, 0, data),
				Count = 1
			});
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00010BAC File Offset: 0x0000EDAC
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			if (environmentData.SubsystemTerrain != null)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
				FurnitureDesign design = environmentData.SubsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					float num = (float)design.Resolution / (float)MathUtils.Max(design.Box.Width, design.Box.Height, design.Box.Depth);
					return this.DefaultIconViewScale * num;
				}
			}
			return base.GetIconViewScale(value, environmentData);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00010C24 File Offset: 0x0000EE24
		public int? GetPaintColor(int value)
		{
			return null;
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00010C3C File Offset: 0x0000EE3C
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			int designIndex = FurnitureBlock.GetDesignIndex(data);
			FurnitureDesign design = terrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design != null)
			{
				List<FurnitureDesign> list = design.CloneChain();
				foreach (FurnitureDesign furnitureDesign in list)
				{
					furnitureDesign.Paint(color);
				}
				FurnitureDesign furnitureDesign2 = terrain.SubsystemFurnitureBlockBehavior.TryAddDesignChain(list[0], true);
				if (furnitureDesign2 != null)
				{
					int data2 = FurnitureBlock.SetDesignIndex(data, furnitureDesign2.Index, furnitureDesign2.ShadowStrengthFactor, furnitureDesign2.IsLightEmitter);
					return Terrain.ReplaceData(value, data2);
				}
				this.DisplayError();
			}
			return value;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00010CF4 File Offset: 0x0000EEF4
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain terrain, string[] ingredients, float heatLevel, float playerLevel)
		{
			if (heatLevel != 0f)
			{
				return null;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			List<FurnitureDesign> list = new List<FurnitureDesign>();
			for (int i = 0; i < ingredients.Length; i++)
			{
				if (!string.IsNullOrEmpty(ingredients[i]))
				{
					string a;
					int? num4;
					CraftingRecipesManager.DecodeIngredient(ingredients[i], out a, out num4);
					if (a == BlocksManager.Blocks[227].CraftingId)
					{
						FurnitureDesign design = terrain.SubsystemFurnitureBlockBehavior.GetDesign(FurnitureBlock.GetDesignIndex(num4.GetValueOrDefault()));
						if (design == null)
						{
							return null;
						}
						list.Add(design);
					}
					else if (a == BlocksManager.Blocks[142].CraftingId)
					{
						num++;
					}
					else if (a == BlocksManager.Blocks[141].CraftingId)
					{
						num2++;
					}
					else
					{
						if (!(a == BlocksManager.Blocks[133].CraftingId))
						{
							return null;
						}
						num3++;
					}
				}
			}
			if (list.Count == 1 && num == 1 && num2 == 0 && num3 == 0)
			{
				FurnitureDesign furnitureDesign = list[0].Clone();
				furnitureDesign.InteractionMode = FurnitureInteractionMode.ElectricButton;
				FurnitureDesign furnitureDesign2 = terrain.SubsystemFurnitureBlockBehavior.TryAddDesignChain(furnitureDesign, true);
				if (furnitureDesign2 == null)
				{
					this.DisplayError();
					return null;
				}
				return new CraftingRecipe
				{
					ResultValue = Terrain.MakeBlockValue(227, 0, FurnitureBlock.SetDesignIndex(0, furnitureDesign2.Index, furnitureDesign2.ShadowStrengthFactor, furnitureDesign2.IsLightEmitter)),
					ResultCount = 1,
					Description = LanguageControl.Get(base.GetType().Name, 0),
					Ingredients = (string[])ingredients.Clone()
				};
			}
			else if (list.Count == 2 && num == 0 && num2 == 1 && num3 == 0)
			{
				List<FurnitureDesign> list2 = (from d in list
				select d.Clone()).ToList<FurnitureDesign>();
				for (int j = 0; j < list2.Count; j++)
				{
					list2[j].InteractionMode = FurnitureInteractionMode.ElectricSwitch;
					list2[j].LinkedDesign = list2[(j + 1) % list2.Count];
				}
				FurnitureDesign furnitureDesign3 = terrain.SubsystemFurnitureBlockBehavior.TryAddDesignChain(list2[0], true);
				if (furnitureDesign3 == null)
				{
					this.DisplayError();
					return null;
				}
				return new CraftingRecipe
				{
					ResultValue = Terrain.MakeBlockValue(227, 0, FurnitureBlock.SetDesignIndex(0, furnitureDesign3.Index, furnitureDesign3.ShadowStrengthFactor, furnitureDesign3.IsLightEmitter)),
					ResultCount = 1,
					Description = LanguageControl.Get(base.GetType().Name, 0),
					Ingredients = (string[])ingredients.Clone()
				};
			}
			else
			{
				if (list.Count < 2 || num != 0 || num2 != 0 || num3 > 1)
				{
					return null;
				}
				List<FurnitureDesign> list3 = (from d in list
				select d.Clone()).ToList<FurnitureDesign>();
				for (int k = 0; k < list3.Count; k++)
				{
					list3[k].InteractionMode = ((num3 == 0) ? FurnitureInteractionMode.Multistate : FurnitureInteractionMode.ConnectedMultistate);
					list3[k].LinkedDesign = list3[(k + 1) % list3.Count];
				}
				FurnitureDesign furnitureDesign4 = terrain.SubsystemFurnitureBlockBehavior.TryAddDesignChain(list3[0], true);
				if (furnitureDesign4 == null)
				{
					this.DisplayError();
					return null;
				}
				return new CraftingRecipe
				{
					ResultValue = Terrain.MakeBlockValue(227, 0, FurnitureBlock.SetDesignIndex(0, furnitureDesign4.Index, furnitureDesign4.ShadowStrengthFactor, furnitureDesign4.IsLightEmitter)),
					ResultCount = 1,
					Description = LanguageControl.Get(base.GetType().Name, 0),
					Ingredients = (string[])ingredients.Clone()
				};
			}
		}

		// Token: 0x06000294 RID: 660 RVA: 0x000110CC File Offset: 0x0000F2CC
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
			FurnitureDesign design = subsystemElectricity.SubsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design != null)
			{
				if (design.InteractionMode == FurnitureInteractionMode.Multistate || design.InteractionMode == FurnitureInteractionMode.ConnectedMultistate)
				{
					return new MultistateFurnitureElectricElement(subsystemElectricity, new Point3(x, y, z));
				}
				if (design.InteractionMode == FurnitureInteractionMode.ElectricButton)
				{
					return new ButtonFurnitureElectricElement(subsystemElectricity, new Point3(x, y, z));
				}
				if (design.InteractionMode == FurnitureInteractionMode.ElectricSwitch)
				{
					return new SwitchFurnitureElectricElement(subsystemElectricity, new Point3(x, y, z), value);
				}
			}
			return null;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00011154 File Offset: 0x0000F354
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int rotation = FurnitureBlock.GetRotation(data);
			int designIndex = FurnitureBlock.GetDesignIndex(data);
			FurnitureDesign design = terrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design != null)
			{
				int num = CellFace.OppositeFace((face < 4) ? ((face - rotation + 4) % 4) : face);
				if ((design.MountingFacesMask & 1 << num) != 0 && SubsystemElectricity.GetConnectorDirection(face, 0, connectorFace) != null)
				{
					Point3 point = CellFace.FaceToPoint3(face);
					int cellValue = terrain.Terrain.GetCellValue(x - point.X, y - point.Y, z - point.Z);
					if (!BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsFaceTransparent(terrain, CellFace.OppositeFace(num), cellValue))
					{
						if (design.InteractionMode == FurnitureInteractionMode.Multistate || design.InteractionMode == FurnitureInteractionMode.ConnectedMultistate)
						{
							return new ElectricConnectorType?(ElectricConnectorType.Input);
						}
						if (design.InteractionMode == FurnitureInteractionMode.ElectricButton || design.InteractionMode == FurnitureInteractionMode.ElectricSwitch)
						{
							return new ElectricConnectorType?(ElectricConnectorType.Output);
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00011250 File Offset: 0x0000F450
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00011257 File Offset: 0x0000F457
		public void DisplayError()
		{
			DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, LanguageControl.Get(base.GetType().Name, 1), LanguageControl.Ok, null, null));
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00011281 File Offset: 0x0000F481
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00011286 File Offset: 0x0000F486
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00011290 File Offset: 0x0000F490
		public static int GetDesignIndex(int data)
		{
			return data >> 15 << 10 | (data >> 2 & 1023);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x000112A4 File Offset: 0x0000F4A4
		public static int SetDesignIndex(int data, int designIndex, int shadowStrengthFactor, bool isLightEmitter)
		{
			int num = designIndex >> 10;
			int num2 = designIndex & 1023;
			data = ((data & 3) | (num2 & 1023) << 2);
			data = ((data & 4095) | (shadowStrengthFactor & 3) << 12);
			data = ((data & 16383) | (isLightEmitter ? 1 : 0) << 14);
			data = ((data & 32767) | num << 15);
			return data;
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00011300 File Offset: 0x0000F500
		public static FurnitureDesign GetDesign(SubsystemFurnitureBlockBehavior subsystemFurnitureBlockBehavior, int value)
		{
			int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
			return subsystemFurnitureBlockBehavior.GetDesign(designIndex);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00011320 File Offset: 0x0000F520
		public static int GetShadowStrengthFactor(int data)
		{
			return data >> 12 & 3;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00011328 File Offset: 0x0000F528
		public static bool GetIsLightEmitter(int data)
		{
			return (data >> 14 & 1) != 0;
		}

		// Token: 0x04000126 RID: 294
		public const int Index = 227;

		// Token: 0x04000127 RID: 295
		public Matrix[] m_matrices = new Matrix[4];

		// Token: 0x04000128 RID: 296
		public int[][] m_facesMaps = new int[][]
		{
			new int[]
			{
				0,
				1,
				2,
				3,
				4,
				5
			},
			new int[]
			{
				1,
				2,
				3,
				0,
				4,
				5
			},
			new int[]
			{
				2,
				3,
				0,
				1,
				4,
				5
			},
			new int[]
			{
				3,
				0,
				1,
				2,
				4,
				5
			}
		};

		// Token: 0x04000129 RID: 297
		public int[][] m_reverseFacesMaps = new int[][]
		{
			new int[]
			{
				0,
				1,
				2,
				3,
				4,
				5
			},
			new int[]
			{
				3,
				0,
				1,
				2,
				4,
				5
			},
			new int[]
			{
				2,
				3,
				0,
				1,
				4,
				5
			},
			new int[]
			{
				1,
				2,
				3,
				0,
				4,
				5
			}
		};
	}
}
