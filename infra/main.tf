data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "this" {
  name     = "${var.resource_group_name}${var.environment}"
  location = var.location
}

resource "azurerm_storage_account" "this" {
  name                     = "${var.storage_account_name}${var.environment}"
  resource_group_name      = azurerm_resource_group.this.name
  location                 = azurerm_resource_group.this.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  is_hns_enabled           = false
}

resource "azurerm_log_analytics_workspace" "log_analytics" {
  name                = var.log_analytics_name
  location            = var.location
  resource_group_name = azurerm_resource_group.this.name
  sku                 = "PerGB2018"
}

resource "azurerm_application_insights" "this" {
  name                = "${var.application_insights_name}${var.environment}"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  application_type    = "other"
  workspace_id        = azurerm_log_analytics_workspace.log_analytics.id
}

module "keyvault" {
  source              = "./core/security/keyvault"
  key_vault_name      = "${var.key_vault_name}${var.environment}"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
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
  key_vault_id                             = module.keyvault.key_vault_id
  tags                                     = var.function_tags
}

module "container_app" {
  source                = "./core/host/container-app"
  name                  = var.container_app_name
  resource_group_name   = azurerm_resource_group.this.name
  container_registry_name = var.container_registry_name
}
