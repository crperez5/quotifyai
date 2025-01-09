# keyvault module

data "azurerm_client_config" "current" {}

# data "external" "generate_certificate" {
#   program = ["powershell", "-File", "${path.module}/generate_certificate.ps1"]
# }

# locals {
#   certificate = data.external.generate_certificate.result
# }

resource "azurerm_key_vault" "vault" {
  name                       = var.key_vault_name
  location                   = var.location
  resource_group_name        = var.resource_group_name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  soft_delete_retention_days = 7
  tags                       = var.tags
}

# resource "azurerm_role_assignment" "service_principal_rbac" {
#   scope                = azurerm_key_vault.vault.id
#   role_definition_name = "Key Vault Secrets Officer"
#   principal_id         = data.azurerm_client_config.current.object_id
# }

resource "azurerm_key_vault_access_policy" "key_vault_default_policy" {
  key_vault_id = azurerm_key_vault.vault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id

  lifecycle {
    create_before_destroy = true
  }

  certificate_permissions = [
    "Backup", "Create", "Delete", "DeleteIssuers", "Get", "GetIssuers", "Import", "List", "ListIssuers", "ManageContacts", "ManageIssuers", "Purge", "Recover", "Restore", "SetIssuers", "Update"
  ]

  key_permissions = [
    "Backup", "Create", "Decrypt", "Delete", "Encrypt", "Get", "Import", "List", "Purge", "Recover", "Restore", "Sign", "UnwrapKey", "Update", "Verify", "WrapKey"
  ]

  secret_permissions = [
    "Backup", "Delete", "Get", "List", "Purge", "Recover", "Restore", "Set"
  ]

  storage_permissions = [
    "Backup", "Delete", "DeleteSAS", "Get", "GetSAS", "List", "ListSAS", "Purge", "Recover", "RegenerateKey", "Restore", "Set", "SetSAS", "Update"
  ]
}


# resource "azurerm_key_vault_certificate" "my_cert_1" {
#   name         = "my-cert-1"
#   key_vault_id = azurerm_key_vault.vault.id

#   certificate {
#     contents = local.certificate.pfx_file
#     password = local.certificate.pfx_password
#   }

#   certificate_policy {
#     issuer_parameters {
#       name = "Unknown"
#     }

#     key_properties {
#       exportable = true
#       key_size   = 2048
#       key_type   = "RSA"
#       reuse_key  = true
#     }

#     secret_properties {
#       content_type = "application/x-pkcs12"
#     }
#     lifetime_action {
#       action {
#         action_type = "EmailContacts"
#       }
#       trigger {
#         days_before_expiry = 10
#       }
#     }

#   }

# }