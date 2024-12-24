resource "azurerm_virtual_network" "vnet" {
  name                = "${var.vnet_name}${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  address_space       = var.address_space
  tags                = var.tags
}

resource "azurerm_subnet" "app_subnet" {
  name                 = "${var.subnet_name}${var.environment}"
  virtual_network_name = azurerm_virtual_network.vnet.name
  resource_group_name  = var.resource_group_name
  address_prefixes     = var.subnet_address_prefix_map["app"]
}
