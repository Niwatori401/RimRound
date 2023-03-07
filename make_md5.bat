@ECHO OFF
setlocal EnableDelayedExpansion
MKDIR SHA_MEASURE
git clone ./ SHA_MEASURE/ 
CD SHA_MEASURE/
ECHO Hashing files... (This can take a few minutes)
FOR /R %%L IN (*) DO (
	SET FILENAME=%%L
	IF x"!FILENAME:.git\\=!"==x"!FILENAME!" (
		IF x"!FILENAME:output.md5=!"==x"!FILENAME!" (
			IF x"!FILENAME:INTEGRITY_HASH=!"==x"!FILENAME!" (
				IF x"!FILENAME:.bat=!"==x"!FILENAME!" (
					certutil -hashfile "!filepath!" MD5 >> output.md5
				)
			)
		)
	)
)
ECHO Cleaning up
COPY output.md5 ..\
CD ..
RMDIR /Q /S SHA_MEASURE
certutil -hashfile ".\output.md5" MD5 > INTEGRITY_HASH
DEL output.md5