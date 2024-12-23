resource "azurerm_user_assigned_identity" "ca_identity" {
    location = var.location
    name = "ca_identity"
    resource_group_name = var.resource_group_name
}

module "container_registry" {
  source = "../container-registry"

  resource_group_name = var.resource_group_name
  location            = var.location

  name = var.container_registry_name
}

resource "azurerm_role_assignment" "ca_rbac" {
  scope                = module.container_registry.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.ca_identity.principal_id
}

