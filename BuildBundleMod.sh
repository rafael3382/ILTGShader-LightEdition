#!/bin/bash
cd ShaderApi
msbuild ShaderApi.csproj -m /p:Configuration=Debug
cd ..

cp CompileFiles/bin/Debug/ShaderApi.dll Bundle/ILTG.dll


cp Shaders/Transparent.psh ILTG/NewShaders/Transparentpsh.txt
cp Shaders/Transparent.vsh ILTG/NewShaders/Transparentvsh.txt

cp Shaders/Model.psh ILTG/NewShaders/Modelpsh.txt
cp Shaders/Model.vsh ILTG/NewShaders/Modelvsh.txt


cp Shaders/Final.psh ILTG/NewShaders/Finalpsh.txt
cp Shaders/Final.vsh ILTG/NewShaders/Finalvsh.txt

cp Shaders/TextureEffects.psh ILTG/NewShaders/TextureEffectspsh.txt
cp Shaders/TextureEffects.vsh ILTG/NewShaders/TextureEffectsvsh.txt




cp Shaders/Textures/shader_tex.png ILTG/NewShaders/Textures/

cd ILTG/NewShaders
dos2unix *
cd ../..

rm -r Bundle/Assets
cp -r ILTG/ Bundle/Assets

cd Bundle
zip -9 -r ILTGShader.zip *
mv ILTGShader.zip ../../Mods/ILTGShader.scmod
cd ..