data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "cosmosdb_resource_group" {
  name     = "${var.resource_group_name}${var.environment}"
  location = var.location
  tags     = var.tags
}
resource "azurerm_cosmosdb_account" "db" {
  name                = "${var.cosmos_db_account_name}${var.environment}"
  location            = var.location
  resource_group_name = azurerm_resource_group.cosmosdb_resource_group.name
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  consistency_policy {
    consistency_level = "Session"
  }

  geo_location {
    location          = var.location
    failover_priority = 0
  }

  capabilities {
    name = "EnableServerless"
  }

  backup {
    type                = "Periodic"
    interval_in_minutes = 240
    retention_in_hours  = 8
    storage_redundancy  = "Local"
  }

  identity {
    type         = "UserAssigned"
    identity_ids = [var.managed_identity_id]
  }

  public_network_access_enabled     = true
  is_virtual_network_filter_enabled = false

  tags = var.tags
}

resource "azurerm_cosmosdb_sql_database" "database" {
  name                = var.database_name
  resource_group_name = azurerm_resource_group.cosmosdb_resource_group.name
  account_name        = azurerm_cosmosdb_account.db.name
}

resource "azurerm_cosmosdb_sql_container" "table" {
  name                = var.table_name
  resource_group_name = azurerm_resource_group.cosmosdb_resource_group.name
  account_name        = azurerm_cosmosdb_account.db.name
  database_name       = azurerm_cosmosdb_sql_database.database.name
  partition_key_paths = var.partition_key_paths

  indexing_policy {
    indexing_mode = "consistent"

    included_path {
      path = "/*"
    }

    excluded_path {
      path = "/content/?"
    }

    dynamic "composite_index" {
      for_each = var.composite_indexes != [] ? [1] : []
      content {
        dynamic "index" {
          for_each = var.composite_indexes
          content {
            path  = index.value.path
            order = index.value.order
          }
        }
      }
    }
  }

  default_ttl = -1
}

resource "azurerm_cosmosdb_sql_role_definition" "custom_role" {
  name                = "cosmosdb-custom-role"
  resource_group_name = azurerm_resource_group.cosmosdb_resource_group.name
  account_name        = azurerm_cosmosdb_account.db.name
  type                = "CustomRole"
  assignable_scopes   = [azurerm_cosmosdb_account.db.id]

  permissions {
    data_actions = [
      "Microsoft.DocumentDB/databaseAccounts/readMetadata",
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/create",
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/read",
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/replace",
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/upsert",
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/delete"
    ]
  }
}

resource "azurerm_cosmosdb_sql_role_assignment" "cosmos_custom_data_contributor_assignment" {
  resource_group_name = azurerm_resource_group.cosmosdb_resource_group.name
  account_name        = azurerm_cosmosdb_account.db.name
  role_definition_id  = azurerm_cosmosdb_sql_role_definition.custom_role.id
  principal_id        = var.managed_principal_id
  scope               = azurerm_cosmosdb_account.db.id
}

