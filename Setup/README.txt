-------------------------------------------------------------
--Auto Meeting Setup--
-------------------------------------------------------------
Create SQL Server job running daily at 12 AM with steps below:

--Auto Create Meetings--
Transact-SQL script
EXEC AORMeetingInstanceAuto_Save;

-------------------------------------------------------------
--SR Import Setup--
-------------------------------------------------------------
Copy WTS_Fileshare folder to D:\
Verify FTrans/WTS_Fileshare/ exists on test.cafdex.com FTP
Create SQL Server job running every 5 minutes with steps below:

--Copy File From FTP--
Operating System
cmd.exe /c "D:\WTS_Fileshare\WTS_Fileshare.bat"

--Check For New File--
PowerShell
$ErrorActionPreference = 'stop'
$file = "D:\WTS_Fileshare\WTS_SR.csv"
if (-not (Test-Path $file))
{
    throw "$file not found."
}

--Import--
Transact-SQL script
EXEC AORSR_Import;

--Move File--
PowerShell
$date = Get-Date -uFormat "%Y%m%d"
$locationPath = "D:\WTS_Fileshare\"
$fileName = "WTS_SR"
$extension = ".csv"
$old = $locationPath + $fileName + $extension
$new =  $locationPath + $fileName + "_" + $date + $extension
$archive = $locationPath + "Archive\"
Rename-Item $old $new
Move-Item $new $archive
