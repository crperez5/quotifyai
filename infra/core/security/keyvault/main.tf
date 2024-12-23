data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "vault" {
  name                       = var.key_vault_name
  location                   = var.location
  resource_group_name        = var.resource_group_name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  soft_delete_retention_days = 7
  enable_rbac_authorization  = false

  network_acls {
    default_action = "Deny"
    bypass         = "AzureServices"
  }
}

