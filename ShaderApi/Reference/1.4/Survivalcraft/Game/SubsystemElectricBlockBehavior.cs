using System;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200019F RID: 415
	public class SubsystemElectricBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000A3B RID: 2619 RVA: 0x00041D89 File Offset: 0x0003FF89
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					133,
					140,
					137,
					143,
					156,
					134,
					135,
					145,
					224,
					146,
					157,
					180,
					181,
					183,
					138,
					139,
					141,
					142,
					184,
					187,
					186,
					188,
					144,
					151,
					179,
					152,
					254,
					253,
					182,
					185,
					56,
					57,
					58,
					83,
					84,
					166,
					194,
					86,
					63,
					97,
					98,
					210,
					211,
					105,
					106,
					107,
					234,
					235,
					236,
					147,
					153,
					154,
					223,
					155,
					243,
					120,
					121,
					199,
					216,
					227,
					237
				};
			}
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x00041D9D File Offset: 0x0003FF9D
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.m_subsystemElectricity.OnElectricElementBlockGenerated(x, y, z);
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x00041DAE File Offset: 0x0003FFAE
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.m_subsystemElectricity.OnElectricElementBlockAdded(x, y, z);
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x00041DC0 File Offset: 0x0003FFC0
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.m_subsystemElectricity.OnElectricElementBlockRemoved(x, y, z);
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x00041DD2 File Offset: 0x0003FFD2
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			this.m_subsystemElectricity.OnElectricElementBlockModified(x, y, z);
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x00041DE4 File Offset: 0x0003FFE4
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			this.m_subsystemElectricity.OnChunkDiscarding(chunk);
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00041DF4 File Offset: 0x0003FFF4
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			for (int i = 0; i < 6; i++)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, i);
				if (electricElement != null)
				{
					electricElement.OnNeighborBlockChanged(new CellFace(x, y, z, i), neighborX, neighborY, neighborZ);
				}
			}
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x00041E38 File Offset: 0x00040038
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int x = raycastResult.CellFace.X;
			int y = raycastResult.CellFace.Y;
			int z = raycastResult.CellFace.Z;
			for (int i = 0; i < 6; i++)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, i);
				if (electricElement != null)
				{
					return electricElement.OnInteract(raycastResult, componentMiner);
				}
			}
			return false;
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x00041E98 File Offset: 0x00040098
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			int x = cellFace.X;
			int y = cellFace.Y;
			int z = cellFace.Z;
			for (int i = 0; i < 6; i++)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, i);
				if (electricElement != null)
				{
					electricElement.OnCollide(cellFace, velocity, componentBody);
					return;
				}
			}
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x00041EE8 File Offset: 0x000400E8
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			int x = cellFace.X;
			int y = cellFace.Y;
			int z = cellFace.Z;
			for (int i = 0; i < 6; i++)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, i);
				if (electricElement != null)
				{
					electricElement.OnHitByProjectile(cellFace, worldItem);
					return;
				}
			}
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00041F35 File Offset: 0x00040135
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemElectricity = base.Project.FindSubsystem<SubsystemElectricity>(true);
		}

		// Token: 0x040004EE RID: 1262
		public SubsystemElectricity m_subsystemElectricity;
	}
}
