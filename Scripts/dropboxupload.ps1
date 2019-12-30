Param(
    [Parameter(Mandatory=$true)]
    [string]$sourcePath,
    [Parameter(Mandatory=$true)]
    [string]$destPath,
    [Parameter(Mandatory=$true)]
    [string]$accessToken
)

function Upload-FileToDropbox(
    $SourceFilePath, 
    $TargetFilePath, 
    $DropBoxAccessToken)
{
    $arg = '{ "path": "' + $TargetFilePath + '", "mode": "add", "autorename": true, "mute": false, "strict_conflict": false }'
    $authorization = "Bearer " + $DropBoxAccessToken

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Authorization", $authorization)
    $headers.Add("Dropbox-API-Arg", $arg)
    $headers.Add("Content-Type", 'application/octet-stream')
 
    Invoke-RestMethod -Uri https://content.dropboxapi.com/2/files/upload -Method Post -InFile $SourceFilePath -Headers $headers
}

function GetFiles($path, $retVal)
{
    foreach ($item in Get-ChildItem $path)
    {
        if (Test-Path $item.FullName -PathType Container)
        {
            GetFiles $item.FullName $retVal
        }
        else
        {
            $retVal.Add($item)
        }
    }
} 

write-host "Enumerating files in source path '" + $sourcePath + "'."

$files = New-Object "System.Collections.Generic.List[Object]"
GetFiles $sourcePath $files
if($files.Count)
{
    foreach($file in $files)
    {
        $relativePath = $file.FullName.Substring($sourcePath.Length)
        $relativePath = $destPath + $relativePath.Replace("\","/")

        write-host "Uploading '" + $file.FullName + "' to '" + $relativePath

        Upload-FileToDropbox $file.FullName $relativePath $accessToken
    }
}

write-host "Upload complete."