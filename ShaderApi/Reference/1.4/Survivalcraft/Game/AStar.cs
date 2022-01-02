using System;
using Engine;

namespace Game
{
	// Token: 0x02000240 RID: 576
	public class AStar<T>
	{
		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x060012B2 RID: 4786 RVA: 0x0008AF64 File Offset: 0x00089164
		// (set) Token: 0x060012B3 RID: 4787 RVA: 0x0008AF6C File Offset: 0x0008916C
		public float PathCost { get; set; }

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x060012B4 RID: 4788 RVA: 0x0008AF75 File Offset: 0x00089175
		// (set) Token: 0x060012B5 RID: 4789 RVA: 0x0008AF7D File Offset: 0x0008917D
		public DynamicArray<T> Path { get; set; }

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x060012B6 RID: 4790 RVA: 0x0008AF86 File Offset: 0x00089186
		// (set) Token: 0x060012B7 RID: 4791 RVA: 0x0008AF8E File Offset: 0x0008918E
		public IAStarWorld<T> World { get; set; }

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x060012B8 RID: 4792 RVA: 0x0008AF97 File Offset: 0x00089197
		// (set) Token: 0x060012B9 RID: 4793 RVA: 0x0008AF9F File Offset: 0x0008919F
		public IAStarStorage<T> OpenStorage { get; set; }

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x060012BA RID: 4794 RVA: 0x0008AFA8 File Offset: 0x000891A8
		// (set) Token: 0x060012BB RID: 4795 RVA: 0x0008AFB0 File Offset: 0x000891B0
		public IAStarStorage<T> ClosedStorage { get; set; }

