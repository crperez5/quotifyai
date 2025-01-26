# Azurite Certificates Configuration

## Generate CA private key
openssl genrsa -out azurite-ca.key 2048

## Create CA configuration
cat > azurite-ca.cnf << EOF
[req]
default_bits = 2048
prompt = no
default_md = sha256
x509_extensions = v3_ca
distinguished_name = dn

[dn]
C=US
ST=Development
L=LocalDev
O=LocalDev
OU=DevTeam
CN=azurite

[v3_ca]
subjectKeyIdentifier=hash
authorityKeyIdentifier=keyid:always,issuer
basicConstraints = critical, CA:TRUE
keyUsage = critical, digitalSignature, cRLSign, keyCertSign
EOF

## Generate CA certificate with specific configuration
openssl req -x509 -new -nodes -key azurite-ca.key -sha256 -days 365 -out azurite-ca.crt -config azurite-ca.cnf

## Generate server key and CSR with SAN
openssl req -new -nodes -keyout azurite-server.key -out azurite-server.csr \
    -config <(cat <<EOF
[req]
default_bits = 2048
prompt = no
default_md = sha256
distinguished_name = dn
req_extensions = req_ext

[dn]
CN=azurite
C=US
ST=Development
L=LocalDev
O=LocalDev
OU=DevTeam

[req_ext]
subjectAltName = @alt_names

[alt_names]
DNS.1 = azurite
DNS.2 = localhost
IP.1 = 127.0.0.1
EOF
)

## Generate server certificate
openssl x509 -req -in azurite-server.csr -CA azurite-ca.crt -CAkey azurite-ca.key \
    -CAcreateserial -out server.crt -days 365 \
    -sha256 \
    -extfile <(cat <<EOF
basicConstraints = CA:FALSE
subjectAltName = @alt_names
keyUsage = digitalSignature, keyEncipherment
extendedKeyUsage = serverAuth

[alt_names]
DNS.1 = azurite
DNS.2 = localhost
IP.1 = 127.0.0.1
EOF
)