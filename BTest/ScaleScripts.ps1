az login

az group create --name jomitsftest --location westus

az group deployment create --name initial-5-nodes --resource-group jomitsftest --template-file template.json --parameters parameters.json

# To SCALE UP Primary VMSS -> Change the capacity in the VMSS and and run the deployment again

az group deployment create --name scale-9-nodes --resource-group jomitsftest --template-file template.json --parameters parameters.json

# To SCALE DOWN Primary VMSS -> run following commands as per the documentation:
# Documentation : https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-cluster-scale-up-down

# sfctl - https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-cli
# pip install sfctl

# sftctl node commands - https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-sfctl-node#sfctl-node-info

sfctl -h

sfctl cluster select --endpoint http://jomitsf.westus.cloudapp.azure.com:19080

sfctl cluster health

sfctl node disable --node-name "_main_8" --deactivation 4

sfctl node info --node-name "_main_8"

az group deployment create --name scale-8-nodes --resource-group jomitsftest --template-file template.json --parameters parameters.json







