# 575_AV - Genera el instalador de ConectAR con Advanced Installer Professional.
# Uso: powershell -ExecutionPolicy Bypass -File instalador\generar_instalador.ps1 [-Salida <carpeta>] [-SqlExpress <SQLEXPR_x64_ENU.exe>]
#
# Pasos (todo sale de archivos versionados; el .aip se vuelve a crear en cada corrida):
#   1. Compila la solucion en Release y las acciones del instalador (compilar_acciones.ps1).
#   2. Crea el proyecto Professional y le aplica ConectAR_575_AV.aic (linea de comandos).
#   3. Automatizacion COM: tipo de instalacion y dialogo "Base de datos" encadenado despues de FolderDlg.
#   4. Agrega BaseDatosDlg_575_AV.xml: controles, eventos, acciones y secuencias que la CLI y COM no exponen.
#   5. Compila un unico setup .exe (con el .msi y SQL Server Express 2025 adentro) en instalador\salida.
#
# El instalador de SQL Server Express 2025 (SQLEXPR_x64_ENU.exe, 714 MB) no se versiona: se toma de
# T.DIPLOMA\_descargas\SQLServerExpress2025 o de la ruta que se pase con -SqlExpress.
param([string]$Salida, [string]$SqlExpress)
$ErrorActionPreference = 'Stop'

$raiz = Split-Path $PSScriptRoot -Parent
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = & $vswhere -latest -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
$ai = if ($env:ADVINST_COM) { $env:ADVINST_COM.Trim('"') }
if (-not $ai) {
    $ai = Get-ChildItem "${env:ProgramFiles(x86)}\Caphyon" -Recurse -Filter AdvancedInstaller.com |
          Select-Object -First 1 -ExpandProperty FullName
}
if (-not $msbuild) { throw 'No se encontro MSBuild (Visual Studio 2022).' }
if (-not $ai) { throw 'No se encontro Advanced Installer.' }
if (-not $SqlExpress) { $SqlExpress = Join-Path (Split-Path $raiz -Parent) '_descargas\SQLServerExpress2025\SQLEXPR_x64_ENU.exe' }
if (-not (Test-Path $SqlExpress)) { throw "No se encontro el instalador de SQL Server Express: $SqlExpress" }

# 1. Compilacion
& $msbuild "$raiz\ConectAR_575_AV.sln" -t:Rebuild -p:Configuration=Release -v:m -nologo
if ($LASTEXITCODE -ne 0) { throw 'Fallo la compilacion en Release.' }

$bin = "$raiz\Presentacion\bin\Release"
$archivos = "$PSScriptRoot\archivos"
$salida = if ($Salida) { $Salida } else { "$PSScriptRoot\salida" }
Remove-Item $archivos, $salida -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory "$archivos\db", "$archivos\acciones", $salida -Force | Out-Null
Copy-Item "$bin\*.exe", "$bin\*.dll", "$bin\*.exe.config" $archivos
Copy-Item "$bin\db\*.sql" "$archivos\db"
Copy-Item "$bin\Ayuda" $archivos -Recurse
Copy-Item "$raiz\lib\PDFsharp-MigraDoc-1.50.5147\LICENSE" "$archivos\PDFsharp-MigraDoc-LICENSE.txt"
& "$PSScriptRoot\compilar_acciones.ps1" -Salida "$archivos\acciones" | Out-Null

# 2. Proyecto desde el .aic
$env:CONECTAR_ARCHIVOS = $archivos
$env:CONECTAR_SALIDA = $salida
$env:CONECTAR_SQLEXPRESS = (Resolve-Path $SqlExpress).Path
$proyecto = "$PSScriptRoot\ConectAR_575_AV.aip"

& $ai /newproject $proyecto -type professional -lang es -overwrite
if ($LASTEXITCODE -ne 0) { throw 'No se pudo crear el proyecto de Advanced Installer.' }
& $ai /execute $proyecto "$PSScriptRoot\ConectAR_575_AV.aic"
if ($LASTEXITCODE -ne 0) { throw 'Fallo la configuracion desde el .aic.' }

