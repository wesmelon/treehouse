@echo off
echo Building Stardew Valley Clone...
dotnet restore
dotnet build -c Release
echo.
echo Build complete! The executable is in: bin\Release\net6.0-windows\
echo.
pause
