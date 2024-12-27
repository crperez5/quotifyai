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

resource "azurerm_container_app_environment" "this" {
  name                       = "${var.container_apps_environment_name}${var.environment}"
  location                   = var.location
  resource_group_name        = var.resource_group_name
  log_analytics_workspace_id = var.log_analytics_workspace_id
  tags                       = var.tags
  depends_on                 = [azurerm_resource_provider_registration.container_apps]
}

resource "azurerm_container_app_environment_storage" "this" {
  name                         = var.container_app_environment_storage_name
  container_app_environment_id = azurerm_container_app_environment.this.id
  account_name                 = var.storage_account_name
  access_key                   = var.storage_account_access_key
  share_name                   = var.file_share_name
  access_mode                  = "ReadWrite"
}

