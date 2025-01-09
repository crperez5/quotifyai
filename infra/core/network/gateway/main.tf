# gateway module

resource "azurerm_subnet" "appgw_subnet" {
  name                 = "appgw-subnet"
  resource_group_name  = var.resource_group_name
  virtual_network_name = var.vnet_name
  address_prefixes     = ["10.0.4.0/23"]
}

resource "azurerm_network_security_group" "appgw_nsg" {
  name                = "appgw-nsg"
  location            = var.location
  resource_group_name = var.resource_group_name

  security_rule {
    name                       = "allow-gateway-manager"
    priority                   = 100
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "65200-65535"
    source_address_prefix      = "GatewayManager"
    destination_address_prefix = "*"
  }

  security_rule {
    name                       = "allow-http"
    priority                   = 110
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "80"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }

  security_rule {
    name                       = "allow-https"
    priority                   = 120
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "443"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }

  tags = var.tags
}

resource "azurerm_subnet_network_security_group_association" "appgw_nsg_association" {
  subnet_id                 = azurerm_subnet.appgw_subnet.id
  network_security_group_id = azurerm_network_security_group.appgw_nsg.id
}

resource "azurerm_public_ip" "appgw_public_ip" {
  name                = "appgw-public-ip"
  resource_group_name = var.resource_group_name
  location            = var.location
  allocation_method   = "Static"
  sku                 = "Standard"
  domain_name_label   = lower("${var.gateway_name}${var.environment}")
  tags                = var.tags
}

resource "azurerm_application_gateway" "this" {
  name                = "${var.gateway_name}${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = var.tags

  sku {
    name     = "Standard_v2"
    tier     = "Standard_v2"
    capacity = 1
  }

  gateway_ip_configuration {
    name      = "gateway-ip-configuration"
    subnet_id = azurerm_subnet.appgw_subnet.id
  }

  frontend_port {
    name = "http-port"
    port = 80
  }

  # frontend_port {
  #   name = "https-port"
  #   port = 443
  # }

  frontend_ip_configuration {
    name                 = "frontend-ip-configuration"
    public_ip_address_id = azurerm_public_ip.appgw_public_ip.id
  }

  backend_address_pool {
    name  = "container-app-pool"
    fqdns = [replace(var.backend_fqdn, "https://", "")]
  }

  probe {
    name                = "api"
    protocol            = "Http"
    path                = "/health"
    host                = replace(var.backend_fqdn, "https://", "")
    interval            = 30
    timeout             = 30
    unhealthy_threshold = 3
    port                = 80

    match {
      status_code = ["200-399"]
    }
  }

  # backend_http_settings {
  #   name                                = "https-settings"
  #   cookie_based_affinity               = "Disabled"
  #   port                                = 443
  #   protocol                            = "Https"
  #   request_timeout                     = 60
  #   pick_host_name_from_backend_address = true
  # }

  backend_http_settings {
    name                                = "http-settings"
    cookie_based_affinity               = "Disabled"
    port                                = 80
    protocol                            = "Http"
    request_timeout                     = 60
    pick_host_name_from_backend_address = true
    probe_name                          = "api" 
  }

  http_listener {
    name                           = "http-listener"
    frontend_ip_configuration_name = "frontend-ip-configuration"
    frontend_port_name             = "http-port"
    protocol                       = "Http"
  }

  # http_listener {
  #   name                           = "https-listener"
  #   frontend_ip_configuration_name = "frontend-ip-configuration"
  #   frontend_port_name             = "https-port"
  #   protocol                       = "Https"
  #   ssl_certificate_name           = var.ssl_certificate_name
  # }

  # request_routing_rule {
  #   name                        = "redirecting-rule"
  #   priority                    = 100
  #   rule_type                   = "Basic"
  #   http_listener_name          = "http-listener"
  #   redirect_configuration_name = "redirect-configuration"
  # }

  # request_routing_rule {
  #   name                       = "routing-rule"
  #   priority                   = 110
  #   rule_type                  = "Basic"
  #   http_listener_name         = "https-listener"
  #   backend_address_pool_name  = "container-app-pool"
  #   backend_http_settings_name = "https-settings"
  # }

  request_routing_rule {
    name                       = "routing-rule"
    priority                   = 110
    rule_type                  = "Basic"
    http_listener_name         = "http-listener"
    backend_address_pool_name  = "container-app-pool"
    backend_http_settings_name = "http-settings"
  }

  # redirect_configuration {
  #   name                 = "redirect-configuration"
  #   redirect_type        = "Permanent"
  #   target_listener_name = "https-listener"
  #   include_path         = true
  #   include_query_string = true
  # }


  identity {
    type         = "UserAssigned"
    identity_ids = [var.gateway_user_assigned_id]
  }

  # ssl_certificate {
  #   name                = var.ssl_certificate_name
  #   key_vault_secret_id = var.ssl_certificate_secret_id
  # }

  # depends_on = [var.gateway_keyvault_access_policy_id]
}
