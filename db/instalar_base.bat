@echo off
rem 575_AV - Instalador de la base ConectAR_DB
rem Uso: instalar_base.bat [instancia] [/limpio]
setlocal EnableExtensions

set "CARPETA=%~dp0"
set "INSTANCIA=localhost\SQLEXPRESS"
set "LIMPIO="

:argumentos
if "%~1"=="" goto fin_argumentos
if /I "%~1"=="/limpio" (
    set "LIMPIO=1"
) else (
    set "INSTANCIA=%~1"
)
shift
goto argumentos
:fin_argumentos

cd /d "%CARPETA%"

where sqlcmd >nul 2>nul
if errorlevel 1 (
    echo No se encontro sqlcmd. Instale las herramientas de linea de comandos de SQL Server.
    exit /b 1
)

echo Instancia: %INSTANCIA%

if not defined LIMPIO goto instalar
echo.
echo ATENCION: se va a borrar la base ConectAR_DB de %INSTANCIA% con todos sus datos.
set "CONFIRMA="
set /p "CONFIRMA=Escriba SI para continuar: "
if /I not "%CONFIRMA%"=="SI" (
    echo Instalacion cancelada.
    exit /b 1
)
sqlcmd -S "%INSTANCIA%" -E -b -Q "IF DB_ID('ConectAR_DB') IS NOT NULL BEGIN ALTER DATABASE ConectAR_DB SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE ConectAR_DB; END"
if errorlevel 1 (
    echo No se pudo borrar ConectAR_DB.
    exit /b 1
)
echo Base anterior borrada.

:instalar
echo.
call :ejecutar 01_esquema.sql || goto fallo
call :ejecutar 02_procedimientos.sql || goto fallo
call :ejecutar 03_datos_seguridad.sql || goto fallo
call :ejecutar 04_datos_idiomas.sql || goto fallo
call :ejecutar 05_esquema_negocio_575_AV.sql || goto fallo
call :ejecutar 06_procedimientos_negocio_575_AV.sql || goto fallo
call :ejecutar 07_datos_prueba_575_AV.sql || goto fallo
call :ejecutar 08_usuarios_prueba_575_AV.sql || goto fallo
call :ejecutar 09_esquema_compras_575_AV.sql || goto fallo
call :ejecutar 10_procedimientos_compras_575_AV.sql || goto fallo
call :ejecutar 11_datos_prueba_compras_575_AV.sql || goto fallo

echo.
echo Base instalada
exit /b 0

:ejecutar
set "SCRIPT=%~1"
echo Ejecutando %SCRIPT% ...
sqlcmd -S "%INSTANCIA%" -E -b -f 65001 -i "%SCRIPT%"
exit /b %errorlevel%

:fallo
echo.
echo Fallo el script %SCRIPT%. La instalacion se detuvo.
exit /b 1
