using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D4 RID: 468
	public class SubsystemSoundMaterials : Subsystem
	{
		// Token: 0x06000C97 RID: 3223 RVA: 0x0005A8AC File Offset: 0x00058AAC
		public void PlayImpactSound(int value, Vector3 position, float loudnessMultiplier)
		{
			int num = Terrain.ExtractContents(value);
			string soundMaterialName = BlocksManager.Blocks[num].GetSoundMaterialName(this.m_subsystemTerrain, value);
			if (!string.IsNullOrEmpty(soundMaterialName))
			{
				string value2 = this.m_impactsSoundsValuesDictionary.GetValue<string>(soundMaterialName, null);
				if (!string.IsNullOrEmpty(value2))
				{
					float pitch = this.m_random.Float(-0.2f, 0.2f);
					this.m_subsystemAudio.PlayRandomSound(value2, 0.5f * loudnessMultiplier, pitch, position, 5f * loudnessMultiplier, true);
				}
			}
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x0005A928 File Offset: 0x00058B28
		public bool PlayFootstepSound(ComponentCreature componentCreature, float loudnessMultiplier)
		{
			string footstepSoundMaterialName = this.GetFootstepSoundMaterialName(componentCreature);
			if (!string.IsNullOrEmpty(footstepSoundMaterialName))
			{
				string value = componentCreature.ComponentCreatureSounds.ValuesDictionary.GetValue<ValuesDictionary>("CustomFootstepSounds").GetValue<string>(footstepSoundMaterialName, null);
				if (string.IsNullOrEmpty(value))
				{
					value = this.m_footstepSoundsValuesDictionary.GetValue<string>(footstepSoundMaterialName, null);
				}
				if (!string.IsNullOrEmpty(value))
				{
					float pitch = this.m_random.Float(-0.2f, 0.2f);
					this.m_subsystemAudio.PlayRandomSound(value, 0.75f * loudnessMultiplier, pitch, componentCreature.ComponentBody.Position, 2f * loudnessMultiplier, true);
					ComponentPlayer componentPlayer = componentCreature as ComponentPlayer;
					if (componentPlayer != null && componentPlayer.ComponentVitalStats.Wetness > 0f)
					{
						string value2 = this.m_footstepSoundsValuesDictionary.GetValue<string>("Squishy", null);
						if (!string.IsNullOrEmpty(value2))
						{
							float volume = 0.7f * loudnessMultiplier * MathUtils.Pow(componentPlayer.ComponentVitalStats.Wetness, 4f);
							this.m_subsystemAudio.PlayRandomSound(value2, volume, pitch, componentCreature.ComponentBody.Position, 2f * loudnessMultiplier, true);
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x0005AA40 File Offset: 0x00058C40
		public string GetFootstepSoundMaterialName(ComponentCreature componentCreature)
		{
			Vector3 position = componentCreature.ComponentBody.Position;
			if (componentCreature.ComponentBody.ImmersionDepth > 0.2f && componentCreature.ComponentBody.ImmersionFluidBlock is WaterBlock)
			{
				return "Water";
			}
			if (componentCreature.ComponentLocomotion.LadderValue != null)
			{
				if (Terrain.ExtractContents(componentCreature.ComponentLocomotion.LadderValue.Value) == 59)
				{
					return "WoodenLadder";
				}
				return "MetalLadder";
			}
			else
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(position.X), Terrain.ToCell(position.Y + 0.1f), Terrain.ToCell(position.Z));
				int num = Terrain.ExtractContents(cellValue);
				string soundMaterialName = BlocksManager.Blocks[num].GetSoundMaterialName(this.m_subsystemTerrain, cellValue);
				if (string.IsNullOrEmpty(soundMaterialName) && componentCreature.ComponentBody.StandingOnValue != null)
				{
					soundMaterialName = BlocksManager.Blocks[Terrain.ExtractContents(componentCreature.ComponentBody.StandingOnValue.Value)].GetSoundMaterialName(this.m_subsystemTerrain, componentCreature.ComponentBody.StandingOnValue.Value);
				}
				if (!string.IsNullOrEmpty(soundMaterialName))
				{
					return soundMaterialName;
				}
				return string.Empty;
			}
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x0005AB84 File Offset: 0x00058D84
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_impactsSoundsValuesDictionary = valuesDictionary.GetValue<ValuesDictionary>("ImpactSounds");
			this.m_footstepSoundsValuesDictionary = valuesDictionary.GetValue<ValuesDictionary>("FootstepSounds");
		}

		// Token: 0x0400066D RID: 1645
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400066E RID: 1646
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400066F RID: 1647
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000670 RID: 1648
		public ValuesDictionary m_impactsSoundsValuesDictionary;

		// Token: 0x04000671 RID: 1649
		public ValuesDictionary m_footstepSoundsValuesDictionary;
	}
}
