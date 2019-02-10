# Build .Net Core Container Apps for Service Fabric

## Build and Push the Docker Image to ACR

- `docker build -t mywebapp .`  

- `az login`

- `az acr login --name <acrname>`

- `az acr show --name <acrName> --query loginServer --output table`

- `docker tag mywebapp <acrName>.azurecr.io/mywebapp:v1`

- `docker push <acrName>.azurecr.io/mywebapp:v1`

- `az acr repository list --name <acrName> --output table`

## Test the Image Locally

- `docker run -t --rm -p 5000:80 --name testwebapp mywebapp`

## Generate SF Manifests

- `yo azuresfcontainer`

- Update the AppManifest and ServiceMaifests as instructed [here](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-tutorial-package-containers)

## Deploy the App using Powershell

- `deployapp.ps1`

## Deploy the App using sfctl

- `openssl pkcs12 -in clustercert.pfx -out clustercert.pem -nodes`

- `sfctl cluster select --endpoint https://<name>.westus.cloudapp.azure.com:19080 --pem clustercert.pem --no-verify`

- `sfctl store delete --content-path mynetcoreapp`

- `sfctl application upload --path mynetcoreapp --show-progress --debug`

- `sfctl application provision --application-type-build-path mynetcoreappType --debug`

- `sfctl application create --app-name fabric:/mynetcoreapp --app-type mynetcoreappType --app-version 1.0.0`


## Additional Resources

- [SF CLI Get Started](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-cli#tips-and-troubleshooting)

- [SF CLI Documentation](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-sfctl-application)

- [SF with Containers Tutorial](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-tutorial-create-container-images)

- [SF Container Samples](https://github.com/Azure-Samples/service-fabric-containers)

- [SF Powershell Samples](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-powershell-samples)

- [SF Cluster Powershell Commands](https://docs.microsoft.com/en-us/powershell/module/servicefabric/?view=azureservicefabricps)

- [Multi container App and Service Manifest examples](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-manifest-example-container-app)

- [Issue : Application provision fails with E_INVALIDARG fabric error](https://github.com/Microsoft/service-fabric-cli/issues/107)
