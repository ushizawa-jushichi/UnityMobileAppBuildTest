rem @echo off
setlocal
pushd "%~dp0"
cd ..
set UNITY_EXE="E:\Unity\6000.0.23f1\Editor\Unity.exe"
set PROJECT_PATH=".."
rem set PROJECT_PATH="E:\Repos\UnityMobileAppBuildTest"
set BUILD_METHOD="Builder.DevelopmentBuild"
rem set BUILD_METHOD="Builder.ReleaseBuild"
set BUILD_LOG=".\build.log"

echo Processing...
%UNITY_EXE% -batchmode -quit -logFile %BUILD_LOG% -projectPath %PROJECT_PATH% -executeMethod %BUILD_METHOD%

echo Wewew...

if %ERROR_LEVEL% == 1
    echo Done.[error]
else
    echo Done.[Succeeded]

popd
pause
