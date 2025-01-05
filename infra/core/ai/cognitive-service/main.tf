resource "azurerm_resource_group" "ai_resource_group" {
  name     = "${var.ai_resource_group_name}${var.environment}"
  location = var.ai_location
  tags     = var.tags
}

resource "azurerm_virtual_network" "ai_vnet" {
  name                = "ai_vnet"
  location            = var.ai_location
  resource_group_name = azurerm_resource_group.ai_resource_group.name
  address_space       = ["10.2.0.0/16"]
  tags                = var.tags
}

resource "azurerm_subnet" "ai_subnet" {
  name                 = "ai_subnet"
  resource_group_name  = azurerm_resource_group.ai_resource_group.name
  virtual_network_name = azurerm_virtual_network.ai_vnet.name
  address_prefixes     = ["10.2.0.0/23"]

  private_endpoint_network_policies = "Disabled"

  service_endpoints = ["Microsoft.CognitiveServices"]
}

resource "azurerm_private_dns_zone" "cognitiveservices" {
  name                = "privatelink.openai.azure.com"
  resource_group_name = azurerm_resource_group.ai_resource_group.name
  tags                = var.tags
}

resource "azurerm_private_dns_zone_virtual_network_link" "cognitiveservices_ai" {
  name                  = "cognitiveservices-ai"
  resource_group_name   = azurerm_resource_group.ai_resource_group.name
  private_dns_zone_name = azurerm_private_dns_zone.cognitiveservices.name
  virtual_network_id    = azurerm_virtual_network.ai_vnet.id
  tags                  = var.tags
  registration_enabled  = true
}

resource "azurerm_private_dns_zone_virtual_network_link" "cognitiveservices_apps" {
  name                  = "cognitiveservices-apps"
  resource_group_name   = azurerm_resource_group.ai_resource_group.name
  private_dns_zone_name = azurerm_private_dns_zone.cognitiveservices.name
  virtual_network_id    = var.apps_vnet_id
  tags                  = var.tags
  registration_enabled  = true
}

resource "azurerm_cognitive_account" "openai" {
  name                  = "${var.cognitive_service_name}${var.environment}"
  location              = var.ai_location
  resource_group_name   = azurerm_resource_group.ai_resource_group.name
  kind                  = "OpenAI"
  sku_name              = "S0"
  custom_subdomain_name = "${var.cognitive_service_name}${var.environment}"

  network_acls {
    default_action = "Deny"
    ip_rules       = ["79.117.193.47"]
    virtual_network_rules {
      subnet_id = azurerm_subnet.ai_subnet.id
    }
  }

  tags = var.tags
}

resource "azurerm_private_endpoint" "openai" {
  name                = "quotifyaipe"
  location            = var.ai_location
  resource_group_name = azurerm_resource_group.ai_resource_group.name
  subnet_id           = azurerm_subnet.ai_subnet.id
  tags                = var.tags

  private_service_connection {
    name                           = "psc-openai"
    private_connection_resource_id = azurerm_cognitive_account.openai.id
    subresource_names              = ["account"]
    is_manual_connection           = false
  }

  private_dns_zone_group {
    name                 = "private-dns-zone-group"
    private_dns_zone_ids = [azurerm_private_dns_zone.cognitiveservices.id]
  }
}

resource "azurerm_virtual_network_peering" "apps_to_ai" {
  name                         = "apps-to-ai"
  resource_group_name          = var.apps_resource_group_name
  virtual_network_name         = var.apps_vnet_name
  remote_virtual_network_id    = azurerm_virtual_network.ai_vnet.id
  allow_forwarded_traffic      = true
  allow_virtual_network_access = true
}

resource "azurerm_virtual_network_peering" "ai_to_apps" {
  name                         = "ai-to-apps"
  resource_group_name          = azurerm_resource_group.ai_resource_group.name
  virtual_network_name         = azurerm_virtual_network.ai_vnet.name
  remote_virtual_network_id    = var.apps_vnet_id
  allow_forwarded_traffic      = true
  allow_virtual_network_access = true
}
