$scriptPath = (Get-Item -Path ".\" -Verbose).FullName
$projectRootDir = $scriptPath.ToUpper().replace("\POSTING\TEAMCITY","")
$projectRootDir = $projectRootDir.ToUpper().replace("\POSTING","")
set-location $projectRootDir
$SVNURLForProject = & svn info --show-item url
$System = $projectRootDir.split("\")[-1]
$blnDev = $SVNURLForProject.split("/")[-1].equals("trunk")

$DeployPhysicalVersion = If ($blnDev) {"Deploy"} Else {"Deploy_Prod"}
$DeployVirtualVersion = If ($blnDev) {$System} Else {$System + "_PROD"}


$Dev = $projectRootDir + "\PrecompiledWeb\" + $System
$Deploy = "C:\inetpub\wwwroot\" + $DeployPhysicalVersion + "\" + $System

#Replace all files and folders in the target folder
Copy-Item $Dev\* -Destination $Deploy -Recurse -Force -Confirm:$false

#Copy the Aspose.Cells.licx file
Copy-Item $projectRootDir\bin\Aspose.Cells.licx -Destination $Deploy\bin -Confirm:$false

#Update the web.config to have the TimeStamp of when the application was last compiled in the Error-EmailFromName
$CompileTimeStamp = Get-Date -Format g
$webConfig = $Deploy + '\Web.config'
$doc = (Get-Content $webConfig) -as [Xml]
$obj = $doc.configuration.appSettings.add | where {$_.Key -eq 'ErrorEmailFromName'}
$obj.value = $System + ' - Development ' + $CompileTimeStamp + ' PST'

$obj = $doc.configuration.appSettings.add | where {$_.Key -eq 'Server'}
$obj.value = 'SQLSERVERVM1'

$doc.Save($webConfig)

