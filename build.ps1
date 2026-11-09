$ErrorActionPreference = 'Stop'

$shihabAppVersion = '14.0.4'
$shihabProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$mehediBinDir = Join-Path $shihabProjectRoot 'bin'
$shihabObjDir = Join-Path $shihabProjectRoot 'obj'
$mehediAssetsDir = Join-Path $shihabProjectRoot 'assets'
$sshihabb007ExtImgDir = Join-Path $shihabProjectRoot 'extension\images'
$shihabIconPng = Join-Path $mehediAssetsDir 'pdfpro.png'
$mehediIconIco = Join-Path $mehediAssetsDir 'pdfpro.ico'
$sshihabb007ExtPng = Join-Path $sshihabb007ExtImgDir 'shihab_pdfpro128.png'

New-Item -ItemType Directory -Force -Path $mehediBinDir, $shihabObjDir, $mehediAssetsDir, $sshihabb007ExtImgDir | Out-Null

# 1. Generate Official PDF PRO Brand Icons (pdfpro.png, pdfpro.ico, extension/images/shihab_pdfpro128.png)
Add-Type -AssemblyName System.Drawing

$shihabBmp = New-Object System.Drawing.Bitmap(256, 256)
$mehediG = [System.Drawing.Graphics]::FromImage($shihabBmp)
$mehediG.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$mehediG.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
$mehediG.Clear([System.Drawing.Color]::Transparent)

$sshihabb007Rect = New-Object System.Drawing.Rectangle(12, 12, 232, 232)
$shihabGradBrush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
    $sshihabb007Rect,
    [System.Drawing.Color]::FromArgb(255, 124, 92, 255),
    [System.Drawing.Color]::FromArgb(255, 6, 182, 212),
    45.0)
$mehediG.FillEllipse($shihabGradBrush, $sshihabb007Rect)

$shihabInnerRect = New-Object System.Drawing.Rectangle(28, 28, 200, 200)
$mehediDarkBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(245, 15, 17, 26))
$mehediG.FillEllipse($mehediDarkBrush, $shihabInnerRect)

$sshihabb007FontMain = New-Object System.Drawing.Font('Segoe UI', 44, [System.Drawing.FontStyle]::Bold)
$shihabFontSub = New-Object System.Drawing.Font('Segoe UI', 22, [System.Drawing.FontStyle]::Bold)
$mehediWhiteBrush = [System.Drawing.Brushes]::White
$sshihabb007CyanBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 6, 182, 212))

$mehediG.DrawString('PDF', $sshihabb007FontMain, $mehediWhiteBrush, 52, 58)
$mehediG.DrawString('PRO', $shihabFontSub, $sshihabb007CyanBrush, 82, 146)

$mehediG.Dispose()
$shihabBmp.Save($shihabIconPng, [System.Drawing.Imaging.ImageFormat]::Png)

$mehediBmp128 = New-Object System.Drawing.Bitmap($shihabBmp, 128, 128)
$mehediBmp128.Save($sshihabb007ExtPng, [System.Drawing.Imaging.ImageFormat]::Png)
$mehediBmp128.Dispose()

# Write PNG-compressed ICO container for Windows Explorer & WinForms
$sshihabb007PngBytes = [System.IO.File]::ReadAllBytes($shihabIconPng)
$shihabIcoStream = New-Object System.IO.MemoryStream
$mehediBw = New-Object System.IO.BinaryWriter($shihabIcoStream)
$mehediBw.Write([UInt16]0)
$mehediBw.Write([UInt16]1)
$mehediBw.Write([UInt16]1)
$mehediBw.Write([byte]0) # 256px width
$mehediBw.Write([byte]0) # 256px height
$mehediBw.Write([byte]0)
$mehediBw.Write([byte]0)
$mehediBw.Write([UInt16]1)
$mehediBw.Write([UInt16]32)
$mehediBw.Write([UInt32]$sshihabb007PngBytes.Length)
$mehediBw.Write([UInt32]22)
$mehediBw.Write($sshihabb007PngBytes)
$mehediBw.Flush()
[System.IO.File]::WriteAllBytes($mehediIconIco, $shihabIcoStream.ToArray())
$mehediBw.Dispose()
$shihabBmp.Dispose()

# 2. Compile x64 pdfpro.exe using .NET Framework 4.8 csc.exe + Windows 10/11 WinRT PDF & OCR Metadata
$shihabFwDir = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319'
$mehediCsc = Join-Path $shihabFwDir 'csc.exe'
$sshihabb007WinMdDir = 'C:\Windows\System32\WinMetadata'
$shihabOutExe = Join-Path $mehediBinDir 'pdfpro.exe'

