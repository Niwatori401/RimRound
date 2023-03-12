@ECHO OFF
setlocal EnableDelayedExpansion

REM Remove files left over from previous runs
DEL verify_output.md5 >nul 2>&1
DEL RESULT_HASH.txt >nul 2>&1

ECHO Hashing files... (This can take a few minutes)
CALL .\Source\InstallationIntegrityChecker\bin\Debug\InstallationIntegrityChecker.exe CHECK "%~dp0


certutil -hashfile ".\verify_output.md5" MD5 > RESULT_HASH.txt
ECHO Installed at: >> RESULT_HASH.txt
ECHO %~dp0 >> RESULT_HASH.txt
DEL verify_output.md5

FINDSTR /N . INTEGRITY_HASH.txt | FINDSTR ^^2 > tmp
set /p integrity_hash= < tmp 

FINDSTR /N . RESULT_HASH.txt | FINDSTR ^^2 > tmp
set /p verify_hash= < tmp 

DEL tmp 

IF !integrity_hash!==!verify_hash! (
	ECHO ------------------------------
	ECHO   Files verified successfully!
	ECHO ------------------------------

) ELSE (
	ECHO ------------------------------
	ECHO   Files failed to validate.
	ECHO ------------------------------
)
PAUSE