data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "vault" {
  name                       = var.key_vault_name
  location                   = var.location
  resource_group_name        = var.resource_group_name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  soft_delete_retention_days = 7
  enable_rbac_authorization  = true
}

resource "azurerm_role_assignment" "service_principal_rbac" {
  scope                = azurerm_key_vault.vault.id
  role_definition_name = "Key Vault Secrets Officer" 
  principal_id         = data.azurerm_client_config.current.object_id
}

module "service_principal_keyvault_access" {
  source                     = "../keyvault-access"
  key_vault_id               = azurerm_key_vault.vault.id
  managed_identity_object_id = data.azurerm_client_config.current.object_id
  enable_set_permission      = true
}

