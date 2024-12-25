module "api" {
  source                        = "../../core/host/container-app"
  app_id                        = "api"
  app_name                      = var.api_name
  resource_group_name           = var.resource_group_name
  image_name                    = null
  container_apps_environment_id = var.container_apps_environment_id
  container_registry_url        = var.container_registry_url
  user_identity_id              = var.user_identity_id
  env                           = var.env
  tags                          = var.tags
}
