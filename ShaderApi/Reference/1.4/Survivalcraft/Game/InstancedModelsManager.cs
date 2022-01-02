using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000144 RID: 324
	public static class InstancedModelsManager
	{
		// Token: 0x06000658 RID: 1624 RVA: 0x0002412B File Offset: 0x0002232B
		static InstancedModelsManager()
		{
			Display.DeviceReset += delegate()
			{
				foreach (InstancedModelData instancedModelData in InstancedModelsManager.m_cache.Values)
				{
					instancedModelData.VertexBuffer.Dispose();
					instancedModelData.IndexBuffer.Dispose();
				}
				InstancedModelsManager.m_cache.Clear();
			};
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x0002414C File Offset: 0x0002234C
		public static InstancedModelData GetInstancedModelData(Model model, int[] meshDrawOrders)
		{
			InstancedModelData instancedModelData;
			if (!InstancedModelsManager.m_cache.TryGetValue(model, out instancedModelData))
			{
				instancedModelData = InstancedModelsManager.CreateInstancedModelData(model, meshDrawOrders);
				InstancedModelsManager.m_cache.Add(model, instancedModelData);
			}
			return instancedModelData;
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00024180 File Offset: 0x00022380
		public static InstancedModelData CreateInstancedModelData(Model model, int[] meshDrawOrders)
		{
			DynamicArray<InstancedModelsManager.InstancedVertex> dynamicArray = new DynamicArray<InstancedModelsManager.InstancedVertex>();
			DynamicArray<ushort> dynamicArray2 = new DynamicArray<ushort>();
			for (int i = 0; i < meshDrawOrders.Length; i++)
			{
				ModelMesh modelMesh = model.Meshes[meshDrawOrders[i]];
				foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
				{
					int count = dynamicArray.Count;
					VertexBuffer vertexBuffer = modelMeshPart.VertexBuffer;
					IndexBuffer indexBuffer = modelMeshPart.IndexBuffer;
					ReadOnlyList<VertexElement> vertexElements = vertexBuffer.VertexDeclaration.VertexElements;
					ushort[] indexData = BlockMesh.GetIndexData<ushort>(indexBuffer);
					Dictionary<ushort, ushort> dictionary = new Dictionary<ushort, ushort>();
					if (vertexElements.Count != 3 || vertexElements[0].Offset != 0 || !(vertexElements[0].Semantic == VertexElementSemantic.Position.GetSemanticString()) || vertexElements[1].Offset != 12 || !(vertexElements[1].Semantic == VertexElementSemantic.Normal.GetSemanticString()) || vertexElements[2].Offset != 24 || !(vertexElements[2].Semantic == VertexElementSemantic.TextureCoordinate.GetSemanticString()))
					{
						throw new InvalidOperationException("Unsupported vertex format.");
					}
					InstancedModelsManager.SourceModelVertex[] vertexData = BlockMesh.GetVertexData<InstancedModelsManager.SourceModelVertex>(vertexBuffer);
					for (int j = modelMeshPart.StartIndex; j < modelMeshPart.StartIndex + modelMeshPart.IndicesCount; j++)
					{
						ushort num = indexData[j];
						if (!dictionary.ContainsKey(num))
						{
							dictionary.Add(num, (ushort)dynamicArray.Count);
							InstancedModelsManager.InstancedVertex item = default(InstancedModelsManager.InstancedVertex);
							InstancedModelsManager.SourceModelVertex sourceModelVertex = vertexData[(int)num];
							item.X = sourceModelVertex.X;
							item.Y = sourceModelVertex.Y;
							item.Z = sourceModelVertex.Z;
							item.Nx = sourceModelVertex.Nx;
							item.Ny = sourceModelVertex.Ny;
							item.Nz = sourceModelVertex.Nz;
							item.Tx = sourceModelVertex.Tx;
							item.Ty = sourceModelVertex.Ty;
							item.Instance = (float)modelMesh.ParentBone.Index;
							dynamicArray.Add(item);
						}
					}
					for (int k = 0; k < modelMeshPart.IndicesCount / 3; k++)
					{
						dynamicArray2.Add(dictionary[indexData[modelMeshPart.StartIndex + 3 * k]]);
						dynamicArray2.Add(dictionary[indexData[modelMeshPart.StartIndex + 3 * k + 1]]);
						dynamicArray2.Add(dictionary[indexData[modelMeshPart.StartIndex + 3 * k + 2]]);
					}
				}
			}
			InstancedModelData instancedModelData = new InstancedModelData();
			instancedModelData.VertexBuffer = new VertexBuffer(InstancedModelData.VertexDeclaration, dynamicArray.Count);
			instancedModelData.IndexBuffer = new IndexBuffer(IndexFormat.SixteenBits, dynamicArray2.Count);
			instancedModelData.VertexBuffer.SetData<InstancedModelsManager.InstancedVertex>(dynamicArray.Array, 0, dynamicArray.Count, 0);
			instancedModelData.IndexBuffer.SetData<ushort>(dynamicArray2.Array, 0, dynamicArray2.Count, 0);
			return instancedModelData;
		}

		// Token: 0x040002BD RID: 701
		public static Dictionary<Model, InstancedModelData> m_cache = new Dictionary<Model, InstancedModelData>();

		// Token: 0x02000430 RID: 1072
		public struct SourceModelVertex
		{
			// Token: 0x0400157E RID: 5502
			public float X;

			// Token: 0x0400157F RID: 5503
			public float Y;

			// Token: 0x04001580 RID: 5504
			public float Z;

			// Token: 0x04001581 RID: 5505
			public float Nx;

			// Token: 0x04001582 RID: 5506
			public float Ny;

			// Token: 0x04001583 RID: 5507
			public float Nz;

			// Token: 0x04001584 RID: 5508
			public float Tx;

			// Token: 0x04001585 RID: 5509
			public float Ty;
		}

		// Token: 0x02000431 RID: 1073
		public struct InstancedVertex
		{
			// Token: 0x04001586 RID: 5510
			public float X;

			// Token: 0x04001587 RID: 5511
			public float Y;

			// Token: 0x04001588 RID: 5512
			public float Z;

			// Token: 0x04001589 RID: 5513
			public float Nx;

			// Token: 0x0400158A RID: 5514
			public float Ny;

			// Token: 0x0400158B RID: 5515
			public float Nz;

			// Token: 0x0400158C RID: 5516
			public float Tx;

			// Token: 0x0400158D RID: 5517
			public float Ty;

			// Token: 0x0400158E RID: 5518
			public float Instance;
		}
	}
}
