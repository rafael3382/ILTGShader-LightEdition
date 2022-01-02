using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Engine;
using Engine.Graphics;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;
using Game;

namespace ShaderApi
{
	// Token: 0x020003F3 RID: 1011
	public class SubsystemMovingBlocks : Game.SubsystemMovingBlocks, IUpdateable, IDrawable
	{
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_shader = new NAlphaTestedShader();
			
		}

		public new void Draw(Camera camera, int drawOrder)
		{
			this.m_shader.GetParameter("u_time", false).SetValue(m_subsystemSky.m_subsystemTimeOfDay.TimeOfDay);
            this.m_shader.GetParameter("u_waterDepth", false).SetValue(m_subsystemSky.ViewUnderWaterDepth);
            base.Draw(camera, drawOrder);
			
			
		}
		
	}
}