$mehediSourceFiles = @(
    (Join-Path $shihabProjectRoot 'src\ShihabPdfProModels.cs'),
    (Join-Path $shihabProjectRoot 'src\MehediPdfCoreEngine.cs'),
    (Join-Path $shihabProjectRoot 'src\ShihabWinRtRenderAndOcrEngine.cs'),
    (Join-Path $shihabProjectRoot 'src\MehediOemEnginesAndConverters.cs'),
    (Join-Path $shihabProjectRoot 'src\ShihabSecuritySignAndAiEngine.cs'),
    (Join-Path $shihabProjectRoot 'src\MehediPluginLoaderAndTelemetry.cs'),
    (Join-Path $shihabProjectRoot 'src\ShihabDialogsAndToolsWindows.cs'),
    (Join-Path $shihabProjectRoot 'src\ShihabMainPdfProWindow.cs'),
    (Join-Path $shihabProjectRoot 'src\MehediProgramEntry.cs')
)

$sshihabb007Refs = @(
    '/r:System.dll',
    '/r:System.Core.dll',
    '/r:System.Drawing.dll',
    '/r:System.Windows.Forms.dll',
    '/r:System.Xml.dll',
    '/r:System.Security.dll',
    '/r:System.IO.Compression.dll',
    '/r:System.IO.Compression.FileSystem.dll',
    "/r:$shihabFwDir\System.Runtime.dll",
    "/r:$shihabFwDir\System.Runtime.WindowsRuntime.dll",
    "/r:$shihabFwDir\System.Runtime.InteropServices.WindowsRuntime.dll",
    "/r:$shihabFwDir\System.Threading.Tasks.dll",
    "/r:$sshihabb007WinMdDir\Windows.Foundation.winmd",
    "/r:$sshihabb007WinMdDir\Windows.Storage.winmd",
    "/r:$sshihabb007WinMdDir\Windows.Data.winmd",
    "/r:$sshihabb007WinMdDir\Windows.Graphics.winmd",
    "/r:$sshihabb007WinMdDir\Windows.Media.winmd",
    "/r:$sshihabb007WinMdDir\Windows.Globalization.winmd",
    "/r:$sshihabb007WinMdDir\Windows.UI.winmd"
)

& $mehediCsc /nologo /target:winexe /platform:x64 /optimize+ `
    @sshihabb007Refs `
    "/win32icon:$mehediIconIco" `
    "/out:$shihabOutExe" `
    @mehediSourceFiles

if ($LASTEXITCODE -ne 0) {
    throw "csc.exe failed compiling pdfpro.exe with exit code $LASTEXITCODE"
}

# Copy runtime plugins, assets, and extension into bin/
foreach ($shihabSub in @('plugins', 'assets', 'extension')) {
    $mehediTargetSub = Join-Path $mehediBinDir $shihabSub
    if (Test-Path $mehediTargetSub) { Remove-Item -LiteralPath $mehediTargetSub -Recurse -Force }
}
Copy-Item -Path (Join-Path $shihabProjectRoot 'plugins') -Destination (Join-Path $mehediBinDir 'plugins') -Recurse -Force
Copy-Item -Path $mehediAssetsDir -Destination (Join-Path $mehediBinDir 'assets') -Recurse -Force
Copy-Item -Path (Join-Path $shihabProjectRoot 'extension') -Destination (Join-Path $mehediBinDir 'extension') -Recurse -Force

$shihabExeHash = (Get-FileHash $shihabOutExe -Algorithm SHA256).Hash.ToLowerInvariant()
$mehediExeSize = (Get-Item $shihabOutExe).Length
Write-Output "BUILD SUCCESS [APP]: $shihabOutExe ($mehediExeSize bytes, SHA256=$shihabExeHash)"

# 2b. Copy Single-File Standalone Portable Executable (PDF-PRO-Portable-14.0.4-x64.exe)
$sshihabb007PortableExe = Join-Path $mehediBinDir "PDF-PRO-Portable-$shihabAppVersion-x64.exe"
Copy-Item -LiteralPath $shihabOutExe -Destination $sshihabb007PortableExe -Force
$shihabPortableExeHash = (Get-FileHash $sshihabb007PortableExe -Algorithm SHA256).Hash.ToLowerInvariant()
$mehediPortableExeSize = (Get-Item $sshihabb007PortableExe).Length
Write-Output "BUILD SUCCESS [PORTABLE EXE]: $sshihabb007PortableExe ($mehediPortableExeSize bytes, SHA256=$shihabPortableExeHash)"

# 3. Stage & Package Setup Wizard Payload ZIP + Portable Bundle ZIP
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$sshihabb007StageDir = Join-Path $shihabObjDir 'installer_stage'
$shihabPayloadZip = Join-Path $shihabObjDir 'shihab_pdfpro_payload.zip'
$mehediPortableZip = Join-Path $mehediBinDir "PDF-PRO-Portable-$shihabAppVersion-x64.zip"
if (Test-Path $sshihabb007StageDir) { Remove-Item -LiteralPath $sshihabb007StageDir -Recurse -Force }
if (Test-Path $shihabPayloadZip) { Remove-Item -LiteralPath $shihabPayloadZip -Force }
if (Test-Path $mehediPortableZip) { Remove-Item -LiteralPath $mehediPortableZip -Force }

