#### Create Docker Image

- `cd node-sample-app`

- `docker build -t node-sample-app .`

- `docker run -p 80:80 -d node-sample-app`

#### Push Docker Image to Azure Container Registry

- `az acr login --name jomitacr -g akstrials`

- `az acr show --name jomitacr --query loginServer --output table`

- `docker tag node-sample-app jomitacr.azurecr.io/node-sample-app:v1`

- `docker push jomitacr.azurecr.io/node-sample-app:v1`

- `az acr repository list --name jomitacr --output table`

#### Create Service Fabric Application

- `npm install -g yo`

- `npm install -g generator-azuresfcontainer`

- `md sf-container-app` | `cd sf-container-app`

- `yo azuresfcontainer`

    - `nodesfapp` | `nodesampleapp` | `jomitacr.azurecr.io/node-sample-app:v1` | `<blank>` | `1`

- `cd nodesfapp\nodesfapp`

- `az acr login -n jomitacr -g akstrials`

- `az acr credential show -n jomitacr --query passwords[0].value`

- Add `ServiceManifestImport\Policies` section in the `ApplicationManifest.xml`

- Add `ServiceManifestImport\Resources` section in the `nodesampleappPkg\ServiceManifest.xml`

- Add `ServiceManifestImport\ContainerHostPolicies\PortBinding` section in the `ApplicationManifest.xml`


#### Build and Deploy the Application

- Install [Service Fabric CLI](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started-linux#set-up-the-service-fabric-cli)
    - `pip install sfctl`

- `cd nodesfapp\nodesfapp`

- Download the SF Certificate file from KeyVault

- `openssl pkcs12 -in jomitsfkeyvault-jacksf-20180402.pfx -out mycert.pem -nodes`  (convert PFX to PEM)

- `sfctl cluster select --endpoint https://jacksf.westus2.cloudapp.azure.com:19080 --pem mycert.pem --no-verify`

- `.\install.sh`

- Add the Load Balancing Rule for Port 80

- Browser the app at : http://jacksf.westus2.cloudapp.azure.com/

- `.\uninstall.sh`





