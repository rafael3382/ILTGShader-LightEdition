using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000283 RID: 643
	public abstract class ElectricElement
	{
		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06001460 RID: 5216 RVA: 0x00099065 File Offset: 0x00097265
		// (set) Token: 0x06001461 RID: 5217 RVA: 0x0009906D File Offset: 0x0009726D
		public SubsystemElectricity SubsystemElectricity { get; set; }

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06001462 RID: 5218 RVA: 0x00099076 File Offset: 0x00097276
		// (set) Token: 0x06001463 RID: 5219 RVA: 0x0009907E File Offset: 0x0009727E
		public ReadOnlyList<CellFace> CellFaces { get; set; }

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06001464 RID: 5220 RVA: 0x00099087 File Offset: 0x00097287
		// (set) Token: 0x06001465 RID: 5221 RVA: 0x0009908F File Offset: 0x0009728F
		public List<ElectricConnection> Connections { get; set; }

		// Token: 0x06001466 RID: 5222 RVA: 0x00099098 File Offset: 0x00097298
		public ElectricElement(SubsystemElectricity subsystemElectricity, IEnumerable<CellFace> cellFaces)
		{
			this.SubsystemElectricity = subsystemElectricity;
			this.CellFaces = new ReadOnlyList<CellFace>(new List<CellFace>(cellFaces));
			this.Connections = new List<ElectricConnection>();
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x000990C3 File Offset: 0x000972C3
		public ElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : this(subsystemElectricity, new List<CellFace>
		{
			cellFace
		})
		{
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x000990D8 File Offset: 0x000972D8
		public virtual float GetOutputVoltage(int face)
		{
			return 0f;
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x000990DF File Offset: 0x000972DF
		public virtual bool Simulate()
		{
			return false;
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x000990E2 File Offset: 0x000972E2
		public virtual void OnAdded()
		{
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x000990E4 File Offset: 0x000972E4
		public virtual void OnRemoved()
		{
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x000990E6 File Offset: 0x000972E6
		public virtual void OnNeighborBlockChanged(CellFace cellFace, int neighborX, int neighborY, int neighborZ)
		{
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x000990E8 File Offset: 0x000972E8
		public virtual bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			return false;
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x000990EB File Offset: 0x000972EB
		public virtual void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x000990ED File Offset: 0x000972ED
		public virtual void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x000990EF File Offset: 0x000972EF
		public virtual void OnConnectionsChanged()
		{
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x000990F1 File Offset: 0x000972F1
		public static bool IsSignalHigh(float voltage)
		{
			return voltage >= 0.5f;
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x00099100 File Offset: 0x00097300
		public int CalculateHighInputsCount()
		{
			int num = 0;
			foreach (ElectricConnection electricConnection in this.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input && ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace)))
				{
					num++;
				}
			}
			return num;
		}
	}
}
