@echo off
setlocal enabledelayedexpansion

:: Set the output file name
set "outputFile=combined_content.txt"

:: Clear the output file if it exists
if exist "%outputFile%" del "%outputFile%"

:: Get the current directory
set "currentDir=%cd%"

echo Starting to combine all file contents...
echo Output file: %outputFile%
echo.

:: Initialize counter
set /a fileCount=0

:: Loop through all files recursively
for /r %%F in (*) do (
    :: Skip the output file itself to avoid infinite loop
    if /i not "%%~nxF"=="%outputFile%" (
        set /a fileCount+=1
        
        echo Processing: %%F
        
        :: Add file header to output
        echo. >> "%outputFile%"
        echo ================================== >> "%outputFile%"
        echo FILE: %%F >> "%outputFile%"
        echo DATE: %date% %time% >> "%outputFile%"
        echo ================================== >> "%outputFile%"
        echo. >> "%outputFile%"
        
        :: Try to append file content, handle errors gracefully
        type "%%F" >> "%outputFile%" 2>nul || (
            echo [ERROR: Could not read file - may be binary or access denied] >> "%outputFile%"
        )
        
        :: Add separator
        echo. >> "%outputFile%"
        echo. >> "%outputFile%"
    )
)

echo.
echo Completed! Processed !fileCount! files.
echo All content has been combined into: %outputFile%
echo.
pause