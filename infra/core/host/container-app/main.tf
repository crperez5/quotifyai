data "azurerm_container_app_environment" "this" {
  name                = "${var.container_apps_environment_name}${var.environment}"
  resource_group_name = var.resource_group_name
}

locals {
  container_image = coalesce(var.image_name, "mcr.microsoft.com/azuredocs/containerapps-helloworld:latest")
}

resource "azurerm_container_app" "this" {
  name                         = var.app_name
  container_app_environment_id = data.azurerm_container_app_environment.this.id
  resource_group_name          = var.resource_group_name
  revision_mode                = "Single"
  tags                         = var.app_id != null ? merge(var.tags, { "azd-service-name" : var.app_id }) : var.tags


  dynamic "identity" {
    for_each = var.user_identity_id != null ? [1] : []
    content {
      type         = "SystemAssigned, UserAssigned"
      identity_ids = [var.user_identity_id]
    }
  }

  dynamic "registry" {
    for_each = var.user_identity_id != null ? [1] : []
    content {
      identity = var.user_identity_id
      server   = var.container_registry_url
    }
  }

  template {
    container {
      name   = var.app_id
      image  = local.container_image
      cpu    = 0.25
      memory = "0.5Gi"

      dynamic "env" {
        for_each = var.env
        content {
          name  = env.value.name
          value = env.value.value
        }
      }

      dynamic "volume_mounts" {
        for_each = var.volume_mount_name != null ? [1] : []
        content {
          name = var.volume_mount_name
          path = var.volume_mount_path
        }
      }

      dynamic "readiness_probe" {
        for_each = var.readiness_probe != null ? [1] : []

        content {
          port      = var.readiness_probe.port
          transport = var.readiness_probe.transport
          path      = var.readiness_probe.path
        }
      }

      dynamic "liveness_probe" {
        for_each = var.liveness_probe != null ? [1] : []

        content {
          port      = var.liveness_probe.port
          transport = var.liveness_probe.transport
          path      = var.liveness_probe.path
        }
      }

    }

    dynamic "volume" {
      for_each = var.file_share_name != null ? [1] : []
      content {
        name         = var.volume_mount_name
        storage_type = "AzureFile"
        storage_name = var.file_share_name
      }
    }

    min_replicas = 0
    max_replicas = 1

  }

  ingress {
    allow_insecure_connections = var.ingress_allow_insecure_connections
    external_enabled           = var.ingress_external_enabled
    target_port                = var.ingress_target_port
    transport                  = var.ingress_transport

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }
}
