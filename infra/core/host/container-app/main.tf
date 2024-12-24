locals {
  container_image = coalesce(var.image_name, "mcr.microsoft.com/azuredocs/containerapps-helloworld:latest")
}

resource "azurerm_container_app" "this" {
  name                         = "${var.app_name}"
  container_app_environment_id = var.container_apps_environment_id
  resource_group_name          = var.resource_group_name
  revision_mode                = "Single"
  tags                         = merge(var.tags, { "azd-service-name": var.app_id })

  identity {
    type         = "SystemAssigned, UserAssigned"
    identity_ids = [var.user_identity_id]
  }

  registry {
    identity = var.user_identity_id
    server   = var.container_registry_url
  }

  template {
    container {
      name   = var.app_id
      image  = local.container_image
      cpu    = 0.25
      memory = "0.5Gi"
    }

    min_replicas = 0
    max_replicas = 2

  }

  ingress {
    allow_insecure_connections = false
    external_enabled           = true
    target_port                = 8080

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }
}