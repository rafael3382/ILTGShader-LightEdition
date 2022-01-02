using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A0 RID: 416
	public class SubsystemElectricity : Subsystem, IUpdateable
	{
		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000A47 RID: 2631 RVA: 0x00041F58 File Offset: 0x00040158
		// (set) Token: 0x06000A48 RID: 2632 RVA: 0x00041F60 File Offset: 0x00040160
		public SubsystemTime SubsystemTime { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000A49 RID: 2633 RVA: 0x00041F69 File Offset: 0x00040169
		// (set) Token: 0x06000A4A RID: 2634 RVA: 0x00041F71 File Offset: 0x00040171
		public SubsystemTerrain SubsystemTerrain { get; set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000A4B RID: 2635 RVA: 0x00041F7A File Offset: 0x0004017A
		// (set) Token: 0x06000A4C RID: 2636 RVA: 0x00041F82 File Offset: 0x00040182
		public SubsystemAudio SubsystemAudio { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000A4D RID: 2637 RVA: 0x00041F8B File Offset: 0x0004018B
		// (set) Token: 0x06000A4E RID: 2638 RVA: 0x00041F93 File Offset: 0x00040193
		public int FrameStartCircuitStep { get; set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000A4F RID: 2639 RVA: 0x00041F9C File Offset: 0x0004019C
		// (set) Token: 0x06000A50 RID: 2640 RVA: 0x00041FA4 File Offset: 0x000401A4
		public int CircuitStep { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000A51 RID: 2641 RVA: 0x00041FAD File Offset: 0x000401AD
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x00041FB0 File Offset: 0x000401B0
		public void OnElectricElementBlockGenerated(int x, int y, int z)
		{
			this.m_pointsToUpdate[new Point3(x, y, z)] = false;
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x00041FC6 File Offset: 0x000401C6
		public void OnElectricElementBlockAdded(int x, int y, int z)
		{
			this.m_pointsToUpdate[new Point3(x, y, z)] = true;
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x00041FDC File Offset: 0x000401DC
		public void OnElectricElementBlockRemoved(int x, int y, int z)
		{
			this.m_pointsToUpdate[new Point3(x, y, z)] = true;
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x00041FF2 File Offset: 0x000401F2
		public void OnElectricElementBlockModified(int x, int y, int z)
		{
			this.m_pointsToUpdate[new Point3(x, y, z)] = true;
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x00042008 File Offset: 0x00040208
		public void OnChunkDiscarding(TerrainChunk chunk)
		{
			foreach (CellFace cellFace in this.m_electricElementsByCellFace.Keys)
			{
				if (cellFace.X >= chunk.Origin.X && cellFace.X < chunk.Origin.X + 16 && cellFace.Z >= chunk.Origin.Y && cellFace.Z < chunk.Origin.Y + 16)
				{
					this.m_pointsToUpdate[new Point3(cellFace.X, cellFace.Y, cellFace.Z)] = false;
				}
			}
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x000420D4 File Offset: 0x000402D4
		public static ElectricConnectorDirection? GetConnectorDirection(int mountingFace, int rotation, int connectorFace)
		{
			ElectricConnectorDirection? result = SubsystemElectricity.m_connectorDirectionsTable[6 * mountingFace + connectorFace];
			if (result == null)
			{
				return null;
			}
			if (result.Value < ElectricConnectorDirection.In)
			{
				return new ElectricConnectorDirection?((result.Value + rotation) % ElectricConnectorDirection.In);
			}
			return result;
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x00042120 File Offset: 0x00040320
		public static int GetConnectorFace(int mountingFace, ElectricConnectorDirection connectionDirection)
		{
			return SubsystemElectricity.m_connectorFacesTable[(int)(5 * mountingFace + connectionDirection)];
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x00042130 File Offset: 0x00040330
		public void GetAllConnectedNeighbors(int x, int y, int z, int mountingFace, DynamicArray<ElectricConnectionPath> list)
		{
			int cellValue = this.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			IElectricElementBlock electricElementBlock = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)] as IElectricElementBlock;
			if (electricElementBlock == null)
			{
				return;
			}
			for (ElectricConnectorDirection electricConnectorDirection = ElectricConnectorDirection.Top; electricConnectorDirection < (ElectricConnectorDirection)5; electricConnectorDirection++)
			{
				for (int i = 0; i < 4; i++)
				{
					ElectricConnectionPath electricConnectionPath = SubsystemElectricity.m_connectionPathsTable[(int)(20 * mountingFace + ElectricConnectorDirection.In * electricConnectorDirection + i)];
					if (electricConnectionPath == null)
					{
						break;
					}
					ElectricConnectorType? connectorType = electricElementBlock.GetConnectorType(this.SubsystemTerrain, cellValue, mountingFace, electricConnectionPath.ConnectorFace, x, y, z);
					if (connectorType == null)
					{
						break;
					}
					int x2 = x + electricConnectionPath.NeighborOffsetX;
					int y2 = y + electricConnectionPath.NeighborOffsetY;
					int z2 = z + electricConnectionPath.NeighborOffsetZ;
					int cellValue2 = this.SubsystemTerrain.Terrain.GetCellValue(x2, y2, z2);
					IElectricElementBlock electricElementBlock2 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)] as IElectricElementBlock;
					if (electricElementBlock2 != null)
					{
						ElectricConnectorType? connectorType2 = electricElementBlock2.GetConnectorType(this.SubsystemTerrain, cellValue2, electricConnectionPath.NeighborFace, electricConnectionPath.NeighborConnectorFace, x2, y2, z2);
						if (connectorType2 != null && ((connectorType.Value != ElectricConnectorType.Input && connectorType2.Value != ElectricConnectorType.Output) || (connectorType.Value != ElectricConnectorType.Output && connectorType2.Value != ElectricConnectorType.Input)))
						{
							bool connectionMask = electricElementBlock.GetConnectionMask(cellValue) != 0;
							int connectionMask2 = electricElementBlock2.GetConnectionMask(cellValue2);
							if (((connectionMask ? 1 : 0) & connectionMask2) != 0)
							{
								list.Add(electricConnectionPath);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x00042294 File Offset: 0x00040494
		public ElectricElement GetElectricElement(int x, int y, int z, int mountingFace)
		{
			ElectricElement result;
			this.m_electricElementsByCellFace.TryGetValue(new CellFace(x, y, z, mountingFace), out result);
			return result;
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x000422BC File Offset: 0x000404BC
		public void QueueElectricElementForSimulation(ElectricElement electricElement, int circuitStep)
		{
			if (circuitStep == this.CircuitStep + 1)
			{
				if (this.m_nextStepSimulateList == null && !this.m_futureSimulateLists.TryGetValue(this.CircuitStep + 1, out this.m_nextStepSimulateList))
				{
					this.m_nextStepSimulateList = this.GetListFromCache();
					this.m_futureSimulateLists.Add(this.CircuitStep + 1, this.m_nextStepSimulateList);
				}
				this.m_nextStepSimulateList[electricElement] = true;
				return;
			}
			if (circuitStep > this.CircuitStep + 1)
			{
				Dictionary<ElectricElement, bool> listFromCache;
				if (!this.m_futureSimulateLists.TryGetValue(circuitStep, out listFromCache))
				{
					listFromCache = this.GetListFromCache();
					this.m_futureSimulateLists.Add(circuitStep, listFromCache);
				}
				listFromCache[electricElement] = true;
			}
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x00042364 File Offset: 0x00040564
		public void QueueElectricElementConnectionsForSimulation(ElectricElement electricElement, int circuitStep)
		{
			foreach (ElectricConnection electricConnection in electricElement.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Input && electricConnection.NeighborConnectorType != ElectricConnectorType.Output)
				{
					this.QueueElectricElementForSimulation(electricConnection.NeighborElectricElement, circuitStep);
				}
			}
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x000423D0 File Offset: 0x000405D0
		public float? ReadPersistentVoltage(Point3 point)
		{
			float value;
			if (this.m_persistentElementsVoltages.TryGetValue(point, out value))
			{
				return new float?(value);
			}
			return null;
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x000423FD File Offset: 0x000405FD
		public void WritePersistentVoltage(Point3 point, float voltage)
		{
			this.m_persistentElementsVoltages[point] = voltage;
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x0004240C File Offset: 0x0004060C
		public void Update(float dt)
		{
			this.FrameStartCircuitStep = this.CircuitStep;
			SubsystemElectricity.SimulatedElectricElements = 0;
			this.m_remainingSimulationTime = MathUtils.Min(this.m_remainingSimulationTime + dt, 0.1f);
			while (this.m_remainingSimulationTime >= 0.01f)
			{
				this.UpdateElectricElements();
				int circuitStep = this.CircuitStep + 1;
				this.CircuitStep = circuitStep;
				this.m_remainingSimulationTime -= 0.01f;
				this.m_nextStepSimulateList = null;
				Dictionary<ElectricElement, bool> dictionary;
				if (this.m_futureSimulateLists.TryGetValue(this.CircuitStep, out dictionary))
				{
					this.m_futureSimulateLists.Remove(this.CircuitStep);
					SubsystemElectricity.SimulatedElectricElements += dictionary.Count;
					foreach (ElectricElement electricElement in dictionary.Keys)
					{
						if (this.m_electricElements.ContainsKey(electricElement))
						{
							this.SimulateElectricElement(electricElement);
						}
					}
					this.ReturnListToCache(dictionary);
				}
			}
			if (SubsystemElectricity.DebugDrawElectrics)
			{
				this.DebugDraw();
			}
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x00042528 File Offset: 0x00040728
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.SubsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.SubsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.SubsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			string[] array = valuesDictionary.GetValue<string>("VoltagesByCell").Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new string[]
				{
					","
				}, StringSplitOptions.None);
				if (array2.Length != 4)
				{
					throw new InvalidOperationException("Invalid number of tokens.");
				}
				int x = int.Parse(array2[0], CultureInfo.InvariantCulture);
				int y = int.Parse(array2[1], CultureInfo.InvariantCulture);
				int z = int.Parse(array2[2], CultureInfo.InvariantCulture);
				float value = float.Parse(array2[3], CultureInfo.InvariantCulture);
				this.m_persistentElementsVoltages[new Point3(x, y, z)] = value;
			}
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x00042614 File Offset: 0x00040814
		public override void Save(ValuesDictionary valuesDictionary)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<Point3, float> keyValuePair in this.m_persistentElementsVoltages)
			{
				if (num > 500)
				{
					break;
				}
				StringBuilder stringBuilder2 = stringBuilder;
				Point3 key = keyValuePair.Key;
				stringBuilder2.Append(key.X.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				StringBuilder stringBuilder3 = stringBuilder;
				key = keyValuePair.Key;
				stringBuilder3.Append(key.Y.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				StringBuilder stringBuilder4 = stringBuilder;
				key = keyValuePair.Key;
				stringBuilder4.Append(key.Z.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(keyValuePair.Value.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(';');
				num++;
			}
			valuesDictionary.SetValue<string>("VoltagesByCell", stringBuilder.ToString());
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x00042734 File Offset: 0x00040934
		public static ElectricConnectionPath GetConnectionPath(int mountingFace, ElectricConnectorDirection localConnector, int neighborIndex)
		{
			return SubsystemElectricity.m_connectionPathsTable[(int)(16 * mountingFace + ElectricConnectorDirection.In * localConnector + neighborIndex)];
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x00042746 File Offset: 0x00040946
		public void SimulateElectricElement(ElectricElement electricElement)
		{
			if (electricElement.Simulate())
			{
				this.QueueElectricElementConnectionsForSimulation(electricElement, this.CircuitStep + 1);
			}
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x00042760 File Offset: 0x00040960
		public void AddElectricElement(ElectricElement electricElement)
		{
			this.m_electricElements.Add(electricElement, true);
			foreach (CellFace cellFace in electricElement.CellFaces)
			{
				this.m_electricElementsByCellFace.Add(cellFace, electricElement);
				this.m_tmpConnectionPaths.Clear();
				this.GetAllConnectedNeighbors(cellFace.X, cellFace.Y, cellFace.Z, cellFace.Face, this.m_tmpConnectionPaths);
				foreach (ElectricConnectionPath electricConnectionPath in this.m_tmpConnectionPaths)
				{
					CellFace cellFace2 = new CellFace(cellFace.X + electricConnectionPath.NeighborOffsetX, cellFace.Y + electricConnectionPath.NeighborOffsetY, cellFace.Z + electricConnectionPath.NeighborOffsetZ, electricConnectionPath.NeighborFace);
					ElectricElement electricElement2;
					if (this.m_electricElementsByCellFace.TryGetValue(cellFace2, out electricElement2) && electricElement2 != electricElement)
					{
						int cellValue = this.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
						int num = Terrain.ExtractContents(cellValue);
						ElectricConnectorType value = ((IElectricElementBlock)BlocksManager.Blocks[num]).GetConnectorType(this.SubsystemTerrain, cellValue, cellFace.Face, electricConnectionPath.ConnectorFace, cellFace.X, cellFace.Y, cellFace.Z).Value;
						int cellValue2 = this.SubsystemTerrain.Terrain.GetCellValue(cellFace2.X, cellFace2.Y, cellFace2.Z);
						int num2 = Terrain.ExtractContents(cellValue2);
						ElectricConnectorType value2 = ((IElectricElementBlock)BlocksManager.Blocks[num2]).GetConnectorType(this.SubsystemTerrain, cellValue2, cellFace2.Face, electricConnectionPath.NeighborConnectorFace, cellFace2.X, cellFace2.Y, cellFace2.Z).Value;
						electricElement.Connections.Add(new ElectricConnection
						{
							CellFace = cellFace,
							ConnectorFace = electricConnectionPath.ConnectorFace,
							ConnectorType = value,
							NeighborElectricElement = electricElement2,
							NeighborCellFace = cellFace2,
							NeighborConnectorFace = electricConnectionPath.NeighborConnectorFace,
							NeighborConnectorType = value2
						});
						electricElement2.Connections.Add(new ElectricConnection
						{
							CellFace = cellFace2,
							ConnectorFace = electricConnectionPath.NeighborConnectorFace,
							ConnectorType = value2,
							NeighborElectricElement = electricElement,
							NeighborCellFace = cellFace,
							NeighborConnectorFace = electricConnectionPath.ConnectorFace,
							NeighborConnectorType = value
						});
					}
				}
			}
			this.QueueElectricElementForSimulation(electricElement, this.CircuitStep + 1);
			this.QueueElectricElementConnectionsForSimulation(electricElement, this.CircuitStep + 2);
			electricElement.OnAdded();
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x00042A5C File Offset: 0x00040C5C
		public void RemoveElectricElement(ElectricElement electricElement)
		{
			electricElement.OnRemoved();
			this.QueueElectricElementConnectionsForSimulation(electricElement, this.CircuitStep + 1);
			this.m_electricElements.Remove(electricElement);
			foreach (CellFace key in electricElement.CellFaces)
			{
				this.m_electricElementsByCellFace.Remove(key);
			}
			Func<ElectricConnection, bool> <>9__0;
			foreach (ElectricConnection electricConnection in electricElement.Connections)
			{
				IEnumerable<ElectricConnection> connections = electricConnection.NeighborElectricElement.Connections;
				Func<ElectricConnection, bool> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((ElectricConnection c) => c.NeighborElectricElement == electricElement));
				}
				int num = connections.FirstIndex(predicate);
				if (num >= 0)
				{
					electricConnection.NeighborElectricElement.Connections.RemoveAt(num);
				}
			}
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x00042B8C File Offset: 0x00040D8C
		public void UpdateElectricElements()
		{
			foreach (KeyValuePair<Point3, bool> keyValuePair in this.m_pointsToUpdate)
			{
				Point3 key = keyValuePair.Key;
				int cellValue = this.SubsystemTerrain.Terrain.GetCellValue(key.X, key.Y, key.Z);
				for (int i = 0; i < 6; i++)
				{
					ElectricElement electricElement = this.GetElectricElement(key.X, key.Y, key.Z, i);
					if (electricElement != null)
					{
						if (electricElement is WireDomainElectricElement)
						{
							this.m_wiresToUpdate[key] = true;
						}
						else
						{
							this.m_electricElementsToRemove[electricElement] = true;
						}
					}
				}
				if (keyValuePair.Value)
				{
					this.m_persistentElementsVoltages.Remove(key);
				}
				int num = Terrain.ExtractContents(cellValue);
				if (BlocksManager.Blocks[num] is IElectricWireElementBlock)
				{
					this.m_wiresToUpdate[key] = true;
				}
				else
				{
					IElectricElementBlock electricElementBlock = BlocksManager.Blocks[num] as IElectricElementBlock;
					if (electricElementBlock != null)
					{
						ElectricElement electricElement2 = electricElementBlock.CreateElectricElement(this, cellValue, key.X, key.Y, key.Z);
						if (electricElement2 != null)
						{
							this.m_electricElementsToAdd[key] = electricElement2;
						}
					}
				}
			}
			this.RemoveWireDomains();
			foreach (KeyValuePair<ElectricElement, bool> keyValuePair2 in this.m_electricElementsToRemove)
			{
				this.RemoveElectricElement(keyValuePair2.Key);
			}
			this.AddWireDomains();
			foreach (ElectricElement electricElement3 in this.m_electricElementsToAdd.Values)
			{
				this.AddElectricElement(electricElement3);
			}
			this.m_pointsToUpdate.Clear();
			this.m_wiresToUpdate.Clear();
			this.m_electricElementsToAdd.Clear();
			this.m_electricElementsToRemove.Clear();
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x00042DD0 File Offset: 0x00040FD0
		public void AddWireDomains()
		{
			this.m_tmpVisited.Clear();
			foreach (Point3 point in this.m_wiresToUpdate.Keys)
			{
				for (int i = point.X - 1; i <= point.X + 1; i++)
				{
					for (int j = point.Y - 1; j <= point.Y + 1; j++)
					{
						for (int k = point.Z - 1; k <= point.Z + 1; k++)
						{
							for (int l = 0; l < 6; l++)
							{
								this.m_tmpResult.Clear();
								this.ScanWireDomain(new CellFace(i, j, k, l), this.m_tmpVisited, this.m_tmpResult);
								if (this.m_tmpResult.Count > 0)
								{
									WireDomainElectricElement electricElement = new WireDomainElectricElement(this, this.m_tmpResult.Keys);
									this.AddElectricElement(electricElement);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x00042EF4 File Offset: 0x000410F4
		public void RemoveWireDomains()
		{
			foreach (Point3 point in this.m_wiresToUpdate.Keys)
			{
				for (int i = point.X - 1; i <= point.X + 1; i++)
				{
					for (int j = point.Y - 1; j <= point.Y + 1; j++)
					{
						for (int k = point.Z - 1; k <= point.Z + 1; k++)
						{
							for (int l = 0; l < 6; l++)
							{
								ElectricElement electricElement;
								if (this.m_electricElementsByCellFace.TryGetValue(new CellFace(i, j, k, l), out electricElement) && electricElement is WireDomainElectricElement)
								{
									this.RemoveElectricElement(electricElement);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x00042FDC File Offset: 0x000411DC
		public void ScanWireDomain(CellFace startCellFace, Dictionary<CellFace, bool> visited, Dictionary<CellFace, bool> result)
		{
			DynamicArray<CellFace> dynamicArray = new DynamicArray<CellFace>();
			dynamicArray.Add(startCellFace);
			while (dynamicArray.Count > 0)
			{
				CellFace[] array = dynamicArray.Array;
				DynamicArray<CellFace> dynamicArray2 = dynamicArray;
				int num = dynamicArray2.Count - 1;
				dynamicArray2.Count = num;
				CellFace cellFace = array[num];
				if (!visited.ContainsKey(cellFace))
				{
					TerrainChunk chunkAtCell = this.SubsystemTerrain.Terrain.GetChunkAtCell(cellFace.X, cellFace.Z);
					if (chunkAtCell != null && chunkAtCell.AreBehaviorsNotified)
					{
						int cellValue = this.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
						int num2 = Terrain.ExtractContents(cellValue);
						IElectricWireElementBlock electricWireElementBlock = BlocksManager.Blocks[num2] as IElectricWireElementBlock;
						if (electricWireElementBlock != null)
						{
							int connectedWireFacesMask = electricWireElementBlock.GetConnectedWireFacesMask(cellValue, cellFace.Face);
							if (connectedWireFacesMask != 0)
							{
								for (int i = 0; i < 6; i++)
								{
									if ((connectedWireFacesMask & 1 << i) != 0)
									{
										CellFace cellFace2 = new CellFace(cellFace.X, cellFace.Y, cellFace.Z, i);
										visited.Add(cellFace2, true);
										result.Add(cellFace2, true);
										this.m_tmpConnectionPaths.Clear();
										this.GetAllConnectedNeighbors(cellFace2.X, cellFace2.Y, cellFace2.Z, cellFace2.Face, this.m_tmpConnectionPaths);
										foreach (ElectricConnectionPath electricConnectionPath in this.m_tmpConnectionPaths)
										{
											int x = cellFace2.X + electricConnectionPath.NeighborOffsetX;
											int y = cellFace2.Y + electricConnectionPath.NeighborOffsetY;
											int z = cellFace2.Z + electricConnectionPath.NeighborOffsetZ;
											dynamicArray.Add(new CellFace(x, y, z, electricConnectionPath.NeighborFace));
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x000431CC File Offset: 0x000413CC
		public Dictionary<ElectricElement, bool> GetListFromCache()
		{
			if (this.m_listsCache.Count > 0)
			{
				Dictionary<ElectricElement, bool> result = this.m_listsCache[this.m_listsCache.Count - 1];
				this.m_listsCache.RemoveAt(this.m_listsCache.Count - 1);
				return result;
			}
			return new Dictionary<ElectricElement, bool>();
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x0004321D File Offset: 0x0004141D
		public void ReturnListToCache(Dictionary<ElectricElement, bool> list)
		{
			list.Clear();
			this.m_listsCache.Add(list);
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x00043231 File Offset: 0x00041431
		public void DebugDraw()
		{
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x000432CC File Offset: 0x000414CC
		// Note: this type is marked as 'beforefieldinit'.
		static SubsystemElectricity()
		{
			ElectricConnectionPath[] array = new ElectricConnectionPath[120];
			array[0] = new ElectricConnectionPath(0, 1, -1, 4, 4, 0);
			array[1] = new ElectricConnectionPath(0, 1, 0, 0, 4, 5);
			array[2] = new ElectricConnectionPath(0, 1, -1, 2, 4, 5);
			array[3] = new ElectricConnectionPath(0, 0, 0, 5, 4, 2);
			array[4] = new ElectricConnectionPath(-1, 0, -1, 3, 3, 0);
			array[5] = new ElectricConnectionPath(-1, 0, 0, 0, 3, 1);
			array[6] = new ElectricConnectionPath(-1, 0, -1, 2, 3, 1);
			array[7] = new ElectricConnectionPath(0, 0, 0, 1, 3, 2);
			array[8] = new ElectricConnectionPath(0, -1, -1, 5, 5, 0);
			array[9] = new ElectricConnectionPath(0, -1, 0, 0, 5, 4);
			array[10] = new ElectricConnectionPath(0, -1, -1, 2, 5, 4);
			array[11] = new ElectricConnectionPath(0, 0, 0, 4, 5, 2);
			array[12] = new ElectricConnectionPath(1, 0, -1, 1, 1, 0);
			array[13] = new ElectricConnectionPath(1, 0, 0, 0, 1, 3);
			array[14] = new ElectricConnectionPath(1, 0, -1, 2, 1, 3);
			array[15] = new ElectricConnectionPath(0, 0, 0, 3, 1, 2);
			array[16] = new ElectricConnectionPath(0, 0, -1, 2, 2, 0);
			array[20] = new ElectricConnectionPath(-1, 1, 0, 4, 4, 1);
			array[21] = new ElectricConnectionPath(0, 1, 0, 1, 4, 5);
			array[22] = new ElectricConnectionPath(-1, 1, 0, 3, 4, 5);
			array[23] = new ElectricConnectionPath(0, 0, 0, 5, 4, 3);
			array[24] = new ElectricConnectionPath(-1, 0, 1, 0, 0, 1);
			array[25] = new ElectricConnectionPath(0, 0, 1, 1, 0, 2);
			array[26] = new ElectricConnectionPath(-1, 0, 1, 3, 0, 2);
			array[27] = new ElectricConnectionPath(0, 0, 0, 2, 0, 3);
			array[28] = new ElectricConnectionPath(-1, -1, 0, 5, 5, 1);
			array[29] = new ElectricConnectionPath(0, -1, 0, 1, 5, 4);
			array[30] = new ElectricConnectionPath(-1, -1, 0, 3, 5, 4);
			array[31] = new ElectricConnectionPath(0, 0, 0, 4, 5, 3);
			array[32] = new ElectricConnectionPath(-1, 0, -1, 2, 2, 1);
			array[33] = new ElectricConnectionPath(0, 0, -1, 1, 2, 0);
			array[34] = new ElectricConnectionPath(-1, 0, -1, 3, 2, 0);
			array[35] = new ElectricConnectionPath(0, 0, 0, 0, 2, 3);
			array[36] = new ElectricConnectionPath(-1, 0, 0, 3, 3, 1);
			array[40] = new ElectricConnectionPath(0, 1, 1, 4, 4, 2);
			array[41] = new ElectricConnectionPath(0, 1, 0, 2, 4, 5);
			array[42] = new ElectricConnectionPath(0, 1, 1, 0, 4, 5);
			array[43] = new ElectricConnectionPath(0, 0, 0, 5, 4, 0);
			array[44] = new ElectricConnectionPath(1, 0, 1, 1, 1, 2);
			array[45] = new ElectricConnectionPath(1, 0, 0, 2, 1, 3);
			array[46] = new ElectricConnectionPath(1, 0, 1, 0, 1, 3);
			array[47] = new ElectricConnectionPath(0, 0, 0, 3, 1, 0);
			array[48] = new ElectricConnectionPath(0, -1, 1, 5, 5, 2);
			array[49] = new ElectricConnectionPath(0, -1, 0, 2, 5, 4);
			array[50] = new ElectricConnectionPath(0, -1, 1, 0, 5, 4);
			array[51] = new ElectricConnectionPath(0, 0, 0, 4, 5, 0);
			array[52] = new ElectricConnectionPath(-1, 0, 1, 3, 3, 2);
			array[53] = new ElectricConnectionPath(-1, 0, 0, 2, 3, 1);
			array[54] = new ElectricConnectionPath(-1, 0, 1, 0, 3, 1);
			array[55] = new ElectricConnectionPath(0, 0, 0, 1, 3, 0);
			array[56] = new ElectricConnectionPath(0, 0, 1, 0, 0, 2);
			array[60] = new ElectricConnectionPath(1, 1, 0, 4, 4, 3);
			array[61] = new ElectricConnectionPath(0, 1, 0, 3, 4, 5);
			array[62] = new ElectricConnectionPath(1, 1, 0, 1, 4, 5);
			array[63] = new ElectricConnectionPath(0, 0, 0, 5, 4, 1);
			array[64] = new ElectricConnectionPath(1, 0, -1, 2, 2, 3);
			array[65] = new ElectricConnectionPath(0, 0, -1, 3, 2, 0);
			array[66] = new ElectricConnectionPath(1, 0, -1, 1, 2, 0);
			array[67] = new ElectricConnectionPath(0, 0, 0, 0, 2, 1);
			array[68] = new ElectricConnectionPath(1, -1, 0, 5, 5, 3);
			array[69] = new ElectricConnectionPath(0, -1, 0, 3, 5, 4);
			array[70] = new ElectricConnectionPath(1, -1, 0, 1, 5, 4);
			array[71] = new ElectricConnectionPath(0, 0, 0, 4, 5, 1);
			array[72] = new ElectricConnectionPath(1, 0, 1, 0, 0, 3);
			array[73] = new ElectricConnectionPath(0, 0, 1, 3, 0, 2);
			array[74] = new ElectricConnectionPath(1, 0, 1, 1, 0, 2);
			array[75] = new ElectricConnectionPath(0, 0, 0, 2, 0, 1);
			array[76] = new ElectricConnectionPath(1, 0, 0, 1, 1, 3);
			array[80] = new ElectricConnectionPath(0, -1, -1, 2, 2, 4);
			array[81] = new ElectricConnectionPath(0, 0, -1, 4, 2, 0);
			array[82] = new ElectricConnectionPath(0, -1, -1, 5, 2, 0);
			array[83] = new ElectricConnectionPath(0, 0, 0, 0, 2, 5);
			array[84] = new ElectricConnectionPath(-1, -1, 0, 3, 3, 4);
			array[85] = new ElectricConnectionPath(-1, 0, 0, 4, 3, 1);
			array[86] = new ElectricConnectionPath(-1, -1, 0, 5, 3, 1);
			array[87] = new ElectricConnectionPath(0, 0, 0, 1, 3, 5);
			array[88] = new ElectricConnectionPath(0, -1, 1, 0, 0, 4);
			array[89] = new ElectricConnectionPath(0, 0, 1, 4, 0, 2);
			array[90] = new ElectricConnectionPath(0, -1, 1, 5, 0, 2);
			array[91] = new ElectricConnectionPath(0, 0, 0, 2, 0, 5);
			array[92] = new ElectricConnectionPath(1, -1, 0, 1, 1, 4);
			array[93] = new ElectricConnectionPath(1, 0, 0, 4, 1, 3);
			array[94] = new ElectricConnectionPath(1, -1, 0, 5, 1, 3);
			array[95] = new ElectricConnectionPath(0, 0, 0, 3, 1, 5);
			array[96] = new ElectricConnectionPath(0, -1, 0, 5, 5, 4);
			array[100] = new ElectricConnectionPath(0, 1, -1, 2, 2, 5);
			array[101] = new ElectricConnectionPath(0, 0, -1, 5, 2, 0);
			array[102] = new ElectricConnectionPath(0, 1, -1, 4, 2, 0);
			array[103] = new ElectricConnectionPath(0, 0, 0, 0, 2, 4);
			array[104] = new ElectricConnectionPath(1, 1, 0, 1, 1, 5);
			array[105] = new ElectricConnectionPath(1, 0, 0, 5, 1, 3);
			array[106] = new ElectricConnectionPath(1, 1, 0, 4, 1, 3);
			array[107] = new ElectricConnectionPath(0, 0, 0, 3, 1, 4);
			array[108] = new ElectricConnectionPath(0, 1, 1, 0, 0, 5);
			array[109] = new ElectricConnectionPath(0, 0, 1, 5, 0, 2);
			array[110] = new ElectricConnectionPath(0, 1, 1, 4, 0, 2);
			array[111] = new ElectricConnectionPath(0, 0, 0, 2, 0, 4);
			array[112] = new ElectricConnectionPath(-1, 1, 0, 3, 3, 5);
			array[113] = new ElectricConnectionPath(-1, 0, 0, 5, 3, 1);
			array[114] = new ElectricConnectionPath(-1, 1, 0, 4, 3, 1);
			array[115] = new ElectricConnectionPath(0, 0, 0, 1, 3, 4);
			array[116] = new ElectricConnectionPath(0, 1, 0, 4, 4, 5);
			SubsystemElectricity.m_connectionPathsTable = array;
			ElectricConnectorDirection?[] array2 = new ElectricConnectorDirection?[36];
			array2[1] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[2] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			array2[3] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[4] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[5] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[6] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[8] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[9] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			array2[10] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[11] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[12] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			array2[13] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[15] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[16] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[17] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[18] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[19] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			array2[20] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[22] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[23] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[24] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[25] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[26] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[27] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[29] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			array2[30] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[31] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[32] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[33] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[34] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			SubsystemElectricity.m_connectorDirectionsTable = array2;
			SubsystemElectricity.m_connectorFacesTable = new int[]
			{
				4,
				3,
				5,
				1,
				2,
				4,
				0,
				5,
				2,
				3,
				4,
				1,
				5,
				3,
				0,
				4,
				2,
				5,
				0,
				1,
				2,
				1,
				0,
				3,
				5,
				0,
				1,
				2,
				3,
				4
			};
			SubsystemElectricity.DebugDrawElectrics = false;
		}

		// Token: 0x040004EF RID: 1263
		public static ElectricConnectionPath[] m_connectionPathsTable;

		// Token: 0x040004F0 RID: 1264
		public static ElectricConnectorDirection?[] m_connectorDirectionsTable;

		// Token: 0x040004F1 RID: 1265
		public static int[] m_connectorFacesTable;

		// Token: 0x040004F2 RID: 1266
		public float m_remainingSimulationTime;

		// Token: 0x040004F3 RID: 1267
		public Dictionary<Point3, float> m_persistentElementsVoltages = new Dictionary<Point3, float>();

		// Token: 0x040004F4 RID: 1268
		public Dictionary<ElectricElement, bool> m_electricElements = new Dictionary<ElectricElement, bool>();

		// Token: 0x040004F5 RID: 1269
		public Dictionary<CellFace, ElectricElement> m_electricElementsByCellFace = new Dictionary<CellFace, ElectricElement>();

		// Token: 0x040004F6 RID: 1270
		public Dictionary<Point3, bool> m_pointsToUpdate = new Dictionary<Point3, bool>();

		// Token: 0x040004F7 RID: 1271
		public Dictionary<Point3, ElectricElement> m_electricElementsToAdd = new Dictionary<Point3, ElectricElement>();

		// Token: 0x040004F8 RID: 1272
		public Dictionary<ElectricElement, bool> m_electricElementsToRemove = new Dictionary<ElectricElement, bool>();

		// Token: 0x040004F9 RID: 1273
		public Dictionary<Point3, bool> m_wiresToUpdate = new Dictionary<Point3, bool>();

		// Token: 0x040004FA RID: 1274
		public List<Dictionary<ElectricElement, bool>> m_listsCache = new List<Dictionary<ElectricElement, bool>>();

		// Token: 0x040004FB RID: 1275
		public Dictionary<int, Dictionary<ElectricElement, bool>> m_futureSimulateLists = new Dictionary<int, Dictionary<ElectricElement, bool>>();

		// Token: 0x040004FC RID: 1276
		public Dictionary<ElectricElement, bool> m_nextStepSimulateList;

		// Token: 0x040004FD RID: 1277
		public DynamicArray<ElectricConnectionPath> m_tmpConnectionPaths = new DynamicArray<ElectricConnectionPath>();

		// Token: 0x040004FE RID: 1278
		public Dictionary<CellFace, bool> m_tmpVisited = new Dictionary<CellFace, bool>();

		// Token: 0x040004FF RID: 1279
		public Dictionary<CellFace, bool> m_tmpResult = new Dictionary<CellFace, bool>();

		// Token: 0x04000500 RID: 1280
		public static bool DebugDrawElectrics;

		// Token: 0x04000501 RID: 1281
		public static int SimulatedElectricElements;

		// Token: 0x04000502 RID: 1282
		public const float CircuitStepDuration = 0.01f;
	}
}
