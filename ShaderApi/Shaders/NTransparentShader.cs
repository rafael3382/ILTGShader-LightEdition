using Engine.Graphics;
using Engine;
using System.IO;

namespace Game
{
   public class NTransparentShader : Shader
    {
        
        public NTransparentShader() : base(ShaderReader.GetShader("Transparent.vsh"), ShaderReader.GetShader("Transparent.psh"), new ShaderMacro("IPASH")) 
       {
        }
        
    }
}
