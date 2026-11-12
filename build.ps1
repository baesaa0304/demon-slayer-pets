param([string]$Output = (Join-Path $PSScriptRoot 'DemonSlayerPets.exe'))

$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$sources = 'App', 'Characters', 'Rendering' | ForEach-Object {
    Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot $_) -Filter '*.cs' -File -Recurse
} | Select-Object -ExpandProperty FullName
$icon = Join-Path $PSScriptRoot 'assets\HinokamiKagura.ico'
& $compiler /nologo /target:winexe "/win32icon:$icon" "/out:$Output" $sources
exit $LASTEXITCODE
