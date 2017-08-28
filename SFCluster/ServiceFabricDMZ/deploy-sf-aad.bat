REM .\AADTools\SF-Setup-AAD.ps1

az login

az group deployment create --name addactivedirectoryauth --resource-group <RGName> --template-file Templates\\azure-deploy-aad.json