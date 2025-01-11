output "cosmos_db_endpoint" {
  value = azurerm_cosmosdb_account.db.endpoint
}

output "cosmos_db_primary_key" {
  value     = azurerm_cosmosdb_account.db.primary_key
  sensitive = true
}

output "cosmos_db_name" {
  value = azurerm_cosmosdb_account.db.name
}

output "cosmos_db_connection_string" {
  value = azurerm_cosmosdb_account.db.primary_sql_connection_string 
}