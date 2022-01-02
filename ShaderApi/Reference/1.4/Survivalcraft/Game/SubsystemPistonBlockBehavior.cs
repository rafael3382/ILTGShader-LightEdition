using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C4 RID: 452
	public class SubsystemPistonBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000BD3 RID: 3027 RVA: 0x00051DFB File Offset: 0x0004FFFB
		public UpdateOrder UpdateOrder
		{
			get
			{
				return this.m_subsystemMovingBlocks.UpdateOrder + 1;
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000BD4 RID: 3028 RVA: 0x00051E0A File Offset: 0x0005000A
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x00051E14 File Offset: 0x00050014
		public void AdjustPiston(Point3 position, int length)
		{
			SubsystemPistonBlockBehavior.QueuedAction queuedAction;
			if (!this.m_actions.TryGetValue(position, out queuedAction))
			{
				queuedAction = new SubsystemPistonBlockBehavior.QueuedAction();
				this.m_actions[position] = queuedAction;
			}
			queuedAction.Move = new int?(length);
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x00051E50 File Offset: 0x00050050
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(0.125, 0.0))
			{
				this.ProcessQueuedActions();
			}
			this.UpdateMovableBlocks();
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x00051E80 File Offset: 0x00050080
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			int value = inventory.GetSlotValue(slotIndex);
			int count = inventory.GetSlotCount(slotIndex);
			int data = Terrain.ExtractData(value);
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditPistonDialog(data, delegate(int newData)
			{
				int num = Terrain.ReplaceData(value, newData);
				if (num != value)
				{
					inventory.RemoveSlotItems(slotIndex, count);
					inventory.AddSlotItems(slotIndex, num, 1);
				}
			}));
			return true;
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x00051EFC File Offset: 0x000500FC
		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			int contents = Terrain.ExtractContents(value);
			int data = Terrain.ExtractData(value);
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditPistonDialog(data, delegate(int newData)
			{
				if (newData != data && this.SubsystemTerrain.Terrain.GetCellContents(x, y, z) == contents)
				{
					int value2 = Terrain.ReplaceData(value, newData);
					this.SubsystemTerrain.ChangeCell(x, y, z, value2, true);
					SubsystemElectricity subsystemElectricity = this.Project.FindSubsystem<SubsystemElectricity>(true);
					ElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, 0);
					if (electricElement != null)
					{
						subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
					}
				}
			}));
			return true;
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x00051F7C File Offset: 0x0005017C
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			int num = Terrain.ExtractContents(value);
			int data = Terrain.ExtractData(value);
			if (num != 237)
			{
				if (num != 238)
				{
					return;
				}
				if (!this.m_allowPistonHeadRemove)
				{
					int face = PistonHeadBlock.GetFace(data);
					Point3 point = CellFace.FaceToPoint3(face);
					int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(x + point.X, y + point.Y, z + point.Z);
					int cellValue2 = this.m_subsystemTerrain.Terrain.GetCellValue(x - point.X, y - point.Y, z - point.Z);
					int num2 = Terrain.ExtractContents(cellValue);
					int num3 = Terrain.ExtractContents(cellValue2);
					int data2 = Terrain.ExtractData(cellValue);
					int data3 = Terrain.ExtractData(cellValue2);
					if (num2 == 238 && PistonHeadBlock.GetFace(data2) == face)
					{
						this.m_subsystemTerrain.DestroyCell(0, x + point.X, y + point.Y, z + point.Z, 0, false, false);
					}
					if (num3 == 237 && PistonBlock.GetFace(data3) == face)
					{
						this.m_subsystemTerrain.DestroyCell(0, x - point.X, y - point.Y, z - point.Z, 0, false, false);
						return;
					}
					if (num3 == 238 && PistonHeadBlock.GetFace(data3) == face)
					{
						this.m_subsystemTerrain.DestroyCell(0, x - point.X, y - point.Y, z - point.Z, 0, false, false);
					}
				}
			}
			else
			{
				this.StopPiston(new Point3(x, y, z));
				int face2 = PistonBlock.GetFace(data);
				Point3 point2 = CellFace.FaceToPoint3(face2);
				int cellValue3 = this.m_subsystemTerrain.Terrain.GetCellValue(x + point2.X, y + point2.Y, z + point2.Z);
				int num4 = Terrain.ExtractContents(cellValue3);
				int data4 = Terrain.ExtractData(cellValue3);
				if (num4 == 238 && PistonHeadBlock.GetFace(data4) == face2)
				{
					this.m_subsystemTerrain.DestroyCell(0, x + point2.X, y + point2.Y, z + point2.Z, 0, false, false);
					return;
				}
			}
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x000521A0 File Offset: 0x000503A0
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			BoundingBox boundingBox = new BoundingBox(chunk.BoundingBox.Min - new Vector3(16f), chunk.BoundingBox.Max + new Vector3(16f));
			DynamicArray<IMovingBlockSet> dynamicArray = new DynamicArray<IMovingBlockSet>();
			this.m_subsystemMovingBlocks.FindMovingBlocks(boundingBox, false, dynamicArray);
			foreach (IMovingBlockSet movingBlockSet in dynamicArray)
			{
				if (movingBlockSet.Id == "Piston")
				{
					this.StopPiston((Point3)movingBlockSet.Tag);
				}
			}
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x0005225C File Offset: 0x0005045C
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemMovingBlocks = base.Project.FindSubsystem<SubsystemMovingBlocks>(true);
			this.m_subsystemMovingBlocks.Stopped += this.MovingBlocksStopped;
			this.m_subsystemMovingBlocks.CollidedWithTerrain += this.MovingBlocksCollidedWithTerrain;
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x000522E8 File Offset: 0x000504E8
		public void ProcessQueuedActions()
		{
			this.m_tmpActions.Clear();
			this.m_tmpActions.AddRange(this.m_actions);
			foreach (KeyValuePair<Point3, SubsystemPistonBlockBehavior.QueuedAction> keyValuePair in this.m_tmpActions)
			{
				Point3 key = keyValuePair.Key;
				SubsystemPistonBlockBehavior.QueuedAction value = keyValuePair.Value;
				if (Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValue(key.X, key.Y, key.Z)) != 237)
				{
					this.StopPiston(key);
					value.Move = null;
					value.Stop = false;
				}
				else if (value.Stop)
				{
					this.StopPiston(key);
					value.Stop = false;
					value.StoppedFrame = Time.FrameIndex;
				}
			}
			foreach (KeyValuePair<Point3, SubsystemPistonBlockBehavior.QueuedAction> keyValuePair2 in this.m_tmpActions)
			{
				Point3 key2 = keyValuePair2.Key;
				SubsystemPistonBlockBehavior.QueuedAction value2 = keyValuePair2.Value;
				if (value2.Move != null && !value2.Stop && Time.FrameIndex != value2.StoppedFrame && this.m_subsystemMovingBlocks.FindMovingBlocks("Piston", key2) == null)
				{
					bool flag = true;
					for (int i = -1; i <= 1; i++)
					{
						for (int j = -1; j <= 1; j++)
						{
							TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(key2.X + i * 16, key2.Z + j * 16);
							if (chunkAtCell == null || chunkAtCell.State <= TerrainChunkState.InvalidContents4)
							{
								flag = false;
							}
						}
					}
					if (flag && this.MovePiston(key2, value2.Move.Value))
					{
						value2.Move = null;
					}
				}
			}
			foreach (KeyValuePair<Point3, SubsystemPistonBlockBehavior.QueuedAction> keyValuePair3 in this.m_tmpActions)
			{
				Point3 key3 = keyValuePair3.Key;
				SubsystemPistonBlockBehavior.QueuedAction value3 = keyValuePair3.Value;
				if (value3.Move == null && !value3.Stop)
				{
					this.m_actions.Remove(key3);
				}
			}
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x0005256C File Offset: 0x0005076C
		public void UpdateMovableBlocks()
		{
			foreach (IMovingBlockSet movingBlockSet in this.m_subsystemMovingBlocks.MovingBlockSets)
			{
				if (movingBlockSet.Id == "Piston")
				{
					Point3 point = (Point3)movingBlockSet.Tag;
					int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
					if (Terrain.ExtractContents(cellValue) == 237)
					{
						int data = Terrain.ExtractData(cellValue);
						PistonMode mode = PistonBlock.GetMode(data);
						int face = PistonBlock.GetFace(data);
						Point3 point2 = CellFace.FaceToPoint3(face);
						int num = int.MaxValue;
						foreach (MovingBlock movingBlock in movingBlockSet.Blocks)
						{
							num = MathUtils.Min(num, movingBlock.Offset.X * point2.X + movingBlock.Offset.Y * point2.Y + movingBlock.Offset.Z * point2.Z);
						}
						float num2 = movingBlockSet.Position.X * (float)point2.X + movingBlockSet.Position.Y * (float)point2.Y + movingBlockSet.Position.Z * (float)point2.Z;
						float num3 = (float)(point.X * point2.X + point.Y * point2.Y + point.Z * point2.Z);
						if (num2 > num3)
						{
							if ((float)num + num2 - num3 > 1f)
							{
								movingBlockSet.SetBlock(point2 * (num - 1), Terrain.MakeBlockValue(238, 0, PistonHeadBlock.SetFace(PistonHeadBlock.SetIsShaft(PistonHeadBlock.SetMode(0, mode), true), face)));
							}
						}
						else if (num2 < num3 && (float)num + num2 - num3 <= 0f)
						{
							movingBlockSet.SetBlock(point2 * num, 0);
						}
					}
				}
			}
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x000527B4 File Offset: 0x000509B4
		public static void GetSpeedAndSmoothness(int pistonSpeed, out float speed, out Vector2 smoothness)
		{
			switch (pistonSpeed)
			{
			case 1:
				speed = 4.5f;
				smoothness = new Vector2(0.6f, 0.6f);
				return;
			case 2:
				speed = 4f;
				smoothness = new Vector2(0.9f, 0.9f);
				return;
			case 3:
				speed = 3.5f;
				smoothness = new Vector2(1.2f, 1.2f);
				return;
			default:
				speed = 5f;
				smoothness = new Vector2(0f, 0.5f);
				return;
			}
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x00052848 File Offset: 0x00050A48
		public bool MovePiston(Point3 position, int length)
		{
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			int data = Terrain.ExtractData(terrain.GetCellValue(position.X, position.Y, position.Z));
			int face = PistonBlock.GetFace(data);
			PistonMode mode = PistonBlock.GetMode(data);
			int maxExtension = PistonBlock.GetMaxExtension(data);
			int pullCount = PistonBlock.GetPullCount(data);
			int speed = PistonBlock.GetSpeed(data);
			Point3 point = CellFace.FaceToPoint3(face);
			length = MathUtils.Clamp(length, 0, maxExtension + 1);
			int num = 0;
			this.m_movingBlocks.Clear();
			Point3 point2 = point;
			while (this.m_movingBlocks.Count < 8)
			{
				int cellValue = terrain.GetCellValue(position.X + point2.X, position.Y + point2.Y, position.Z + point2.Z);
				int num2 = Terrain.ExtractContents(cellValue);
				int face2 = PistonHeadBlock.GetFace(Terrain.ExtractData(cellValue));
				if (num2 != 238 || face2 != face)
				{
					break;
				}
				DynamicArray<MovingBlock> movingBlocks = this.m_movingBlocks;
				MovingBlock item = new MovingBlock
				{
					Offset = point2,
					Value = cellValue
				};
				movingBlocks.Add(item);
				point2 += point;
				num++;
			}
			if (length > num)
			{
				DynamicArray<MovingBlock> movingBlocks2 = this.m_movingBlocks;
				MovingBlock item = new MovingBlock
				{
					Offset = Point3.Zero,
					Value = Terrain.MakeBlockValue(238, 0, PistonHeadBlock.SetFace(PistonHeadBlock.SetMode(PistonHeadBlock.SetIsShaft(0, num > 0), mode), face))
				};
				movingBlocks2.Add(item);
				int i = 0;
				while (i < 8)
				{
					int cellValue2 = terrain.GetCellValue(position.X + point2.X, position.Y + point2.Y, position.Z + point2.Z);
					bool flag;
					if (!SubsystemPistonBlockBehavior.IsBlockMovable(cellValue2, face, position.Y + point2.Y, out flag))
					{
						break;
					}
					DynamicArray<MovingBlock> movingBlocks3 = this.m_movingBlocks;
					item = new MovingBlock
					{
						Offset = point2,
						Value = cellValue2
					};
					movingBlocks3.Add(item);
					i++;
					point2 += point;
					if (flag)
					{
						break;
					}
				}
				if (!SubsystemPistonBlockBehavior.IsBlockBlocking(terrain.GetCellValue(position.X + point2.X, position.Y + point2.Y, position.Z + point2.Z)))
				{
					float speed2;
					Vector2 smoothness;
					SubsystemPistonBlockBehavior.GetSpeedAndSmoothness(speed, out speed2, out smoothness);
					Point3 p = position + (length - num) * point;
					if (this.m_subsystemMovingBlocks.AddMovingBlockSet(new Vector3(position) + 0.01f * new Vector3(point), new Vector3(p), speed2, 0f, 0f, smoothness, this.m_movingBlocks, "Piston", position, true) != null)
					{
						this.m_allowPistonHeadRemove = true;
						try
						{
							foreach (MovingBlock movingBlock in this.m_movingBlocks)
							{
								if (movingBlock.Offset != Point3.Zero)
								{
									this.m_subsystemTerrain.ChangeCell(position.X + movingBlock.Offset.X, position.Y + movingBlock.Offset.Y, position.Z + movingBlock.Offset.Z, 0, true);
								}
							}
						}
						finally
						{
							this.m_allowPistonHeadRemove = false;
						}
						this.m_subsystemTerrain.ChangeCell(position.X, position.Y, position.Z, Terrain.MakeBlockValue(237, 0, PistonBlock.SetIsExtended(data, true)), true);
						this.m_subsystemAudio.PlaySound("Audio/Piston", 1f, 0f, new Vector3(position), 2f, true);
					}
				}
				return false;
			}
			if (length < num)
			{
				if (mode != PistonMode.Pushing)
				{
					int num3 = 0;
					for (int j = 0; j < pullCount + 1; j++)
					{
						int cellValue3 = terrain.GetCellValue(position.X + point2.X, position.Y + point2.Y, position.Z + point2.Z);
						bool flag2;
						if (!SubsystemPistonBlockBehavior.IsBlockMovable(cellValue3, face, position.Y + point2.Y, out flag2))
						{
							break;
						}
						DynamicArray<MovingBlock> movingBlocks4 = this.m_movingBlocks;
						MovingBlock item = new MovingBlock
						{
							Offset = point2,
							Value = cellValue3
						};
						movingBlocks4.Add(item);
						point2 += point;
						num3++;
						if (flag2)
						{
							break;
						}
					}
					if (mode == PistonMode.StrictPulling && num3 < pullCount + 1)
					{
						return false;
					}
				}
				float speed3;
				Vector2 smoothness2;
				SubsystemPistonBlockBehavior.GetSpeedAndSmoothness(speed, out speed3, out smoothness2);
				float s = (length == 0) ? 0.01f : 0f;
				Vector3 targetPosition = new Vector3(position) + (float)(length - num) * new Vector3(point) + s * new Vector3(point);
				if (this.m_subsystemMovingBlocks.AddMovingBlockSet(new Vector3(position), targetPosition, speed3, 0f, 0f, smoothness2, this.m_movingBlocks, "Piston", position, true) != null)
				{
					this.m_allowPistonHeadRemove = true;
					try
					{
						foreach (MovingBlock movingBlock2 in this.m_movingBlocks)
						{
							this.m_subsystemTerrain.ChangeCell(position.X + movingBlock2.Offset.X, position.Y + movingBlock2.Offset.Y, position.Z + movingBlock2.Offset.Z, 0, true);
						}
					}
					finally
					{
						this.m_allowPistonHeadRemove = false;
					}
					this.m_subsystemAudio.PlaySound("Audio/Piston", 1f, 0f, new Vector3(position), 2f, true);
				}
				return false;
			}
			return true;
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x00052E2C File Offset: 0x0005102C
		public void StopPiston(Point3 position)
		{
			IMovingBlockSet movingBlockSet = this.m_subsystemMovingBlocks.FindMovingBlocks("Piston", position);
			if (movingBlockSet != null)
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(position.X, position.Y, position.Z);
				int num = Terrain.ExtractContents(cellValue);
				int data = Terrain.ExtractData(cellValue);
				bool flag = num == 237;
				bool isExtended = false;
				this.m_subsystemMovingBlocks.RemoveMovingBlockSet(movingBlockSet);
				foreach (MovingBlock movingBlock in movingBlockSet.Blocks)
				{
					int x = Terrain.ToCell(MathUtils.Round(movingBlockSet.Position.X)) + movingBlock.Offset.X;
					int y = Terrain.ToCell(MathUtils.Round(movingBlockSet.Position.Y)) + movingBlock.Offset.Y;
					int z = Terrain.ToCell(MathUtils.Round(movingBlockSet.Position.Z)) + movingBlock.Offset.Z;
					if (!(new Point3(x, y, z) == position))
					{
						int num2 = Terrain.ExtractContents(movingBlock.Value);
						if (flag || num2 != 238)
						{
							this.m_subsystemTerrain.DestroyCell(0, x, y, z, movingBlock.Value, false, false);
							if (num2 == 238)
							{
								isExtended = true;
							}
						}
					}
				}
				if (flag)
				{
					this.m_subsystemTerrain.ChangeCell(position.X, position.Y, position.Z, Terrain.MakeBlockValue(237, 0, PistonBlock.SetIsExtended(data, isExtended)), true);
				}
			}
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x00052FDC File Offset: 0x000511DC
		public void MovingBlocksCollidedWithTerrain(IMovingBlockSet movingBlockSet, Point3 p)
		{
			if (!(movingBlockSet.Id == "Piston"))
			{
				return;
			}
			Point3 point = (Point3)movingBlockSet.Tag;
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
			if (Terrain.ExtractContents(cellValue) != 237)
			{
				return;
			}
			Point3 point2 = CellFace.FaceToPoint3(PistonBlock.GetFace(Terrain.ExtractData(cellValue)));
			int num = p.X * point2.X + p.Y * point2.Y + p.Z * point2.Z;
			int num2 = point.X * point2.X + point.Y * point2.Y + point.Z * point2.Z;
			if (num > num2)
			{
				if (SubsystemPistonBlockBehavior.IsBlockBlocking(base.SubsystemTerrain.Terrain.GetCellValue(p.X, p.Y, p.Z)))
				{
					movingBlockSet.Stop();
					return;
				}
				base.SubsystemTerrain.DestroyCell(0, p.X, p.Y, p.Z, 0, false, false);
			}
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x000530F4 File Offset: 0x000512F4
		public void MovingBlocksStopped(IMovingBlockSet movingBlockSet)
		{
			if (!(movingBlockSet.Id == "Piston") || !(movingBlockSet.Tag is Point3))
			{
				return;
			}
			Point3 point = (Point3)movingBlockSet.Tag;
			if (Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z)) == 237)
			{
				SubsystemPistonBlockBehavior.QueuedAction queuedAction;
				if (!this.m_actions.TryGetValue(point, out queuedAction))
				{
					queuedAction = new SubsystemPistonBlockBehavior.QueuedAction();
					this.m_actions.Add(point, queuedAction);
				}
				queuedAction.Stop = true;
			}
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x00053188 File Offset: 0x00051388
		public static bool IsBlockMovable(int value, int pistonFace, int y, out bool isEnd)
		{
			isEnd = false;
			int num = Terrain.ExtractContents(value);
			int data = Terrain.ExtractData(value);
			if (num <= 132)
			{
				if (num <= 27)
				{
					if (num == 1)
					{
						return y > 1;
					}
					if (num != 27)
					{
						goto IL_B1;
					}
				}
				else if (num != 45 && num - 64 > 1)
				{
					switch (num)
					{
					case 126:
						return false;
					case 127:
						return false;
					case 128:
					case 129:
					case 130:
						goto IL_B1;
					case 131:
					case 132:
						return false;
					default:
						goto IL_B1;
					}
				}
			}
			else if (num <= 227)
			{
				if (num != 216)
				{
					if (num != 227)
					{
						goto IL_B1;
					}
					return true;
				}
			}
			else
			{
				if (num == 237)
				{
					return !PistonBlock.GetIsExtended(data);
				}
				if (num == 238)
				{
					return false;
				}
				if (num != 244)
				{
					goto IL_B1;
				}
				return false;
			}
			return false;
			IL_B1:
			Block block = BlocksManager.Blocks[num];
			if (block is BottomSuckerBlock)
			{
				return false;
			}
			if (block is MountedElectricElementBlock)
			{
				isEnd = true;
				return ((MountedElectricElementBlock)block).GetFace(value) == pistonFace;
			}
			if (block is DoorBlock || block is TrapdoorBlock)
			{
				return false;
			}
			if (block is LadderBlock)
			{
				isEnd = true;
				return pistonFace == LadderBlock.GetFace(data);
			}
			if (block is AttachedSignBlock)
			{
				isEnd = true;
				return pistonFace == AttachedSignBlock.GetFace(data);
			}
			return !block.IsNonDuplicable_(value) && block.IsCollidable_(value);
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x000532C6 File Offset: 0x000514C6
		public static bool IsBlockBlocking(int value)
		{
			return BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable_(value);
		}

		// Token: 0x040005DB RID: 1499
		public SubsystemTime m_subsystemTime;

		// Token: 0x040005DC RID: 1500
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040005DD RID: 1501
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040005DE RID: 1502
		public SubsystemMovingBlocks m_subsystemMovingBlocks;

		// Token: 0x040005DF RID: 1503
		public bool m_allowPistonHeadRemove;

		// Token: 0x040005E0 RID: 1504
		public Dictionary<Point3, SubsystemPistonBlockBehavior.QueuedAction> m_actions = new Dictionary<Point3, SubsystemPistonBlockBehavior.QueuedAction>();

		// Token: 0x040005E1 RID: 1505
		public List<KeyValuePair<Point3, SubsystemPistonBlockBehavior.QueuedAction>> m_tmpActions = new List<KeyValuePair<Point3, SubsystemPistonBlockBehavior.QueuedAction>>();

		// Token: 0x040005E2 RID: 1506
		public DynamicArray<MovingBlock> m_movingBlocks = new DynamicArray<MovingBlock>();

		// Token: 0x040005E3 RID: 1507
		public const string IdString = "Piston";

		// Token: 0x040005E4 RID: 1508
		public const int PistonMaxMovedBlocks = 8;

		// Token: 0x040005E5 RID: 1509
		public const int PistonMaxExtension = 8;

		// Token: 0x040005E6 RID: 1510
		public const int PistonMaxSpeedSetting = 3;

		// Token: 0x0200049F RID: 1183
		public class QueuedAction
		{
			// Token: 0x04001704 RID: 5892
			public int StoppedFrame;

			// Token: 0x04001705 RID: 5893
			public bool Stop;

			// Token: 0x04001706 RID: 5894
			public int? Move;
		}
	}
}
