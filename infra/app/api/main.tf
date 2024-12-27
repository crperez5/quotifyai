module "api" {
  source                          = "../../core/host/container-app"
  environment                     = var.environment
  container_apps_environment_name = var.container_apps_environment_name
  app_id                          = "api"
  app_name                        = var.api_name
  resource_group_name             = var.resource_group_name
  image_name                      = null
  container_registry_url          = var.container_registry_url
  user_identity_id                = var.user_identity_id
  env                             = var.env
  liveness_probe                  = var.liveness_probe
  tags                            = var.tags
}
