using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000186 RID: 390
	public abstract class SubsystemBlockBehavior : Subsystem
	{
		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060008EB RID: 2283
		public abstract int[] HandledBlocks { get; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060008EC RID: 2284 RVA: 0x000388FB File Offset: 0x00036AFB
		// (set) Token: 0x060008ED RID: 2285 RVA: 0x00038903 File Offset: 0x00036B03
		public SubsystemTerrain SubsystemTerrain { get; set; }

		// Token: 0x060008EE RID: 2286 RVA: 0x0003890C File Offset: 0x00036B0C
		public virtual void OnChunkInitialized(TerrainChunk chunk)
		{
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x0003890E File Offset: 0x00036B0E
		public virtual void OnChunkDiscarding(TerrainChunk chunk)
		{
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x00038910 File Offset: 0x00036B10
		public virtual void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x00038912 File Offset: 0x00036B12
		public virtual void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x00038914 File Offset: 0x00036B14
		public virtual void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x00038916 File Offset: 0x00036B16
		public virtual void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
		}

		// Token: 0x060008F4 RID: 2292 RVA: 0x00038918 File Offset: 0x00036B18
		public virtual void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x0003891A File Offset: 0x00036B1A
		public virtual bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			return false;
		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x0003891D File Offset: 0x00036B1D
		public virtual bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			return false;
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x00038920 File Offset: 0x00036B20
		public virtual bool OnAim(Ray3 aim, ComponentMiner componentMiner, AimState state)
		{
			return false;
		}

		// Token: 0x060008F8 RID: 2296 RVA: 0x00038923 File Offset: 0x00036B23
		public virtual bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			return false;
		}

		// Token: 0x060008F9 RID: 2297 RVA: 0x00038926 File Offset: 0x00036B26
		public virtual bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			return false;
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x00038929 File Offset: 0x00036B29
		public virtual void OnItemPlaced(int x, int y, int z, ref BlockPlacementData placementData, int itemValue)
		{
		}

		// Token: 0x060008FB RID: 2299 RVA: 0x0003892B File Offset: 0x00036B2B
		public virtual void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x0003892D File Offset: 0x00036B2D
		public virtual void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x0003892F File Offset: 0x00036B2F
		public virtual void OnExplosion(int value, int x, int y, int z, float damage)
		{
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x00038931 File Offset: 0x00036B31
		public virtual void OnFiredAsProjectile(Projectile projectile)
		{
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x00038933 File Offset: 0x00036B33
		public virtual bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			return false;
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x00038936 File Offset: 0x00036B36
		public virtual void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x00038938 File Offset: 0x00036B38
		public virtual int GetProcessInventoryItemCapacity(IInventory inventory, int slotIndex, int value)
		{
			return 0;
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x0003893B File Offset: 0x00036B3B
		public virtual void ProcessInventoryItem(IInventory inventory, int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			throw new InvalidOperationException("Cannot process items.");
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x00038947 File Offset: 0x00036B47
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.SubsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
		}
	}
}
