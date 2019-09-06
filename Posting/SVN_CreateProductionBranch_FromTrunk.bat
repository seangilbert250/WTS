@ECHO OFF

setlocal

REM Get DateTime

FOR /F "TOKENS=1,2 DELIMS=/ " %%A IN ('DATE /T') DO SET mm=%%B
FOR /F "TOKENS=2,3 DELIMS=/ " %%A IN ('DATE /T') DO SET dd=%%B
FOR /F "TOKENS=3* DELIMS=/ " %%A IN ('DATE /T') DO SET yyyy=%%B

set yyyy=%yyyy:~0,4%
set d=%dd%-%mm%-%yyyy%

set t=%time%

REM Build Tag Name

REM Get Custom Tag Details from user
set /p custom_TagDetails=Enter deployed production version 
if defined custom_TagDetails (set custom_TagDetails=: %custom_TagDetails%)


REM Create Production Branch
ECHO Creating new Production Branch from WTS Trunk
c:
cd "C:\Program Files\TortoiseSVN\bin"
svn copy "https://dev.cafdex.com:9443/svn/ITI_Folsom/LOCAL/WTS/trunk" "https://dev.cafdex.com:9443/svn/ITI_Folsom/LOCAL/WTS/branches/Production/" -m "%custom_TagDetails% Production Branch from current dev Source on %d% at %t%"



endlocal


PAUSE