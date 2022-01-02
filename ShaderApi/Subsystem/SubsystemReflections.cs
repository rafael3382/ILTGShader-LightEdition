using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using Game;

namespace ShaderApi
{
	public static class SubsystemReflections
	{
	
	    public static bool LightRender = false;
	    private static PrimitivesRenderer2D m_primitiveRender;
		public static void Load(SubsystemSky subsystemSky)
		{
			//SubsystemReflections.m_renderTarget = new RenderTarget2D(Display.Viewport.Width, Display.Viewport.Height, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
			SubsystemReflections.Screen = new RenderTarget2D(Display.Viewport.Width, Display.Viewport.Height, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
			//SubsystemReflections.Light = new RenderTarget2D(Display.Viewport.Width, Display.Viewport.Height, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
			//SubsystemReflections.Shadow = new RenderTarget2D(Display.Viewport.Width, Display.Viewport.Height, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
			//SubsystemReflections.PlayerShadow = new RenderTarget2D(320, 320, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
			
			
			//SubsystemReflections.Shadow = new RenderTarget2D(Display.Viewport.Width, Display.Viewport.Height, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
			
			m_primitiveRender = new PrimitivesRenderer2D();
			m_finalShader = new CustomUnlitShader("Final", false, true, false);
			
		}
		private static bool BlockNextDraw = false;
		
		public static void Draw(Camera camera, SubsystemSky subsystemSky, int drawOrder)
		{
		    if (BlockNextDraw) { return; }
		    foreach (ComponentPlayer componentPlayer in subsystemSky.Project.FindSubsystem<SubsystemPlayers>(true).ComponentPlayers)
			{
					if (componentPlayer.ComponentGui.IsInventoryVisible())
					{ SubsystemTerrain.TerrainRenderingEnabled = true;
                       return;}
             }
            
		    if (SubsystemReflections.Screen == null || SubsystemReflections.Screen.Width != Display.Viewport.Width || SubsystemReflections.Screen.Height != Display.Viewport.Height)
		    {
		        if (SubsystemReflections.Screen != null)
		        {
		            SubsystemReflections.Screen.Dispose();
		        }
		        SubsystemReflections.Screen  = new RenderTarget2D(Display.Viewport.Width, Display.Viewport.Height, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
		
		    }
		
		 
		    
			if (drawOrder == 1250) {
			
			TexturedBatch2D m_texturedBatch = m_primitiveRender.TexturedBatch(SubsystemReflections.Screen);
			m_texturedBatch.QueueQuad(new Vector2(0f, 0f), new Vector2(Display.Viewport.Width, Display.Viewport.Height), 0f, new Vector2(0f, 0f), new Vector2(1f, 1f), Color.White);
			Display.DepthStencilState = m_texturedBatch.DepthStencilState;
			Display.RasterizerState = m_texturedBatch.RasterizerState;
			Display.BlendState = BlendState.Opaque;
			
			if (camera is BasePerspectiveCamera)
			{
			m_finalShader.GetParameter("u_viewDir").SetValue(((BasePerspectiveCamera) camera).ViewDirection);
			}
			/*
           m_finalShader.GetParameter("u_light").SetValue(SubsystemReflections.Light);
           m_finalShader.GetParameter("u_lightState").SetValue(m_texturedBatch.SamplerState);
			*/
			
			m_finalShader.Texture = m_texturedBatch.Texture;
			m_finalShader.SamplerState = m_texturedBatch.SamplerState;
			m_finalShader.Transforms.World[0] = PrimitivesRenderer2D.ViewportMatrix();
			m_texturedBatch.FlushWithCurrentStateAndShader(m_finalShader, true);
			
			
			 }
				
				if (drawOrder == int.MinValue) {
				RenderTarget2D renderTarget = Display.RenderTarget;
				Rectangle scissorRectangle = Display.ScissorRectangle;

				Display.RenderTarget = SubsystemReflections.Screen;
				
				
			    
				
				
				
				Display.Clear(Color.Black);
				
				foreach (ComponentPlayer componentPlayer in subsystemSky.Project.FindSubsystem<SubsystemPlayers>(true).ComponentPlayers)
				{
					componentPlayer.ComponentGui.ControlsContainerWidget.IsVisible = false;
				}
				SubsystemTerrain.TerrainRenderingEnabled = true;
				
				BlockNextDraw = true;
				ScreensManager.Draw();
				BlockNextDraw = false;
				
				foreach (ComponentPlayer componentPlayer2 in subsystemSky.Project.FindSubsystem<SubsystemPlayers>(true).ComponentPlayers)
				{
					componentPlayer2.ComponentGui.ControlsContainerWidget.IsVisible = true;
				}
                
                
			
				
				
				Display.RenderTarget = renderTarget;
				Display.ScissorRectangle = scissorRectangle;
				Display.Clear(new Color?(Color.Black));
			    SubsystemTerrain.TerrainRenderingEnabled = false;
				}
				
			
			
		}

		
		
		
		public static RenderTarget2D Screen;
		
		public static SubsystemParticles m_subsystemParticles;
		
		public static SubsystemGlow m_subsystemGlow;
		
		public static RenderTarget2D Shadow;
		
		public static CustomUnlitShader m_finalShader;
		
		
		
		public static Vector3 ViewPosition;
		
		
		
		public static bool IsPlayerMoving;
		
		public static Quaternion PreviousCameraDirection = new Quaternion(0f, 0f, 0f, 0f);
		
		public static Vector3 PreviousCameraPosition = new Vector3(0f, 0f, 0f);
	}
}
