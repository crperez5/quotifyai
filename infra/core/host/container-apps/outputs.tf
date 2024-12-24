output "container_app_environment_id" { value = azurerm_container_app_environment.this.id }
output "container_registry_url" { value = module.container_registry.url }
output "user_identity_id" { value = azurerm_user_assigned_identity.this.id }