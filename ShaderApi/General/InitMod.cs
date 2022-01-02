using System;
using Game;
using Engine;
using Engine.Content;

namespace ShaderApi
{
public class ModInit  : CubeBlock {
public static  bool started;

public override void Initialize() {



ScreensManager.FindScreen<LoadingScreen>("Loading").AddLoadAction(delegate() {
ModInit.started = true;


});




}
public const int Index = 1023;

public string DefaultDisplayName = "Shader API Initialization";
}
}