@ECHO OFF
setlocal EnableDelayedExpansion

REM Remove files left over from previous runs
RMDIR /Q /S SHA_MEASURE >nul 2>&1
DEL output.md5 >nul 2>&1
DEL INTEGRITY_HASH.txt >nul 2>&1

MKDIR SHA_MEASURE
git clone ./ SHA_MEASURE/ 
CD SHA_MEASURE/
ECHO Hashing files...
CALL ..\Source\InstallationIntegrityChecker\bin\Debug\InstallationIntegrityChecker.exe CREATE "%~dp0
ECHO Cleaning up...
CD ..
RMDIR /Q /S SHA_MEASURE
certutil -hashfile ".\output.md5" MD5 > INTEGRITY_HASH.txt
DEL output.md5