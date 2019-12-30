Param(
    [Parameter(Mandatory=$true)]
    [string]$binPath,
	[Parameter(Mandatory=$true)]
    [string]$outputFile
)

$newtonsoft = [Reflection.Assembly]::LoadFile("$binPath\Newtonsoft.Json.dll")
$cachybuild = [Reflection.Assembly]::LoadFile("$binPath\devoctomy.cachy.Build.dll")
[devoctomy.cachy.Build.Config.ChangeLog]::CreateLatestReleaseSummary() > "$binPath\$outputFile"