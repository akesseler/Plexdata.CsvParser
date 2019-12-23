@echo off

set SRC=%1
set CFG=%2
set OUT=%3
set PWD=%cd%
set MSB="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"

if "%CFG%" == "Release" (
	cd %SRC%
	%MSB% /p:Configuration=%CFG% Plexdata.CsvParser.NET.help.shfbproj
	cd "%PWD%"
	move /y %OUT% "%PWD%"
) else (
	echo.
	echo Creating help file is enabled only for Release builds!
	echo.
)