# 3. Automatizacion COM
$advinst = New-Object -ComObject AdvancedInstaller
try {
    $p = $advinst.LoadProject($proyecto)
    $p.InstallParameters.InstallationType = 'PerMachineIfAdmin'
    # Un solo setup .exe que lleva adentro el .msi y el prerequisito de SQL Server Express.
    $build = $p.BuildComponent.Builds | Select-Object -First 1
    $build.PackageType = 'SingleExe'
    $build.PackageName = 'ConectAR_575_AV_Setup'
    $dialogos = $p.DialogsComponent
    $dlg = $dialogos.NewDialog('BaseDatosDlg')
    $dialogos.AddDialogToChain($dlg, 'Install', 'FolderDlg')
    $dlg.FindControl('Title').Text = 'Base de datos'
    $dlg.FindControl('Description').Text = 'Elija la instancia de SQL Server donde se crea ConectAR_DB.'
    # Mismo titulo de ventana que el resto de las pantallas: "Instalacion de ConectAR 575_AV".
    $dlg.Title = '[Setup] de [ProductName]'
    $p.Save()
    Write-Output ('Dialogos de instalacion: ' + ($dialogos.GetChainDialogs('Install') -join ' > '))
}
finally {
    $advinst.Exit()
    [void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($advinst)
}

# 4. Filas del fragmento versionado
$aip = New-Object Xml.XmlDocument
$aip.PreserveWhitespace = $true
$aip.Load($proyecto)
$fragmento = New-Object Xml.XmlDocument
$fragmento.Load("$PSScriptRoot\BaseDatosDlg_575_AV.xml")
foreach ($origen in $fragmento.DocumentElement.SelectNodes('COMPONENT')) {
    $cid = $origen.GetAttribute('cid')
    $destino = $aip.DocumentElement.SelectSingleNode("COMPONENT[@cid='$cid']")
    if (-not $destino) {
        $destino = $aip.CreateElement('COMPONENT')
        $destino.SetAttribute('cid', $cid)
        [void]$aip.DocumentElement.AppendChild($destino)
    }
    foreach ($fila in $origen.SelectNodes('ROW')) {
        [void]$destino.AppendChild($aip.CreateWhitespace("`r`n    "))
        [void]$destino.AppendChild($aip.ImportNode($fila, $true))
    }
}
$siguiente = $aip.SelectSingleNode("//COMPONENT[@cid='caphyon.advinst.msicomp.MsiControlEventComponent']/ROW[@Dialog_='BaseDatosDlg' and @Control_='Next' and @Event='NewDialog']")
$siguiente.SetAttribute('Ordering', '3')
$siguiente.SetAttribute('Condition', 'AI_INSTALL AND (CONECTAR_PRUEBA = "1" OR CONECTAR_SIN_BASE = "1")')
# Actualizacion mayor: 769 = MigrateFeatures + VersionMinInclusive + VersionMaxInclusive. Con el maximo inclusivo,
# otra compilacion de la misma version (cada compilacion tiene un ProductCode nuevo) reemplaza a la instalada.
$anteriores = $aip.SelectSingleNode("//COMPONENT[@cid='caphyon.advinst.msicomp.MsiUpgradeComponent']/ROW[@ActionProperty='OLDPRODUCTS']")
$anteriores.SetAttribute('Attributes', '769')
# El setup .exe extrae el .msi en ProgramData: desde AppData\Roaming, la instalacion por usuario falla con el
# error 2203 (el servicio no abre el paquete dentro de la transaccion multipaquete del bootstrapper).
$build = $aip.SelectSingleNode("//COMPONENT[@cid='caphyon.advinst.msicomp.BuildComponent']/ROW[@BuildKey='DefaultBuild']")
$build.SetAttribute('ExtractionFolder', '[CommonAppDataFolder][|Manufacturer]\[|ProductName] [|ProductVersion]\install')
$descargas = $aip.SelectSingleNode("//COMPONENT[@cid='caphyon.advinst.msicomp.BootstrOptComponent']/ROW[@BootstrOptKey='GlobalOptions']")
$descargas.SetAttribute('DownloadFolder', '[CommonAppDataFolder][|Manufacturer]\[|ProductName]\prerequisites')
$seguras = $aip.SelectSingleNode("//COMPONENT[@cid='caphyon.advinst.msicomp.MsiPropsComponent']/ROW[@Property='SecureCustomProperties']")
$seguras.SetAttribute('Value', $seguras.GetAttribute('Value') + ';CONECTAR_INSTANCIA;CONECTAR_SIN_BASE;MSIINSTALLPERUSER;AI_INSTALLPERUSER;APPDIR;SHORTCUTDIR')
$escritor = New-Object Xml.XmlWriterSettings
$escritor.Encoding = New-Object Text.UTF8Encoding($false)
$xml = [Xml.XmlWriter]::Create($proyecto, $escritor)
$aip.Save($xml)
$xml.Close()

# 5. Compilacion del setup
& $ai /rebuild $proyecto
if ($LASTEXITCODE -ne 0) { throw 'Fallo la generacion del instalador.' }

Get-ChildItem $salida -File | Select-Object Name, @{ n = 'MB'; e = { [math]::Round($_.Length / 1MB, 2) } }
