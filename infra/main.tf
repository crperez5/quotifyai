data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "this" {
  name     = "${var.resource_group_name}${var.environment}"
  location = var.location
  tags     = var.tags
}

resource "azurerm_storage_account" "this" {
  name                     = "${var.storage_account_name}${var.environment}"
  resource_group_name      = azurerm_resource_group.this.name
  location                 = azurerm_resource_group.this.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  is_hns_enabled           = false
  tags                     = var.tags
}

resource "azurerm_log_analytics_workspace" "log_analytics" {
  name                = var.log_analytics_name
  location            = var.location
  resource_group_name = azurerm_resource_group.this.name
  sku                 = "PerGB2018"
  tags                = var.tags
}

resource "azurerm_application_insights" "this" {
  name                = "${var.application_insights_name}${var.environment}"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  application_type    = "other"
  workspace_id        = azurerm_log_analytics_workspace.log_analytics.id
  tags                = var.tags
}

module "keyvault" {
  source              = "./core/security/keyvault"
  key_vault_name      = "${var.key_vault_name}${var.environment}"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
  tags                = var.tags
}

module "keyvault_secrets" {
  source              = "./core/security/keyvault-secrets"
  resource_group_name = azurerm_resource_group.this.name
  key_vault_name      = module.keyvault.key_vault_name
  depends_on          = [module.keyvault]

  secrets = [
    {
      name  = "DummySecretName"
      value = "DummySecretValue"
    },
    {
      name  = "DummySecretName2"
      value = "DummySecretValue"
    }
  ]
}

resource "azurerm_user_assigned_identity" "this" {
  location            = var.location
  name                = "${var.app_identity_name}${var.environment}"
  resource_group_name = azurerm_resource_group.this.name
  tags                = var.tags
}

module "function_app" {
  source                                   = "./app/function"
  function_app_name                        = "${var.function_app_name}${var.environment}"
  resource_group_name                      = azurerm_resource_group.this.name
  location                                 = azurerm_resource_group.this.location
  service_plan_name                        = "${var.service_plan_name}${var.environment}"
  storage_account_name                     = azurerm_storage_account.this.name
  storage_account_access_key               = azurerm_storage_account.this.primary_access_key
  application_insights_instrumentation_key = azurerm_application_insights.this.instrumentation_key
  application_insights_connection_string   = azurerm_application_insights.this.connection_string
  user_identity_id                         = azurerm_user_assigned_identity.this.id
  tags                                     = var.tags
}

module "container_apps" {
  source                          = "./core/host/container-apps"
  container_apps_environment_name = var.container_apps_environment_name
  environment                     = var.environment
  resource_group_name             = azurerm_resource_group.this.name
  container_registry_name         = var.container_registry_name
  virtual_network_name            = var.virtual_network_name
  subnet_name                     = var.subnet_name
  log_analytics_workspace_id      = azurerm_log_analytics_workspace.log_analytics.id
  user_identity_id                = azurerm_user_assigned_identity.this.principal_id
  tags                            = var.tags
}

module "api" {
  source                        = "./app/api"
  api_name                      = var.api_name
  resource_group_name           = azurerm_resource_group.this.name
  image_name                    = null
  container_apps_environment_id = module.container_apps.container_app_environment_id
  container_registry_url        = module.container_apps.container_registry_url
  user_identity_id              = azurerm_user_assigned_identity.this.id
  tags                          = var.tags
}

# Permissions 
resource "azurerm_role_assignment" "apps_keyvault_access" {
  scope                = module.keyvault.key_vault_id
  role_definition_name = "Key Vault Reader"
  principal_id         = azurerm_user_assigned_identity.this.principal_id
}

resource "azurerm_role_assignment" "apps_storage_access" {
  scope                = azurerm_storage_account.this.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_user_assigned_identity.this.principal_id
}
