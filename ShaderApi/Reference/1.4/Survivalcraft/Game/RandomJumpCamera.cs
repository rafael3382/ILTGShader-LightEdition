using System;
using Engine;

namespace Game
{
	// Token: 0x020002F3 RID: 755
	public class RandomJumpCamera : BasePerspectiveCamera
	{
		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06001692 RID: 5778 RVA: 0x000A9B0E File Offset: 0x000A7D0E
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06001693 RID: 5779 RVA: 0x000A9B11 File Offset: 0x000A7D11
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001694 RID: 5780 RVA: 0x000A9B14 File Offset: 0x000A7D14
		public RandomJumpCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x06001695 RID: 5781 RVA: 0x000A9B33 File Offset: 0x000A7D33
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x000A9B50 File Offset: 0x000A7D50
		public override void Update(float dt)
		{
			if (this.m_random.Float(0f, 1f) < 0.1f * dt)
			{
				this.m_frequency = this.m_random.Float(0.33f, 5f) * 0.5f;
			}
			if (this.m_random.Float(0f, 1f) < this.m_frequency * dt)
			{
				SubsystemPlayers subsystemPlayers = base.GameWidget.SubsystemGameWidgets.Project.FindSubsystem<SubsystemPlayers>(true);
				if (subsystemPlayers.PlayersData.Count > 0)
				{
					Vector3 spawnPosition = subsystemPlayers.PlayersData[0].SpawnPosition;
					spawnPosition.X += this.m_random.Float(-150f, 150f);
					spawnPosition.Y = this.m_random.Float(70f, 120f);
					spawnPosition.Z += this.m_random.Float(-150f, 150f);
					Vector3 direction = this.m_random.Vector3(1f);
					base.SetupPerspectiveCamera(spawnPosition, direction, Vector3.UnitY);
				}
			}
			if (this.m_random.Float(0f, 1f) < 0.5f * this.m_frequency * dt)
			{
				base.GameWidget.SubsystemGameWidgets.Project.FindSubsystem<SubsystemTimeOfDay>(true).TimeOfDayOffset = (double)this.m_random.Float(0f, 1f);
			}
			if (this.m_random.Float(0f, 1f) < 1f * dt * 0.5f)
			{
				GameManager.SaveProject(false, false);
			}
		}

		// Token: 0x04000F44 RID: 3908
		public const float frequencyFactor = 0.5f;

		// Token: 0x04000F45 RID: 3909
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000F46 RID: 3910
		public float m_frequency = 0.5f;
	}
}
