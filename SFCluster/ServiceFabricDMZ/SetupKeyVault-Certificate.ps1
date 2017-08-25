cd "C:\github\secureservicefabric\SFCluster\ServiceFabricDMZ"

Login-AzureRmAccount

Select-AzureRmSubscription -SubscriptionName "Jomit's Internal Subscription"

$KeyVaultName = 'appCerts'
$KeyVaultSecretName = 'jomitsf'
$ResourceGroupName = 'AllVaults'
$Location = 'westus'
$ClusterName = 'jomitsf'
$CertDNSName = $ClusterName + '.' + $Location + '.cloudapp.azure.com'
$Password = "pass@word1"
$CertFileFullPath = $pwd.Path + '\' + $CertDNSName + '.pfx'


## 1) Create Key Vault
#New-AzureRmKeyVault -VaultName  $KeyVaultName -ResourceGroupName $ResourceGroupName -Location $Location -EnabledForDeployment




## 2) Create Certificate
.\CreateAndInstallCert.ps1 $CertDNSName $Password -Install



## 3) Export .pfx file
$Thumbprint = "‎‎‎a6e85f72174d91581d7d86a8d3e4ff1abcb66601"
$CertPath = "Cert:\LocalMachine\My\" + $Thumbprint 

$SecurePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force
Get-Item Cert:\LocalMachine\My\a6e85f72174d91581d7d86a8d3e4ff1abcb66601 | Export-PfxCertificate -FilePath $CertFileFullPath -Password $SecurePassword


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
#Value : A6E85F72174D91581D7D86A8D3E4FF1ABCB66601

#Name  : SourceVault
#Value : /subscriptions/d0c802cd-23ce-4323-a183-5f6d9a84743e/resourceGroups/AllVaults/providers/Microsoft.KeyVault/vaults/appCerts

#Name  : CertificateURL
#Value : https://appcerts.vault.azure.net:443/secrets/jomitsf/1dbb72d2155741bbb83a6df950121d16

