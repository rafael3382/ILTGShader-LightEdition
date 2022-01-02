using System;
using System.Collections.Generic;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200019D RID: 413
	public abstract class SubsystemEditableItemBehavior<T> : SubsystemBlockBehavior where T : IEditableItemData, new()
	{
		// Token: 0x06000A2B RID: 2603 RVA: 0x000416FE File Offset: 0x0003F8FE
		public SubsystemEditableItemBehavior(int contents)
		{
			this.m_contents = contents;
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x00041724 File Offset: 0x0003F924
		public T GetBlockData(Point3 point)
		{
			T result;
			this.m_blocksData.TryGetValue(point, out result);
			return result;
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x00041741 File Offset: 0x0003F941
		public void SetBlockData(Point3 point, T t)
		{
			if (t != null)
			{
				this.m_blocksData[point] = t;
				return;
			}
			this.m_blocksData.Remove(point);
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x00041768 File Offset: 0x0003F968
		public T GetItemData(int id)
		{
			T result;
			this.m_itemsData.TryGetValue(id, out result);
			return result;
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x00041788 File Offset: 0x0003F988
		public int StoreItemDataAtUniqueId(T t)
		{
			int num = this.FindFreeItemId();
			this.m_itemsData[num] = t;
			return num;
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x000417AC File Offset: 0x0003F9AC
		public override void OnItemPlaced(int x, int y, int z, ref BlockPlacementData placementData, int itemValue)
		{
			int id = Terrain.ExtractData(itemValue);
			T itemData = this.GetItemData(id);
			if (itemData != null)
			{
				this.m_blocksData[new Point3(x, y, z)] = (T)((object)itemData.Copy());
			}
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x000417F8 File Offset: 0x0003F9F8
		public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			T blockData = this.GetBlockData(new Point3(x, y, z));
			if (blockData != null)
			{
				int num = this.FindFreeItemId();
				this.m_itemsData.Add(num, (T)((object)blockData.Copy()));
				dropValue.Value = Terrain.ReplaceData(dropValue.Value, num);
			}
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x00041855 File Offset: 0x0003FA55
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.m_blocksData.Remove(new Point3(x, y, z));
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x00041870 File Offset: 0x0003FA70
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemItemsScanner = base.Project.FindSubsystem<SubsystemItemsScanner>(true);
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("Blocks"))
			{
				Point3 key = HumanReadableConverter.ConvertFromString<Point3>(keyValuePair.Key);
				T value = Activator.CreateInstance<T>();
				value.LoadString((string)keyValuePair.Value);
				this.m_blocksData[key] = value;
			}
			foreach (KeyValuePair<string, object> keyValuePair2 in valuesDictionary.GetValue<ValuesDictionary>("Items"))
			{
				int key2 = HumanReadableConverter.ConvertFromString<int>(keyValuePair2.Key);
				T value2 = Activator.CreateInstance<T>();
				value2.LoadString((string)keyValuePair2.Value);
				this.m_itemsData[key2] = value2;
			}
			SubsystemItemsScanner subsystemItemsScanner = this.m_subsystemItemsScanner;
			subsystemItemsScanner.ItemsScanned = (Action<ReadOnlyList<ScannedItemData>>)Delegate.Combine(subsystemItemsScanner.ItemsScanned, new Action<ReadOnlyList<ScannedItemData>>(this.GarbageCollectItems));
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x000419AC File Offset: 0x0003FBAC
		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Blocks", valuesDictionary2);
			foreach (KeyValuePair<Point3, T> keyValuePair in this.m_blocksData)
			{
				ValuesDictionary valuesDictionary3 = valuesDictionary2;
				string key = HumanReadableConverter.ConvertToString(keyValuePair.Key);
				T value = keyValuePair.Value;
				valuesDictionary3.SetValue<string>(key, value.SaveString());
			}
			ValuesDictionary valuesDictionary4 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Items", valuesDictionary4);
			foreach (KeyValuePair<int, T> keyValuePair2 in this.m_itemsData)
			{
				ValuesDictionary valuesDictionary5 = valuesDictionary4;
				string key2 = HumanReadableConverter.ConvertToString(keyValuePair2.Key);
				T value = keyValuePair2.Value;
				valuesDictionary5.SetValue<string>(key2, value.SaveString());
			}
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x00041ABC File Offset: 0x0003FCBC
		public int FindFreeItemId()
		{
			for (int i = 1; i < 1000; i++)
			{
				if (!this.m_itemsData.ContainsKey(i))
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x00041AEC File Offset: 0x0003FCEC
		public void GarbageCollectItems(ReadOnlyList<ScannedItemData> allExistingItems)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (ScannedItemData scannedItemData in allExistingItems)
			{
				if (Terrain.ExtractContents(scannedItemData.Value) == this.m_contents)
				{
					hashSet.Add(Terrain.ExtractData(scannedItemData.Value));
				}
			}
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, T> keyValuePair in this.m_itemsData)
			{
				if (!hashSet.Contains(keyValuePair.Key))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (int key in list)
			{
				this.m_itemsData.Remove(key);
			}
		}

		// Token: 0x040004E6 RID: 1254
		public SubsystemItemsScanner m_subsystemItemsScanner;

		// Token: 0x040004E7 RID: 1255
		public int m_contents;

		// Token: 0x040004E8 RID: 1256
		public Dictionary<int, T> m_itemsData = new Dictionary<int, T>();

		// Token: 0x040004E9 RID: 1257
		public Dictionary<Point3, T> m_blocksData = new Dictionary<Point3, T>();
	}
}
