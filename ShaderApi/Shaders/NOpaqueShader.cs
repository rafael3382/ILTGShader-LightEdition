using Engine.Graphics;
using Engine;
using System.IO;

namespace Game
{
   public class NOpaqueShader : Shader
    {
        public ShaderTransforms Transforms;
        
        
        
        public Camera m_camera;
        
        public NOpaqueShader() : base(ShaderReader.GetShader("Opaque.vsh"), ShaderReader.GetShader("Opaque.psh"), new ShaderMacro("OPAQUE")) { 
        
        
        }
        public void PrepareWorldMatrix(Camera camera) {
        
        
        }
        
    }
}
