# Wait for DynamoDB Local to start
Write-Host "Waiting for DynamoDB Local to start..."
Start-Sleep -Seconds 5

# Create the quotes table
aws dynamodb create-table `
    --table-name quotes `
    --attribute-definitions `
        AttributeName=quoteId,AttributeType=S `
        AttributeName=createdDate,AttributeType=S `
    --key-schema `
        AttributeName=quoteId,KeyType=HASH `
        AttributeName=createdDate,KeyType=RANGE `
    --provisioned-throughput `
        ReadCapacityUnits=5,WriteCapacityUnits=5 `
    --endpoint-url http://localhost:8000

Write-Host "DynamoDB Local initialized with the 'quotes' table."
