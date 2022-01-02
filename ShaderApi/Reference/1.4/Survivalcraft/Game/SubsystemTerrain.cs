using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D8 RID: 472
	public class SubsystemTerrain : Subsystem, IDrawable, IUpdateable
	{
		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000CBE RID: 3262 RVA: 0x0005C124 File Offset: 0x0005A324
		// (set) Token: 0x06000CBF RID: 3263 RVA: 0x0005C12C File Offset: 0x0005A32C
		public SubsystemGameInfo SubsystemGameInfo { get; set; }

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000CC0 RID: 3264 RVA: 0x0005C135 File Offset: 0x0005A335
		// (set) Token: 0x06000CC1 RID: 3265 RVA: 0x0005C13D File Offset: 0x0005A33D
		public SubsystemAnimatedTextures SubsystemAnimatedTextures { get; set; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000CC2 RID: 3266 RVA: 0x0005C146 File Offset: 0x0005A346
		// (set) Token: 0x06000CC3 RID: 3267 RVA: 0x0005C14E File Offset: 0x0005A34E
		public SubsystemFurnitureBlockBehavior SubsystemFurnitureBlockBehavior { get; set; }

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000CC4 RID: 3268 RVA: 0x0005C157 File Offset: 0x0005A357
		// (set) Token: 0x06000CC5 RID: 3269 RVA: 0x0005C15F File Offset: 0x0005A35F
		public SubsystemPalette SubsystemPalette { get; set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000CC6 RID: 3270 RVA: 0x0005C168 File Offset: 0x0005A368
		// (set) Token: 0x06000CC7 RID: 3271 RVA: 0x0005C170 File Offset: 0x0005A370
		public Terrain Terrain { get; set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000CC8 RID: 3272 RVA: 0x0005C179 File Offset: 0x0005A379
		// (set) Token: 0x06000CC9 RID: 3273 RVA: 0x0005C181 File Offset: 0x0005A381
		public TerrainUpdater TerrainUpdater { get; set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000CCA RID: 3274 RVA: 0x0005C18A File Offset: 0x0005A38A
		// (set) Token: 0x06000CCB RID: 3275 RVA: 0x0005C192 File Offset: 0x0005A392
		public TerrainRenderer TerrainRenderer { get; set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000CCC RID: 3276 RVA: 0x0005C19B File Offset: 0x0005A39B
		// (set) Token: 0x06000CCD RID: 3277 RVA: 0x0005C1A3 File Offset: 0x0005A3A3
		public TerrainSerializer22 TerrainSerializer { get; set; }

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000CCE RID: 3278 RVA: 0x0005C1AC File Offset: 0x0005A3AC
		// (set) Token: 0x06000CCF RID: 3279 RVA: 0x0005C1B4 File Offset: 0x0005A3B4
		public ITerrainContentsGenerator TerrainContentsGenerator { get; set; }

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000CD0 RID: 3280 RVA: 0x0005C1BD File Offset: 0x0005A3BD
		// (set) Token: 0x06000CD1 RID: 3281 RVA: 0x0005C1C5 File Offset: 0x0005A3C5
		public BlockGeometryGenerator BlockGeometryGenerator { get; set; }

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000CD2 RID: 3282 RVA: 0x0005C1CE File Offset: 0x0005A3CE
		public int[] DrawOrders
		{
			get
			{
				return SubsystemTerrain.m_drawOrders;
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000CD3 RID: 3283 RVA: 0x0005C1D5 File Offset: 0x0005A3D5
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Terrain;
			}
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x0005C1DC File Offset: 0x0005A3DC
		public void ProcessModifiedCells()
		{
			this.m_modifiedList.Clear();
			foreach (Point3 item in this.m_modifiedCells.Keys)
			{
				this.m_modifiedList.Add(item);
			}
			this.m_modifiedCells.Clear();
			for (int i = 0; i < this.m_modifiedList.Count; i++)
			{
				Point3 point = this.m_modifiedList.Array[i];
				for (int j = 0; j < SubsystemTerrain.m_neighborOffsets.Length; j++)
				{
					Point3 point2 = SubsystemTerrain.m_neighborOffsets[j];
					int cellValue = this.Terrain.GetCellValue(point.X + point2.X, point.Y + point2.Y, point.Z + point2.Z);
					SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(cellValue));
					for (int k = 0; k < blockBehaviors.Length; k++)
					{
						blockBehaviors[k].OnNeighborBlockChanged(point.X + point2.X, point.Y + point2.Y, point.Z + point2.Z, point.X, point.Y, point.Z);
					}
				}
			}
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x0005C34C File Offset: 0x0005A54C
		public TerrainRaycastResult? Raycast(Vector3 start, Vector3 end, bool useInteractionBoxes, bool skipAirBlocks, Func<int, float, bool> action)
		{
			float num = Vector3.Distance(start, end);
			if (num > 1000f)
			{
				Log.Warning("Terrain raycast too long, trimming.");
				end = start + 1000f * Vector3.Normalize(end - start);
			}
			Ray3 ray = new Ray3(start, Vector3.Normalize(end - start));
			float x = start.X;
			float y = start.Y;
			float z = start.Z;
			float x2 = end.X;
			float y2 = end.Y;
			float z2 = end.Z;
			int num2 = Terrain.ToCell(x);
			int num3 = Terrain.ToCell(y);
			int num4 = Terrain.ToCell(z);
			int num5 = Terrain.ToCell(x2);
			int num6 = Terrain.ToCell(y2);
			int num7 = Terrain.ToCell(z2);
			int num8 = (x < x2) ? 1 : ((x > x2) ? -1 : 0);
			int num9 = (y < y2) ? 1 : ((y > y2) ? -1 : 0);
			int num10 = (z < z2) ? 1 : ((z > z2) ? -1 : 0);
			float num11 = MathUtils.Floor(x);
			float num12 = num11 + 1f;
			float num13 = ((x > x2) ? (x - num11) : (num12 - x)) / Math.Abs(x2 - x);
			float num14 = MathUtils.Floor(y);
			float num15 = num14 + 1f;
			float num16 = ((y > y2) ? (y - num14) : (num15 - y)) / Math.Abs(y2 - y);
			float num17 = MathUtils.Floor(z);
			float num18 = num17 + 1f;
			float num19 = ((z > z2) ? (z - num17) : (num18 - z)) / Math.Abs(z2 - z);
			float num20 = 1f / Math.Abs(x2 - x);
			float num21 = 1f / Math.Abs(y2 - y);
			float num22 = 1f / Math.Abs(z2 - z);
			BoundingBox boundingBox;
			int collisionBoxIndex;
			float? num23;
			int cellValue;
			for (;;)
			{
				boundingBox = default(BoundingBox);
				collisionBoxIndex = 0;
				num23 = null;
				cellValue = this.Terrain.GetCellValue(num2, num3, num4);
				int num24 = Terrain.ExtractContents(cellValue);
				if (num24 != 0 || !skipAirBlocks)
				{
					Ray3 ray2 = new Ray3(ray.Position - new Vector3((float)num2, (float)num3, (float)num4), ray.Direction);
					int num26;
					BoundingBox boundingBox2;
					float? num25 = BlocksManager.Blocks[num24].Raycast(ray2, this, cellValue, useInteractionBoxes, out num26, out boundingBox2);
					if (num25 != null && (num23 == null || num25.Value < num23.Value))
					{
						num23 = num25;
						collisionBoxIndex = num26;
						boundingBox = boundingBox2;
					}
				}
				if (num23 != null && num23.Value <= num && (action == null || action(cellValue, num23.Value)))
				{
					break;
				}
				if (num13 <= num16 && num13 <= num19)
				{
					if (num2 == num5)
					{
						goto IL_46D;
					}
					num13 += num20;
					num2 += num8;
				}
				else if (num16 <= num13 && num16 <= num19)
				{
					if (num3 == num6)
					{
						goto IL_46D;
					}
					num16 += num21;
					num3 += num9;
				}
				else
				{
					if (num4 == num7)
					{
						goto IL_46D;
					}
					num19 += num22;
					num4 += num10;
				}
			}
			int face = 0;
			Vector3 vector = start - new Vector3((float)num2, (float)num3, (float)num4) + num23.Value * ray.Direction;
			float num27 = float.MaxValue;
			float num28 = MathUtils.Abs(vector.X - boundingBox.Min.X);
			if (num28 < num27)
			{
				num27 = num28;
				face = 3;
			}
			num28 = MathUtils.Abs(vector.X - boundingBox.Max.X);
			if (num28 < num27)
			{
				num27 = num28;
				face = 1;
			}
			num28 = MathUtils.Abs(vector.Y - boundingBox.Min.Y);
			if (num28 < num27)
			{
				num27 = num28;
				face = 5;
			}
			num28 = MathUtils.Abs(vector.Y - boundingBox.Max.Y);
			if (num28 < num27)
			{
				num27 = num28;
				face = 4;
			}
			num28 = MathUtils.Abs(vector.Z - boundingBox.Min.Z);
			if (num28 < num27)
			{
				num27 = num28;
				face = 2;
			}
			num28 = MathUtils.Abs(vector.Z - boundingBox.Max.Z);
			if (num28 < num27)
			{
				face = 0;
			}
			return new TerrainRaycastResult?(new TerrainRaycastResult
			{
				Ray = ray,
				Value = cellValue,
				CellFace = new CellFace
				{
					X = num2,
					Y = num3,
					Z = num4,
					Face = face
				},
				CollisionBoxIndex = collisionBoxIndex,
				Distance = num23.Value
			});
			IL_46D:
			return null;
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0005C7D0 File Offset: 0x0005A9D0
		public void ChangeCell(int x, int y, int z, int value, bool updateModificationCounter = true)
		{
			if (!this.Terrain.IsCellValid(x, y, z))
			{
				return;
			}
			int num = this.Terrain.GetCellValueFast(x, y, z);
			value = Terrain.ReplaceLight(value, 0);
			num = Terrain.ReplaceLight(num, 0);
			if (value == num)
			{
				return;
			}
			this.Terrain.SetCellValueFast(x, y, z, value);
			TerrainChunk chunkAtCell = this.Terrain.GetChunkAtCell(x, z);
			if (chunkAtCell != null)
			{
				if (updateModificationCounter)
				{
					chunkAtCell.ModificationCounter++;
				}
				this.TerrainUpdater.DowngradeChunkNeighborhoodState(chunkAtCell.Coords, 1, TerrainChunkState.InvalidLight, false);
			}
			this.m_modifiedCells[new Point3(x, y, z)] = true;
			int num2 = Terrain.ExtractContents(num);
			if (Terrain.ExtractContents(value) != num2)
			{
				SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(num));
				for (int i = 0; i < blockBehaviors.Length; i++)
				{
					blockBehaviors[i].OnBlockRemoved(num, value, x, y, z);
				}
				SubsystemBlockBehavior[] blockBehaviors2 = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(value));
				for (int j = 0; j < blockBehaviors2.Length; j++)
				{
					blockBehaviors2[j].OnBlockAdded(value, num, x, y, z);
				}
				return;
			}
			SubsystemBlockBehavior[] blockBehaviors3 = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(value));
			for (int k = 0; k < blockBehaviors3.Length; k++)
			{
				blockBehaviors3[k].OnBlockModified(value, num, x, y, z);
			}
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x0005C924 File Offset: 0x0005AB24
		public void DestroyCell(int toolLevel, int x, int y, int z, int newValue, bool noDrop, bool noParticleSystem)
		{
			int cellValue = this.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			if (num != 0)
			{
				bool flag = true;
				if (!noDrop)
				{
					this.m_dropValues.Clear();
					block.GetDropValues(this, cellValue, newValue, toolLevel, this.m_dropValues, out flag);
					for (int i = 0; i < this.m_dropValues.Count; i++)
					{
						BlockDropValue blockDropValue = this.m_dropValues[i];
						if (blockDropValue.Count > 0)
						{
							SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(blockDropValue.Value));
							for (int j = 0; j < blockBehaviors.Length; j++)
							{
								blockBehaviors[j].OnItemHarvested(x, y, z, cellValue, ref blockDropValue, ref newValue);
							}
							if (blockDropValue.Count > 0 && Terrain.ExtractContents(blockDropValue.Value) != 0)
							{
								Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
								this.m_subsystemPickables.AddPickable(blockDropValue.Value, blockDropValue.Count, position, null, null);
							}
						}
					}
				}
				if (flag && !noParticleSystem && this.m_subsystemViews.CalculateDistanceFromNearestView(new Vector3((float)x, (float)y, (float)z)) < 16f)
				{
					this.m_subsystemParticles.AddParticleSystem(block.CreateDebrisParticleSystem(this, new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f), cellValue, 1f));
				}
			}
			this.ChangeCell(x, y, z, newValue, true);
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x0005CAC8 File Offset: 0x0005ACC8
		public void Draw(Camera camera, int drawOrder)
		{
			if (SubsystemTerrain.TerrainRenderingEnabled)
			{
				if (drawOrder == this.DrawOrders[0])
				{
					this.TerrainUpdater.PrepareForDrawing(camera);
					this.TerrainRenderer.PrepareForDrawing(camera);
					this.TerrainRenderer.DrawOpaque(camera);
					this.TerrainRenderer.DrawAlphaTested(camera);
					return;
				}
				if (drawOrder == SubsystemTerrain.m_drawOrders[1])
				{
					this.TerrainRenderer.DrawTransparent(camera);
				}
			}
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x0005CB2E File Offset: 0x0005AD2E
		public void Update(float dt)
		{
			this.TerrainUpdater.Update();
			this.ProcessModifiedCells();
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x0005CB44 File Offset: 0x0005AD44
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
			this.SubsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.SubsystemAnimatedTextures = base.Project.FindSubsystem<SubsystemAnimatedTextures>(true);
			this.SubsystemFurnitureBlockBehavior = base.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
			this.m_subsystemsky = base.Project.FindSubsystem<SubsystemSky>();
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>();
			this.m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>();
			this.SubsystemPalette = base.Project.FindSubsystem<SubsystemPalette>(true);
			this.Terrain = new Terrain();
			this.TerrainRenderer = new TerrainRenderer(this);
			this.TerrainUpdater = new TerrainUpdater(this);
			this.TerrainSerializer = new TerrainSerializer22(this.Terrain, this.SubsystemGameInfo.DirectoryName);
			this.BlockGeometryGenerator = new BlockGeometryGenerator(this.Terrain, this, base.Project.FindSubsystem<SubsystemElectricity>(true), this.SubsystemFurnitureBlockBehavior, base.Project.FindSubsystem<SubsystemMetersBlockBehavior>(true), this.SubsystemPalette);
			if (string.CompareOrdinal(this.SubsystemGameInfo.WorldSettings.OriginalSerializationVersion, "2.1") <= 0)
			{
				TerrainGenerationMode terrainGenerationMode = this.SubsystemGameInfo.WorldSettings.TerrainGenerationMode;
				ITerrainContentsGenerator terrainContentsGenerator2;
				if (terrainGenerationMode != TerrainGenerationMode.FlatContinent && terrainGenerationMode != TerrainGenerationMode.FlatIsland)
				{
					ITerrainContentsGenerator terrainContentsGenerator = new TerrainContentsGenerator21(this);
					terrainContentsGenerator2 = terrainContentsGenerator;
				}
				else
				{
					ITerrainContentsGenerator terrainContentsGenerator = new TerrainContentsGeneratorFlat(this);
					terrainContentsGenerator2 = terrainContentsGenerator;
				}
				this.TerrainContentsGenerator = terrainContentsGenerator2;
				return;
			}
			TerrainGenerationMode terrainGenerationMode2 = this.SubsystemGameInfo.WorldSettings.TerrainGenerationMode;
			ITerrainContentsGenerator terrainContentsGenerator3;
			if (terrainGenerationMode2 != TerrainGenerationMode.FlatContinent && terrainGenerationMode2 != TerrainGenerationMode.FlatIsland)
			{
				ITerrainContentsGenerator terrainContentsGenerator = new TerrainContentsGenerator22(this);
				terrainContentsGenerator3 = terrainContentsGenerator;
			}
			else
			{
				ITerrainContentsGenerator terrainContentsGenerator = new TerrainContentsGeneratorFlat(this);
				terrainContentsGenerator3 = terrainContentsGenerator;
			}
			this.TerrainContentsGenerator = terrainContentsGenerator3;
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x0005CD0C File Offset: 0x0005AF0C
		public override void Save(ValuesDictionary valuesDictionary)
		{
			this.TerrainUpdater.UpdateEvent.WaitOne();
			try
			{
				foreach (TerrainChunk chunk in this.Terrain.AllocatedChunks)
				{
					this.TerrainSerializer.SaveChunk(chunk);
				}
			}
			finally
			{
				this.TerrainUpdater.UpdateEvent.Set();
			}
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x0005CD78 File Offset: 0x0005AF78
		public override void Dispose()
		{
			this.TerrainRenderer.Dispose();
			this.TerrainUpdater.Dispose();
			this.TerrainSerializer.Dispose();
			this.Terrain.Dispose();
		}

		// Token: 0x04000689 RID: 1673
		public static bool TerrainRenderingEnabled = true;

		// Token: 0x0400068A RID: 1674
		public Dictionary<Point3, bool> m_modifiedCells = new Dictionary<Point3, bool>();

		// Token: 0x0400068B RID: 1675
		public DynamicArray<Point3> m_modifiedList = new DynamicArray<Point3>();

		// Token: 0x0400068C RID: 1676
		public static Point3[] m_neighborOffsets = new Point3[]
		{
			new Point3(0, 0, 0),
			new Point3(-1, 0, 0),
			new Point3(1, 0, 0),
			new Point3(0, -1, 0),
			new Point3(0, 1, 0),
			new Point3(0, 0, -1),
			new Point3(0, 0, 1)
		};

		// Token: 0x0400068D RID: 1677
		public SubsystemSky m_subsystemsky;

		// Token: 0x0400068E RID: 1678
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400068F RID: 1679
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x04000690 RID: 1680
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x04000691 RID: 1681
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000692 RID: 1682
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x04000693 RID: 1683
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x04000694 RID: 1684
		public List<BlockDropValue> m_dropValues = new List<BlockDropValue>();

		// Token: 0x04000695 RID: 1685
		public static int[] m_drawOrders = new int[]
		{
			0,
			100
		};
	}
}
