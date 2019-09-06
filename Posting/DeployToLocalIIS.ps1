$scriptPath = (Get-Item -Path ".\" -Verbose).FullName
$projectRootDir = $scriptPath.ToUpper().replace("\POSTING\TEAMCITY","");
$projectRootDir = $projectRootDir.ToUpper().replace("\POSTING","");
set-location $projectRootDir
$SVNURLForProject = & svn info --show-item url
$System = $projectRootDir.split("\")[-1]
$blnDev = $SVNURLForProject.split("/")[-1].equals("trunk")

$DeployPhysicalVersion = If ($blnDev) {"Deploy"} Else {"Deploy_Prod"}
$DeployVirtualVersion = If ($blnDev) {$System} Else {$System + "_PROD"}

$Deploy = "C:\inetpub\wwwroot\" + $DeployPhysicalVersion + "\" + $System


#Delete all files and folders in the target folder
Remove-Item $Deploy\* -recurse -force

#Build the back-end
$aspnetCompiler = (Join-Path $env:windir 'Microsoft.NET\Framework\v4.0.30319\aspnet_compiler')
& $aspnetCompiler -p $projectRootDir -v -fixednames -f $Deploy

#Copy the Aspose.Cells.licx file
Copy-Item $projectRootDir\bin\Aspose.Cells.licx -Destination $Deploy\bin -Confirm:$false

#Update the web.config to have the TimeStamp of when the application was last compiled in the Error-EmailFromName
$CompileTimeStamp = Get-Date -Format g
$webConfig = $Deploy + '\Web.config'
$doc = (Get-Content $webConfig) -as [Xml]
$obj = $doc.configuration.appSettings.add | where {$_.Key -eq 'ErrorEmailFromName'}
$obj.value = $System + ' - Development ' + $CompileTimeStamp + ' PST'

$obj = $doc.configuration.appSettings.add | where {$_.Key -eq 'Server'}
$obj.value = 'SQLSERVERVM2'

$doc.Save($webConfig)


$IE=new-object -com internetexplorer.application
$IE.navigate2("http://localhost/" + $DeployVirtualVersion)
$IE.visible=$true 