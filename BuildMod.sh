#!/bin/bash
cd ShaderApi
msbuild ShaderApi.csproj -m /p:Configuration=Release

cd ..
cp CompileFiles/bin/Release/ShaderApi.dll DEVPacking/

cd DEVPacking
zip -0 -r ShaderApi.zip *
mv ShaderApi.zip ../../Mods/ShaderApi.scmod