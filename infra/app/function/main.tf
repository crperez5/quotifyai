resource "azurerm_storage_container" "this" {
  name                  = "instructions"
  storage_account_name  = var.storage_account_name
  container_access_type = "private"
}

resource "azurerm_service_plan" "this" {
  name                = var.service_plan_name
  resource_group_name = var.resource_group_name
  location            = var.location
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_windows_function_app" "this" {
  name                = var.function_app_name
  resource_group_name = var.resource_group_name
  location            = var.location

  storage_account_name       = var.storage_account_name
  storage_account_access_key = var.storage_account_access_key
  service_plan_id            = azurerm_service_plan.this.id

  tags = var.tags

  identity {
    type = "SystemAssigned"
  }  

  app_settings = {
    AzureWebJobsStorage         = "DefaultEndpointsProtocol=https;AccountName=${var.storage_account_name};AccountKey=${var.storage_account_access_key}"
    FUNCTIONS_EXTENSION_VERSION = "~4"
  }

  site_config {
    application_insights_key               = var.application_insights_instrumentation_key
    application_insights_connection_string = var.application_insights_connection_string
    application_stack {
      dotnet_version              = "v9.0"
      use_dotnet_isolated_runtime = true
    }
  }
}

resource "azurerm_role_assignment" "function_keyvault_rbac" {
  scope                = var.key_vault_id
  role_definition_name = "Key Vault Reader" 
  principal_id         = azurerm_windows_function_app.this.identity[0].principal_id
}