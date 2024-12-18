data "azurerm_subscription" "sub" {
}

module "resource_group" {
  source = "./modules/resource_group"

  resource_group_name = var.resource_group_name
  location            = var.location
}

module "deployment_user_identity" {
  source   = "./modules/user_identity"
  name     = "${var.deployment_user_name}"
  location = var.location
  rg_name  = module.resource_group.resource_group_name
  tags     = var.tags
}

module "deployment_role_assignment" {
  source       = "./modules/role-assignment"
  principal_id = module.deployment_user_identity.user_assinged_identity_principal_id
  role_name    = var.owner_role_name
  scope_id     = data.azurerm_subscription.sub.id
}

module "gh_federated_credential" {
  source                             = "./modules/federated-identity-credential"
  federated_identity_credential_name = "${var.github_organization_target}-${var.github_repository}-${var.environment}"
  rg_name                            = module.resource_group.resource_group_name
  user_assigned_identity_id          = module.deployment_user_identity.user_assinged_identity_id
  subject                            = "repo:${var.github_organization_target}/${var.github_repository}:ref:refs/heads/main"
  audience_name                      = local.default_audience_name
  issuer_url                         = local.github_issuer_url
}

# module "tf-state-storage" {
#   source                   = "./modules/storage"
#   storage_account_name     = var.storage_account_name
#   resource_group_name      = module.resource_group.resource_group_name
#   location                 = var.location
#   tags                     = var.tags
#   account_replication_type = var.account_replication_type
#   account_tier             = var.account_tier
#   container_name           = var.container_name
#   automatic_container_name = var.automatic_container_name
# }


# module "tfstate_role_assignment" {
#   source       = "./modules/role-assignment"
#   principal_id = module.deployment_user_identity.user_assinged_identity_principal_id
#   role_name    = "Storage Blob Data Contributor"
#   scope_id     = module.tf-state-storage.id
# }


