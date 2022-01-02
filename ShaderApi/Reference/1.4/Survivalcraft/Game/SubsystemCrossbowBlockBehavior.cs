using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000199 RID: 409
	public class SubsystemCrossbowBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000A0A RID: 2570 RVA: 0x00040755 File Offset: 0x0003E955
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x0004075D File Offset: 0x0003E95D
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			componentPlayer.ComponentGui.ModalPanelWidget = ((componentPlayer.ComponentGui.ModalPanelWidget == null) ? new CrossbowWidget(inventory, slotIndex) : null);
			return true;
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x00040784 File Offset: 0x0003E984
		public override bool OnAim(Ray3 aim, ComponentMiner componentMiner, AimState state)
		{
			IInventory inventory = componentMiner.Inventory;
			if (inventory != null)
			{
				int activeSlotIndex = inventory.ActiveSlotIndex;
				if (activeSlotIndex >= 0)
				{
					int slotValue = inventory.GetSlotValue(activeSlotIndex);
					int slotCount = inventory.GetSlotCount(activeSlotIndex);
					int num = Terrain.ExtractContents(slotValue);
					int data = Terrain.ExtractData(slotValue);
					if (num == 200 && slotCount > 0)
					{
						int draw = CrossbowBlock.GetDraw(data);
						double gameTime;
						if (!this.m_aimStartTimes.TryGetValue(componentMiner, out gameTime))
						{
							gameTime = this.m_subsystemTime.GameTime;
							this.m_aimStartTimes[componentMiner] = gameTime;
						}
						float num2 = (float)(this.m_subsystemTime.GameTime - gameTime);
						float num3 = (float)MathUtils.Remainder(this.m_subsystemTime.GameTime, 1000.0);
						Vector3 v = ((componentMiner.ComponentCreature.ComponentBody.IsSneaking ? 0.01f : 0.03f) + 0.15f * MathUtils.Saturate((num2 - 2.5f) / 6f)) * new Vector3
						{
							X = SimplexNoise.OctavedNoise(num3, 2f, 3, 2f, 0.5f, false),
							Y = SimplexNoise.OctavedNoise(num3 + 100f, 2f, 3, 2f, 0.5f, false),
							Z = SimplexNoise.OctavedNoise(num3 + 200f, 2f, 3, 2f, 0.5f, false)
						};
						aim.Direction = Vector3.Normalize(aim.Direction + v);
						switch (state)
						{
						case AimState.InProgress:
						{
							if (num2 >= 10f)
							{
								componentMiner.ComponentCreature.ComponentCreatureSounds.PlayMoanSound();
								return true;
							}
							ComponentFirstPersonModel componentFirstPersonModel = componentMiner.Entity.FindComponent<ComponentFirstPersonModel>();
							if (componentFirstPersonModel != null)
							{
								ComponentPlayer componentPlayer = componentMiner.ComponentPlayer;
								if (componentPlayer != null)
								{
									componentPlayer.ComponentAimingSights.ShowAimingSights(aim.Position, aim.Direction);
								}
								componentFirstPersonModel.ItemOffsetOrder = new Vector3(-0.22f, 0.15f, 0.1f);
								componentFirstPersonModel.ItemRotationOrder = new Vector3(-0.7f, 0f, 0f);
							}
							componentMiner.ComponentCreature.ComponentCreatureModel.AimHandAngleOrder = 1.3f;
							componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemOffsetOrder = new Vector3(-0.08f, -0.1f, 0.07f);
							componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemRotationOrder = new Vector3(-1.55f, 0f, 0f);
							break;
						}
						case AimState.Cancelled:
							this.m_aimStartTimes.Remove(componentMiner);
							break;
						case AimState.Completed:
						{
							ArrowBlock.ArrowType? arrowType = CrossbowBlock.GetArrowType(data);
							if (draw != 15)
							{
								ComponentPlayer componentPlayer2 = componentMiner.ComponentPlayer;
								if (componentPlayer2 != null)
								{
									componentPlayer2.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemCrossbowBlockBehavior.fName, 0), Color.White, true, false);
								}
							}
							else if (arrowType == null)
							{
								ComponentPlayer componentPlayer3 = componentMiner.ComponentPlayer;
								if (componentPlayer3 != null)
								{
									componentPlayer3.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemCrossbowBlockBehavior.fName, 1), Color.White, true, false);
								}
							}
							else
							{
								Vector3 vector = componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition + componentMiner.ComponentCreature.ComponentBody.Matrix.Right * 0.3f - componentMiner.ComponentCreature.ComponentBody.Matrix.Up * 0.2f;
								Vector3 v2 = Vector3.Normalize(vector + aim.Direction * 10f - vector);
								int value = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, arrowType.Value));
								float s = 38f;
								if (this.m_subsystemProjectiles.FireProjectile(value, vector, s * v2, Vector3.Zero, componentMiner.ComponentCreature) != null)
								{
									data = CrossbowBlock.SetArrowType(data, null);
									this.m_subsystemAudio.PlaySound("Audio/Bow", 1f, this.m_random.Float(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition, 3f, 0.05f);
								}
							}
							inventory.RemoveSlotItems(activeSlotIndex, 1);
							int value2 = Terrain.MakeBlockValue(num, 0, CrossbowBlock.SetDraw(data, 0));
							inventory.AddSlotItems(activeSlotIndex, value2, 1);
							if (draw > 0)
							{
								componentMiner.DamageActiveTool(1);
								this.m_subsystemAudio.PlaySound("Audio/CrossbowBoing", 1f, this.m_random.Float(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition, 3f, 0f);
							}
							this.m_aimStartTimes.Remove(componentMiner);
							break;
						}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x00040C3C File Offset: 0x0003EE3C
		public override int GetProcessInventoryItemCapacity(IInventory inventory, int slotIndex, int value)
		{
			int num = Terrain.ExtractContents(value);
			ArrowBlock.ArrowType arrowType = ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (num != 192 || !this.m_supportedArrowTypes.Contains(arrowType))
			{
				return 0;
			}
			int data = Terrain.ExtractData(inventory.GetSlotValue(slotIndex));
			ArrowBlock.ArrowType? arrowType2 = CrossbowBlock.GetArrowType(data);
			int draw = CrossbowBlock.GetDraw(data);
			if (arrowType2 == null && draw == 15)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x00040C9C File Offset: 0x0003EE9C
		public override void ProcessInventoryItem(IInventory inventory, int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			if (processCount == 1)
			{
				ArrowBlock.ArrowType arrowType = ArrowBlock.GetArrowType(Terrain.ExtractData(value));
				int data = Terrain.ExtractData(inventory.GetSlotValue(slotIndex));
				processedValue = 0;
				processedCount = 0;
				inventory.RemoveSlotItems(slotIndex, 1);
				inventory.AddSlotItems(slotIndex, Terrain.MakeBlockValue(200, 0, CrossbowBlock.SetArrowType(data, new ArrowBlock.ArrowType?(arrowType))), 1);
				return;
			}
			processedValue = value;
			processedCount = count;
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x00040D01 File Offset: 0x0003EF01
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x040004D7 RID: 1239
		public SubsystemTime m_subsystemTime;

		// Token: 0x040004D8 RID: 1240
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x040004D9 RID: 1241
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040004DA RID: 1242
		public Game.Random m_random = new Game.Random();

		// Token: 0x040004DB RID: 1243
		public static string fName = "SubsystemCrossbowBlockBehavior";

		// Token: 0x040004DC RID: 1244
		public Dictionary<ComponentMiner, double> m_aimStartTimes = new Dictionary<ComponentMiner, double>();

		// Token: 0x040004DD RID: 1245
		public ArrowBlock.ArrowType[] m_supportedArrowTypes = new ArrowBlock.ArrowType[]
		{
			ArrowBlock.ArrowType.IronBolt,
			ArrowBlock.ArrowType.DiamondBolt,
			ArrowBlock.ArrowType.ExplosiveBolt
		};
	}
}
