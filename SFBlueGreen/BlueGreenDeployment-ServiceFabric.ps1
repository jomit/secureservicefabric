# -------------------------------------------------------------------
# Import Service Fabric SDK Module and Connect with local cluster
# -------------------------------------------------------------------
Import-Module "$ENV:ProgramFiles\Microsoft SDKs\Service Fabric\Tools\PSModule\ServiceFabricSDK\ServiceFabricSDK.psm1"

$clusterEndPoint = "localhost:19000"
$applicationType = "SFBlueGreenType"
$packagePath = "C:\github\secureservicefabric\SFBlueGreen\SFBlueGreen\pkg\Release"

Connect-ServiceFabricCluster $clusterEndPoint

# ------------------------------------------------------
# Deploy Version 1.0
# ------------------------------------------------------

$appPathInImageStore = "Store\SFBlueGreenType"
$appName = "fabric:/MyApp"
$appVersion = "1.0"

Copy-ServiceFabricApplicationPackage `
-ApplicationPackagePath $packagePath `
-ImageStoreConnectionString (Get-ImageStoreConnectionStringFromClusterManifest(Get-ServiceFabricClusterManifest)) `
-ApplicationPackagePathInImageStore $appPathInImageStore  

Register-ServiceFabricApplicationType -ApplicationPathInImageStore $appPathInImageStore

New-ServiceFabricApplication -ApplicationName $appName -ApplicationTypeName $applicationType -ApplicationTypeVersion $appVersion


# ------------------------------------------------------
# Deploy Version 2.0
# ------------------------------------------------------

$testappPathInImageStore = "Store\SFBlueGreenType-Test"
$testappName = "fabric:/MyAppTest"
$testappVersion = "2.0"

Copy-ServiceFabricApplicationPackage `
-ApplicationPackagePath $packagePath `
-ImageStoreConnectionString "file:C:\SfDevCluster\Data\ImageStoreShare" `
-ApplicationPackagePathInImageStore $testappPathInImageStore


Register-ServiceFabricApplicationType -ApplicationPathInImageStore $testappPathInImageStore

New-ServiceFabricApplication -ApplicationName $testappName  -ApplicationTypeName $applicationType -ApplicationTypeVersion $testappVersion


# Update Version 1.0.0 to 2.0.0
# ------------------------------------------------------

Start-ServiceFabricApplicationUpgrade -ApplicationName $appName `
-ApplicationTypeVersion $testappVersion `
-HealthCheckStableDurationSec 10 `
-UpgradeDomainTimeoutSec 1200 `
-UpgradeTimeout 3000   `
-FailureAction Rollback `
-Monitored