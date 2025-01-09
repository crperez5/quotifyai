$CertDir = ".\certs"
$PfxPassword = "kalyan"
$PfxFile = "$CertDir\httpd.pfx"

if (Test-Path -Path $PfxFile) {
    $pfxContent = [Convert]::ToBase64String([IO.File]::ReadAllBytes($PfxFile))

    $result = @{
        pfx_file = $pfxContent
        pfx_password = $PfxPassword
    }

    $result | ConvertTo-Json -Compress
} else {
    if (!(Test-Path -Path $CertDir)) {
        New-Item -ItemType Directory -Path $CertDir
    }

    $KeyFile = "$CertDir\httpd.key"
    $CrtFile = "$CertDir\httpd.crt"

    openssl req -newkey rsa:2048 -nodes -keyout $KeyFile -x509 -days 7300 -out $CrtFile -subj "/CN=quotifyai.io"
    openssl pkcs12 -export -out $PfxFile -inkey $KeyFile -in $CrtFile -passout pass:$PfxPassword

    $pfxContent = [Convert]::ToBase64String([IO.File]::ReadAllBytes($PfxFile))

    $result = @{
        pfx_file = $pfxContent
        pfx_password = $PfxPassword
    }

    # Return the new certificate data as JSON
    $result | ConvertTo-Json -Compress
}
