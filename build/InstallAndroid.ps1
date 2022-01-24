$url = "https://dl.google.com/android/repository/commandlinetools-win-7583922_latest.zip"
$fileName = "AndroidTools.zip"
$source = "C:\Downloads\$fileName"
$destination = '$(Build.SourcesDirectory)\AndroidSdkTools'
$androidSdk = "C:\Program Files (x86)\Android\android-sdk\"

$ErrorActionPreference = "Stop"

if (Test-Path "$PSScriptRoot\win-installer-helper.psm1") 
{
    Import-Module "$PSScriptRoot\win-installer-helper.psm1" -DisableNameChecking
} elseif (Test-Path "$PSScriptRoot\..\..\Helpers\win-installer-helper.psm1") 
{
    Import-Module "$PSScriptRoot\..\..\Helpers\win-installer-helper.psm1" -DisableNameChecking
}

try 
{
    Get-File -Url $url -FileName $fileName
    Expand-ArchiveWith7Zip -Source $source -Destination $destination
} 
finally 
{
    Stop-Setup
}

echo "installing android"
$(Build.SourcesDirectory)\AndroidSdkTools\cmdline-tools\bin\.\sdkmanager "platforms;android-29" --sdk_root=$androidSdk