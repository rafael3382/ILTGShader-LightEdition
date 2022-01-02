using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C9 RID: 201
	public abstract class PostedSignBlock : SignBlock, IElectricElementBlock, IPaintableBlock
	{
		// Token: 0x0600040E RID: 1038 RVA: 0x000176A8 File Offset: 0x000158A8
		public PostedSignBlock(string modelName, int coloredTextureSlot, int attachedSignBlockIndex)
		{
			this.m_modelName = modelName;
			this.m_coloredTextureSlot = coloredTextureSlot;
			this.m_attachedSignBlockIndex = attachedSignBlockIndex;
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x00017734 File Offset: 0x00015934
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName, null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Sign", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Post", true).ParentBone);
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Surface", true).ParentBone);
			for (int i = 0; i < 16; i++)
			{
				bool hanging = PostedSignBlock.GetHanging(i);
				Matrix matrix = Matrix.CreateRotationY((float)PostedSignBlock.GetDirection(i) * 3.14159274f / 4f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				if (hanging)
				{
					matrix *= Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, 1f, 0f);
				}
				this.m_directions[i] = matrix.Forward;
				BlockMesh blockMesh = new BlockMesh();
				blockMesh.AppendModelMeshPart(model.FindMesh("Sign", true).MeshParts[0], boneAbsoluteTransform * matrix, false, hanging, false, false, Color.White);
				BlockMesh blockMesh2 = new BlockMesh();
				blockMesh2.AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform2 * matrix, false, hanging, false, false, Color.White);
				this.m_blockMeshes[i] = new BlockMesh();
				this.m_blockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_blockMeshes[i].AppendBlockMesh(blockMesh2);
				this.m_coloredBlockMeshes[i] = new BlockMesh();
				this.m_coloredBlockMeshes[i].AppendBlockMesh(this.m_blockMeshes[i]);
				this.m_blockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
				this.m_coloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
				this.m_collisionBoxes[i] = new BoundingBox[2];
				this.m_collisionBoxes[i][0] = blockMesh.CalculateBoundingBox();
				this.m_collisionBoxes[i][1] = blockMesh2.CalculateBoundingBox();
				this.m_surfaceMeshes[i] = new BlockMesh();
				this.m_surfaceMeshes[i].AppendModelMeshPart(model.FindMesh("Surface", true).MeshParts[0], boneAbsoluteTransform3 * matrix, false, hanging, false, false, Color.White);
				this.m_surfaceNormals[i] = -matrix.Forward;
				if (hanging)
				{
					for (int j = 0; j < this.m_surfaceMeshes[i].Vertices.Count; j++)
					{
						Vector2 textureCoordinates = this.m_surfaceMeshes[i].Vertices.Array[j].TextureCoordinates;
						textureCoordinates.Y = 1f - textureCoordinates.Y;
						this.m_surfaceMeshes[i].Vertices.Array[j].TextureCoordinates = textureCoordinates;
					}
				}
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Sign", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.6f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.6f, 0f), false, false, false, false, Color.White);
			this.m_standaloneColoredBlockMesh.AppendBlockMesh(this.m_standaloneBlockMesh);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneColoredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
			base.Initialize();
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x00017BA0 File Offset: 0x00015DA0
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = PostedSignBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, base.GetDisplayName(subsystemTerrain, value));
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00017BC8 File Offset: 0x00015DC8
		public override string GetCategory(int value)
		{
			if (PostedSignBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return "Painted";
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00017BF7 File Offset: 0x00015DF7
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, PostedSignBlock.SetColor(0, null));
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(this.BlockIndex, 0, PostedSignBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x00017C08 File Offset: 0x00015E08
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int? color = PostedSignBlock.GetColor(Terrain.ExtractData(oldValue));
			int data = PostedSignBlock.SetColor(0, color);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, data),
				Count = 1
			});
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x00017C5C File Offset: 0x00015E5C
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int? color = PostedSignBlock.GetColor(Terrain.ExtractData(value));
			if (color != null)
			{
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, color), this.m_coloredTextureSlot);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.DefaultTextureSlot);
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00017CB8 File Offset: 0x00015EB8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int variant = PostedSignBlock.GetVariant(data);
			int? color = PostedSignBlock.GetColor(data);
			if (color != null)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_coloredBlockMeshes[variant], SubsystemPalette.GetColor(generator, color), null, geometry.SubsetOpaque);
			}
			else
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[variant], Color.White, null, geometry.SubsetOpaque);
			}
			generator.GenerateWireVertices(value, x, y, z, PostedSignBlock.GetHanging(data) ? 5 : 4, 0.01f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00017D64 File Offset: 0x00015F64
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = PostedSignBlock.GetColor(Terrain.ExtractData(value));
			if (color2 != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneColoredBlockMesh, color * SubsystemPalette.GetColor(environmentData, color2), 1.25f * size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 1.25f * size, ref matrix, environmentData);
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x00017DC6 File Offset: 0x00015FC6
		public int? GetPaintColor(int value)
		{
			return PostedSignBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x00017DD4 File Offset: 0x00015FD4
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, PostedSignBlock.SetColor(data, color));
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00017DF8 File Offset: 0x00015FF8
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int variant = PostedSignBlock.GetVariant(Terrain.ExtractData(value));
			return this.m_collisionBoxes[variant];
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x00017E1C File Offset: 0x0001601C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int? color = PostedSignBlock.GetColor(Terrain.ExtractData(value));
			if (raycastResult.CellFace.Face < 4)
			{
				int data = AttachedSignBlock.SetFace(AttachedSignBlock.SetColor(0, color), raycastResult.CellFace.Face);
				return new BlockPlacementData
				{
					Value = Terrain.MakeBlockValue(this.m_attachedSignBlockIndex, 0, data),
					CellFace = raycastResult.CellFace
				};
			}
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = float.MinValue;
			int direction = 0;
			for (int i = 0; i < 8; i++)
			{
				float num2 = Vector3.Dot(forward, this.m_directions[i]);
				if (num2 > num)
				{
					num = num2;
					direction = i;
				}
			}
			int data2 = PostedSignBlock.SetHanging(PostedSignBlock.SetDirection(PostedSignBlock.SetColor(0, color), direction), raycastResult.CellFace.Face == 5);
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, data2),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x00017F34 File Offset: 0x00016134
		public override BlockMesh GetSignSurfaceBlockMesh(int data)
		{
			return this.m_surfaceMeshes[PostedSignBlock.GetVariant(data)];
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x00017F43 File Offset: 0x00016143
		public override Vector3 GetSignSurfaceNormal(int data)
		{
			return this.m_surfaceNormals[PostedSignBlock.GetVariant(data)];
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x00017F58 File Offset: 0x00016158
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			return new SignElectricElement(subsystemElectricity, new CellFace(x, y, z, PostedSignBlock.GetHanging(data) ? 5 : 4));
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x00017F88 File Offset: 0x00016188
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (PostedSignBlock.GetHanging(Terrain.ExtractData(value)))
			{
				if (face != 5 || SubsystemElectricity.GetConnectorDirection(face, 0, connectorFace) == null)
				{
					return null;
				}
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			else
			{
				if (face != 4 || SubsystemElectricity.GetConnectorDirection(face, 0, connectorFace) == null)
				{
					return null;
				}
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00017FF1 File Offset: 0x000161F1
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00017FF8 File Offset: 0x000161F8
		public static int GetDirection(int data)
		{
			return data & 7;
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00017FFD File Offset: 0x000161FD
		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00018007 File Offset: 0x00016207
		public static bool GetHanging(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x0001800F File Offset: 0x0001620F
		public static int SetHanging(int data, bool hanging)
		{
			if (!hanging)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0001801C File Offset: 0x0001621C
		public static int GetVariant(int data)
		{
			return data & 15;
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00018022 File Offset: 0x00016222
		public static int SetVariant(int data, int variant)
		{
			return (data & -16) | (variant & 15);
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00018030 File Offset: 0x00016230
		public static int? GetColor(int data)
		{
			if ((data & 16) != 0)
			{
				return new int?(data >> 5 & 15);
			}
			return null;
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x00018058 File Offset: 0x00016258
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -497) | 16 | (color.Value & 15) << 5;
			}
			return data & -497;
		}

		// Token: 0x040001C9 RID: 457
		public string m_modelName;

		// Token: 0x040001CA RID: 458
		public int m_coloredTextureSlot;

		// Token: 0x040001CB RID: 459
		public int m_attachedSignBlockIndex;

		// Token: 0x040001CC RID: 460
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040001CD RID: 461
		public BlockMesh m_standaloneColoredBlockMesh = new BlockMesh();

		// Token: 0x040001CE RID: 462
		public BlockMesh[] m_blockMeshes = new BlockMesh[16];

		// Token: 0x040001CF RID: 463
		public BlockMesh[] m_coloredBlockMeshes = new BlockMesh[16];

		// Token: 0x040001D0 RID: 464
		public BlockMesh[] m_surfaceMeshes = new BlockMesh[16];

		// Token: 0x040001D1 RID: 465
		public Vector3[] m_surfaceNormals = new Vector3[16];

		// Token: 0x040001D2 RID: 466
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[16][];

		// Token: 0x040001D3 RID: 467
		public Vector3[] m_directions = new Vector3[16];
	}
}
