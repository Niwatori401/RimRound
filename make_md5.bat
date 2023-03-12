@ECHO OFF
setlocal EnableDelayedExpansion

REM Remove files left over from previous runs
RMDIR /Q /S SHA_MEASURE >nul 2>&1
DEL output.md5 >nul 2>&1
DEL INTEGRITY_HASH.txt >nul 2>&1

MKDIR SHA_MEASURE
git clone ./ SHA_MEASURE/ 
CD SHA_MEASURE/
C:\cygwin64\bin\bash --login -c "cd '/cygdrive/c/Program Files (x86)/Steam/steamapps/common/RimWorld/Mods/RimRound/'; cd SHA_MEASURE; find . -type f -print0 | xargs -0 dos2unix;"
ECHO Hashing files...
CALL ..\InstallationIntegrityChecker.exe CREATE "%~dp0
ECHO Cleaning up...
CD ..
RMDIR /Q /S SHA_MEASURE
certutil -hashfile ".\output.md5" MD5 > INTEGRITY_HASH.txt
DEL output.md5