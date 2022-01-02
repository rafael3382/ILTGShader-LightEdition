using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200018B RID: 395
	public class SubsystemBoatBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600091C RID: 2332 RVA: 0x00038F81 File Offset: 0x00037181
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					178
				};
			}
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x00038F94 File Offset: 0x00037194
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			IInventory inventory = componentMiner.Inventory;
			if (Terrain.ExtractContents(componentMiner.ActiveBlockValue) == 178)
			{
				TerrainRaycastResult? terrainRaycastResult = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Digging, true, true, true);
				if (terrainRaycastResult != null)
				{
					Vector3 vector = terrainRaycastResult.Value.HitPoint(0f);
					DynamicArray<ComponentBody> dynamicArray = new DynamicArray<ComponentBody>();
					this.m_subsystemBodies.FindBodiesInArea(new Vector2(vector.X, vector.Z) - new Vector2(8f), new Vector2(vector.X, vector.Z) + new Vector2(8f), dynamicArray);
					if (dynamicArray.Count((ComponentBody b) => b.Entity.ValuesDictionary.DatabaseObject.Name == "Boat") < 6)
					{
						Entity entity = DatabaseManager.CreateEntity(base.Project, "Boat", true);
						entity.FindComponent<ComponentFrame>(true).Position = vector;
						entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.m_random.Float(0f, 6.28318548f));
						entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
						base.Project.AddEntity(entity);
						componentMiner.RemoveActiveTool(1);
						this.m_subsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, vector, 3f, true);
					}
					else
					{
						ComponentPlayer componentPlayer = componentMiner.ComponentPlayer;
						if (componentPlayer != null)
						{
							componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemBoatBlockBehavior.fName, 1), Color.White, true, false);
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x00039128 File Offset: 0x00037328
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
		}

		// Token: 0x04000491 RID: 1169
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000492 RID: 1170
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000493 RID: 1171
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000494 RID: 1172
		public static string fName = "SubsystemBoatBlockBehavior";
	}
}
