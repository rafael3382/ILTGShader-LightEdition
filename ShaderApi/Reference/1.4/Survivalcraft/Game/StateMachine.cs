using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x02000312 RID: 786
	public class StateMachine
	{
		// Token: 0x1700037E RID: 894
		// (get) Token: 0x060016FA RID: 5882 RVA: 0x000AD428 File Offset: 0x000AB628
		public string PreviousState
		{
			get
			{
				if (this.m_previousState == null)
				{
					return null;
				}
				return this.m_previousState.Name;
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x060016FB RID: 5883 RVA: 0x000AD43F File Offset: 0x000AB63F
		public string CurrentState
		{
			get
			{
				if (this.m_currentState == null)
				{
					return null;
				}
				return this.m_currentState.Name;
			}
		}

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x060016FC RID: 5884 RVA: 0x000AD458 File Offset: 0x000AB658
		// (remove) Token: 0x060016FD RID: 5885 RVA: 0x000AD490 File Offset: 0x000AB690
		public event Action<string> OnTransitionTo;

		// Token: 0x060016FE RID: 5886 RVA: 0x000AD4C8 File Offset: 0x000AB6C8
		public void AddState(string name, Action enter, Action update, Action leave)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new Exception("State name must not be empty or null.");
			}
			this.m_states.Add(name, new StateMachine.State
			{
				Name = name,
				Enter = enter,
				Update = update,
				Leave = leave
			});
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x000AD518 File Offset: 0x000AB718
		public void TransitionTo(string stateName)
		{
			StateMachine.State state = this.FindState(stateName);
			if (state != this.m_currentState)
			{
				if (this.m_currentState != null && this.m_currentState.Leave != null)
				{
					this.m_currentState.Leave();
				}
				this.m_previousState = this.m_currentState;
				this.m_currentState = state;
				if (this.m_currentState != null && this.m_currentState.Enter != null)
				{
					this.m_currentState.Enter();
				}
				Action<string> onTransitionTo = this.OnTransitionTo;
				if (onTransitionTo == null)
				{
					return;
				}
				onTransitionTo(stateName);
			}
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x000AD5A4 File Offset: 0x000AB7A4
		public void Update()
		{
			if (this.m_currentState != null && this.m_currentState.Update != null)
			{
				this.m_currentState.Update();
			}
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x000AD5CC File Offset: 0x000AB7CC
		public StateMachine.State FindState(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			StateMachine.State result;
			if (!this.m_states.TryGetValue(name, out result))
			{
				throw new InvalidOperationException("State \"" + name + "\" not found.");
			}
			return result;
		}

		// Token: 0x04000FBD RID: 4029
		public Dictionary<string, StateMachine.State> m_states = new Dictionary<string, StateMachine.State>();

		// Token: 0x04000FBE RID: 4030
		public StateMachine.State m_currentState;

		// Token: 0x04000FBF RID: 4031
		public StateMachine.State m_previousState;

		// Token: 0x02000540 RID: 1344
		public class State
		{
			// Token: 0x040018EC RID: 6380
			public string Name;

			// Token: 0x040018ED RID: 6381
			public Action Enter;

			// Token: 0x040018EE RID: 6382
			public Action Update;

			// Token: 0x040018EF RID: 6383
			public Action Leave;
		}
	}
}
