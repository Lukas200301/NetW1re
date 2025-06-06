# Define output path
$outputPath = "$env:USERPROFILE\Desktop\NetW1reInstaller"

# Create output directory if it doesn't exist
if (!(Test-Path $outputPath)) {
    New-Item -Path $outputPath -ItemType Directory -Force | Out-Null
    Write-Output "Created directory: $outputPath"
}

# Create InstallerOutput directory if it doesn't exist
if (!(Test-Path "InstallerOutput")) {
    New-Item -Path "InstallerOutput" -ItemType Directory -Force | Out-Null
    Write-Output "Created InstallerOutput directory"
}

# Step 1: Clean and build the application
Write-Output "Building the application..."

# Clean previous outputs
if (Test-Path "Output.Windows") {
    Remove-Item "Output.Windows" -Force -Recurse
    Write-Output "Cleaned previous build output"
}

# Build the Windows application
dotnet publish NetW1reAvalonia.Windows/NetW1reAvalonia.Windows.csproj -c Release --output ./Output.Windows --runtime win-x86 --self-contained false
if ($LASTEXITCODE -ne 0) {
    Write-Output "Failed to build the application!"
    exit 1
}

# Verify the executable name
$exePath = ".\Output.Windows\NetW1re.exe"
if (!(Test-Path $exePath)) {
    Write-Output "ERROR: Expected executable not found at: $exePath"
    $actualExe = Get-ChildItem -Path ".\Output.Windows\*.exe" | Select-Object -First 1
    if ($actualExe) {
        Write-Output "Found executable: $($actualExe.Name)"
        # Update the InnoSetup script to use the correct name
        (Get-Content "NetW1reSetupScript.Windows.iss") -replace '#define MyAppExeName "NetW1re.exe"', "#define MyAppExeName `"$($actualExe.Name)`"" | Set-Content "NetW1reSetupScript.Windows.iss"
        Write-Output "Updated InnoSetup script to use executable: $($actualExe.Name)"
    } else {
        Write-Output "No executable found in Output.Windows folder!"
        exit 1
    }
}

# Step 2: Create the installer
Write-Output "Creating the installer..."
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" NetW1reSetupScript.Windows.iss
if ($LASTEXITCODE -ne 0) {
    Write-Output "Failed to create the installer!"
    exit 1
}

# Step 3: Copy the installer to the desktop
if (Test-Path "InstallerOutput/NetW1reSetup.exe") {
    Copy-Item "InstallerOutput/NetW1reSetup.exe" -Destination "$outputPath/NetW1reSetup.exe" -Force
    Write-Output "Installer created successfully at: $outputPath/NetW1reSetup.exe"
} else {
    Write-Output "Installer file not found!"
    exit 1
}

Write-Output "Build process completed successfully!"
