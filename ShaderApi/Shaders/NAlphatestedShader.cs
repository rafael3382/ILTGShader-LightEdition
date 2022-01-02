using Engine.Graphics;
using Engine;
using System.IO;

namespace Game
{
   public class NAlphaTestedShader : Shader
    {
        
        public NAlphaTestedShader() : base(ShaderReader.GetShader("AlphaTested.vsh"), ShaderReader.GetShader("AlphaTested.psh"), new ShaderMacro("IPASH")) 
        { 
        }
        
        
    }
}
