resource "azurerm_resource_provider_registration" "container_apps" {
  name = "Microsoft.App"
}

module "container_registry" {
  source              = "../container-registry"
  name                = var.container_registry_name
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = var.tags
}

resource "azurerm_role_assignment" "this" {
  scope                = module.container_registry.id
  role_definition_name = "AcrPull"
  principal_id         = var.user_identity_id
}

module "virtual_network" {
  source = "../virtual-network"

  resource_group_name       = var.resource_group_name
  location                  = var.location
  environment               = var.environment
  vnet_name                 = var.virtual_network_name
  subnet_name               = var.subnet_name
  address_space             = var.address_space
  subnet_address_prefix_map = var.subnet_address_prefix_map
  tags                      = var.tags
}

resource "azurerm_container_app_environment" "this" {
  name                       = "${var.container_apps_environment_name}${var.environment}"
  location                   = var.location
  resource_group_name        = var.resource_group_name
  log_analytics_workspace_id = var.log_analytics_workspace_id
  infrastructure_subnet_id   = module.virtual_network.app_subnet_id
  tags                       = var.tags
  depends_on                 = [azurerm_resource_provider_registration.container_apps]


  # We use "Consumption Plan", therefore workflow_profile is not needed. 
  # If we ever need dedicated resources, we could go with:
  /*
  workload_profile {
    name                 = "test-1"
    workload_profile_type = "D1"
    maximum_count        = 1
    minimum_count        = 1
  }
  */
}

