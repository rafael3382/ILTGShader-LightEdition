using System;
using System.Globalization;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200021D RID: 541
	public class ComponentMiner : Component, IUpdateable
	{
		// Token: 0x1700023A RID: 570
		// (get) Token: 0x060010B7 RID: 4279 RVA: 0x0007D2C3 File Offset: 0x0007B4C3
		// (set) Token: 0x060010B8 RID: 4280 RVA: 0x0007D2CB File Offset: 0x0007B4CB
		public ComponentCreature ComponentCreature { get; set; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x060010B9 RID: 4281 RVA: 0x0007D2D4 File Offset: 0x0007B4D4
		// (set) Token: 0x060010BA RID: 4282 RVA: 0x0007D2DC File Offset: 0x0007B4DC
		public ComponentPlayer ComponentPlayer { get; set; }

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x060010BB RID: 4283 RVA: 0x0007D2E5 File Offset: 0x0007B4E5
		// (set) Token: 0x060010BC RID: 4284 RVA: 0x0007D2ED File Offset: 0x0007B4ED
		public IInventory Inventory { get; set; }

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x060010BD RID: 4285 RVA: 0x0007D2F6 File Offset: 0x0007B4F6
		public int ActiveBlockValue
		{
			get
			{
				if (this.Inventory == null)
				{
					return 0;
				}
				return this.Inventory.GetSlotValue(this.Inventory.ActiveSlotIndex);
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x060010BE RID: 4286 RVA: 0x0007D318 File Offset: 0x0007B518
		// (set) Token: 0x060010BF RID: 4287 RVA: 0x0007D320 File Offset: 0x0007B520
		public float AttackPower { get; set; }

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x060010C0 RID: 4288 RVA: 0x0007D329 File Offset: 0x0007B529
		// (set) Token: 0x060010C1 RID: 4289 RVA: 0x0007D331 File Offset: 0x0007B531
		public float PokingPhase { get; set; }

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x060010C2 RID: 4290 RVA: 0x0007D33A File Offset: 0x0007B53A
		// (set) Token: 0x060010C3 RID: 4291 RVA: 0x0007D342 File Offset: 0x0007B542
		public CellFace? DigCellFace { get; set; }

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x060010C4 RID: 4292 RVA: 0x0007D34C File Offset: 0x0007B54C
		public float DigTime
		{
			get
			{
				if (this.DigCellFace == null)
				{
					return 0f;
				}
				return (float)(this.m_subsystemTime.GameTime - this.m_digStartTime);
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x060010C5 RID: 4293 RVA: 0x0007D384 File Offset: 0x0007B584
		public float DigProgress
		{
			get
			{
				if (this.DigCellFace == null)
				{
					return 0f;
				}
				return this.m_digProgress;
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x060010C6 RID: 4294 RVA: 0x0007D3AD File Offset: 0x0007B5AD
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x0007D3B0 File Offset: 0x0007B5B0
		public virtual void Poke(bool forceRestart)
		{
			this.PokingPhase = (forceRestart ? 0.0001f : MathUtils.Max(0.0001f, this.PokingPhase));
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x0007D3D4 File Offset: 0x0007B5D4
		public bool Dig(TerrainRaycastResult raycastResult)
		{
			bool result = false;
			this.m_lastDigFrameIndex = Time.FrameIndex;
			CellFace cellFace = raycastResult.CellFace;
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			int activeBlockValue = this.ActiveBlockValue;
			int num2 = Terrain.ExtractContents(activeBlockValue);
			Block block2 = BlocksManager.Blocks[num2];
			if (this.DigCellFace == null || this.DigCellFace.Value.X != cellFace.X || this.DigCellFace.Value.Y != cellFace.Y || this.DigCellFace.Value.Z != cellFace.Z)
			{
				this.m_digStartTime = this.m_subsystemTime.GameTime;
				this.DigCellFace = new CellFace?(cellFace);
			}
			float num3 = this.CalculateDigTime(cellValue, activeBlockValue);
			this.m_digProgress = ((num3 > 0f) ? MathUtils.Saturate((float)(this.m_subsystemTime.GameTime - this.m_digStartTime) / num3) : 1f);
			if (!this.CanUseTool(activeBlockValue))
			{
				this.m_digProgress = 0f;
				if (this.m_subsystemTime.PeriodicGameTimeEvent(5.0, this.m_digStartTime + 1.0))
				{
					ComponentPlayer componentPlayer = this.ComponentPlayer;
					if (componentPlayer != null)
					{
						componentPlayer.ComponentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentMiner.fName, 1), block2.PlayerLevelRequired, block2.GetDisplayName(this.m_subsystemTerrain, activeBlockValue)), Color.White, true, true);
					}
				}
			}
			bool flag = this.ComponentPlayer != null && !this.ComponentPlayer.ComponentInput.IsControlledByTouch && this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative;
			ModsManager.HookAction("OnMinerDig", delegate(ModLoader modLoader)
			{
				bool flag;
				modLoader.OnMinerDig(this, raycastResult, ref this.m_digProgress, out flag);
				flag = (flag || flag);
				return false;
			});
			if (flag || (this.m_lastPokingPhase <= 0.5f && this.PokingPhase > 0.5f))
			{
				if (this.m_digProgress >= 1f)
				{
					this.DigCellFace = null;
					if (flag)
					{
						this.Poke(true);
					}
					BlockPlacementData digValue = block.GetDigValue(this.m_subsystemTerrain, this, cellValue, activeBlockValue, raycastResult);
					this.m_subsystemTerrain.DestroyCell(block2.ToolLevel, digValue.CellFace.X, digValue.CellFace.Y, digValue.CellFace.Z, digValue.Value, false, false);
					this.m_subsystemSoundMaterials.PlayImpactSound(cellValue, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2f);
					this.DamageActiveTool(1);
					if (this.ComponentCreature.PlayerStats != null)
					{
						this.ComponentCreature.PlayerStats.BlocksDug += 1L;
					}
					result = true;
				}
				else
				{
					this.m_subsystemSoundMaterials.PlayImpactSound(cellValue, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 1f);
					BlockDebrisParticleSystem particleSystem = block.CreateDebrisParticleSystem(this.m_subsystemTerrain, raycastResult.HitPoint(0.1f), cellValue, 0.35f);
					base.Project.FindSubsystem<SubsystemParticles>(true).AddParticleSystem(particleSystem);
				}
			}
			return result;
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x0007D75A File Offset: 0x0007B95A
		public bool Place(TerrainRaycastResult raycastResult)
		{
			if (this.Place(raycastResult, this.ActiveBlockValue))
			{
				if (this.Inventory != null)
				{
					this.Inventory.RemoveSlotItems(this.Inventory.ActiveSlotIndex, 1);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x0007D790 File Offset: 0x0007B990
		public bool Place(TerrainRaycastResult raycastResult, int value)
		{
			int num = Terrain.ExtractContents(value);
			if (BlocksManager.Blocks[num].IsPlaceable_(value))
			{
				Block block = BlocksManager.Blocks[num];
				BlockPlacementData placementValue = block.GetPlacementValue(this.m_subsystemTerrain, this, value, raycastResult);
				if (placementValue.Value != 0)
				{
					Point3 point = CellFace.FaceToPoint3(placementValue.CellFace.Face);
					int num2 = placementValue.CellFace.X + point.X;
					int num3 = placementValue.CellFace.Y + point.Y;
					int num4 = placementValue.CellFace.Z + point.Z;
					bool pass = false;
					ModsManager.HookAction("JumpToPlace", delegate(ModLoader loader)
					{
						loader.JumpToPlace(out pass);
						return true;
					});
					if (num3 > 0 && num3 < 255 && (pass || ComponentMiner.IsBlockPlacingAllowed(this.ComponentCreature.ComponentBody) || this.m_subsystemGameInfo.WorldSettings.GameMode <= GameMode.Harmless))
					{
						bool flag = false;
						if (block.IsCollidable)
						{
							BoundingBox boundingBox = this.ComponentCreature.ComponentBody.BoundingBox;
							boundingBox.Min += new Vector3(0.2f);
							boundingBox.Max -= new Vector3(0.2f);
							foreach (BoundingBox box in block.GetCustomCollisionBoxes(this.m_subsystemTerrain, placementValue.Value))
							{
								box.Min += new Vector3((float)num2, (float)num3, (float)num4);
								box.Max += new Vector3((float)num2, (float)num3, (float)num4);
								if (boundingBox.Intersection(box))
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(placementValue.Value));
							for (int j = 0; j < blockBehaviors.Length; j++)
							{
								blockBehaviors[j].OnItemPlaced(num2, num3, num4, ref placementValue, value);
							}
							this.m_subsystemTerrain.DestroyCell(0, num2, num3, num4, placementValue.Value, false, false);
							this.m_subsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, new Vector3((float)placementValue.CellFace.X, (float)placementValue.CellFace.Y, (float)placementValue.CellFace.Z), 5f, false);
							this.Poke(false);
							if (this.ComponentCreature.PlayerStats != null)
							{
								this.ComponentCreature.PlayerStats.BlocksPlaced += 1L;
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x0007DA58 File Offset: 0x0007BC58
		public bool Use(Ray3 ray)
		{
			int num = Terrain.ExtractContents(this.ActiveBlockValue);
			Block block = BlocksManager.Blocks[num];
			if (!this.CanUseTool(this.ActiveBlockValue))
			{
				ComponentPlayer componentPlayer = this.ComponentPlayer;
				if (componentPlayer != null)
				{
					componentPlayer.ComponentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentMiner.fName, 1), block.PlayerLevelRequired, block.GetDisplayName(this.m_subsystemTerrain, this.ActiveBlockValue)), Color.White, true, true);
				}
				this.Poke(false);
				return false;
			}
			SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(this.ActiveBlockValue));
			for (int i = 0; i < blockBehaviors.Length; i++)
			{
				if (blockBehaviors[i].OnUse(ray, this))
				{
					this.Poke(false);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x0007DB18 File Offset: 0x0007BD18
		public bool Interact(TerrainRaycastResult raycastResult)
		{
			SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(raycastResult.Value));
			for (int i = 0; i < blockBehaviors.Length; i++)
			{
				if (blockBehaviors[i].OnInteract(raycastResult, this))
				{
					if (this.ComponentCreature.PlayerStats != null)
					{
						this.ComponentCreature.PlayerStats.BlocksInteracted += 1L;
					}
					this.Poke(false);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x0007DB88 File Offset: 0x0007BD88
		public void Hit(ComponentBody componentBody, Vector3 hitPoint, Vector3 hitDirection)
		{
			if (this.m_subsystemTime.GameTime - this.m_lastHitTime <= 0.6600000262260437)
			{
				return;
			}
			this.m_lastHitTime = this.m_subsystemTime.GameTime;
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(this.ActiveBlockValue)];
			if (!this.CanUseTool(this.ActiveBlockValue))
			{
				ComponentPlayer componentPlayer = this.ComponentPlayer;
				if (componentPlayer != null)
				{
					componentPlayer.ComponentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentMiner.fName, 1), block.PlayerLevelRequired, block.GetDisplayName(this.m_subsystemTerrain, this.ActiveBlockValue)), Color.White, true, true);
				}
				this.Poke(false);
				return;
			}
			float num = 0f;
			float num2 = 1f;
			if (this.ActiveBlockValue != 0)
			{
				num = block.GetMeleePower(this.ActiveBlockValue) * this.AttackPower * this.m_random.Float(0.8f, 1.2f);
				num2 = block.GetMeleeHitProbability(this.ActiveBlockValue);
			}
			else
			{
				num = this.AttackPower * this.m_random.Float(0.8f, 1.2f);
				num2 = 0.66f;
			}
			ModsManager.HookAction("OnMinerHit", delegate(ModLoader modLoader)
			{
				bool result;
				modLoader.OnMinerHit(this, componentBody, hitPoint, hitDirection, ref num, ref num2, out result);
				return result;
			});
			bool flag;
			if (this.ComponentPlayer != null)
			{
				this.m_subsystemAudio.PlaySound("Audio/Swoosh", 1f, this.m_random.Float(-0.2f, 0.2f), componentBody.Position, 3f, false);
				flag = this.m_random.Bool(num2);
				num *= this.ComponentPlayer.ComponentLevel.StrengthFactor;
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				ComponentMiner.AttackBody(componentBody, this.ComponentCreature, hitPoint, hitDirection, num, true);
				this.DamageActiveTool(1);
			}
			else if (this.ComponentCreature is ComponentPlayer)
			{
				HitValueParticleSystem particleSystem = new HitValueParticleSystem(hitPoint + 0.75f * hitDirection, 1f * hitDirection + this.ComponentCreature.ComponentBody.Velocity, Color.White, LanguageControl.Get(ComponentMiner.fName, 2));
				ModsManager.HookAction("SetHitValueParticleSystem", delegate(ModLoader modLoader)
				{
					modLoader.SetHitValueParticleSystem(particleSystem, false);
					return false;
				});
				base.Project.FindSubsystem<SubsystemParticles>(true).AddParticleSystem(particleSystem);
			}
			if (this.ComponentCreature.PlayerStats != null)
			{
				this.ComponentCreature.PlayerStats.MeleeAttacks += 1L;
				if (flag)
				{
					this.ComponentCreature.PlayerStats.MeleeHits += 1L;
				}
			}
			this.Poke(false);
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x0007DE8C File Offset: 0x0007C08C
		public bool Aim(Ray3 aim, AimState state)
		{
			int num = Terrain.ExtractContents(this.ActiveBlockValue);
			Block block = BlocksManager.Blocks[num];
			if (block.IsAimable_(this.ActiveBlockValue))
			{
				if (!this.CanUseTool(this.ActiveBlockValue))
				{
					ComponentPlayer componentPlayer = this.ComponentPlayer;
					if (componentPlayer != null)
					{
						componentPlayer.ComponentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentMiner.fName, 1), block.GetPlayerLevelRequired(this.ActiveBlockValue), block.GetDisplayName(this.m_subsystemTerrain, this.ActiveBlockValue)), Color.White, true, true);
					}
					this.Poke(false);
					return true;
				}
				SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(this.ActiveBlockValue));
				for (int i = 0; i < blockBehaviors.Length; i++)
				{
					if (blockBehaviors[i].OnAim(aim, this, state))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x0007DF5C File Offset: 0x0007C15C
		public object Raycast(Ray3 ray, RaycastMode mode, bool raycastTerrain = true, bool raycastBodies = true, bool raycastMovingBlocks = true)
		{
			float reach = (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) ? SettingsManager.CreativeReach : 5f;
			Vector3 creaturePosition = this.ComponentCreature.ComponentCreatureModel.EyePosition;
			Vector3 start = ray.Position;
			Vector3 direction = Vector3.Normalize(ray.Direction);
			Vector3 end = ray.Position + direction * 15f;
			Point3 startCell = Terrain.ToCell(start);
			BodyRaycastResult? bodyRaycastResult = this.m_subsystemBodies.Raycast(start, end, 0.35f, (ComponentBody body, float distance) => Vector3.DistanceSquared(start + distance * direction, creaturePosition) <= reach * reach && body.Entity != this.Entity && !body.IsChildOfBody(this.ComponentCreature.ComponentBody) && !this.ComponentCreature.ComponentBody.IsChildOfBody(body) && Vector3.Dot(Vector3.Normalize(body.BoundingBox.Center() - start), direction) > 0.7f);
			MovingBlocksRaycastResult? movingBlocksRaycastResult = this.m_subsystemMovingBlocks.Raycast(start, end, true);
			TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(start, end, true, true, delegate(int value, float distance)
			{
				if (Vector3.DistanceSquared(start + distance * direction, creaturePosition) <= reach * reach)
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
					if (distance == 0f && block is CrossBlock && Vector3.Dot(direction, new Vector3(startCell) + new Vector3(0.5f) - start) < 0f)
					{
						return false;
					}
					if (mode == RaycastMode.Digging)
					{
						return !block.IsDiggingTransparent;
					}
					if (mode == RaycastMode.Interaction)
					{
						return !block.IsPlacementTransparent_(value) || block.IsInteractive(this.m_subsystemTerrain, value);
					}
					if (mode == RaycastMode.Gathering)
					{
						return block.IsGatherable_(value);
					}
				}
				return false;
			});
			float num = (bodyRaycastResult != null) ? bodyRaycastResult.Value.Distance : float.PositiveInfinity;
			float num2 = (movingBlocksRaycastResult != null) ? movingBlocksRaycastResult.Value.Distance : float.PositiveInfinity;
			float num3 = (terrainRaycastResult != null) ? terrainRaycastResult.Value.Distance : float.PositiveInfinity;
			if (num < num2 && num < num3)
			{
				return bodyRaycastResult.Value;
			}
			if (num2 < num && num2 < num3)
			{
				return movingBlocksRaycastResult.Value;
			}
			if (num3 < num && num3 < num2)
			{
				return terrainRaycastResult.Value;
			}
			return new Ray3(start, direction);
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x0007E118 File Offset: 0x0007C318
		public T? Raycast<T>(Ray3 ray, RaycastMode mode, bool raycastTerrain = true, bool raycastBodies = true, bool raycastMovingBlocks = true) where T : struct
		{
			object obj = this.Raycast(ray, mode, raycastTerrain, raycastBodies, raycastMovingBlocks);
			if (!(obj is T))
			{
				return null;
			}
			return new T?((T)((object)obj));
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x0007E150 File Offset: 0x0007C350
		public virtual void RemoveActiveTool(int removeCount)
		{
			if (this.Inventory != null)
			{
				this.Inventory.RemoveSlotItems(this.Inventory.ActiveSlotIndex, removeCount);
			}
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x0007E174 File Offset: 0x0007C374
		public virtual void DamageActiveTool(int damageCount)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || this.Inventory == null)
			{
				return;
			}
			int num = BlocksManager.DamageItem(this.ActiveBlockValue, damageCount);
			if (num != 0)
			{
				int slotCount = this.Inventory.GetSlotCount(this.Inventory.ActiveSlotIndex);
				this.Inventory.RemoveSlotItems(this.Inventory.ActiveSlotIndex, slotCount);
				if (this.Inventory.GetSlotCount(this.Inventory.ActiveSlotIndex) == 0)
				{
					this.Inventory.AddSlotItems(this.Inventory.ActiveSlotIndex, num, slotCount);
					return;
				}
			}
			else
			{
				this.Inventory.RemoveSlotItems(this.Inventory.ActiveSlotIndex, 1);
			}
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x0007E224 File Offset: 0x0007C424
		public static void AttackBody(ComponentBody target, ComponentCreature attacker, Vector3 hitPoint, Vector3 hitDirection, float attackPower, bool isMeleeAttack)
		{
			if (attacker != null && attacker is ComponentPlayer && target.Entity.FindComponent<ComponentPlayer>() != null && !target.Project.FindSubsystem<SubsystemGameInfo>(true).WorldSettings.IsFriendlyFireEnabled)
			{
				attacker.Entity.FindComponent<ComponentGui>(true).DisplaySmallMessage(LanguageControl.Get(ComponentMiner.fName, 3), Color.White, true, true);
				return;
			}
			ModsManager.HookAction("AttackBody", (ModLoader modloader) => modloader.AttackBody(target, attacker, hitPoint, hitDirection, ref attackPower, isMeleeAttack));
			if (attackPower > 0f)
			{
				ComponentClothing componentClothing = target.Entity.FindComponent<ComponentClothing>();
				if (componentClothing != null)
				{
					attackPower = componentClothing.ApplyArmorProtection(attackPower);
				}
				ComponentLevel componentLevel = target.Entity.FindComponent<ComponentLevel>();
				if (componentLevel != null)
				{
					attackPower /= componentLevel.ResilienceFactor;
				}
				ComponentHealth componentHealth = target.Entity.FindComponent<ComponentHealth>();
				if (componentHealth != null)
				{
					float num = attackPower / componentHealth.AttackResilience;
					string cause;
					if (attacker != null)
					{
						string text = attacker.KillVerbs[ComponentMiner.s_random.Int(0, attacker.KillVerbs.Count - 1)];
						string displayName = attacker.DisplayName;
						cause = string.Format(LanguageControl.Get(ComponentMiner.fName, 4), displayName, LanguageControl.Get(new string[]
						{
							ComponentMiner.fName,
							text
						}));
					}
					else
					{
						switch (ComponentMiner.s_random.Int(0, 5))
						{
						case 0:
							cause = LanguageControl.Get(ComponentMiner.fName, 5);
							break;
						case 1:
							cause = LanguageControl.Get(ComponentMiner.fName, 6);
							break;
						case 2:
							cause = LanguageControl.Get(ComponentMiner.fName, 7);
							break;
						case 3:
							cause = LanguageControl.Get(ComponentMiner.fName, 8);
							break;
						case 4:
							cause = LanguageControl.Get(ComponentMiner.fName, 9);
							break;
						default:
							cause = LanguageControl.Get(ComponentMiner.fName, 10);
							break;
						}
					}
					float health = componentHealth.Health;
					componentHealth.Injure(num, attacker, false, cause);
					if (num > 0f)
					{
						target.Project.FindSubsystem<SubsystemAudio>(true).PlayRandomSound("Audio/Impacts/Body", 1f, ComponentMiner.s_random.Float(-0.3f, 0.3f), target.Position, 4f, false);
						float num2 = (health - componentHealth.Health) * componentHealth.AttackResilience;
						if (attacker is ComponentPlayer && num2 > 0f)
						{
							string text2 = (0f - num2).ToString("0", CultureInfo.InvariantCulture);
							HitValueParticleSystem particleSystem = new HitValueParticleSystem(hitPoint + 0.75f * hitDirection, 1f * hitDirection + attacker.ComponentBody.Velocity, Color.White, text2);
							ModsManager.HookAction("SetHitValueParticleSystem", delegate(ModLoader modLoader)
							{
								modLoader.SetHitValueParticleSystem(particleSystem, true);
								return false;
							});
							target.Project.FindSubsystem<SubsystemParticles>(true).AddParticleSystem(particleSystem);
						}
					}
				}
				ComponentDamage componentDamage = target.Entity.FindComponent<ComponentDamage>();
				if (componentDamage != null)
				{
					float num3 = attackPower / componentDamage.AttackResilience;
					componentDamage.Damage(num3);
					if (num3 > 0f)
					{
						target.Project.FindSubsystem<SubsystemAudio>(true).PlayRandomSound(componentDamage.DamageSoundName, 1f, ComponentMiner.s_random.Float(-0.3f, 0.3f), target.Position, 4f, false);
					}
				}
			}
			float num4 = 0f;
			float x = 0f;
			if (isMeleeAttack && attacker != null)
			{
				float num5 = (attackPower >= 2f) ? 1.25f : 1f;
				float num6 = MathUtils.Pow(attacker.ComponentBody.Mass / target.Mass, 0.5f);
				float x2 = num5 * num6;
				num4 = 5.5f * MathUtils.Saturate(x2);
				x = 0.25f * MathUtils.Saturate(x2);
			}
			else if (attackPower > 0f)
			{
				num4 = 2f;
				x = 0.2f;
			}
			if (num4 > 0f)
			{
				target.ApplyImpulse(num4 * Vector3.Normalize(hitDirection + ComponentMiner.s_random.Vector3(0.1f) + 0.2f * Vector3.UnitY));
				ComponentLocomotion componentLocomotion = target.Entity.FindComponent<ComponentLocomotion>();
				if (componentLocomotion != null)
				{
					componentLocomotion.StunTime = MathUtils.Max(componentLocomotion.StunTime, x);
				}
			}
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0007E748 File Offset: 0x0007C948
		public void Update(float dt)
		{
			float num = (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) ? (0.5f / SettingsManager.CreativeDigTime) : 4f;
			this.m_lastPokingPhase = this.PokingPhase;
			if (this.DigCellFace != null || this.PokingPhase > 0f)
			{
				this.PokingPhase += num * this.m_subsystemTime.GameTimeDelta;
				if (this.PokingPhase > 1f)
				{
					this.PokingPhase = ((this.DigCellFace != null) ? MathUtils.Remainder(this.PokingPhase, 1f) : 0f);
				}
			}
			if (this.DigCellFace != null && Time.FrameIndex - this.m_lastDigFrameIndex > 1)
			{
				this.DigCellFace = null;
			}
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x0007E828 File Offset: 0x0007CA28
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemMovingBlocks = base.Project.FindSubsystem<SubsystemMovingBlocks>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemSoundMaterials = base.Project.FindSubsystem<SubsystemSoundMaterials>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.ComponentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.ComponentPlayer = base.Entity.FindComponent<ComponentPlayer>();
			IInventory inventory2;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative || this.ComponentPlayer == null)
			{
				IInventory inventory = base.Entity.FindComponent<ComponentInventory>();
				inventory2 = inventory;
			}
			else
			{
				IInventory inventory = base.Entity.FindComponent<ComponentCreativeInventory>();
				inventory2 = inventory;
			}
			this.Inventory = inventory2;
			this.AttackPower = valuesDictionary.GetValue<float>("AttackPower");
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x0007E938 File Offset: 0x0007CB38
		public static bool IsBlockPlacingAllowed(ComponentBody componentBody)
		{
			if (componentBody.StandingOnBody != null || componentBody.StandingOnValue != null)
			{
				return true;
			}
			if (componentBody.ImmersionFactor > 0.01f)
			{
				return true;
			}
			if (componentBody.ParentBody != null && ComponentMiner.IsBlockPlacingAllowed(componentBody.ParentBody))
			{
				return true;
			}
			ComponentLocomotion componentLocomotion = componentBody.Entity.FindComponent<ComponentLocomotion>();
			return componentLocomotion != null && componentLocomotion.LadderValue != null;
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x0007E9A8 File Offset: 0x0007CBA8
		public virtual float CalculateDigTime(int digValue, int toolValue)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(toolValue)];
			Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(digValue)];
			float digResilience = block2.GetDigResilience(digValue);
			BlockDigMethod blockDigMethod = block2.GetBlockDigMethod(digValue);
			float shovelPower = block.GetShovelPower(toolValue);
			float quarryPower = block.GetQuarryPower(toolValue);
			float hackPower = block.GetHackPower(toolValue);
			if (this.ComponentPlayer != null && this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
			{
				if (digResilience < float.PositiveInfinity)
				{
					return 0f;
				}
				return float.PositiveInfinity;
			}
			else if (this.ComponentPlayer != null && this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure)
			{
				float num = 0f;
				if (blockDigMethod == BlockDigMethod.Shovel && shovelPower >= 2f)
				{
					num = shovelPower;
				}
				else if (blockDigMethod == BlockDigMethod.Quarry && quarryPower >= 2f)
				{
					num = quarryPower;
				}
				else if (blockDigMethod == BlockDigMethod.Hack && hackPower >= 2f)
				{
					num = hackPower;
				}
				num *= this.ComponentPlayer.ComponentLevel.StrengthFactor;
				if (num <= 0f)
				{
					return float.PositiveInfinity;
				}
				return MathUtils.Max(digResilience / num, 0f);
			}
			else
			{
				float num2 = 0f;
				if (blockDigMethod == BlockDigMethod.Shovel)
				{
					num2 = shovelPower;
				}
				else if (blockDigMethod == BlockDigMethod.Quarry)
				{
					num2 = quarryPower;
				}
				else if (blockDigMethod == BlockDigMethod.Hack)
				{
					num2 = hackPower;
				}
				if (this.ComponentPlayer != null)
				{
					num2 *= this.ComponentPlayer.ComponentLevel.StrengthFactor;
				}
				if (num2 <= 0f)
				{
					return float.PositiveInfinity;
				}
				return MathUtils.Max(digResilience / num2, 0f);
			}
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x0007EB08 File Offset: 0x0007CD08
		public virtual bool CanUseTool(int toolValue)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(toolValue)];
				if (this.ComponentPlayer != null && this.ComponentPlayer.PlayerData.Level < (float)block.GetPlayerLevelRequired(toolValue))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x040009FC RID: 2556
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040009FD RID: 2557
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040009FE RID: 2558
		public SubsystemMovingBlocks m_subsystemMovingBlocks;

		// Token: 0x040009FF RID: 2559
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000A00 RID: 2560
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A01 RID: 2561
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000A02 RID: 2562
		public SubsystemSoundMaterials m_subsystemSoundMaterials;

		// Token: 0x04000A03 RID: 2563
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x04000A04 RID: 2564
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000A05 RID: 2565
		public static Game.Random s_random = new Game.Random();

		// Token: 0x04000A06 RID: 2566
		public double m_digStartTime;

		// Token: 0x04000A07 RID: 2567
		public float m_digProgress;

		// Token: 0x04000A08 RID: 2568
		public double m_lastHitTime;

		// Token: 0x04000A09 RID: 2569
		public static string fName = "ComponentMiner";

		// Token: 0x04000A0A RID: 2570
		public int m_lastDigFrameIndex;

		// Token: 0x04000A0B RID: 2571
		public float m_lastPokingPhase;
	}
}