New-Item -ItemType Directory -Force -Path $sshihabb007StageDir | Out-Null
Copy-Item -LiteralPath $shihabOutExe -Destination (Join-Path $sshihabb007StageDir 'pdfpro.exe') -Force
Copy-Item -LiteralPath $sshihabb007PortableExe -Destination (Join-Path $sshihabb007StageDir "PDF-PRO-Portable-$shihabAppVersion-x64.exe") -Force
Copy-Item -Path (Join-Path $shihabProjectRoot 'plugins') -Destination (Join-Path $sshihabb007StageDir 'plugins') -Recurse -Force
Copy-Item -Path $mehediAssetsDir -Destination (Join-Path $sshihabb007StageDir 'assets') -Recurse -Force
Copy-Item -Path (Join-Path $shihabProjectRoot 'extension') -Destination (Join-Path $sshihabb007StageDir 'extension') -Recurse -Force
Copy-Item -LiteralPath (Join-Path $shihabProjectRoot 'README.md') -Destination (Join-Path $sshihabb007StageDir 'README.md') -Force
Copy-Item -LiteralPath (Join-Path $shihabProjectRoot 'LICENSE') -Destination (Join-Path $sshihabb007StageDir 'LICENSE') -Force

[System.IO.Compression.ZipFile]::CreateFromDirectory(
    $sshihabb007StageDir,
    $shihabPayloadZip,
    [System.IO.Compression.CompressionLevel]::Optimal,
    $false)
Copy-Item -LiteralPath $shihabPayloadZip -Destination $mehediPortableZip -Force
Remove-Item -LiteralPath $sshihabb007StageDir -Recurse -Force -ErrorAction SilentlyContinue

$sshihabb007PortableZipHash = (Get-FileHash $mehediPortableZip -Algorithm SHA256).Hash.ToLowerInvariant()
$shihabPortableZipSize = (Get-Item $mehediPortableZip).Length
Write-Output "BUILD SUCCESS [PORTABLE ZIP]: $mehediPortableZip ($shihabPortableZipSize bytes, SHA256=$sshihabb007PortableZipHash)"

# 4. Compile x64 Setup Wizard (PDF-PRO-Setup-14.0.4-x64.exe)
$mehediSetupExe = Join-Path $mehediBinDir "PDF-PRO-Setup-$shihabAppVersion-x64.exe"
$sshihabb007SetupSrc = Join-Path $shihabProjectRoot 'src\installer\ShihabPdfProSetupWizard.cs'

& $mehediCsc /nologo /target:winexe /platform:x64 /optimize+ `
    /r:System.dll `
    /r:System.Core.dll `
    /r:System.Drawing.dll `
    /r:System.Windows.Forms.dll `
    /r:System.IO.Compression.dll `
    /r:System.IO.Compression.FileSystem.dll `
    "/win32icon:$mehediIconIco" `
    "/resource:$shihabPayloadZip,ShihabMehediPdfProInstaller.payload.zip" `
    "/out:$mehediSetupExe" `
    $sshihabb007SetupSrc

if ($LASTEXITCODE -ne 0) {
    throw "csc.exe failed compiling Setup Wizard with exit code $LASTEXITCODE"
}

$sshihabb007SetupHash = (Get-FileHash $mehediSetupExe -Algorithm SHA256).Hash.ToLowerInvariant()
$shihabSetupSize = (Get-Item $mehediSetupExe).Length
Write-Output "BUILD SUCCESS [INSTALLER]: $mehediSetupExe ($shihabSetupSize bytes, SHA256=$sshihabb007SetupHash)"

# 4b. Write SHA256SUMS.txt for GitHub Releases
$mehediSha256SumsPath = Join-Path $mehediBinDir 'SHA256SUMS.txt'
$sshihabb007ShaLines = @(
    "$shihabPortableExeHash  PDF-PRO-Portable-$shihabAppVersion-x64.exe",
    "$sshihabb007SetupHash  PDF-PRO-Setup-$shihabAppVersion-x64.exe",
    "$sshihabb007PortableZipHash  PDF-PRO-Portable-$shihabAppVersion-x64.zip",
    "$shihabExeHash  pdfpro.exe"
)
[System.IO.File]::WriteAllLines($mehediSha256SumsPath, $sshihabb007ShaLines)
Write-Output "BUILD SUCCESS [SHA256SUMS]: $mehediSha256SumsPath"

# 5. Execute Automated End-to-End Self-Test Suite Synchronously
$mehediSamplesDir = Join-Path $mehediBinDir 'samples'
$sshihabb007TestProc = Start-Process -FilePath $shihabOutExe -ArgumentList @('--self-test', $mehediSamplesDir) -Wait -PassThru -NoNewWindow
if ($sshihabb007TestProc.ExitCode -ne 0) {
    throw "pdfpro.exe --self-test failed with exit code $($sshihabb007TestProc.ExitCode)"
}
Write-Output "ALL SELF-TESTS PASSED SUCCESSFULLY!"
