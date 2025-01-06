output "container_app_environment_id" { value = azurerm_container_app_environment.this.id }
output "container_registry_url" { value = module.container_registry.url }

output "env_static_ip_address" {
  value = azurerm_container_app_environment.this.static_ip_address
}
