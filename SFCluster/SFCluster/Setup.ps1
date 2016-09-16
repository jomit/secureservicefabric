Login-AzureRmAccount

Select-AzureRmSubscription -SubscriptionName "Jomit's Internal Subscription"

$keyvaultName = 'sfclustercerts'
$keyvaultSecretName = 'servicefabriccert'
$resourceGroupName = 'SFCluster'
$location = 'WestUS'

#New-AzureRmKeyVault -VaultName  $keyvaultName -ResourceGroupName $resourceGroupName -Location $location -EnabledForDeployment

# SF Cluster Name - jomitsecuresf.westus.cloudapp.azure.com

.\New-ServiceFabricClusterCertificate.ps1 '<password>' 'jomitsecuresf.westus.cloudapp.azure.com' $keyvaultName $keyvaultSecretName

# Details for "sfmaincert"

#Source Vault Resource Id:  /subscriptions/d0c802cd-23ce-4323-a183-5f6d9a84743e/resourceGroups/SFCluster/providers/Microsoft.KeyVault/vaults/sfclustercerts
#Certificate URL :  https://sfclustercerts.vault.azure.net:443/secrets/sfmaincert/b1c862e77164444387fda5d445606c76
#Certificate Thumbprint :  29DB87FE66B9893B203BD60914A4EBA4B9925A2F


#Source Vault Resource Id:  /subscriptions/d0c802cd-23ce-4323-a183-5f6d9a84743e/resourceGroups/SFCluster/providers/Microsoft.KeyVault/vaults/sfclustercerts
#Certificate URL :  https://sfclustercerts.vault.azure.net:443/secrets/servicefabriccert/a8e4dd92e27f40198d8911180bef85f1
#Certificate Thumbprint :  AB39FA32563F31C02524767265A5BB02A18A9E8C

