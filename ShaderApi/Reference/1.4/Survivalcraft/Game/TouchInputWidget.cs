using System;
using Engine;
using Engine.Input;

namespace Game
{
	// Token: 0x020003A4 RID: 932
	public class TouchInputWidget : Widget
	{
		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06001C64 RID: 7268 RVA: 0x000DC11D File Offset: 0x000DA31D
		// (set) Token: 0x06001C65 RID: 7269 RVA: 0x000DC125 File Offset: 0x000DA325
		public float Radius
		{
			get
			{
				return this.m_radius;
			}
			set
			{
				this.m_radius = MathUtils.Max(value, 1f);
			}
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x06001C66 RID: 7270 RVA: 0x000DC138 File Offset: 0x000DA338
		public TouchInput? TouchInput
		{
			get
			{
				if (base.IsEnabledGlobal && base.IsVisibleGlobal)
				{
					return this.m_touchInput;
				}
				return null;
			}
		}

		// Token: 0x06001C67 RID: 7271 RVA: 0x000DC168 File Offset: 0x000DA368
		public override void Update()
		{
			this.m_touchInput = null;
			double frameStartTime = Time.FrameStartTime;
			int frameIndex = Time.FrameIndex;
			foreach (TouchLocation touchLocation in base.Input.TouchLocations)
			{
				if (touchLocation.State == TouchLocationState.Pressed)
				{
					if (base.HitTestGlobal(touchLocation.Position, null) == this)
					{
						this.m_touchId = new int?(touchLocation.Id);
						this.m_touchLastPosition = touchLocation.Position;
						this.m_touchOrigin = touchLocation.Position;
						this.m_touchOriginLimited = touchLocation.Position;
						this.m_touchTime = frameStartTime;
						this.m_touchFrameIndex = frameIndex;
						this.m_touchMoved = false;
					}
				}
				else if (touchLocation.State == TouchLocationState.Moved)
				{
					if (this.m_touchId != null && touchLocation.Id == this.m_touchId.Value)
					{
						this.m_touchMoved |= (Vector2.Distance(touchLocation.Position, this.m_touchOrigin) > SettingsManager.MinimumDragDistance * base.GlobalScale);
						TouchInput touchInput = new TouchInput
						{
							InputType = ((!this.m_touchMoved) ? TouchInputType.Hold : TouchInputType.Move),
							Duration = (float)(frameStartTime - this.m_touchTime),
							DurationFrames = frameIndex - this.m_touchFrameIndex,
							Position = touchLocation.Position,
							Move = touchLocation.Position - this.m_touchLastPosition,
							TotalMove = touchLocation.Position - this.m_touchOrigin,
							TotalMoveLimited = touchLocation.Position - this.m_touchOriginLimited
						};
						if (MathUtils.Abs(touchInput.TotalMoveLimited.X) > this.m_radius)
						{
							this.m_touchOriginLimited.X = touchLocation.Position.X - MathUtils.Sign(touchInput.TotalMoveLimited.X) * this.m_radius;
						}
						if (MathUtils.Abs(touchInput.TotalMoveLimited.Y) > this.m_radius)
						{
							this.m_touchOriginLimited.Y = touchLocation.Position.Y - MathUtils.Sign(touchInput.TotalMoveLimited.Y) * this.m_radius;
						}
						this.m_touchInput = new TouchInput?(touchInput);
						this.m_touchLastPosition = touchLocation.Position;
					}
				}
				else if (touchLocation.State == TouchLocationState.Released && this.m_touchId != null && touchLocation.Id == this.m_touchId.Value)
				{
					if (frameStartTime - this.m_touchTime <= (double)SettingsManager.MinimumHoldDuration && Vector2.Distance(touchLocation.Position, this.m_touchOrigin) < SettingsManager.MinimumDragDistance * base.GlobalScale)
					{
						this.m_touchInput = new TouchInput?(new TouchInput
						{
							InputType = TouchInputType.Tap,
							Duration = (float)(frameStartTime - this.m_touchTime),
							DurationFrames = frameIndex - this.m_touchFrameIndex,
							Position = touchLocation.Position
						});
					}
					this.m_touchId = null;
				}
			}
		}

		// Token: 0x04001326 RID: 4902
		public int? m_touchId;

		// Token: 0x04001327 RID: 4903
		public Vector2 m_touchLastPosition;

		// Token: 0x04001328 RID: 4904
		public Vector2 m_touchOrigin;

		// Token: 0x04001329 RID: 4905
		public Vector2 m_touchOriginLimited;

		// Token: 0x0400132A RID: 4906
		public bool m_touchMoved;

		// Token: 0x0400132B RID: 4907
		public double m_touchTime;

		// Token: 0x0400132C RID: 4908
		public int m_touchFrameIndex;

		// Token: 0x0400132D RID: 4909
		public TouchInput? m_touchInput;

		// Token: 0x0400132E RID: 4910
		public float m_radius = 30f;
	}
}
