@ECHO OFF
setlocal EnableDelayedExpansion
ECHO Hashing files... (This can take a few minutes)
FOR /R %%L IN (*) DO (
	SET FILENAME=%%L
	IF x"!FILENAME:.git\\=!"==x"!FILENAME!" (
		IF x"!FILENAME:output.md5=!"==x"!FILENAME!" (
			IF x"!FILENAME:INTEGRITY_HASH=!"==x"!FILENAME!" (
				IF x"!FILENAME:RESULT_HASH=!"==x"!FILENAME!" (
					IF x"!FILENAME:.bat=!"==x"!FILENAME!" (
						certutil -hashfile "!filepath!" MD5 >> output.md5
					)
				)
			)
		)
	)
)
certutil -hashfile ".\output.md5" MD5 > RESULT_HASH
DEL output.md5