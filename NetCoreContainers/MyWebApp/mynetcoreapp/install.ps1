$AppPath = "$PSScriptRoot\mynetcoreapp"
Copy-ServiceFabricApplicationPackage -ApplicationPackagePath $AppPath -ApplicationPackagePathInImageStore mynetcoreapp
Register-ServiceFabricApplicationType mynetcoreapp
New-ServiceFabricApplication fabric:/mynetcoreapp mynetcoreappType 1.0.0