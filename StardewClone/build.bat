@echo off
echo Installing MonoGame Content Builder Tool...
dotnet new tool-manifest --force
dotnet tool install --local dotnet-mgcb --version 3.8.1.303
echo.
echo Building Stardew Valley Clone...
dotnet restore
dotnet build -c Release
echo.
echo Build complete! The executable is in: bin\Release\net8.0-windows\
echo.
pause
