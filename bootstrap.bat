@echo off
echo %~dp0
if exist Smanager/DHCP (
		echo "copy DHCP to debug!"
		xcopy Smanager\DHCP Smanager\bin\Debug\DHCP /s /e /y
) else (
		echo "No DHCP modules found!"
)

if exist Smanager/TFTP (
		echo "copy TFTP to debug!"
		xcopy Smanager\TFTP Smanager\bin\Debug\TFTP /s /e /y
) else (
		echo "No TFTP modules found!"
)

if exist Smanager\bin\Debug\ (
		xcopy lib\ Smanager\bin\Debug\ /s /e /y
)

if exist Example_folder (
		echo "initialize example folder"
		md Smanager\bin\Debug\example
		xcopy Example_folder Smanager\bin\Debug\example /s /e /y
)
pause