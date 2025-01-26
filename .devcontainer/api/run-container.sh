#!/bin/bash

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
sleep infinity
