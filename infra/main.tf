resource "azurerm_resource_group" "this" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_storage_account" "this" {
  name                     = var.storage_account_name
  resource_group_name      = azurerm_resource_group.this.name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  is_hns_enabled           = false
}

resource "azurerm_application_insights" "this" {
  name                = var.application_insights_name
  location            = var.location
  resource_group_name = azurerm_resource_group.this.name
  application_type    = "other"
}


module "function_app" {
  source                                   = "./app/function"
  function_app_name                        = var.function_app_name
  resource_group_name                      = azurerm_resource_group.this.name
  location                                 = var.location
  service_plan_name                        = var.service_plan_name
  storage_account_name                     = azurerm_storage_account.this.name
  storage_account_access_key               = azurerm_storage_account.this.primary_access_key
  application_insights_instrumentation_key = azurerm_application_insights.this.instrumentation_key
  application_insights_connection_string   = azurerm_application_insights.this.connection_string
  tags                                     = var.function_tags
}
