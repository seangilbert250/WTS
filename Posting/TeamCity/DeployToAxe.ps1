$scriptPath = (Get-Item -Path ".\" -Verbose).FullName
$projectRootDir = $scriptPath.ToUpper().replace("\POSTING\TEAMCITY","")
$projectRootDir = $projectRootDir.ToUpper().replace("\POSTING","")
set-location $projectRootDir
$SVNURLForProject = & svn info --show-item url
$System = $projectRootDir.split("\")[-1]
$blnDev = $SVNURLForProject.split("/")[-1].equals("trunk")

$DeployPhysicalVersion = If ($blnDev) {"Deploy"} Else {"Deploy_Prod"}
$DeployVirtualVersion = If ($blnDev) {$System} Else {$System + "_PROD"}



$axe = "\\axe\deploy$\" + $DeployVirtualVersion
$Deploy = "C:\inetpub\wwwroot\" + $DeployPhysicalVersion + "\" + $System

#Delete all files and folders in the target folder
Remove-Item $axe\* -recurse

#Delete any c sharp files in the target axe folder
#Remove-Item $axe\*.cs  

#Copy all files and folders from deploy to axe
Copy-Item $Deploy\* -Destination $axe -Recurse -Force -Confirm:$false



#Replace all instances of http://localhost with http://axe in the web.config on Axe
(Get-Content $axe\Web.config).replace('Server=.;', 'Server=SQLSERVERVM1;') | Set-Content $axe\Web.config
(Get-Content $axe\Web.config).replace('http://localhost', 'http://axe') | Set-Content $axe\Web.config

