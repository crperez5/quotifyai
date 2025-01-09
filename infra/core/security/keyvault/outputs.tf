output "key_vault_id" {
  value = azurerm_key_vault.vault.id
}

output "key_vault_name" {
  value = azurerm_key_vault.vault.name
}

output "key_vault_uri" {
  value       = azurerm_key_vault.vault.vault_uri
  description = "URL of the Key Vault"
}

output "key_vault_secret_id" {
  value = azurerm_key_vault_certificate.my_cert_1.secret_id
}

output "ssl_certificate_name" {
  value = "keyvault-my-cert-1"
}