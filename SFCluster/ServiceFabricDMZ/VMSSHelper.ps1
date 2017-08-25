Login-AzureRmAccount

Select-AzureRmSubscription -SubscriptionName "Jomit's Internal Subscription"

$ResourceGroupName = 'SFCluster'
$VMScaleSetName = 'FrontEnd'

Get-AzureRmVmss -ResourceGroupName $ResourceGroupName -VMScaleSetName $VMScaleSetName

Get-AzureRmVmssVM -ResourceGroupName $ResourceGroupName -VMScaleSetName $VMScaleSetName -InstanceId "1"

Get-AzureRmVM -ResourceGroupName $ResourceGroupName -Name "FrontEnd000001"