output "cognitive_account_id" {
  value = azurerm_cognitive_account.openai.id
}

output "cognitive_account_subdomain_name" {
  value = azurerm_cognitive_account.openai.custom_subdomain_name
}
