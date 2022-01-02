using Game;
using Engine;
using Engine.Graphics;
using System.IO;

namespace Game 
{
public static class ShaderReader 
{


public static string GetShader(string ShaderName)
{

#if TRACE

return new StreamReader(Storage.OpenFile(Storage.CombinePaths(ModsManager.ExternelPath, "/LightGraphics/Shaders/", ShaderName),OpenFileMode.Read)).ReadToEnd();

#endif
#if DEBUG
return ContentManager.Get<string>("NewShaders/"+ShaderName.Replace(".", ""));
#endif


}
public static Texture2D GetImage(string ImageName)
{

#if TRACE

return Texture2D.Load(Storage.OpenFile(ModsManager.ExternelPath+"/LightGraphics/Shaders/Textures/"+ImageName+".png",OpenFileMode.Read), false, 1);

#endif
#if DEBUG
return ContentManager.Get<Texture2D>("NewShaders/Textures/"+ImageName);
#endif


}


}
}