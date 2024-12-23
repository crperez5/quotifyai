data "azurerm_key_vault" "existing" {
  name                = var.key_vault_name
  resource_group_name = var.resource_group_name
}

resource "azurerm_key_vault_secret" "this" {
  for_each = { for secret in var.secrets : secret.name => secret }

  name         = each.value.name
  value        = each.value.value
  key_vault_id = data.azurerm_key_vault.existing.id
  content_type = each.value.contentType
}
