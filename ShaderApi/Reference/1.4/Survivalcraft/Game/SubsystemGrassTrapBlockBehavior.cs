using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x020001AE RID: 430
	public class SubsystemGrassTrapBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000B17 RID: 2839 RVA: 0x0004ABB1 File Offset: 0x00048DB1
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					87
				};
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000B18 RID: 2840 RVA: 0x0004ABBE File Offset: 0x00048DBE
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0004ABC4 File Offset: 0x00048DC4
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			if (cellFace.Face == 4 && componentBody.Mass > 20f)
			{
				Point3 key = new Point3(cellFace.X, cellFace.Y, cellFace.Z);
				SubsystemGrassTrapBlockBehavior.TrapValue trapValue;
				if (!this.m_trapValues.TryGetValue(key, out trapValue))
				{
					trapValue = new SubsystemGrassTrapBlockBehavior.TrapValue();
					this.m_trapValues.Add(key, trapValue);
				}
				trapValue.Damage += 0f - velocity;
			}
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x0004AC38 File Offset: 0x00048E38
		public void Update(float dt)
		{
			foreach (KeyValuePair<Point3, SubsystemGrassTrapBlockBehavior.TrapValue> keyValuePair in this.m_trapValues)
			{
				if (keyValuePair.Value.Damage > 1f)
				{
					for (int i = -1; i <= 1; i++)
					{
						for (int j = -1; j <= 1; j++)
						{
							if (MathUtils.Abs(i) + MathUtils.Abs(j) <= 1 && base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X + i, keyValuePair.Key.Y, keyValuePair.Key.Z + j) == 87)
							{
								base.SubsystemTerrain.DestroyCell(0, keyValuePair.Key.X + i, keyValuePair.Key.Y, keyValuePair.Key.Z + j, 0, false, false);
							}
						}
					}
					keyValuePair.Value.Damage = 0f;
				}
				else
				{
					keyValuePair.Value.Damage -= 0.5f * dt;
				}
				if (keyValuePair.Value.Damage <= 0f)
				{
					this.m_toRemove.Add(keyValuePair.Key);
				}
			}
			foreach (Point3 key in this.m_toRemove)
			{
				this.m_trapValues.Remove(key);
			}
			this.m_toRemove.Clear();
		}

		// Token: 0x04000568 RID: 1384
		public Dictionary<Point3, SubsystemGrassTrapBlockBehavior.TrapValue> m_trapValues = new Dictionary<Point3, SubsystemGrassTrapBlockBehavior.TrapValue>();

		// Token: 0x04000569 RID: 1385
		public List<Point3> m_toRemove = new List<Point3>();

		// Token: 0x02000493 RID: 1171
		public class TrapValue
		{
			// Token: 0x040016C8 RID: 5832
			public float Damage;
		}
	}
}
