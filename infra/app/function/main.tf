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
  sku_name            = "S1"
}

resource "azurerm_windows_function_app" "this" {
  name                = var.function_app_name
  resource_group_name = var.resource_group_name
  location            = var.location

  storage_account_name       = var.storage_account_name
  storage_account_access_key = var.storage_account_access_key
  service_plan_id            = azurerm_service_plan.this.id

  virtual_network_subnet_id = var.infrastructure_subnet_id

  tags = merge(var.tags, {
    "azd-service-name" = "function"
  })

  identity {
    type         = "SystemAssigned, UserAssigned"
    identity_ids = [var.user_identity_id]
  }

  app_settings = merge(
    {
      for item in var.env : item.name => item.value
    },
    {
      AzureWebJobsStorage = "DefaultEndpointsProtocol=https;AccountName=${var.storage_account_name};AccountKey=${var.storage_account_access_key}"
    },
    {
      FUNCTIONS_EXTENSION_VERSION = "~4"
    }
  )

  site_config {
    # vnet_route_all_enabled = true

    # dynamic "ip_restriction" {
    #   for_each = var.peer_subnet_id != null ? [1] : []

    #   content {
    #     virtual_network_subnet_id = var.peer_subnet_id
    #     priority                  = 100
    #     name                      = "Allow Peer Subnet"
    #   }
    # }

    application_insights_key               = var.application_insights_instrumentation_key
    application_insights_connection_string = var.application_insights_connection_string
    application_stack {
      dotnet_version              = "v9.0"
      use_dotnet_isolated_runtime = true
    }
  }
}

# resource "azurerm_network_security_group" "function_nsg" {
#   name                = "function-nsg"
#   location            = var.location
#   resource_group_name = var.resource_group_name

#   security_rule {
#     name                       = "AllowPeerSubnet"
#     priority                   = 100
#     direction                  = "Outbound"
#     access                     = "Allow"
#     protocol                   = "*"
#     source_port_range          = "*"
#     destination_port_range     = "*"
#     source_address_prefix      = var.infrastructure_subnet_address_prefix
#     destination_address_prefix = var.peer_subnet_address_prefix
#   }
# }


# resource "azurerm_subnet_network_security_group_association" "function_nsg_association" {
#   subnet_id                 = var.infrastructure_subnet_id
#   network_security_group_id = azurerm_network_security_group.function_nsg.id
# }
