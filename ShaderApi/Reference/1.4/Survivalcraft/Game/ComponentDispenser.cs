using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FE RID: 510
	public class ComponentDispenser : ComponentInventoryBase
	{
		// Token: 0x06000ECF RID: 3791 RVA: 0x0006C210 File Offset: 0x0006A410
		public virtual void Dispense()
		{
			Point3 coordinates = this.m_componentBlockEntity.Coordinates;
			int data = Terrain.ExtractData(this.m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
			int direction = DispenserBlock.GetDirection(data);
			DispenserBlock.Mode mode = DispenserBlock.GetMode(data);
			for (int i = 0; i < this.SlotsCount; i++)
			{
				int slotValue = this.GetSlotValue(i);
				int slotCount = this.GetSlotCount(i);
				if (slotValue != 0 && slotCount > 0)
				{
					int num = this.RemoveSlotItems(i, 1);
					for (int j = 0; j < num; j++)
					{
						this.DispenseItem(coordinates, direction, slotValue, mode);
					}
					return;
				}
			}
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x0006C2B0 File Offset: 0x0006A4B0
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_componentBlockEntity = base.Entity.FindComponent<ComponentBlockEntity>(true);
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x0006C320 File Offset: 0x0006A520
		public virtual void DispenseItem(Point3 point, int face, int value, DispenserBlock.Mode mode)
		{
			Vector3 vector = CellFace.FaceToVector3(face);
			Vector3 vector2 = new Vector3((float)point.X + 0.5f, (float)point.Y + 0.5f, (float)point.Z + 0.5f) + 0.6f * vector;
			if (mode == DispenserBlock.Mode.Dispense)
			{
				float s = 1.8f;
				this.m_subsystemPickables.AddPickable(value, 1, vector2, new Vector3?(s * (vector + this.m_random.Vector3(0.2f))), null);
				this.m_subsystemAudio.PlaySound("Audio/DispenserDispense", 1f, 0f, new Vector3(vector2.X, vector2.Y, vector2.Z), 3f, true);
				return;
			}
			float s2 = this.m_random.Float(39f, 41f);
			if (this.m_subsystemProjectiles.FireProjectile(value, vector2, s2 * (vector + this.m_random.Vector3(0.025f) + new Vector3(0f, 0.05f, 0f)), Vector3.Zero, null) != null)
			{
				this.m_subsystemAudio.PlaySound("Audio/DispenserShoot", 1f, 0f, new Vector3(vector2.X, vector2.Y, vector2.Z), 4f, true);
				return;
			}
			this.DispenseItem(point, face, value, DispenserBlock.Mode.Dispense);
		}

		// Token: 0x04000822 RID: 2082
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000823 RID: 2083
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000824 RID: 2084
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x04000825 RID: 2085
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x04000826 RID: 2086
		public ComponentBlockEntity m_componentBlockEntity;
	}
}
