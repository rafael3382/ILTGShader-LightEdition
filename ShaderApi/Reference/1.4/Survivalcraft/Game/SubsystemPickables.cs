using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C3 RID: 451
	public class SubsystemPickables : Subsystem, IDrawable, IUpdateable
	{
		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000BC5 RID: 3013 RVA: 0x000508D5 File Offset: 0x0004EAD5
		public ReadOnlyList<Pickable> Pickables
		{
			get
			{
				return new ReadOnlyList<Pickable>(this.m_pickables);
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000BC6 RID: 3014 RVA: 0x000508E2 File Offset: 0x0004EAE2
		public int[] DrawOrders
		{
			get
			{
				return SubsystemPickables.m_drawOrders;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000BC7 RID: 3015 RVA: 0x000508E9 File Offset: 0x0004EAE9
		// (set) Token: 0x06000BC8 RID: 3016 RVA: 0x000508F1 File Offset: 0x0004EAF1
		public virtual Action<Pickable> PickableAdded { get; set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000BC9 RID: 3017 RVA: 0x000508FA File Offset: 0x0004EAFA
		// (set) Token: 0x06000BCA RID: 3018 RVA: 0x00050902 File Offset: 0x0004EB02
		public virtual Action<Pickable> PickableRemoved { get; set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000BCB RID: 3019 RVA: 0x0005090B File Offset: 0x0004EB0B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x00050910 File Offset: 0x0004EB10
		public Pickable AddPickable(int value, int count, Vector3 position, Vector3? velocity, Matrix? stuckMatrix)
		{
			Pickable pickable = new Pickable();
			pickable.Value = value;
			pickable.Count = count;
			pickable.Position = position;
			pickable.StuckMatrix = stuckMatrix;
			pickable.CreationTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			if (velocity != null)
			{
				pickable.Velocity = velocity.Value;
			}
			else if (Terrain.ExtractContents(value) == 248)
			{
				Vector2 vector = this.m_random.Vector2(1.5f, 2f);
				pickable.Velocity = new Vector3(vector.X, 3f, vector.Y);
			}
			else
			{
				pickable.Velocity = new Vector3(this.m_random.Float(-0.5f, 0.5f), this.m_random.Float(1f, 1.2f), this.m_random.Float(-0.5f, 0.5f));
			}
			this.m_pickables.Add(pickable);
			Action<Pickable> pickableAdded = this.PickableAdded;
			if (pickableAdded != null)
			{
				pickableAdded(pickable);
			}
			return pickable;
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x00050A18 File Offset: 0x0004EC18
		public void Draw(Camera camera, int drawOrder)
		{
			double totalElapsedGameTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			this.m_drawBlockEnvironmentData.SubsystemTerrain = this.m_subsystemTerrain;
			Matrix matrix = Matrix.CreateRotationY((float)MathUtils.Remainder(totalElapsedGameTime, 6.2831854820251465));
			float num = MathUtils.Min(this.m_subsystemSky.VisibilityRange, 30f);
			foreach (Pickable pickable in this.m_pickables)
			{
				Vector3 position = pickable.Position;
				float num2 = Vector3.Dot(camera.ViewDirection, position - camera.ViewPosition);
				if (num2 > -0.5f && num2 < num)
				{
					int num3 = Terrain.ExtractContents(pickable.Value);
					Block block = BlocksManager.Blocks[num3];
					float num4 = (float)(totalElapsedGameTime - pickable.CreationTime);
					if (pickable.StuckMatrix == null)
					{
						position.Y += 0.25f * MathUtils.Saturate(3f * num4);
					}
					int x = Terrain.ToCell(position.X);
					int num5 = Terrain.ToCell(position.Y);
					int z = Terrain.ToCell(position.Z);
					TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(x, z);
					if (chunkAtCell != null && chunkAtCell.State >= TerrainChunkState.InvalidVertices1 && num5 >= 0 && num5 < 255)
					{
						this.m_drawBlockEnvironmentData.Humidity = this.m_subsystemTerrain.Terrain.GetSeasonalHumidity(x, z);
						this.m_drawBlockEnvironmentData.Temperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(num5);
						float f = MathUtils.Max(position.Y - (float)num5 - 0.75f, 0f) / 0.25f;
						pickable.Light = (int)MathUtils.Lerp((float)this.m_subsystemTerrain.Terrain.GetCellLightFast(x, num5, z), (float)this.m_subsystemTerrain.Terrain.GetCellLightFast(x, num5 + 1, z), f);
					}
					this.m_drawBlockEnvironmentData.Light = pickable.Light;
					this.m_drawBlockEnvironmentData.BillboardDirection = new Vector3?(pickable.Position - camera.ViewPosition);
					this.m_drawBlockEnvironmentData.InWorldMatrix.Translation = position;
					if (pickable.StuckMatrix != null)
					{
						Matrix value = pickable.StuckMatrix.Value;
						block.DrawBlock(this.m_primitivesRenderer, pickable.Value, Color.White, 0.3f, ref value, this.m_drawBlockEnvironmentData);
					}
					else
					{
						matrix.Translation = position + new Vector3(0f, 0.04f * MathUtils.Sin(3f * num4), 0f);
						block.DrawBlock(this.m_primitivesRenderer, pickable.Value, Color.White, 0.3f, ref matrix, this.m_drawBlockEnvironmentData);
					}
				}
			}
			this.m_primitivesRenderer.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x00050D50 File Offset: 0x0004EF50
		public void Update(float dt)
		{
			double totalElapsedGameTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			float s = MathUtils.Pow(0.5f, dt);
			float num = MathUtils.Pow(0.001f, dt);
			this.m_tmpPlayers.Clear();
			foreach (ComponentPlayer componentPlayer in this.m_subsystemPlayers.ComponentPlayers)
			{
				if (componentPlayer.ComponentHealth.Health > 0f)
				{
					this.m_tmpPlayers.Add(componentPlayer);
				}
			}
			foreach (Pickable pickable in this.m_pickables)
			{
				if (pickable.ToRemove)
				{
					this.m_pickablesToRemove.Add(pickable);
				}
				else
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(pickable.Value)];
					int num2 = this.m_pickables.Count - this.m_pickablesToRemove.Count;
					float num3 = MathUtils.Lerp(300f, 90f, MathUtils.Saturate((float)num2 / 60f));
					double num4 = totalElapsedGameTime - pickable.CreationTime;
					if (num4 > (double)num3)
					{
						pickable.ToRemove = true;
					}
					else
					{
						TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Z));
						if (chunkAtCell != null && chunkAtCell.State > TerrainChunkState.InvalidContents4)
						{
							Vector3 position = pickable.Position;
							Vector3 vector = position + pickable.Velocity * dt;
							if (pickable.FlyToPosition == null && num4 > 0.5)
							{
								foreach (ComponentPlayer componentPlayer2 in this.m_tmpPlayers)
								{
									ComponentBody componentBody = componentPlayer2.ComponentBody;
									Vector3 v = componentBody.Position + new Vector3(0f, 0.75f, 0f);
									float num5 = (v - pickable.Position).LengthSquared();
									if (num5 < 3.0625f)
									{
										bool flag = Terrain.ExtractContents(pickable.Value) == 248;
										IInventory inventory = componentPlayer2.ComponentMiner.Inventory;
										if (flag || ComponentInventoryBase.FindAcquireSlotForItem(inventory, pickable.Value) >= 0)
										{
											if (num5 < 1f)
											{
												if (flag)
												{
													componentPlayer2.ComponentLevel.AddExperience(pickable.Count, true);
													pickable.ToRemove = true;
												}
												else
												{
													pickable.Count = ComponentInventoryBase.AcquireItems(inventory, pickable.Value, pickable.Count);
													if (pickable.Count == 0)
													{
														pickable.ToRemove = true;
														this.m_subsystemAudio.PlaySound("Audio/PickableCollected", 0.7f, -0.4f, pickable.Position, 2f, false);
													}
												}
											}
											else if (pickable.StuckMatrix == null)
											{
												pickable.FlyToPosition = new Vector3?(v + 0.1f * MathUtils.Sqrt(num5) * componentBody.Velocity);
											}
										}
									}
								}
							}
							if (pickable.FlyToPosition != null)
							{
								Vector3 v2 = pickable.FlyToPosition.Value - pickable.Position;
								float num6 = v2.LengthSquared();
								if (num6 >= 0.25f)
								{
									pickable.Velocity = 6f * v2 / MathUtils.Sqrt(num6);
								}
								else
								{
									pickable.FlyToPosition = null;
								}
							}
							else
							{
								FluidBlock fluidBlock;
								float? num7;
								Vector2? vector2 = this.m_subsystemFluidBlockBehavior.CalculateFlowSpeed(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y + 0.1f), Terrain.ToCell(pickable.Position.Z), out fluidBlock, out num7);
								if (pickable.StuckMatrix == null)
								{
									TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(position, vector, false, true, (int value, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable_(value));
									if (terrainRaycastResult != null)
									{
										int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(terrainRaycastResult.Value.CellFace.X, terrainRaycastResult.Value.CellFace.Y, terrainRaycastResult.Value.CellFace.Z);
										SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(cellValue));
										for (int i = 0; i < blockBehaviors.Length; i++)
										{
											blockBehaviors[i].OnHitByProjectile(terrainRaycastResult.Value.CellFace, pickable);
										}
										if (this.m_subsystemTerrain.Raycast(position, position, false, true, (int value2, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value2)].IsCollidable_(value2)) != null)
										{
											int num8 = Terrain.ToCell(position.X);
											int num9 = Terrain.ToCell(position.Y);
											int num10 = Terrain.ToCell(position.Z);
											int num11 = 0;
											int num12 = 0;
											int num13 = 0;
											int? num14 = null;
											for (int j = -3; j <= 3; j++)
											{
												for (int k = -3; k <= 3; k++)
												{
													for (int l = -3; l <= 3; l++)
													{
														int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(j + num8, k + num9, l + num10);
														if (!BlocksManager.Blocks[Terrain.ExtractContents(cellContents)].IsCollidable_(cellContents))
														{
															int num15 = j * j + k * k + l * l;
															if (num14 == null || num15 < num14.Value)
															{
																num11 = j + num8;
																num12 = k + num9;
																num13 = l + num10;
																num14 = new int?(num15);
															}
														}
													}
												}
											}
											if (num14 != null)
											{
												pickable.FlyToPosition = new Vector3?(new Vector3((float)num11, (float)num12, (float)num13) + new Vector3(0.5f));
											}
											else
											{
												pickable.ToRemove = true;
											}
										}
										else
										{
											Plane plane = terrainRaycastResult.Value.CellFace.CalculatePlane();
											bool flag2 = vector2 != null && vector2.Value != Vector2.Zero;
											if (plane.Normal.X != 0f)
											{
												float num16 = (flag2 || MathUtils.Sqrt(MathUtils.Sqr(pickable.Velocity.Y) + MathUtils.Sqr(pickable.Velocity.Z)) > 10f) ? 0.95f : 0.25f;
												pickable.Velocity *= new Vector3(0f - num16, num16, num16);
											}
											if (plane.Normal.Y != 0f)
											{
												float num17 = (flag2 || MathUtils.Sqrt(MathUtils.Sqr(pickable.Velocity.X) + MathUtils.Sqr(pickable.Velocity.Z)) > 10f) ? 0.95f : 0.25f;
												pickable.Velocity *= new Vector3(num17, 0f - num17, num17);
												if (flag2)
												{
													Pickable pickable2 = pickable;
													pickable2.Velocity.Y = pickable2.Velocity.Y + 0.1f * plane.Normal.Y;
												}
											}
											if (plane.Normal.Z != 0f)
											{
												float num18 = (flag2 || MathUtils.Sqrt(MathUtils.Sqr(pickable.Velocity.X) + MathUtils.Sqr(pickable.Velocity.Y)) > 10f) ? 0.95f : 0.25f;
												pickable.Velocity *= new Vector3(num18, num18, 0f - num18);
											}
											vector = position;
										}
									}
								}
								else
								{
									Vector3 vector3 = pickable.StuckMatrix.Value.Translation + pickable.StuckMatrix.Value.Up * block.ProjectileTipOffset;
									if (this.m_subsystemTerrain.Raycast(vector3, vector3, false, true, (int value, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable_(value)) == null)
									{
										pickable.Position = pickable.StuckMatrix.Value.Translation;
										pickable.Velocity = Vector3.Zero;
										pickable.StuckMatrix = null;
									}
								}
								if (fluidBlock is WaterBlock && !pickable.SplashGenerated)
								{
									this.m_subsystemParticles.AddParticleSystem(new WaterSplashParticleSystem(this.m_subsystemTerrain, pickable.Position, false));
									this.m_subsystemAudio.PlayRandomSound("Audio/Splashes", 1f, this.m_random.Float(-0.2f, 0.2f), pickable.Position, 6f, true);
									pickable.SplashGenerated = true;
								}
								else if (fluidBlock is MagmaBlock && !pickable.SplashGenerated)
								{
									this.m_subsystemParticles.AddParticleSystem(new MagmaSplashParticleSystem(this.m_subsystemTerrain, pickable.Position, false));
									this.m_subsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.2f, 0.2f), pickable.Position, 3f, true);
									pickable.ToRemove = true;
									pickable.SplashGenerated = true;
									this.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y), Terrain.ToCell(pickable.Position.Z), pickable.Value);
								}
								else if (fluidBlock == null)
								{
									pickable.SplashGenerated = false;
								}
								if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)(pickable.GetHashCode() % 100) / 100.0) && (this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y + 0.1f), Terrain.ToCell(pickable.Position.Z)) == 104 || this.m_subsystemFireBlockBehavior.IsCellOnFire(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y + 0.1f), Terrain.ToCell(pickable.Position.Z))))
								{
									this.m_subsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.2f, 0.2f), pickable.Position, 3f, true);
									pickable.ToRemove = true;
									this.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y), Terrain.ToCell(pickable.Position.Z), pickable.Value);
								}
								if (pickable.StuckMatrix == null)
								{
									if (vector2 != null && num7 != null)
									{
										float num19 = num7.Value - pickable.Position.Y;
										float num20 = MathUtils.Saturate(3f * num19);
										Pickable pickable3 = pickable;
										pickable3.Velocity.X = pickable3.Velocity.X + 4f * dt * (vector2.Value.X - pickable.Velocity.X);
										Pickable pickable4 = pickable;
										pickable4.Velocity.Y = pickable4.Velocity.Y - 10f * dt;
										Pickable pickable5 = pickable;
										pickable5.Velocity.Y = pickable5.Velocity.Y + 10f * (1f / block.GetDensity(pickable.Value) * num20) * dt;
										Pickable pickable6 = pickable;
										pickable6.Velocity.Z = pickable6.Velocity.Z + 4f * dt * (vector2.Value.Y - pickable.Velocity.Z);
										Pickable pickable7 = pickable;
										pickable7.Velocity.Y = pickable7.Velocity.Y * num;
									}
									else
									{
										Pickable pickable8 = pickable;
										pickable8.Velocity.Y = pickable8.Velocity.Y - 10f * dt;
										pickable.Velocity *= s;
									}
								}
							}
							pickable.Position = vector;
						}
					}
				}
			}
			foreach (Pickable pickable9 in this.m_pickablesToRemove)
			{
				this.m_pickables.Remove(pickable9);
				Action<Pickable> pickableRemoved = this.PickableRemoved;
				if (pickableRemoved != null)
				{
					pickableRemoved(pickable9);
				}
			}
			this.m_pickablesToRemove.Clear();
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x00051ABC File Offset: 0x0004FCBC
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_subsystemFireBlockBehavior = base.Project.FindSubsystem<SubsystemFireBlockBehavior>(true);
			this.m_subsystemFluidBlockBehavior = base.Project.FindSubsystem<SubsystemFluidBlockBehavior>(true);
			foreach (object obj in from v in valuesDictionary.GetValue<ValuesDictionary>("Pickables").Values
			where v is ValuesDictionary
			select v)
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)obj;
				Pickable pickable = new Pickable();
				pickable.Value = valuesDictionary2.GetValue<int>("Value");
				pickable.Count = valuesDictionary2.GetValue<int>("Count");
				pickable.Position = valuesDictionary2.GetValue<Vector3>("Position");
				pickable.Velocity = valuesDictionary2.GetValue<Vector3>("Velocity");
				pickable.CreationTime = valuesDictionary2.GetValue<double>("CreationTime", 0.0);
				if (valuesDictionary2.ContainsKey("StuckMatrix"))
				{
					pickable.StuckMatrix = new Matrix?(valuesDictionary2.GetValue<Matrix>("StuckMatrix"));
				}
				this.m_pickables.Add(pickable);
			}
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x00051C94 File Offset: 0x0004FE94
		public override void Save(ValuesDictionary valuesDictionary)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Pickables", valuesDictionary2);
			int num = 0;
			foreach (Pickable pickable in this.m_pickables)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary2.SetValue<ValuesDictionary>(num.ToString(), valuesDictionary3);
				valuesDictionary3.SetValue<int>("Value", pickable.Value);
				valuesDictionary3.SetValue<int>("Count", pickable.Count);
				valuesDictionary3.SetValue<Vector3>("Position", pickable.Position);
				valuesDictionary3.SetValue<Vector3>("Velocity", pickable.Velocity);
				valuesDictionary3.SetValue<double>("CreationTime", pickable.CreationTime);
				if (pickable.StuckMatrix != null)
				{
					valuesDictionary3.SetValue<Matrix>("StuckMatrix", pickable.StuckMatrix.Value);
				}
				num++;
			}
		}

		// Token: 0x040005C7 RID: 1479
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040005C8 RID: 1480
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x040005C9 RID: 1481
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040005CA RID: 1482
		public SubsystemSky m_subsystemSky;

		// Token: 0x040005CB RID: 1483
		public SubsystemTime m_subsystemTime;

		// Token: 0x040005CC RID: 1484
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040005CD RID: 1485
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040005CE RID: 1486
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x040005CF RID: 1487
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x040005D0 RID: 1488
		public SubsystemFireBlockBehavior m_subsystemFireBlockBehavior;

		// Token: 0x040005D1 RID: 1489
		public SubsystemFluidBlockBehavior m_subsystemFluidBlockBehavior;

		// Token: 0x040005D2 RID: 1490
		public List<ComponentPlayer> m_tmpPlayers = new List<ComponentPlayer>();

		// Token: 0x040005D3 RID: 1491
		public List<Pickable> m_pickables = new List<Pickable>();

		// Token: 0x040005D4 RID: 1492
		public List<Pickable> m_pickablesToRemove = new List<Pickable>();

		// Token: 0x040005D5 RID: 1493
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x040005D6 RID: 1494
		public Game.Random m_random = new Game.Random();

		// Token: 0x040005D7 RID: 1495
		public DrawBlockEnvironmentData m_drawBlockEnvironmentData = new DrawBlockEnvironmentData();

		// Token: 0x040005D8 RID: 1496
		public static int[] m_drawOrders = new int[]
		{
			10
		};
	}
}