		// Token: 0x060012BC RID: 4796 RVA: 0x0008AFBC File Offset: 0x000891BC
		public void BuildPathFromEndNode(AStar<T>.Node startNode, AStar<T>.Node endNode)
		{
			this.PathCost = endNode.G;
			this.Path.Clear();
			for (AStar<T>.Node node = endNode; node != startNode; node = (AStar<T>.Node)this.ClosedStorage.Get(node.PreviousPosition))
			{
				this.Path.Add(node.Position);
			}
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x0008B010 File Offset: 0x00089210
		public void FindPath(T start, T end, float minHeuristic, int maxPositionsToCheck)
		{
			if (this.Path == null)
			{
				throw new InvalidOperationException("Path not specified.");
			}
			if (this.World == null)
			{
				throw new InvalidOperationException("AStar World not specified.");
			}
			if (this.OpenStorage == null)
			{
				throw new InvalidOperationException("AStar OpenStorage not specified.");
			}
			if (this.OpenStorage == null)
			{
				throw new InvalidOperationException("AStar ClosedStorage not specified.");
			}
			this.m_nodesCacheIndex = 0;
			this.m_openHeap.Clear();
			this.OpenStorage.Clear();
			this.ClosedStorage.Clear();
			AStar<T>.Node node = this.NewNode(start, default(T), 0f, 0f);
			this.OpenStorage.Set(start, node);
			this.HeapEnqueue(node);
			AStar<T>.Node node2 = null;
			int num = 0;
			AStar<T>.Node node3;
			for (;;)
			{
				node3 = ((this.m_openHeap.Count > 0) ? this.HeapDequeue() : null);
				if (node3 == null || num >= maxPositionsToCheck)
				{
					break;
				}
				if (this.World.IsGoal(node3.Position))
				{
					goto IL_264;
				}
				this.ClosedStorage.Set(node3.Position, node3);
				this.OpenStorage.Set(node3.Position, null);
				num++;
				this.m_neighbors.Clear();
				this.World.Neighbors(node3.Position, this.m_neighbors);
				for (int i = 0; i < this.m_neighbors.Count; i++)
				{
					T t = this.m_neighbors.Array[i];
					if (this.ClosedStorage.Get(t) == null)
					{
						float num2 = this.World.Cost(node3.Position, t);
						if (num2 != float.PositiveInfinity)
						{
							float num3 = node3.G + num2;
							float num4 = this.World.Heuristic(t, end);
							if (node3 != node && (node2 == null || num4 < node2.H))
							{
								node2 = node3;
							}
							AStar<T>.Node node4 = (AStar<T>.Node)this.OpenStorage.Get(t);
							if (node4 != null)
							{
								if (num3 < node4.G)
								{
									node4.G = num3;
									node4.F = num3 + node4.H;
									node4.PreviousPosition = node3.Position;
									this.HeapUpdate(node4);
								}
							}
							else
							{
								node4 = this.NewNode(t, node3.Position, num3, num4);
								this.OpenStorage.Set(t, node4);
								this.HeapEnqueue(node4);
							}
						}
					}
				}
			}
			if (node2 != null)
			{
				this.BuildPathFromEndNode(node, node2);
				return;
			}
			this.Path.Clear();
			this.PathCost = 0f;
			return;
			IL_264:
			this.BuildPathFromEndNode(node, node3);
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x0008B289 File Offset: 0x00089489
		public void HeapEnqueue(AStar<T>.Node node)
		{
			this.m_openHeap.Add(node);
			this.HeapifyFromPosToStart(this.m_openHeap.Count - 1);
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x0008B2AC File Offset: 0x000894AC
		public AStar<T>.Node HeapDequeue()
		{
			AStar<T>.Node result = this.m_openHeap.Array[0];
			if (this.m_openHeap.Count <= 1)
			{
				this.m_openHeap.Clear();
				return result;
			}
			this.m_openHeap.Array[0] = this.m_openHeap.Array[this.m_openHeap.Count - 1];
			DynamicArray<AStar<T>.Node> openHeap = this.m_openHeap;
			int count = openHeap.Count - 1;
			openHeap.Count = count;
			this.HeapifyFromPosToEnd(0);
			return result;
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x0008B328 File Offset: 0x00089528
		public void HeapUpdate(AStar<T>.Node node)
		{
			int pos = -1;
			for (int i = 0; i < this.m_openHeap.Count; i++)
			{
				if (this.m_openHeap.Array[i] == node)
				{
					pos = i;
					break;
				}
			}
			this.HeapifyFromPosToStart(pos);
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x0008B368 File Offset: 0x00089568
		public void HeapifyFromPosToEnd(int pos)
		{
			for (;;)
			{
				int num = pos;
				int num2 = 2 * pos + 1;
				int num3 = 2 * pos + 2;
				if (num2 < this.m_openHeap.Count && this.m_openHeap.Array[num2].F < this.m_openHeap.Array[num].F)
				{
					num = num2;
				}
				if (num3 < this.m_openHeap.Count && this.m_openHeap.Array[num3].F < this.m_openHeap.Array[num].F)
				{
					num = num3;
				}
				if (num == pos)
				{
					break;
				}
				AStar<T>.Node node = this.m_openHeap.Array[num];
				this.m_openHeap.Array[num] = this.m_openHeap.Array[pos];
				this.m_openHeap.Array[pos] = node;
				pos = num;
			}
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x0008B434 File Offset: 0x00089634
		public void HeapifyFromPosToStart(int pos)
		{
			int num;
			for (int i = pos; i > 0; i = num)
			{
				num = (i - 1) / 2;
				AStar<T>.Node node = this.m_openHeap.Array[num];
				AStar<T>.Node node2 = this.m_openHeap.Array[i];
				if (node.F <= node2.F)
				{
					break;
				}
				this.m_openHeap.Array[num] = node2;
				this.m_openHeap.Array[i] = node;
			}
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x0008B498 File Offset: 0x00089698
		public AStar<T>.Node NewNode(T position, T previousPosition, float g, float h)
		{
			while (this.m_nodesCacheIndex >= this.m_nodesCache.Count)
			{
				this.m_nodesCache.Add(new AStar<T>.Node());
			}
			AStar<T>.Node[] array = this.m_nodesCache.Array;
			int nodesCacheIndex = this.m_nodesCacheIndex;
			this.m_nodesCacheIndex = nodesCacheIndex + 1;
			object obj = array[nodesCacheIndex];
			obj.Position = position;
			obj.PreviousPosition = previousPosition;
			obj.F = g + h;
			obj.G = g;
			obj.H = h;
			return obj;
		}

		// Token: 0x04000B81 RID: 2945
		public int m_nodesCacheIndex;

		// Token: 0x04000B82 RID: 2946
		public DynamicArray<AStar<T>.Node> m_nodesCache = new DynamicArray<AStar<T>.Node>();

		// Token: 0x04000B83 RID: 2947
		public DynamicArray<AStar<T>.Node> m_openHeap = new DynamicArray<AStar<T>.Node>();

		// Token: 0x04000B84 RID: 2948
		public DynamicArray<T> m_neighbors = new DynamicArray<T>();

		// Token: 0x020004F1 RID: 1265
		public class Node
		{
			// Token: 0x040017F7 RID: 6135
			public T Position;

			// Token: 0x040017F8 RID: 6136
			public T PreviousPosition;

			// Token: 0x040017F9 RID: 6137
			public float F;

			// Token: 0x040017FA RID: 6138
			public float G;

			// Token: 0x040017FB RID: 6139
			public float H;
		}
	}
}
