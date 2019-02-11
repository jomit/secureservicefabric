$endpoint = 'jomitsf.westus.cloudapp.azure.com:19000'
$thumbprint = '<CertThumbprint>'

# Connect to the cluster using a client certificate.
Connect-ServiceFabricCluster -ConnectionEndpoint $endpoint `
          -KeepAliveIntervalInSec 10 `
          -X509Credential -ServerCertThumbprint $thumbprint `
          -FindType FindByThumbprint -FindValue $thumbprint `
          -StoreLocation CurrentUser -StoreName My

# Remove an application instance
Remove-ServiceFabricApplication -ApplicationName fabric:/MultipleNamedAppInstances/MilkyWay

# Remove an application instance
Remove-ServiceFabricApplication -ApplicationName fabric:/MultipleNamedAppInstances/Andromeda

# Unregister the application type
Unregister-ServiceFabricApplicationType -ApplicationTypeName MultipleNamedAppInstancesType -ApplicationTypeVersion 1.0.0

# Remove the application package to free system resources.
Remove-ServiceFabricApplicationPackage -ImageStoreConnectionString fabric:ImageStore -ApplicationPackagePathInImageStore MultipleNamedAppInstances