﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000297 RID: 663
	public class FurnitureDesign
	{
		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060014AE RID: 5294 RVA: 0x0009B390 File Offset: 0x00099590
		public int Resolution
		{
			get
			{
				return this.m_resolution;
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060014AF RID: 5295 RVA: 0x0009B398 File Offset: 0x00099598
		public int Hash
		{
			get
			{
				if (this.m_hash == null)
				{
					this.m_hash = new int?((int)(this.m_resolution + ((int)this.m_interactionMode << 4)));
					for (int i = 0; i < this.m_values.Length; i++)
					{
						this.m_hash += this.m_values[i] * (1 + 113 * i);
					}
				}
				return this.m_hash.Value;
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060014B0 RID: 5296 RVA: 0x0009B42C File Offset: 0x0009962C
		public Box Box
		{
			get
			{
				if (this.m_box == null)
				{
					this.m_box = new Box?(this.CalculateBox(new Box(0, 0, 0, this.Resolution, this.Resolution, this.Resolution), this.CreatePrecedingEmptySpacesArray()));
				}
				return this.m_box.Value;
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060014B1 RID: 5297 RVA: 0x0009B482 File Offset: 0x00099682
		public int ShadowStrengthFactor
		{
			get
			{
				if (this.m_shadowStrengthFactor == null)
				{
					this.CalculateShadowStrengthFactor();
				}
				return this.m_shadowStrengthFactor.Value;
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060014B2 RID: 5298 RVA: 0x0009B4A2 File Offset: 0x000996A2
		public bool IsLightEmitter
		{
			get
			{
				return this.GetTorchPoints(0).Length != 0;
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060014B3 RID: 5299 RVA: 0x0009B4AF File Offset: 0x000996AF
		public int MainValue
		{
			get
			{
				if (this.m_mainValue == 0)
				{
					this.CalculateMainValue();
				}
				return this.m_mainValue;
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060014B4 RID: 5300 RVA: 0x0009B4C5 File Offset: 0x000996C5
		public int MountingFacesMask
		{
			get
			{
				if (this.m_mountingFacesMask < 0)
				{
					this.CalculateFacesMasks();
				}
				return this.m_mountingFacesMask;
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060014B5 RID: 5301 RVA: 0x0009B4DC File Offset: 0x000996DC
		public int TransparentFacesMask
		{
			get
			{
				if (this.m_transparentFacesMask < 0)
				{
					this.CalculateFacesMasks();
				}
				return this.m_transparentFacesMask;
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060014B6 RID: 5302 RVA: 0x0009B4F3 File Offset: 0x000996F3
		// (set) Token: 0x060014B7 RID: 5303 RVA: 0x0009B4FB File Offset: 0x000996FB
		public int Index
		{
			get
			{
				return this.m_index;
			}
			set
			{
				this.m_index = value;
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060014B8 RID: 5304 RVA: 0x0009B504 File Offset: 0x00099704
		// (set) Token: 0x060014B9 RID: 5305 RVA: 0x0009B50C File Offset: 0x0009970C
		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				if (value.Length > 0)
				{
					if (value[0] == ' ' || value[value.Length - 1] == ' ')
					{
						throw new InvalidOperationException(LanguageControl.Get(FurnitureDesign.fName, 1));
					}
					foreach (char c in value)
					{
						if (c > '\u007f' || (!char.IsLetterOrDigit(c) && c != ' '))
						{
							throw new InvalidOperationException(LanguageControl.Get(FurnitureDesign.fName, 1));
						}
					}
					if (value.Length > 20)
					{
						value = value.Substring(0, 20);
					}
				}
				this.m_name = value;
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x060014BA RID: 5306 RVA: 0x0009B5AD File Offset: 0x000997AD
		// (set) Token: 0x060014BB RID: 5307 RVA: 0x0009B5B5 File Offset: 0x000997B5
		public FurnitureSet FurnitureSet
		{
			get
			{
				return this.m_furnitureSet;
			}
			set
			{
				this.m_furnitureSet = value;
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060014BC RID: 5308 RVA: 0x0009B5BE File Offset: 0x000997BE
		// (set) Token: 0x060014BD RID: 5309 RVA: 0x0009B5C6 File Offset: 0x000997C6
		public FurnitureDesign LinkedDesign
		{
			get
			{
				return this.m_linkedDesign;
			}
			set
			{
				if (value != this.m_linkedDesign)
				{
					this.m_linkedDesign = value;
					this.m_hash = null;
				}
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060014BE RID: 5310 RVA: 0x0009B5E4 File Offset: 0x000997E4
		// (set) Token: 0x060014BF RID: 5311 RVA: 0x0009B5EC File Offset: 0x000997EC
		public FurnitureInteractionMode InteractionMode
		{
			get
			{
				return this.m_interactionMode;
			}
			set
			{
				if (value != this.m_interactionMode)
				{
					this.m_interactionMode = value;
					this.m_hash = null;
				}
			}
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060014C0 RID: 5312 RVA: 0x0009B60A File Offset: 0x0009980A
		public FurnitureGeometry Geometry
		{
			get
			{
				if (this.m_geometry == null)
				{
					this.CreateGeometry();
				}
				return this.m_geometry;
			}
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x0009B620 File Offset: 0x00099820
		public FurnitureDesign(SubsystemTerrain subsystemTerrain)
		{
			this.m_subsystemTerrain = subsystemTerrain;
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x0009B658 File Offset: 0x00099858
		public FurnitureDesign(int index, SubsystemTerrain subsystemTerrain, ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTerrain = subsystemTerrain;
			this.m_index = index;
			this.Name = valuesDictionary.GetValue<string>("Name", string.Empty);
			this.m_terrainUseCount = valuesDictionary.GetValue<int>("TerrainUseCount");
			int value = valuesDictionary.GetValue<int>("Resolution");
			this.InteractionMode = valuesDictionary.GetValue<FurnitureInteractionMode>("InteractionMode");
			this.m_loadTimeLinkedDesignIndex = valuesDictionary.GetValue<int>("LinkedDesign", -1);
			string value2 = valuesDictionary.GetValue<string>("Values");
			int num = 0;
			int[] array = new int[value * value * value];
			string[] array2 = value2.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new char[]
				{
					'*'
				}, StringSplitOptions.None);
				if (array3.Length != 2)
				{
					throw new InvalidOperationException(LanguageControl.Get(FurnitureDesign.fName, 2));
				}
				int num2 = int.Parse(array3[0], CultureInfo.InvariantCulture);
				int num3 = int.Parse(array3[1], CultureInfo.InvariantCulture);
				int j = 0;
				while (j < num2)
				{
					array[num] = num3;
					j++;
					num++;
				}
			}
			this.SetValues(value, array);
		}

		// Token: 0x060014C3 RID: 5315 RVA: 0x0009B79D File Offset: 0x0009999D
		public int GetValue(int index)
		{
			return this.m_values[index];
		}

		// Token: 0x060014C4 RID: 5316 RVA: 0x0009B7A8 File Offset: 0x000999A8
		public void SetValues(int resolution, int[] values)
		{
			if (resolution < 2 || resolution > 128)
			{
				throw new ArgumentException(LanguageControl.Get(FurnitureDesign.fName, 3));
			}
			if (values.Length != resolution * resolution * resolution)
			{
				throw new ArgumentException(LanguageControl.Get(FurnitureDesign.fName, 4));
			}
			this.m_resolution = resolution;
			if (this.m_values == null || this.m_values.Length != resolution * resolution * resolution)
			{
				this.m_values = new int[resolution * resolution * resolution];
			}
			values.CopyTo(this.m_values, 0);
			this.m_hash = null;
			this.m_geometry = null;
			this.m_box = null;
			this.m_collisionBoxesByRotation = null;
			this.m_interactionBoxesByRotation = null;
			this.m_torchPointsByRotation = null;
			this.m_mainValue = 0;
			this.m_mountingFacesMask = -1;
			this.m_transparentFacesMask = -1;
		}

		// Token: 0x060014C5 RID: 5317 RVA: 0x0009B874 File Offset: 0x00099A74
		public string GetDefaultName()
		{
			if (this.InteractionMode == FurnitureInteractionMode.Multistate)
			{
				int count = this.ListChain().Count;
				if (count > 1)
				{
					return string.Format(LanguageControl.Get(FurnitureDesign.fName, 5), count);
				}
			}
			else
			{
				if (this.InteractionMode == FurnitureInteractionMode.ElectricButton)
				{
					return LanguageControl.Get(FurnitureDesign.fName, 6);
				}
				if (this.InteractionMode == FurnitureInteractionMode.ElectricSwitch)
				{
					return LanguageControl.Get(FurnitureDesign.fName, 7);
				}
				if (this.InteractionMode == FurnitureInteractionMode.ConnectedMultistate)
				{
					int count2 = this.ListChain().Count;
					if (count2 > 1)
					{
						return string.Format(LanguageControl.Get(FurnitureDesign.fName, 8), count2);
					}
				}
			}
			return LanguageControl.Get(FurnitureDesign.fName, 9);
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x0009B917 File Offset: 0x00099B17
		public BoundingBox[] GetCollisionBoxes(int rotation)
		{
			if (this.m_collisionBoxesByRotation == null)
			{
				this.CreateCollisionAndInteractionBoxes();
			}
			return this.m_collisionBoxesByRotation[rotation];
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x0009B92F File Offset: 0x00099B2F
		public BoundingBox[] GetInteractionBoxes(int rotation)
		{
			if (this.m_interactionBoxesByRotation == null)
			{
				this.CreateCollisionAndInteractionBoxes();
			}
			return this.m_interactionBoxesByRotation[rotation];
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x0009B947 File Offset: 0x00099B47
		public BoundingBox[] GetTorchPoints(int rotation)
		{
			if (this.m_torchPointsByRotation == null)
			{
				this.CreateTorchPoints();
			}
			return this.m_torchPointsByRotation[rotation];
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x0009B960 File Offset: 0x00099B60
		public void Paint(int? color)
		{
			int[] array = new int[this.m_values.Length];
			for (int i = 0; i < this.m_values.Length; i++)
			{
				int num = this.m_values[i];
				int num2 = Terrain.ExtractContents(num);
				IPaintableBlock paintableBlock = BlocksManager.Blocks[num2] as IPaintableBlock;
				array[i] = ((paintableBlock != null) ? paintableBlock.Paint(null, num, color) : num);
			}
			this.SetValues(this.Resolution, array);
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x0009B9D0 File Offset: 0x00099BD0
		public void Resize(int resolution)
		{
			if (resolution < 2 || resolution > 128)
			{
				throw new ArgumentException(LanguageControl.Get(FurnitureDesign.fName, 3));
			}
			if (resolution == this.m_resolution)
			{
				return;
			}
			int[] array = new int[resolution * resolution * resolution];
			for (int i = 0; i < resolution; i++)
			{
				for (int j = 0; j < resolution; j++)
				{
					for (int k = 0; k < resolution; k++)
					{
						if (k >= 0 && k < this.m_resolution && j >= 0 && j < this.m_resolution && i >= 0 && i < this.m_resolution)
						{
							array[k + j * resolution + i * resolution * resolution] = this.m_values[k + j * this.m_resolution + i * this.m_resolution * this.m_resolution];
						}
					}
				}
			}
			this.SetValues(resolution, array);
		}

		// Token: 0x060014CB RID: 5323 RVA: 0x0009BA90 File Offset: 0x00099C90
		public void Shift(Point3 delta)
		{
			if (!(delta != Point3.Zero))
			{
				return;
			}
			int[] array = new int[this.m_resolution * this.m_resolution * this.m_resolution];
			for (int i = 0; i < this.m_resolution; i++)
			{
				for (int j = 0; j < this.m_resolution; j++)
				{
					for (int k = 0; k < this.m_resolution; k++)
					{
						int num = k + delta.X;
						int num2 = j + delta.Y;
						int num3 = i + delta.Z;
						if (num >= 0 && num < this.m_resolution && num2 >= 0 && num2 < this.m_resolution && num3 >= 0 && num3 < this.m_resolution)
						{
							array[num + num2 * this.m_resolution + num3 * this.m_resolution * this.m_resolution] = this.m_values[k + j * this.m_resolution + i * this.m_resolution * this.m_resolution];
						}
					}
				}
			}
			this.SetValues(this.m_resolution, array);
		}

		// Token: 0x060014CC RID: 5324 RVA: 0x0009BBA4 File Offset: 0x00099DA4
		public void Rotate(int axis, int steps)
		{
			steps %= 4;
			if (steps < 0)
			{
				steps += 4;
			}
			if (steps <= 0)
			{
				return;
			}
			int[] array = new int[this.m_resolution * this.m_resolution * this.m_resolution];
			for (int i = 0; i < this.m_resolution; i++)
			{
				for (int j = 0; j < this.m_resolution; j++)
				{
					for (int k = 0; k < this.m_resolution; k++)
					{
						Vector3 vector = FurnitureDesign.RotatePoint(new Vector3((float)k, (float)j, (float)i) - new Vector3((float)this.m_resolution / 2f - 0.5f), axis, steps) + new Vector3((float)this.m_resolution / 2f - 0.5f);
						Point3 point = new Point3((int)MathUtils.Round(vector.X), (int)MathUtils.Round(vector.Y), (int)MathUtils.Round(vector.Z));
						if (point.X >= 0 && point.X < this.m_resolution && point.Y >= 0 && point.Y < this.m_resolution && point.Z >= 0 && point.Z < this.m_resolution)
						{
							array[point.X + point.Y * this.m_resolution + point.Z * this.m_resolution * this.m_resolution] = this.m_values[k + j * this.m_resolution + i * this.m_resolution * this.m_resolution];
						}
					}
				}
			}
			this.SetValues(this.m_resolution, array);
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x0009BD4C File Offset: 0x00099F4C
		public void Mirror(int axis)
		{
			int[] array = new int[this.m_resolution * this.m_resolution * this.m_resolution];
			for (int i = 0; i < this.m_resolution; i++)
			{
				for (int j = 0; j < this.m_resolution; j++)
				{
					for (int k = 0; k < this.m_resolution; k++)
					{
						Vector3 vector = FurnitureDesign.MirrorPoint(new Vector3((float)k, (float)j, (float)i) - new Vector3((float)this.m_resolution / 2f - 0.5f), axis) + new Vector3((float)this.m_resolution / 2f - 0.5f);
						Point3 point = new Point3((int)MathUtils.Round(vector.X), (int)MathUtils.Round(vector.Y), (int)MathUtils.Round(vector.Z));
						if (point.X >= 0 && point.X < this.m_resolution && point.Y >= 0 && point.Y < this.m_resolution && point.Z >= 0 && point.Z < this.m_resolution)
						{
							array[point.X + point.Y * this.m_resolution + point.Z * this.m_resolution * this.m_resolution] = this.m_values[k + j * this.m_resolution + i * this.m_resolution * this.m_resolution];
						}
					}
				}
			}
			this.SetValues(this.m_resolution, array);
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x0009BEE0 File Offset: 0x0009A0E0
		public ValuesDictionary Save()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = this.m_values[0];
			int num2 = 1;
			for (int i = 1; i < this.m_values.Length; i++)
			{
				if (this.m_values[i] != num)
				{
					stringBuilder.Append(num2.ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append('*');
					stringBuilder.Append(num.ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append(',');
					num = this.m_values[i];
					num2 = 1;
				}
				else
				{
					num2++;
				}
			}
			stringBuilder.Append(num2.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append('*');
			stringBuilder.Append(num.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append(',');
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			if (!string.IsNullOrEmpty(this.Name))
			{
				valuesDictionary.SetValue<string>("Name", this.Name);
			}
			valuesDictionary.SetValue<int>("TerrainUseCount", this.m_terrainUseCount);
			valuesDictionary.SetValue<int>("Resolution", this.m_resolution);
			valuesDictionary.SetValue<FurnitureInteractionMode>("InteractionMode", this.m_interactionMode);
			if (this.LinkedDesign != null)
			{
				valuesDictionary.SetValue<int>("LinkedDesign", this.LinkedDesign.Index);
			}
			valuesDictionary.SetValue<string>("Values", stringBuilder.ToString());
			return valuesDictionary;
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x0009C02C File Offset: 0x0009A22C
		public bool Compare(FurnitureDesign other)
		{
			if (this == other)
			{
				return true;
			}
			if (this.Resolution == other.Resolution && this.InteractionMode == other.InteractionMode && this.Hash == other.Hash && this.Name == other.Name)
			{
				for (int i = 0; i < this.m_values.Length; i++)
				{
					if (this.m_values[i] != other.m_values[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x0009C0A8 File Offset: 0x0009A2A8
		public bool CompareChain(FurnitureDesign other)
		{
			if (this == other)
			{
				return true;
			}
			List<FurnitureDesign> list = this.ListChain();
			List<FurnitureDesign> list2 = other.ListChain();
			if (list.Count != list2.Count)
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].Compare(list2[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x0009C104 File Offset: 0x0009A304
		public FurnitureDesign Clone()
		{
			FurnitureDesign furnitureDesign = new FurnitureDesign(this.m_subsystemTerrain);
			furnitureDesign.SetValues(this.Resolution, this.m_values);
			furnitureDesign.Name = this.Name;
			furnitureDesign.LinkedDesign = this.LinkedDesign;
			furnitureDesign.InteractionMode = this.InteractionMode;
			return furnitureDesign;
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x0009C154 File Offset: 0x0009A354
		public List<FurnitureDesign> CloneChain()
		{
			List<FurnitureDesign> list = this.ListChain();
			List<FurnitureDesign> list2 = new List<FurnitureDesign>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				list2.Add(list[i].Clone());
			}
			for (int j = 0; j < list2.Count - 1; j++)
			{
				list2[j].LinkedDesign = list2[j + 1];
			}
			int num = list.IndexOf(list[list.Count - 1].LinkedDesign);
			if (num >= 0)
			{
				list2[list2.Count - 1].LinkedDesign = list2[num];
			}
			return list2;
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x0009C200 File Offset: 0x0009A400
		public List<FurnitureDesign> ListChain()
		{
			FurnitureDesign furnitureDesign = this;
			HashSet<FurnitureDesign> hashSet = new HashSet<FurnitureDesign>();
			List<FurnitureDesign> list = new List<FurnitureDesign>();
			do
			{
				hashSet.Add(furnitureDesign);
				list.Add(furnitureDesign);
				furnitureDesign = furnitureDesign.LinkedDesign;
			}
			while (furnitureDesign != null && !hashSet.Contains(furnitureDesign));
			return list;
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x0009C240 File Offset: 0x0009A440
		public static List<List<FurnitureDesign>> ListChains(IEnumerable<FurnitureDesign> designs)
		{
			List<List<FurnitureDesign>> list = new List<List<FurnitureDesign>>();
			List<FurnitureDesign> list2 = new List<FurnitureDesign>(designs);
			while (list2.Count > 0)
			{
				List<FurnitureDesign> list3 = list2[0].ListChain();
				list.Add(list3);
				foreach (FurnitureDesign item in list3)
				{
					list2.Remove(item);
				}
			}
			return list;
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x0009C2C0 File Offset: 0x0009A4C0
		public byte[] CreatePrecedingEmptySpacesArray()
		{
			byte[] array = new byte[this.m_values.Length];
			int num = 0;
			for (int i = 0; i < this.Resolution; i++)
			{
				for (int j = 0; j < this.Resolution; j++)
				{
					int num2 = 0;
					int k = 0;
					while (k < this.Resolution)
					{
						num2 = ((this.m_values[num] == 0) ? (num2 + 1) : 0);
						array[num] = (byte)num2;
						k++;
						num++;
					}
				}
			}
			return array;
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0009C338 File Offset: 0x0009A538
		public Box CalculateBox(Box box, byte[] precedingEmptySpaces)
		{
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			int num3 = int.MaxValue;
			int num4 = int.MinValue;
			int num5 = int.MinValue;
			int num6 = int.MinValue;
			for (int i = box.Near; i < box.Far; i++)
			{
				int num7 = Math.Min(num3, i);
				int num8 = Math.Max(num6, i);
				int j = box.Top;
				int num9 = (j + i * this.Resolution) * this.Resolution;
				while (j < box.Bottom)
				{
					int num10 = box.Right - 1 - (int)precedingEmptySpaces[num9 + box.Right - 1];
					if (num10 >= box.Left)
					{
						num4 = Math.Max(num4, num10);
						num2 = Math.Min(num2, j);
						num5 = Math.Max(num5, j);
						num3 = num7;
						num6 = num8;
						int num11 = num - 1;
						for (int k = box.Left; k <= num11; k++)
						{
							if (this.m_values[num9 + k] != 0)
							{
								num = Math.Min(num, k);
								break;
							}
						}
					}
					j++;
					num9 += this.Resolution;
				}
			}
			return new Box(num, num2, num3, num4 - num + 1, num5 - num2 + 1, num6 - num3 + 1);
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x0009C478 File Offset: 0x0009A678
		public void CalculateShadowStrengthFactor()
		{
			float[] array = new float[this.Resolution * this.Resolution];
			int num = 0;
			for (int i = 0; i < this.Resolution; i++)
			{
				for (int j = 0; j < this.Resolution; j++)
				{
					float x = (float)(j + 1) / (float)this.Resolution;
					for (int k = 0; k < this.Resolution; k++)
					{
						if (!FurnitureDesign.IsValueTransparent(this.m_values[num++]))
						{
							array[k + i * this.Resolution] = MathUtils.Max(array[k + i * this.Resolution], x);
						}
					}
				}
			}
			float num2 = 0f;
			for (int l = 0; l < this.Resolution * this.Resolution; l++)
			{
				num2 += array[l];
			}
			num2 /= (float)(this.Resolution * this.Resolution);
			float num3 = 1.5f;
			this.m_shadowStrengthFactor = new int?((int)MathUtils.Clamp(MathUtils.Round(num2 * 3f * num3), 0f, 3f));
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x0009C588 File Offset: 0x0009A788
		public void CreateGeometry()
		{
			this.m_geometry = new FurnitureGeometry();
			for (int i = 0; i < 6; i++)
			{
				int num = CellFace.OppositeFace(i);
				Point3 point;
				Point3 point2;
				Point3 point3;
				Point3 point4;
				Point3 point5;
				switch (i)
				{
				case 0:
					point = new Point3(0, 0, 1);
					point2 = new Point3(-1, 0, 0);
					point3 = new Point3(0, -1, 0);
					point4 = new Point3(this.m_resolution, this.m_resolution, 0);
					point5 = new Point3(this.m_resolution - 1, this.m_resolution - 1, 0);
					break;
				case 1:
					point = new Point3(1, 0, 0);
					point2 = new Point3(0, 0, 1);
					point3 = new Point3(0, -1, 0);
					point4 = new Point3(0, this.m_resolution, 0);
					point5 = new Point3(0, this.m_resolution - 1, 0);
					break;
				case 2:
					point = new Point3(0, 0, -1);
					point2 = new Point3(1, 0, 0);
					point3 = new Point3(0, -1, 0);
					point4 = new Point3(0, this.m_resolution, this.m_resolution);
					point5 = new Point3(0, this.m_resolution - 1, this.m_resolution - 1);
					break;
				case 3:
					point = new Point3(-1, 0, 0);
					point2 = new Point3(0, 0, -1);
					point3 = new Point3(0, -1, 0);
					point4 = new Point3(this.m_resolution, this.m_resolution, this.m_resolution);
					point5 = new Point3(this.m_resolution - 1, this.m_resolution - 1, this.m_resolution - 1);
					break;
				case 4:
					point = new Point3(0, 1, 0);
					point2 = new Point3(-1, 0, 0);
					point3 = new Point3(0, 0, 1);
					point4 = new Point3(this.m_resolution, 0, 0);
					point5 = new Point3(this.m_resolution - 1, 0, 0);
					break;
				default:
					point = new Point3(0, -1, 0);
					point2 = new Point3(-1, 0, 0);
					point3 = new Point3(0, 0, -1);
					point4 = new Point3(this.m_resolution, this.m_resolution, this.m_resolution);
					point5 = new Point3(this.m_resolution - 1, this.m_resolution - 1, this.m_resolution - 1);
					break;
				}
				BlockMesh blockMesh = new BlockMesh();
				BlockMesh blockMesh2 = new BlockMesh();
				for (int j = 0; j < this.m_resolution; j++)
				{
					FurnitureDesign.Cell[] array = new FurnitureDesign.Cell[this.m_resolution * this.m_resolution];
					for (int k = 0; k < this.m_resolution; k++)
					{
						for (int l = 0; l < this.m_resolution; l++)
						{
							int num2 = j * point.X + k * point3.X + l * point2.X + point5.X;
							int num3 = j * point.Y + k * point3.Y + l * point2.Y + point5.Y;
							int num4 = j * point.Z + k * point3.Z + l * point2.Z + point5.Z;
							int num5 = num2 + num3 * this.m_resolution + num4 * this.m_resolution * this.m_resolution;
							int num6 = this.m_values[num5];
							FurnitureDesign.Cell cell = new FurnitureDesign.Cell
							{
								Value = num6
							};
							if (j > 0 && num6 != 0)
							{
								int num7 = num2 - point.X + (num3 - point.Y) * this.m_resolution + (num4 - point.Z) * this.m_resolution * this.m_resolution;
								int value = this.m_values[num7];
								if (!FurnitureDesign.IsValueTransparent(value) || Terrain.ExtractContents(num6) == Terrain.ExtractContents(value))
								{
									cell.Value = 0;
								}
							}
							array[l + k * this.m_resolution] = cell;
						}
					}
					for (int m = 0; m < this.m_resolution; m++)
					{
						for (int n = 0; n < this.m_resolution; n++)
						{
							int value2 = array[n + m * this.m_resolution].Value;
							if (value2 != 0)
							{
								Point2 point6 = this.FindLargestSize(array, new Point2(n, m), value2);
								if (!(point6 == Point2.Zero))
								{
									this.MarkUsed(array, new Point2(n, m), point6);
									float num8 = 0.0005f * (float)this.m_resolution;
									float num9 = (float)n - num8;
									float num10 = (float)(n + point6.X) + num8;
									float num11 = (float)m - num8;
									float num12 = (float)(m + point6.Y) + num8;
									float x = (float)(j * point.X) + num11 * (float)point3.X + num9 * (float)point2.X + (float)point4.X;
									float y = (float)(j * point.Y) + num11 * (float)point3.Y + num9 * (float)point2.Y + (float)point4.Y;
									float z = (float)(j * point.Z) + num11 * (float)point3.Z + num9 * (float)point2.Z + (float)point4.Z;
									float x2 = (float)(j * point.X) + num11 * (float)point3.X + num10 * (float)point2.X + (float)point4.X;
									float y2 = (float)(j * point.Y) + num11 * (float)point3.Y + num10 * (float)point2.Y + (float)point4.Y;
									float z2 = (float)(j * point.Z) + num11 * (float)point3.Z + num10 * (float)point2.Z + (float)point4.Z;
									float x3 = (float)(j * point.X) + num12 * (float)point3.X + num10 * (float)point2.X + (float)point4.X;
									float y3 = (float)(j * point.Y) + num12 * (float)point3.Y + num10 * (float)point2.Y + (float)point4.Y;
									float z3 = (float)(j * point.Z) + num12 * (float)point3.Z + num10 * (float)point2.Z + (float)point4.Z;
									float x4 = (float)(j * point.X) + num12 * (float)point3.X + num9 * (float)point2.X + (float)point4.X;
									float y4 = (float)(j * point.Y) + num12 * (float)point3.Y + num9 * (float)point2.Y + (float)point4.Y;
									float z4 = (float)(j * point.Z) + num12 * (float)point3.Z + num9 * (float)point2.Z + (float)point4.Z;
									BlockMesh blockMesh3 = blockMesh;
									int num13 = Terrain.ExtractContents(value2);
									Block block = BlocksManager.Blocks[num13];
									int num14 = block.GetFaceTextureSlot(i, value2);
									bool isEmissive = false;
									Color color = Color.White;
									IPaintableBlock paintableBlock = block as IPaintableBlock;
									if (paintableBlock != null)
									{
										int? paintColor = paintableBlock.GetPaintColor(value2);
										color = SubsystemPalette.GetColor(this.m_subsystemTerrain, paintColor);
									}
									else if (block is WaterBlock)
									{
										color = BlockColorsMap.WaterColorsMap.Lookup(12, 12);
										num14 = 189;
									}
									else if (block is CarpetBlock)
									{
										int color2 = CarpetBlock.GetColor(Terrain.ExtractData(value2));
										color = SubsystemPalette.GetFabricColor(this.m_subsystemTerrain, new int?(color2));
									}
									else if (block is TorchBlock || block is WickerLampBlock)
									{
										isEmissive = true;
										num14 = 31;
									}
									else if (block is GlassBlock)
									{
										blockMesh3 = blockMesh2;
									}
									int num15 = num14 % 16;
									int num16 = num14 / 16;
									int count = blockMesh3.Vertices.Count;
									blockMesh3.Vertices.Count += 4;
									BlockMeshVertex[] array2 = blockMesh3.Vertices.Array;
									float x5 = (((float)n + 0.01f) / (float)this.m_resolution + (float)num15) / 16f;
									float x6 = (((float)(n + point6.X) - 0.01f) / (float)this.m_resolution + (float)num15) / 16f;
									float y5 = (((float)m + 0.01f) / (float)this.m_resolution + (float)num16) / 16f;
									float y6 = (((float)(m + point6.Y) - 0.01f) / (float)this.m_resolution + (float)num16) / 16f;
									array2[count] = new BlockMeshVertex
									{
										Position = new Vector3(x, y, z) / (float)this.m_resolution,
										Color = color,
										Face = (byte)num,
										TextureCoordinates = new Vector2(x5, y5),
										IsEmissive = isEmissive
									};
									array2[count + 1] = new BlockMeshVertex
									{
										Position = new Vector3(x2, y2, z2) / (float)this.m_resolution,
										Color = color,
										Face = (byte)num,
										TextureCoordinates = new Vector2(x6, y5),
										IsEmissive = isEmissive
									};
									array2[count + 2] = new BlockMeshVertex
									{
										Position = new Vector3(x3, y3, z3) / (float)this.m_resolution,
										Color = color,
										Face = (byte)num,
										TextureCoordinates = new Vector2(x6, y6),
										IsEmissive = isEmissive
									};
									array2[count + 3] = new BlockMeshVertex
									{
										Position = new Vector3(x4, y4, z4) / (float)this.m_resolution,
										Color = color,
										Face = (byte)num,
										TextureCoordinates = new Vector2(x5, y6),
										IsEmissive = isEmissive
									};
									int count2 = blockMesh3.Indices.Count;
									blockMesh3.Indices.Count += 6;
									ushort[] array3 = blockMesh3.Indices.Array;
									array3[count2] = (ushort)count;
									array3[count2 + 1] = (ushort)(count + 1);
									array3[count2 + 2] = (ushort)(count + 2);
									array3[count2 + 3] = (ushort)(count + 2);
									array3[count2 + 4] = (ushort)(count + 3);
									array3[count2 + 5] = (ushort)count;
								}
							}
						}
					}
				}
				if (blockMesh.Indices.Count > 0)
				{
					blockMesh.Trim();
					blockMesh.GenerateSidesData();
					this.m_geometry.SubsetOpaqueByFace[i] = blockMesh;
				}
				if (blockMesh2.Indices.Count > 0)
				{
					blockMesh2.Trim();
					blockMesh2.GenerateSidesData();
					this.m_geometry.SubsetAlphaTestByFace[i] = blockMesh2;
				}
			}
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x0009CFF0 File Offset: 0x0009B1F0
		public void CreateCollisionAndInteractionBoxes()
		{
			FurnitureDesign.Subdivision subdivision = this.CreateBoundingBoxesHelper(this.Box, 0, this.CreatePrecedingEmptySpacesArray());
			List<BoundingBox> list = new List<BoundingBox>(subdivision.Boxes.Count);
			for (int i = 0; i < subdivision.Boxes.Count; i++)
			{
				Box box = subdivision.Boxes[i];
				Vector3 min = new Vector3((float)box.Left, (float)box.Top, (float)box.Near) / (float)this.Resolution;
				Vector3 max = new Vector3((float)box.Right, (float)box.Bottom, (float)box.Far) / (float)this.Resolution;
				list.Add(new BoundingBox(min, max));
			}
			this.m_collisionBoxesByRotation = new BoundingBox[4][];
			for (int j = 0; j < 4; j++)
			{
				Matrix m = Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationY((float)j * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_collisionBoxesByRotation[j] = new BoundingBox[list.Count];
				for (int k = 0; k < list.Count; k++)
				{
					Vector3 v = Vector3.Transform(list[k].Min, m);
					Vector3 v2 = Vector3.Transform(list[k].Max, m);
					BoundingBox boundingBox = new BoundingBox(Vector3.Min(v, v2), Vector3.Max(v, v2));
					this.m_collisionBoxesByRotation[j][k] = boundingBox;
				}
			}
			List<BoundingBox> list2 = new List<BoundingBox>(list);
			for (;;)
			{
				IL_1A2:
				for (int l = 0; l < list2.Count; l++)
				{
					for (int n = 0; n < list2.Count; n++)
					{
						if (l != n)
						{
							BoundingBox b = list2[l];
							BoundingBox b2 = list2[n];
							BoundingBox item = BoundingBox.Union(b, b2);
							Vector3 vector = item.Size();
							if ((item.Volume() - b.Volume() - b2.Volume()) / MathUtils.Min(vector.X, vector.Y, vector.Z) < 0.4f)
							{
								list2.RemoveAt(l);
								list2.RemoveAt((l < n) ? (n - 1) : n);
								list2.Add(item);
								goto IL_1A2;
							}
						}
					}
				}
				break;
			}
			bool flag = false;
			for (int num = 0; num < list2.Count; num++)
			{
				Vector3 vector2 = list2[num].Size();
				flag |= (vector2.X >= 0.6f && vector2.Y >= 0.6f);
				flag |= (vector2.X >= 0.6f && vector2.Z >= 0.6f);
				flag |= (vector2.Y >= 0.6f && vector2.Z >= 0.6f);
			}
			float minSize = flag ? 0.0625f : 0.6f;
			for (int num2 = 0; num2 < list2.Count; num2++)
			{
				BoundingBox value = list2[num2];
				FurnitureDesign.EnsureMinSize(ref value.Min.X, ref value.Max.X, minSize);
				FurnitureDesign.EnsureMinSize(ref value.Min.Y, ref value.Max.Y, minSize);
				FurnitureDesign.EnsureMinSize(ref value.Min.Z, ref value.Max.Z, minSize);
				list2[num2] = value;
			}
			this.m_interactionBoxesByRotation = new BoundingBox[4][];
			for (int num3 = 0; num3 < 4; num3++)
			{
				Matrix m2 = Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationY((float)num3 * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_interactionBoxesByRotation[num3] = new BoundingBox[list2.Count];
				for (int num4 = 0; num4 < list2.Count; num4++)
				{
					Vector3 v3 = Vector3.Transform(list2[num4].Min, m2);
					Vector3 v4 = Vector3.Transform(list2[num4].Max, m2);
					BoundingBox boundingBox2 = new BoundingBox(Vector3.Min(v3, v4), Vector3.Max(v3, v4));
					this.m_interactionBoxesByRotation[num3][num4] = boundingBox2;
				}
			}
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x0009D490 File Offset: 0x0009B690
		public void CreateTorchPoints()
		{
			List<BoundingBox> list = new List<BoundingBox>();
			for (int i = 0; i < this.Resolution; i++)
			{
				for (int j = 0; j < this.Resolution; j++)
				{
					for (int k = 0; k < this.Resolution; k++)
					{
						int num = Terrain.ExtractContents(this.m_values[k + j * this.Resolution + i * this.Resolution * this.Resolution]);
						if (num == 31 || num == 17)
						{
							BoundingBox boundingBox = new BoundingBox(new Vector3((float)k, (float)j, (float)i) / (float)this.Resolution, new Vector3((float)(k + 1), (float)(j + 1), (float)(i + 1)) / (float)this.Resolution);
							int num2 = -1;
							for (int l = 0; l < list.Count; l++)
							{
								BoundingBox boundingBox2 = list[l];
								Vector3 vector = boundingBox2.Size();
								Vector3 vector2 = boundingBox2.Center() - boundingBox.Center();
								vector2.X = MathUtils.Max(MathUtils.Abs(vector2.X) - vector.X / 2f, 0f);
								vector2.Y = MathUtils.Max(MathUtils.Abs(vector2.Y) - vector.Y / 2f, 0f);
								vector2.Z = MathUtils.Max(MathUtils.Abs(vector2.Z) - vector.Z / 2f, 0f);
								if (vector2.Length() < 0.15f)
								{
									num2 = l;
									break;
								}
							}
							if (num2 >= 0)
							{
								list[num2] = BoundingBox.Union(list[num2], boundingBox);
							}
							else if (list.Count < 4)
							{
								list.Add(boundingBox);
							}
						}
					}
				}
			}
			this.m_torchPointsByRotation = new BoundingBox[4][];
			for (int m = 0; m < 4; m++)
			{
				Matrix m2 = Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationY((float)m * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_torchPointsByRotation[m] = new BoundingBox[list.Count];
				for (int n = 0; n < list.Count; n++)
				{
					Vector3 v = Vector3.Transform(list[n].Min, m2);
					Vector3 v2 = Vector3.Transform(list[n].Max, m2);
					this.m_torchPointsByRotation[m][n] = new BoundingBox(Vector3.Min(v, v2), Vector3.Max(v, v2));
				}
			}
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x0009D74C File Offset: 0x0009B94C
		public void CalculateMainValue()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 0; i < this.Resolution; i++)
			{
				for (int j = 0; j < this.Resolution; j++)
				{
					for (int k = this.Resolution - 1; k >= 0; k--)
					{
						int num = this.m_values[j + k * this.Resolution + i * this.Resolution * this.Resolution];
						if (num != 0)
						{
							int num2;
							dictionary.TryGetValue(num, out num2);
							dictionary[num] = num2 + 1;
							break;
						}
					}
				}
			}
			int num3 = 0;
			foreach (KeyValuePair<int, int> keyValuePair in dictionary)
			{
				if (keyValuePair.Value > num3)
				{
					this.m_mainValue = keyValuePair.Key;
					num3 = keyValuePair.Value;
				}
			}
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x0009D838 File Offset: 0x0009BA38
		public void CalculateFacesMasks()
		{
			this.m_mountingFacesMask = 0;
			this.m_transparentFacesMask = 0;
			for (int i = 0; i < this.Resolution; i++)
			{
				for (int j = 0; j < this.Resolution; j++)
				{
					int[] values = this.m_values;
					int num = i + j * this.Resolution;
					int resolution = this.Resolution;
					int num2 = num;
					int resolution2 = this.Resolution;
					int value = values[num2 + 0];
					int value2 = this.m_values[i + j * this.Resolution + (this.Resolution - 1) * this.Resolution * this.Resolution];
					if (FurnitureDesign.IsValueTransparent(value))
					{
						this.m_transparentFacesMask |= 4;
					}
					else
					{
						this.m_mountingFacesMask |= 4;
					}
					if (FurnitureDesign.IsValueTransparent(value2))
					{
						this.m_transparentFacesMask |= 1;
					}
					else
					{
						this.m_mountingFacesMask |= 1;
					}
				}
			}
			for (int k = 0; k < this.Resolution; k++)
			{
				for (int l = 0; l < this.Resolution; l++)
				{
					int value3 = this.m_values[k * this.Resolution + l * this.Resolution * this.Resolution];
					int value4 = this.m_values[this.Resolution - 1 + k * this.Resolution + l * this.Resolution * this.Resolution];
					if (FurnitureDesign.IsValueTransparent(value3))
					{
						this.m_transparentFacesMask |= 8;
					}
					else
					{
						this.m_mountingFacesMask |= 8;
					}
					if (FurnitureDesign.IsValueTransparent(value4))
					{
						this.m_transparentFacesMask |= 2;
					}
					else
					{
						this.m_mountingFacesMask |= 2;
					}
				}
			}
			for (int m = 0; m < this.Resolution; m++)
			{
				for (int n = 0; n < this.Resolution; n++)
				{
					int[] values2 = this.m_values;
					int num3 = m;
					int resolution3 = this.Resolution;
					int value5 = values2[num3 + n * this.Resolution * this.Resolution];
					int value6 = this.m_values[m + (this.Resolution - 1) * this.Resolution + n * this.Resolution * this.Resolution];
					if (FurnitureDesign.IsValueTransparent(value5))
					{
						this.m_transparentFacesMask |= 32;
					}
					else
					{
						this.m_mountingFacesMask |= 32;
					}
					if (FurnitureDesign.IsValueTransparent(value6))
					{
						this.m_transparentFacesMask |= 16;
					}
					else
					{
						this.m_mountingFacesMask |= 16;
					}
				}
			}
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x0009DAC0 File Offset: 0x0009BCC0
		public FurnitureDesign.Subdivision CreateBoundingBoxesHelper(Box box, int depth, byte[] precedingEmptySpaces)
		{
			int num = 0;
			FurnitureDesign.Subdivision subdivision = default(FurnitureDesign.Subdivision);
			subdivision.TotalVolume = box.Width * box.Height * box.Depth;
			subdivision.MinVolume = subdivision.TotalVolume;
			subdivision.Boxes = new List<Box>
			{
				box
			};
			if (depth < 2)
			{
				for (int i = box.Bottom - 1; i >= box.Top + 1; i--)
				{
					Box box2 = this.CalculateBox(new Box(box.Left, box.Top, box.Near, box.Width, i - box.Top, box.Depth), precedingEmptySpaces);
					Box box3 = this.CalculateBox(new Box(box.Left, i, box.Near, box.Width, box.Bottom - i, box.Depth), precedingEmptySpaces);
					FurnitureDesign.Subdivision subdivision2 = this.CreateBoundingBoxesHelper(box2, depth + 1, precedingEmptySpaces);
					FurnitureDesign.Subdivision subdivision3 = this.CreateBoundingBoxesHelper(box3, depth + 1, precedingEmptySpaces);
					int num2 = subdivision2.Boxes.Count + subdivision3.Boxes.Count;
					int num3 = subdivision2.TotalVolume + subdivision3.TotalVolume;
					int num4 = MathUtils.Min(subdivision2.MinVolume, subdivision3.MinVolume);
					int num5 = (num2 > subdivision.Boxes.Count) ? (num3 + num) : num3;
					if (num5 < subdivision.TotalVolume || (num5 == subdivision.TotalVolume && num4 > subdivision.MinVolume))
					{
						subdivision.TotalVolume = num3;
						subdivision.MinVolume = num4;
						subdivision.Boxes = subdivision2.Boxes;
						subdivision.Boxes.AddRange(subdivision3.Boxes);
					}
				}
				for (int j = box.Near + 1; j < box.Far; j++)
				{
					Box box4 = this.CalculateBox(new Box(box.Left, box.Top, box.Near, box.Width, box.Height, j - box.Near), precedingEmptySpaces);
					Box box5 = this.CalculateBox(new Box(box.Left, box.Top, j, box.Width, box.Height, box.Far - j), precedingEmptySpaces);
					FurnitureDesign.Subdivision subdivision4 = this.CreateBoundingBoxesHelper(box4, depth + 1, precedingEmptySpaces);
					FurnitureDesign.Subdivision subdivision5 = this.CreateBoundingBoxesHelper(box5, depth + 1, precedingEmptySpaces);
					int num6 = subdivision4.Boxes.Count + subdivision5.Boxes.Count;
					int num7 = subdivision4.TotalVolume + subdivision5.TotalVolume;
					int num8 = MathUtils.Min(subdivision4.MinVolume, subdivision5.MinVolume);
					int num9 = (num6 > subdivision.Boxes.Count) ? (num7 + num) : num7;
					if (num9 < subdivision.TotalVolume || (num9 == subdivision.TotalVolume && num8 > subdivision.MinVolume))
					{
						subdivision.TotalVolume = num7;
						subdivision.MinVolume = num8;
						subdivision.Boxes = subdivision4.Boxes;
						subdivision.Boxes.AddRange(subdivision5.Boxes);
					}
				}
				for (int k = box.Left + 1; k < box.Right; k++)
				{
					Box box6 = this.CalculateBox(new Box(box.Left, box.Top, box.Near, k - box.Left, box.Height, box.Depth), precedingEmptySpaces);
					Box box7 = this.CalculateBox(new Box(k, box.Top, box.Near, box.Right - k, box.Height, box.Depth), precedingEmptySpaces);
					FurnitureDesign.Subdivision subdivision6 = this.CreateBoundingBoxesHelper(box6, depth + 1, precedingEmptySpaces);
					FurnitureDesign.Subdivision subdivision7 = this.CreateBoundingBoxesHelper(box7, depth + 1, precedingEmptySpaces);
					int num10 = subdivision6.Boxes.Count + subdivision7.Boxes.Count;
					int num11 = subdivision6.TotalVolume + subdivision7.TotalVolume;
					int num12 = MathUtils.Min(subdivision6.MinVolume, subdivision7.MinVolume);
					int num13 = (num10 > subdivision.Boxes.Count) ? (num11 + num) : num11;
					if (num13 < subdivision.TotalVolume || (num13 == subdivision.TotalVolume && num12 > subdivision.MinVolume))
					{
						subdivision.TotalVolume = num11;
						subdivision.MinVolume = num12;
						subdivision.Boxes = subdivision6.Boxes;
						subdivision.Boxes.AddRange(subdivision7.Boxes);
					}
				}
			}
			return subdivision;
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x0009DEF8 File Offset: 0x0009C0F8
		public Point2 FindLargestSize(FurnitureDesign.Cell[] surface, Point2 start, int value)
		{
			Point2 point = Point2.Zero;
			int num = this.m_resolution;
			for (int i = start.Y; i < this.m_resolution; i++)
			{
				for (int j = start.X; j <= num; j++)
				{
					if (j == num || surface[j + i * this.m_resolution].Value != value)
					{
						num = j;
						Point2 point2 = new Point2(num - start.X, i - start.Y + 1);
						if (point2.X * point2.Y > point.X * point.Y)
						{
							point = point2;
						}
					}
				}
			}
			return point;
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x0009DF94 File Offset: 0x0009C194
		public void MarkUsed(FurnitureDesign.Cell[] surface, Point2 start, Point2 size)
		{
			for (int i = start.Y; i < start.Y + size.Y; i++)
			{
				for (int j = start.X; j < start.X + size.X; j++)
				{
					surface[j + i * this.m_resolution].Value = 0;
				}
			}
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x0009DFF4 File Offset: 0x0009C1F4
		public static Vector3 RotatePoint(Vector3 p, int axis, int steps)
		{
			for (int i = 0; i < steps; i++)
			{
				if (axis != 0)
				{
					if (axis != 1)
					{
						p = new Vector3(0f - p.Y, p.X, p.Z);
					}
					else
					{
						p = new Vector3(0f - p.Z, p.Y, p.X);
					}
				}
				else
				{
					p = new Vector3(p.X, p.Z, 0f - p.Y);
				}
			}
			return p;
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x0009E078 File Offset: 0x0009C278
		public static Vector3 MirrorPoint(Vector3 p, int axis)
		{
			if (axis != 0)
			{
				if (axis != 1)
				{
					p = new Vector3(0f - p.X, p.Y, p.Z);
				}
				else
				{
					p = new Vector3(0f - p.X, p.Y, p.Z);
				}
			}
			else
			{
				p = new Vector3(p.X, p.Y, 0f - p.Z);
			}
			return p;
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x0009E0F0 File Offset: 0x0009C2F0
		public static void EnsureMinSize(ref float min, ref float max, float minSize)
		{
			float num = max - min;
			if (num < minSize)
			{
				float num2 = minSize - num;
				min -= num2 / 2f;
				max += num2 / 2f;
				if (min < 0f)
				{
					max -= min;
					min = 0f;
					return;
				}
				if (max > 1f)
				{
					min -= max - 1f;
					max = 1f;
				}
			}
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x0009E158 File Offset: 0x0009C358
		public static bool IsValueTransparent(int value)
		{
			return value == 0 || Terrain.ExtractContents(value) == 15;
		}

		// Token: 0x04000D81 RID: 3457
		public static string fName = "FurnitureDesign";

		// Token: 0x04000D82 RID: 3458
		public const int MinResolution = 2;

		// Token: 0x04000D83 RID: 3459
		public const int maxDesign = 8192;

		// Token: 0x04000D84 RID: 3460
		public const int MaxResolution = 128;

		// Token: 0x04000D85 RID: 3461
		public const int MaxTriangles = 300;

		// Token: 0x04000D86 RID: 3462
		public const int MaxNameLength = 20;

		// Token: 0x04000D87 RID: 3463
		public int m_index = -1;

		// Token: 0x04000D88 RID: 3464
		public string m_name = string.Empty;

		// Token: 0x04000D89 RID: 3465
		public FurnitureSet m_furnitureSet;

		// Token: 0x04000D8A RID: 3466
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000D8B RID: 3467
		public bool m_gcUsed;

		// Token: 0x04000D8C RID: 3468
		public int m_terrainUseCount;

		// Token: 0x04000D8D RID: 3469
		public int m_loadTimeLinkedDesignIndex = -1;

		// Token: 0x04000D8E RID: 3470
		public int m_resolution;

		// Token: 0x04000D8F RID: 3471
		public int[] m_values;

		// Token: 0x04000D90 RID: 3472
		public int? m_hash;

		// Token: 0x04000D91 RID: 3473
		public FurnitureGeometry m_geometry;

		// Token: 0x04000D92 RID: 3474
		public Box? m_box;

		// Token: 0x04000D93 RID: 3475
		public int? m_shadowStrengthFactor;

		// Token: 0x04000D94 RID: 3476
		public BoundingBox[][] m_collisionBoxesByRotation;

		// Token: 0x04000D95 RID: 3477
		public BoundingBox[][] m_interactionBoxesByRotation;

		// Token: 0x04000D96 RID: 3478
		public BoundingBox[][] m_torchPointsByRotation;

		// Token: 0x04000D97 RID: 3479
		public int m_mainValue;

		// Token: 0x04000D98 RID: 3480
		public int m_mountingFacesMask = -1;

		// Token: 0x04000D99 RID: 3481
		public int m_transparentFacesMask = -1;

		// Token: 0x04000D9A RID: 3482
		public FurnitureDesign m_linkedDesign;

		// Token: 0x04000D9B RID: 3483
		public FurnitureInteractionMode m_interactionMode;

		// Token: 0x02000510 RID: 1296
		public struct Cell
		{
			// Token: 0x04001852 RID: 6226
			public int Value;
		}

		// Token: 0x02000511 RID: 1297
		public struct Subdivision
		{
			// Token: 0x04001853 RID: 6227
			public int TotalVolume;

			// Token: 0x04001854 RID: 6228
			public int MinVolume;

			// Token: 0x04001855 RID: 6229
			public List<Box> Boxes;
		}
	}
}
