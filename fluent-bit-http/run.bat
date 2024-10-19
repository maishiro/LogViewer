@title http

cd /d %~dp0

fluent-bit.exe  -w %~dp0  -c %~dp0\fluent-bit.conf

