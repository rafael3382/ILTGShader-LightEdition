﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Engine;

namespace Game
{
	// Token: 0x02000328 RID: 808
	public class TerrainUpdater
	{
		// Token: 0x17000388 RID: 904
		// (get) Token: 0x0600181A RID: 6170 RVA: 0x000BD6DD File Offset: 0x000BB8DD
		public AutoResetEvent UpdateEvent
		{
			get
			{
				return this.m_updateEvent;
			}
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x000BD6E8 File Offset: 0x000BB8E8
		public TerrainUpdater(SubsystemTerrain subsystemTerrain)
		{
			this.m_subsystemTerrain = subsystemTerrain;
			this.m_subsystemSky = this.m_subsystemTerrain.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemBlockBehaviors = this.m_subsystemTerrain.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_subsystemAnimatedTextures = this.m_subsystemTerrain.Project.FindSubsystem<SubsystemAnimatedTextures>(true);
			this.m_terrain = subsystemTerrain.Terrain;
			this.m_updateParameters.Chunks = new TerrainChunk[0];
			this.m_updateParameters.Locations = new Dictionary<int, TerrainUpdater.UpdateLocation>();
			this.m_threadUpdateParameters.Chunks = new TerrainChunk[0];
			this.m_threadUpdateParameters.Locations = new Dictionary<int, TerrainUpdater.UpdateLocation>();
			SettingsManager.SettingChanged += this.SettingsManager_SettingChanged;
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x000BD7F8 File Offset: 0x000BB9F8
		public void Dispose()
		{
			SettingsManager.SettingChanged -= this.SettingsManager_SettingChanged;
			this.m_quitUpdateThread = true;
			this.UnpauseUpdateThread();
			this.m_updateEvent.Set();
			if (this.m_task != null)
			{
				this.m_task.Wait();
				this.m_task = null;
			}
			this.m_pauseEvent.Dispose();
			this.m_updateEvent.Dispose();
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x000BD861 File Offset: 0x000BBA61
		public void RequestSynchronousUpdate()
		{
			this.m_synchronousUpdateFrame = Time.FrameIndex;
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x000BD870 File Offset: 0x000BBA70
		public void SetUpdateLocation(int locationIndex, Vector2 center, float visibilityDistance, float contentDistance)
		{
			contentDistance = MathUtils.Max(contentDistance, visibilityDistance);
			TerrainUpdater.UpdateLocation updateLocation;
			this.m_updateParameters.Locations.TryGetValue(locationIndex, out updateLocation);
			if (contentDistance != updateLocation.ContentDistance || visibilityDistance != updateLocation.VisibilityDistance || updateLocation.LastChunksUpdateCenter == null || Vector2.DistanceSquared(center, updateLocation.LastChunksUpdateCenter.Value) > 64f)
			{
				updateLocation.Center = center;
				updateLocation.VisibilityDistance = visibilityDistance;
				updateLocation.ContentDistance = contentDistance;
				updateLocation.LastChunksUpdateCenter = new Vector2?(center);
				this.m_pendingLocations[locationIndex] = new TerrainUpdater.UpdateLocation?(updateLocation);
			}
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x000BD910 File Offset: 0x000BBB10
		public void RemoveUpdateLocation(int locationIndex)
		{
			this.m_pendingLocations[locationIndex] = null;
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x000BD934 File Offset: 0x000BBB34
		public float GetUpdateProgress(int locationIndex, float visibilityDistance, float contentDistance)
		{
			int num = 0;
			int num2 = 0;
			TerrainUpdater.UpdateLocation updateLocation;
			if (!this.m_updateParameters.Locations.TryGetValue(locationIndex, out updateLocation))
			{
				return 0f;
			}
			visibilityDistance = MathUtils.Max(MathUtils.Min(visibilityDistance, updateLocation.VisibilityDistance) - 8f - 0.1f, 0f);
			contentDistance = MathUtils.Max(MathUtils.Min(contentDistance, updateLocation.ContentDistance) - 8f - 0.1f, 0f);
			float num3 = MathUtils.Sqr(visibilityDistance);
			float num4 = MathUtils.Sqr(contentDistance);
			float v = MathUtils.Max(visibilityDistance, contentDistance);
			Point2 point = Terrain.ToChunk(updateLocation.Center - new Vector2(v));
			Point2 point2 = Terrain.ToChunk(updateLocation.Center + new Vector2(v));
			for (int i = point.X; i <= point2.X; i++)
			{
				for (int j = point.Y; j <= point2.Y; j++)
				{
					TerrainChunk chunkAtCoords = this.m_terrain.GetChunkAtCoords(i, j);
					Vector2 v2 = new Vector2(((float)i + 0.5f) * 16f, ((float)j + 0.5f) * 16f);
					float num5 = Vector2.DistanceSquared(updateLocation.Center, v2);
					if (num5 <= num3)
					{
						if (chunkAtCoords == null || chunkAtCoords.State < TerrainChunkState.Valid)
						{
							num2++;
						}
						else
						{
							num++;
						}
					}
					else if (num5 <= num4)
					{
						if (chunkAtCoords == null || chunkAtCoords.State < TerrainChunkState.InvalidLight)
						{
							num2++;
						}
						else
						{
							num++;
						}
					}
				}
			}
			if (num2 <= 0)
			{
				return 1f;
			}
			return (float)num / (float)(num2 + num);
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x000BDAD0 File Offset: 0x000BBCD0
		public void Update()
		{
			if (this.m_subsystemSky.SkyLightValue != this.m_lastSkylightValue)
			{
				this.m_lastSkylightValue = this.m_subsystemSky.SkyLightValue;
				this.DowngradeAllChunksState(TerrainChunkState.InvalidLight, false);
			}
			if (!SettingsManager.MultithreadedTerrainUpdate)
			{
				if (this.m_task != null)
				{
					this.m_quitUpdateThread = true;
					this.UnpauseUpdateThread();
					this.m_updateEvent.Set();
					this.m_task.Wait();
					this.m_task = null;
				}
				double realTime = Time.RealTime;
				while (!this.SynchronousUpdateFunction())
				{
					if (Time.RealTime - realTime >= 0.0099999997764825821)
					{
						break;
					}
				}
			}
			else if (this.m_task == null)
			{
				this.m_quitUpdateThread = false;
				this.m_task = Task.Run(new Action(this.ThreadUpdateFunction));
				this.UnpauseUpdateThread();
				this.m_updateEvent.Set();
			}
			if (this.m_pendingLocations.Count > 0)
			{
				this.m_pauseEvent.Reset();
				if (!this.m_updateEvent.WaitOne(0))
				{
					goto IL_1FB;
				}
				this.m_pauseEvent.Set();
				try
				{
					foreach (KeyValuePair<int, TerrainUpdater.UpdateLocation?> keyValuePair in this.m_pendingLocations)
					{
						if (keyValuePair.Value != null)
						{
							this.m_updateParameters.Locations[keyValuePair.Key] = keyValuePair.Value.Value;
						}
						else
						{
							this.m_updateParameters.Locations.Remove(keyValuePair.Key);
						}
					}
					if (this.AllocateAndFreeChunks(this.m_updateParameters.Locations.Values.ToArray<TerrainUpdater.UpdateLocation>()))
					{
						this.m_updateParameters.Chunks = this.m_terrain.AllocatedChunks;
					}
					this.m_pendingLocations.Clear();
					goto IL_1FB;
				}
				finally
				{
					this.m_updateEvent.Set();
				}
			}
			object updateParametersLock = this.m_updateParametersLock;
			lock (updateParametersLock)
			{
				if (this.SendReceiveChunkStates())
				{
					this.UnpauseUpdateThread();
				}
			}
			IL_1FB:
			foreach (TerrainChunk terrainChunk in this.m_terrain.AllocatedChunks)
			{
				if (terrainChunk.State >= TerrainChunkState.InvalidVertices1 && !terrainChunk.AreBehaviorsNotified)
				{
					terrainChunk.AreBehaviorsNotified = true;
					this.NotifyBlockBehaviors(terrainChunk);
				}
			}
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x000BDD4C File Offset: 0x000BBF4C
		public void PrepareForDrawing(Camera camera)
		{
			this.SetUpdateLocation(camera.GameWidget.PlayerData.PlayerIndex, camera.ViewPosition.XZ, this.m_subsystemSky.VisibilityRange, 64f);
			if (this.m_synchronousUpdateFrame == Time.FrameIndex)
			{
				List<TerrainChunk> list = this.DetermineSynchronousUpdateChunks(camera.ViewPosition, camera.ViewDirection);
				if (list.Count > 0)
				{
					this.m_updateEvent.WaitOne();
					try
					{
						this.SendReceiveChunkStates();
						this.SendReceiveChunkStatesThread();
						foreach (TerrainChunk terrainChunk in list)
						{
							while (terrainChunk.ThreadState < TerrainChunkState.Valid)
							{
								this.UpdateChunkSingleStep(terrainChunk, this.m_subsystemSky.SkyLightValue);
							}
						}
						this.SendReceiveChunkStatesThread();
						this.SendReceiveChunkStates();
					}
					finally
					{
						this.m_updateEvent.Set();
					}
				}
			}
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x000BDE54 File Offset: 0x000BC054
		public void DowngradeChunkNeighborhoodState(Point2 coordinates, int radius, TerrainChunkState state, bool forceGeometryRegeneration)
		{
			for (int i = -radius; i <= radius; i++)
			{
				for (int j = -radius; j <= radius; j++)
				{
					TerrainChunk chunkAtCoords = this.m_terrain.GetChunkAtCoords(coordinates.X + i, coordinates.Y + j);
					if (chunkAtCoords != null)
					{
						if (chunkAtCoords.State > state)
						{
							chunkAtCoords.State = state;
							if (forceGeometryRegeneration)
							{
								chunkAtCoords.Geometry.InvalidateSliceContentsHashes();
							}
						}
						chunkAtCoords.WasDowngraded = true;
					}
				}
			}
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x000BDEC0 File Offset: 0x000BC0C0
		public void DowngradeAllChunksState(TerrainChunkState state, bool forceGeometryRegeneration)
		{
			foreach (TerrainChunk terrainChunk in this.m_terrain.AllocatedChunks)
			{
				if (terrainChunk.State > state)
				{
					terrainChunk.State = state;
					if (forceGeometryRegeneration)
					{
						terrainChunk.Geometry.InvalidateSliceContentsHashes();
					}
				}
				terrainChunk.WasDowngraded = true;
			}
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x000BDF10 File Offset: 0x000BC110
		public static bool IsChunkInRange(Vector2 chunkCenter, TerrainUpdater.UpdateLocation[] locations)
		{
			for (int i = 0; i < locations.Length; i++)
			{
				if (Vector2.DistanceSquared(locations[i].Center, chunkCenter) <= MathUtils.Sqr(locations[i].ContentDistance))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x000BDF54 File Offset: 0x000BC154
		public bool AllocateAndFreeChunks(TerrainUpdater.UpdateLocation[] locations)
		{
			bool result = false;
			foreach (TerrainChunk terrainChunk in this.m_terrain.AllocatedChunks)
			{
				if (!TerrainUpdater.IsChunkInRange(terrainChunk.Center, locations))
				{
					result = true;
					foreach (SubsystemBlockBehavior subsystemBlockBehavior in this.m_subsystemBlockBehaviors.BlockBehaviors)
					{
						subsystemBlockBehavior.OnChunkDiscarding(terrainChunk);
					}
					this.m_subsystemTerrain.TerrainSerializer.SaveChunk(terrainChunk);
					this.m_terrain.FreeChunk(terrainChunk);
					this.m_subsystemTerrain.TerrainRenderer.DisposeTerrainChunkGeometryVertexIndexBuffers(terrainChunk.Geometry);
				}
			}
			for (int j = 0; j < locations.Length; j++)
			{
				Point2 point = Terrain.ToChunk(locations[j].Center - new Vector2(locations[j].ContentDistance));
				Point2 point2 = Terrain.ToChunk(locations[j].Center + new Vector2(locations[j].ContentDistance));
				for (int k = point.X; k <= point2.X; k++)
				{
					for (int l = point.Y; l <= point2.Y; l++)
					{
						Vector2 chunkCenter = new Vector2(((float)k + 0.5f) * 16f, ((float)l + 0.5f) * 16f);
						TerrainChunk chunkAtCoords = this.m_terrain.GetChunkAtCoords(k, l);
						if (chunkAtCoords == null)
						{
							if (TerrainUpdater.IsChunkInRange(chunkCenter, locations))
							{
								result = true;
								this.m_terrain.AllocateChunk(k, l);
								this.DowngradeChunkNeighborhoodState(new Point2(k, l), 0, TerrainChunkState.NotLoaded, false);
								this.DowngradeChunkNeighborhoodState(new Point2(k, l), 1, TerrainChunkState.InvalidLight, false);
							}
						}
						else if (chunkAtCoords.Coords.X != k || chunkAtCoords.Coords.Y != l)
						{
							Log.Error("Chunk wraparound detected at {0}", new object[]
							{
								chunkAtCoords.Coords
							});
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x000BE188 File Offset: 0x000BC388
		public bool SendReceiveChunkStates()
		{
			bool result = false;
			foreach (TerrainChunk terrainChunk in this.m_updateParameters.Chunks)
			{
				if (terrainChunk.WasDowngraded)
				{
					terrainChunk.DowngradedState = new TerrainChunkState?(terrainChunk.State);
					terrainChunk.WasDowngraded = false;
					result = true;
				}
				else if (terrainChunk.UpgradedState != null)
				{
					terrainChunk.State = terrainChunk.UpgradedState.Value;
				}
				terrainChunk.UpgradedState = null;
			}
			return result;
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x000BE204 File Offset: 0x000BC404
		public void SendReceiveChunkStatesThread()
		{
			foreach (TerrainChunk terrainChunk in this.m_threadUpdateParameters.Chunks)
			{
				if (terrainChunk.DowngradedState != null)
				{
					terrainChunk.ThreadState = terrainChunk.DowngradedState.Value;
					terrainChunk.DowngradedState = null;
				}
				else if (terrainChunk.WasUpgraded)
				{
					terrainChunk.UpgradedState = new TerrainChunkState?(terrainChunk.ThreadState);
				}
				terrainChunk.WasUpgraded = false;
			}
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x000BE27C File Offset: 0x000BC47C
		public void ThreadUpdateFunction()
		{
			while (!this.m_quitUpdateThread)
			{
				this.m_pauseEvent.WaitOne();
				this.m_updateEvent.WaitOne();
				try
				{
					if (this.SynchronousUpdateFunction())
					{
						object unpauseLock = this.m_unpauseLock;
						lock (unpauseLock)
						{
							if (!this.m_unpauseUpdateThread)
							{
								this.m_pauseEvent.Reset();
							}
							this.m_unpauseUpdateThread = false;
						}
					}
				}
				catch (Exception)
				{
				}
				finally
				{
					this.m_updateEvent.Set();
				}
			}
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x000BE328 File Offset: 0x000BC528
		public bool SynchronousUpdateFunction()
		{
			object updateParametersLock = this.m_updateParametersLock;
			lock (updateParametersLock)
			{
				this.m_threadUpdateParameters = this.m_updateParameters;
				this.SendReceiveChunkStatesThread();
			}
			TerrainChunkState terrainChunkState;
			TerrainChunk terrainChunk = this.FindBestChunkToUpdate(out terrainChunkState);
			if (terrainChunk != null)
			{
				double realTime = Time.RealTime;
				do
				{
					this.UpdateChunkSingleStep(terrainChunk, this.m_subsystemSky.SkyLightValue);
				}
				while (terrainChunk.ThreadState < terrainChunkState && Time.RealTime - realTime < 0.0099999997764825821);
				return false;
			}
			if (TerrainUpdater.LogTerrainUpdateStats)
			{
				this.m_statistics.Log();
				this.m_statistics = new TerrainUpdater.UpdateStatistics();
			}
			return true;
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x000BE3D8 File Offset: 0x000BC5D8
		public TerrainChunk FindBestChunkToUpdate(out TerrainChunkState desiredState)
		{
			double realTime = Time.RealTime;
			TerrainChunk[] chunks = this.m_threadUpdateParameters.Chunks;
			TerrainUpdater.UpdateLocation[] array = this.m_threadUpdateParameters.Locations.Values.ToArray<TerrainUpdater.UpdateLocation>();
			float num = float.MaxValue;
			TerrainChunk result = null;
			desiredState = TerrainChunkState.NotLoaded;
			foreach (TerrainChunk terrainChunk in chunks)
			{
				if (terrainChunk.ThreadState < TerrainChunkState.Valid)
				{
					for (int j = 0; j < array.Length; j++)
					{
						float num2 = Vector2.DistanceSquared(array[j].Center, terrainChunk.Center);
						if (num2 < num)
						{
							if (num2 <= MathUtils.Sqr(array[j].VisibilityDistance))
							{
								desiredState = TerrainChunkState.Valid;
								num = num2;
								result = terrainChunk;
							}
							else if (terrainChunk.ThreadState < TerrainChunkState.InvalidVertices1 && num2 <= MathUtils.Sqr(array[j].ContentDistance))
							{
								desiredState = TerrainChunkState.InvalidVertices1;
								num = num2;
								result = terrainChunk;
							}
						}
					}
				}
			}
			double realTime2 = Time.RealTime;
			this.m_statistics.FindBestChunkTime += realTime2 - realTime;
			this.m_statistics.FindBestChunkCount++;
			return result;
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x000BE4F4 File Offset: 0x000BC6F4
		public List<TerrainChunk> DetermineSynchronousUpdateChunks(Vector3 viewPosition, Vector3 viewDirection)
		{
			Vector3 vector = Vector3.Normalize(Vector3.Cross(viewDirection, Vector3.UnitY));
			Vector3 v = Vector3.Normalize(Vector3.Cross(viewDirection, vector));
			Vector3[] array = new Vector3[]
			{
				viewPosition,
				viewPosition + 6f * viewDirection,
				viewPosition + 6f * viewDirection - 6f * vector,
				viewPosition + 6f * viewDirection + 6f * vector,
				viewPosition + 6f * viewDirection - 2f * v,
				viewPosition + 6f * viewDirection + 2f * v
			};
			List<TerrainChunk> list = new List<TerrainChunk>();
			foreach (Vector3 vector2 in array)
			{
				TerrainChunk chunkAtCell = this.m_terrain.GetChunkAtCell(Terrain.ToCell(vector2.X), Terrain.ToCell(vector2.Z));
				if (chunkAtCell != null && chunkAtCell.State < TerrainChunkState.Valid && !list.Contains(chunkAtCell))
				{
					list.Add(chunkAtCell);
				}
			}
			return list;
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x000BE650 File Offset: 0x000BC850
		public void UpdateChunkSingleStep(TerrainChunk chunk, int skylightValue)
		{
			switch (chunk.ThreadState)
			{
			case TerrainChunkState.NotLoaded:
			{
				double realTime = Time.RealTime;
				if (this.m_subsystemTerrain.TerrainSerializer.LoadChunk(chunk))
				{
					chunk.ThreadState = TerrainChunkState.InvalidLight;
					chunk.WasUpgraded = true;
					double realTime2 = Time.RealTime;
					chunk.IsLoaded = true;
					this.m_statistics.LoadingCount++;
					this.m_statistics.LoadingTime += realTime2 - realTime;
					return;
				}
				chunk.ThreadState = TerrainChunkState.InvalidContents1;
				chunk.WasUpgraded = true;
				return;
			}
			case TerrainChunkState.InvalidContents1:
			{
				double realTime3 = Time.RealTime;
				this.m_subsystemTerrain.TerrainContentsGenerator.GenerateChunkContentsPass1(chunk);
				chunk.ThreadState = TerrainChunkState.InvalidContents2;
				chunk.WasUpgraded = true;
				double realTime4 = Time.RealTime;
				this.m_statistics.ContentsCount1++;
				this.m_statistics.ContentsTime1 += realTime4 - realTime3;
				return;
			}
			case TerrainChunkState.InvalidContents2:
			{
				double realTime5 = Time.RealTime;
				this.m_subsystemTerrain.TerrainContentsGenerator.GenerateChunkContentsPass2(chunk);
				chunk.ThreadState = TerrainChunkState.InvalidContents3;
				chunk.WasUpgraded = true;
				double realTime6 = Time.RealTime;
				this.m_statistics.ContentsCount2++;
				this.m_statistics.ContentsTime2 += realTime6 - realTime5;
				return;
			}
			case TerrainChunkState.InvalidContents3:
			{
				double realTime7 = Time.RealTime;
				this.m_subsystemTerrain.TerrainContentsGenerator.GenerateChunkContentsPass3(chunk);
				chunk.ThreadState = TerrainChunkState.InvalidContents4;
				chunk.WasUpgraded = true;
				double realTime8 = Time.RealTime;
				this.m_statistics.ContentsCount3++;
				this.m_statistics.ContentsTime3 += realTime8 - realTime7;
				ModsManager.HookAction("OnTerrainContentsGenerated", delegate(ModLoader modLoader)
				{
					modLoader.OnTerrainContentsGenerated(chunk);
					return true;
				});
				return;
			}
			case TerrainChunkState.InvalidContents4:
			{
				double realTime9 = Time.RealTime;
				this.m_subsystemTerrain.TerrainContentsGenerator.GenerateChunkContentsPass4(chunk);
				chunk.ThreadState = TerrainChunkState.InvalidLight;
				chunk.WasUpgraded = true;
				double realTime10 = Time.RealTime;
				this.m_statistics.ContentsCount4++;
				this.m_statistics.ContentsTime4 += realTime10 - realTime9;
				return;
			}
			case TerrainChunkState.InvalidLight:
			{
				double realTime11 = Time.RealTime;
				this.GenerateChunkSunLightAndHeight(chunk, skylightValue);
				chunk.ThreadState = TerrainChunkState.InvalidPropagatedLight;
				chunk.WasUpgraded = true;
				chunk.LightPropagationMask = 0;
				double realTime12 = Time.RealTime;
				this.m_statistics.LightCount++;
				this.m_statistics.LightTime += realTime12 - realTime11;
				return;
			}
			case TerrainChunkState.InvalidPropagatedLight:
			{
				for (int i = -2; i <= 2; i++)
				{
					for (int j = -2; j <= 2; j++)
					{
						TerrainChunk chunkAtCell = this.m_terrain.GetChunkAtCell(chunk.Origin.X + i * 16, chunk.Origin.Y + j * 16);
						if (chunkAtCell != null && chunkAtCell.ThreadState < TerrainChunkState.InvalidPropagatedLight)
						{
							this.UpdateChunkSingleStep(chunkAtCell, skylightValue);
							return;
						}
					}
				}
				double realTime13 = Time.RealTime;
				this.m_lightSources.Clear();
				for (int k = -1; k <= 1; k++)
				{
					for (int l = -1; l <= 1; l++)
					{
						int num = TerrainUpdater.CalculateLightPropagationBitIndex(k, l);
						if ((chunk.LightPropagationMask >> num & 1) == 0)
						{
							TerrainChunk chunkAtCell2 = this.m_terrain.GetChunkAtCell(chunk.Origin.X + k * 16, chunk.Origin.Y + l * 16);
							if (chunkAtCell2 != null)
							{
								this.GenerateChunkLightSources(chunkAtCell2);
								this.UpdateNeighborsLightPropagationBitmasks(chunkAtCell2);
							}
						}
					}
				}
				double realTime14 = Time.RealTime;
				this.m_statistics.LightSourcesCount++;
				this.m_statistics.LightSourcesTime += realTime14 - realTime13;
				double realTime15 = Time.RealTime;
				this.PropagateLight();
				chunk.ThreadState = TerrainChunkState.InvalidVertices1;
				chunk.WasUpgraded = true;
				double realTime16 = Time.RealTime;
				this.m_statistics.LightPropagateCount++;
				this.m_statistics.LightSourceInstancesCount += this.m_lightSources.Count;
				this.m_statistics.LightPropagateTime += realTime16 - realTime15;
				return;
			}
			case TerrainChunkState.InvalidVertices1:
			{
				double realTime17 = Time.RealTime;
				TerrainChunkGeometry geometry = chunk.Geometry;
				lock (geometry)
				{
					chunk.NewGeometryData = false;
					this.GenerateChunkVertices(chunk, true);
				}
				chunk.ThreadState = TerrainChunkState.InvalidVertices2;
				chunk.WasUpgraded = true;
				double realTime18 = Time.RealTime;
				this.m_statistics.VerticesCount1++;
				this.m_statistics.VerticesTime1 += realTime18 - realTime17;
				return;
			}
			case TerrainChunkState.InvalidVertices2:
			{
				double realTime19 = Time.RealTime;
				TerrainChunkGeometry geometry = chunk.Geometry;
				lock (geometry)
				{
					this.GenerateChunkVertices(chunk, false);
					chunk.NewGeometryData = true;
				}
				chunk.ThreadState = TerrainChunkState.Valid;
				chunk.WasUpgraded = true;
				double realTime20 = Time.RealTime;
				this.m_statistics.VerticesCount2++;
				this.m_statistics.VerticesTime2 += realTime20 - realTime19;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x000BEC48 File Offset: 0x000BCE48
		public void GenerateChunkSunLightAndHeight(TerrainChunk chunk, int skylightValue)
		{
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int num = 0;
					int num2 = 255;
					int k = 255;
					int num3 = TerrainChunk.CalculateCellIndex(i, 255, j);
					while (k >= 0)
					{
						int value = chunk.GetCellValueFast(num3);
						if (Terrain.ExtractContents(value) != 0)
						{
							num = k;
							break;
						}
						value = Terrain.ReplaceLight(value, skylightValue);
						chunk.SetCellValueFast(num3, value);
						k--;
						num3--;
					}
					k = 0;
					num3 = TerrainChunk.CalculateCellIndex(i, 0, j);
					while (k <= num + 1)
					{
						int value2 = chunk.GetCellValueFast(num3);
						int num4 = Terrain.ExtractContents(value2);
						if (BlocksManager.Blocks[num4].IsTransparent)
						{
							num2 = k;
							break;
						}
						value2 = Terrain.ReplaceLight(value2, 0);
						chunk.SetCellValueFast(num3, value2);
						k++;
						num3++;
					}
					int num5 = skylightValue;
					k = num;
					num3 = TerrainChunk.CalculateCellIndex(i, num, j);
					if (num5 > 0)
					{
						while (k >= num2)
						{
							int value3 = chunk.GetCellValueFast(num3);
							int num6 = Terrain.ExtractContents(value3);
							if (num6 != 0)
							{
								Block block = BlocksManager.Blocks[num6];
								if (!block.IsTransparent || block.LightAttenuation >= num5)
								{
									break;
								}
								num5 -= block.LightAttenuation;
							}
							value3 = Terrain.ReplaceLight(value3, num5);
							chunk.SetCellValueFast(num3, value3);
							k--;
							num3--;
						}
					}
					int sunlightHeight = k + 1;
					while (k >= num2)
					{
						int value4 = chunk.GetCellValueFast(num3);
						value4 = Terrain.ReplaceLight(value4, 0);
						chunk.SetCellValueFast(num3, value4);
						k--;
						num3--;
					}
					chunk.SetTopHeightFast(i, j, num);
					chunk.SetBottomHeightFast(i, j, num2);
					chunk.SetSunlightHeightFast(i, j, sunlightHeight);
				}
			}
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x000BEE08 File Offset: 0x000BD008
		public void GenerateChunkLightSources(TerrainChunk chunk)
		{
			Block[] blocks = BlocksManager.Blocks;
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int num = i + chunk.Origin.X;
					int num2 = j + chunk.Origin.Y;
					TerrainChunk chunkAtCell = this.m_terrain.GetChunkAtCell(num - 1, num2);
					TerrainChunk chunkAtCell2 = this.m_terrain.GetChunkAtCell(num + 1, num2);
					TerrainChunk chunkAtCell3 = this.m_terrain.GetChunkAtCell(num, num2 - 1);
					TerrainChunk chunkAtCell4 = this.m_terrain.GetChunkAtCell(num, num2 + 1);
					if (chunkAtCell != null && chunkAtCell2 != null && chunkAtCell3 != null && chunkAtCell4 != null)
					{
						int topHeightFast = chunk.GetTopHeightFast(i, j);
						int bottomHeightFast = chunk.GetBottomHeightFast(i, j);
						int x = num - 1 - chunkAtCell.Origin.X;
						int z = num2 - chunkAtCell.Origin.Y;
						int x2 = num + 1 - chunkAtCell2.Origin.X;
						int z2 = num2 - chunkAtCell2.Origin.Y;
						int x3 = num - chunkAtCell3.Origin.X;
						int z3 = num2 - 1 - chunkAtCell3.Origin.Y;
						int x4 = num - chunkAtCell4.Origin.X;
						int z4 = num2 + 1 - chunkAtCell4.Origin.Y;
						int shaftValueFast = chunkAtCell.GetShaftValueFast(x, z);
						int shaftValueFast2 = chunkAtCell2.GetShaftValueFast(x2, z2);
						int shaftValueFast3 = chunkAtCell3.GetShaftValueFast(x3, z3);
						int shaftValueFast4 = chunkAtCell4.GetShaftValueFast(x4, z4);
						int x5 = Terrain.ExtractSunlightHeight(shaftValueFast);
						int x6 = Terrain.ExtractSunlightHeight(shaftValueFast2);
						int x7 = Terrain.ExtractSunlightHeight(shaftValueFast3);
						int x8 = Terrain.ExtractSunlightHeight(shaftValueFast4);
						int num3 = MathUtils.Min(x5, x6, x7, x8);
						int k = bottomHeightFast;
						int num4 = TerrainChunk.CalculateCellIndex(i, bottomHeightFast, j);
						while (k <= topHeightFast)
						{
							int cellValueFast = chunk.GetCellValueFast(num4);
							int num5 = 0;
							Block block = blocks[Terrain.ExtractContents(cellValueFast)];
							if (k >= num3 && block.IsTransparent)
							{
								int cellLightFast = chunkAtCell.GetCellLightFast(x, k, z);
								int cellLightFast2 = chunkAtCell2.GetCellLightFast(x2, k, z2);
								int cellLightFast3 = chunkAtCell3.GetCellLightFast(x3, k, z3);
								int cellLightFast4 = chunkAtCell4.GetCellLightFast(x4, k, z4);
								num5 = MathUtils.Max(cellLightFast, cellLightFast2, cellLightFast3, cellLightFast4) - 1 - block.LightAttenuation;
							}
							if (block.DefaultEmittedLightAmount > 0)
							{
								num5 = MathUtils.Max(num5, block.GetEmittedLightAmount(cellValueFast));
							}
							if (num5 > Terrain.ExtractLight(cellValueFast))
							{
								chunk.SetCellValueFast(num4, Terrain.ReplaceLight(cellValueFast, num5));
								this.m_lightSources.Add(new TerrainUpdater.LightSource
								{
									X = num,
									Y = k,
									Z = num2,
									Light = num5
								});
							}
							k++;
							num4++;
						}
					}
				}
			}
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x000BF0D8 File Offset: 0x000BD2D8
		public void PropagateLightSource(int x, int y, int z, int light)
		{
			TerrainChunk chunkAtCell = this.m_terrain.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return;
			}
			int index = TerrainChunk.CalculateCellIndex(x & 15, y, z & 15);
			int cellValueFast = chunkAtCell.GetCellValueFast(index);
			int num = Terrain.ExtractContents(cellValueFast);
			Block block = BlocksManager.Blocks[num];
			if (block.IsTransparent)
			{
				int num2 = light - block.LightAttenuation - 1;
				if (num2 > Terrain.ExtractLight(cellValueFast))
				{
					this.m_lightSources.Add(new TerrainUpdater.LightSource
					{
						X = x,
						Y = y,
						Z = z,
						Light = num2
					});
					chunkAtCell.SetCellValueFast(index, Terrain.ReplaceLight(cellValueFast, num2));
				}
			}
		}

		// Token: 0x06001831 RID: 6193 RVA: 0x000BF184 File Offset: 0x000BD384
		public void PropagateLight()
		{
			int num = 0;
			while (num < this.m_lightSources.Count && num < 120000)
			{
				TerrainUpdater.LightSource lightSource = this.m_lightSources.Array[num];
				int light = lightSource.Light;
				if (light > 1)
				{
					this.PropagateLightSource(lightSource.X - 1, lightSource.Y, lightSource.Z, light);
					this.PropagateLightSource(lightSource.X + 1, lightSource.Y, lightSource.Z, light);
					if (lightSource.Y > 0)
					{
						this.PropagateLightSource(lightSource.X, lightSource.Y - 1, lightSource.Z, light);
					}
					if (lightSource.Y < 255)
					{
						this.PropagateLightSource(lightSource.X, lightSource.Y + 1, lightSource.Z, light);
					}
					this.PropagateLightSource(lightSource.X, lightSource.Y, lightSource.Z - 1, light);
					this.PropagateLightSource(lightSource.X, lightSource.Y, lightSource.Z + 1, light);
				}
				num++;
			}
		}

		// Token: 0x06001832 RID: 6194 RVA: 0x000BF290 File Offset: 0x000BD490
		public void GenerateChunkVertices(TerrainChunk chunk, bool even)
		{
			this.m_subsystemTerrain.BlockGeometryGenerator.ResetCache();
			TerrainChunk chunkAtCoords = this.m_terrain.GetChunkAtCoords(chunk.Coords.X - 1, chunk.Coords.Y - 1);
			bool chunkAtCoords2 = this.m_terrain.GetChunkAtCoords(chunk.Coords.X, chunk.Coords.Y - 1) != null;
			TerrainChunk chunkAtCoords3 = this.m_terrain.GetChunkAtCoords(chunk.Coords.X + 1, chunk.Coords.Y - 1);
			bool chunkAtCoords4 = this.m_terrain.GetChunkAtCoords(chunk.Coords.X - 1, chunk.Coords.Y) != null;
			TerrainChunk chunkAtCoords5 = this.m_terrain.GetChunkAtCoords(chunk.Coords.X + 1, chunk.Coords.Y);
			TerrainChunk chunkAtCoords6 = this.m_terrain.GetChunkAtCoords(chunk.Coords.X - 1, chunk.Coords.Y + 1);
			TerrainChunk chunkAtCoords7 = this.m_terrain.GetChunkAtCoords(chunk.Coords.X, chunk.Coords.Y + 1);
			TerrainChunk chunkAtCoords8 = this.m_terrain.GetChunkAtCoords(chunk.Coords.X + 1, chunk.Coords.Y + 1);
			int num = 0;
			int num2 = 0;
			int num3 = 16;
			int num4 = 16;
			if (!chunkAtCoords4)
			{
				num++;
			}
			if (!chunkAtCoords2)
			{
				num2++;
			}
			if (chunkAtCoords5 == null)
			{
				num3--;
			}
			if (chunkAtCoords7 == null)
			{
				num4--;
			}
			for (int i = 0; i < 16; i++)
			{
				if (i % 2 == 0 == even)
				{
					TerrainChunkSliceGeometry terrainChunkSliceGeometry = chunk.Geometry.Slices[i];
					chunk.SliceContentsHashes[i] = this.CalculateChunkSliceContentsHash(chunk, i);
					if (terrainChunkSliceGeometry.ContentsHash != 0 && terrainChunkSliceGeometry.ContentsHash == chunk.SliceContentsHashes[i])
					{
						this.m_statistics.SkippedSlices++;
					}
					else
					{
						this.m_statistics.GeneratedSlices++;
						terrainChunkSliceGeometry.ClearSubsets(this.m_subsystemAnimatedTextures);
						for (int j = num; j < num3; j++)
						{
							int k = num2;
							while (k < num4)
							{
								if (j != 0)
								{
									if (j != 15)
									{
										goto IL_240;
									}
									if ((k != 0 || chunkAtCoords3 != null) && (k != 15 || chunkAtCoords8 != null))
									{
										goto IL_240;
									}
								}
								else if (k != 0 || chunkAtCoords != null)
								{
									if (k != 15 || chunkAtCoords6 != null)
									{
										goto IL_240;
									}
								}
								IL_354:
								k++;
								continue;
								IL_240:
								int num5 = j + chunk.Origin.X;
								int num6 = k + chunk.Origin.Y;
								int bottomHeightFast = chunk.GetBottomHeightFast(j, k);
								int bottomHeight = this.m_terrain.GetBottomHeight(num5 - 1, num6);
								int bottomHeight2 = this.m_terrain.GetBottomHeight(num5 + 1, num6);
								int bottomHeight3 = this.m_terrain.GetBottomHeight(num5, num6 - 1);
								int bottomHeight4 = this.m_terrain.GetBottomHeight(num5, num6 + 1);
								int x = MathUtils.Min(bottomHeightFast - 1, MathUtils.Min(bottomHeight, bottomHeight2, bottomHeight3, bottomHeight4));
								int x2 = chunk.GetTopHeightFast(j, k) + 1;
								int num7 = MathUtils.Max(16 * i, x, 1);
								int num8 = MathUtils.Min(16 * (i + 1), x2, 255);
								int num9 = TerrainChunk.CalculateCellIndex(j, 0, k);
								for (int l = num7; l < num8; l++)
								{
									int cellValueFast = chunk.GetCellValueFast(num9 + l);
									int num10 = Terrain.ExtractContents(cellValueFast);
									if (num10 != 0)
									{
										BlocksManager.Blocks[num10].GenerateTerrainVertices(this.m_subsystemTerrain.BlockGeometryGenerator, terrainChunkSliceGeometry, cellValueFast, num5, l, num6);
									}
								}
								goto IL_354;
							}
						}
					}
				}
			}
		}

		// Token: 0x06001833 RID: 6195 RVA: 0x000BF61E File Offset: 0x000BD81E
		public static int CalculateLightPropagationBitIndex(int x, int z)
		{
			return x + 1 + 3 * (z + 1);
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x000BF62C File Offset: 0x000BD82C
		public void UpdateNeighborsLightPropagationBitmasks(TerrainChunk chunk)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					TerrainChunk chunkAtCoords = this.m_terrain.GetChunkAtCoords(chunk.Coords.X + i, chunk.Coords.Y + j);
					if (chunkAtCoords != null)
					{
						int num = TerrainUpdater.CalculateLightPropagationBitIndex(-i, -j);
						chunkAtCoords.LightPropagationMask |= 1 << num;
					}
				}
			}
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x000BF698 File Offset: 0x000BD898
		public int CalculateChunkSliceContentsHash(TerrainChunk chunk, int sliceIndex)
		{
			double realTime = Time.RealTime;
			int num = 1;
			int num2 = chunk.Origin.X - 1;
			int num3 = chunk.Origin.X + 16 + 1;
			int num4 = chunk.Origin.Y - 1;
			int num5 = chunk.Origin.Y + 16 + 1;
			int x = MathUtils.Max(16 * sliceIndex - 1, 0);
			int x2 = MathUtils.Min(16 * (sliceIndex + 1) + 1, 256);
			for (int i = num2; i < num3; i++)
			{
				for (int j = num4; j < num5; j++)
				{
					TerrainChunk chunkAtCell = this.m_terrain.GetChunkAtCell(i, j);
					if (chunkAtCell != null)
					{
						int x3 = i & 15;
						int z = j & 15;
						int shaftValueFast = chunkAtCell.GetShaftValueFast(x3, z);
						int num6 = Terrain.ExtractBottomHeight(shaftValueFast);
						int num7 = Terrain.ExtractTopHeight(shaftValueFast);
						int num8 = MathUtils.Max(x, num6 - 1);
						int num9 = MathUtils.Min(x2, num7 + 2);
						int k = TerrainChunk.CalculateCellIndex(x3, num8, z);
						int num10 = k + num9 - num8;
						while (k < num10)
						{
							num += chunkAtCell.GetCellValueFast(k++);
							num *= 31;
						}
						num += Terrain.ExtractTemperature(shaftValueFast);
						num *= 31;
						num += Terrain.ExtractHumidity(shaftValueFast);
						num *= 31;
						num += num8;
						num *= 31;
					}
				}
			}
			num += this.m_terrain.SeasonTemperature;
			num *= 31;
			num += this.m_terrain.SeasonHumidity;
			num *= 31;
			double realTime2 = Time.RealTime;
			this.m_statistics.HashCount++;
			this.m_statistics.HashTime += realTime2 - realTime;
			return num;
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x000BF84C File Offset: 0x000BDA4C
		public void NotifyBlockBehaviors(TerrainChunk chunk)
		{
			foreach (SubsystemBlockBehavior subsystemBlockBehavior in this.m_subsystemBlockBehaviors.BlockBehaviors)
			{
				subsystemBlockBehavior.OnChunkInitialized(chunk);
			}
			bool isLoaded = chunk.IsLoaded;
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int x = i + chunk.Origin.X;
					int z = j + chunk.Origin.Y;
					int num = TerrainChunk.CalculateCellIndex(i, 0, j);
					int k = 0;
					while (k < 255)
					{
						int cellValueFast = chunk.GetCellValueFast(num);
						int num2 = Terrain.ExtractContents(cellValueFast);
						if (num2 != 0)
						{
							SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num2);
							for (int l = 0; l < blockBehaviors.Length; l++)
							{
								blockBehaviors[l].OnBlockGenerated(cellValueFast, x, k, z, isLoaded);
							}
						}
						k++;
						num++;
					}
				}
			}
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x000BF968 File Offset: 0x000BDB68
		public void UnpauseUpdateThread()
		{
			object unpauseLock = this.m_unpauseLock;
			lock (unpauseLock)
			{
				this.m_unpauseUpdateThread = true;
				this.m_pauseEvent.Set();
			}
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x000BF9B8 File Offset: 0x000BDBB8
		public void SettingsManager_SettingChanged(string name)
		{
			if (name == "Brightness")
			{
				this.DowngradeAllChunksState(TerrainChunkState.InvalidVertices1, true);
			}
		}

		// Token: 0x040010C6 RID: 4294
		public const int m_lightAttenuationWithDistance = 1;

		// Token: 0x040010C7 RID: 4295
		public const float m_updateHysteresis = 8f;

		// Token: 0x040010C8 RID: 4296
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040010C9 RID: 4297
		public SubsystemSky m_subsystemSky;

		// Token: 0x040010CA RID: 4298
		public SubsystemAnimatedTextures m_subsystemAnimatedTextures;

		// Token: 0x040010CB RID: 4299
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x040010CC RID: 4300
		public Terrain m_terrain;

		// Token: 0x040010CD RID: 4301
		public DynamicArray<TerrainUpdater.LightSource> m_lightSources = new DynamicArray<TerrainUpdater.LightSource>();

		// Token: 0x040010CE RID: 4302
		public TerrainUpdater.UpdateStatistics m_statistics = new TerrainUpdater.UpdateStatistics();

		// Token: 0x040010CF RID: 4303
		public Task m_task;

		// Token: 0x040010D0 RID: 4304
		public AutoResetEvent m_updateEvent = new AutoResetEvent(true);

		// Token: 0x040010D1 RID: 4305
		public ManualResetEvent m_pauseEvent = new ManualResetEvent(true);

		// Token: 0x040010D2 RID: 4306
		public volatile bool m_quitUpdateThread;

		// Token: 0x040010D3 RID: 4307
		public bool m_unpauseUpdateThread;

		// Token: 0x040010D4 RID: 4308
		public object m_updateParametersLock = new object();

		// Token: 0x040010D5 RID: 4309
		public object m_unpauseLock = new object();

		// Token: 0x040010D6 RID: 4310
		public TerrainUpdater.UpdateParameters m_updateParameters;

		// Token: 0x040010D7 RID: 4311
		public TerrainUpdater.UpdateParameters m_threadUpdateParameters;

		// Token: 0x040010D8 RID: 4312
		public int m_lastSkylightValue;

		// Token: 0x040010D9 RID: 4313
		public int m_synchronousUpdateFrame;

		// Token: 0x040010DA RID: 4314
		public Dictionary<int, TerrainUpdater.UpdateLocation?> m_pendingLocations = new Dictionary<int, TerrainUpdater.UpdateLocation?>();

		// Token: 0x040010DB RID: 4315
		public static int SlowTerrainUpdate;

		// Token: 0x040010DC RID: 4316
		public static bool LogTerrainUpdateStats;

		// Token: 0x02000550 RID: 1360
		public class UpdateStatistics
		{
			// Token: 0x0600228D RID: 8845 RVA: 0x000ED290 File Offset: 0x000EB490
			public void Log()
			{
				Engine.Log.Information("Terrain Update {0}", new object[]
				{
					TerrainUpdater.UpdateStatistics.m_counter++
				});
				if (this.FindBestChunkCount > 0)
				{
					Engine.Log.Information("    FindBestChunk:          {0:0.0}ms ({1}x)", new object[]
					{
						this.FindBestChunkTime * 1000.0,
						this.FindBestChunkCount
					});
				}
				if (this.LoadingCount > 0)
				{
					Engine.Log.Information("    Loading:                {0:0.0}ms ({1}x)", new object[]
					{
						this.LoadingTime * 1000.0,
						this.LoadingCount
					});
				}
				if (this.ContentsCount1 > 0)
				{
					Engine.Log.Information("    Contents1:              {0:0.0}ms ({1}x)", new object[]
					{
						this.ContentsTime1 * 1000.0,
						this.ContentsCount1
					});
				}
				if (this.ContentsCount2 > 0)
				{
					Engine.Log.Information("    Contents2:              {0:0.0}ms ({1}x)", new object[]
					{
						this.ContentsTime2 * 1000.0,
						this.ContentsCount2
					});
				}
				if (this.ContentsCount3 > 0)
				{
					Engine.Log.Information("    Contents3:              {0:0.0}ms ({1}x)", new object[]
					{
						this.ContentsTime3 * 1000.0,
						this.ContentsCount3
					});
				}
				if (this.ContentsCount4 > 0)
				{
					Engine.Log.Information("    Contents4:              {0:0.0}ms ({1}x)", new object[]
					{
						this.ContentsTime4 * 1000.0,
						this.ContentsCount4
					});
				}
				if (this.LightCount > 0)
				{
					Engine.Log.Information("    Light:                  {0:0.0}ms ({1}x)", new object[]
					{
						this.LightTime * 1000.0,
						this.LightCount
					});
				}
				if (this.LightSourcesCount > 0)
				{
					Engine.Log.Information("    LightSources:           {0:0.0}ms ({1}x)", new object[]
					{
						this.LightSourcesTime * 1000.0,
						this.LightSourcesCount
					});
				}
				if (this.LightPropagateCount > 0)
				{
					Engine.Log.Information("    LightPropagate:         {0:0.0}ms ({1}x) {2} ls", new object[]
					{
						this.LightPropagateTime * 1000.0,
						this.LightPropagateCount,
						this.LightSourceInstancesCount
					});
				}
				if (this.VerticesCount1 > 0)
				{
					Engine.Log.Information("    Vertices1:              {0:0.0}ms ({1}x)", new object[]
					{
						this.VerticesTime1 * 1000.0,
						this.VerticesCount1
					});
				}
				if (this.VerticesCount2 > 0)
				{
					Engine.Log.Information("    Vertices2:              {0:0.0}ms ({1}x)", new object[]
					{
						this.VerticesTime2 * 1000.0,
						this.VerticesCount2
					});
				}
				if (this.VerticesCount1 + this.VerticesCount2 > 0)
				{
					Engine.Log.Information("    AllVertices:            {0:0.0}ms ({1}x)", new object[]
					{
						(this.VerticesTime1 + this.VerticesTime2) * 1000.0,
						this.VerticesCount1 + this.VerticesCount2
					});
				}
				if (this.HashCount > 0)
				{
					Engine.Log.Information("        Hash:               {0:0.0}ms ({1}x)", new object[]
					{
						this.HashTime * 1000.0,
						this.HashCount
					});
				}
				if (this.GeneratedSlices > 0)
				{
					Engine.Log.Information("        Generated Slices:   {0}/{1}", new object[]
					{
						this.GeneratedSlices,
						this.GeneratedSlices + this.SkippedSlices
					});
				}
			}

			// Token: 0x04001924 RID: 6436
			public static int m_counter;

			// Token: 0x04001925 RID: 6437
			public double FindBestChunkTime;

			// Token: 0x04001926 RID: 6438
			public int FindBestChunkCount;

			// Token: 0x04001927 RID: 6439
			public double LoadingTime;

			// Token: 0x04001928 RID: 6440
			public int LoadingCount;

			// Token: 0x04001929 RID: 6441
			public double ContentsTime1;

			// Token: 0x0400192A RID: 6442
			public int ContentsCount1;

			// Token: 0x0400192B RID: 6443
			public double ContentsTime2;

			// Token: 0x0400192C RID: 6444
			public int ContentsCount2;

			// Token: 0x0400192D RID: 6445
			public double ContentsTime3;

			// Token: 0x0400192E RID: 6446
			public int ContentsCount3;

			// Token: 0x0400192F RID: 6447
			public double ContentsTime4;

			// Token: 0x04001930 RID: 6448
			public int ContentsCount4;

			// Token: 0x04001931 RID: 6449
			public double LightTime;

			// Token: 0x04001932 RID: 6450
			public int LightCount;

			// Token: 0x04001933 RID: 6451
			public double LightSourcesTime;

			// Token: 0x04001934 RID: 6452
			public int LightSourcesCount;

			// Token: 0x04001935 RID: 6453
			public double LightPropagateTime;

			// Token: 0x04001936 RID: 6454
			public int LightPropagateCount;

			// Token: 0x04001937 RID: 6455
			public int LightSourceInstancesCount;

			// Token: 0x04001938 RID: 6456
			public double VerticesTime1;

			// Token: 0x04001939 RID: 6457
			public int VerticesCount1;

			// Token: 0x0400193A RID: 6458
			public double VerticesTime2;

			// Token: 0x0400193B RID: 6459
			public int VerticesCount2;

			// Token: 0x0400193C RID: 6460
			public int HashCount;

			// Token: 0x0400193D RID: 6461
			public double HashTime;

			// Token: 0x0400193E RID: 6462
			public int GeneratedSlices;

			// Token: 0x0400193F RID: 6463
			public int SkippedSlices;
		}

		// Token: 0x02000551 RID: 1361
		public struct UpdateLocation
		{
			// Token: 0x04001940 RID: 6464
			public Vector2 Center;

			// Token: 0x04001941 RID: 6465
			public Vector2? LastChunksUpdateCenter;

			// Token: 0x04001942 RID: 6466
			public float VisibilityDistance;

			// Token: 0x04001943 RID: 6467
			public float ContentDistance;
		}

		// Token: 0x02000552 RID: 1362
		public struct UpdateParameters
		{
			// Token: 0x04001944 RID: 6468
			public TerrainChunk[] Chunks;

			// Token: 0x04001945 RID: 6469
			public Dictionary<int, TerrainUpdater.UpdateLocation> Locations;
		}

		// Token: 0x02000553 RID: 1363
		public struct LightSource
		{
			// Token: 0x04001946 RID: 6470
			public int X;

			// Token: 0x04001947 RID: 6471
			public int Y;

			// Token: 0x04001948 RID: 6472
			public int Z;

			// Token: 0x04001949 RID: 6473
			public int Light;
		}
	}
}
