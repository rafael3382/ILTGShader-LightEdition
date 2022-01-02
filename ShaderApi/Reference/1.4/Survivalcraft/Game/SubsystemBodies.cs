﻿using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;

namespace Game
{
	// Token: 0x0200018C RID: 396
	public class SubsystemBodies : Subsystem, IUpdateable
	{
		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000921 RID: 2337 RVA: 0x00039174 File Offset: 0x00037374
		public Dictionary<ComponentBody, Point2>.KeyCollection Bodies
		{
			get
			{
				return this.m_areaByComponentBody.Keys;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000922 RID: 2338 RVA: 0x00039181 File Offset: 0x00037381
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x00039184 File Offset: 0x00037384
		public void FindBodiesAroundPoint(Vector2 point, float radius, DynamicArray<ComponentBody> result)
		{
			int num = (int)MathUtils.Floor((point.X - radius) / 8f);
			int num2 = (int)MathUtils.Floor((point.Y - radius) / 8f);
			int num3 = (int)MathUtils.Floor((point.X + radius) / 8f);
			int num4 = (int)MathUtils.Floor((point.Y + radius) / 8f);
			for (int i = num; i <= num3; i++)
			{
				for (int j = num2; j <= num4; j++)
				{
					DynamicArray<ComponentBody> dynamicArray;
					if (this.m_componentBodiesByArea.TryGetValue(new Point2(i, j), out dynamicArray))
					{
						for (int k = 0; k < dynamicArray.Count; k++)
						{
							result.Add(dynamicArray.Array[k]);
						}
					}
				}
			}
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x0003923C File Offset: 0x0003743C
		public void FindBodiesInArea(Vector2 corner1, Vector2 corner2, DynamicArray<ComponentBody> result)
		{
			Point2 point = new Point2((int)MathUtils.Floor(corner1.X / 8f), (int)MathUtils.Floor(corner1.Y / 8f));
			Point2 point2 = new Point2((int)MathUtils.Floor(corner2.X / 8f), (int)MathUtils.Floor(corner2.Y / 8f));
			int num = MathUtils.Min(point.X, point2.X) - 1;
			int num2 = MathUtils.Min(point.Y, point2.Y) - 1;
			int num3 = MathUtils.Max(point.X, point2.X) + 1;
			int num4 = MathUtils.Max(point.Y, point2.Y) + 1;
			for (int i = num; i <= num3; i++)
			{
				for (int j = num2; j <= num4; j++)
				{
					DynamicArray<ComponentBody> dynamicArray;
					if (this.m_componentBodiesByArea.TryGetValue(new Point2(i, j), out dynamicArray))
					{
						for (int k = 0; k < dynamicArray.Count; k++)
						{
							result.Add(dynamicArray.Array[k]);
						}
					}
				}
			}
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x0003934C File Offset: 0x0003754C
		public BodyRaycastResult? Raycast(Vector3 start, Vector3 end, float inflateAmount, Func<ComponentBody, float, bool> action)
		{
			float num = Vector3.Distance(start, end);
			Ray3 ray = new Ray3(start, (num > 0f) ? ((end - start) / num) : Vector3.UnitX);
			Vector2 corner = new Vector2(start.X, start.Z);
			Vector2 corner2 = new Vector2(end.X, end.Z);
			BodyRaycastResult bodyRaycastResult = new BodyRaycastResult
			{
				Ray = ray,
				Distance = float.MaxValue
			};
			this.m_componentBodies.Clear();
			this.FindBodiesInArea(corner, corner2, this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentBody componentBody = this.m_componentBodies.Array[i];
				float? num2;
				if (inflateAmount > 0f)
				{
					BoundingBox boundingBox = componentBody.BoundingBox;
					boundingBox.Min -= new Vector3(inflateAmount);
					boundingBox.Max += new Vector3(inflateAmount);
					num2 = ray.Intersection(boundingBox);
				}
				else
				{
					num2 = ray.Intersection(componentBody.BoundingBox);
				}
				if (num2 != null && num2.Value <= num && num2.Value < bodyRaycastResult.Distance && action(componentBody, num2.Value))
				{
					bodyRaycastResult.Distance = num2.Value;
					bodyRaycastResult.ComponentBody = componentBody;
				}
			}
			if (bodyRaycastResult.ComponentBody == null)
			{
				return null;
			}
			return new BodyRaycastResult?(bodyRaycastResult);
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x000394E4 File Offset: 0x000376E4
		public override void OnEntityAdded(Entity entity)
		{
			foreach (ComponentBody componentBody in entity.FindComponents<ComponentBody>())
			{
				this.AddBody(componentBody);
			}
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x0003953C File Offset: 0x0003773C
		public override void OnEntityRemoved(Entity entity)
		{
			foreach (ComponentBody componentBody in entity.FindComponents<ComponentBody>())
			{
				this.RemoveBody(componentBody);
			}
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x00039594 File Offset: 0x00037794
		public void Update(float dt)
		{
			foreach (ComponentBody componentBody in this.Bodies)
			{
				this.UpdateBody(componentBody);
			}
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x000395E8 File Offset: 0x000377E8
		public void AddBody(ComponentBody componentBody)
		{
			Vector3 position = componentBody.Position;
			Point2 point = new Point2((int)MathUtils.Floor(position.X / 8f), (int)MathUtils.Floor(position.Z / 8f));
			this.m_areaByComponentBody.Add(componentBody, point);
			DynamicArray<ComponentBody> dynamicArray;
			if (!this.m_componentBodiesByArea.TryGetValue(point, out dynamicArray))
			{
				dynamicArray = new DynamicArray<ComponentBody>();
				this.m_componentBodiesByArea.Add(point, dynamicArray);
			}
			dynamicArray.Add(componentBody);
			componentBody.PositionChanged = (Action<ComponentFrame>)Delegate.Combine(componentBody.PositionChanged, new Action<ComponentFrame>(this.ComponentBody_PositionChanged));
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x00039680 File Offset: 0x00037880
		public void RemoveBody(ComponentBody componentBody)
		{
			Point2 key = this.m_areaByComponentBody[componentBody];
			this.m_areaByComponentBody.Remove(componentBody);
			this.m_componentBodiesByArea[key].Remove(componentBody);
			componentBody.PositionChanged = (Action<ComponentFrame>)Delegate.Remove(componentBody.PositionChanged, new Action<ComponentFrame>(this.ComponentBody_PositionChanged));
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x000396DC File Offset: 0x000378DC
		public void UpdateBody(ComponentBody componentBody)
		{
			Vector3 position = componentBody.Position;
			Point2 point = new Point2((int)MathUtils.Floor(position.X / 8f), (int)MathUtils.Floor(position.Z / 8f));
			Point2 point2 = this.m_areaByComponentBody[componentBody];
			if (point != point2)
			{
				this.m_areaByComponentBody[componentBody] = point;
				this.m_componentBodiesByArea[point2].Remove(componentBody);
				DynamicArray<ComponentBody> dynamicArray;
				if (!this.m_componentBodiesByArea.TryGetValue(point, out dynamicArray))
				{
					dynamicArray = new DynamicArray<ComponentBody>();
					this.m_componentBodiesByArea.Add(point, dynamicArray);
				}
				dynamicArray.Add(componentBody);
			}
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x0003977B File Offset: 0x0003797B
		public void ComponentBody_PositionChanged(ComponentFrame componentFrame)
		{
			this.UpdateBody((ComponentBody)componentFrame);
		}

		// Token: 0x04000495 RID: 1173
		public const float m_areaSize = 8f;

		// Token: 0x04000496 RID: 1174
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x04000497 RID: 1175
		public Dictionary<ComponentBody, Point2> m_areaByComponentBody = new Dictionary<ComponentBody, Point2>();

		// Token: 0x04000498 RID: 1176
		public Dictionary<Point2, DynamicArray<ComponentBody>> m_componentBodiesByArea = new Dictionary<Point2, DynamicArray<ComponentBody>>();
	}
}
