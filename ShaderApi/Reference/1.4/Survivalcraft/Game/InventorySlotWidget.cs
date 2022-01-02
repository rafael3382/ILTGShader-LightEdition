using System;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using GameEntitySystem;

namespace Game
{
	// Token: 0x0200038E RID: 910
	public class InventorySlotWidget : CanvasWidget, IDragTargetWidget
	{
		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06001B1A RID: 6938 RVA: 0x000D4798 File Offset: 0x000D2998
		// (set) Token: 0x06001B1B RID: 6939 RVA: 0x000D47A0 File Offset: 0x000D29A0
		public bool HideBlockIcon { get; set; }

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06001B1C RID: 6940 RVA: 0x000D47A9 File Offset: 0x000D29A9
		// (set) Token: 0x06001B1D RID: 6941 RVA: 0x000D47B1 File Offset: 0x000D29B1
		public bool HideEditOverlay { get; set; }

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06001B1E RID: 6942 RVA: 0x000D47BA File Offset: 0x000D29BA
		// (set) Token: 0x06001B1F RID: 6943 RVA: 0x000D47C2 File Offset: 0x000D29C2
		public bool HideInteractiveOverlay { get; set; }

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06001B20 RID: 6944 RVA: 0x000D47CB File Offset: 0x000D29CB
		// (set) Token: 0x06001B21 RID: 6945 RVA: 0x000D47D3 File Offset: 0x000D29D3
		public bool HideFoodOverlay { get; set; }

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06001B22 RID: 6946 RVA: 0x000D47DC File Offset: 0x000D29DC
		// (set) Token: 0x06001B23 RID: 6947 RVA: 0x000D47E4 File Offset: 0x000D29E4
		public bool HideHighlightRectangle { get; set; }

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06001B24 RID: 6948 RVA: 0x000D47ED File Offset: 0x000D29ED
		// (set) Token: 0x06001B25 RID: 6949 RVA: 0x000D47F5 File Offset: 0x000D29F5
		public bool HideHealthBar { get; set; }

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06001B26 RID: 6950 RVA: 0x000D47FE File Offset: 0x000D29FE
		// (set) Token: 0x06001B27 RID: 6951 RVA: 0x000D4806 File Offset: 0x000D2A06
		public bool ProcessingOnly { get; set; }

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06001B28 RID: 6952 RVA: 0x000D480F File Offset: 0x000D2A0F
		// (set) Token: 0x06001B29 RID: 6953 RVA: 0x000D481C File Offset: 0x000D2A1C
		public Color CenterColor
		{
			get
			{
				return this.m_rectangleWidget.CenterColor;
			}
			set
			{
				this.m_rectangleWidget.CenterColor = value;
			}
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06001B2A RID: 6954 RVA: 0x000D482A File Offset: 0x000D2A2A
		// (set) Token: 0x06001B2B RID: 6955 RVA: 0x000D4837 File Offset: 0x000D2A37
		public Color BevelColor
		{
			get
			{
				return this.m_rectangleWidget.BevelColor;
			}
			set
			{
				this.m_rectangleWidget.BevelColor = value;
			}
		}

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06001B2C RID: 6956 RVA: 0x000D4845 File Offset: 0x000D2A45
		// (set) Token: 0x06001B2D RID: 6957 RVA: 0x000D4852 File Offset: 0x000D2A52
		public Matrix? CustomViewMatrix
		{
			get
			{
				return this.m_blockIconWidget.CustomViewMatrix;
			}
			set
			{
				this.m_blockIconWidget.CustomViewMatrix = value;
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06001B2E RID: 6958 RVA: 0x000D4860 File Offset: 0x000D2A60
		public GameWidget GameWidget
		{
			get
			{
				if (this.m_gameWidget == null)
				{
					for (ContainerWidget parentWidget = base.ParentWidget; parentWidget != null; parentWidget = parentWidget.ParentWidget)
					{
						GameWidget gameWidget = parentWidget as GameWidget;
						if (gameWidget != null)
						{
							this.m_gameWidget = gameWidget;
							break;
						}
					}
				}
				return this.m_gameWidget;
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06001B2F RID: 6959 RVA: 0x000D48A1 File Offset: 0x000D2AA1
		public DragHostWidget DragHostWidget
		{
			get
			{
				if (this.m_dragHostWidget == null)
				{
					this.m_dragHostWidget = ((this.GameWidget != null) ? this.GameWidget.Children.Find<DragHostWidget>(false) : null);
				}
				return this.m_dragHostWidget;
			}
		}

		// Token: 0x06001B30 RID: 6960 RVA: 0x000D48D4 File Offset: 0x000D2AD4
		public InventorySlotWidget()
		{
			base.Size = new Vector2(72f, 72f);
			WidgetsList children = this.Children;
			Widget[] array = new Widget[7];
			BevelledRectangleWidget bevelledRectangleWidget = new BevelledRectangleWidget
			{
				BevelSize = -2f,
				DirectionalLight = 0.15f,
				CenterColor = Color.Transparent
			};
			BevelledRectangleWidget bevelledRectangleWidget2 = bevelledRectangleWidget;
			this.m_rectangleWidget = bevelledRectangleWidget;
			array[0] = bevelledRectangleWidget2;
			RectangleWidget rectangleWidget = new RectangleWidget
			{
				FillColor = Color.Transparent,
				OutlineColor = Color.Transparent
			};
			RectangleWidget rectangleWidget2 = rectangleWidget;
			this.m_highlightWidget = rectangleWidget;
			array[1] = rectangleWidget2;
			BlockIconWidget blockIconWidget = new BlockIconWidget
			{
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(2f, 2f)
			};
			BlockIconWidget blockIconWidget2 = blockIconWidget;
			this.m_blockIconWidget = blockIconWidget;
			array[2] = blockIconWidget2;
			LabelWidget labelWidget = new LabelWidget
			{
				FontScale = 1f,
				HorizontalAlignment = WidgetAlignment.Far,
				VerticalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(6f, 2f)
			};
			LabelWidget labelWidget2 = labelWidget;
			this.m_countWidget = labelWidget;
			array[3] = labelWidget2;
			ValueBarWidget valueBarWidget = new ValueBarWidget
			{
				LayoutDirection = LayoutDirection.Vertical,
				HorizontalAlignment = WidgetAlignment.Near,
				VerticalAlignment = WidgetAlignment.Far,
				BarsCount = 3,
				FlipDirection = true,
				LitBarColor = new Color(32, 128, 0),
				UnlitBarColor = new Color(24, 24, 24, 64),
				BarSize = new Vector2(12f, 12f),
				BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/ProgressBar", null),
				Margin = new Vector2(4f, 4f)
			};
			ValueBarWidget valueBarWidget2 = valueBarWidget;
			this.m_healthBarWidget = valueBarWidget;
			array[4] = valueBarWidget2;
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(3f, 3f)
			};
			WidgetsList children2 = stackPanelWidget.Children;
			RectangleWidget rectangleWidget3 = new RectangleWidget
			{
				Subtexture = ContentManager.Get<Subtexture>("Textures/Atlas/InteractiveItemOverlay", null),
				Size = new Vector2(13f, 14f),
				FillColor = new Color(160, 160, 160),
				OutlineColor = Color.Transparent
			};
			rectangleWidget2 = rectangleWidget3;
			this.m_interactiveOverlayWidget = rectangleWidget3;
			children2.Add(rectangleWidget2);
			WidgetsList children3 = stackPanelWidget.Children;
			RectangleWidget rectangleWidget4 = new RectangleWidget
			{
				Subtexture = ContentManager.Get<Subtexture>("Textures/Atlas/EditItemOverlay", null),
				Size = new Vector2(12f, 14f),
				FillColor = new Color(160, 160, 160),
				OutlineColor = Color.Transparent
			};
			rectangleWidget2 = rectangleWidget4;
			this.m_editOverlayWidget = rectangleWidget4;
			children3.Add(rectangleWidget2);
			WidgetsList children4 = stackPanelWidget.Children;
			RectangleWidget rectangleWidget5 = new RectangleWidget
			{
				Subtexture = ContentManager.Get<Subtexture>("Textures/Atlas/FoodItemOverlay", null),
				Size = new Vector2(11f, 14f),
				FillColor = new Color(160, 160, 160),
				OutlineColor = Color.Transparent
			};
			rectangleWidget2 = rectangleWidget5;
			this.m_foodOverlayWidget = rectangleWidget5;
			children4.Add(rectangleWidget2);
			array[5] = stackPanelWidget;
			LabelWidget labelWidget3 = new LabelWidget
			{
				Text = "Split",
				Color = new Color(255, 64, 0),
				HorizontalAlignment = WidgetAlignment.Near,
				VerticalAlignment = WidgetAlignment.Near,
				Margin = new Vector2(2f, 0f)
			};
			labelWidget2 = labelWidget3;
			this.m_splitLabelWidget = labelWidget3;
			array[6] = labelWidget2;
			children.Add(array);
		}

		// Token: 0x06001B31 RID: 6961 RVA: 0x000D4C60 File Offset: 0x000D2E60
		public void AssignInventorySlot(IInventory inventory, int slotIndex)
		{
			this.m_inventory = inventory;
			this.m_slotIndex = slotIndex;
			this.m_subsystemTerrain = ((inventory != null) ? inventory.Project.FindSubsystem<SubsystemTerrain>(true) : null);
			this.m_componentPlayer = ((inventory is Component) ? ((Component)inventory).Entity.FindComponent<ComponentPlayer>() : null);
			this.m_blockIconWidget.DrawBlockEnvironmentData.SubsystemTerrain = this.m_subsystemTerrain;
		}

		// Token: 0x06001B32 RID: 6962 RVA: 0x000D4CCC File Offset: 0x000D2ECC
		public override void Update()
		{
			if (this.m_inventory == null || this.DragHostWidget == null)
			{
				return;
			}
			WidgetInput input = base.Input;
			ComponentPlayer viewPlayer = this.GetViewPlayer();
			int slotValue = this.m_inventory.GetSlotValue(this.m_slotIndex);
			int num = Terrain.ExtractContents(slotValue);
			Block block = BlocksManager.Blocks[num];
			if (this.m_componentPlayer != null)
			{
				this.m_blockIconWidget.DrawBlockEnvironmentData.InWorldMatrix = this.m_componentPlayer.ComponentBody.Matrix;
			}
			if (this.m_focus && input.Press == null)
			{
				this.m_focus = false;
			}
			else if (input.Tap != null && base.HitTestGlobal(input.Tap.Value, null) == this)
			{
				this.m_focus = true;
			}
			if (input.SpecialClick != null && base.HitTestGlobal(input.SpecialClick.Value.Start, null) == this && base.HitTestGlobal(input.SpecialClick.Value.End, null) == this)
			{
				IInventory inventory = null;
				foreach (InventorySlotWidget inventorySlotWidget in ((ContainerWidget)base.RootWidget).AllChildren.OfType<InventorySlotWidget>())
				{
					if (inventorySlotWidget.m_inventory != null && inventorySlotWidget.m_inventory != this.m_inventory && inventorySlotWidget.Input == base.Input && inventorySlotWidget.IsEnabledGlobal && inventorySlotWidget.IsVisibleGlobal)
					{
						inventory = inventorySlotWidget.m_inventory;
						break;
					}
				}
				if (inventory != null)
				{
					int num2 = ComponentInventoryBase.FindAcquireSlotForItem(inventory, slotValue);
					if (num2 >= 0)
					{
						this.HandleMoveItem(this.m_inventory, this.m_slotIndex, inventory, num2, this.m_inventory.GetSlotCount(this.m_slotIndex));
					}
				}
			}
			if (input.Click != null && base.HitTestGlobal(input.Click.Value.Start, null) == this && base.HitTestGlobal(input.Click.Value.End, null) == this)
			{
				bool flag = false;
				if (viewPlayer != null)
				{
					if (viewPlayer.ComponentInput.SplitSourceInventory == this.m_inventory && viewPlayer.ComponentInput.SplitSourceSlotIndex == this.m_slotIndex)
					{
						viewPlayer.ComponentInput.SetSplitSourceInventoryAndSlot(null, -1);
						flag = true;
					}
					else if (viewPlayer.ComponentInput.SplitSourceInventory != null)
					{
						flag = this.HandleMoveItem(viewPlayer.ComponentInput.SplitSourceInventory, viewPlayer.ComponentInput.SplitSourceSlotIndex, this.m_inventory, this.m_slotIndex, 1);
						AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					}
				}
				if (!flag && this.m_inventory.ActiveSlotIndex != this.m_slotIndex && this.m_slotIndex < 10)
				{
					this.m_inventory.ActiveSlotIndex = this.m_slotIndex;
					if (this.m_inventory.ActiveSlotIndex == this.m_slotIndex)
					{
						AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					}
				}
			}
			if (!this.m_focus || this.ProcessingOnly || viewPlayer == null)
			{
				return;
			}
			Vector2? hold = input.Hold;
			if (hold != null && base.HitTestGlobal(hold.Value, null) == this && !this.DragHostWidget.IsDragInProgress && this.m_inventory.GetSlotCount(this.m_slotIndex) > 0 && (viewPlayer.ComponentInput.SplitSourceInventory != this.m_inventory || viewPlayer.ComponentInput.SplitSourceSlotIndex != this.m_slotIndex))
			{
				input.Clear();
				viewPlayer.ComponentInput.SetSplitSourceInventoryAndSlot(this.m_inventory, this.m_slotIndex);
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			}
			Vector2? drag = input.Drag;
			if (drag == null || base.HitTestGlobal(drag.Value, null) != this || this.DragHostWidget.IsDragInProgress)
			{
				return;
			}
			int slotCount = this.m_inventory.GetSlotCount(this.m_slotIndex);
			if (slotCount > 0)
			{
				DragMode dragMode = input.DragMode;
				if (viewPlayer.ComponentInput.SplitSourceInventory == this.m_inventory && viewPlayer.ComponentInput.SplitSourceSlotIndex == this.m_slotIndex)
				{
					dragMode = DragMode.SingleItem;
				}
				int num3 = (dragMode != DragMode.AllItems) ? 1 : slotCount;
				SubsystemTerrain subsystemTerrain = this.m_inventory.Project.FindSubsystem<SubsystemTerrain>();
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(null, ContentManager.Get<XElement>("Widgets/InventoryDragWidget", null), null);
				containerWidget.Children.Find<BlockIconWidget>("InventoryDragWidget.Icon", true).Value = Terrain.ReplaceLight(slotValue, 15);
				containerWidget.Children.Find<BlockIconWidget>("InventoryDragWidget.Icon", true).DrawBlockEnvironmentData.SubsystemTerrain = subsystemTerrain;
				containerWidget.Children.Find<LabelWidget>("InventoryDragWidget.Name", true).Text = block.GetDisplayName(subsystemTerrain, slotValue);
				containerWidget.Children.Find<LabelWidget>("InventoryDragWidget.Count", true).Text = num3.ToString();
				containerWidget.Children.Find<LabelWidget>("InventoryDragWidget.Count", true).IsVisible = (!(this.m_inventory is ComponentCreativeInventory) && !(this.m_inventory is ComponentFurnitureInventory));
				this.DragHostWidget.BeginDrag(containerWidget, new InventoryDragData
				{
					Inventory = this.m_inventory,
					SlotIndex = this.m_slotIndex,
					DragMode = dragMode
				}, delegate
				{
					this.m_dragMode = null;
				});
				this.m_dragMode = new DragMode?(dragMode);
			}
		}

		// Token: 0x06001B33 RID: 6963 RVA: 0x000D5268 File Offset: 0x000D3468
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (this.m_inventory != null)
			{
				bool flag = this.m_inventory is ComponentCreativeInventory || this.m_inventory is ComponentFurnitureInventory;
				int num = this.m_inventory.GetSlotCount(this.m_slotIndex);
				if (!flag && this.m_dragMode != null)
				{
					num = ((this.m_dragMode.Value != DragMode.AllItems) ? MathUtils.Max(num - 1, 0) : 0);
				}
				this.m_rectangleWidget.IsVisible = true;
				if (num > 0)
				{
					int slotValue = this.m_inventory.GetSlotValue(this.m_slotIndex);
					int num2 = Terrain.ExtractContents(slotValue);
					Block block = BlocksManager.Blocks[num2];
					bool flag2 = block.GetRotPeriod(slotValue) > 0 && block.GetDamage(slotValue) > 0;
					this.m_blockIconWidget.Value = Terrain.ReplaceLight(slotValue, 15);
					this.m_blockIconWidget.IsVisible = !this.HideBlockIcon;
					if (num != this.m_lastCount)
					{
						this.m_countWidget.Text = num.ToString();
						this.m_lastCount = num;
					}
					this.m_countWidget.IsVisible = (num > 1 && !flag);
					this.m_editOverlayWidget.IsVisible = (!this.HideEditOverlay && block.IsEditable_(slotValue));
					this.m_interactiveOverlayWidget.IsVisible = (!this.HideInteractiveOverlay && block.IsInteractive(this.m_subsystemTerrain, slotValue));
					this.m_foodOverlayWidget.IsVisible = (!this.HideFoodOverlay && block.GetRotPeriod(slotValue) > 0);
					this.m_foodOverlayWidget.FillColor = (flag2 ? new Color(128, 64, 0) : new Color(160, 160, 160));
					if (!flag)
					{
						float blockHealth = block.GetBlockHealth(slotValue);
						if (blockHealth >= 0f)
						{
							this.m_healthBarWidget.IsVisible = true;
							this.m_healthBarWidget.Value = blockHealth;
						}
						else
						{
							this.m_healthBarWidget.IsVisible = false;
						}
					}
					else
					{
						this.m_healthBarWidget.IsVisible = false;
					}
				}
				else
				{
					this.m_blockIconWidget.IsVisible = false;
					this.m_countWidget.IsVisible = false;
					this.m_healthBarWidget.IsVisible = false;
					this.m_editOverlayWidget.IsVisible = false;
					this.m_interactiveOverlayWidget.IsVisible = false;
					this.m_foodOverlayWidget.IsVisible = false;
				}
				this.m_highlightWidget.IsVisible = !this.HideHighlightRectangle;
				this.m_highlightWidget.OutlineColor = Color.Transparent;
				this.m_highlightWidget.FillColor = Color.Transparent;
				this.m_splitLabelWidget.IsVisible = false;
				if (this.m_slotIndex == this.m_inventory.ActiveSlotIndex)
				{
					this.m_highlightWidget.OutlineColor = new Color(0, 0, 0);
					this.m_highlightWidget.FillColor = new Color(0, 0, 0, 80);
				}
				if (this.IsSplitMode())
				{
					this.m_highlightWidget.OutlineColor = new Color(255, 64, 0);
					this.m_splitLabelWidget.IsVisible = true;
				}
			}
			else
			{
				this.m_rectangleWidget.IsVisible = false;
				this.m_highlightWidget.IsVisible = false;
				this.m_blockIconWidget.IsVisible = false;
				this.m_countWidget.IsVisible = false;
				this.m_healthBarWidget.IsVisible = false;
				this.m_editOverlayWidget.IsVisible = false;
				this.m_interactiveOverlayWidget.IsVisible = false;
				this.m_foodOverlayWidget.IsVisible = false;
				this.m_splitLabelWidget.IsVisible = false;
			}
			base.IsDrawRequired = (this.m_inventoryDragData != null);
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001B34 RID: 6964 RVA: 0x000D55E4 File Offset: 0x000D37E4
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.m_inventory != null && this.m_inventoryDragData != null)
			{
				int slotValue = this.m_inventoryDragData.Inventory.GetSlotValue(this.m_inventoryDragData.SlotIndex);
				if (this.m_inventory.GetSlotProcessCapacity(this.m_slotIndex, slotValue) >= 0 || this.m_inventory.GetSlotCapacity(this.m_slotIndex, slotValue) > 0)
				{
					float num = 80f * base.GlobalTransform.Right.Length();
					Vector2 center = Vector2.Transform(base.ActualSize / 2f, base.GlobalTransform);
					FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(100, null, null, null);
					flatBatch2D.QueueEllipse(center, new Vector2(num), 0f, new Color(0, 0, 0, 96) * base.GlobalColorTransform, 64, 0f, 6.28318548f);
					flatBatch2D.QueueEllipse(center, new Vector2(num - 0.5f), 0f, new Color(0, 0, 0, 64) * base.GlobalColorTransform, 64, 0f, 6.28318548f);
					flatBatch2D.QueueEllipse(center, new Vector2(num + 0.5f), 0f, new Color(0, 0, 0, 48) * base.GlobalColorTransform, 64, 0f, 6.28318548f);
					flatBatch2D.QueueDisc(center, new Vector2(num), 0f, new Color(0, 0, 0, 48) * base.GlobalColorTransform, 64, 0f, 6.28318548f);
				}
			}
			this.m_inventoryDragData = null;
		}

		// Token: 0x06001B35 RID: 6965 RVA: 0x000D5778 File Offset: 0x000D3978
		public void DragOver(Widget dragWidget, object data)
		{
			this.m_inventoryDragData = (data as InventoryDragData);
		}

		// Token: 0x06001B36 RID: 6966 RVA: 0x000D5788 File Offset: 0x000D3988
		public void DragDrop(Widget dragWidget, object data)
		{
			InventoryDragData inventoryDragData = data as InventoryDragData;
			if (this.m_inventory != null && inventoryDragData != null)
			{
				this.HandleDragDrop(inventoryDragData.Inventory, inventoryDragData.SlotIndex, inventoryDragData.DragMode, this.m_inventory, this.m_slotIndex);
			}
		}

		// Token: 0x06001B37 RID: 6967 RVA: 0x000D57CC File Offset: 0x000D39CC
		public ComponentPlayer GetViewPlayer()
		{
			if (this.GameWidget == null)
			{
				return null;
			}
			return this.GameWidget.PlayerData.ComponentPlayer;
		}

		// Token: 0x06001B38 RID: 6968 RVA: 0x000D57E8 File Offset: 0x000D39E8
		public bool IsSplitMode()
		{
			ComponentPlayer viewPlayer = this.GetViewPlayer();
			return viewPlayer != null && (this.m_inventory != null && this.m_inventory == viewPlayer.ComponentInput.SplitSourceInventory) && this.m_slotIndex == viewPlayer.ComponentInput.SplitSourceSlotIndex;
		}

		// Token: 0x06001B39 RID: 6969 RVA: 0x000D5834 File Offset: 0x000D3A34
		public bool HandleMoveItem(IInventory sourceInventory, int sourceSlotIndex, IInventory targetInventory, int targetSlotIndex, int count)
		{
			int slotValue = sourceInventory.GetSlotValue(sourceSlotIndex);
			int slotValue2 = targetInventory.GetSlotValue(targetSlotIndex);
			int slotCount = sourceInventory.GetSlotCount(sourceSlotIndex);
			int slotCount2 = targetInventory.GetSlotCount(targetSlotIndex);
			if (slotCount2 == 0 || slotValue == slotValue2)
			{
				int num = MathUtils.Min(targetInventory.GetSlotCapacity(targetSlotIndex, slotValue) - slotCount2, slotCount, count);
				if (num > 0)
				{
					int count2 = sourceInventory.RemoveSlotItems(sourceSlotIndex, num);
					targetInventory.AddSlotItems(targetSlotIndex, slotValue, count2);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001B3A RID: 6970 RVA: 0x000D58A0 File Offset: 0x000D3AA0
		public bool HandleDragDrop(IInventory sourceInventory, int sourceSlotIndex, DragMode dragMode, IInventory targetInventory, int targetSlotIndex)
		{
			int slotValue = sourceInventory.GetSlotValue(sourceSlotIndex);
			int slotValue2 = targetInventory.GetSlotValue(targetSlotIndex);
			int num = sourceInventory.GetSlotCount(sourceSlotIndex);
			int slotCount = targetInventory.GetSlotCount(targetSlotIndex);
			int slotCapacity = targetInventory.GetSlotCapacity(targetSlotIndex, slotValue);
			int slotProcessCapacity = targetInventory.GetSlotProcessCapacity(targetSlotIndex, slotValue);
			if (dragMode == DragMode.SingleItem)
			{
				num = MathUtils.Min(num, 1);
			}
			bool flag = false;
			if (slotProcessCapacity > 0)
			{
				int processCount = sourceInventory.RemoveSlotItems(sourceSlotIndex, MathUtils.Min(num, slotProcessCapacity));
				int num2;
				int num3;
				targetInventory.ProcessSlotItems(targetSlotIndex, slotValue, num, processCount, out num2, out num3);
				if (num2 != 0 && num3 != 0)
				{
					int count = MathUtils.Min(sourceInventory.GetSlotCapacity(sourceSlotIndex, num2), num3);
					sourceInventory.AddSlotItems(sourceSlotIndex, num2, count);
				}
				flag = true;
			}
			else if (!this.ProcessingOnly && (slotCount == 0 || slotValue == slotValue2) && slotCount < slotCapacity)
			{
				int num4 = MathUtils.Min(slotCapacity - slotCount, num);
				if (num4 > 0)
				{
					int count2 = sourceInventory.RemoveSlotItems(sourceSlotIndex, num4);
					targetInventory.AddSlotItems(targetSlotIndex, slotValue, count2);
					flag = true;
				}
			}
			else if (!this.ProcessingOnly && targetInventory.GetSlotCapacity(targetSlotIndex, slotValue) >= num && sourceInventory.GetSlotCapacity(sourceSlotIndex, slotValue2) >= slotCount && sourceInventory.GetSlotCount(sourceSlotIndex) == num)
			{
				int count3 = targetInventory.RemoveSlotItems(targetSlotIndex, slotCount);
				int count4 = sourceInventory.RemoveSlotItems(sourceSlotIndex, num);
				targetInventory.AddSlotItems(targetSlotIndex, slotValue, count4);
				sourceInventory.AddSlotItems(sourceSlotIndex, slotValue2, count3);
				flag = true;
			}
			if (flag)
			{
				AudioManager.PlaySound("Audio/UI/ItemMoved", 1f, 0f, 0f);
			}
			return flag;
		}

		// Token: 0x0400126B RID: 4715
		public BevelledRectangleWidget m_rectangleWidget;

		// Token: 0x0400126C RID: 4716
		public RectangleWidget m_highlightWidget;

		// Token: 0x0400126D RID: 4717
		public BlockIconWidget m_blockIconWidget;

		// Token: 0x0400126E RID: 4718
		public LabelWidget m_countWidget;

		// Token: 0x0400126F RID: 4719
		public ValueBarWidget m_healthBarWidget;

		// Token: 0x04001270 RID: 4720
		public RectangleWidget m_editOverlayWidget;

		// Token: 0x04001271 RID: 4721
		public RectangleWidget m_interactiveOverlayWidget;

		// Token: 0x04001272 RID: 4722
		public RectangleWidget m_foodOverlayWidget;

		// Token: 0x04001273 RID: 4723
		public LabelWidget m_splitLabelWidget;

		// Token: 0x04001274 RID: 4724
		public GameWidget m_gameWidget;

		// Token: 0x04001275 RID: 4725
		public DragHostWidget m_dragHostWidget;

		// Token: 0x04001276 RID: 4726
		public IInventory m_inventory;

		// Token: 0x04001277 RID: 4727
		public int m_slotIndex;

		// Token: 0x04001278 RID: 4728
		public DragMode? m_dragMode;

		// Token: 0x04001279 RID: 4729
		public bool m_focus;

		// Token: 0x0400127A RID: 4730
		public int m_lastCount = -1;

		// Token: 0x0400127B RID: 4731
		public InventoryDragData m_inventoryDragData;

		// Token: 0x0400127C RID: 4732
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400127D RID: 4733
		public ComponentPlayer m_componentPlayer;
	}
}
