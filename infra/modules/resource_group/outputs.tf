output "resource_group_id" {
  description = "Resource Group ID"
  value       = azurerm_resource_group.rg.id
}

output "resource_group_name" {
  description = "Resource Group Name"
  value       = azurerm_resource_group.rg.name
}
