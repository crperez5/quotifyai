data "azurerm_client_config" "current" {}

resource "azurerm_key_vault_access_policy" "app_policy" {
  key_vault_id = var.key_vault_id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = var.managed_identity_object_id

  secret_permissions = flatten([
    ["Get", "List"],                         
    var.enable_set_permission ? ["Set", "Delete"] : []  
  ])
}