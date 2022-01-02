using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000226 RID: 550
	public class ComponentPlayer : ComponentCreature, IUpdateable
	{
		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06001166 RID: 4454 RVA: 0x00081662 File Offset: 0x0007F862
		// (set) Token: 0x06001167 RID: 4455 RVA: 0x0008166A File Offset: 0x0007F86A
		public PlayerData PlayerData { get; set; }

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06001168 RID: 4456 RVA: 0x00081673 File Offset: 0x0007F873
		public GameWidget GameWidget
		{
			get
			{
				return this.PlayerData.GameWidget;
			}
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06001169 RID: 4457 RVA: 0x00081680 File Offset: 0x0007F880
		public ContainerWidget GuiWidget
		{
			get
			{
				return this.PlayerData.GameWidget.GuiWidget;
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x0600116A RID: 4458 RVA: 0x00081692 File Offset: 0x0007F892
		public ViewWidget ViewWidget
		{
			get
			{
				return this.PlayerData.GameWidget.ViewWidget;
			}
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x0600116B RID: 4459 RVA: 0x000816A4 File Offset: 0x0007F8A4
		// (set) Token: 0x0600116C RID: 4460 RVA: 0x000816AC File Offset: 0x0007F8AC
		public ComponentGui ComponentGui { get; set; }

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x0600116D RID: 4461 RVA: 0x000816B5 File Offset: 0x0007F8B5
		// (set) Token: 0x0600116E RID: 4462 RVA: 0x000816BD File Offset: 0x0007F8BD
		public ComponentInput ComponentInput { get; set; }

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x0600116F RID: 4463 RVA: 0x000816C6 File Offset: 0x0007F8C6
		// (set) Token: 0x06001170 RID: 4464 RVA: 0x000816CE File Offset: 0x0007F8CE
		public ComponentBlockHighlight ComponentBlockHighlight { get; set; }

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06001171 RID: 4465 RVA: 0x000816D7 File Offset: 0x0007F8D7
		// (set) Token: 0x06001172 RID: 4466 RVA: 0x000816DF File Offset: 0x0007F8DF
		public ComponentScreenOverlays ComponentScreenOverlays { get; set; }

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06001173 RID: 4467 RVA: 0x000816E8 File Offset: 0x0007F8E8
		// (set) Token: 0x06001174 RID: 4468 RVA: 0x000816F0 File Offset: 0x0007F8F0
		public ComponentAimingSights ComponentAimingSights { get; set; }

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06001175 RID: 4469 RVA: 0x000816F9 File Offset: 0x0007F8F9
		// (set) Token: 0x06001176 RID: 4470 RVA: 0x00081701 File Offset: 0x0007F901
		public ComponentMiner ComponentMiner { get; set; }

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06001177 RID: 4471 RVA: 0x0008170A File Offset: 0x0007F90A
		// (set) Token: 0x06001178 RID: 4472 RVA: 0x00081712 File Offset: 0x0007F912
		public ComponentRider ComponentRider { get; set; }

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06001179 RID: 4473 RVA: 0x0008171B File Offset: 0x0007F91B
		// (set) Token: 0x0600117A RID: 4474 RVA: 0x00081723 File Offset: 0x0007F923
		public ComponentSleep ComponentSleep { get; set; }

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x0600117B RID: 4475 RVA: 0x0008172C File Offset: 0x0007F92C
		// (set) Token: 0x0600117C RID: 4476 RVA: 0x00081734 File Offset: 0x0007F934
		public ComponentVitalStats ComponentVitalStats { get; set; }

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x0600117D RID: 4477 RVA: 0x0008173D File Offset: 0x0007F93D
		// (set) Token: 0x0600117E RID: 4478 RVA: 0x00081745 File Offset: 0x0007F945
		public ComponentSickness ComponentSickness { get; set; }

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x0600117F RID: 4479 RVA: 0x0008174E File Offset: 0x0007F94E
		// (set) Token: 0x06001180 RID: 4480 RVA: 0x00081756 File Offset: 0x0007F956
		public ComponentFlu ComponentFlu { get; set; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06001181 RID: 4481 RVA: 0x0008175F File Offset: 0x0007F95F
		// (set) Token: 0x06001182 RID: 4482 RVA: 0x00081767 File Offset: 0x0007F967
		public ComponentLevel ComponentLevel { get; set; }

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06001183 RID: 4483 RVA: 0x00081770 File Offset: 0x0007F970
		// (set) Token: 0x06001184 RID: 4484 RVA: 0x00081778 File Offset: 0x0007F978
		public ComponentClothing ComponentClothing { get; set; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06001185 RID: 4485 RVA: 0x00081781 File Offset: 0x0007F981
		// (set) Token: 0x06001186 RID: 4486 RVA: 0x00081789 File Offset: 0x0007F989
		public ComponentOuterClothingModel ComponentOuterClothingModel { get; set; }

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06001187 RID: 4487 RVA: 0x00081792 File Offset: 0x0007F992
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x00081798 File Offset: 0x0007F998
		public void Update(float dt)
		{
			PlayerInput playerInput = this.ComponentInput.PlayerInput;
			if (this.ComponentInput.IsControlledByTouch && this.m_aim != null)
			{
				playerInput.Look = Vector2.Zero;
			}
			if (this.ComponentMiner.Inventory != null)
			{
				this.ComponentMiner.Inventory.ActiveSlotIndex += playerInput.ScrollInventory;
				if (playerInput.SelectInventorySlot != null)
				{
					this.ComponentMiner.Inventory.ActiveSlotIndex = MathUtils.Clamp(playerInput.SelectInventorySlot.Value, 0, 9);
				}
			}
			ComponentSteedBehavior componentSteedBehavior = null;
			ComponentBoat componentBoat = null;
			ComponentMount mount = this.ComponentRider.Mount;
			if (mount != null)
			{
				componentSteedBehavior = mount.Entity.FindComponent<ComponentSteedBehavior>();
				componentBoat = mount.Entity.FindComponent<ComponentBoat>();
			}
			if (componentSteedBehavior != null)
			{
				if (playerInput.Move.Z > 0.5f && !this.m_speedOrderBlocked)
				{
					if (this.PlayerData.PlayerClass == PlayerClass.Male)
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/MaleYellFast", 0.75f, 0f, base.ComponentBody.Position, 2f, false);
					}
					else
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/FemaleYellFast", 0.75f, 0f, base.ComponentBody.Position, 2f, false);
					}
					componentSteedBehavior.SpeedOrder = 1;
					this.m_speedOrderBlocked = true;
				}
				else if (playerInput.Move.Z < -0.5f && !this.m_speedOrderBlocked)
				{
					if (this.PlayerData.PlayerClass == PlayerClass.Male)
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/MaleYellSlow", 0.75f, 0f, base.ComponentBody.Position, 2f, false);
					}
					else
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/FemaleYellSlow", 0.75f, 0f, base.ComponentBody.Position, 2f, false);
					}
					componentSteedBehavior.SpeedOrder = -1;
					this.m_speedOrderBlocked = true;
				}
				else if (MathUtils.Abs(playerInput.Move.Z) <= 0.25f)
				{
					this.m_speedOrderBlocked = false;
				}
				componentSteedBehavior.TurnOrder = playerInput.Move.X;
				componentSteedBehavior.JumpOrder = (float)(playerInput.Jump ? 1 : 0);
				base.ComponentLocomotion.LookOrder = new Vector2(playerInput.Look.X, 0f);
			}
			else if (componentBoat != null)
			{
				componentBoat.TurnOrder = playerInput.Move.X;
				componentBoat.MoveOrder = playerInput.Move.Z;
				base.ComponentLocomotion.LookOrder = new Vector2(playerInput.Look.X, 0f);
				base.ComponentCreatureModel.RowLeftOrder = (playerInput.Move.X < -0.2f || playerInput.Move.Z > 0.2f);
				base.ComponentCreatureModel.RowRightOrder = (playerInput.Move.X > 0.2f || playerInput.Move.Z > 0.2f);
			}
			else
			{
				base.ComponentLocomotion.WalkOrder = new Vector2?(base.ComponentBody.IsSneaking ? (0.66f * new Vector2(playerInput.SneakMove.X, playerInput.SneakMove.Z)) : new Vector2(playerInput.Move.X, playerInput.Move.Z));
				base.ComponentLocomotion.FlyOrder = new Vector3?(new Vector3(0f, playerInput.Move.Y, 0f));
				base.ComponentLocomotion.TurnOrder = playerInput.Look * new Vector2(1f, 0f);
				base.ComponentLocomotion.JumpOrder = MathUtils.Max((float)(playerInput.Jump ? 1 : 0), base.ComponentLocomotion.JumpOrder);
			}
			base.ComponentLocomotion.LookOrder += playerInput.Look * (SettingsManager.FlipVerticalAxis ? new Vector2(0f, -1f) : new Vector2(0f, 1f));
			base.ComponentLocomotion.VrLookOrder = playerInput.VrLook;
			base.ComponentLocomotion.VrMoveOrder = playerInput.VrMove;
			int num = Terrain.ExtractContents(this.ComponentMiner.ActiveBlockValue);
			Block block = BlocksManager.Blocks[num];
			bool flag = false;
			if (playerInput.Interact != null && !flag && this.m_subsystemTime.GameTime - this.m_lastActionTime > 0.33000001311302185)
			{
				if (!this.ComponentMiner.Use(playerInput.Interact.Value))
				{
					TerrainRaycastResult? terrainRaycastResult = this.ComponentMiner.Raycast<TerrainRaycastResult>(playerInput.Interact.Value, RaycastMode.Interaction, true, true, true);
					if (terrainRaycastResult != null)
					{
						if (!this.ComponentMiner.Interact(terrainRaycastResult.Value))
						{
							if (this.ComponentMiner.Place(terrainRaycastResult.Value))
							{
								this.m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
								flag = true;
								this.m_isAimBlocked = true;
							}
						}
						else
						{
							this.m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
							flag = true;
							this.m_isAimBlocked = true;
						}
					}
				}
				else
				{
					this.m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
					flag = true;
					this.m_isAimBlocked = true;
				}
			}
			float num2 = (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) ? 0.1f : 1.4f;
			if (playerInput.Aim != null && block.IsAimable_(this.ComponentMiner.ActiveBlockValue) && this.m_subsystemTime.GameTime - this.m_lastActionTime > (double)num2)
			{
				if (!this.m_isAimBlocked)
				{
					Ray3 value = playerInput.Aim.Value;
					Vector3 vector = this.GameWidget.ActiveCamera.WorldToScreen(value.Position + value.Direction, Matrix.Identity);
					Point2 size = Window.Size;
					if (vector.X >= (float)size.X * 0.02f && vector.X < (float)size.X * 0.98f && vector.Y >= (float)size.Y * 0.02f && vector.Y < (float)size.Y * 0.98f)
					{
						this.m_aim = new Ray3?(value);
						if (this.ComponentMiner.Aim(value, AimState.InProgress))
						{
							this.ComponentMiner.Aim(this.m_aim.Value, AimState.Cancelled);
							this.m_aim = null;
							this.m_isAimBlocked = true;
						}
						else if (!this.m_aimHintIssued && Time.PeriodicEvent(1.0, 0.0))
						{
							Time.QueueTimeDelayedExecution(Time.RealTime + 3.0, delegate
							{
								if (!this.m_aimHintIssued && this.m_aim != null && !base.ComponentBody.IsSneaking)
								{
									this.m_aimHintIssued = true;
									this.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentPlayer.fName, 1), Color.White, true, true);
								}
							});
						}
					}
					else if (this.m_aim != null)
					{
						this.ComponentMiner.Aim(this.m_aim.Value, AimState.Cancelled);
						this.m_aim = null;
						this.m_isAimBlocked = true;
					}
				}
			}
			else
			{
				this.m_isAimBlocked = false;
				if (this.m_aim != null)
				{
					this.ComponentMiner.Aim(this.m_aim.Value, AimState.Completed);
					this.m_aim = null;
					this.m_lastActionTime = this.m_subsystemTime.GameTime;
				}
			}
			flag |= (this.m_aim != null);
			if (playerInput.Hit != null && !flag && this.m_subsystemTime.GameTime - this.m_lastActionTime > 0.33000001311302185)
			{
				BodyRaycastResult? bodyRaycastResult = this.ComponentMiner.Raycast<BodyRaycastResult>(playerInput.Hit.Value, RaycastMode.Interaction, true, true, true);
				if (bodyRaycastResult != null)
				{
					flag = true;
					this.m_isDigBlocked = true;
					if (Vector3.Distance(bodyRaycastResult.Value.HitPoint(), base.ComponentCreatureModel.EyePosition) <= 2f)
					{
						this.ComponentMiner.Hit(bodyRaycastResult.Value.ComponentBody, bodyRaycastResult.Value.HitPoint(), playerInput.Hit.Value.Direction);
					}
				}
			}
			if (playerInput.Dig != null && !flag && !this.m_isDigBlocked && this.m_subsystemTime.GameTime - this.m_lastActionTime > 0.33000001311302185)
			{
				TerrainRaycastResult? terrainRaycastResult2 = this.ComponentMiner.Raycast<TerrainRaycastResult>(playerInput.Dig.Value, RaycastMode.Digging, true, true, true);
				if (terrainRaycastResult2 != null && this.ComponentMiner.Dig(terrainRaycastResult2.Value))
				{
					this.m_lastActionTime = this.m_subsystemTime.GameTime;
					this.m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
				}
			}
			if (playerInput.Dig == null)
			{
				this.m_isDigBlocked = false;
			}
			if (playerInput.Drop && this.ComponentMiner.Inventory != null)
			{
				IInventory inventory = this.ComponentMiner.Inventory;
				int slotValue = inventory.GetSlotValue(inventory.ActiveSlotIndex);
				int slotCount = inventory.GetSlotCount(inventory.ActiveSlotIndex);
				int num3 = inventory.RemoveSlotItems(inventory.ActiveSlotIndex, slotCount);
				if (slotValue != 0 && num3 != 0)
				{
					Vector3 position = base.ComponentBody.Position + new Vector3(0f, base.ComponentBody.BoxSize.Y * 0.66f, 0f) + 0.25f * base.ComponentBody.Matrix.Forward;
					Vector3 value2 = 8f * Matrix.CreateFromQuaternion(base.ComponentCreatureModel.EyeRotation).Forward;
					this.m_subsystemPickables.AddPickable(slotValue, num3, position, new Vector3?(value2), null);
				}
			}
			if (playerInput.PickBlockType == null || flag)
			{
				return;
			}
			ComponentCreativeInventory componentCreativeInventory = this.ComponentMiner.Inventory as ComponentCreativeInventory;
			if (componentCreativeInventory == null)
			{
				return;
			}
			TerrainRaycastResult? terrainRaycastResult3 = this.ComponentMiner.Raycast<TerrainRaycastResult>(playerInput.PickBlockType.Value, RaycastMode.Digging, true, false, false);
			if (terrainRaycastResult3 == null)
			{
				return;
			}
			int num4 = terrainRaycastResult3.Value.Value;
			num4 = Terrain.ReplaceLight(num4, 0);
			int num5 = Terrain.ExtractContents(num4);
			Block block2 = BlocksManager.Blocks[num5];
			int num6 = 0;
			IEnumerable<int> creativeValues = block2.GetCreativeValues();
			if (block2.GetCreativeValues().Contains(num4))
			{
				num6 = num4;
			}
			if (num6 == 0 && !block2.IsNonDuplicable_(num4))
			{
				List<BlockDropValue> list = new List<BlockDropValue>();
				bool flag2;
				block2.GetDropValues(this.m_subsystemTerrain, num4, 0, int.MaxValue, list, out flag2);
				if (list.Count > 0 && list[0].Count > 0)
				{
					num6 = list[0].Value;
				}
			}
			if (num6 == 0)
			{
				num6 = creativeValues.FirstOrDefault<int>();
			}
			if (num6 == 0)
			{
				return;
			}
			int num7 = -1;
			for (int i = 0; i < 10; i++)
			{
				if (componentCreativeInventory.GetSlotCapacity(i, num6) > 0 && componentCreativeInventory.GetSlotCount(i) > 0 && componentCreativeInventory.GetSlotValue(i) == num6)
				{
					num7 = i;
					break;
				}
			}
			if (num7 < 0)
			{
				for (int j = 0; j < 10; j++)
				{
					if (componentCreativeInventory.GetSlotCapacity(j, num6) > 0 && (componentCreativeInventory.GetSlotCount(j) == 0 || componentCreativeInventory.GetSlotValue(j) == 0))
					{
						num7 = j;
						break;
					}
				}
			}
			if (num7 < 0)
			{
				num7 = componentCreativeInventory.ActiveSlotIndex;
			}
			componentCreativeInventory.RemoveSlotItems(num7, int.MaxValue);
			componentCreativeInventory.AddSlotItems(num7, num6, 1);
			componentCreativeInventory.ActiveSlotIndex = num7;
			this.ComponentGui.DisplaySmallMessage(block2.GetDisplayName(this.m_subsystemTerrain, num4), Color.White, false, false);
			this.m_subsystemAudio.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f, 0f);
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x000823A8 File Offset: 0x000805A8
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.ComponentGui = base.Entity.FindComponent<ComponentGui>(true);
			this.ComponentInput = base.Entity.FindComponent<ComponentInput>(true);
			this.ComponentScreenOverlays = base.Entity.FindComponent<ComponentScreenOverlays>(true);
			this.ComponentBlockHighlight = base.Entity.FindComponent<ComponentBlockHighlight>(true);
			this.ComponentAimingSights = base.Entity.FindComponent<ComponentAimingSights>(true);
			this.ComponentMiner = base.Entity.FindComponent<ComponentMiner>(true);
			this.ComponentRider = base.Entity.FindComponent<ComponentRider>(true);
			this.ComponentSleep = base.Entity.FindComponent<ComponentSleep>(true);
			this.ComponentVitalStats = base.Entity.FindComponent<ComponentVitalStats>(true);
			this.ComponentSickness = base.Entity.FindComponent<ComponentSickness>(true);
			this.ComponentFlu = base.Entity.FindComponent<ComponentFlu>(true);
			this.ComponentLevel = base.Entity.FindComponent<ComponentLevel>(true);
			this.ComponentClothing = base.Entity.FindComponent<ComponentClothing>(true);
			this.ComponentOuterClothingModel = base.Entity.FindComponent<ComponentOuterClothingModel>(true);
			int playerIndex = valuesDictionary.GetValue<int>("PlayerIndex");
			this.PlayerData = base.Project.FindSubsystem<SubsystemPlayers>(true).PlayersData.First((PlayerData d) => d.PlayerIndex == playerIndex);
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x00082557 File Offset: 0x00080757
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue<int>("PlayerIndex", this.PlayerData.PlayerIndex);
		}

		// Token: 0x04000A74 RID: 2676
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000A75 RID: 2677
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A76 RID: 2678
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000A77 RID: 2679
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x04000A78 RID: 2680
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000A79 RID: 2681
		public bool m_aimHintIssued;

		// Token: 0x04000A7A RID: 2682
		public static string fName = "ComponentPlayer";

		// Token: 0x04000A7B RID: 2683
		public double m_lastActionTime;

		// Token: 0x04000A7C RID: 2684
		public bool m_speedOrderBlocked;

		// Token: 0x04000A7D RID: 2685
		public Ray3? m_aim;

		// Token: 0x04000A7E RID: 2686
		public bool m_isAimBlocked;

		// Token: 0x04000A7F RID: 2687
		public bool m_isDigBlocked;
	}
}
