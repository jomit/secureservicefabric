Login-AzureRmAccount

Select-AzureRmSubscription -SubscriptionName "Jomit's Internal Subscription"

$KeyVaultName = 'appCerts'
$KeyVaultSecretName = 'jackschcert'
$ResourceGroupName = 'AllVaults'
$Location = 'westus'
$ClusterName = 'jacksch'
$CertDNSName = $ClusterName + '.' + $Location + '.cloudapp.azure.com'
$Password = "pass@word1"
$CertFileFullPath = $pwd.Path + '\' + $CertDNSName + '.pfx'


## 1) Create Key Vault
#New-AzureRmKeyVault -VaultName  $KeyVaultName -ResourceGroupName $ResourceGroupName -Location $Location -EnabledForDeployment


## 2) Create Certificate
.\CreateAndInstallCert.ps1 $CertDNSName $Password -Install



## 3) Export .pfx file
$Thumbprint = "‎‎a226ea7b883b4d7da32eb16c5981407aba274532"
$CertPath = "Cert:\LocalMachine\My\" + $Thumbprint 

$SecurePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force
Get-Item Cert:\LocalMachine\My\a226ea7b883b4d7da32eb16c5981407aba274532 | Export-PfxCertificate -FilePath $CertFileFullPath -Password $SecurePassword


## 4) 
Import-Module "C:\github\Service-Fabric\Scripts\ServiceFabricRPHelpers\ServiceFabricRPHelpers.psm1"

Invoke-AddCertToKeyVault -SubscriptionId d0c802cd-23ce-4323-a183-5f6d9a84743e `
-ResourceGroupName $ResourceGroupName `
-Location $Location `
-VaultName $KeyVaultName `
-CertificateName $KeyVaultSecretName `
-Password $Password `
-UseExistingCertificate `
-ExistingPfxFilePath $CertFileFullPath


## 5) Set ACL On Certificate
.\SetCertACL.ps1 $Thumbprint "NETWORK SERVICE"




#Name  : CertificateThumbprint
#Value : A226EA7B883B4D7DA32EB16C5981407ABA274532

#Name  : SourceVault
#Value : /subscriptions/d0c802cd-23ce-4323-a183-5f6d9a84743e/resourceGroups/AllVaults/providers/Microsoft.KeyVault/vaults/appCerts

#Name  : CertificateURL
#Value : https://appcerts.vault.azure.net:443/secrets/jackschcert/bbf155a676b146e68e39d3e089c70cfd

