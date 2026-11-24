$ErrorActionPreference = 'Stop'

$Repo = 'rolfwessels/init-stack'
$Bin = 'init-stack'
$Releases = "https://github.com/$Repo/releases"
$InstallDir = if ($env:INSTALL_DIR) { $env:INSTALL_DIR } else { Join-Path $env:LOCALAPPDATA 'Programs\init-stack' }
$InstallDir = [IO.Path]::GetFullPath($InstallDir)

$HostArch = if ($env:PROCESSOR_ARCHITEW6432) { $env:PROCESSOR_ARCHITEW6432 } else { $env:PROCESSOR_ARCHITECTURE }
$Arch = switch ($HostArch) {
    'AMD64' { 'x64' }
    'ARM64' { 'arm64' }
    default { throw "Unsupported CPU: $HostArch. See $Releases/latest." }
}

$Tag = (Invoke-RestMethod -Uri "https://api.github.com/repos/$Repo/releases/latest" -UseBasicParsing).tag_name
if ($Tag -notmatch '^v\d+(\.\d+)+$') {
    throw "Could not resolve the latest release. See $Releases/latest."
}

$Archive = "$Bin-win-$Arch.zip"
$Download = "$Releases/download/$Tag"
$Tmp = Join-Path $InstallDir ".init-stack-install-$([guid]::NewGuid())"
[IO.Directory]::CreateDirectory($Tmp) | Out-Null
try {
    $ArchivePath = Join-Path $Tmp $Archive
    $Checksums = Join-Path $Tmp 'checksums.txt'
    Write-Host "Downloading $Archive from $Tag..."
    try {
        Invoke-WebRequest -Uri "$Download/$Archive" -OutFile $ArchivePath -UseBasicParsing
    } catch {
        throw "Could not download $Archive from $Tag. See $Releases/latest. $($_.Exception.Message)"
    }
    Invoke-WebRequest -Uri "$Download/checksums.txt" -OutFile $Checksums -UseBasicParsing

    $Pattern = '^([0-9a-f]{64}) [ *]' + [regex]::Escape($Archive) + '$'
    $Expected = @(Get-Content -LiteralPath $Checksums | ForEach-Object {
        if ($_ -match $Pattern) { $Matches[1] }
    })
    $Actual = (Get-FileHash -LiteralPath $ArchivePath -Algorithm SHA256).Hash
    if ($Expected.Count -ne 1 -or $Expected[0] -ne $Actual) {
        throw "SHA-256 verification failed for $Archive."
    }

    Expand-Archive -LiteralPath $ArchivePath -DestinationPath $Tmp
    $StagedExe = Join-Path $Tmp "$Bin.exe"
    $Version = & $StagedExe --version
    if ($LASTEXITCODE -ne 0) {
        throw "The downloaded executable exited with code $LASTEXITCODE."
    }

    $Exe = Join-Path $InstallDir "$Bin.exe"
    if ([IO.File]::Exists($Exe)) {
        [IO.File]::Replace($StagedExe, $Exe, (Join-Path $Tmp 'previous.exe'))
    } else {
        [IO.File]::Move($StagedExe, $Exe)
    }
    Write-Host "Installed init-stack $Version at $Exe"

    $userPath = [Environment]::GetEnvironmentVariable('Path', 'User')
    if (-not ($userPath -split ';' -contains $InstallDir)) {
        Write-Host "Adding $InstallDir to user PATH..."
        $newPath = if ($userPath) { "$userPath;$InstallDir" } else { $InstallDir }
        [Environment]::SetEnvironmentVariable('Path', $newPath, 'User')
        Write-Host "Restart your terminal for PATH changes to apply to new shells."
    }
    if (-not ($env:Path -split ';' -contains $InstallDir)) {
        $env:Path = "$env:Path;$InstallDir"
    }
} finally {
    Remove-Item -Recurse -Force -LiteralPath $Tmp -ErrorAction SilentlyContinue
}
