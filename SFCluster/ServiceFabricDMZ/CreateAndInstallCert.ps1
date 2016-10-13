param
(
    [Parameter(Position=1, Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string]$certDnsName,

	[Parameter(Position=2, Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string]$certPassword,

	[Parameter(Mandatory=$True, ParameterSetName = "Install")]
    [switch] $Install,

	[Parameter(Mandatory=$True, ParameterSetName = "Clean")]
    [switch] $Clean
)

function Cleanup-Cert
{
    Write-Host "Cleaning existing certificates..."

    $cerLocations = @("cert:\LocalMachine\My", "cert:\LocalMachine\root", "cert:\LocalMachine\CA", "cert:\CurrentUser\My")

    foreach($cerLoc in $cerLocations)
    {
        Get-ChildItem -Path $cerLoc | ? { $_.Subject -like "*" + $certDnsName + "*" } | Remove-Item
    }
}

$warningMessage = @"
This will cleanup, create and install certificate with $certDnsName in following stores:
    
    # LocalMachine\My
    # LocalMachine\root &
    # CurrentUser\My
"@

$X509KeyUsageFlags = @{
DIGITAL_SIGNATURE = 0x80
KEY_ENCIPHERMENT = 0x20
DATA_ENCIPHERMENT = 0x10
}

$X509KeySpec = @{
NONE = 0
KEYEXCHANGE = 1
SIGNATURE = 2
}

$X509PrivateKeyExportFlags = @{
EXPORT_NONE = 0
EXPORT_FLAG = 0x1
PLAINTEXT_EXPORT_FLAG = 0x2
ARCHIVING_FLAG = 0x4
PLAINTEXT_ARCHIVING_FLAG = 0x8
}

$X509CertificateEnrollmentContext = @{
USER = 0x1
MACHINE = 0x2
ADMINISTRATOR_FORCE_MACHINE = 0x3
}

$EncodingType = @{
STRING_BASE64HEADER = 0
STRING_BASE64 = 0x1
STRING_BINARY = 0x2
STRING_BASE64REQUESTHEADER = 0x3
STRING_HEX = 0x4
STRING_HEXASCII = 0x5
STRING_BASE64_ANY = 0x6
STRING_ANY = 0x7
STRING_HEX_ANY = 0x8
STRING_BASE64X509CRLHEADER = 0x9
STRING_HEXADDR = 0xa
STRING_HEXASCIIADDR = 0xb
STRING_HEXRAW = 0xc
STRING_NOCRLF = 0x40000000
STRING_NOCR = 0x80000000
}

$InstallResponseRestrictionFlags = @{
ALLOW_NONE = 0x00000000
ALLOW_NO_OUTSTANDING_REQUEST = 0x00000001
ALLOW_UNTRUSTED_CERTIFICATE = 0x00000002
ALLOW_UNTRUSTED_ROOT = 0x00000004
}

if($Install)
{
	#cleanup previous installs of the certificate
	Cleanup-Cert

	Write-Host "Installing new certificates..."
	Write-Warning $warningMessage

	$identity = [Security.Principal.WindowsIdentity]::GetCurrent()
	$certSubjectName = "CN=" + $certDnsName
	$name = new-object -com "X509Enrollment.CX500DistinguishedName"
	$name.Encode($certSubjectName, 0x00100000)

	$key = new-object -com "X509Enrollment.CX509PrivateKey.1"
	$key.ProviderName = "Microsoft RSA SChannel Cryptographic Provider"
	$key.ExportPolicy = $X509PrivateKeyExportFlags.PLAINTEXT_EXPORT_FLAG
	$key.KeySpec = $X509KeySpec.KEYEXCHANGE
	$key.Length = 1024
	$sd = "D:PAI(A;;0xd01f01ff;;;SY)(A;;0xd01f01ff;;;BA)(A;;0xd01f01ff;;;NS)(A;;0xd01f01ff;;;" + $identity.User.Value + ")"
	$key.SecurityDescriptor = $sd
	$key.MachineContext = $TRUE
	$key.Create()

	#set server auth keyspec
	$serverauthoid = new-object -com "X509Enrollment.CObjectId.1"
	$serverauthoid.InitializeFromValue("1.3.6.1.5.5.7.3.1")
	$ekuoids = new-object -com "X509Enrollment.CObjectIds.1"

	$ekuoids.add($serverauthoid)

	$clientauthoid = new-object -com "X509Enrollment.CObjectId.1"
	$clientauthoid.InitializeFromValue("1.3.6.1.5.5.7.3.2")

	$ekuoids.add($clientauthoid)

	$ekuext = new-object -com "X509Enrollment.CX509ExtensionEnhancedKeyUsage.1"
	$ekuext.InitializeEncode($ekuoids)

	$keyUsageExt = New-Object -ComObject X509Enrollment.CX509ExtensionKeyUsage
	$keyUsageExt.InitializeEncode($X509KeyUsageFlags.KEY_ENCIPHERMENT -bor $X509KeyUsageFlags.DIGITAL_SIGNATURE)

	$certTemplateName = ""
	$cert = new-object -com "X509Enrollment.CX509CertificateRequestCertificate.1"
	$cert.InitializeFromPrivateKey($X509CertificateEnrollmentContext.MACHINE, $key, $certTemplateName)
	$cert.Subject = $name
	$cert.Issuer = $cert.Subject
	$notbefore = get-date
	$ts = new-timespan -Days 2
	$cert.NotBefore = $notbefore.Subtract($ts)
	$cert.NotAfter = $cert.NotBefore.AddYears(1)
	$cert.X509Extensions.Add($ekuext)
	$cert.X509Extensions.Add($keyUsageExt)
	$cert.Encode()

	#install certificate in LocalMachine My store
	$enrollment = new-object -com "X509Enrollment.CX509Enrollment.1"
	$enrollment.InitializeFromRequest($cert)

	$certdata = $enrollment.CreateRequest($EncodingType.STRING_BASE64HEADER)

	$enrollment.InstallResponse($InstallResponseRestrictionFlags.ALLOW_UNTRUSTED_CERTIFICATE, $certdata, $EncodingType.STRING_BASE64HEADER, $certPassword)

	if (!$?)
	{
		Write-Warning "Failed to create certificates required for cluster"
		return
	}

	$srcStoreScope = "LocalMachine"
	$srcStoreName = "My"

	$srcStore = New-Object System.Security.Cryptography.X509Certificates.X509Store $srcStoreName, $srcStoreScope
	$srcStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)

	$cert = $srcStore.certificates -match $certSubjectName
	$dstStoreScope = "LocalMachine"
	$dstStoreName = "root"

	#copy cert to root store so chain build succeeds
	$dstStore = New-Object System.Security.Cryptography.X509Certificates.X509Store $dstStoreName, $dstStoreScope
	$dstStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
	foreach($c in $cert)
	{
		$dstStore.Add($c)
	}

	$dstStore.Close()

	$dstStoreScope = "CurrentUser"
	$dstStoreName = "My"

	$dstStore = New-Object System.Security.Cryptography.X509Certificates.X509Store $dstStoreName, $dstStoreScope
	$dstStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
	foreach($c in $cert)
	{
		$dstStore.Add($c)
	}
	$srcStore.Close()
	$dstStore.Close()
}

if($Clean)
{
    Cleanup-Cert
}