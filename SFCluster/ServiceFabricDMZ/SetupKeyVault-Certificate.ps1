Login-AzureRmAccount

Select-AzureRmSubscription -SubscriptionName "Jomit's Internal Subscription"

$KeyVaultName = 'appCerts'
$KeyVaultSecretName = 'jomitsfcert'
$ResourceGroupName = 'AllVaults'
$Location = 'westus'
$ClusterName = 'jomitsf'
$CertDNSName = $ClusterName + '.' + $Location + '.cloudapp.azure.com'
$Password = "pass@word1"

New-AzureRmKeyVault -VaultName  $KeyVaultName -ResourceGroupName $ResourceGroupName -Location $Location -EnabledForDeployment

$SecurePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force
$CertFileFullPath = $pwd.Path + '\' + $CertDNSName + '.pfx'

$NewCert = New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My -DnsName $CertDNSName 
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
#Certificate URL :  https://appcerts.vault.azure.net:443/secrets/jomitsfcert/9eeb5ca4f44b49e3ae7c59129e9ae230
#Certificate Thumbprint :  87AAA3D37BDBDC5FD05828FC97697D451E4C5FD4