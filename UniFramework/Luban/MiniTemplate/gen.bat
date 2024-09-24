set WORKSPACE=..\..\Assets
set LUBAN_DLL=..\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=%WORKSPACE%\Scripts\AutoCodeByLuban ^
    -x outputDataDir=%WORKSPACE%\Resources\Config 

pause