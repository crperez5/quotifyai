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

resource "azurerm_storage_share" "this" {
  name                 = var.container_app_environment_storage_name
  storage_account_name = azurerm_storage_account.this.name
  quota                = 1
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
      name  = "DummySecret"
      value = "DummyValue"
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
  env = [
    {
      name : "AZURE_CLIENT_ID",
      value : azurerm_user_assigned_identity.this.client_id
    },
    {
      name : "AZURE_KEY_VAULT_ENDPOINT",
      value : module.keyvault.key_vault_uri
    }
  ]
  tags = var.tags
}

module "container_apps" {
  source                                 = "./core/host/container-apps"
  container_apps_environment_name        = var.container_apps_environment_name
  environment                            = var.environment
  resource_group_name                    = azurerm_resource_group.this.name
  container_registry_name                = var.container_registry_name
  log_analytics_workspace_id             = azurerm_log_analytics_workspace.log_analytics.id
  user_identity_id                       = azurerm_user_assigned_identity.this.principal_id
  container_app_environment_storage_name = var.container_app_environment_storage_name
  storage_account_name                   = azurerm_storage_account.this.name
  storage_account_access_key             = azurerm_storage_account.this.primary_access_key
  file_share_name                        = azurerm_storage_share.this.name
  tags                                   = var.tags
}

module "vector_store" {
  source                             = "./core/host/container-app"
  environment                        = var.environment
  container_apps_environment_name    = var.container_apps_environment_name
  app_id                             = "vectorstore"
  app_name                           = var.vector_store_name
  resource_group_name                = azurerm_resource_group.this.name
  image_name                         = var.vector_store_image_name
  volume_mount_name                  = var.vector_store_volume_mount_name
  volume_mount_path                  = var.vector_store_volume_mount_path
  file_share_name                    = azurerm_storage_share.this.name
  ingress_external_enabled           = false
  ingress_target_port                = 80
  ingress_transport                  = "http2"
  ingress_allow_insecure_connections = true
  depends_on                         = [module.container_apps]
  env = [
    {
      name : "QDRANT__SERVICE__GRPC_PORT",
      value : 80
    }    
  ]
  liveness_probe = {
    path      = "/healthz",
    transport = "HTTP",
    port      = 6333
  }
  readiness_probe = {
    path      = "/readyz",
    transport = "HTTP",
    port      = 6333
  }
  tags = var.tags
}

module "api" {
  source                          = "./app/api"
  environment                     = var.environment
  container_apps_environment_name = var.container_apps_environment_name
  api_name                        = var.api_name
  resource_group_name             = azurerm_resource_group.this.name
  image_name                      = null
  container_apps_environment_id   = module.container_apps.container_app_environment_id
  container_registry_url          = module.container_apps.container_registry_url
  user_identity_id                = azurerm_user_assigned_identity.this.id
  env = [
    {
      name : "AZURE_CLIENT_ID",
      value : azurerm_user_assigned_identity.this.client_id
    },
    {
      name : "AZURE_KEY_VAULT_ENDPOINT",
      value : module.keyvault.key_vault_uri
    },
    {
      name : "AzureStorageAccountEndpoint",
      value : azurerm_storage_account.this.primary_blob_endpoint
    },
    {
      name : "AzureStorageContainer",
      value : var.storage_account_container
    },
    {
      name : "VectorStoreEndpoint",
      value : var.vector_store_name
    },
    {
      name : "VectorStorePort",
      value : 80
    },
    {
      name : "VectorStoreUseHttps",
      value : false
    }
  ]
  liveness_probe = {
    path      = "/health",
    transport = "HTTP",
    port      = 80
  }  
  tags       = var.tags
  depends_on = [module.vector_store]
}

# Permissions 
resource "azurerm_role_assignment" "apps_keyvault_access" {
  scope                = module.keyvault.key_vault_id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_user_assigned_identity.this.principal_id
}

resource "azurerm_role_assignment" "apps_storage_access" {
  scope                = azurerm_storage_account.this.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_user_assigned_identity.this.principal_id
}
