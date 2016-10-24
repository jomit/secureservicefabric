Login-AzureRmAccount

Select-AzureRmSubscription -SubscriptionName "Jomit's Internal Subscription"

$KeyVaultName = 'appCerts'
$KeyVaultSecretName = 'jackschcert'
$ResourceGroupName = 'AllVaults'
$Location = 'westus'
$ClusterName = 'jacksch'
$CertDNSName = $ClusterName + '.' + $Location + '.cloudapp.azure.com'
$Password = "pass@word1"

#New-AzureRmKeyVault -VaultName  $KeyVaultName -ResourceGroupName $ResourceGroupName -Location $Location -EnabledForDeployment

$SecurePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force
$CertFileFullPath = $pwd.Path + '\' + $CertDNSName + '.pfx'
$NewCert = New-SelfSignedCertificate -CertStoreLocation Cert:\LocalMachine\My -DnsName $CertDNSName
Export-PfxCertificate -FilePath $CertFileFullPath -Password $SecurePassword -Cert $NewCert

$Bytes = [System.IO.File]::ReadAllBytes($CertFileFullPath)
$Base64 = [System.Convert]::ToBase64String($Bytes)

$JSONBlob = @{
    data = $Base64
    dataType = 'pfx'
    password = $Password
} | ConvertTo-Json

$ContentBytes = [System.Text.Encoding]::UTF8.GetBytes($JSONBlob)
$Content = [System.Convert]::ToBase64String($ContentBytes)

$SecretValue = ConvertTo-SecureString -String $Content -AsPlainText -Force
$NewSecret = Set-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $KeyVaultSecretName -SecretValue $SecretValue -Verbose

Write-Host
Write-Host "Source Vault Resource Id: "$(Get-AzureRmKeyVault -VaultName $KeyVaultName).ResourceId
Write-Host "Certificate URL : "$NewSecret.Id
Write-Host "Certificate Thumbprint : "$NewCert.Thumbprint


#Source Vault Resource Id:  /subscriptions/d0c802cd-23ce-4323-a183-5f6d9a84743e/resourceGroups/AllVaults/providers/Microsoft.KeyVault/vaults/appCerts
#Certificate URL :  https://appcerts.vault.azure.net:443/secrets/jackschcert/dd0c2b330a9b472598bf6fe951d123e5
#Certificate Thumbprint :  2DD0A569FD7D117BFA4BF9815D184C43E55BE43C

###============================================================================================

# CREATE CERTIFICATE USING A DIFFERENT METHOD TO SET ACL FOR RPC CALLS


.\CreateAndInstallCert.ps1 $CertDNSName $Password -Install


## TODO - EXPORT THE PFX FILE AND GET THE THUMBPRINT BEFORE PROCEDDING

#$Thumbprint = "‎ece0a281ca9ce7d85653334946c03518c12121ea"
$Thumbprint = "‎ECE0A281CA9CE7D85653334946C03518C12121EA"

$CertFileFullPath = $pwd.Path + '\' + $CertDNSName + '.pfx'
$CertPath = "Cert:\LocalMachine\My\" + $Thumbprint 

$SecurePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force
Get-Item Cert:\LocalMachine\My\ECE0A281CA9CE7D85653334946C03518C12121EA | Export-PfxCertificate -FilePath $CertFileFullPath -Password $SecurePassword

.\SetCertACL.ps1 $Thumbprint "NETWORK SERVICE"


$flag = [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::Exportable
$collection = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2Collection 
$collection.Import($CertFileFullPath, $Password, $flag)
$pkcs12ContentType = [System.Security.Cryptography.X509Certificates.X509ContentType]::Pkcs12
$clearBytes = $collection.Export($pkcs12ContentType)
$fileContentEncoded = [System.Convert]::ToBase64String($clearBytes)

$JSONBlob = @{
    data = $fileContentEncoded
    dataType = 'pfx'
    password = $Password
} | ConvertTo-Json

$ContentBytes = [System.Text.Encoding]::UTF8.GetBytes($JSONBlob)
$Content = [System.Convert]::ToBase64String($ContentBytes)

$secret = ConvertTo-SecureString -String $Content -AsPlainText –Force
$secretContentType = 'application/x-pkcs12'
$NewSecret = Set-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $KeyVaultSecretName -SecretValue $Secret -ContentType $secretContentType


Write-Host "Source Vault Resource Id: "$(Get-AzureRmKeyVault -VaultName $KeyVaultName).ResourceId
Write-Host "Certificate URL : "$NewSecret.Id


#Source Vault Resource Id:  /subscriptions/d0c802cd-23ce-4323-a183-5f6d9a84743e/resourceGroups/AllVaults/providers/Microsoft.KeyVault/vaults/appCerts
#Certificate URL :  https://appcerts.vault.azure.net:443/secrets/jackschcert/62f839077c2b40569b00448ccfc9eff8


## -------------------------------------------------------------------------------------------------------


#$pswd = ConvertTo-SecureString -String "pass@word1" -Force –AsPlainText
#Get-Item Cert:\LocalMachine\My\053a87f6c1e3d08ec7fc28522a2cf1921c9daa5e | Export-PfxCertificate -FilePath C:\github\secureservicefabric\SFCluster\ServiceFabricDMZ\jacksch.westus.cloudapp.azure.com.pfx -Password $pswd

#$pswd = "pass@word1"
#$PfcFilePath ="C:\github\secureservicefabric\SFCluster\ServiceFabricDMZ\jacksch.westus.cloudapp.azure.com.pfx"
#Import-PfxCertificate -Exportable -CertStoreLocation Cert:\LocalMachine\My -FilePath $PfxFilePath -Password (ConvertTo-SecureString -String $pswd -AsPlainText -Force)

#$cert = Get-Item Cert:\LocalMachine\My\053a87f6c1e3d08ec7fc28522a2cf1921c9daa5e
#Write-Host $cert.ToString($true)


$secretRetrieved = Get-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $KeyVaultSecretName
$pfxBytes = [System.Convert]::FromBase64String($secretRetrieved.SecretValueText)
[io.file]::WriteAllBytes("C:\github\secureservicefabric\SFCluster\ServiceFabricDMZ\certificate.pfx", $pfxBytes)