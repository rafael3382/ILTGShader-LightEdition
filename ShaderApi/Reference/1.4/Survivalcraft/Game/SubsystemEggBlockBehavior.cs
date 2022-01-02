using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200019E RID: 414
	public class SubsystemEggBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000A37 RID: 2615 RVA: 0x00041C04 File Offset: 0x0003FE04
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x00041C0C File Offset: 0x0003FE0C
		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			int data = Terrain.ExtractData(worldItem.Value);
			bool isCooked = EggBlock.GetIsCooked(data);
			bool isLaid = EggBlock.GetIsLaid(data);
			if (!isCooked && (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || this.m_random.Float(0f, 1f) <= (isLaid ? 0.15f : 1f)))
			{
				if (this.m_subsystemCreatureSpawn.Creatures.Count < 35)
				{
					EggBlock.EggType eggType = this.m_eggBlock.GetEggType(data);
					Entity entity = DatabaseManager.CreateEntity(base.Project, eggType.TemplateName, true);
					entity.FindComponent<ComponentBody>(true).Position = worldItem.Position;
					entity.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.m_random.Float(0f, 6.28318548f));
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0.25f;
					base.Project.AddEntity(entity);
				}
				else
				{
					Projectile projectile = worldItem as Projectile;
					ComponentPlayer componentPlayer = ((projectile != null) ? projectile.Owner : null) as ComponentPlayer;
					if (componentPlayer != null)
					{
						componentPlayer.ComponentGui.DisplaySmallMessage("Too many creatures", Color.White, true, false);
					}
				}
			}
			return true;
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x00041D36 File Offset: 0x0003FF36
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemCreatureSpawn = base.Project.FindSubsystem<SubsystemCreatureSpawn>(true);
		}

		// Token: 0x040004EA RID: 1258
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040004EB RID: 1259
		public SubsystemCreatureSpawn m_subsystemCreatureSpawn;

		// Token: 0x040004EC RID: 1260
		public EggBlock m_eggBlock = (EggBlock)BlocksManager.Blocks[118];

		// Token: 0x040004ED RID: 1261
		public Game.Random m_random = new Game.Random();
	}
}
