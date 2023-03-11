@ECHO OFF
setlocal EnableDelayedExpansion

REM Remove files left over from previous runs
RMDIR /Q /S SHA_MEASURE >nul 2>&1
DEL output.md5 >nul 2>&1
DEL INTEGRITY_HASH.txt >nul 2>&1

MKDIR SHA_MEASURE
git clone ./ SHA_MEASURE/ 
CD SHA_MEASURE/
ECHO Hashing files... (This can take a few minutes)
FOR /R %%L IN (*) DO (
	SET FILENAME=%%L
	IF x"!FILENAME:.git\=!"==x"!FILENAME!" (
		IF x"!FILENAME:.md5=!"==x"!FILENAME!" (
			IF x"!FILENAME:INTEGRITY_HASH=!"==x"!FILENAME!" (
				IF x"!FILENAME:RESULT_HASH=!"==x"!FILENAME!" (
					IF x"!FILENAME:.bat=!"==x"!FILENAME!" (
						FOR /F %%N IN ('certutil -hashfile "!FILENAME!" MD5') DO (
							SET LINE=%%N
							IF x"!LINE:MD5=!"==x"!LINE!" (
								IF x"!LINE:CertUtil=!"==x"!LINE!" (
									ECHO !LINE!
									ECHO !LINE! >> output.md5
								)
							) 
						)
					)
				)
			)
		)
	)
)
ECHO Cleaning up...
COPY output.md5 ..\
CD ..
RMDIR /Q /S SHA_MEASURE
certutil -hashfile ".\output.md5" MD5 > INTEGRITY_HASH.txt
DEL output.md5