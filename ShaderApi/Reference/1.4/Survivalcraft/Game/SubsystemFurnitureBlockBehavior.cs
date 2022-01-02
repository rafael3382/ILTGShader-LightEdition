using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001AA RID: 426
	public class SubsystemFurnitureBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000AD4 RID: 2772 RVA: 0x000488E4 File Offset: 0x00046AE4
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x000488EC File Offset: 0x00046AEC
		public ReadOnlyList<FurnitureSet> FurnitureSets
		{
			get
			{
				return new ReadOnlyList<FurnitureSet>(this.m_furnitureSets);
			}
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x000488F9 File Offset: 0x00046AF9
		public FurnitureDesign GetDesign(int index)
		{
			if (index < 0 || index >= this.m_furnitureDesigns.Length)
			{
				return null;
			}
			return this.m_furnitureDesigns[index];
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x00048914 File Offset: 0x00046B14
		public FurnitureDesign FindMatchingDesign(FurnitureDesign design)
		{
			for (int i = 0; i < this.m_furnitureDesigns.Length; i++)
			{
				if (this.m_furnitureDesigns[i] != null && this.m_furnitureDesigns[i].Compare(design))
				{
					return this.m_furnitureDesigns[i];
				}
			}
			return null;
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x00048958 File Offset: 0x00046B58
		public FurnitureDesign FindMatchingDesignChain(FurnitureDesign design)
		{
			FurnitureDesign furnitureDesign = this.FindMatchingDesign(design);
			if (furnitureDesign != null && design.CompareChain(furnitureDesign))
			{
				return furnitureDesign;
			}
			return null;
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x0004897C File Offset: 0x00046B7C
		public FurnitureDesign TryAddDesign(FurnitureDesign design)
		{
			for (int i = 0; i < this.m_furnitureDesigns.Length; i++)
			{
				if (this.m_furnitureDesigns[i] != null && this.m_furnitureDesigns[i].Compare(design))
				{
					return this.m_furnitureDesigns[i];
				}
			}
			for (int j = 0; j < this.m_furnitureDesigns.Length; j++)
			{
				if (this.m_furnitureDesigns[j] == null)
				{
					this.AddDesign(j, design);
					return design;
				}
			}
			this.GarbageCollectDesigns();
			for (int k = 0; k < this.m_furnitureDesigns.Length; k++)
			{
				if (this.m_furnitureDesigns[k] == null)
				{
					this.AddDesign(k, design);
					return design;
				}
			}
			return null;
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x00048A14 File Offset: 0x00046C14
		public FurnitureDesign TryAddDesignChain(FurnitureDesign design, bool garbageCollectIfNeeded)
		{
			FurnitureDesign furnitureDesign = this.FindMatchingDesignChain(design);
			if (furnitureDesign != null)
			{
				return furnitureDesign;
			}
			List<FurnitureDesign> list = design.ListChain();
			if (garbageCollectIfNeeded)
			{
				if (this.m_furnitureDesigns.Count((FurnitureDesign d) => d == null) < list.Count)
				{
					this.GarbageCollectDesigns();
				}
			}
			if (this.m_furnitureDesigns.Count((FurnitureDesign d) => d == null) < list.Count)
			{
				return null;
			}
			int num = 0;
			int num2 = 0;
			while (num2 < this.m_furnitureDesigns.Length && num < list.Count)
			{
				if (this.m_furnitureDesigns[num2] == null)
				{
					this.AddDesign(num2, list[num]);
					num++;
				}
				num2++;
			}
			if (num != list.Count)
			{
				throw new InvalidOperationException("public error.");
			}
			return design;
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x00048AF4 File Offset: 0x00046CF4
		public void ScanDesign(CellFace start, Vector3 direction, ComponentMiner componentMiner)
		{
			FurnitureDesign design = null;
			FurnitureDesign furnitureDesign = null;
			Dictionary<Point3, int> valuesDictionary = new Dictionary<Point3, int>();
			Point3 point = start.Point;
			Point3 point2 = start.Point;
			int startValue = base.SubsystemTerrain.Terrain.GetCellValue(start.Point.X, start.Point.Y, start.Point.Z);
			int num = Terrain.ExtractContents(startValue);
			if (BlocksManager.Blocks[num] is FurnitureBlock)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(startValue));
				furnitureDesign = this.GetDesign(designIndex);
				if (furnitureDesign == null)
				{
					ComponentPlayer componentPlayer = componentMiner.ComponentPlayer;
					if (componentPlayer == null)
					{
						return;
					}
					componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemFurnitureBlockBehavior.fName, 0), Color.White, true, false);
					return;
				}
				else
				{
					design = furnitureDesign.Clone();
					design.LinkedDesign = null;
					design.InteractionMode = FurnitureInteractionMode.None;
					valuesDictionary.Add(start.Point, startValue);
				}
			}
			else
			{
				Stack<Point3> stack = new Stack<Point3>();
				stack.Push(start.Point);
				while (stack.Count > 0)
				{
					Point3 point3 = stack.Pop();
					if (!valuesDictionary.ContainsKey(point3))
					{
						int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(point3.X, point3.Y, point3.Z);
						if (SubsystemFurnitureBlockBehavior.IsValueDisallowed(cellValue))
						{
							ComponentPlayer componentPlayer2 = componentMiner.ComponentPlayer;
							if (componentPlayer2 == null)
							{
								return;
							}
							componentPlayer2.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemFurnitureBlockBehavior.fName, 1), Color.White, true, false);
							return;
						}
						else if (SubsystemFurnitureBlockBehavior.IsValueAllowed(cellValue))
						{
							if (point3.X < point.X)
							{
								point.X = point3.X;
							}
							if (point3.Y < point.Y)
							{
								point.Y = point3.Y;
							}
							if (point3.Z < point.Z)
							{
								point.Z = point3.Z;
							}
							if (point3.X > point2.X)
							{
								point2.X = point3.X;
							}
							if (point3.Y > point2.Y)
							{
								point2.Y = point3.Y;
							}
							if (point3.Z > point2.Z)
							{
								point2.Z = point3.Z;
							}
							if (MathUtils.Abs(point.X - point2.X) >= 128 || MathUtils.Abs(point.Y - point2.Y) >= 128 || MathUtils.Abs(point.Z - point2.Z) >= 128)
							{
								ComponentPlayer componentPlayer3 = componentMiner.ComponentPlayer;
								if (componentPlayer3 == null)
								{
									return;
								}
								componentPlayer3.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemFurnitureBlockBehavior.fName, 2), Color.White, true, false);
								return;
							}
							else
							{
								valuesDictionary[point3] = cellValue;
								stack.Push(new Point3(point3.X - 1, point3.Y, point3.Z));
								stack.Push(new Point3(point3.X + 1, point3.Y, point3.Z));
								stack.Push(new Point3(point3.X, point3.Y - 1, point3.Z));
								stack.Push(new Point3(point3.X, point3.Y + 1, point3.Z));
								stack.Push(new Point3(point3.X, point3.Y, point3.Z - 1));
								stack.Push(new Point3(point3.X, point3.Y, point3.Z + 1));
							}
						}
					}
				}
				if (valuesDictionary.Count == 0)
				{
					ComponentPlayer componentPlayer4 = componentMiner.ComponentPlayer;
					if (componentPlayer4 == null)
					{
						return;
					}
					componentPlayer4.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemFurnitureBlockBehavior.fName, 0), Color.White, true, false);
					return;
				}
				else
				{
					design = new FurnitureDesign(base.SubsystemTerrain);
					Point3 point4 = point2 - point;
					int num2 = MathUtils.Max(MathUtils.Max(point4.X, point4.Y, point4.Z) + 1, 2);
					int[] array = new int[num2 * num2 * num2];
					foreach (KeyValuePair<Point3, int> keyValuePair in valuesDictionary)
					{
						Point3 point5 = keyValuePair.Key - point;
						array[point5.X + point5.Y * num2 + point5.Z * num2 * num2] = keyValuePair.Value;
					}
					design.SetValues(num2, array);
					int steps = (start.Face > 3) ? CellFace.Vector3ToFace(direction, 3) : CellFace.OppositeFace(start.Face);
					design.Rotate(1, steps);
					Point3 location = design.Box.Location;
					Point3 point6 = new Point3(design.Resolution) - (design.Box.Location + design.Box.Size);
					Point3 delta = new Point3((point6.X - location.X) / 2, -location.Y, (point6.Z - location.Z) / 2);
					design.Shift(delta);
				}
			}
			Action <>9__1;
			BuildFurnitureDialog dialog = new BuildFurnitureDialog(design, furnitureDesign, delegate(bool result)
			{
				if (result)
				{
					design = this.TryAddDesign(design);
					if (design == null)
					{
						ComponentPlayer componentPlayer5 = componentMiner.ComponentPlayer;
						if (componentPlayer5 == null)
						{
							return;
						}
						componentPlayer5.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemFurnitureBlockBehavior.fName, 3), Color.White, true, false);
						return;
					}
					else
					{
						if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative)
						{
							foreach (KeyValuePair<Point3, int> keyValuePair2 in valuesDictionary)
							{
								this.SubsystemTerrain.DestroyCell(0, keyValuePair2.Key.X, keyValuePair2.Key.Y, keyValuePair2.Key.Z, 0, true, true);
							}
						}
						int value = Terrain.MakeBlockValue(227, 0, FurnitureBlock.SetDesignIndex(0, design.Index, design.ShadowStrengthFactor, design.IsLightEmitter));
						int num3 = MathUtils.Clamp(design.Resolution, 4, 8);
						Matrix matrix = componentMiner.ComponentCreature.ComponentBody.Matrix;
						Vector3 position = matrix.Translation + 1f * matrix.Forward + 1f * Vector3.UnitY;
						this.m_subsystemPickables.AddPickable(value, num3, position, null, null);
						componentMiner.DamageActiveTool(1);
						componentMiner.Poke(false);
						for (int i = 0; i < 3; i++)
						{
							double time = Time.FrameStartTime + (double)((float)i * 0.25f);
							Action action;
							if ((action = <>9__1) == null)
							{
								action = (<>9__1 = delegate()
								{
									this.m_subsystemSoundMaterials.PlayImpactSound(startValue, new Vector3(start.Point), 1f);
								});
							}
							Time.QueueTimeDelayedExecution(time, action);
						}
						if (componentMiner.ComponentCreature.PlayerStats != null)
						{
							componentMiner.ComponentCreature.PlayerStats.FurnitureItemsMade += (long)num3;
						}
					}
				}
			});
			if (componentMiner.ComponentPlayer != null)
			{
				DialogsManager.ShowDialog(componentMiner.ComponentPlayer.GuiWidget, dialog);
			}
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x00049110 File Offset: 0x00047310
		public void SwitchToNextState(int x, int y, int z, bool playSound)
		{
			HashSet<Point3> hashSet = new HashSet<Point3>();
			List<Point3> list = new List<Point3>();
			list.Add(new Point3(x, y, z));
			int num = 0;
			while (num < list.Count && num < 4096)
			{
				Point3 point = list[num++];
				if (hashSet.Add(point))
				{
					int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
					if (Terrain.ExtractContents(cellValue) == 227)
					{
						int data = Terrain.ExtractData(cellValue);
						int designIndex = FurnitureBlock.GetDesignIndex(data);
						FurnitureDesign design = this.GetDesign(designIndex);
						if (design != null && design.LinkedDesign != null && design.LinkedDesign.Index >= 0 && (list.Count == 1 || design.InteractionMode == FurnitureInteractionMode.ConnectedMultistate))
						{
							int data2 = FurnitureBlock.SetDesignIndex(data, design.LinkedDesign.Index, design.LinkedDesign.ShadowStrengthFactor, design.LinkedDesign.IsLightEmitter);
							int value = Terrain.ReplaceData(cellValue, data2);
							base.SubsystemTerrain.ChangeCell(point.X, point.Y, point.Z, value, true);
							if (design.InteractionMode == FurnitureInteractionMode.ConnectedMultistate)
							{
								list.Add(new Point3(point.X - 1, point.Y, point.Z));
								list.Add(new Point3(point.X + 1, point.Y, point.Z));
								list.Add(new Point3(point.X, point.Y - 1, point.Z));
								list.Add(new Point3(point.X, point.Y + 1, point.Z));
								list.Add(new Point3(point.X, point.Y, point.Z - 1));
								list.Add(new Point3(point.X, point.Y, point.Z + 1));
							}
						}
					}
				}
			}
			if (playSound)
			{
				this.m_subsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, new Vector3((float)x, (float)y, (float)z), 2f, true);
			}
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x00049344 File Offset: 0x00047544
		public void GarbageCollectDesigns()
		{
			this.GarbageCollectDesigns(this.m_subsystemItemsScanner.ScanItems());
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x00049358 File Offset: 0x00047558
		public FurnitureSet NewFurnitureSet(string name, string importedFrom)
		{
			if (name.Length > 64)
			{
				name = name.Substring(0, 64);
			}
			int num = 0;
			while (this.FurnitureSets.FirstOrDefault((FurnitureSet fs) => fs.Name == name) != null)
			{
				num++;
				name = ((num > 0) ? (name + num.ToString(CultureInfo.InvariantCulture)) : name);
			}
			FurnitureSet furnitureSet = new FurnitureSet
			{
				Name = name,
				ImportedFrom = importedFrom
			};
			this.m_furnitureSets.Add(furnitureSet);
			return furnitureSet;
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x0004940C File Offset: 0x0004760C
		public void DeleteFurnitureSet(FurnitureSet furnitureSet)
		{
			foreach (FurnitureDesign furnitureDesign in this.GetFurnitureSetDesigns(furnitureSet))
			{
				furnitureDesign.FurnitureSet = null;
			}
			this.m_furnitureSets.Remove(furnitureSet);
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x00049468 File Offset: 0x00047668
		public void MoveFurnitureSet(FurnitureSet furnitureSet, int move)
		{
			int num = this.m_furnitureSets.IndexOf(furnitureSet);
			if (num >= 0)
			{
				this.m_furnitureSets.RemoveAt(num);
				this.m_furnitureSets.Insert(MathUtils.Clamp(num + move, 0, this.m_furnitureSets.Count), furnitureSet);
			}
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x000494B4 File Offset: 0x000476B4
		public void AddToFurnitureSet(FurnitureDesign design, FurnitureSet furnitureSet)
		{
			foreach (FurnitureDesign furnitureDesign in design.ListChain())
			{
				furnitureDesign.FurnitureSet = furnitureSet;
			}
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x00049508 File Offset: 0x00047708
		public IEnumerable<FurnitureDesign> GetFurnitureSetDesigns(FurnitureSet furnitureSet)
		{
			return from fd in this.m_furnitureDesigns
			where fd != null && fd.FurnitureSet == furnitureSet
			select fd;
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x0004953C File Offset: 0x0004773C
		public static List<FurnitureDesign> LoadFurnitureDesigns(SubsystemTerrain subsystemTerrain, ValuesDictionary valuesDictionary)
		{
			List<FurnitureDesign> list = new List<FurnitureDesign>();
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary)
			{
				int index = int.Parse(keyValuePair.Key, CultureInfo.InvariantCulture);
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)keyValuePair.Value;
				FurnitureDesign item = new FurnitureDesign(index, subsystemTerrain, valuesDictionary2);
				list.Add(item);
			}
			using (List<FurnitureDesign>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					FurnitureDesign design = enumerator2.Current;
					if (design.m_loadTimeLinkedDesignIndex >= 0)
					{
						design.LinkedDesign = list.FirstOrDefault((FurnitureDesign d) => d.Index == design.m_loadTimeLinkedDesignIndex);
					}
				}
			}
			return list;
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x00049628 File Offset: 0x00047828
		public static void SaveFurnitureDesigns(ValuesDictionary valuesDictionary, ICollection<FurnitureDesign> designs)
		{
			foreach (FurnitureDesign furnitureDesign in designs)
			{
				valuesDictionary.SetValue<ValuesDictionary>(furnitureDesign.Index.ToString(CultureInfo.InvariantCulture), furnitureDesign.Save());
			}
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x00049688 File Offset: 0x00047888
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.AddTerrainFurniture(value);
			this.AddParticleSystems(value, x, y, z);
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x0004969D File Offset: 0x0004789D
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.RemoveTerrainFurniture(value);
			this.RemoveParticleSystems(x, y, z);
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x000496B1 File Offset: 0x000478B1
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			this.RemoveTerrainFurniture(oldValue);
			this.RemoveParticleSystems(x, y, z);
			this.AddTerrainFurniture(value);
			this.AddParticleSystems(value, x, y, z);
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x000496D8 File Offset: 0x000478D8
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (!isLoaded)
			{
				this.AddTerrainFurniture(value);
			}
			this.AddParticleSystems(value, x, y, z);
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x000496F0 File Offset: 0x000478F0
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_particleSystemsByCell.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 point2 in list)
			{
				this.RemoveParticleSystems(point2.X, point2.Y, point2.Z);
			}
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x000497F4 File Offset: 0x000479F4
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (Terrain.ExtractContents(cellValue) == 227)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(cellValue));
				FurnitureDesign design = this.GetDesign(designIndex);
				if (design != null && (design.InteractionMode == FurnitureInteractionMode.Multistate || design.InteractionMode == FurnitureInteractionMode.ConnectedMultistate))
				{
					this.SwitchToNextState(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z, true);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x00049894 File Offset: 0x00047A94
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemSoundMaterials = base.Project.FindSubsystem<SubsystemSoundMaterials>(true);
			this.m_subsystemItemsScanner = base.Project.FindSubsystem<SubsystemItemsScanner>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			ValuesDictionary value = valuesDictionary.GetValue<ValuesDictionary>("FurnitureDesigns");
			foreach (FurnitureDesign furnitureDesign in SubsystemFurnitureBlockBehavior.LoadFurnitureDesigns(base.SubsystemTerrain, value))
			{
				this.m_furnitureDesigns[furnitureDesign.Index] = furnitureDesign;
			}
			foreach (object obj in from v in valuesDictionary.GetValue<ValuesDictionary>("FurnitureSets").Values
			where v is ValuesDictionary
			select v)
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)obj;
				string value2 = valuesDictionary2.GetValue<string>("Name");
				string value3 = valuesDictionary2.GetValue<string>("ImportedFrom", null);
				string value4 = valuesDictionary2.GetValue<string>("Indices");
				int[] array = HumanReadableConverter.ValuesListFromString<int>(';', value4);
				FurnitureSet furnitureSet = new FurnitureSet
				{
					Name = value2,
					ImportedFrom = value3
				};
				this.m_furnitureSets.Add(furnitureSet);
				foreach (int num in array)
				{
					if (num >= 0 && num < this.m_furnitureDesigns.Length && this.m_furnitureDesigns[num] != null)
					{
						this.m_furnitureDesigns[num].FurnitureSet = furnitureSet;
					}
				}
			}
			SubsystemItemsScanner subsystemItemsScanner = this.m_subsystemItemsScanner;
			subsystemItemsScanner.ItemsScanned = (Action<ReadOnlyList<ScannedItemData>>)Delegate.Combine(subsystemItemsScanner.ItemsScanned, new Action<ReadOnlyList<ScannedItemData>>(this.GarbageCollectDesigns));
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x00049AA4 File Offset: 0x00047CA4
		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			this.GarbageCollectDesigns();
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("FurnitureDesigns", valuesDictionary2);
			SubsystemFurnitureBlockBehavior.SaveFurnitureDesigns(valuesDictionary2, (from d in this.m_furnitureDesigns
			where d != null
			select d).ToArray<FurnitureDesign>());
			ValuesDictionary valuesDictionary3 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("FurnitureSets", valuesDictionary3);
			int num = 0;
			foreach (FurnitureSet furnitureSet in this.FurnitureSets)
			{
				ValuesDictionary valuesDictionary4 = new ValuesDictionary();
				valuesDictionary3.SetValue<ValuesDictionary>(num.ToString(CultureInfo.InvariantCulture), valuesDictionary4);
				valuesDictionary4.SetValue<string>("Name", furnitureSet.Name);
				if (furnitureSet.ImportedFrom != null)
				{
					valuesDictionary4.SetValue<string>("ImportedFrom", furnitureSet.ImportedFrom);
				}
				string value = HumanReadableConverter.ValuesListToString<int>(';', (from d in this.GetFurnitureSetDesigns(furnitureSet)
				select d.Index).ToArray<int>());
				valuesDictionary4.SetValue<string>("Indices", value);
				num++;
			}
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x00049BFC File Offset: 0x00047DFC
		public void AddDesign(int index, FurnitureDesign design)
		{
			this.m_furnitureDesigns[index] = design;
			design.Index = index;
			design.m_terrainUseCount = 0;
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x00049C18 File Offset: 0x00047E18
		public void AddTerrainFurniture(int value)
		{
			if (Terrain.ExtractContents(value) == 227)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
				if (designIndex < this.m_furnitureDesigns.Length)
				{
					this.m_furnitureDesigns[designIndex].m_terrainUseCount++;
				}
			}
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x00049C60 File Offset: 0x00047E60
		public void RemoveTerrainFurniture(int value)
		{
			if (Terrain.ExtractContents(value) == 227)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
				if (designIndex < this.m_furnitureDesigns.Length)
				{
					this.m_furnitureDesigns[designIndex].m_terrainUseCount = MathUtils.Max(this.m_furnitureDesigns[designIndex].m_terrainUseCount - 1, 0);
				}
			}
		}

		// Token: 0x06000AF0 RID: 2800 RVA: 0x00049CB4 File Offset: 0x00047EB4
		public void GarbageCollectDesigns(ReadOnlyList<ScannedItemData> allExistingItems)
		{
			for (int i = 0; i < this.m_furnitureDesigns.Length; i++)
			{
				if (this.m_furnitureDesigns[i] != null)
				{
					this.m_furnitureDesigns[i].m_gcUsed = (this.m_furnitureDesigns[i].m_terrainUseCount > 0);
				}
			}
			foreach (ScannedItemData scannedItemData in allExistingItems)
			{
				if (Terrain.ExtractContents(scannedItemData.Value) == 227)
				{
					int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(scannedItemData.Value));
					FurnitureDesign design = this.GetDesign(designIndex);
					if (design != null)
					{
						design.m_gcUsed = true;
					}
				}
			}
			for (int j = 0; j < this.m_furnitureDesigns.Length; j++)
			{
				if (this.m_furnitureDesigns[j] != null && this.m_furnitureDesigns[j].m_gcUsed)
				{
					FurnitureDesign linkedDesign = this.m_furnitureDesigns[j].LinkedDesign;
					while (linkedDesign != null && !linkedDesign.m_gcUsed)
					{
						linkedDesign.m_gcUsed = true;
						linkedDesign = linkedDesign.LinkedDesign;
					}
				}
			}
			for (int k = 0; k < this.m_furnitureDesigns.Length; k++)
			{
				if (this.m_furnitureDesigns[k] != null && !this.m_furnitureDesigns[k].m_gcUsed && this.m_furnitureDesigns[k].FurnitureSet == null)
				{
					this.m_furnitureDesigns[k].Index = -1;
					this.m_furnitureDesigns[k] = null;
				}
			}
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x00049E2C File Offset: 0x0004802C
		public static bool IsValueAllowed(int value)
		{
			int num = Terrain.ExtractContents(value);
			if (num <= 46)
			{
				if (num <= 21)
				{
					switch (num)
					{
					case 3:
						return true;
					case 4:
						return true;
					case 5:
						return true;
					case 6:
						break;
					case 7:
						return true;
					default:
						switch (num)
						{
						case 15:
							return true;
						case 17:
							return true;
						case 18:
							return true;
						case 21:
							return true;
						}
						break;
					}
				}
				else
				{
					if (num == 26)
					{
						return true;
					}
					if (num == 31)
					{
						return true;
					}
					if (num == 46)
					{
						return true;
					}
				}
			}
			else if (num <= 92)
			{
				if (num == 47)
				{
					return true;
				}
				switch (num)
				{
				case 67:
					return true;
				case 68:
					return true;
				case 69:
				case 70:
					break;
				case 71:
					return true;
				case 72:
					return true;
				case 73:
					return true;
				default:
					if (num == 92)
					{
						return true;
					}
					break;
				}
			}
			else
			{
				if (num == 126)
				{
					return true;
				}
				if (num == 150)
				{
					return true;
				}
				if (num == 208)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x00049F1C File Offset: 0x0004811C
		public static bool IsValueDisallowed(int value)
		{
			int num = Terrain.ExtractContents(value);
			int data = Terrain.ExtractData(value);
			return (num == 18 || num == 92) && FluidBlock.GetLevel(data) != 0 && FluidBlock.GetIsTop(data);
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x00049F54 File Offset: 0x00048154
		public void AddParticleSystems(int value, int x, int y, int z)
		{
			if (Terrain.ExtractContents(value) != 227)
			{
				return;
			}
			int data = Terrain.ExtractData(value);
			int rotation = FurnitureBlock.GetRotation(data);
			int designIndex = FurnitureBlock.GetDesignIndex(data);
			FurnitureDesign design = this.GetDesign(designIndex);
			if (design == null)
			{
				return;
			}
			List<FireParticleSystem> list = new List<FireParticleSystem>();
			BoundingBox[] torchPoints = design.GetTorchPoints(rotation);
			if (torchPoints.Length != 0)
			{
				foreach (BoundingBox boundingBox in torchPoints)
				{
					float num = (boundingBox.Size().X + boundingBox.Size().Y + boundingBox.Size().Z) / 3f;
					float size = MathUtils.Clamp(1.5f * num, 0.1f, 1f);
					FireParticleSystem fireParticleSystem = new FireParticleSystem(new Vector3((float)x, (float)y, (float)z) + boundingBox.Center(), size, 24f);
					this.m_subsystemParticles.AddParticleSystem(fireParticleSystem);
					list.Add(fireParticleSystem);
				}
			}
			if (list.Count > 0)
			{
				this.m_particleSystemsByCell[new Point3(x, y, z)] = list;
			}
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x0004A070 File Offset: 0x00048270
		public void RemoveParticleSystems(int x, int y, int z)
		{
			List<FireParticleSystem> list;
			if (this.m_particleSystemsByCell.TryGetValue(new Point3(x, y, z), out list))
			{
				foreach (FireParticleSystem fireParticleSystem in list)
				{
					fireParticleSystem.IsStopped = true;
				}
				this.m_particleSystemsByCell.Remove(new Point3(x, y, z));
			}
		}

		// Token: 0x0400054D RID: 1357
		public const int MaxFurnitureSetNameLength = 64;

		// Token: 0x0400054E RID: 1358
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400054F RID: 1359
		public SubsystemSoundMaterials m_subsystemSoundMaterials;

		// Token: 0x04000550 RID: 1360
		public SubsystemItemsScanner m_subsystemItemsScanner;

		// Token: 0x04000551 RID: 1361
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000552 RID: 1362
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x04000553 RID: 1363
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000554 RID: 1364
		public static string fName = "SubsystemFurnitureBlockBehavior";

		// Token: 0x04000555 RID: 1365
		public List<FurnitureSet> m_furnitureSets = new List<FurnitureSet>();

		// Token: 0x04000556 RID: 1366
		public FurnitureDesign[] m_furnitureDesigns = new FurnitureDesign[8192];

		// Token: 0x04000557 RID: 1367
		public Dictionary<Point3, List<FireParticleSystem>> m_particleSystemsByCell = new Dictionary<Point3, List<FireParticleSystem>>();
	}
}
