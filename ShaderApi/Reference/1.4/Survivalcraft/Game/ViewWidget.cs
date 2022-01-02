using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020003A7 RID: 935
	public class ViewWidget : TouchInputWidget, IDragTargetWidget
	{
		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06001C8C RID: 7308 RVA: 0x000DCFD0 File Offset: 0x000DB1D0
		// (set) Token: 0x06001C8D RID: 7309 RVA: 0x000DCFD8 File Offset: 0x000DB1D8
		public GameWidget GameWidget { get; set; }

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06001C8E RID: 7310 RVA: 0x000DCFE4 File Offset: 0x000DB1E4
		public Point2? ScalingRenderTargetSize
		{
			get
			{
				if (this.m_scalingRenderTarget == null)
				{
					return null;
				}
				return new Point2?(new Point2(this.m_scalingRenderTarget.Width, this.m_scalingRenderTarget.Height));
			}
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x000DD024 File Offset: 0x000DB224
		public override void ChangeParent(ContainerWidget parentWidget)
		{
			if (parentWidget is GameWidget)
			{
				this.GameWidget = (GameWidget)parentWidget;
				this.m_subsystemDrawing = this.GameWidget.SubsystemGameWidgets.Project.FindSubsystem<SubsystemDrawing>(true);
				base.ChangeParent(parentWidget);
				return;
			}
			throw new InvalidOperationException("ViewWidget must be a child of GameWidget.");
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x000DD073 File Offset: 0x000DB273
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x000DD083 File Offset: 0x000DB283
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.GameWidget.PlayerData.ComponentPlayer != null && this.GameWidget.PlayerData.IsReadyForPlaying)
			{
				this.DrawToScreen(dc);
			}
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x000DD0B0 File Offset: 0x000DB2B0
		public override void Dispose()
		{
			base.Dispose();
			Utilities.Dispose<RenderTarget2D>(ref this.m_scalingRenderTarget);
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x000DD0C3 File Offset: 0x000DB2C3
		public void DragOver(Widget dragWidget, object data)
		{
		}

		// Token: 0x06001C94 RID: 7316 RVA: 0x000DD0C8 File Offset: 0x000DB2C8
		public void DragDrop(Widget dragWidget, object data)
		{
			InventoryDragData inventoryDragData = data as InventoryDragData;
			if (inventoryDragData != null && GameManager.Project != null)
			{
				SubsystemPickables subsystemPickables = GameManager.Project.FindSubsystem<SubsystemPickables>(true);
				ComponentPlayer componentPlayer = this.GameWidget.PlayerData.ComponentPlayer;
				int slotValue = inventoryDragData.Inventory.GetSlotValue(inventoryDragData.SlotIndex);
				int count = (componentPlayer != null && componentPlayer.ComponentInput.SplitSourceInventory == inventoryDragData.Inventory && componentPlayer.ComponentInput.SplitSourceSlotIndex == inventoryDragData.SlotIndex) ? 1 : ((inventoryDragData.DragMode != DragMode.SingleItem) ? inventoryDragData.Inventory.GetSlotCount(inventoryDragData.SlotIndex) : MathUtils.Min(inventoryDragData.Inventory.GetSlotCount(inventoryDragData.SlotIndex), 1));
				int num = inventoryDragData.Inventory.RemoveSlotItems(inventoryDragData.SlotIndex, count);
				if (num > 0)
				{
					Vector2 vector = dragWidget.WidgetToScreen(dragWidget.ActualSize / 2f);
					Vector3 value = Vector3.Normalize(this.GameWidget.ActiveCamera.ScreenToWorld(new Vector3(vector.X, vector.Y, 1f), Matrix.Identity) - this.GameWidget.ActiveCamera.ViewPosition) * 12f;
					subsystemPickables.AddPickable(slotValue, num, this.GameWidget.ActiveCamera.ViewPosition, new Vector3?(value), null);
				}
			}
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x000DD230 File Offset: 0x000DB430
		public void SetupScalingRenderTarget()
		{
			float num = (SettingsManager.ResolutionMode == ResolutionMode.Low) ? 0.5f : ((SettingsManager.ResolutionMode != ResolutionMode.Medium) ? 1f : 0.75f);
			float num2 = base.GlobalTransform.Right.Length();
			float num3 = base.GlobalTransform.Up.Length();
			Vector2 vector = new Vector2(base.ActualSize.X * num2, base.ActualSize.Y * num3);
			Point2 point = new Point2
			{
				X = (int)MathUtils.Round(vector.X * num),
				Y = (int)MathUtils.Round(vector.Y * num)
			};
			if ((num < 1f || base.GlobalColorTransform != Color.White) && point.X > 0 && point.Y > 0)
			{
				if (this.m_scalingRenderTarget == null || this.m_scalingRenderTarget.Width != point.X || this.m_scalingRenderTarget.Height != point.Y)
				{
					Utilities.Dispose<RenderTarget2D>(ref this.m_scalingRenderTarget);
					this.m_scalingRenderTarget = new RenderTarget2D(point.X, point.Y, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
				}
				Display.RenderTarget = this.m_scalingRenderTarget;
				Display.Clear(new Color?(Color.Black), new float?(1f), new int?(0));
				return;
			}
			Utilities.Dispose<RenderTarget2D>(ref this.m_scalingRenderTarget);
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x000DD3B0 File Offset: 0x000DB5B0
		public void ApplyScalingRenderTarget(Widget.DrawContext dc)
		{
			if (this.m_scalingRenderTarget != null)
			{
				BlendState blendState = (base.GlobalColorTransform.A < byte.MaxValue) ? BlendState.AlphaBlend : BlendState.Opaque;
				TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(this.m_scalingRenderTarget, false, 0, DepthStencilState.None, RasterizerState.CullNoneScissor, blendState, SamplerState.PointClamp);
				int count = texturedBatch2D.TriangleVertices.Count;
				texturedBatch2D.QueueQuad(Vector2.Zero, base.ActualSize, 0f, Vector2.Zero, Vector2.One, base.GlobalColorTransform);
				texturedBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
				dc.PrimitivesRenderer2D.Flush(true, int.MaxValue);
			}
		}

		// Token: 0x06001C97 RID: 7319 RVA: 0x000DD460 File Offset: 0x000DB660
		public void DrawToScreen(Widget.DrawContext dc)
		{
			this.GameWidget.ActiveCamera.PrepareForDrawing();
			RenderTarget2D renderTarget = Display.RenderTarget;
			this.SetupScalingRenderTarget();
			try
			{
				this.m_subsystemDrawing.Draw(this.GameWidget.ActiveCamera);
			}
			finally
			{
				Display.RenderTarget = renderTarget;
			}
			this.ApplyScalingRenderTarget(dc);
			ModsManager.HookAction("DrawToScreen", delegate(ModLoader loader)
			{
				loader.DrawToScreen(this, dc);
				return false;
			});
		}

		// Token: 0x0400133F RID: 4927
		public SubsystemDrawing m_subsystemDrawing;

		// Token: 0x04001340 RID: 4928
		public RenderTarget2D m_scalingRenderTarget;

		// Token: 0x04001341 RID: 4929
		public static RenderTarget2D ScreenTexture = new RenderTarget2D(Window.Size.X, Window.Size.Y, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
	}
}
