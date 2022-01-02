using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001EE RID: 494
	public class ComponentBlockHighlight : Component, IDrawable, IUpdateable
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000D91 RID: 3473 RVA: 0x00062BA4 File Offset: 0x00060DA4
		// (set) Token: 0x06000D92 RID: 3474 RVA: 0x00062BAC File Offset: 0x00060DAC
		public Point3? NearbyEditableCell { get; set; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x06000D93 RID: 3475 RVA: 0x00062BB5 File Offset: 0x00060DB5
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.BlockHighlight;
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000D94 RID: 3476 RVA: 0x00062BBC File Offset: 0x00060DBC
		public int[] DrawOrders
		{
			get
			{
				return ComponentBlockHighlight.m_drawOrders;
			}
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x00062BC4 File Offset: 0x00060DC4
		public void Update(float dt)
		{
			Camera activeCamera = this.m_componentPlayer.GameWidget.ActiveCamera;
			Ray3? ray = new Ray3?(new Ray3(activeCamera.ViewPosition, activeCamera.ViewDirection));
			this.NearbyEditableCell = null;
			if (ray != null)
			{
				this.m_highlightRaycastResult = this.m_componentPlayer.ComponentMiner.Raycast(ray.Value, RaycastMode.Digging, true, true, true);
				if (!(this.m_highlightRaycastResult is TerrainRaycastResult))
				{
					return;
				}
				TerrainRaycastResult terrainRaycastResult = (TerrainRaycastResult)this.m_highlightRaycastResult;
				if (terrainRaycastResult.Distance < 3f)
				{
					Point3 point = terrainRaycastResult.CellFace.Point;
					int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
					if (block is CrossBlock)
					{
						terrainRaycastResult.Distance = MathUtils.Max(terrainRaycastResult.Distance, 0.1f);
						this.m_highlightRaycastResult = terrainRaycastResult;
					}
					if (block.IsEditable_(cellValue))
					{
						this.NearbyEditableCell = new Point3?(terrainRaycastResult.CellFace.Point);
						return;
					}
				}
			}
			else
			{
				this.m_highlightRaycastResult = null;
			}
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x00062CF8 File Offset: 0x00060EF8
		public void Draw(Camera camera, int drawOrder)
		{
			if (camera.GameWidget.PlayerData == this.m_componentPlayer.PlayerData)
			{
				if (drawOrder == ComponentBlockHighlight.m_drawOrders[0])
				{
					this.DrawFillHighlight(camera);
					this.DrawOutlineHighlight(camera);
					this.DrawReticleHighlight(camera);
					return;
				}
				this.DrawRayHighlight(camera);
			}
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x00062D44 File Offset: 0x00060F44
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAnimatedTextures = base.Project.FindSubsystem<SubsystemAnimatedTextures>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_shader = new Shader(ModsManager.GetInPakOrStorageFile<string>("Shaders/Highlight", ".vsh"), ModsManager.GetInPakOrStorageFile<string>("Shaders/Highlight", ".psh"), new ShaderMacro[]
			{
				new ShaderMacro("ShadowShader")
			});
		}

		// Token: 0x06000D98 RID: 3480 RVA: 0x00062DD8 File Offset: 0x00060FD8
		public void DrawRayHighlight(Camera camera)
		{
			Ray3 ray = default(Ray3);
			float num;
			if (this.m_highlightRaycastResult is TerrainRaycastResult)
			{
				TerrainRaycastResult terrainRaycastResult = (TerrainRaycastResult)this.m_highlightRaycastResult;
				ray = terrainRaycastResult.Ray;
				num = MathUtils.Min(terrainRaycastResult.Distance, 2f);
			}
			else if (this.m_highlightRaycastResult is BodyRaycastResult)
			{
				BodyRaycastResult bodyRaycastResult = (BodyRaycastResult)this.m_highlightRaycastResult;
				ray = bodyRaycastResult.Ray;
				num = MathUtils.Min(bodyRaycastResult.Distance, 2f);
			}
			else if (this.m_highlightRaycastResult is MovingBlocksRaycastResult)
			{
				MovingBlocksRaycastResult movingBlocksRaycastResult = (MovingBlocksRaycastResult)this.m_highlightRaycastResult;
				ray = movingBlocksRaycastResult.Ray;
				num = MathUtils.Min(movingBlocksRaycastResult.Distance, 2f);
			}
			else
			{
				if (!(this.m_highlightRaycastResult is Ray3))
				{
					return;
				}
				ray = (Ray3)this.m_highlightRaycastResult;
				num = 2f;
			}
			Color color = Color.White * 0.5f;
			Color color2 = Color.Lerp(color, Color.Transparent, MathUtils.Saturate(num / 2f));
			FlatBatch3D flatBatch3D = this.m_primitivesRenderer3D.FlatBatch(0, null, null, null);
			flatBatch3D.QueueLine(ray.Position, ray.Position + ray.Direction * num, color, color2);
			flatBatch3D.Flush(camera.ViewProjectionMatrix, true);
		}

		// Token: 0x06000D99 RID: 3481 RVA: 0x00062F0B File Offset: 0x0006110B
		public void DrawReticleHighlight(Camera camera)
		{
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x00062F0D File Offset: 0x0006110D
		public void DrawFillHighlight(Camera camera)
		{
		}

		// Token: 0x06000D9B RID: 3483 RVA: 0x00062F10 File Offset: 0x00061110
		public void DrawOutlineHighlight(Camera camera)
		{
			if (camera.UsesMovementControls || this.m_componentPlayer.ComponentHealth.Health <= 0f || !this.m_componentPlayer.ComponentGui.ControlsContainerWidget.IsVisible)
			{
				return;
			}
			if (this.m_componentPlayer.ComponentMiner.DigCellFace != null)
			{
				CellFace value = this.m_componentPlayer.ComponentMiner.DigCellFace.Value;
				BoundingBox cellFaceBoundingBox = this.GetCellFaceBoundingBox(value.Point);
				ComponentBlockHighlight.DrawBoundingBoxFace(this.m_primitivesRenderer3D.FlatBatch(0, DepthStencilState.None, null, null), value.Face, cellFaceBoundingBox.Min, cellFaceBoundingBox.Max, Color.Black);
			}
			else
			{
				if (!this.m_componentPlayer.ComponentAimingSights.IsSightsVisible && (SettingsManager.LookControlMode == LookControlMode.SplitTouch || !this.m_componentPlayer.ComponentInput.IsControlledByTouch) && this.m_highlightRaycastResult is TerrainRaycastResult)
				{
					CellFace cellFace = ((TerrainRaycastResult)this.m_highlightRaycastResult).CellFace;
					BoundingBox cellFaceBoundingBox2 = this.GetCellFaceBoundingBox(cellFace.Point);
					ComponentBlockHighlight.DrawBoundingBoxFace(this.m_primitivesRenderer3D.FlatBatch(0, DepthStencilState.None, null, null), cellFace.Face, cellFaceBoundingBox2.Min, cellFaceBoundingBox2.Max, Color.Black);
				}
				if (this.NearbyEditableCell != null)
				{
					BoundingBox cellFaceBoundingBox3 = this.GetCellFaceBoundingBox(this.NearbyEditableCell.Value);
					this.m_primitivesRenderer3D.FlatBatch(0, DepthStencilState.None, null, null).QueueBoundingBox(cellFaceBoundingBox3, Color.Black);
				}
			}
			this.m_primitivesRenderer3D.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
		}

		// Token: 0x06000D9C RID: 3484 RVA: 0x000630B4 File Offset: 0x000612B4
		public static void DrawBoundingBoxFace(FlatBatch3D batch, int face, Vector3 c1, Vector3 c2, Color color)
		{
			switch (face)
			{
			case 0:
				batch.QueueLine(new Vector3(c1.X, c1.Y, c2.Z), new Vector3(c2.X, c1.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c2.Y, c2.Z), new Vector3(c1.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c1.Y, c2.Z), new Vector3(c2.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c1.X, c2.Y, c2.Z), new Vector3(c1.X, c1.Y, c2.Z), color);
				return;
			case 1:
				batch.QueueLine(new Vector3(c2.X, c1.Y, c2.Z), new Vector3(c2.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c1.Y, c1.Z), new Vector3(c2.X, c2.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c2.X, c2.Y, c1.Z), new Vector3(c2.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c1.Y, c1.Z), new Vector3(c2.X, c1.Y, c2.Z), color);
				return;
			case 2:
				batch.QueueLine(new Vector3(c1.X, c1.Y, c1.Z), new Vector3(c2.X, c1.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c2.X, c1.Y, c1.Z), new Vector3(c2.X, c2.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c2.X, c2.Y, c1.Z), new Vector3(c1.X, c2.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c1.X, c2.Y, c1.Z), new Vector3(c1.X, c1.Y, c1.Z), color);
				return;
			case 3:
				batch.QueueLine(new Vector3(c1.X, c2.Y, c2.Z), new Vector3(c1.X, c1.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c1.X, c2.Y, c1.Z), new Vector3(c1.X, c1.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c1.X, c1.Y, c1.Z), new Vector3(c1.X, c1.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c1.X, c2.Y, c1.Z), new Vector3(c1.X, c2.Y, c2.Z), color);
				return;
			case 4:
				batch.QueueLine(new Vector3(c2.X, c2.Y, c2.Z), new Vector3(c1.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c2.Y, c1.Z), new Vector3(c1.X, c2.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c1.X, c2.Y, c1.Z), new Vector3(c1.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c2.Y, c1.Z), new Vector3(c2.X, c2.Y, c2.Z), color);
				return;
			case 5:
				batch.QueueLine(new Vector3(c1.X, c1.Y, c2.Z), new Vector3(c2.X, c1.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c1.X, c1.Y, c1.Z), new Vector3(c2.X, c1.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c1.X, c1.Y, c1.Z), new Vector3(c1.X, c1.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c1.Y, c1.Z), new Vector3(c2.X, c1.Y, c2.Z), color);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000D9D RID: 3485 RVA: 0x000635F8 File Offset: 0x000617F8
		public BoundingBox GetCellFaceBoundingBox(Point3 point)
		{
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
			BoundingBox[] customCollisionBoxes = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].GetCustomCollisionBoxes(this.m_subsystemTerrain, cellValue);
			Vector3 vector = new Vector3((float)point.X, (float)point.Y, (float)point.Z);
			if (customCollisionBoxes.Length != 0)
			{
				BoundingBox? boundingBox = null;
				for (int i = 0; i < customCollisionBoxes.Length; i++)
				{
					if (customCollisionBoxes[i] != default(BoundingBox))
					{
						boundingBox = new BoundingBox?((boundingBox != null) ? BoundingBox.Union(boundingBox.Value, customCollisionBoxes[i]) : customCollisionBoxes[i]);
					}
				}
				if (boundingBox == null)
				{
					boundingBox = new BoundingBox?(new BoundingBox(Vector3.Zero, Vector3.One));
				}
				return new BoundingBox(boundingBox.Value.Min + vector, boundingBox.Value.Max + vector);
			}
			return new BoundingBox(vector, vector + Vector3.One);
		}

		// Token: 0x0400071C RID: 1820
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400071D RID: 1821
		public SubsystemAnimatedTextures m_subsystemAnimatedTextures;

		// Token: 0x0400071E RID: 1822
		public SubsystemSky m_subsystemSky;

		// Token: 0x0400071F RID: 1823
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000720 RID: 1824
		public PrimitivesRenderer3D m_primitivesRenderer3D = new PrimitivesRenderer3D();

		// Token: 0x04000721 RID: 1825
		public Shader m_shader;

		// Token: 0x04000722 RID: 1826
		public ComponentBlockHighlight.Geometry m_geometry;

		// Token: 0x04000723 RID: 1827
		public CellFace m_cellFace;

		// Token: 0x04000724 RID: 1828
		public int m_value;

		// Token: 0x04000725 RID: 1829
		public object m_highlightRaycastResult;

		// Token: 0x04000726 RID: 1830
		public static int[] m_drawOrders = new int[]
		{
			1,
			2000
		};

		// Token: 0x020004BB RID: 1211
		public class Geometry : TerrainGeometry
		{
			// Token: 0x060020EE RID: 8430 RVA: 0x000E9790 File Offset: 0x000E7990
			public Geometry()
			{
				TerrainGeometrySubset terrainGeometrySubset = new TerrainGeometrySubset();
				TerrainGeometrySubset[] array = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
				this.SubsetOpaque = terrainGeometrySubset;
				this.SubsetAlphaTest = terrainGeometrySubset;
				this.SubsetTransparent = terrainGeometrySubset;
				this.OpaqueSubsetsByFace = array;
				this.AlphaTestSubsetsByFace = array;
				this.TransparentSubsetsByFace = array;
			}
		}
	}
}
