@ECHO OFF

setlocal

REM Get DateTime

FOR /F "TOKENS=1,2 DELIMS=/ " %%A IN ('DATE /T') DO SET mm=%%B
FOR /F "TOKENS=2,3 DELIMS=/ " %%A IN ('DATE /T') DO SET dd=%%B
FOR /F "TOKENS=3* DELIMS=/ " %%A IN ('DATE /T') DO SET yyyy=%%B

set yyyy=%yyyy:~0,4%
set d=%yyyy%-%mm%-%dd%

set t=%time%
set dt=%d% %t%

REM Build Tag Name

REM Get Custom Tag Details from user
set /p custom_TagDetails=Enter deployed production version 
if defined custom_TagDetails (set custom_TagDetails=: %custom_TagDetails%)


set defaultTag=Posted to Prod
REM defaultTag=%defaultTag%

set tagName=%custom_TagDetails% %defaultTag% %d%

REM tagName=%tagName%


REM Create Production Tag
ECHO Creating new tag from WTS Development-Production Branch: "%tagName%"
c:
cd "C:\Program Files\TortoiseSVN\bin"
svn move "https://dev.cafdex.com:9443/svn/ITI_Folsom/LOCAL/WTS/branches/Production/" "https://dev.cafdex.com:9443/svn/ITI_Folsom/LOCAL/WTS/tags/Production/%tagName%" -m "Moved Production Source from Production Branch in order to bring in current dev source for %custom_TagDetails%: %d%at%t%"


endlocal


PAUSE