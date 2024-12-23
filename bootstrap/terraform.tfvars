azure_subscription_id        = "b01be5c4-2593-4dba-9bea-c484e74ec324"
azure_tenant_id              = "4d88b5e5-2d7e-44fd-a348-af5ee06e099a"
azure_roles                  = [
    "Contributor", 
    "User Access Administrator", 
    "Storage Blob Data Reader", 
    "Storage Blob Data Contributor"]
azure_container_registry_url = "quotifyaicr.azurecr.io"
azure_resource_group         = "quotifyairgdev"
branches                     = ["main"]
environments                 = ["dev"]
tags                         = ["latest"]
pull_request                 = true
github_organization_name     = "crperez5"
github_repository_name       = "quotifyai"
rs_container_name            = "tfstate"
rs_resource_group            = "tfstate"
rs_storage_account           = "tfstatequotifyai"
