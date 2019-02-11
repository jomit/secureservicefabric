# Create Cluster using Azure CLI

- `az login`

- `az group create --name ASFTrials --location westus`

- `az group deployment create -g ASFTrials --template-file template.json --parameters parameters.json`