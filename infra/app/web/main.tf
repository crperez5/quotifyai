resource "azurerm_user_assigned_identity" "web_identity" {
    location = var.location
    name = "web"
    resource_group_name = var.resource_group_name
}