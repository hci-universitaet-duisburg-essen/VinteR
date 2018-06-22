# to run this script the execution policy RemoteSigned must be run
# 1. run a powershell instance as administrator
# 2. type: Set-ExecutionPolicy "RemoteSigned"
# 3. approve the question

Write-Output "Copying configuration to root"
Write-Output "============================="
Write-Output ""

Write-Output "VinteR\vinter.config.json"
Copy-Item -Path VinteR\vinter.config.json -Destination . -Force

Write-Output "VinteR\vinter.config.schema.json"
Copy-Item -Path VinteR\vinter.config.schema.json -Destination . -Force

Write-Output ""
Write-Output "Running unit tests"
Write-Output "=================="
Write-Output ""

& 'VinteR\packages\NUnit.ConsoleRunner.3.8.0\tools\nunit3-console.exe' .\VinteR.Tests\bin\x64\Debug\VinteR.Tests.dll

Write-Output ""
Write-Output "Deleting configuration"
Write-Output "======================"
Write-Output ""

Write-Output "vinter.config.json"
Remove-Item -Path vinter.config.json -Force

Write-Output "vinter.config.schema.json"
Remove-Item -Path vinter.config.schema.json -Force