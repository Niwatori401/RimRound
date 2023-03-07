@ECHO OFF
setlocal EnableDelayedExpansion
FOR /R %%L IN (*) DO (
	SET FILENAME=%%L
	IF x"!FILENAME:.git\\=!"==x"!FILENAME!" (
		IF x"!FILENAME:output.md5=!"==x"!FILENAME!" (
			IF x"!FILENAME:INTEGRITY_HASH=!"==x"!FILENAME!" (
				certutil -hashfile "!filepath!" MD5 >> output.md5
			)
		)
	)
)
COPY output.md5 ..\
CD ..
RMDIR /Q /S SHA_MEASURE
certutil -hashfile ".\output.md5" MD5 > RESULT_HASH
DEL output.md5