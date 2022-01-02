using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Media;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D0 RID: 464
	public class SubsystemSignBlockBehavior : SubsystemBlockBehavior, IDrawable, IUpdateable
	{
		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000C49 RID: 3145 RVA: 0x00056E62 File Offset: 0x00055062
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					23,
					97,
					98,
					210,
					211
				};
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000C4A RID: 3146 RVA: 0x00056E75 File Offset: 0x00055075
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000C4B RID: 3147 RVA: 0x00056E78 File Offset: 0x00055078
		public int[] DrawOrders
		{
			get
			{
				return SubsystemSignBlockBehavior.m_drawOrders;
			}
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x00056E80 File Offset: 0x00055080
		public SignData GetSignData(Point3 point)
		{
			SubsystemSignBlockBehavior.TextData textData;
			if (this.m_textsByPoint.TryGetValue(point, out textData))
			{
				return new SignData
				{
					Lines = textData.Lines.ToArray<string>(),
					Colors = textData.Colors.ToArray<Color>(),
					Url = textData.Url
				};
			}
			return null;
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x00056ED4 File Offset: 0x000550D4
		public void SetSignData(Point3 point, string[] lines, Color[] colors, string url)
		{
			SubsystemSignBlockBehavior.TextData textData = new SubsystemSignBlockBehavior.TextData();
			textData.Point = point;
			for (int i = 0; i < 4; i++)
			{
				textData.Lines[i] = lines[i];
				textData.Colors[i] = colors[i];
			}
			textData.Url = url;
			this.m_textsByPoint[point] = textData;
			this.m_lastUpdatePositions.Clear();
		}

		// Token: 0x06000C4E RID: 3150 RVA: 0x00056F38 File Offset: 0x00055138
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValueFast = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
			int num = Terrain.ExtractContents(cellValueFast);
			int data = Terrain.ExtractData(cellValueFast);
			Block block = BlocksManager.Blocks[num];
			if (block is AttachedSignBlock)
			{
				Point3 point = CellFace.FaceToPoint3(AttachedSignBlock.GetFace(data));
				int x2 = x - point.X;
				int y2 = y - point.Y;
				int z2 = z - point.Z;
				int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x2, y2, z2);
				int num2 = Terrain.ExtractContents(cellValue);
				if (!BlocksManager.Blocks[num2].IsCollidable_(cellValue))
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
					return;
				}
			}
			else if (block is PostedSignBlock)
			{
				int value = PostedSignBlock.GetHanging(data) ? base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z) : base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
				if (!BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable_(value))
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				}
			}
		}

		// Token: 0x06000C4F RID: 3151 RVA: 0x0005704C File Offset: 0x0005524C
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			Point3 point = new Point3(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure)
			{
				SignData signData = this.GetSignData(point);
				if (signData != null && !string.IsNullOrEmpty(signData.Url))
				{
					WebBrowserManager.LaunchBrowser(signData.Url);
				}
			}
			else if (componentMiner.ComponentPlayer != null)
			{
				DialogsManager.ShowDialog(componentMiner.ComponentPlayer.GuiWidget, new EditSignDialog(this, point));
			}
			return true;
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x000570F4 File Offset: 0x000552F4
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			this.m_textsByPoint.Remove(key);
			this.m_lastUpdatePositions.Clear();
		}

		// Token: 0x06000C51 RID: 3153 RVA: 0x00057125 File Offset: 0x00055325
		public void Update(float dt)
		{
			this.UpdateRenderTarget();
		}

		// Token: 0x06000C52 RID: 3154 RVA: 0x0005712D File Offset: 0x0005532D
		public void Draw(Camera camera, int drawOrder)
		{
			this.DrawSigns(camera);
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x00057138 File Offset: 0x00055338
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			foreach (object obj in valuesDictionary.GetValue<ValuesDictionary>("Texts").Values)
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)obj;
				Point3 value = valuesDictionary2.GetValue<Point3>("Point");
				string value2 = valuesDictionary2.GetValue<string>("Line1", string.Empty);
				string value3 = valuesDictionary2.GetValue<string>("Line2", string.Empty);
				string value4 = valuesDictionary2.GetValue<string>("Line3", string.Empty);
				string value5 = valuesDictionary2.GetValue<string>("Line4", string.Empty);
				Color value6 = valuesDictionary2.GetValue<Color>("Color1", Color.Black);
				Color value7 = valuesDictionary2.GetValue<Color>("Color2", Color.Black);
				Color value8 = valuesDictionary2.GetValue<Color>("Color3", Color.Black);
				Color value9 = valuesDictionary2.GetValue<Color>("Color4", Color.Black);
				string value10 = valuesDictionary2.GetValue<string>("Url", string.Empty);
				this.SetSignData(value, new string[]
				{
					value2,
					value3,
					value4,
					value5
				}, new Color[]
				{
					value6,
					value7,
					value8,
					value9
				}, value10);
			}
			Display.DeviceReset += this.Display_DeviceReset;
		}

		// Token: 0x06000C54 RID: 3156 RVA: 0x000572E4 File Offset: 0x000554E4
		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			int num = 0;
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Texts", valuesDictionary2);
			foreach (SubsystemSignBlockBehavior.TextData textData in this.m_textsByPoint.Values)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary3.SetValue<Point3>("Point", textData.Point);
				if (!string.IsNullOrEmpty(textData.Lines[0]))
				{
					valuesDictionary3.SetValue<string>("Line1", textData.Lines[0]);
				}
				if (!string.IsNullOrEmpty(textData.Lines[1]))
				{
					valuesDictionary3.SetValue<string>("Line2", textData.Lines[1]);
				}
				if (!string.IsNullOrEmpty(textData.Lines[2]))
				{
					valuesDictionary3.SetValue<string>("Line3", textData.Lines[2]);
				}
				if (!string.IsNullOrEmpty(textData.Lines[3]))
				{
					valuesDictionary3.SetValue<string>("Line4", textData.Lines[3]);
				}
				if (textData.Colors[0] != Color.Black)
				{
					valuesDictionary3.SetValue<Color>("Color1", textData.Colors[0]);
				}
				if (textData.Colors[1] != Color.Black)
				{
					valuesDictionary3.SetValue<Color>("Color2", textData.Colors[1]);
				}
				if (textData.Colors[2] != Color.Black)
				{
					valuesDictionary3.SetValue<Color>("Color3", textData.Colors[2]);
				}
				if (textData.Colors[3] != Color.Black)
				{
					valuesDictionary3.SetValue<Color>("Color4", textData.Colors[3]);
				}
				if (!string.IsNullOrEmpty(textData.Url))
				{
					valuesDictionary3.SetValue<string>("Url", textData.Url);
				}
				valuesDictionary2.SetValue<ValuesDictionary>(num++.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
			}
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x00057504 File Offset: 0x00055704
		public override void Dispose()
		{
			base.Dispose();
			Utilities.Dispose<RenderTarget2D>(ref this.m_renderTarget);
			Display.DeviceReset -= this.Display_DeviceReset;
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x00057528 File Offset: 0x00055728
		public void Display_DeviceReset()
		{
			this.InvalidateRenderTarget();
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x00057530 File Offset: 0x00055730
		public RenderTarget2D CreateRenderTarget()
		{
			return new RenderTarget2D(128, 128, 1, ColorFormat.Rgba8888, DepthFormat.None);
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x00057544 File Offset: 0x00055744
		public void InvalidateRenderTarget()
		{
			this.m_lastUpdatePositions.Clear();
			for (int i = 0; i < this.m_textureLocations.Length; i++)
			{
				this.m_textureLocations[i] = null;
			}
			foreach (SubsystemSignBlockBehavior.TextData textData in this.m_textsByPoint.Values)
			{
				textData.TextureLocation = null;
			}
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x000575C8 File Offset: 0x000557C8
		public void RenderText(FontBatch2D fontBatch, FlatBatch2D flatBatch, SubsystemSignBlockBehavior.TextData textData)
		{
			if (textData.TextureLocation == null)
			{
				return;
			}
			List<string> list = new List<string>();
			List<Color> list2 = new List<Color>();
			for (int i = 0; i < textData.Lines.Length; i++)
			{
				if (!string.IsNullOrEmpty(textData.Lines[i]))
				{
					list.Add(textData.Lines[i].Replace("\\", "").ToUpper());
					list2.Add(textData.Colors[i]);
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			for (int j = 0; j < list.Count; j++)
			{
				fontBatch.QueueText(list[j], new Vector2(0f, (float)j * fontBatch.Font.LineHeight), 0f, list2[j], TextAnchor.Default);
			}
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x00057694 File Offset: 0x00055894
		public void UpdateRenderTarget()
		{
			bool flag = false;
			foreach (GameWidget gameWidget in this.m_subsystemViews.GameWidgets)
			{
				bool flag2 = false;
				foreach (Vector3 v2 in this.m_lastUpdatePositions)
				{
					if (Vector3.DistanceSquared(gameWidget.ActiveCamera.ViewPosition, v2) < 4f)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			this.m_lastUpdatePositions.Clear();
			this.m_lastUpdatePositions.AddRange(from v in this.m_subsystemViews.GameWidgets
			select v.ActiveCamera.ViewPosition);
			this.m_nearTexts.Clear();
			this.m_texturesByPoint.Clear();
			foreach (SubsystemSignBlockBehavior.TextData textData in this.m_textsByPoint.Values)
			{
				Point3 point = textData.Point;
				float num = this.m_subsystemViews.CalculateSquaredDistanceFromNearestView(new Vector3(point));
				if (num <= 400f)
				{
					textData.Distance = num;
					this.m_nearTexts.Add(textData);
				}
			}
			this.m_nearTexts.Sort((SubsystemSignBlockBehavior.TextData d1, SubsystemSignBlockBehavior.TextData d2) => Comparer<float>.Default.Compare(d1.Distance, d2.Distance));
			if (this.m_nearTexts.Count > 32)
			{
				this.m_nearTexts.RemoveRange(32, this.m_nearTexts.Count - 32);
			}
			foreach (SubsystemSignBlockBehavior.TextData textData2 in this.m_nearTexts)
			{
				textData2.ToBeRenderedFrame = Time.FrameIndex;
			}
			bool flag3 = false;
			for (int i = 0; i < MathUtils.Min(this.m_nearTexts.Count, 32); i++)
			{
				SubsystemSignBlockBehavior.TextData textData3 = this.m_nearTexts[i];
				if (textData3.TextureLocation == null)
				{
					int num2 = this.m_textureLocations.FirstIndex((SubsystemSignBlockBehavior.TextData d) => d == null);
					if (num2 < 0)
					{
						num2 = this.m_textureLocations.FirstIndex((SubsystemSignBlockBehavior.TextData d) => d.ToBeRenderedFrame != Time.FrameIndex);
					}
					if (num2 >= 0)
					{
						SubsystemSignBlockBehavior.TextData textData4 = this.m_textureLocations[num2];
						if (textData4 != null)
						{
							textData4.TextureLocation = null;
							this.m_textureLocations[num2] = null;
						}
						this.m_textureLocations[num2] = textData3;
						textData3.TextureLocation = new int?(num2);
						flag3 = true;
					}
				}
			}
			if (flag3)
			{
				RenderTarget2D renderTarget = Display.RenderTarget;
				try
				{
					FlatBatch2D flatBatch = this.m_primitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, BlendState.Opaque);
					FontBatch2D fontBatch = this.m_primitivesRenderer2D.FontBatch(this.m_font, 1, DepthStencilState.None, null, BlendState.Opaque, SamplerState.PointClamp);
					for (int j = 0; j < this.m_textsByPoint.Values.Count; j++)
					{
						SubsystemSignBlockBehavior.TextData textData5 = this.m_textsByPoint.Values.ElementAt(j);
						if (textData5.renderTarget2D == null)
						{
							textData5.renderTarget2D = this.CreateRenderTarget();
						}
						Display.RenderTarget = textData5.renderTarget2D;
						Display.Clear(new Vector4?(new Vector4(Color.Transparent)), null, null);
						if (textData5 != null)
						{
							this.RenderText(fontBatch, flatBatch, textData5);
						}
						this.m_primitivesRenderer2D.Flush(true, int.MaxValue);
					}
				}
				finally
				{
					Display.RenderTarget = renderTarget;
				}
			}
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x00057AC0 File Offset: 0x00055CC0
		public void DrawSigns(Camera camera)
		{
			if (this.m_nearTexts.Count > 0)
			{
				foreach (SubsystemSignBlockBehavior.TextData textData in this.m_nearTexts)
				{
					TexturedBatch3D texturedBatch3D = this.m_primitivesRenderer3D.TexturedBatch(textData.renderTarget2D, false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
					if (textData.TextureLocation != null)
					{
						int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(textData.Point.X, textData.Point.Y, textData.Point.Z);
						int num = Terrain.ExtractContents(cellValue);
						SignBlock signBlock = BlocksManager.Blocks[num] as SignBlock;
						if (signBlock != null)
						{
							int data = Terrain.ExtractData(cellValue);
							BlockMesh signSurfaceBlockMesh = signBlock.GetSignSurfaceBlockMesh(data);
							if (signSurfaceBlockMesh != null)
							{
								TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(textData.Point.X, textData.Point.Z);
								if (chunkAtCell != null && chunkAtCell.State >= TerrainChunkState.InvalidVertices1)
								{
									textData.Light = Terrain.ExtractLight(cellValue);
								}
								float num2 = LightingManager.LightIntensityByLightValue[textData.Light];
								Color color = new Color(num2, num2, num2);
								Vector3 signSurfaceNormal = signBlock.GetSignSurfaceNormal(data);
								Vector3 vector = new Vector3((float)textData.Point.X, (float)textData.Point.Y, (float)textData.Point.Z);
								float num3 = Vector3.Dot(camera.ViewPosition - (vector + new Vector3(0.5f)), signSurfaceNormal);
								Vector3 v = MathUtils.Max(0.01f * num3, 0.005f) * signSurfaceNormal;
								for (int i = 0; i < signSurfaceBlockMesh.Indices.Count / 3; i++)
								{
									BlockMeshVertex blockMeshVertex = signSurfaceBlockMesh.Vertices.Array[(int)signSurfaceBlockMesh.Indices.Array[i * 3]];
									BlockMeshVertex blockMeshVertex2 = signSurfaceBlockMesh.Vertices.Array[(int)signSurfaceBlockMesh.Indices.Array[i * 3 + 1]];
									BlockMeshVertex blockMeshVertex3 = signSurfaceBlockMesh.Vertices.Array[(int)signSurfaceBlockMesh.Indices.Array[i * 3 + 2]];
									Vector3 p = blockMeshVertex.Position + vector + v;
									Vector3 p2 = blockMeshVertex2.Position + vector + v;
									Vector3 p3 = blockMeshVertex3.Position + vector + v;
									Vector2 textureCoordinates = blockMeshVertex.TextureCoordinates;
									Vector2 textureCoordinates2 = blockMeshVertex2.TextureCoordinates;
									Vector2 textureCoordinates3 = blockMeshVertex3.TextureCoordinates;
									texturedBatch3D.QueueTriangle(p, p2, p3, textureCoordinates, textureCoordinates2, textureCoordinates3, color);
								}
							}
						}
					}
				}
				this.m_primitivesRenderer3D.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
			}
		}

		// Token: 0x04000620 RID: 1568
		public const float m_maxVisibilityDistanceSqr = 400f;

		// Token: 0x04000621 RID: 1569
		public const float m_minUpdateDistance = 2f;

		// Token: 0x04000622 RID: 1570
		public const int m_textWidth = 128;

		// Token: 0x04000623 RID: 1571
		public const int m_textHeight = 32;

		// Token: 0x04000624 RID: 1572
		public const int m_maxTexts = 32;

		// Token: 0x04000625 RID: 1573
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x04000626 RID: 1574
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000627 RID: 1575
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000628 RID: 1576
		public Dictionary<Point3, SubsystemSignBlockBehavior.TextData> m_textsByPoint = new Dictionary<Point3, SubsystemSignBlockBehavior.TextData>();

		// Token: 0x04000629 RID: 1577
		public List<RenderTarget2D> m_texturesByPoint = new List<RenderTarget2D>();

		// Token: 0x0400062A RID: 1578
		public SubsystemSignBlockBehavior.TextData[] m_textureLocations = new SubsystemSignBlockBehavior.TextData[32];

		// Token: 0x0400062B RID: 1579
		public List<SubsystemSignBlockBehavior.TextData> m_nearTexts = new List<SubsystemSignBlockBehavior.TextData>();

		// Token: 0x0400062C RID: 1580
		public BitmapFont m_font = LabelWidget.BitmapFont;

		// Token: 0x0400062D RID: 1581
		public RenderTarget2D m_renderTarget;

		// Token: 0x0400062E RID: 1582
		public List<Vector3> m_lastUpdatePositions = new List<Vector3>();

		// Token: 0x0400062F RID: 1583
		public PrimitivesRenderer2D m_primitivesRenderer2D = new PrimitivesRenderer2D();

		// Token: 0x04000630 RID: 1584
		public PrimitivesRenderer3D m_primitivesRenderer3D = new PrimitivesRenderer3D();

		// Token: 0x04000631 RID: 1585
		public bool ShowSignsTexture;

		// Token: 0x04000632 RID: 1586
		public bool CopySignsText;

		// Token: 0x04000633 RID: 1587
		public static int[] m_drawOrders = new int[]
		{
			50
		};

		// Token: 0x020004A5 RID: 1189
		public class TextData
		{
			// Token: 0x0400171D RID: 5917
			public Point3 Point;

			// Token: 0x0400171E RID: 5918
			public string[] Lines = new string[]
			{
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty
			};

			// Token: 0x0400171F RID: 5919
			public Color[] Colors = new Color[]
			{
				Color.Black,
				Color.Black,
				Color.Black,
				Color.Black
			};

			// Token: 0x04001720 RID: 5920
			public string Url = string.Empty;

			// Token: 0x04001721 RID: 5921
			public int? TextureLocation;

			// Token: 0x04001722 RID: 5922
			public float UsedTextureWidth;

			// Token: 0x04001723 RID: 5923
			public float UsedTextureHeight;

			// Token: 0x04001724 RID: 5924
			public float Distance;

			// Token: 0x04001725 RID: 5925
			public int ToBeRenderedFrame;

			// Token: 0x04001726 RID: 5926
			public RenderTarget2D renderTarget2D;

			// Token: 0x04001727 RID: 5927
			public int Light;
		}
	}
}
