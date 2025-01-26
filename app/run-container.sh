#!/bin/bash

if [ -z "$1" ]; then
  echo "Usage: $0 <dll_name>"
  exit 1
fi

dll_name="$1"

while true; do
  curl --insecure https://cosmosdb:8081/_explorer/emulator.pem > /usr/local/share/ca-certificates/cosmosdb-ca.crt
  if [ $? -eq 0 ]; then
    echo "Successfully downloaded CosmosDB emulator certificate."
    break
  else
    echo "Failed to download CosmosDB emulator certificate. Retrying..."
    sleep 5
  fi
done

update-ca-certificates
if [ $? -ne 0 ]; then
  echo "Failed to update CA certificates."
  exit 3
fi

dotnet "$dll_name"
if [ $? -ne 0 ]; then
  echo "Failed to run the .NET application."
  exit 4
fi
