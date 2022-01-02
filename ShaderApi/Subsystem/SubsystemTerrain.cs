using Engine;
using Engine.Graphics;
using GameEntitySystem;
using System;
using System.Collections.Generic;
using TemplatesDatabase;
using Game;


namespace ShaderApi
{
    public class SubsystemTerrain : Game.SubsystemTerrain, IDrawable, IUpdateable
    {
        public SubsystemGraphics m_subsystemGraphics;
        
        
        
        
        
        
        public override void Load(ValuesDictionary valuesDictionary)
        {
            m_subsystemViews = Project.FindSubsystem<SubsystemGameWidgets>(throwOnError: true);
            SubsystemGameInfo = Project.FindSubsystem<SubsystemGameInfo>(throwOnError: true);
            m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(throwOnError: true);
            m_subsystemPickables = Project.FindSubsystem<SubsystemPickables>(throwOnError: true);
            
            m_subsystemBlockBehaviors = Project.FindSubsystem<SubsystemBlockBehaviors>(throwOnError: true);
            SubsystemAnimatedTextures = Project.FindSubsystem<SubsystemAnimatedTextures>(throwOnError: true);
            SubsystemFurnitureBlockBehavior = Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(throwOnError: true);
            SubsystemPalette = Project.FindSubsystem<SubsystemPalette>(throwOnError: true);
            Terrain = new Terrain();
            
            TerrainRenderer = new TerrainRenderer(this);
            
            TerrainUpdater = new TerrainUpdater(this);
            TerrainSerializer = new TerrainSerializer22(Terrain, SubsystemGameInfo.DirectoryName);
            BlockGeometryGenerator = new Game.BlockGeometryGenerator(Terrain, this, Project.FindSubsystem<SubsystemElectricity>(throwOnError: true), SubsystemFurnitureBlockBehavior, Project.FindSubsystem<SubsystemMetersBlockBehavior>(throwOnError: true), SubsystemPalette);
            if (string.CompareOrdinal(SubsystemGameInfo.WorldSettings.OriginalSerializationVersion, "2.1") <= 0)
            {
                TerrainGenerationMode terrainGenerationMode = SubsystemGameInfo.WorldSettings.TerrainGenerationMode;
                TerrainContentsGenerator = terrainGenerationMode == TerrainGenerationMode.FlatContinent || terrainGenerationMode == TerrainGenerationMode.FlatIsland
                    ? new TerrainContentsGeneratorFlat(this)
                    : (ITerrainContentsGenerator)new TerrainContentsGenerator21(this);
            }
            else
            {
                TerrainGenerationMode terrainGenerationMode2 = SubsystemGameInfo.WorldSettings.TerrainGenerationMode;
                TerrainContentsGenerator = terrainGenerationMode2 == TerrainGenerationMode.FlatContinent || terrainGenerationMode2 == TerrainGenerationMode.FlatIsland
                    ? new TerrainContentsGeneratorFlat(this)
                    : (ITerrainContentsGenerator)new TerrainContentsGenerator22(this);
            }
            TerrainRenderingEnabled = true;
        }
        public void Update(float dt)
        {
            if (m_subsystemGraphics == null)
            {m_subsystemGraphics = new SubsystemGraphics(Project); }
            
            base.Update(dt);
            m_subsystemGraphics.Update(dt);
        }
        
        
    
        public new void Draw(Camera camera, int drawOrder)
        {
            if (m_subsystemGraphics == null)
            {m_subsystemGraphics = new SubsystemGraphics(Project); }
            
            if (TerrainRenderingEnabled)
            {
                
                if (drawOrder == m_drawOrders[0])
                {
                    
                    TerrainUpdater.PrepareForDrawing(camera);
                    m_subsystemGraphics.PrepareForDrawing(TerrainRenderer, camera);
                    //TerrainRenderer.PrepareForDrawing(camera);
                    
                    try {
                       
                       TerrainRenderer.DrawOpaque(camera);
                       
                    }
                    catch (Exception e)
                    {
                      TerrainRenderingEnabled = false;
                      DialogsManager.ShowDialog(null, new MessageDialog("Unhandled Exception", ExceptionManager.MakeFullErrorMessage(e), LanguageControl.Get("Usual", "ok"), null, null));
                    }
                    
                    
                }
                else if (drawOrder == m_drawOrders[1])
               {
                   try {
                       
                       TerrainRenderer.DrawAlphaTested(camera);
                       m_subsystemGraphics.DrawTransparent(camera, TerrainRenderer);
                    }
                    catch (Exception e)
                    {
                      TerrainRenderingEnabled = false;
                      DialogsManager.ShowDialog(null, new MessageDialog("Unhandled Exception", ExceptionManager.MakeFullErrorMessage(e), LanguageControl.Get("Usual", "ok"), null, null));
                    }
                   
                   
               } 
            }
            /*Display.RenderTarget = NTerrainRenderer.m_OldRenderTarget;
            NTerrainRenderer.DrawSSR(camera);*/
        }
        public override void Dispose()
        {
            base.Dispose();
            m_subsystemGraphics.Dispose();
        }
    }
}
