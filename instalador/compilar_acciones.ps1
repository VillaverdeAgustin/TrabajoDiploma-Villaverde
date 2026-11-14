# 575_AV - Compila las acciones personalizadas del instalador como DLL x64 con funciones nativas exportadas.
# Windows Installer llama a funciones nativas (UINT __stdcall F(MSIHANDLE)). La DLL se compila en C# con los
# mismos archivos de la DAL que usa el programa; despues ildasm/ilasm agregan una exportacion por accion.
# Uso: compilar_acciones.ps1 -Salida <carpeta>
param([Parameter(Mandatory = $true)][string]$Salida)
$ErrorActionPreference = 'Stop'

$raiz = Split-Path $PSScriptRoot -Parent
$acciones = Join-Path $PSScriptRoot 'acciones'
$nombre = 'AccionesInstalador_575_AV'
$exportadas = 'ListarInstancias', 'ProbarConexion', 'PrepararBaseDatos', 'CrearBaseDatos'

$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$vs = & $vswhere -latest -property installationPath
$csc = Join-Path $vs 'MSBuild\Current\Bin\Roslyn\csc.exe'
$fw = "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319"
$ilasm = Join-Path $fw 'ilasm.exe'
$ildasm = Get-ChildItem "${env:ProgramFiles(x86)}\Microsoft SDKs\Windows" -Recurse -Filter ildasm.exe |
          Where-Object { $_.FullName -match 'NETFX 4\.\d+ Tools\\ildasm\.exe$' } | Select-Object -First 1 -ExpandProperty FullName
foreach ($herramienta in $csc, $ilasm, $ildasm) {
    if (-not $herramienta -or -not (Test-Path $herramienta)) { throw "Falta una herramienta de compilacion: $herramienta" }
}

$temporal = Join-Path $Salida 'compilacion'
Remove-Item $temporal -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory $temporal -Force | Out-Null

$fuentes = @(
    "$acciones\AccionesInstalador_575_AV.cs",
    "$acciones\SesionMsi_575_AV.cs",
    "$raiz\Acceso_DAL\ConfiguracionConexion_575_AV.cs",
    "$raiz\Acceso_DAL\InstaladorBaseDatos_575_AV.cs",
    "$raiz\Entidad_BE\AvanceInstalacion_575_AV.cs"
)
$referencias = 'System.dll', 'System.Core.dll', 'System.Data.dll' | ForEach-Object { "-r:$fw\$_" }
& $csc -nologo -noconfig -nostdlib -r:"$fw\mscorlib.dll" @referencias -target:library -platform:x64 -optimize `
    -out:"$temporal\$nombre.dll" @fuentes
if ($LASTEXITCODE -ne 0) { throw 'Fallo la compilacion de las acciones del instalador.' }

& $ildasm "$temporal\$nombre.dll" /out:"$temporal\$nombre.il" /nobar
if ($LASTEXITCODE -ne 0) { throw 'Fallo ildasm.' }

$il = [IO.File]::ReadAllText("$temporal\$nombre.il")
$tabla = New-Object System.Text.StringBuilder
for ($i = 0; $i -lt $exportadas.Count; $i++) {
    $n = $i + 1
    $patron = '(\.method public hidebysig static uint32\s+' + $exportadas[$i] + '\(uint32 manejador\) cil managed\s*\{)'
    if ($il -notmatch $patron) { throw "No se encontro el metodo $($exportadas[$i]) en el IL." }
    $il = [regex]::Replace($il, $patron, "`$1`r`n    .vtentry $n : 1`r`n    .export [$n] as $($exportadas[$i])")
    [void]$tabla.AppendLine(".vtfixup [1] int64 fromunmanaged at VT_$n")
    [void]$tabla.AppendLine(".data VT_$n = int64(0)")
}
$il = $il -replace '\.corflags 0x0000000\d', ".corflags 0x00000000`r`n$($tabla.ToString())"
[IO.File]::WriteAllText("$temporal\$nombre.il", $il)

& $ilasm /nologo /quiet /dll /x64 /pe64 /output:"$Salida\$nombre.dll" "$temporal\$nombre.il"
if ($LASTEXITCODE -ne 0) { throw 'Fallo ilasm.' }
Remove-Item $temporal -Recurse -Force
Get-Item "$Salida\$nombre.dll"
