resource "azurerm_resource_group" "this" {
  name     = "${var.resource_group_name}${var.environment}"
  location = var.location
  tags     = var.tags
}

resource "azurerm_cognitive_account" "openai" {
  name                  = "${var.cognitive_service_name}${var.environment}"
  location              = var.location
  resource_group_name   = var.resource_group_name
  kind                  = "OpenAI"
  sku_name              = "S0"
  custom_subdomain_name = "${var.cognitive_service_name}${var.environment}"

  network_acls {
    default_action = "Deny"
    ip_rules       = []
  }

  tags = var.tags
}

# Private DNS zone. Enables DNS resolution within the virtual network. Required by private endpoints.
resource "azurerm_private_dns_zone" "openai" {
  name                = "privatelink.openai.azure.com"
  resource_group_name = var.resource_group_name
  tags                = var.tags
}

# Link between the private DNS zone and the virtual network.
resource "azurerm_private_dns_zone_virtual_network_link" "openai" {
  name                  = "pdnslink-openai"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.openai.name
  virtual_network_id    = var.vnet_id
  tags                  = var.tags
}

# Private endpoint. Makes AI services available within the network.
resource "azurerm_private_endpoint" "openai" {
  name                = "${var.private_endpoint_name}${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.subnet_id
  tags                = var.tags

  private_service_connection {
    name                           = "psc-openai"
    private_connection_resource_id = azurerm_cognitive_account.openai.id
    subresource_names              = ["account"]
    is_manual_connection           = false
  }

  private_dns_zone_group {
    name                 = "private-dns-zone-group"
    private_dns_zone_ids = [azurerm_private_dns_zone.openai.id]
  }
}

resource "azurerm_cognitive_deployment" "chatgpt" {
  name                 = "chatgpt-model-deployment"
  cognitive_account_id = azurerm_cognitive_account.openai.id

  model {
    format  = "OpenAI"
    name    = "gpt-4o-mini"
    version = "2024-07-18"
  }

  sku {
    name = "GlobalStandard"
  }
}

resource "azurerm_cognitive_deployment" "embeddings" {
  name                 = "embedding-model-deployment"
  cognitive_account_id = azurerm_cognitive_account.openai.id

  model {
    format  = "OpenAI"
    name    = "text-embedding-ada-002"
    version = "2"
  }

  sku {
    name = "Standard"
  }
}
