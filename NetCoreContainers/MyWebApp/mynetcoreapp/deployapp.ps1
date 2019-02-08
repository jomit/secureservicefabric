$endpoint = 'jacknewasf.westus.cloudapp.azure.com:19000'
$thumbprint = ''

# Connect to the cluster using a client certificate.
Connect-ServiceFabricCluster -ConnectionEndpoint $endpoint `
          -KeepAliveIntervalInSec 10 `
          -X509Credential -ServerCertThumbprint $thumbprint `
          -FindType FindByThumbprint -FindValue $thumbprint `
          -StoreLocation CurrentUser -StoreName My

$AppPath = "$PSScriptRoot\mynetcoreapp"

# Copy the application package to the cluster image store.
Copy-ServiceFabricApplicationPackage -ApplicationPackagePath $AppPath -ImageStoreConnectionString fabric:ImageStore -ApplicationPackagePathInImageStore mynetcoreapp

# Register the application type.
Register-ServiceFabricApplicationType -ApplicationPathInImageStore mynetcoreapp

# Create the application instance.
New-ServiceFabricApplication -ApplicationName fabric:/mynetcoreapp -ApplicationTypeName mynetcoreappType -ApplicationTypeVersion 1.0.0

