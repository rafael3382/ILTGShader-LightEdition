using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E7 RID: 487
	public class ComponentAutoJump : Component, IUpdateable
	{
		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000D5E RID: 3422 RVA: 0x00060EBD File Offset: 0x0005F0BD
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x00060EC0 File Offset: 0x0005F0C0
		public void Update(float dt)
		{
			if ((SettingsManager.AutoJump || this.m_alwaysEnabled) && this.m_subsystemTime.GameTime - this.m_lastAutoJumpTime > 0.25)
			{
				Vector2? lastWalkOrder = this.m_componentCreature.ComponentLocomotion.LastWalkOrder;
				if (lastWalkOrder != null)
				{
					Vector2 vector = new Vector2(this.m_componentCreature.ComponentBody.CollisionVelocityChange.X, this.m_componentCreature.ComponentBody.CollisionVelocityChange.Z);
					if (vector != Vector2.Zero && !this.m_collidedWithBody)
					{
						Vector2 v = Vector2.Normalize(vector);
						Vector3 vector2 = this.m_componentCreature.ComponentBody.Matrix.Right * lastWalkOrder.Value.X + this.m_componentCreature.ComponentBody.Matrix.Forward * lastWalkOrder.Value.Y;
						Vector2 v2 = Vector2.Normalize(new Vector2(vector2.X, vector2.Z));
						bool flag = false;
						Vector3 vector3 = Vector3.Zero;
						Vector3 vector4 = Vector3.Zero;
						Vector3 vector5 = Vector3.Zero;
						if (Vector2.Dot(v2, -v) > 0.6f)
						{
							if (Vector2.Dot(v2, Vector2.UnitX) > 0.6f)
							{
								vector3 = this.m_componentCreature.ComponentBody.Position + Vector3.UnitX;
								vector4 = vector3 - Vector3.UnitZ;
								vector5 = vector3 + Vector3.UnitZ;
								flag = true;
							}
							else if (Vector2.Dot(v2, -Vector2.UnitX) > 0.6f)
							{
								vector3 = this.m_componentCreature.ComponentBody.Position - Vector3.UnitX;
								vector4 = vector3 - Vector3.UnitZ;
								vector5 = vector3 + Vector3.UnitZ;
								flag = true;
							}
							else if (Vector2.Dot(v2, Vector2.UnitY) > 0.6f)
							{
								vector3 = this.m_componentCreature.ComponentBody.Position + Vector3.UnitZ;
								vector4 = vector3 - Vector3.UnitX;
								vector5 = vector3 + Vector3.UnitX;
								flag = true;
							}
							else if (Vector2.Dot(v2, -Vector2.UnitY) > 0.6f)
							{
								vector3 = this.m_componentCreature.ComponentBody.Position - Vector3.UnitZ;
								vector4 = vector3 - Vector3.UnitX;
								vector5 = vector3 + Vector3.UnitX;
								flag = true;
							}
						}
						if (flag)
						{
							int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector3.X), Terrain.ToCell(vector3.Y), Terrain.ToCell(vector3.Z));
							int cellValue2 = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector4.X), Terrain.ToCell(vector4.Y), Terrain.ToCell(vector4.Z));
							int cellValue3 = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector5.X), Terrain.ToCell(vector5.Y), Terrain.ToCell(vector5.Z));
							int cellValue4 = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector3.X), Terrain.ToCell(vector3.Y) + 1, Terrain.ToCell(vector3.Z));
							int cellValue5 = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector4.X), Terrain.ToCell(vector4.Y) + 1, Terrain.ToCell(vector4.Z));
							int cellValue6 = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector5.X), Terrain.ToCell(vector5.Y) + 1, Terrain.ToCell(vector5.Z));
							int num = Terrain.ExtractContents(cellValue);
							int num2 = Terrain.ExtractContents(cellValue2);
							int num3 = Terrain.ExtractContents(cellValue3);
							int num4 = Terrain.ExtractContents(cellValue4);
							int num5 = Terrain.ExtractContents(cellValue5);
							int num6 = Terrain.ExtractContents(cellValue6);
							Block block = BlocksManager.Blocks[num];
							Block block2 = BlocksManager.Blocks[num2];
							Block block3 = BlocksManager.Blocks[num3];
							Block block4 = BlocksManager.Blocks[num4];
							Block block5 = BlocksManager.Blocks[num5];
							Block block6 = BlocksManager.Blocks[num6];
							if (!block.NoAutoJump && ((block.IsCollidable_(cellValue) && !block4.IsCollidable_(cellValue4)) || (block2.IsCollidable_(cellValue2) && !block5.IsCollidable_(cellValue5)) || (block3.IsCollidable_(cellValue3) && !block6.IsCollidable_(cellValue6))))
							{
								this.m_componentCreature.ComponentLocomotion.JumpOrder = MathUtils.Max(this.m_jumpStrength, this.m_componentCreature.ComponentLocomotion.JumpOrder);
								this.m_lastAutoJumpTime = this.m_subsystemTime.GameTime;
							}
						}
					}
				}
			}
			this.m_collidedWithBody = false;
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x000613B0 File Offset: 0x0005F5B0
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_alwaysEnabled = valuesDictionary.GetValue<bool>("AlwaysEnabled");
			this.m_jumpStrength = valuesDictionary.GetValue<float>("JumpStrength");
			ComponentBody componentBody = this.m_componentCreature.ComponentBody;
			componentBody.CollidedWithBody = (Action<ComponentBody>)Delegate.Combine(componentBody.CollidedWithBody, new Action<ComponentBody>(delegate(ComponentBody <p0>)
			{
				this.m_collidedWithBody = true;
			}));
		}

		// Token: 0x040006E6 RID: 1766
		public SubsystemTime m_subsystemTime;

		// Token: 0x040006E7 RID: 1767
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040006E8 RID: 1768
		public ComponentCreature m_componentCreature;

		// Token: 0x040006E9 RID: 1769
		public double m_lastAutoJumpTime;

		// Token: 0x040006EA RID: 1770
		public bool m_alwaysEnabled;

		// Token: 0x040006EB RID: 1771
		public float m_jumpStrength;

		// Token: 0x040006EC RID: 1772
		public bool m_collidedWithBody;
	}
}
